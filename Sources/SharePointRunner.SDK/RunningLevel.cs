using System.Collections.Generic;
using System.Linq;

namespace SharePointRunner.SDK
{
    /// <summary>
    /// Enumeration of different running levels
    /// </summary>
    public enum BaseRunningLevel
    {
        /// <summary>
        /// Tenant level
        /// </summary>
        Tenant,

        /// <summary>
        /// Site collection level
        /// </summary>
        SiteCollection,

        /// <summary>
        /// Site level
        /// </summary>
        Site,

        /// <summary>
        /// List level
        /// </summary>
        List,

        /// <summary>
        /// View Level
        /// </summary>
        View,

        /// <summary>
        /// Folder Level
        /// </summary>
        Folder,

        /// <summary>
        /// List item level
        /// </summary>
        ListItem,

        /// <summary>
        /// File level
        /// </summary>
        File,

        /// <summary>
        /// Term store level
        /// </summary>
        TermStore,

        /// <summary>
        /// Term group level
        /// </summary>
        TermGroup,

        /// <summary>
        /// Term set level
        /// </summary>
        TermSet,

        /// <summary>
        /// Term
        /// </summary>
        Term
    }

    /// <summary>
    /// Wrapper of enumeration of running levels, adding properties
    /// </summary>
    public class RunningLevel
    {
        /// <summary>
        /// Dictionary of RunningLevel by RunningLevelEnum
        /// </summary>
        public static readonly Dictionary<BaseRunningLevel, RunningLevel> Values = new List<RunningLevel>()
        {
            new RunningLevel() { BaseRunningLevel = BaseRunningLevel.Tenant, Children = new List<BaseRunningLevel>() { BaseRunningLevel.SiteCollection, BaseRunningLevel.TermStore } },
            new RunningLevel() { BaseRunningLevel = BaseRunningLevel.SiteCollection, Children = new List<BaseRunningLevel>() { BaseRunningLevel.Site }},
            new RunningLevel() { BaseRunningLevel = BaseRunningLevel.Site, Children = new List<BaseRunningLevel>() { BaseRunningLevel.List }},
            new RunningLevel() { BaseRunningLevel = BaseRunningLevel.List, Children = new List<BaseRunningLevel>() { BaseRunningLevel.View, BaseRunningLevel.Folder, BaseRunningLevel.ListItem }},
            new RunningLevel() { BaseRunningLevel = BaseRunningLevel.View },
            new RunningLevel() { BaseRunningLevel = BaseRunningLevel.Folder, Children = new List<BaseRunningLevel>() { BaseRunningLevel.ListItem }},
            new RunningLevel() { BaseRunningLevel = BaseRunningLevel.ListItem, Children = new List<BaseRunningLevel>() { BaseRunningLevel.File }},
            new RunningLevel() { BaseRunningLevel = BaseRunningLevel.File },
            new RunningLevel() { BaseRunningLevel = BaseRunningLevel.TermStore, Children = new List<BaseRunningLevel>() { BaseRunningLevel.TermGroup } },
            new RunningLevel() { BaseRunningLevel = BaseRunningLevel.TermGroup, Children = new List<BaseRunningLevel>() { BaseRunningLevel.TermSet } },
            new RunningLevel() { BaseRunningLevel = BaseRunningLevel.TermSet, Children = new List<BaseRunningLevel>() { BaseRunningLevel.Term } },
            new RunningLevel() { BaseRunningLevel = BaseRunningLevel.Term }
        }.ToDictionary(r => r.BaseRunningLevel);

        /// <summary>
        /// Constructor
        /// </summary>
        private RunningLevel() { }

        /// <summary>
        /// Enumeration value of running level
        /// </summary>
        public BaseRunningLevel BaseRunningLevel { get; internal set; }

        /// <summary>
        /// List of next running levels to this current level
        /// </summary>
        public List<BaseRunningLevel> Children { get; internal set; } = new List<BaseRunningLevel>();

        /// <summary>
        /// Tenant level
        /// </summary>
        public static RunningLevel Tenant => Values[BaseRunningLevel.Tenant];

        /// <summary>
        /// Site collection level
        /// </summary>
        public static RunningLevel SiteCollection => Values[BaseRunningLevel.SiteCollection];

        /// <summary>
        /// Site level
        /// </summary>
        public static RunningLevel Site => Values[BaseRunningLevel.Site];

        /// <summary>
        /// List level
        /// </summary>
        public static RunningLevel List => Values[BaseRunningLevel.List];

