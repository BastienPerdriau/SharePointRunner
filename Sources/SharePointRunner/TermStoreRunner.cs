using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using SharePointRunner.SDK;
using System.Collections.Generic;
using System.Linq;

namespace SharePointRunner
{
    internal class TermStoreRunner : Runner<TermStore>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="termStore">Term store</param>
        public TermStoreRunner(RunningManager runningManager, ClientContext context, TermStore termStore) : base(runningManager, context, termStore, RunningLevel.TermStore) { }

        /// <summary>
        /// Action for this SharePoint term store
        /// </summary>
        public override void Process()
        {
            RunningManager.Logger.Debug($"TermStoreRunner Process() - {ActiveReceivers.Count} active receivers");
            Context.Load(Element,
                s => s.Name);
            Context.ExecuteQuery();
            RunningManager.Logger.Debug($"TermStore | Name: {Element.Name}");

            // OnTermStoreRunningStart
            RunningManager.Logger.Debug("TermStoreRunner OnTermStoreRunningStart()");
            ActiveReceivers.ForEach(r => r.OnTermStoreRunningStart(Element));

            // If at least one receiver run term groups or deeper
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.TermGroup)))
            {
                // Crawl term groups
                Context.Load(Element.Groups);
                Context.ExecuteQuery();

                List<TermGroupRunner> termGroupRunners = new List<TermGroupRunner>();
                foreach (TermGroup group in Element.Groups)
                {
                    termGroupRunners.Add(new TermGroupRunner(Manager, Context, group));
                }

                termGroupRunners.ForEach(r => r.Process());
            }

            // OnTermStoreRunningEnd
            RunningManager.Logger.Debug("TermStoreRunner OnTermStoreRunningEnd()");
            ActiveReceivers.ForEach(r => r.OnTermStoreRunningEnd(Element));
        }
    }
}
