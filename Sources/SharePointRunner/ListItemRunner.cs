using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System.Linq;

namespace SharePointRunner
{
    internal class ListItemRunner : Runner<ListItem>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="listItem">List item</param>
        public ListItemRunner(RunningManager runningManager, ClientContext context, ListItem listItem) : base(runningManager, context, listItem, RunningLevel.ListItem) { }

        /// <summary>
        /// Action for this SharePoint list item
        /// </summary>
        public override void Process()
        {
            RunningManager.Logger.Debug("ListItemRunner Process()");
            Context.Load(Element,
                li => li.DisplayName);
            Context.ExecuteQuery();
            RunningManager.Logger.Debug($"List item Display Name: {Element.DisplayName}");

            // OnListItemRunning
            RunningManager.Logger.Debug("ListItemRunner OnListItemRunning()");
            ActiveReceivers.ForEach(r => r.OnListItemRunning(Element));

            // If at least one receiver run files
            // TODO ERROR
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.File)))
            {
                Context.Load(Element,
                    li => li.File.Exists,
                    li => li.File.ServerRelativeUrl);
                Context.ExecuteQuery();

                // If there is a file
                // TODO ERROR
                if (Element.File.Exists)
                {
                    // Run file on current list item
                    FileRunner fileRunner = new FileRunner(Manager, Context, Element.File);
                    fileRunner.Process();
                }
            }
        }
    }
}
