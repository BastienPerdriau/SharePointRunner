using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System.Collections.Generic;
using System.Linq;

namespace SharePointRunner
{
    internal class ListRunner : Runner<List>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="list">List</param>
        public ListRunner(RunningManager runningManager, ClientContext context, List list) : base(runningManager, context, list, RunningLevel.List) { }

        /// <summary>
        /// Action for this SharePoint list
        /// </summary>
        public override void Process()
        {
            RunningManager.Logger.Debug($"ListRunner Process() - {ActiveReceivers.Count} active receivers");
            Context.Load(Element,
                l => l.Title,
                l => l.RootFolder.ServerRelativeUrl);
            Context.ExecuteQuery();
            RunningManager.Logger.Debug($"List | Title: {Element.Title} / URL: {Element.RootFolder.ServerRelativeUrl}");

            // OnListRunningStart
            RunningManager.Logger.Debug("ListRunner OnListRunningStart()");
            ActiveReceivers.ForEach(r => r.OnListRunningStart(Element));

            // If at least one receiver run views
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.View)))
            {
                // Crawl views
                Context.Load(Element.Views);
                Context.ExecuteQuery();

                List<ViewRunner> viewRunners = new List<ViewRunner>();
                foreach (View view in Element.Views)
                {
                    viewRunners.Add(new ViewRunner(Manager, Context, view));
                }

                viewRunners.ForEach(r => r.Process());
            }

            // If at least one receiver run folders or deeper
            if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.Folder)))
            {
                List<ListItem> folders = new List<ListItem>();

                if (Element.ItemCount > 5000)
                {
                    // Manage large lists
                    int count = 0;
                    int inter = 1000;
                    int countList = Element.ItemCount;

                    while (count < countList)
                    {
                        CamlQuery foldersQuery = new CamlQuery()
                        {
                            ViewXml = $"<View><Query><Where><And><And><Gt><FieldRef Name='ID'/><Value Type='Counter'>{count}</Value></Gt><Leq><FieldRef Name='ID'/><Value Type='Counter'>{count + inter}</Value></Leq></And><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>1</Value></Eq></And></Where><OrderBy Override='TRUE'><FieldRef Name='ID'/></OrderBy></Query></View><RowLimit>{inter}</RowLimit>"
                        };

                        ListItemCollection foldersResult = Element.GetItems(foldersQuery);
                        Context.Load(foldersResult);
                        Context.ExecuteQuery();
                        folders.AddRange(foldersResult);

                        count += inter;
                    }
                }
                else
                {
                    CamlQuery foldersQuery = new CamlQuery()
                    {
                        ViewXml = "<View><Query><Where><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>1</Value></Eq></Where></Query></View>"
                    };

                    ListItemCollection foldersResult = Element.GetItems(foldersQuery);
                    Context.Load(foldersResult,
                        coll => coll.Include(
                            f => f.Folder));
                    Context.ExecuteQuery();
                    folders = foldersResult.ToList();
                }

                List<FolderRunner> folderRunners = new List<FolderRunner>();
                foreach (ListItem folder in folders)
                {
                    folderRunners.Add(new FolderRunner(Manager, Context, folder.Folder));
                }

                folderRunners.ForEach(r => r.Process());
            }
            else if (Manager.Receivers.Any(r => r.IsReceiverCalledOrDeeper(RunningLevel.ListItem)))
            {
                List<ListItem> items = new List<ListItem>();

                if (Element.ItemCount > 5000)
                {
                    // Manage large lists
                    int count = 0;
                    int inter = 1000;
                    int countList = Element.ItemCount;

                    while (count < countList)
                    {
                        CamlQuery itemsQuery = new CamlQuery()
                        {
                            ViewXml = $"<View Scope='RecursiveAll'><Query><Where><And><Gt><FieldRef Name='ID'/><Value Type='Counter'>{count}</Value></Gt><Leq><FieldRef Name='ID'/><Value Type='Counter'>{count + inter}</Value></Leq></And></Where><OrderBy Override='TRUE'><FieldRef Name='ID'/></OrderBy></Query></View><RowLimit>{inter}</RowLimit>"
                        };

                        ListItemCollection itemsResult = Element.GetItems(itemsQuery);
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
                        ViewXml = "<View Scope='RecursiveAll'><Query></Query></View>"
                    };

                    ListItemCollection itemsResult = Element.GetItems(itemsQuery);
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

            // OnListRunningEnd
            RunningManager.Logger.Debug("ListRunner OnListRunningEnd()");
            ActiveReceivers.ForEach(r => r.OnListRunningEnd(Element));
        }
    }
}
