using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using SharePointRunner.SDK;
using System.Collections.Generic;

namespace SharePointRunner
{
    internal class TermRunner : Runner<Term>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="term">Term</param>
        public TermRunner(RunningManager runningManager, ClientContext context, Term term) : base(runningManager, context, term, RunningLevel.Term) { }

        /// <summary>
        /// Action for this SharePoint term
        /// </summary>
        public override void Process()
        {
            RunningManager.Logger.Debug($"TermRunner Process() - {ActiveReceivers.Count} active receivers");
            Context.Load(Element,
                t => t.Name);
            Context.ExecuteQuery();
            RunningManager.Logger.Debug($"Term | Name: {Element.Name}");

            // OnTermRunningStart
            RunningManager.Logger.Debug("TermRunner OnTermRunningStart()");
            ActiveReceivers.ForEach(r => r.OnTermRunningStart(Element));

            // Crawl sub terms
            Context.Load(Element.Terms);
            Context.ExecuteQuery();

            List<TermRunner> termRunners = new List<TermRunner>();
            foreach (Term term in Element.Terms)
            {
                termRunners.Add(new TermRunner(Manager, Context, term));
            }

            termRunners.ForEach(r => r.Process());

            // OnTermRunningEnd
            RunningManager.Logger.Debug("TermRunner OnTermRunningEnd()");
            ActiveReceivers.ForEach(r => r.OnTermRunningEnd(Element));
        }
    }
}
