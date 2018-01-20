using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System.Security;

namespace SharePointRunner.LauncherV1
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO V2 Set Receivers to DLL with XML file
            // TODO V2 Parameterize logs: file, console, trace
            // TODO V3 Customize logs (activate or not, log level, file location...)
            // TODO V4 Create PS Cmdlets

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
            manager.Receivers.Add(new PermissionsReceiver());
            manager.Receivers.Add(new WebUsageReceiver());
            manager.Receivers.Add(new GroupsReceiver());
            manager.Receivers.Add(new WebPartsReceiver());

            manager.Run();
        }
    }
}
