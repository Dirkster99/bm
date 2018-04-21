namespace DirectoryInfoExLib.IO.Header.KnownFolder.Enums
{
    /// <summary>
    /// Specify special retrieval options for known folders.
    /// These values supersede CSIDL values, which have parallel meanings.
    /// </summary>
    public enum KnownFolderRetrievalOptions
    {
        /// <summary>
        /// 0x00008000. Forces the creation of the specified folder
        /// if that folder does not already exist. The security provisions
        /// predefined for that folder are applied. If the folder does not
        /// exist and cannot be created, the function returns a failure code
        /// and no path is returned. This value can be used only with the
        /// following functions and methods:
        /// 
        /// SHGetKnownFolderPath
        /// SHGetKnownFolderIDList
        /// IKnownFolder::GetIDList
        /// IKnownFolder::GetPath
        /// IKnownFolder::GetShellItem
        /// </summary>
        Create = 0x00008000,

        /// <summary>
        /// 
        /// 0x00004000. Do not verify the folder's existence before
        /// attempting to retrieve the path or IDList. If this flag
        /// is not set, an attempt is made to verify that the folder
        /// is truly present at the path. If that verification fails
        /// due to the folder being absent or inaccessible, the function
        /// returns a failure code and no path is returned.
        /// 
        /// If the folder is located on a network, the function might
        /// take a longer time to execute. Setting this flag can reduce
        /// that lag time.
        /// </summary>
        DontVerify = 0x00004000,

        /// <summary>
        /// 0x00002000. Stores the full path in the registry without
        /// using environment strings. If this flag is not set, portions
        /// of the path may be represented by environment strings
        /// such as %USERPROFILE%. This flag can only be used
        /// with
        /// SHSetKnownFolderPath and
        /// IKnownFolder::SetPath.:SetPath.
        /// </summary>
        DontUnexpand = 0x00002000,

        /// <summary>
        /// 0x00001000. Gets the true system path for the folder,
        /// free of any aliased placeholders such as %USERPROFILE%,
        /// returned by SHGetKnownFolderIDList and IKnownFolder::GetIDList.
        /// 
        /// This flag has no effect on paths returned by SHGetKnownFolderPath
        /// and IKnownFolder::GetPath.
        /// 
        /// By default, known folder retrieval functions and methods
        /// return the aliased path if an alias exists.
        /// </summary>
        NoAlias = 0x00001000,

        /// <summary>
        /// 0x00000800. Initializes the folder using its Desktop.ini
        /// settings.If the folder cannot be initialized, the function
        /// returns a failure code and no path is returned. This flag
        /// should always be combined with KF_FLAG_CREATE.
        /// 
        /// If the folder is located on a network, the function might
        /// take a longer time to execute.
        /// </summary>
        Init = 0x00000800,

        /// <summary>
        /// 0x00000400. Gets the default path for a known folder.
        /// If this flag is not set, the function retrieves the
        /// current—and possibly redirected—path of the folder.
        /// The execution of this flag includes a verification of
        /// the folder's existence unless KF_FLAG_DONT_VERIFY is set.
        /// </summary>
        DefaultPath = 0x00000400,

        /// <summary>
        /// 0x00000200. Gets the folder's default path independent
        /// of the current location of its parent.
        /// 
        /// KF_FLAG_DEFAULT_PATH must also be set.
        /// </summary>
        NotParentRelative = 0x00000200
    }
}
