using CommonLibraryExample;
using CsvHelper.Configuration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using SharePointRunner.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SecondLibraryExample
{
    /// <summary>
    /// Page informations
    /// </summary>
    public class PageInfo
    {
        public string SiteUrl { get; set; } = string.Empty;

        public string SiteTitle { get; set; } = string.Empty;

        public string PageUrl { get; set; } = string.Empty;

        public string PageName { get; set; } = string.Empty;

        public int WebPartCount { get; set; } = 0;

        public PageInfo(Web web, ListItem listItem, LimitedWebPartManager webPartManager)
        {
            SiteUrl = web.Url;
            SiteTitle = web.Title;
            PageUrl = listItem["FileRef"]?.ToString();
            PageName = listItem["FileLeafRef"]?.ToString();
            WebPartCount = webPartManager.WebParts.Count;
        }
    }

    /// <summary>
    /// CSV mapping
    /// </summary>
    internal class PageInfoMap : ClassMap<PageInfo>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PageInfoMap()
        {
            Map(m => m.SiteUrl);
            Map(m => m.SiteTitle);
            Map(m => m.PageUrl);
            Map(m => m.PageName);
            Map(m => m.WebPartCount);
        }
    }

    public class WebPartsReceiver : Receiver
    {
        private CsvWriterWrapper<PageInfo, PageInfoMap> webPartsFileWriter;

        private IEnumerable<ListItem> LoadItems(List list, string viewXml, List<string> viewFields, params Expression<Func<ListItem, object>>[] properties)
        {
            // Used to resolve 'the query expression is not supported' error loading item with FieldValues...
            // http://www.manvir.net/invalidqueryexpressionexception-the-query-expression-is-not-supported/
            CamlQuery query = new CamlQuery();
            query.ViewXml = viewXml;

            ListItemCollection items = list.GetItems(query);
            List<Expression<Func<ListItem, object>>> listItemExpressions = new List<Expression<Func<ListItem, object>>>(properties);

            foreach (string viewField in viewFields)
            {
                Expression<Func<ListItem, object>> retrieveFiedlDataExpresion = item => item[viewField];
                listItemExpressions.Add(retrieveFiedlDataExpresion);
            }

            IEnumerable<ListItem> resultData = list.Context.LoadQuery(items.Include(listItemExpressions.ToArray()));
            list.Context.ExecuteQuery();

            return resultData;
        }

        public override void OnSiteCollectionRunningStart(Site site, Web rootWeb)
        {
            rootWeb.Context.Load(rootWeb,
                w => w.Title);
            rootWeb.Context.ExecuteQuery();

            string webPartsFileName = $"AuditWebParts-{rootWeb.Title}-{DateTime.Now.ToString("yyyy-MM-dd HH,mm,ss")}.csv";
            webPartsFileWriter = new CsvWriterWrapper<PageInfo, PageInfoMap>(webPartsFileName);
        }

        public override void OnSiteRunningStart(Web web)
        {
            web.Context.Load(web,
                w => w.Title,
                w => w.Url,
                w => w.Lists.Include(
                    l => l.RootFolder.ServerRelativeUrl));
            web.Context.ExecuteQuery();

            List pages = web.Lists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals($"{web.ServerRelativeUrl}/Pages", StringComparison.InvariantCultureIgnoreCase));

            if (pages == null)
            {
                pages = web.Lists.FirstOrDefault(l => l.RootFolder.ServerRelativeUrl.Equals($"{web.ServerRelativeUrl}/SitePages", StringComparison.InvariantCultureIgnoreCase));
            }

            if (pages != null)
            {
                web.Context.Load(pages);
                web.Context.ExecuteQuery();

                string viewXml = "<View Scope='RecursiveAll'><Query><Where><And><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>0</Value></Eq><Eq><FieldRef Name='File_x0020_Type' /><Value Type='text'>aspx</Value></Eq></And></Where></Query></View>";
                List<string> viewFields = new List<string>()
                {
                    "FileRef",
                    "FileLeafRef"
                };

                IEnumerable<ListItem> items = LoadItems(pages, viewXml, viewFields, li => li.File.Exists, li => li.FileSystemObjectType);

                foreach (ListItem item in items)
                {
                    if (item.FileSystemObjectType == FileSystemObjectType.File && item.File.Exists)
                    {
                        LimitedWebPartManager webPartManager = item.File.GetLimitedWebPartManager(PersonalizationScope.Shared);
                        web.Context.Load(webPartManager,
                            wpm => wpm.WebParts);
                        web.Context.ExecuteQuery();

                        PageInfo pageInfo = new PageInfo(web, item, webPartManager);

                        // Write CSV
                        webPartsFileWriter.WriteRecord(pageInfo);
                    }
                }
            }
        }
    }
}
