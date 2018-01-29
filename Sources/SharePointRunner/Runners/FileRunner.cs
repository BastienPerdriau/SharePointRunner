using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;

namespace SharePointRunner
{
    internal class FileRunner : Runner<File>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint context</param>
        /// <param name="file">File</param>
        public FileRunner(RunningManager runningManager, ClientContext context, File file) : base(runningManager, context, file, RunningLevel.File) { }

        /// <summary>
        /// Action for this SharePoint file
        /// </summary>
        public override void Process()
        {
            RunningManager.Logger.Debug($"FileRunner Process() - {ActiveReceivers.Count} active receivers");
            Context.Load(Element,
                f => f.Name,
                f => f.ServerRelativeUrl);
            Context.ExecuteQuery();
            RunningManager.Logger.Debug($"File | Name: {Element.Name} / URL: {Element.ServerRelativeUrl}");

            // OnFileRunning
            RunningManager.Logger.Debug("FileRunner OnFileRunning()");
            ActiveReceivers.ForEach(r => r.OnFileRunning(Element));
        }
    }
}
