using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharePointRunner
{
    /// <summary>
    /// Controller class to run SharePoint sites
    /// </summary>
    public class RunningManager
    {
        /// <summary>
        /// List of receivers
        /// </summary>
        public List<Receiver> Receivers { get; } = new List<Receiver>();

        /// <summary>
        /// SharePoint Credentials
        /// </summary>
        public SharePointOnlineCredentials Credentials { get; set; }

        /// <summary>
        /// List of URLs
        /// </summary>
        public List<string> Urls { get; } = new List<string>();

        /// <summary>
        /// Starting running level
        /// </summary>
        public RunningLevel StartingRunningLevel { get; set; } = RunningLevel.Tenant;

        /// <summary>
        /// Start a run
        /// </summary>
        public void Run()
        {
            List<Runner> runners;
            switch (StartingRunningLevel)
            {
                case RunningLevel.Tenant:
                    // If Tenant, must be only one url
                    runners = new List<Runner>() { GetTenantRunner(Urls.FirstOrDefault()) };
                    break;
                case RunningLevel.SiteCollection:
                    runners = new List<Runner>(GetSiteCollectionRunners(Urls));
                    break;
                case RunningLevel.Site:
                    runners = new List<Runner>(GetSiteRunners(Urls));
                    break;
                case RunningLevel.List:
                    runners = new List<Runner>(GetListRunners(Urls));
                    break;
                default:
                    throw new Exception($"Run cannot start at '{StartingRunningLevel.ToString()}' level");
            }

            // OnStart
            Receivers.ForEach(r => r.OnStart());

            // Launch runners
            runners.ForEach(a => a.Process());

            // OnEnd
            Receivers.ForEach(r => r.OnEnd());
        }

        /// <summary>
        /// Get runner for a tenant
        /// </summary>
        /// <param name="tenantUrl">Tenant URL</param>
        private TenantRunner GetTenantRunner(string tenantUrl)
        {
            ClientContext tenantCtx = OpenClientContext(tenantUrl);
            Tenant tenant = new Tenant(tenantCtx);

            return new TenantRunner(this, tenantCtx, tenant);
        }

        /// <summary>
        /// Get runners of site collections
        /// </summary>
        /// <param name="siteCollectionUrls">Sites collections URLs</param>
        private List<SiteCollectionRunner> GetSiteCollectionRunners(List<string> siteCollectionUrls)
        {
            List<SiteCollectionRunner> runners = new List<SiteCollectionRunner>();

            foreach (string siteCollectionUrl in siteCollectionUrls)
            {
                ClientContext ctx = OpenClientContext(siteCollectionUrl);
                runners.Add(new SiteCollectionRunner(this, ctx, ctx.Site));
            }

            return runners;
        }

        /// <summary>
        /// Get runners of sites
        /// </summary>
        /// <param name="siteUrls">Sites SURLs</param>
        private List<SiteRunner> GetSiteRunners(List<string> siteUrls)
        {
            List<SiteRunner> runners = new List<SiteRunner>();

            foreach (string siteUrl in siteUrls)
            {
                ClientContext ctx = OpenClientContext(siteUrl);
                runners.Add(new SiteRunner(this, ctx, ctx.Web));
            }

            return runners;
        }

        /// <summary>
        /// Get runners of lists
        /// </summary>
        /// <param name="listUrls">Lists URLs</param>
        private List<ListRunner> GetListRunners(List<string> listUrls)
        {
            List<ListRunner> runners = new List<ListRunner>();

            // Open each web then get list
            foreach (string listUrl in listUrls)
            {
                string listServerRelativeUrl = Regex.Match(listUrl, @"(?:http|https):\/\/[^\/]*(.*)", RegexOptions.IgnoreCase).Groups[1].Value;

                // Get web url from listUrls
                // TODO V2 Do with only one regex
                string webUrl = string.Empty;
                if (Regex.IsMatch(listUrl, @"\/Lists\/", RegexOptions.IgnoreCase))
                {
                    webUrl = Regex.Match(webUrl, @"(.*)\/Lists\/.*", RegexOptions.IgnoreCase).Groups[1].Value;
                }
                else
                {
                    webUrl = Regex.Match(webUrl, @"(.*)\/.*", RegexOptions.IgnoreCase).Groups[1].Value;
                }

                ClientContext ctx = OpenClientContext(webUrl);
                Web web = ctx.Web;

                ctx.Load(web,
                    w => w.Lists.Include(
                        l => l.RootFolder.ServerRelativeUrl));
                ctx.ExecuteQuery();

                List list = web.Lists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals(listServerRelativeUrl, StringComparison.InvariantCultureIgnoreCase));

                if (list != null)
                {
                    runners.Add(new ListRunner(this, ctx, list));
                }
            }

            return runners;
        }

        #region Utils
        /// <summary>
        /// Open a SharePoint context
        /// </summary>
        /// <param name="url">Sharepoint site URL</param>
        /// <returns>SharePoint context</returns>
        private ClientContext OpenClientContext(string url)
        {
            ClientContext ctx = new ClientContext(url);

            if (Credentials != null)
            {
                ctx.Credentials = Credentials;
            }

            return ctx;
        }
        #endregion
    }
}
