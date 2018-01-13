using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System.Collections.Generic;
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
                // TODO V2 Manage large lists
                CamlQuery foldersQuery = new CamlQuery()
                {
                    ViewXml = "<View><Query><Where><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>1</Value></Eq></Where></Query></View>"
                };
                
                ListItemCollection folders = Element.GetItems(foldersQuery);
                Context.Load(folders,
                    coll => coll.Include(
                        f => f.Folder));
                Context.ExecuteQuery();

                List<FolderRunner> folderRunners = new List<FolderRunner>();
                foreach (ListItem folder in folders)
                {
                    folderRunners.Add(new FolderRunner(Manager, Context, folder.Folder));
                }

                folderRunners.ForEach(r => r.Process());
            }
            else if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.ListItem)))
            {
                // TODO V2 Manage large lists
                CamlQuery itemsQuery = new CamlQuery()
                {
                    ViewXml = "<View Scope='RecursiveAll'><Query></Query></View>"
                };

                ListItemCollection items = Element.GetItems(itemsQuery);
                Context.Load(items);
                Context.ExecuteQuery();

                List<ListItemRunner> itemRunners = new List<ListItemRunner>();
                foreach (ListItem item in items)
                {
                    itemRunners.Add(new ListItemRunner(Manager, Context, item));
                }

                itemRunners.ForEach(r => r.Process());
            }

            // OnListRunningEnd
            ActiveReceivers.ForEach(r => r.OnListRunningEnd(Element));
        }
    }
}
