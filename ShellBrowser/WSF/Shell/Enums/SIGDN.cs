namespace WSF.Shell.Enums
{
    /// <summary>
    /// Requests the form of an item's display name to retrieve through
    /// IShellItem::GetDisplayName
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb761140(v=vs.85).aspx
    /// 
    /// and
    /// SHGetNameFromIDList.
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb762191(v=vs.85).aspx
    /// 
    /// Different forms of an item's name can be retrieved through the item's properties,
    /// including those listed here. Note that not all properties are present on all items,
    /// so only those appropriate to the item will appear. 
    /// 
    /// About SIGDN
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb762544(v=vs.85).aspx
    /// </summary>
    public enum SIGDN
    {
        /// <summary>
        /// Returns the display name relative to the parent folder.
        /// In UI this name is generally ideal for display to the user.
        /// </summary>
        NORMALDISPLAY = 0x00000000,

        /// <summary>
        /// Returns the parsing name relative to the parent folder.
        /// This name is not suitable for use in UI.
        /// </summary>
        ParentRelativeParsing = unchecked((int)0x80018001),   // SIGDN_INFOLDER | SIGDN_FORPARSING

        /// <summary>
        /// Returns the parsing name relative to the desktop.
        /// This name is not suitable for use in UI.
        /// </summary>
        DesktopAbsoluteParsing = unchecked((int)0x80028000),  // SIGDN_FORPARSING

        /// <summary>
        /// Returns the editing name relative to the parent folder.
        /// In UI this name is suitable for display to the user.
        /// </summary>
        ParentRelativeEditing = unchecked((int)0x80031001),   // SIGDN_INFOLDER | SIGDN_FOREDITING

        /// <summary>
        /// Returns the editing name relative to the desktop.
        /// In UI this name is suitable for display to the user.
        /// </summary>
        DesktopAbsoluteEditing = unchecked((int)0x8004c000),  // SIGDN_FORPARSING | SIGDN_FORADDRESSBAR

        /// <summary>
        /// Returns the item's file system path, if it has one.
        /// 
        /// Only items that report SFGAO_FILESYSTEM have a file system path.
        /// When an item does not have a file system path, a call
        /// to IShellItem::GetDisplayName on that item will fail.
        /// 
        /// In UI this name is suitable for display to the user in some cases,
        /// but note that it might not be specified for all items.
        /// </summary>
        FileSystemPath = unchecked((int)0x80058000),             // SIGDN_FORPARSING

        /// <summary>
        /// Returns the item's URL, if it has one. Some items do not have a URL,
        /// and in those cases a call to IShellItem::GetDisplayName will fail.
        /// 
        /// This name is suitable for display to the user in some cases,
        /// but note that it might not be specified for all items.
        /// </summary>
        Url = unchecked((int)0x80068000),                     // SIGDN_FORPARSING

        /// <summary>
        /// Returns the path relative to the parent folder in a friendly format as
        /// displayed in an address bar. This name is suitable for display to the user.
        /// </summary>
        ParentRelativeForAddressBar = unchecked((int)0x8007c001),     // SIGDN_INFOLDER | SIGDN_FORPARSING | SIGDN_FORADDRESSBAR

        /// <summary>
        /// Returns the path relative to the parent folder.
        /// </summary>
        ParentRelative = unchecked((int)0x80080001),          // SIGDN_INFOLDER

        /// <summary>
        /// Introduced in Windows 8.
        /// </summary>
        ParentRelativeForUI = unchecked((int)0x80094001)
    }
}
