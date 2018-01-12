using System;
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

        public List<RunningLevel> Children { get; set; } = new List<RunningLevel>();

        private RunningLevel() { }

        public static RunningLevel Tenant => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.Tenant, Children = new List<RunningLevel>() { SiteCollection } };

        public static RunningLevel SiteCollection => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.SiteCollection, Children = new List<RunningLevel>() { Site } };

        public static RunningLevel Site => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.Site, Children = new List<RunningLevel>() { Site, List } };

        public static RunningLevel List => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.List, Children = new List<RunningLevel>() { View, Folder, ListItem } };

        public static RunningLevel View => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.View };

        public static RunningLevel Folder => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.Folder, Children = new List<RunningLevel>() { Folder, ListItem } };

        public static RunningLevel ListItem => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.ListItem, Children = new List<RunningLevel>() { File } };

        public static RunningLevel File => new RunningLevel() { RunningLevelEnum = RunningLevelEnum.File };

        public bool HasChild(RunningLevel otherRunningLevel)
        {
            return Children.Contains(otherRunningLevel) || Children.Any(l => l.HasChild(otherRunningLevel));
        }

        public bool Equals(RunningLevel otherRunningLevel)
        {
            if (ReferenceEquals(otherRunningLevel, null))
            {
                return false;
            }

            if (ReferenceEquals(this, otherRunningLevel))
            {
                return true;
            }
            
            return RunningLevelEnum == otherRunningLevel.RunningLevelEnum;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj);
        }

        // TODO V1 Override GetHashCode()

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
