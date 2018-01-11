namespace SharePointRunner.SDK
{
    // TODO V1 Re-think the whole mecanism of running levels
    /// <summary>
    /// Enumeration of levels of running SharePoint
    /// </summary>
    public enum RunningLevel
    {
        Tenant = 0,

        SiteCollection = 1,

        Site = 2,

        List = 3,

        Folder = 4,

        ListItem = 5,

        File = 6
    }
}
