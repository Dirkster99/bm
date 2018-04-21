namespace DirectoryInfoExLib.IO.Header.KnownFolder.Enums
{
    /// <summary>
    /// One of more values from the KF_DEFINITION_FLAGS
    /// enumeration that allow you to restrict redirection,
    /// allow PC-to-PC roaming, and control the time at which the
    /// known folder is created. Set to 0 if not needed.
    /// </summary>
    public enum KnownFolderDefinitionFlags
    {

        /// <summary>
        /// Prevent a per-user known folder from being redirected
        /// to a network location. Note that if the known folder has
        /// been flagged with KFDF_LOCAL_REDIRECT_ONLY but it is a
        /// subfolder of a known folder that is redirected to a network
        /// location, this subfolder is redirected also.
        /// </summary>
        LocalRedirectOnly = 0x2,

        /// <summary>
        /// Can be roamed through a PC-to-PC synchronization.
        /// </summary>
        Roamable = 0x4,

        /// <summary>
        /// Create the folder when the user first logs on.
        /// 
        /// Normally a known folder is not created until it is first called.
        /// At that time, an API such as SHCreateItemInKnownFolder or
        /// IKnownFolder::GetShellItem is called with the
        /// KF_FLAG_CREATE flag. However, some known folders need to exist
        /// immediately. An example is those known folders under
        /// %USERPROFILE%, which must exist to provide a proper view.
        /// In those cases, KFDF_PRECREATE is set and Windows Explorer
        /// calls the creation API during its user initialization.
        /// </summary>
        Precreate = 0x8
    }
}
