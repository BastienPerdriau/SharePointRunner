using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using SP = Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

namespace SharePointRunner.SDK
{
    /// <summary>
    /// Base class of receiver of running process
    /// </summary>
    public abstract class Receiver
    {
        /// <summary>
        /// True if the receiver needs to include sub sites when running, False if not
        /// </summary>
        public virtual bool IncludeSubSites { get; set; } = true;

        /// <summary>
        /// True if the receiver needs to include hidden lists when running, False if not
        /// </summary>
        public virtual bool IncludeHiddenLists { get; set; } = false;

        /// <summary>
        /// Properties loading promises
        /// </summary>
        private Promise<T>[] promises = new Promise<T>[];

        /// <summary>
        /// List of running levels implemented by the receiver
        /// </summary>
        private List<RunningLevel> runningLevels;

        /// <summary>
        /// Constructor
        /// </summary>
        public Receiver()
        {
            // Initialize the list of current receiver running levels
            runningLevels = new List<RunningLevel>();

            // Get all the names of the overriden methods by the current type
            List<string> methodNames = GetType().GetMethods().Where(m => IsMethodOverriden(m)).Select(m => m.Name).ToList();

            foreach (BaseRunningLevel level in RunningLevel.Values.Keys)
            {
                // Initialize the regex with the current running level
                Regex regex = new Regex($"On{level}Running[a-zA-Z]*");

                // If at least one of the methods matches the regex, add the current running level to the list
                if (methodNames.Any(m => regex.IsMatch(m)))
                {
                    runningLevels.Add(RunningLevel.Values[level]);
                }
            }
        }

        /// <summary>
        /// Know if the type has his own declaration of the method
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="methodName">Name of the method</param>
        /// <param name="includeAbstractDeclaration">True if an abstract implementation should be included, False if not (False by default)</param>
        /// <returns>True if the type has his own declaration of the method, False if not</returns>
        private bool IsMethodOverriden(Type type, string methodName, bool includeAbstractDeclaration = false)
        {
            MethodInfo method = type.GetMethod(methodName);

            return IsMethodOverriden(method, includeAbstractDeclaration);
        }

        /// <summary>
        /// Know if the method is overriden
        /// </summary>
        /// <param name="method">Method</param>
        /// <param name="includeAbstractDeclaration">True if an abstract implementation should be included, False if not (False by default)</param>
        /// <returns>True if the type has his own declaration of the method, False if not</returns>
        private bool IsMethodOverriden(MethodInfo method, bool includeAbstractDeclaration = false)
        {
            return method.DeclaringType != method.GetBaseDefinition().DeclaringType && !method.IsAbstract;
        }

        /// <summary>
        /// Add properties to load before execution
        /// </summary>
        /// <typeparam name="T">ClientObject inherited class</typeparam>
        /// <param name="runningLevel">Running level of properties</param>
        /// <param name="expressions">Expressions of properties to load</param>
        protected void AddPropertiesLoading<T>(RunningLevel runningLevel, params Expression<Func<T, object>>[] expressions) where T : ClientObject
        {
            // TODO Secure RunningLevel and expressions
            // TODO Remove RunningLevel from parameters et get it from T with switch and return false if not added to Promises
            promises.Add(new Promise<T>() { RunningLevel = runningLevel, Properties = expressions });
        }
        // TODO Manage common properties in a same Receiver and between receivers

        public Expression<Func<T, object>>[] GetPropertiesLoading<T>()
        {

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
        /// Event at the start of handling the term store
        /// </summary>
        /// <param name="termStore">Term store</param>
        public virtual void OnTermStoreRunningStart(TermStore termStore) { }

        /// <summary>
        /// Event at the start of handling a term group
        /// </summary>
        /// <param name="termGroup">Term group</param>
        public virtual void OnTermGroupRunningStart(TermGroup termGroup) { }

        /// <summary>
        /// Event at the start of handling a term set
        /// </summary>
        /// <param name="termSet">Term set</param>
        public virtual void OnTermSetRunningStart(TermSet termSet) { }

        /// <summary>
        /// Event at the start of handling a term
        /// </summary>
        /// <param name="term">Term</param>
        public virtual void OnTermRunningStart(Term term) { }

        /// <summary>
        /// Event at the end of handling a term
        /// </summary>
        /// <param name="term">Term</param>
        public virtual void OnTermRunningEnd(Term term) { }

        /// <summary>
        /// Event at the end of handling a term set
        /// </summary>
        /// <param name="termSet">Term set</param>
        public virtual void OnTermSetRunningEnd(TermSet termSet) { }

        /// <summary>
        /// Event at the end of handling a term group
        /// </summary>
        /// <param name="termGroup">Term group</param>
        public virtual void OnTermGroupRunningEnd(TermGroup termGroup) { }

        /// <summary>
        /// Event at the end of handling the term store
        /// </summary>
        /// <param name="termStore">Term store</param>
        public virtual void OnTermStoreRunningEnd(TermStore termStore) { }

        /// <summary>
        /// Event at the start of handling a site collection
        /// </summary>
        /// <param name="site">Site collection</param>
        /// <param name="rootWeb">Root site</param>
        public virtual void OnSiteCollectionRunningStart(Site site, Web rootWeb) { }

        /// <summary>
        /// Event to handle a group
        /// </summary>
        /// <param name="group">Group</param>
        public virtual void OnGroupRunning(SP.Group group) { }

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
        /// Event to handle a view
        /// </summary>
        /// <param name="view">View</param>
        public virtual void OnViewRunning(View view) { }

        /// <summary>
        /// Event at the start of handling a folder
        /// </summary>
        /// <param name="folder">Folder</param>
        public virtual void OnFolderRunningStart(Folder folder) { }

        /// <summary>
        /// Event to handle a list item
        /// </summary>
        /// <param name="listItem">List item</param>
        public virtual void OnListItemRunning(ListItem listItem) { }

        /// <summary>
        /// Event to handle a file
        /// </summary>
        /// <param name="file">File</param>
        public virtual void OnFileRunning(File file) { }

        /// <summary>
        /// Event at the end of handling a folder
        /// </summary>
        /// <param name="folder">Folder</param>
        public virtual void OnFolderRunningEnd(Folder folder) { }

        /// <summary>
        /// Event at the end of handling sub folders of a folder
        /// </summary>
        /// <param name="folder">Folder</param>
        public virtual void OnFolderRunningEndAfterSubFolders(Folder folder) { }

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
        public bool IsReceiverCalled(RunningLevel runningLevel) => runningLevels.Contains(runningLevel);

        /// <summary>
        /// Know if the receiver will be called specific running level nor one of next level
        /// </summary>
        /// <param name="runningLevel">Running level</param>
        /// <returns>True if the receiver will be called, False if not</returns>
        public bool IsReceiverCalledOrDeeper(RunningLevel runningLevel) => runningLevels.Any(l => l <= runningLevel);
    }
}
