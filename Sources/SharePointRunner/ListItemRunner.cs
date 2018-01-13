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
        public ListItemRunner(RunningManager runningManager, ClientContext context, ListItem listItem) : base(runningManager, context, listItem, RunningLevelEnum.ListItem) { }

        /// <summary>
        /// Action for this SharePoint list item
        /// </summary>
        public override void Process()
        {
            Context.Load(Element);
            Context.ExecuteQuery();

            // OnListItemRunning
            ActiveReceivers.ForEach(r => r.OnListItemRunning(Element));

            // If at least one receiver run files
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevelEnum.File)))
            {
                Context.Load(Element,
                    li => li.File);
                Context.ExecuteQuery();

                // If there is a file
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
