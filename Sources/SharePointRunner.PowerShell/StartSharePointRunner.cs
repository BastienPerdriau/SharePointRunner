using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace SharePointRunner.PowerShell
{
    [Cmdlet(VerbsLifecycle.Start, "SharePointRunner")]
    public class StartSharePointRunner : Cmdlet
    {
        [Parameter(Position = 0)]
        public string ConfigFilePath { get; set; } = string.Empty;

        [Parameter(Position = 1, Mandatory = false)]
        public PSCredential Credentials { get; set; } = null;

        protected override void ProcessRecord()
        {
            SharePointOnlineCredentials spoCreds = null;

            if (Credentials != null)
            {
                spoCreds = new SharePointOnlineCredentials(Credentials.UserName, Credentials.Password);
            }

            SharePointRunner.Run(ConfigFilePath, spoCreds);
        }
    }
}
