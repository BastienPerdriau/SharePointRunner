using log4net;
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
        /// Logger
        /// </summary>
        internal static readonly ILog Logger = LogManager.GetLogger(typeof(RunningManager).Namespace);

        /// <summary>
        /// SharePoint Credentials
        /// </summary>
        public SharePointOnlineCredentials Credentials { get; set; }

        /// <summary>
        /// List of receivers
        /// </summary>
        public List<Receiver> Receivers { get; } = new List<Receiver>();

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
            if (Receivers.Count == 0)
            {
                Exception ex = new Exception($"No receiver declared");
                Logger.Warn(ex.Message, ex);
                return;
            }

            Logger.Info("RunningManager initialized");
            List<Runner> runners;
            switch (StartingRunningLevel.BaseRunningLevel)
            {
                case BaseRunningLevel.Tenant:
                    // If Tenant, must be only one url
                    runners = new List<Runner>() { GetTenantRunner(Urls.FirstOrDefault()) };
                    break;
                case BaseRunningLevel.SiteCollection:
                    runners = new List<Runner>(GetSiteCollectionRunners(Urls));
                    break;
                case BaseRunningLevel.Site:
                    runners = new List<Runner>(GetSiteRunners(Urls));
                    break;
                case BaseRunningLevel.List:
                    runners = new List<Runner>(GetListRunners(Urls));
                    break;
                default:
                    throw new Exception($"Run cannot start at '{StartingRunningLevel.ToString()}' level");
            }

            Logger.Info("RunningManager initialized");
            Logger.Info($"Receivers count: {Receivers.Count}");
            Logger.Info($"StartingRunningLevel: {StartingRunningLevel}");
            Logger.Info($"Runners count: {runners.Count} for URLs: '{string.Join(", ", Urls)}'");

            // OnStart
            Logger.Debug("RunningManager OnStart()");
            Receivers.ForEach(r => r.OnStart());

            // Launch runners
            runners.ForEach(r => r.Process());

            // OnEnd
            Logger.Debug("RunningManager OnEnd()");
            Receivers.ForEach(r => r.OnEnd());

            Logger.Info("RunningManager finished");
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
        /// <param name="siteCollectionUrls">Site collections URLs</param>
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
                    webUrl = Regex.Match(listUrl, @"(.*)\/Lists\/.*", RegexOptions.IgnoreCase).Groups[1].Value;
                }
                else
                {
                    webUrl = Regex.Match(listUrl, @"(.*)\/.*", RegexOptions.IgnoreCase).Groups[1].Value;
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
        /// <param name="url">SharePoint site URL</param>
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
