using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;

namespace SharePointRunner
{
    internal class GroupRunner : Runner<Group>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="group">Group</param>
        public GroupRunner(RunningManager runningManager, ClientContext context, Group group) : base(runningManager, context, group, RunningLevel.Group) { }

        /// <summary>
        /// Action for this SharePoint group
        /// </summary>
        public override void Process()
        {
            RunningManager.Logger.Debug($"GroupRunner Process() - {ActiveReceivers.Count} active receivers");
            Context.Load(Element,
                f => f.Id,
                f => f.Title);
            Context.ExecuteQuery();
            RunningManager.Logger.Debug($"Group | Id: {Element.Id} / Title: {Element.Title}");

            // OnGroupRunning
            RunningManager.Logger.Debug("GroupRunner OnGroupRunning()");
            ActiveReceivers.ForEach(r => r.OnGroupRunning(Element));
        }
    }
}
