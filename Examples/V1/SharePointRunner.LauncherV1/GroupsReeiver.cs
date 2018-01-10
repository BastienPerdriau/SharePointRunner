using CsvHelper.Configuration;
using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharePointRunner.LauncherV1
{
    /// <summary>
    /// Group user informations
    /// </summary>
    public class GroupUserInfo
    {
        public string SiteUrl { get; set; }

        public string SiteTitle { get; set; }

        public string GroupName { get; set; }

        public string UserLogin { get; set; }

        public string UserName { get; set; }

        public string UserMail { get; set; }

        public GroupUserInfo(Site site, Web web, Group group, User user)
        {
            SiteUrl = site.Url;
            SiteTitle = web.Title;
            GroupName = group.Title;
            UserLogin = user.LoginName;
            UserName = user.Title;
            UserMail = user.Email;
        }
    }

    /// <summary>
    /// CSV mapping
    /// </summary>
    internal class GroupUserInfoMap : ClassMap<GroupUserInfo>
    {
        public GroupUserInfoMap()
        {
            Map(m => m.SiteUrl);
            Map(m => m.SiteTitle);
            Map(m => m.GroupName);
            Map(m => m.UserLogin);
            Map(m => m.UserName);
            Map(m => m.UserMail);
        }
    }

    public class GroupsReceiver : Receiver
    {
        /// <summary>
        /// Get running levels declared by the receiver
        /// </summary>
        /// <returns>List of running levels</returns>
        public override List<RunningLevel> GetRunningLevels()
        {
            return GetRunningLevels<GroupsReceiver>();
        }

        // TODO V2 To parameters
        List<string> groupNames = new List<string>() { "owners" };

        public override void OnSiteCollectionRunningStart(Site site, Web rootWeb)
        {
            site.Context.Load(site,
                s => s.Url);
            site.Context.Load(rootWeb,
                w => w.Title,
                w => w.Url,
                w => w.SiteGroups.Include(
                    g => g.Title));
            site.Context.ExecuteQuery();

            string groupsFileName = $"AuditGroups-{rootWeb.Title}-{DateTime.Now.ToString("yyyy-MM-dd HH,mm,ss")}.csv";
            CsvWriterWrapper<GroupUserInfo, GroupUserInfoMap> groupsFileWriter = new CsvWriterWrapper<GroupUserInfo, GroupUserInfoMap>(groupsFileName);

            foreach (string groupName in groupNames)
            {
                Group group = rootWeb.SiteGroups.FirstOrDefault(g => g.Title.ToLowerInvariant().Contains(groupName.ToLowerInvariant()));

                if (group != null)
                {
                    site.Context.Load(group,
                        g => g.Users.Include(
                            u => u.Title,
                            u => u.LoginName,
                            u => u.Email));
                    site.Context.ExecuteQuery();

                    foreach (User user in group.Users)
                    {
                        GroupUserInfo groupUserInfo = new GroupUserInfo(site, rootWeb, group, user);

                        // Write CSV
                        groupsFileWriter.WriteRecord(groupUserInfo);
                    }
                }
            }
        }
    }
}
