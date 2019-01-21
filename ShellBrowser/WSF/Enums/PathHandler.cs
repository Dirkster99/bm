namespace WSF.Enums
{
    /// <summary>
    /// Defines a type of path handler that indicates the
    /// handling object that should be used to manipulate
    /// this item and its children.
    /// </summary>
    public enum PathHandler
    {
        /// <summary>
        /// There is no path handler available for handling
        /// an item that was tagged with this value.
        /// 
        /// Most likely processing the item or retrieving its
        /// children is not going to be succesful.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// A given path appeared to be invalid or
        /// follows no known convention.
        /// </summary>
        InvalidPath = 1,

        /// <summary>
        /// An item tagged with this value has an associated storage location
        /// in the file system and could therefore, be processed using file system
        /// tools or libraries...
        /// </summary>
        FileSystem = 2,

        /// <summary>
        /// This item is the desktop root item which is virtual and
        /// has ALL other items in its list of child entries.
        /// </summary>
        DesktopRoot = 4,

        /// <summary>
        /// An item tagged with this value has an associated storage location
        /// in a zip file with in the file system and could therefore,
        /// be processed using file system ZIP tools or libraries...
        /// </summary>
        ZipFileSystem = 8
    }
}
