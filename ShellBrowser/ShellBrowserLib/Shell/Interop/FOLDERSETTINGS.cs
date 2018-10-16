namespace ShellBrowserLib.SharpShell.Interop
{
    using ShellBrowserLib.Shell.Enums;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains folder view information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FOLDERSETTINGS
    {
        /// <summary>
        /// Folder view mode. One of the FOLDERVIEWMODE values.
        /// </summary>
        public FOLDERVIEWMODE ViewMode;

        /// <summary>
        /// A set of flags that indicate the options for the folder. This can be zero or a combination of the FOLDERFLAGS values.
        /// </summary>
        public FOLDERFLAGS fFlags;
    }
}