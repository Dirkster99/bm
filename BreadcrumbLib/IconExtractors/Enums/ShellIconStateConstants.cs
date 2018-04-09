namespace BreadcrumbLib.IconExtractors.Enums
{
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
        ShellIconStateLinkOverlay = 0x8000,

        /// <summary>
        /// show icon in selected state 
        /// </summary>
        ShellIconStateSelected = 0x10000,

        /// <summary>
        /// get open icon 
        /// </summary>
        ShellIconStateOpen = 0x2,

        /// <summary>
        /// apply the appropriate overlays
        /// </summary>
        ShellIconAddOverlays = 0x000000020,
    }
}
