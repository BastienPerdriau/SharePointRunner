using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System.Linq;

namespace SharePointRunner
{
    internal class ListRunner : Runner<List>
    {
        /// <summary>
        /// Running level
        /// </summary>
        public override RunningLevel RunningLevel => RunningLevel.List;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="list">List</param>
        public ListRunner(RunningManager runningManager, ClientContext context, List list) : base(runningManager, context, list) { }

        /// <summary>
        /// Action for this SharePoint list
        /// </summary>

        public override void Process()
        {
            Context.Load(Element);
            Context.ExecuteQuery();

            // OnListRunningStart
            ActiveReceivers.ForEach(r => r.OnListRunningStart(Element));

            // If at least one receiver run folders or deeper
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.Folder)))
            {
                // TODO V2 Folders in lists
            }

            // TODO V2 ListItems & files with or without folders

            // OnListRunningEnd
            ActiveReceivers.ForEach(r => r.OnListRunningEnd(Element));
        }
    }
}
