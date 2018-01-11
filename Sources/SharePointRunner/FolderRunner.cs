using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;

namespace SharePointRunner
{
    internal class FolderRunner : Runner<Folder>
    {
        /// <summary>
        /// Running level
        /// </summary>
        public override RunningLevel RunningLevel => RunningLevel.Folder;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="folder">Folder</param>
        public FolderRunner(RunningManager runningManager, ClientContext context, Folder folder) : base(runningManager, context, folder) { }

        /// <summary>
        /// Action for this SharePoint folder
        /// </summary>
        public override void Process()
        {
            // TODO V1 Folder Process
        }
    }
}
