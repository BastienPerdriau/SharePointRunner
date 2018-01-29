using CommonLibraryExample;
using CsvHelper.Configuration;
using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System;

namespace SecondLibraryExample
{
    /// <summary>
    /// Site usage informations
    /// </summary>
    public class WebUsageInfo
    {
        public string SiteUrl { get; set; } = string.Empty;

        public string SiteTitle { get; set; } = string.Empty;

        public DateTime SiteCreationDate { get; set; }

        public string SiteCreationDateString => SiteCreationDate.ToString("yyyy-MM-dd HH,mm,ss");

        public int SiteItemCount { get; set; } = 0;

        public string ListUrl { get; set; } = string.Empty;

        public string ListTitle { get; set; } = string.Empty;

        public DateTime LastItemUpdate { get; set; } = new DateTime(1900, 1, 1);

        public string LastItemUpdateString => LastItemUpdate.ToString("yyyy-MM-dd HH,mm,ss");

        public string LastEditor { get; set; }
    }

    /// <summary>
    /// CSV mapping
    /// </summary>
    internal class WebUsageInfoMap : ClassMap<WebUsageInfo>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WebUsageInfoMap()
        {
            Map(m => m.SiteUrl);
            Map(m => m.SiteTitle);
            Map(m => m.SiteCreationDateString);
            Map(m => m.SiteItemCount);
            Map(m => m.ListTitle);
            Map(m => m.LastItemUpdateString);
        }
    }

    public class WebUsageReceiver : Receiver
    {
        private CsvWriterWrapper<WebUsageInfo, WebUsageInfoMap> webUsageFileWriter;
        private WebUsageInfo webUsageInfo;

        public override void OnStart()
        {
            string webUsageFileName = $"AuditWebUsage-{DateTime.Now.ToString("yyyy-MM-dd HH,mm,ss")}.csv";
            webUsageFileWriter = new CsvWriterWrapper<WebUsageInfo, WebUsageInfoMap>(webUsageFileName);
        }

        public override void OnSiteRunningStart(Web web)
        {
            web.Context.Load(web,
                w => w.Title,
                w => w.Url,
                w => w.Created);
            web.Context.ExecuteQuery();

            webUsageInfo = new WebUsageInfo()
            {
                SiteUrl = web.Url,
                SiteTitle = web.Title,
                SiteCreationDate = web.Created
            };
        }

        public override void OnListRunningStart(List list)
        {
            list.Context.Load(list,
                l => l.Title,
                l => l.RootFolder.ServerRelativeUrl,
                l => l.Hidden,
                l => l.ItemCount);
            list.Context.ExecuteQuery();

            if (list.ItemCount < 5000 && !list.Hidden && !list.RootFolder.ServerRelativeUrl.Contains("/Catalogs/") && list.Title != "Long Running Operation Status")
            {
                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View Scope='Recursive'><RowLimit>1</RowLimit><Query><OrderBy><FieldRef Name='Modified' Ascending='False' /></OrderBy></Query></View>";
                ListItemCollection items = list.GetItems(query);

                list.Context.Load(items);
                list.Context.ExecuteQuery();

                webUsageInfo.SiteItemCount += list.ItemCount;

                foreach (ListItem listItem in items)
                {
                    list.Context.Load(listItem);
                    list.Context.ExecuteQuery();

                    if (DateTime.TryParse(listItem["Modified"]?.ToString(), out DateTime modified) && modified > webUsageInfo.LastItemUpdate)
                    {
                        webUsageInfo.LastItemUpdate = modified;
                        webUsageInfo.ListUrl = list.RootFolder.ServerRelativeUrl;
                        webUsageInfo.ListTitle = list.Title;

                        FieldUserValue userField = listItem["Editor"] as FieldUserValue;
                        webUsageInfo.LastEditor = userField.LookupValue;
                    }
                }
            }
        }

        public override void OnSiteRunningEnd(Web web)
        {
            // Write CSV
            webUsageFileWriter.WriteRecord(webUsageInfo);
        }
    }
}
