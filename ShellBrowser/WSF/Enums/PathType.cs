namespace WSF.Enums
{
    /// <summary>
    /// Defines a type of path based on the way it is stated in a string.
    /// </summary>
    public enum PathType
    {
        /// <summary>
        /// The type of path cannot be determined by method that returns this value.
        /// </summary>
        Unknown,

        /// <summary>
        /// The path is based on a sequence of localized named items.
        /// Eg:
        /// 'Libraries\Documents'    in English
        /// 'Bibleotheken\Dokumente' in German
        /// </summary>
        WinShellPath,

        /// <summary>
        /// The path is based on a SpecialFolder id
        /// Eg: '::{0AC0837C-BBF8-452A-850D-79D08E667CA7}' <seealso cref="WSF.IDs.KF_IID"/>
        /// </summary>
        SpecialFolder,

        /// <summary>
        /// The path is based on a drive or UNC based reference.
        /// Eg: 'C:\Windows\' or '\\MyServer\MyShare\'
        /// </summary>
        FileSystemPath
    };
}
