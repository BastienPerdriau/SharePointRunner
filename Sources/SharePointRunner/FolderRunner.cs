using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System.Collections.Generic;
using System.Linq;

namespace SharePointRunner
{
    internal class FolderRunner : Runner<Folder>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="folder">Folder</param>
        public FolderRunner(RunningManager runningManager, ClientContext context, Folder folder) : base(runningManager, context, folder, RunningLevel.Folder) { }

        /// <summary>
        /// Action for this SharePoint folder
        /// </summary>
        public override void Process()
        {
            Context.Load(Element,
                f => f.ListItemAllFields.ParentList);
            Context.ExecuteQuery();

            // OnFolderRunning
            ActiveReceivers.ForEach(r => r.OnFolderRunning(Element));

            // If at least one receiver run list items of deeper
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.ListItem)))
            {
                // TODO V2 Manage large lists
                CamlQuery itemsQuery = new CamlQuery()
                {
                    FolderServerRelativeUrl = Element.ListItemAllFields["FileRef"].ToString(),
                    ViewXml = "<View><Query></Query></View>"
                };

                ListItemCollection items = Element.ListItemAllFields.ParentList.GetItems(itemsQuery);
                Context.Load(items);
                Context.ExecuteQuery();

                List<ListItemRunner> itemRunners = new List<ListItemRunner>();
                foreach (ListItem item in items)
                {
                    itemRunners.Add(new ListItemRunner(Manager, Context, item));
                }

                itemRunners.ForEach(r => r.Process());
            }

            // TODO V2 Manage large lists
            CamlQuery subFoldersQuery = new CamlQuery()
            {
                FolderServerRelativeUrl = Element.ListItemAllFields["FileRef"].ToString(),
                ViewXml = "<View><Query><Where><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>1</Value></Eq></Where></Query></View>"
            };

            ListItemCollection subFolders = Element.ListItemAllFields.ParentList.GetItems(subFoldersQuery);
            Context.Load(subFolders,
                coll => coll.Include(
                    f => f.Folder));
            Context.ExecuteQuery();

            List<FolderRunner> folderRunners = new List<FolderRunner>();
            foreach (ListItem folder in subFolders)
            {
                folderRunners.Add(new FolderRunner(Manager, Context, folder.Folder));
            }

            folderRunners.ForEach(r => r.Process());
        }
    }
}
