using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharePointRunner.SDK
{
    /// <summary>
    /// Base class of receiver of running process
    /// </summary>
    public abstract class Receiver
    {
        /// <summary>
        /// Dictionnary of methods of the receiver corresponding to each running level
        /// </summary>
        private Dictionary<RunningLevel, List<string>> RunningMethodsMap = new Dictionary<RunningLevel, List<string>>()
        {
            { RunningLevel.Tenant, new List<string>() { "OnTenantRunningStart", "OnTenantRunningEnd" } },
            { RunningLevel.SiteCollection, new List<string>() { "OnSiteCollectionRunningStart", "OnSiteCollectionRunningEnd" } },
            { RunningLevel.Site, new List<string>() { "OnSiteRunningStart", "OnSiteRunningEnd", "OnSiteRunningEndAfterSubSites" } },
            { RunningLevel.List, new List<string>() { "OnListRunningStart", "OnListRunningEnd" } },
            { RunningLevel.Folder, new List<string>() { "OnFolderRunning" } },
            { RunningLevel.ListItem, new List<string>() { "OnListItemRunning" } },
            { RunningLevel.File, new List<string>() { "OnFileRunning" } }
        };

        /// <summary>
        /// True if the receiver needs to include sub sites when running, False if not
        /// </summary>
        public virtual bool IncludeSubSites { get; set; } = true;

        /// <summary>
        /// List of running levels implemented by the receiver
        /// </summary>
        private List<RunningLevel> RunningLevels { get; set; }

        /// <summary>
        /// Know if the type has his own declaration of the method
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="methodName">Name of the method</param>
        /// <param name="includeAbstractDeclaration">True if an abstract implementation should be included, False if not (False by default)</param>
        /// <returns>True if the type has his own declaration of the method, False if not</returns>
        private bool IsMethodOverriden(Type type, string methodName, bool includeAbstractDeclaration = false)
        {
            MethodInfo method = type.GetMethod(methodName);

            return method.DeclaringType != method.GetBaseDefinition().DeclaringType && !method.IsAbstract;
        }

        /// <summary>
        /// Get the running levels implemented by the receiver
        /// </summary>
        /// <returns>List of running levels</returns>
        public List<RunningLevel> GetRunningLevels()
        {
            if (RunningLevels == null)
            {
                RunningLevels = new List<RunningLevel>();

                foreach (KeyValuePair<RunningLevel, List<string>> runningMethods in RunningMethodsMap)
                {
                    if (runningMethods.Value.Any(m => IsMethodOverriden(GetType(), m)))
                    {
                        RunningLevels.Add(runningMethods.Key);
                    }
                }
            }

            return RunningLevels;
        }

        /// <summary>
        /// Event at the start of process
        /// </summary>
        public virtual void OnStart() { }

        /// <summary>
        /// Event at the start of handling a tenant 
        /// </summary>
        /// <param name="tenant">Tenant</param>
        public virtual void OnTenantRunningStart(Tenant tenant) { }

        /// <summary>
        /// Event at the start of handling a site collection
        /// </summary>
        /// <param name="site">Site collection</param>
        /// <param name="rootWeb">Root site</param>
        public virtual void OnSiteCollectionRunningStart(Site site, Web rootWeb) { }

        /// <summary>
        /// Event at the start of handling a site
        /// </summary>
        /// <param name="web">Site</param>
        public virtual void OnSiteRunningStart(Web web) { }

        /// <summary>
        /// Event at the start of handling a list
        /// </summary>
        /// <param name="list">List</param>
        public virtual void OnListRunningStart(List list) { }

        /// <summary>
        /// Event at the start of handling a folder
        /// </summary>
        /// <param name="folder">Folder</param>
        public virtual void OnFolderRunning(Folder folder) { }

        /// <summary>
        /// Event at the start of handling a list item
        /// </summary>
        /// <param name="listItem">List item</param>
        public virtual void OnListItemRunning(ListItem listItem) { }

        /// <summary>
        /// Event at the start of handling a site
        /// </summary>
        /// <param name="file">File</param>
        public virtual void OnFileRunning(File file) { }

        /// <summary>
        /// Event at the end of handling a list
        /// </summary>
        /// <param name="list">List</param>
        public virtual void OnListRunningEnd(List list) { }

        /// <summary>
        /// Event at the end of handling a site
        /// </summary>
        /// <param name="web">Site</param>
        public virtual void OnSiteRunningEnd(Web web) { }

        /// <summary>
        /// Event at the end of handling sub sites of a site
        /// </summary>
        /// <param name="web">Site</param>
        public virtual void OnSiteRunningEndAfterSubSites(Web web) { }

        /// <summary>
        /// Event at the end of handling a site collection
        /// </summary>
        /// <param name="site">Site collection</param>
        /// <param name="rootWeb">Root site</param>
        public virtual void OnSiteCollectionRunningEnd(Site site, Web rootWeb) { }

        /// <summary>
        /// Event at the end of handling a tenant 
        /// </summary>
        /// <param name="tenant">Tenant</param>
        public virtual void OnTenantRunningEnd(Tenant tenant) { }

        /// <summary>
        /// Event et the end of the process
        /// </summary>
        public virtual void OnEnd() { }

        /// <summary>
        /// Know if the receiver should be called at a specific running level
        /// </summary>
        /// <param name="runningLevel">Running level</param>
        /// <returns>True if the receiver should be called, False if not</returns>
        public bool IsReceiverCalled(RunningLevel runningLevel) => GetRunningLevels().Contains(runningLevel);

        /// <summary>
        /// Know if the receiver will be called specific running level nor one of next level
        /// </summary>
        /// <param name="runningLevel">Running level</param>
        /// <returns>True if the receiver will be called, False if not</returns>
        public bool IsReceiverCalledOrDeeper(RunningLevel runningLevel) => GetRunningLevels().Any(l => l >= runningLevel);
    }
}
