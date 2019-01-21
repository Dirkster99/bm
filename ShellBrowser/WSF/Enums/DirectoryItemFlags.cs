namespace WSF.Enums
{
    /// <summary>
    /// Provides high level flags to identify specific types of a Windows Shell item.
    /// </summary>
    public enum DirectoryItemFlags : uint
    {
        /// <summary>
        /// Default option to use for initialization.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Indicates a special folder item that represents
        /// the root item in the shell browsing structure.
        /// </summary>
        DesktopRoot = 1024,

        /// <summary>
        /// This item is a Windows special folder.
        /// </summary>
        Special = 1,

        /// <summary>
        /// This item has a representation in the file system storage system.
        /// (eg. 'Documents')
        /// </summary>
        FileSystemDirectory = 2,

        /// <summary>
        /// This item has a representation in the file system storage system
        /// as a valid file that contains a sub-directroy structure (zip) in turn.
        /// 
        /// So, its a file (*.zip) but it can be browsed further as if it was a regular
        /// folder data structure on disk.
        /// </summary>
        DataFileContainer = 2048,

        /// <summary>
        /// This is a zip file or folder inside a zip data file container.
        /// </summary>
        DataFileContainerFolder = 4096,

        /// <summary>
        /// This item DOES NOT have a representation in the file system storage system.
        /// (eg. 'My PC')
        /// </summary>
        Virtual = 4,

        /// <summary>
        /// This item represents a drive in the file system.
        /// (eg. 'C:\')
        /// </summary>
        Drive = 8,

        /// <summary>
        /// This item represents the desktop special folder.
        /// </summary>
        Desktop = 16,

        /// <summary>
        /// Indicates the special folder Desktop item in the browsing structure.
        /// </summary>
        SpecialDesktopFileSystemDirectory = Desktop | Special | FileSystemDirectory,

        /// <summary>
        /// This item represents the documents special folder.
        /// </summary>
        Documents = 32,

        /// <summary>
        /// This item represents the Downloads special folder.
        /// </summary>
        Downloads = 64,

        /// <summary>
        /// This item represents the Music special folder.
        /// </summary>
        Music = 128,

        /// <summary>
        /// This item represents the Pictures special folder.
        /// </summary>
        Pictures = 256,

        /// <summary>
        /// This item represents the Videos special folder.
        /// </summary>
        Videos = 512
    }
}