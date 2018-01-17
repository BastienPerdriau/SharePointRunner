using Xunit;

namespace SharePointRunner.SDK.Tests
{
    /// <summary>
    /// Test class for RunningLevel class
    /// </summary>
    public class RunningLevelTests
    {
        #region Enumeration values
        [Fact]
        public void TenantRunningLevelTest()
        {
            Assert.Equal(BaseRunningLevel.Tenant, RunningLevel.Tenant.BaseRunningLevel);
        }

        [Fact]
        public void SiteCollectionRunningLevelTest()
        {
            Assert.Equal(BaseRunningLevel.SiteCollection, RunningLevel.SiteCollection.BaseRunningLevel);
        }

        [Fact]
        public void SiteRunningLevelTest()
        {
            Assert.Equal(BaseRunningLevel.Site, RunningLevel.Site.BaseRunningLevel);
        }

        [Fact]
        public void ListRunningLevelTest()
        {
            Assert.Equal(BaseRunningLevel.List, RunningLevel.List.BaseRunningLevel);
        }

        [Fact]
        public void ViewRunningLevelTest()
        {
            Assert.Equal(BaseRunningLevel.View, RunningLevel.View.BaseRunningLevel);
        }

        [Fact]
        public void FolderRunningLevelTest()
        {
            Assert.Equal(BaseRunningLevel.Folder, RunningLevel.Folder.BaseRunningLevel);
        }

        [Fact]
        public void ListItemRunningLevelTest()
        {
            Assert.Equal(BaseRunningLevel.ListItem, RunningLevel.ListItem.BaseRunningLevel);
        }

        [Fact]
        public void FileRunningLevelTest()
        {
            Assert.Equal(BaseRunningLevel.File, RunningLevel.File.BaseRunningLevel);
        }
        #endregion

        #region HasChild()
        [Fact]
        public void TenantHasChildListItemTest()
        {
            Assert.True(RunningLevel.Tenant.HasChild(RunningLevel.ListItem));
        }

        [Fact]
        public void ListItemHasChildTenantTest()
        {
            Assert.False(RunningLevel.ListItem.HasChild(RunningLevel.Tenant));
        }

        [Fact]
        public void TenantHasChildTenantTest()
        {
            Assert.False(RunningLevel.Tenant.HasChild(RunningLevel.Tenant));
        }
        #endregion

        #region ToString()
        [Fact]
        public void ToStringTest()
        {
            Assert.Equal(BaseRunningLevel.Tenant.ToString(), RunningLevel.Tenant.BaseRunningLevel.ToString());
        }
        #endregion

        #region GetHashCode()
        [Fact]
        public void GetHashCodeTest()
        {
            Assert.Equal(RunningLevel.List.GetHashCode(), RunningLevel.List.GetHashCode());
        }

        [Fact]
        public void GetHashCodeOtherTest()
        {
            Assert.NotEqual(RunningLevel.List.GetHashCode(), RunningLevel.View.GetHashCode());
        }
        #endregion

        #region Equals()
        [Fact]
        public void EqualsTest()
        {
            Assert.True(RunningLevel.List.Equals(RunningLevel.List));
        }

        [Fact]
        public void NotEqualsTest()
        {
            Assert.False(RunningLevel.List.Equals(RunningLevel.View));
        }

        [Fact]
        public void OtherTypeNotEqualsTest()
        {
            Assert.False(RunningLevel.List.Equals(string.Empty));
        }

        [Fact]
        public void NullTypeNotEqualsTest()
        {
            Assert.False(RunningLevel.List.Equals(null));
        }
        #endregion

        #region Operators
        [Fact]
        public void EqualOperatorTest()
        {

        }
        
        // TODO Tests oprator overrides
        #endregion
    }
}
