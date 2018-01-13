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
            { RunningLevelEnum.Tenant, Tenant },
            { RunningLevelEnum.SiteCollection, SiteCollection },
            { RunningLevelEnum.Site, Site },
            { RunningLevelEnum.List, List },
            { RunningLevelEnum.View, View },
            { RunningLevelEnum.Folder, Folder },
            { RunningLevelEnum.ListItem, ListItem },
            { RunningLevelEnum.File, File },
        };

        public RunningLevelEnum RunningLevelEnum { get; internal set; }

        public List<RunningLevelEnum> Children { get; set; } = new List<RunningLevelEnum>();

        private RunningLevel() { }

        public static RunningLevel Tenant => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.Tenant, Children = new List<RunningLevelEnum>() { RunningLevelEnum.SiteCollection } };

        public static RunningLevel SiteCollection => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.SiteCollection, Children = new List<RunningLevelEnum>() { RunningLevelEnum.Site } };

        public static RunningLevel Site => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.Site, Children = new List<RunningLevelEnum>() { RunningLevelEnum.Site, RunningLevelEnum.List } };

        public static RunningLevel List => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.List, Children = new List<RunningLevelEnum>() { RunningLevelEnum.View, RunningLevelEnum.Folder, RunningLevelEnum.ListItem } };

        public static RunningLevel View => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.View };

        public static RunningLevel Folder => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.Folder, Children = new List<RunningLevelEnum>() { RunningLevelEnum.Folder, RunningLevelEnum.ListItem } };

        public static RunningLevel ListItem => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.ListItem, Children = new List<RunningLevelEnum>() { RunningLevelEnum.File } };

        public static RunningLevel File => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.File };

        public bool HasChild(RunningLevel otherRunningLevel)
        {
            return Children.Contains(otherRunningLevel.RunningLevelEnum) || Children.Any(l => Values[l].HasChild(otherRunningLevel));
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
