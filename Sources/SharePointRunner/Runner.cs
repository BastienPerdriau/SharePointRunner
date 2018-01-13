using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System.Collections.Generic;
using System.Linq;

namespace SharePointRunner
{
    /// <summary>
    /// Base class of a runner
    /// </summary>
    internal abstract class Runner
    {
        /// <summary>
        /// Running Maanger
        /// </summary>
        public RunningManager Manager { get; }

        /// <summary>
        /// Running level
        /// </summary>
        public RunningLevelEnum RunningLevel { get; }

        /// <summary>
        /// List of active receivers for this runner
        /// </summary>
        protected virtual List<Receiver> ActiveReceivers => Manager.Receivers.Where(r => r.IsReceiverCalled(RunningLevel)).ToList();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="runningLevel">Running level</param>
        public Runner(RunningManager runningManager, RunningLevelEnum runningLevel)
        {
            Manager = runningManager;
            RunningLevel = runningLevel;
        }

        /// <summary>
        /// Action for this SharePoint object
        /// </summary>
        public abstract void Process();
    }

    /// <summary>
    /// Base class of a runner of SharePoint object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class Runner<T> : Runner where T : ClientObject
    {
        /// <summary>
        /// SharePoint Element
        /// </summary>
        public T Element { get; }

        /// <summary>
        /// SharePoint Context
        /// </summary>
        public ClientContext Context { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="context">SharePoint Context</param>
        /// <param name="element">Current SharePoint object</param>
        /// <param name="runningLevel">Running level</param>
        public Runner(RunningManager runningManager, ClientContext context, T element, RunningLevelEnum runningLevel) : base(runningManager, runningLevel)
        {
            Context = context;
            Element = element;
        }
    }
}
