namespace DirectoryInfoExLib.IO.Header.KnownFolder.Enums
{
    /// <summary>
    /// Specifies the precision of the match of path and known folder
    /// when sreaching for known folders based on a string path.
    /// </summary>
    public enum KnownFolderFindMode : int
    {
        /// <summary>
        /// Retrieve only the specific known folder for the given file path.
        /// </summary>
        ExactMatch = 0,

        /// <summary>
        /// If an exact match is not found for the given file path,
        /// retrieve the first known folder that matches one of its
        /// parent folders walking up the parent tree.
        /// </summary>
        NearestParentMatch = ExactMatch + 1
    };

}
