using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using SharePointRunner.SDK;
using System.Collections.Generic;
using System.Linq;

namespace SharePointRunner
{
    internal class TermSetRunner : Runner<TermSet>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="termSet">Term set</param>
        public TermSetRunner(RunningManager runningManager, ClientContext context, TermSet termSet) : base(runningManager, context, termSet, RunningLevel.TermSet) { }

        /// <summary>
        /// Action for this SharePoint term set
        /// </summary>
        public override void Process()
        {
            RunningManager.Logger.Debug($"TermSetRunner Process() - {ActiveReceivers.Count} active receivers");
            Context.Load(Element,
                s => s.Name);
            Context.ExecuteQuery();
            RunningManager.Logger.Debug($"TermSet | Name: {Element.Name}");

            // OnTermSetRunningStart
            RunningManager.Logger.Debug("TermSetRunner OnTermSetRunningStart()");
            ActiveReceivers.ForEach(r => r.OnTermSetRunningStart(Element));

            // If at least one receiver run terms or deeper
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.Term)))
            {
                // Crawl terms
                Context.Load(Element.Terms);
                Context.ExecuteQuery();

                List<TermRunner> termRunners = new List<TermRunner>();
                foreach (Term term in Element.Terms)
                {
                    termRunners.Add(new TermRunner(Manager, Context, term));
                }

                termRunners.ForEach(r => r.Process());
            }

            // OnTermSetRunningEnd
            RunningManager.Logger.Debug("TermSetRunner OnTermSetRunningEnd()");
            ActiveReceivers.ForEach(r => r.OnTermSetRunningEnd(Element));
        }
    }
}
