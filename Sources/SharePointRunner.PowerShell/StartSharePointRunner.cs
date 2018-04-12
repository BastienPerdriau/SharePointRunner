using log4net;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace SharePointRunner.PowerShell
{
    [Cmdlet(VerbsLifecycle.Start, "SharePointRunner")]
    public class StartSharePointRunner : Cmdlet
    {
        /// <summary>
        /// Logger
        /// </summary>
        internal static readonly ILog Logger = LogManager.GetLogger(typeof(StartSharePointRunner).Namespace);

        [Parameter(Position = 0)]
        public string ConfigFilePath { get; set; } = string.Empty;

        [Parameter(Position = 1, Mandatory = false)]
        public PSCredential Credentials { get; set; } = null;

        protected override void ProcessRecord()
        {
            SharePointOnlineCredentials spoCreds = null;

            if (Credentials != null)
            {
                Logger.Debug("Creating SPO credentials from PS credentials");
                spoCreds = new SharePointOnlineCredentials(Credentials.UserName, Credentials.Password);
            }

            SharePointRunner.Run(ConfigFilePath, spoCreds);
        }
    }
}
