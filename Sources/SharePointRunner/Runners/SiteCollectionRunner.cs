using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System.Collections.Generic;
using System.Linq;

namespace SharePointRunner
{
    internal class SiteCollectionRunner : Runner<Site>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="site">Site collection</param>
        public SiteCollectionRunner(RunningManager runningManager, ClientContext context, Site site) : base(runningManager, context, site, RunningLevel.SiteCollection) { }

        /// <summary>
        /// Action for this SharePoint site collection
        /// </summary>
        public override void Process()
        {
            RunningManager.Logger.Debug($"SiteCollectionRunner Process() - {ActiveReceivers.Count} active receivers");
            Context.Load(Element,
                s => s.Url,
                s => s.RootWeb);
            Context.ExecuteQuery();
            RunningManager.Logger.Debug($"SiteCollection | URL: {Element.Url}");

            // OnSiteCollectionRunningStart
            RunningManager.Logger.Debug("SiteCollectionRunner OnSiteCollectionRunningStart()");
            ActiveReceivers.ForEach(r => r.OnSiteCollectionRunningStart(Element, Element.RootWeb));

            // If at least one receiver run groups
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.Group)))
            {
                // Crawl groups
                Context.Load(Element.RootWeb.SiteGroups);
                Context.ExecuteQuery();

                List<GroupRunner> groupRunners = new List<GroupRunner>();

                foreach (Group group in Element.RootWeb.SiteGroups)
                {
                    groupRunners.Add(new GroupRunner(Manager, Context, group));
                }

                groupRunners.ForEach(r => r.Process());
            }

            // If at least one receiver run sites or deeper
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.Site)))
            {
                // Run site on current root site
                SiteRunner siteRunner = new SiteRunner(Manager, Context, Element.RootWeb);
                siteRunner.Process();
            }

            // OnSiteCollectionRunningEnd
            RunningManager.Logger.Debug("SiteCollectionRunner OnSiteCollectionRunningEnd()");
            ActiveReceivers.ForEach(r => r.OnSiteCollectionRunningEnd(Element, Element.RootWeb));
        }
    }
}
