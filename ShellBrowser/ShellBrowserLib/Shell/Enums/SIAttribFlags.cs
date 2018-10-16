namespace ShellBrowserLib.Shell.Enums
{
    /// <summary>
    /// IShellItemArray::GetAttributes
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb761096(v=vs.85).aspx
    /// 
    /// Gets the attributes of the set of items contained in an IShellItemArray.
    /// If the array contains more than one item, the attributes retrieved by this
    /// method are not the attributes of single items, but a logical combination of
    /// all of the requested attributes of all of the items.
    /// </summary>
    internal enum SIAttribFlags
    {
        /// <summary>
        /// if multiple items and the attirbutes together.
        /// </summary>
        And = 0x00000001,

        /// <summary>
        /// if multiple items or the attributes together.
        /// </summary>
        Or = 0x00000002,

        /// <summary>
        /// Call GetAttributes directly on the ShellFolder for multiple attributes.
        /// </summary>
        AppCompat = 0x00000003,

        /// <summary>
        /// A mask for SIATTRIBFLAGS_AND, SIATTRIBFLAGS_OR, and SIATTRIBFLAGS_APPCOMPAT.
        /// Callers normally do not use this value.
        /// </summary>
        Mask = 0x00000003,

        /// <summary>
        /// Windows 7 and later. Examine all items in the array to compute the attributes. 
        /// Note that this can result in poor performance over large arrays and therefore it 
        /// should be used only when needed. Cases in which you pass this flag should be extremely rare.
        /// </summary>
        AllItems = 0x00004000
    }
}
