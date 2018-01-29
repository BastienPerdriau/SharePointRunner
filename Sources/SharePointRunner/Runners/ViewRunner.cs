using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;

namespace SharePointRunner
{

    internal class ViewRunner : Runner<View>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="view">View</param>
        public ViewRunner(RunningManager runningManager, ClientContext context, View view) : base(runningManager, context, view, RunningLevel.View) { }

        /// <summary>
        /// Action for this SharePoint view
        /// </summary>
        public override void Process()
        {
            RunningManager.Logger.Debug($"ViewRunner Process() - {ActiveReceivers.Count} active receivers");
            Context.Load(Element,
                v => v.Title,
                v => v.ServerRelativeUrl);
            Context.ExecuteQuery();
            RunningManager.Logger.Debug($"View | Title: {Element.Title} / URL: {Element.ServerRelativeUrl}");

            // OnViewRunning
            RunningManager.Logger.Debug("ViewRunner OnViewRunning()");
            ActiveReceivers.ForEach(r => r.OnViewRunning(Element));
        }
    }
}
