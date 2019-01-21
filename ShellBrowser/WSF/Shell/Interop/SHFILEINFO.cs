namespace WSF.Shell.Interop
{
    using System;
    using System.Runtime.InteropServices;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb759792(v=vs.85).aspx
    /// Contains information about a file object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        /// <summary>
        /// A handle to the icon that represents the file. You are responsible for destroying this handle with DestroyIcon when you no longer need it.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
        public IntPtr hIcon;

        /// <summary>
        /// The index of the icon image within the system image list.
        /// </summary>
        public int iIcon;

        /// <summary>
        /// An array of values that indicates the attributes of the file object. For information about these values, see the IShellFolder::GetAttributesOf method.
        /// </summary>
        public uint dwAttributes;

        /// <summary>
        /// A string that contains the name of the file as it appears in the Windows Shell,
        /// or the path and file name of the file that contains the icon representing the file.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] // MAX_PATH
        public string szDisplayName;

        /// <summary>
        /// A string that describes the type of file.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    // ReSharper restore InconsistentNaming
}