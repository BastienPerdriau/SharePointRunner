using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;

namespace SharePointRunner.SDK
{
    public abstract class Promise { }

    public class Promise<T> : Promise where T : ClientObject
    {
        public Expression<Func<T, object>>[] Properties { get; set; }
    }
}
