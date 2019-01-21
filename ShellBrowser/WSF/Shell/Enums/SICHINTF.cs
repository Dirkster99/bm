namespace WSF.Shell.Enums
{
    /// <summary>
    /// Used to determine how to compare two Shell items.
    /// IShellItem::Compare uses this enumerated type.
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb761136(v=vs.85).aspx
    /// 
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb762543(v=vs.85).aspx
    /// </summary>
    public enum SICHINTF
    {
        /// <summary>
        /// This relates to the iOrder parameter of the IShellItem::Compare interface
        /// and indicates that the comparison is based on the display in a folder view.
        /// </summary>
        SICHINT_DISPLAY = 0x00000000,

        /// <summary>
        /// Exact comparison of two instances of a Shell item.
        /// </summary>
        SICHINT_ALLFIELDS = unchecked((int)0x80000000),

        /// <summary>
        /// This relates to the iOrder parameter of the IShellItem::Compare interface and
        /// indicates that the comparison is based on a canonical name.
        /// </summary>
        SICHINT_CANONICAL = 0x10000000,

        /// <summary>
        /// Windows 7 and later. If the Shell items are not the same, test the file system paths.
        /// </summary>
        SICHINT_TEST_FILESYSPATH_IF_NOT_EQUAL = 0x20000000,
    }
}