using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;

namespace SharePointRunner
{
    internal class ListItemRunner : Runner<ListItem>
    {
        /// <summary>
        /// Running level
        /// </summary>
        public override RunningLevel RunningLevel => RunningLevel.ListItem;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="listItem">List item</param>
        public ListItemRunner(RunningManager runningManager, ClientContext context, ListItem listItem) : base(runningManager, context, listItem) { }

        /// <summary>
        /// Action for this SharePoint list item
        /// </summary>
        public override void Process()
        {
            // TODO V1 ListItem Process
        }
    }
}
