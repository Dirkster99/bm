namespace WSF.Shell.Interop.ResourceIds
{
    /// <summary>
    /// Gets the size of a Windows Shell icon.
    /// </summary>
    internal enum IconSize : int
    {
        /// <summary>
        /// Get a small icon
        /// </summary>
        small = 0x1,

        /// <summary>
        /// Get a large icon
        /// </summary>
        large = 0x0,

        /// <summary>
        /// Get a very large icon
        /// </summary>
        extraLarge = 0x2,

        /// <summary>
        /// Get a extra large icon
        /// </summary>
        jumbo = 0x4,

        /// <summary>
        /// Get a thumbnail sized icon
        /// </summary>
        thumbnail = 0x5
    }
}
