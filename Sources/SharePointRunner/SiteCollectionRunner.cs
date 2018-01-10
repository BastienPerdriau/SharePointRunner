using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System.Linq;

namespace SharePointRunner
{
    internal class SiteCollectionRunner : Runner<Site>
    {
        /// <summary>
        /// Running level
        /// </summary>
        public override RunningLevel RunningLevel => RunningLevel.SiteCollection;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="site">Site collection</param>
        public SiteCollectionRunner(RunningManager runningManager, ClientContext context, Site site) : base(runningManager, context, site) { }

        /// <summary>
        /// Action for this SharePoint site collection
        /// </summary>
        public override void Process()
        {
            Context.Load(Element,
                s => s.RootWeb);
            Context.ExecuteQuery();

            // OnSiteCollectionRunningStart
            ActiveReceivers.ForEach(r => r.OnSiteCollectionRunningStart(Element, Element.RootWeb));

            // If at least one receiver run sites or deeper
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.Site)))
            {
                // Run site on current root site
                SiteRunner siteRunner = new SiteRunner(Manager, Context, Element.RootWeb);
                siteRunner.Process();
            }

            // OnSiteCollectionRunningEnd
            ActiveReceivers.ForEach(r => r.OnSiteCollectionRunningEnd(Element, Element.RootWeb));
        }
    }
}
