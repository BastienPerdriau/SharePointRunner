using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using SharePointRunner.SDK;
using System.Collections.Generic;
using System.Linq;

namespace SharePointRunner
{
    internal class TermGroupRunner : Runner<TermGroup>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="termGroup">Term group</param>
        public TermGroupRunner(RunningManager runningManager, ClientContext context, TermGroup termGroup) : base(runningManager, context, termGroup, RunningLevel.TermGroup) { }

        /// <summary>
        /// Action for this SharePoint term group
        /// </summary>
        public override void Process()
        {
            RunningManager.Logger.Debug($"TermGroupRunner Process() - {ActiveReceivers.Count} active receivers");
            Context.Load(Element,
                g => g.Name);
            Context.ExecuteQuery();
            RunningManager.Logger.Debug($"TermGroup | Name: {Element.Name}");

            // OnTermGroupRunningStart
            RunningManager.Logger.Debug("TermGroupRunner OnTermGroupRunningStart()");
            ActiveReceivers.ForEach(r => r.OnTermGroupRunningStart(Element));

            // If at least one receiver run term sets or deeper
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.TermSet)))
            {
                // Crawl term sets
                Context.Load(Element.TermSets);
                Context.ExecuteQuery();

                List<TermSetRunner> termSetRunners = new List<TermSetRunner>();
                foreach (TermSet set in Element.TermSets)
                {
                    termSetRunners.Add(new TermSetRunner(Manager, Context, set));
                }

                termSetRunners.ForEach(r => r.Process());
            }

            // OnTermGroupRunningEnd
            RunningManager.Logger.Debug("TermGroupRunner OnTermGroupRunningEnd()");
            ActiveReceivers.ForEach(r => r.OnTermGroupRunningEnd(Element));
        }
    }
}
