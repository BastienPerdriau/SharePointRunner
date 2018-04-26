using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System.Collections.Generic;
using System.Security;

namespace SharePointRunner.LauncherV1
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<RunningLevel> levels = new List<RunningLevel>()
            //{
            //    RunningLevel.Tenant,
            //    RunningLevel.SiteCollection,
            //    RunningLevel.Site,
            //    RunningLevel.List,
            //    RunningLevel.View,
            //    RunningLevel.Folder,
            //    RunningLevel.ListItem,
            //    RunningLevel.File,
            //    RunningLevel.TermStore,
            //    RunningLevel.TermGroup,
            //    RunningLevel.TermSet,
            //    RunningLevel.Term
            //};

            //string str = "";

            //foreach (RunningLevel level1 in levels)
            //{
            //    str += $"{level1.ToString()}\n";
            //    foreach (RunningLevel level2 in levels)
            //    {
            //        str += $"{level1} < {level2} : {level1 < level2}\n";
            //        str += $"{level1} > {level2} : {level1 > level2}\n";
            //    }
            //    str += $"------------------------------------------------\n";
            //    str += $"\n";
            //}

            if (args.Length < 3)
            {
                return;
            }

            string tenantUrl = args[0];
            string adminLogin = args[1];
            string adminPassword = args[2];

            SecureString pwd = new SecureString();

            foreach (char c in adminPassword)
            {
                pwd.AppendChar(c);
            }

            SharePointOnlineCredentials cred = new SharePointOnlineCredentials(adminLogin, pwd);

            RunningManager manager = new RunningManager()
            {
                Credentials = cred,
                StartingRunningLevel = RunningLevel.Tenant,
            };

            manager.Urls.Add(tenantUrl);
            manager.Receivers.Add(new ManagedMetadataReceiver());
            //manager.Receivers.Add(new PermissionsReceiver());
            //manager.Receivers.Add(new WebUsageReceiver());
            //manager.Receivers.Add(new GroupsReceiver());
            //manager.Receivers.Add(new WebPartsReceiver());

            manager.Run();
        }
    }
}
