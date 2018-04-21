namespace DirectoryInfoExLib.IO.Header.KnownFolder.Enums
{
    /// <summary>
    /// Defines values that state whether the known folder
    /// can have its path set to a new value or what specific restrictions
    /// or prohibitions are placed on that redirection.
    /// </summary>
    public enum KnownFolderRedirectionCapabilities
    {
        /// <summary>
        /// The folder can be redirected if any of the bits in the
        /// lower byte of the value are set but no DENY flag is set.
        /// DENY flags are found in the upper byte of the value.
        /// </summary>
        AllowAll = 0xff,

        /// <summary>
        /// The folder can be redirected. Currently, redirection exists
        /// for only common and user folders; fixed and virtual folders
        /// cannot be redirected.
        /// </summary>
        Redirectable = 0x1,

        /// <summary>
        /// Redirection is not allowed.
        /// </summary>
        DenyAll = 0xfff00,

        /// <summary>
        /// The folder cannot be redirected
        /// because it is already redirected by group policy.
        /// </summary>
        DenyPolicyRedirected = 0x100,

        /// <summary>
        /// The folder cannot be redirected
        /// because the policy prohibits redirecting this folder.
        /// </summary>
        DenyPolicy = 0x200,

        /// <summary>
        /// The folder cannot be redirected
        /// because the calling application does
        /// not have sufficient permissions.
        /// </summary>
        DenyPermissions = 0x400
    }
}
