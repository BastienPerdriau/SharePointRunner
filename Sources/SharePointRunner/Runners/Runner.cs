using Microsoft.SharePoint.Client;
using SharePointRunner.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
        public RunningLevel RunningLevel { get; }

        /// <summary>
        /// List of active receivers for this runner
        /// </summary>
        protected virtual List<Receiver> ActiveReceivers => Manager.Receivers.Where(r => r.IsReceiverCalled(RunningLevel)).ToList();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runningManager">Running manager</param>
        /// <param name="runningLevel">Running level</param>
        public Runner(RunningManager runningManager, RunningLevel runningLevel)
        {
            Manager = runningManager;
            RunningLevel = runningLevel;
        }

        /// <summary>
        /// Action for this object
        /// </summary>
        public abstract void Process();
        // TODO At the start of each Process(), get the properties loading promises and call it
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
        
        protected Expression<Func<T, object>>[] Expressions => Manager.Receivers.SelectMany(r => r.Promises).Where(p => p.RunningLevel == RunningLevel);

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
        public Runner(RunningManager runningManager, ClientContext context, T element, RunningLevel runningLevel) : base(runningManager, runningLevel)
        {
            Context = context;
            Element = element;
        }
    }
}
