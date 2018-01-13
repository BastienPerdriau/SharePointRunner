using System.Collections.Generic;
using System.Linq;

namespace SharePointRunner.SDK
{
    public enum RunningLevelEnum
    {
        Tenant,
        SiteCollection,
        Site,
        List,
        View,
        Folder,
        ListItem,
        File
    }

    public class RunningLevel
    {
        public static readonly Dictionary<RunningLevelEnum, RunningLevel> Values = new Dictionary<RunningLevelEnum, RunningLevel>()
        {
            { RunningLevelEnum.Tenant, new RunningLevel() { RunningLevelEnum = RunningLevelEnum.Tenant, Children = new List<RunningLevelEnum>() { RunningLevelEnum.SiteCollection } } },
            { RunningLevelEnum.SiteCollection, new RunningLevel() { RunningLevelEnum = RunningLevelEnum.SiteCollection, Children = new List<RunningLevelEnum>() { RunningLevelEnum.Site }} },
            { RunningLevelEnum.Site, new RunningLevel() { RunningLevelEnum = RunningLevelEnum.Site, Recursive = true, Children = new List<RunningLevelEnum>() { RunningLevelEnum.List }} },
            { RunningLevelEnum.List, new RunningLevel() { RunningLevelEnum = RunningLevelEnum.List, Children = new List<RunningLevelEnum>() { RunningLevelEnum.View, RunningLevelEnum.Folder, RunningLevelEnum.ListItem }} },
            { RunningLevelEnum.View, new RunningLevel() { RunningLevelEnum = RunningLevelEnum.View } },
            { RunningLevelEnum.Folder, new RunningLevel() { RunningLevelEnum = RunningLevelEnum.Folder, Recursive = true, Children = new List<RunningLevelEnum>() { RunningLevelEnum.ListItem }} },
            { RunningLevelEnum.ListItem, new RunningLevel() { RunningLevelEnum = RunningLevelEnum.ListItem, Children = new List<RunningLevelEnum>() { RunningLevelEnum.File }} },
            { RunningLevelEnum.File, new RunningLevel() { RunningLevelEnum = RunningLevelEnum.File } }
        };

        public RunningLevelEnum RunningLevelEnum { get; internal set; }

        public bool Recursive { get; internal set; } = false;

        public List<RunningLevelEnum> Children { get; internal set; } = new List<RunningLevelEnum>();

        public RunningLevel() { }

        public static RunningLevel Tenant => Values[RunningLevelEnum.Tenant];

        public static RunningLevel SiteCollection => Values[RunningLevelEnum.SiteCollection];

        public static RunningLevel Site => Values[RunningLevelEnum.Site];

        public static RunningLevel List => Values[RunningLevelEnum.List];

        public static RunningLevel View => Values[RunningLevelEnum.View];

        public static RunningLevel Folder => Values[RunningLevelEnum.Folder];

        public static RunningLevel ListItem => Values[RunningLevelEnum.ListItem];

        public static RunningLevel File => Values[RunningLevelEnum.File];

        public bool HasChild(RunningLevel otherRunningLevel)
        {
            return Children.Contains(otherRunningLevel.RunningLevelEnum) || Children.Any(l => Values[l].HasChild(otherRunningLevel));
        }

        public override string ToString()
        {
            return RunningLevelEnum.ToString();
        }

        public override int GetHashCode()
        {
            return RunningLevelEnum.GetHashCode();
        }

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

        public static bool operator ==(RunningLevel r1, RunningLevel r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(RunningLevel r1, RunningLevel r2)
        {
            return !r1.Equals(r2);
        }

        public static bool operator <(RunningLevel r1, RunningLevel r2)
        {
            return r1 != r2 && !(r1 > r2);
        }

        public static bool operator >(RunningLevel r1, RunningLevel r2)
        {
            return r1 != r2 && r1.HasChild(r2);
        }

        public static bool operator <=(RunningLevel r1, RunningLevel r2)
        {
            return (r1 < r2) || (r1 == r2);
        }

        public static bool operator >=(RunningLevel r1, RunningLevel r2)
        {
            return (r1 > r2) || (r1 == r2);
        }
    }
}