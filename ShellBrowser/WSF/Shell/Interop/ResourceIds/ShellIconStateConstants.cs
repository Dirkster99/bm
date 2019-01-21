namespace WSF.Shell.Interop.ResourceIds
{
    using WSF.Shell.Enums;

    /// <summary>
    /// Flags specifying the state of the icon to draw from the Shell
    /// </summary>
    [System.Flags]
    public enum ShellIconStateConstants
    {
        /// <summary>
        /// Get icon in normal state
        /// </summary>
        ShellIconStateNormal = 0,

        /// <summary>
        /// Put a link overlay on icon 
        /// </summary>
        ShellIconStateLinkOverlay = SHGFI.SHGFI_LINKOVERLAY,

        /// <summary>
        /// show icon in selected state 
        /// </summary>
        ShellIconStateSelected = SHGFI.SHGFI_SELECTED,

        /// <summary>
        /// get open icon 
        /// </summary>
        ShellIconStateOpen = SHGFI.SHGFI_OPENICON,

        /// <summary>
        /// apply the appropriate overlays
        /// </summary>
        ShellIconAddOverlays = SHGFI.SHGFI_ADDOVERLAYS
    }
}
