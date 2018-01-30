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
            RunningManager.Logger.Debug($"FolderRunner Process() - {ActiveReceivers.Count} active receivers");
            Context.Load(Element,
                f => f.Name,
                f => f.ServerRelativeUrl,
                f => f.ListItemAllFields["FileRef"],
                f => f.ListItemAllFields.ParentList.ItemCount);
            Context.ExecuteQuery();
            RunningManager.Logger.Debug($"Folder | Name: {Element.Name} / URL: {Element.ServerRelativeUrl}");

            // OnFolderRunningStart
            RunningManager.Logger.Debug("FolderRunner OnFolderRunningStart()");
            ActiveReceivers.ForEach(r => r.OnFolderRunningStart(Element));

            // If at least one receiver run list items of deeper
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.ListItem)))
            {
                List<ListItem> items = new List<ListItem>();

                if (Element.ListItemAllFields.ParentList.ItemCount > 5000)
                {
                    // Manage large lists
                    int count = 0;
                    int inter = 1000;
                    int countList = Element.ListItemAllFields.ParentList.ItemCount;

                    while (count < countList)
                    {
                        CamlQuery itemsQuery = new CamlQuery()
                        {
                            FolderServerRelativeUrl = Element.ListItemAllFields["FileRef"].ToString(),
                            ViewXml = $"<View><Query><Where><And><Gt><FieldRef Name='ID'/><Value Type='Counter'>{count}</Value></Gt><Leq><FieldRef Name='ID'/><Value Type='Counter'>{count + inter}</Value></Leq></And></Where><OrderBy Override='TRUE'><FieldRef Name='ID'/></OrderBy></Query></View><RowLimit>{inter}</RowLimit>"
                        };

                        ListItemCollection itemsResult = Element.ListItemAllFields.ParentList.GetItems(itemsQuery);
                        Context.Load(itemsResult);
                        Context.ExecuteQuery();
                        items.AddRange(itemsResult);

                        count += inter;
                    }
                }
                else
                {
                    CamlQuery itemsQuery = new CamlQuery()
                    {
                        FolderServerRelativeUrl = Element.ListItemAllFields["FileRef"].ToString(),
                        ViewXml = "<View><Query></Query></View>"
                    };

                    ListItemCollection itemsResult = Element.ListItemAllFields.ParentList.GetItems(itemsQuery);
                    Context.Load(itemsResult);
                    Context.ExecuteQuery();
                    items = itemsResult.ToList();
                }

                List<ListItemRunner> itemRunners = new List<ListItemRunner>();
                foreach (ListItem item in items)
                {
                    itemRunners.Add(new ListItemRunner(Manager, Context, item));
                }

                itemRunners.ForEach(r => r.Process());
            }

            // OnFolderRunningEnd
            RunningManager.Logger.Debug("FolderRunner OnFolderRunningEnd()");
            ActiveReceivers.ForEach(r => r.OnFolderRunningEnd(Element));

            List<ListItem> subFolders = new List<ListItem>();

            if (Element.ListItemAllFields.ParentList.ItemCount > 5000)
            {
                // Manage large lists
                int count = 0;
                int inter = 1000;
                int countList = Element.ListItemAllFields.ParentList.ItemCount;

                while (count < countList)
                {
                    CamlQuery subFoldersQuery = new CamlQuery()
                    {
                        FolderServerRelativeUrl = Element.ListItemAllFields["FileRef"].ToString(),
                        ViewXml = $"<View><Query><Where><And><And><Gt><FieldRef Name='ID'/><Value Type='Counter'>{count}</Value></Gt><Leq><FieldRef Name='ID'/><Value Type='Counter'>{count + inter}</Value></Leq></And><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>1</Value></Eq></And></Where><OrderBy Override='TRUE'><FieldRef Name='ID'/></OrderBy></Query></View><RowLimit>{inter}</RowLimit>"
                    };

                    ListItemCollection subFoldersResult = Element.ListItemAllFields.ParentList.GetItems(subFoldersQuery);
                    Context.Load(subFoldersResult);
                    Context.ExecuteQuery();
                    subFolders.AddRange(subFoldersResult);

                    count += inter;
                }
            }
            else
            {
                CamlQuery subFoldersQuery = new CamlQuery()
                {
                    FolderServerRelativeUrl = Element.ListItemAllFields["FileRef"].ToString(),
                    ViewXml = "<View><Query><Where><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>1</Value></Eq></Where></Query></View>"
                };

                // Crawl sub folders
                ListItemCollection subFoldersResult = Element.ListItemAllFields.ParentList.GetItems(subFoldersQuery);
                Context.Load(subFoldersResult,
                    coll => coll.Include(
                        f => f.Folder));
                Context.ExecuteQuery();
                subFolders = subFoldersResult.ToList();
            }

            List<FolderRunner> folderRunners = new List<FolderRunner>();
            foreach (ListItem folder in subFolders)
            {
                folderRunners.Add(new FolderRunner(Manager, Context, folder.Folder));
            }

            folderRunners.ForEach(r => r.Process());

            // OnFolderRunningEndAfterSubFolders
            RunningManager.Logger.Debug("FolderRunner OnFolderRunningEndAfterSubFolders()");
            ActiveReceivers.ForEach(r => r.OnFolderRunningEndAfterSubFolders(Element));
        }
    }
}
