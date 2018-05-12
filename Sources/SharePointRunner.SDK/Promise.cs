using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;

namespace SharePointRunner.SDK
{
    /// <summary>
    /// Base class of the properties loading promises
    /// </summary>
    public abstract class Promise
    {
        /// <summary>
        /// Running level of the promise
        /// </summary>
        public RunningLevel RunningLevel { get; set; }
    }

    /// <summary>
    /// Class of the properties loading promises
    /// </summary>
    /// <typeparam name="T">ClientObject inherited class</typeparam>
    public class Promise<T> : Promise where T : ClientObject
    {
        /// <summary>
        /// Properties to load
        /// </summary>
        public Expression<Func<T, object>>[] Properties { get; set; }
    }
}
