using CsvHelper.Configuration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using SharePointRunner.SDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharePointRunner.LauncherV1
{
    /// <summary>
    /// Securable object informations
    /// </summary>
    public class SecurableObjectInfo
    {
        /// <summary>
        /// Enumeration of types of securable objects
        /// </summary>
        public enum SecurableObjectType
        {
            Site,
            Liste,
            Dossier
        }

        public SecurableObjectType Type { get; set; }

        public string TypeString => Type.ToString();

        public string Url { get; set; } = string.Empty;

        public string SiteTitle { get; set; } = string.Empty;

        public string ListTitle { get; set; } = string.Empty;

        public string FolderTitle { get; set; } = string.Empty;

        public bool PermissionsInherited { get; set; }

        public string PermissionsInheritedString => PermissionsInherited ? "Oui" : "Non";

        public string UserName { get; set; } = string.Empty;

        public string Permissions { get; set; } = string.Empty;

        private SecurableObjectInfo(SecurableObject securableObject, RoleAssignment roleAssignment)
        {
            PermissionsInherited = !securableObject.HasUniqueRoleAssignments;
            UserName = roleAssignment.Member.Title;
            Permissions = string.Join("|", roleAssignment.RoleDefinitionBindings.Select(b => b.Name));
        }

        public SecurableObjectInfo(Web web, RoleAssignment roleAssignment) : this((SecurableObject)web, roleAssignment)
        {
            Type = SecurableObjectType.Site;
            Url = web.Url;
            SiteTitle = web.Title;
        }

        public SecurableObjectInfo(List list, RoleAssignment roleAssignment) : this((SecurableObject)list, roleAssignment)
        {
            Type = SecurableObjectType.Liste;
            Url = list.RootFolder.ServerRelativeUrl;
            SiteTitle = list.ParentWeb.Title;
            ListTitle = list.Title;
        }

        public SecurableObjectInfo(ListItem listItem, RoleAssignment roleAssignment) : this((SecurableObject)listItem, roleAssignment)
        {
            Type = SecurableObjectType.Dossier;
            Url = listItem["FileRef"]?.ToString();
            SiteTitle = listItem.ParentList.ParentWeb.Title;
            ListTitle = listItem.ParentList.Title;
            FolderTitle = listItem.DisplayName;
        }
    }

    /// <summary>
    /// CSV mapping
    /// </summary>
    internal class SecurableObjectInfoMap : ClassMap<SecurableObjectInfo>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SecurableObjectInfoMap()
        {
            Map(m => m.TypeString).Name("Type");
            Map(m => m.Url).Name("URL");
            Map(m => m.SiteTitle).Name("Titre du site");
            Map(m => m.ListTitle).Name("Titre de la site");
            Map(m => m.FolderTitle).Name("Titre du dossier");
            Map(m => m.PermissionsInheritedString).Name("Permissions héritées");
            Map(m => m.UserName).Name("Utilisateur / Groupe");
            Map(m => m.Permissions).Name("Permissions");
        }
    }

    /// <summary>
    /// Informations d'un groupe
    /// </summary>
    public class GroupInfo
    {
        public string Name { get; set; } = string.Empty;

        public string Page { get; set; } = string.Empty;

        public string Members { get; set; } = string.Empty;

        public GroupInfo(string siteUrl, Group group)
        {
            Name = group.Title;
            Page = $"{siteUrl}/_layouts/people.aspx?MembershipGroupId={group.Id}";
            Members = string.Join("|", group.Users.Select(u => u.LoginName));
        }
    }

    /// <summary>
    /// CSV mapping
    /// </summary>
    internal class GroupInfoMap : ClassMap<GroupInfo>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GroupInfoMap()
        {
            Map(m => m.Name);
            Map(m => m.Page);
            Map(m => m.Members);
        }
    }

    /// <summary>
    /// Permissions manager receiver
    /// </summary>
    public class PermissionsReceiver : Receiver
    {
        private List<int> groupIds;

        private CsvWriterWrapper<SecurableObjectInfo, SecurableObjectInfoMap> permissionsFileWriter;
        private CsvWriterWrapper<GroupInfo, GroupInfoMap> permissionsGroupsFileWriter;

        /// <summary>
        /// Load a securable object
        /// </summary>
        /// <param name="securableObject">Securable object</param>
        private void LoadSecurableObject(SecurableObject securableObject)
        {
            switch (securableObject)
            {
                case Web web:
                    securableObject.Context.Load(web,
                        w => w.Title,
                        w => w.Url);
                    break;
                case List list:
                    securableObject.Context.Load(list,
                        l => l.Title,
                        l => l.RootFolder.ServerRelativeUrl,
                        l => l.ParentWeb.Title);
                    break;
                case ListItem listItem:
                    securableObject.Context.Load(listItem);
                    break;
            }

            securableObject.Context.Load(securableObject,
                o => o.HasUniqueRoleAssignments,
                o => o.RoleAssignments.Include(
                    a => a.Member.Title,
                    a => a.Member.PrincipalType,
                    a => a.Member.Id,
                    a => a.RoleDefinitionBindings.Include(
                        b => b.Name)));
            securableObject.Context.ExecuteQuery();
        }

        /// <summary>
        /// Export the properties of a securable objet to CSV
        /// </summary>
        /// <param name="securableObject">Securable object</param>
        private void WriteCsv(SecurableObject securableObject)
        {
            LoadSecurableObject(securableObject);

            SecurableObjectInfo securableObjectInfo = null;
            foreach (RoleAssignment roleAssignment in securableObject.RoleAssignments)
            {
                switch (securableObject)
                {
                    case Web web:
                        securableObjectInfo = new SecurableObjectInfo(web, roleAssignment);
                        break;
                    case List list:
                        securableObjectInfo = new SecurableObjectInfo(list, roleAssignment);
                        break;
                    case ListItem listItem:
                        securableObjectInfo = new SecurableObjectInfo(listItem, roleAssignment);
                        break;
                }

                // Write CSV
                permissionsFileWriter.WriteRecord(securableObjectInfo);

                if (roleAssignment.Member.PrincipalType == PrincipalType.SharePointGroup && !groupIds.Contains(roleAssignment.Member.Id))
                {
                    groupIds.Add(roleAssignment.Member.Id);
                }
            }
        }

        public override void OnSiteCollectionRunningStart(Site site, Web rootWeb)
        {
            // Init file names
            rootWeb.Context.Load(rootWeb,
                w => w.Title);
            rootWeb.Context.ExecuteQuery();

            string permissionsFileName = $"AuditPermissions-{rootWeb.Title}-{DateTime.Now.ToString("yyyy-MM-dd HH,mm,ss")}.csv";
            permissionsFileWriter = new CsvWriterWrapper<SecurableObjectInfo, SecurableObjectInfoMap>(permissionsFileName);

            string permissionsGroupsFileName = $"AuditPermissionsGroups-{rootWeb.Title}-{DateTime.Now.ToString("yyyy-MM-dd HH,mm,ss")}.csv";
            permissionsGroupsFileWriter = new CsvWriterWrapper<GroupInfo, GroupInfoMap>(permissionsGroupsFileName);

            groupIds = new List<int>();
        }

        public override void OnSiteRunningStart(Web web)
        {
            WriteCsv(web);
        }

        public override void OnListRunningStart(List list)
        {
            WriteCsv(list);
        }

        public override void OnSiteCollectionRunningEnd(Site site, Web rootWeb)
        {
            // Crawl groups
            groupIds = groupIds.Distinct().ToList();

            rootWeb.Context.Load(rootWeb,
                w => w.Url,
                w => w.SiteGroups);


            foreach (int groupId in groupIds)
            {
                Group group = rootWeb.SiteGroups.GetById(groupId);

                rootWeb.Context.Load(group,
                    g => g.Id,
                    g => g.Title,
                    g => g.Users.Include(
                        u => u.LoginName));
                rootWeb.Context.ExecuteQuery();

                GroupInfo groupInfo = new GroupInfo(rootWeb.Url, group);

                // Write CSV
                permissionsGroupsFileWriter.WriteRecord(groupInfo);
            }
        }
    }
}