        /// <summary>
        /// View level
        /// </summary>
        public static RunningLevel View => Values[BaseRunningLevel.View];

        /// <summary>
        /// Folder level
        /// </summary>
        public static RunningLevel Folder => Values[BaseRunningLevel.Folder];

        /// <summary>
        /// List item level
        /// </summary>
        public static RunningLevel ListItem => Values[BaseRunningLevel.ListItem];

        /// <summary>
        /// File Level
        /// </summary>
        public static RunningLevel File => Values[BaseRunningLevel.File];

        /// <summary>
        /// Term store level
        /// </summary>
        public static RunningLevel TermStore => Values[BaseRunningLevel.TermStore];

        /// <summary>
        /// Term group level
        /// </summary>
        public static RunningLevel TermGroup => Values[BaseRunningLevel.TermGroup];

        /// <summary>
        /// Term set level
        /// </summary>
        public static RunningLevel TermSet => Values[BaseRunningLevel.TermSet];

        /// <summary>
        /// Term
        /// </summary>
        public static RunningLevel Term => Values[BaseRunningLevel.Term];

        /// <summary>
        /// Know if the current running level has another running level to child level
        /// </summary>
        /// <param name="otherRunningLevel">Another running level</param>
        /// <returns>True if the other running is a child level of the current, False if not</returns>
        public bool HasChild(RunningLevel otherRunningLevel)
        {
            return Children.Contains(otherRunningLevel.BaseRunningLevel) || Children.Any(l => Values[l].HasChild(otherRunningLevel));
        }

        /// <summary>
        /// Override of ToString() method to display the ToString() of the enum value
        /// </summary>
        /// <returns>The string value</returns>
        public override string ToString()
        {
            return BaseRunningLevel.ToString();
        }

        /// <summary>
        /// Override of GetHeshCode() method
        /// </summary>
        /// <returns>The hash value</returns>
        public override int GetHashCode()
        {
            return BaseRunningLevel.GetHashCode();
        }

        /// <summary>
        /// Override of Equals() method to compare enum value of both objects
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>True if the objects have the same running level enum value, False if not</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            RunningLevel otherRunningLevel = (RunningLevel)obj;

            return BaseRunningLevel == otherRunningLevel.BaseRunningLevel;
        }

        /// <summary>
        /// Override of == operator
        /// </summary>
        /// <param name="r1">First object to compare</param>
        /// <param name="r2">Second object to compare</param>
        /// <returns>True if both running levels are equal, False if not</returns>
        public static bool operator ==(RunningLevel r1, RunningLevel r2)
        {
            return r1.Equals(r2);
        }

        /// <summary>
        /// Override of != operator
        /// </summary>
        /// <param name="r1">First object to compare</param>
        /// <param name="r2">Second object to compare</param>
        /// <returns>True if both running levels are not equal, False if not</returns>
        public static bool operator !=(RunningLevel r1, RunningLevel r2)
        {
            return !r1.Equals(r2);
        }

        /// <summary>
        /// Override of &lt; operator
        /// </summary>
        /// <param name="r1">First object to compare</param>
        /// <param name="r2">Second object to compare</param>
        /// <returns>True if the first running level is at a lower level to the second, False if not</returns>
        public static bool operator <(RunningLevel r1, RunningLevel r2)
        {
            return r1 != r2 && !(r1 > r2);
        }

        /// <summary>
        /// Override of &gt; operator
        /// </summary>
        /// <param name="r1">First object to compare</param>
        /// <param name="r2">Second object to compare</param>
        /// <returns>True if the first running level is at a greater level to the second, False if not</returns>
        public static bool operator >(RunningLevel r1, RunningLevel r2)
        {
            return r1 != r2 && r1.HasChild(r2);
        }

        /// <summary>
        /// Override of &lt;= operator
        /// </summary>
        /// <param name="r1">First object to compare</param>
        /// <param name="r2">Second object to compare</param>
        /// <returns>True if the first running level is at a lower level or equal to the second, False if not</returns>
        public static bool operator <=(RunningLevel r1, RunningLevel r2)
        {
            return (r1 < r2) || (r1 == r2);
        }

        /// <summary>
        /// Override of &gt;= operator
        /// </summary>
        /// <param name="r1">First object to compare</param>
        /// <param name="r2">Second object to compare</param>
        /// <returns>True if the first running level is at a greater level or equal to the second, False if not</returns>
        public static bool operator >=(RunningLevel r1, RunningLevel r2)
        {
            return (r1 > r2) || (r1 == r2);
        }
    }
}