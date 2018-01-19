using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System.Collections.Generic;
using System.Linq;

namespace SharePointRunner
{
    internal class SiteRunner : Runner<Web>
    {
        /// <summary>
        /// List of active receivers for this runner
        /// </summary>
        protected override List<Receiver> ActiveReceivers
        {
            get
            {
                if (IsSubSite)
                {
                    return base.ActiveReceivers.Where(r => r.IncludeSubSites).ToList();
                }
                else
                {
                    return base.ActiveReceivers;
                }
            }
        }

        /// <summary>
        /// True if the site is a sub site, False if not
        /// </summary>
        public virtual bool IsSubSite { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="web">Site</param>
        /// <param name="isSubSite">True if the site is a sub site, False if not</param>
        public SiteRunner(RunningManager runningManager, ClientContext context, Web web, bool isSubSite = false) : base(runningManager, context, web, RunningLevel.Site)
        {
            IsSubSite = isSubSite;
        }

        /// <summary>
        /// Action for this SharePoint site
        /// </summary>
        public override void Process()
        {
            RunningManager.Logger.Debug($"SiteRunner Process() - {ActiveReceivers.Count} active receivers");
            Context.Load(Element,
                s => s.Title,
                s => s.Url);
            Context.ExecuteQuery();
            RunningManager.Logger.Debug($"Site | Title: {Element.Title} / URL: {Element.Url}");

            // OnSiteRunningStart
            RunningManager.Logger.Debug("SiteRunner OnSiteRunningStart()");
            ActiveReceivers.ForEach(r => r.OnSiteRunningStart(Element));

            // If at least one receiver run lists or deeper
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.List)))
            {
                // Crawl Lists
                Context.Load(Element.Lists,
                    coll => coll.Include(
                        l => l.Hidden));
                Context.ExecuteQuery();

                List<ListRunner> listRunners = new List<ListRunner>();

                IEnumerable<List> lists;
                if (Manager.Receivers.Any(r => r.IncludeHiddenLists))
                {
                    lists = Element.Lists;
                }
                else
                {
                    lists = Element.Lists.Where(l => !l.Hidden);
                }

                foreach (List list in lists)
                {
                    listRunners.Add(new ListRunner(Manager, Context, list));
                }

                listRunners.ForEach(r => r.Process());
            }

            // OnSiteRunningEnd
            RunningManager.Logger.Debug("SiteRunner OnSiteRunningEnd()");
            ActiveReceivers.ForEach(r => r.OnSiteRunningEnd(Element));

            // If at least one receiver run subsites
            if (Manager.Receivers.Any(r => r.IncludeSubSites))
            {
                // Crawl Subsites
                Context.Load(Element.Webs);
                Context.ExecuteQuery();

                List<SiteRunner> siteRunners = new List<SiteRunner>();
                foreach (Web subWeb in Element.Webs)
                {
                    siteRunners.Add(new SiteRunner(Manager, Context, subWeb, true));
                }

                siteRunners.ForEach(r => r.Process());

                // OnSiteRunningEndAfterSubSites
                RunningManager.Logger.Debug("SiteRunner OnSiteRunningEndAfterSubSites()");
                ActiveReceivers.ForEach(r => r.OnSiteRunningEndAfterSubSites(Element));
            }
        }
    }
}
