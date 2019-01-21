namespace WSF.Shell.Interop.Interfaces.KnownFolders
{
    /// <summary>
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb762513(v=vs.85).aspx
    /// Specifies behaviors for known folders.
    /// </summary>
    [System.Flags]
    public enum DefinitionOptions
    {
        /// <summary>
        /// No behaviors are defined.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Prevents a per-user known folder from being 
        /// redirected to a network location.
        /// 
        /// Prevent a per-user known folder from being redirected to a network location.
        /// Note that if the known folder has been flagged with KFDF_LOCAL_REDIRECT_ONLY
        /// but it is a subfolder of a known folder that is redirected to a
        /// network location, this subfolder is redirected also.
        /// </summary>
        LocalRedirectOnly = 0x2,

        /// <summary>
        /// The known folder can be roamed through PC-to-PC synchronization.
        /// </summary>
        Roamable = 0x4,

        /// <summary>
        /// Creates the known folder when the user first logs on.
        /// 
        /// Create the folder when the user first logs on.
        /// 
        /// Normally a known folder is not created until it is first called.
        /// At that time, an API such as SHCreateItemInKnownFolder or IKnownFolder::GetShellItem
        /// is called with the KF_FLAG_CREATE flag. However, some known folders need to exist
        /// immediately. An example is those known folders under %USERPROFILE%, which must
        /// exist to provide a proper view. In those cases, KFDF_PRECREATE is set and Windows
        /// Explorer calls the creation API during its user initialization.
        /// </summary>
        Precreate = 0x8,

        /// <summary>
        /// Introduced in Windows 7. The known folder is a file rather than a folder.
        /// </summary>
        STREAM = 0x10,

        /// <summary>
        /// Introduced in Windows 7. The full path of the known folder,
        /// with any environment variables fully expanded, is stored in the registry
        /// under HKEY_CURRENT_USER.
        /// </summary>
        KFDF_PUBLISHEXPANDEDPATH = 0x20,

        /// <summary>
        /// Introduced in Windows 8.1. Prevent showing the Locations tab in the property
        /// dialog of the known folder. 
        /// </summary>
        KFDF_NO_REDIRECT_UI = 0x40
    }
}
