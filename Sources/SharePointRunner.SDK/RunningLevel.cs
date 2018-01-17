using System.Collections.Generic;
using System.Linq;

namespace SharePointRunner.SDK
{
    /// <summary>
    /// Enumeration of different running levels
    /// </summary>
    public enum RunningLevelEnum
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
        File
    }

    /// <summary>
    /// Wrapper of enumeration of running levels, adding properties
    /// </summary>
    public class RunningLevel
    {
        /// <summary>
        /// Dictionary of RunningLevel by RunningLevelEnum
        /// </summary>
        public static readonly Dictionary<RunningLevelEnum, RunningLevel> Values = new List<RunningLevel>()
        {
            new RunningLevel() { RunningLevelEnum = RunningLevelEnum.Tenant, Children = new List<RunningLevelEnum>() { RunningLevelEnum.SiteCollection } },
            new RunningLevel() { RunningLevelEnum = RunningLevelEnum.SiteCollection, Children = new List<RunningLevelEnum>() { RunningLevelEnum.Site }},
            new RunningLevel() { RunningLevelEnum = RunningLevelEnum.Site, Children = new List<RunningLevelEnum>() { RunningLevelEnum.List }},
            new RunningLevel() { RunningLevelEnum = RunningLevelEnum.List, Children = new List<RunningLevelEnum>() { RunningLevelEnum.View, RunningLevelEnum.Folder, RunningLevelEnum.ListItem }},
            new RunningLevel() { RunningLevelEnum = RunningLevelEnum.View },
            new RunningLevel() { RunningLevelEnum = RunningLevelEnum.Folder, Children = new List<RunningLevelEnum>() { RunningLevelEnum.ListItem }},
            new RunningLevel() { RunningLevelEnum = RunningLevelEnum.ListItem, Children = new List<RunningLevelEnum>() { RunningLevelEnum.File }},
            new RunningLevel() { RunningLevelEnum = RunningLevelEnum.File }
        }.ToDictionary(r => r.RunningLevelEnum);

        /// <summary>
        /// Constructor
        /// </summary>
        private RunningLevel() { }

        /// <summary>
        /// Enumeration value of running level
        /// </summary>
        public RunningLevelEnum RunningLevelEnum { get; internal set; }

        /// <summary>
        /// List of next running levels to this current level
        /// </summary>
        public List<RunningLevelEnum> Children { get; internal set; } = new List<RunningLevelEnum>();

        /// <summary>
        /// Tenant level
        /// </summary>
        public static RunningLevel Tenant => Values[RunningLevelEnum.Tenant];

        /// <summary>
        /// Site collection level
        /// </summary>
        public static RunningLevel SiteCollection => Values[RunningLevelEnum.SiteCollection];

        /// <summary>
        /// Site level
        /// </summary>
        public static RunningLevel Site => Values[RunningLevelEnum.Site];

        /// <summary>
        /// List level
        /// </summary>
        public static RunningLevel List => Values[RunningLevelEnum.List];

        /// <summary>
        /// View level
        /// </summary>
        public static RunningLevel View => Values[RunningLevelEnum.View];

        /// <summary>
        /// Folder level
        /// </summary>
        public static RunningLevel Folder => Values[RunningLevelEnum.Folder];

        /// <summary>
        /// List item level
        /// </summary>
        public static RunningLevel ListItem => Values[RunningLevelEnum.ListItem];

        /// <summary>
        /// File Level
        /// </summary>
        public static RunningLevel File => Values[RunningLevelEnum.File];

        /// <summary>
        /// Know if the current running level has another running level to child level
        /// </summary>
        /// <param name="otherRunningLevel">Another running level</param>
        /// <returns>True if the other running is a child level of the current, False if not</returns>
        public bool HasChild(RunningLevel otherRunningLevel)
        {
            return Children.Contains(otherRunningLevel.RunningLevelEnum) || Children.Any(l => Values[l].HasChild(otherRunningLevel));
        }

        /// <summary>
        /// Override of ToString() method to display the ToString() of the enum value
        /// </summary>
        /// <returns>The string value</returns>
        public override string ToString()
        {
            return RunningLevelEnum.ToString();
        }

        /// <summary>
        /// Override of GetHeshCode() method
        /// </summary>
        /// <returns>The hash value</returns>
        public override int GetHashCode()
        {
            return RunningLevelEnum.GetHashCode();
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

            return RunningLevelEnum == otherRunningLevel.RunningLevelEnum;
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