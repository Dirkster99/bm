namespace DirectoryInfoExLib.IO.Header.ShellDll
{
    using System;
    using System.Text;
    using System.Runtime.InteropServices;

    /// <summary>
    /// This class contains every method, enumeration, struct and constants from
    /// the Windows API, which are required by the FileBrowser
    /// </summary>
    internal static class ShellAPI
    {
        #region Variables and Constants
        public const int MAX_PATH = 260;

        public const int S_OK = 0, S_FALSE = 1;
        #endregion

        #region DLL Import

        #region Shell32
        // Retrieves the IShellFolder interface for the desktop folder,
        // which is the root of the Shell's namespace. 
        [DllImport("shell32.dll")]
        public static extern Int32 SHGetDesktopFolder(
            out IntPtr ppshf);

        // Converts an item identifier list to a file system path
        [DllImport("shell32.dll")]
        public static extern bool SHGetPathFromIDList(
            IntPtr pidl,
            StringBuilder pszPath);

        // Tests whether two ITEMIDLIST structures are equal in a binary comparison
        [DllImport("shell32.dll",
            EntryPoint = "ILIsEqual", 
            ExactSpelling = true, 
            CharSet = CharSet.Ansi, 
            SetLastError = true)]
        public static extern bool ILIsEqual(
            IntPtr pidl1,
            IntPtr pidl2);

        #endregion

        #region ShlwAPI
        // Takes a STRRET structure returned by IShellFolder::GetDisplayNameOf,
        // converts it to a string, and places the result in a buffer. 
        [DllImport("shlwapi.dll", 
            EntryPoint = "StrRetToBuf", 
            ExactSpelling = false, 
            CharSet = CharSet.Auto, 
            SetLastError = true)]
        public static extern Int32 StrRetToBuf(
            IntPtr pstr, 
            IntPtr pidl,
            StringBuilder pszBuf, 
            int cchBuf);

        #endregion
        #endregion

        #region Shell CLSID GUIDs
        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/bb762494(VS.85).aspx
        ///
        /// Note  As of Windows Vista, these values have been replaced by KNOWNFOLDERID values.
        /// See that topic for a list of the new constants and their corresponding CSIDL values.
        /// For convenience, corresponding KNOWNFOLDERID values are also noted here for each CSIDL
        /// value.
        /// 
        /// The CSIDL system is supported under Windows Vista for compatibility reasons.
        /// However, new development should use KNOWNFOLDERID values rather than CSIDL values.
        ///
        /// CLSID is short for Class ID. CLSID is the identification of a COM object.
        /// Applications that support Microsoft's COM architecture register their objects as class IDs.
        /// https://www.codeproject.com/articles/14693/use-clsid-to-secure-folders-in-winxp
        /// </summary>
        internal static Guid IID_IShellFolder2 = new Guid("{93F2F68C-1D1B-11D3-A30E-00C04F79ABD1}");
        #endregion

        #region Enums
        /// <summary>
        /// Defines the values used with the IShellFolder::GetDisplayNameOf and IShellFolder::SetNameOf 
        /// methods to specify the type of file or folder names used by those methods
        /// </summary>
        [Flags]
        internal enum SHGNO
        {
            NORMAL = 0x0000,
            INFOLDER = 0x0001,
            FOREDITING = 0x1000,
            FORADDRESSBAR = 0x4000,
            FORPARSING = 0x8000
        }

        /// <summary>
        /// Enumeration specifies attributes that can be retrieved on an item
        /// (file or folder) or set of items.
        /// 
        /// The attributes that the caller is requesting, when calling IShellFolder::GetAttributesOf
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb762589(v=vs.85).aspx
        /// </summary>
        [Flags]
        internal enum SFGAO : uint
        {
            /// <summary>
            /// The specified items can be hosted inside a web browser or Windows Explorer frame.
            /// </summary>
            BROWSABLE = 0x8000000,
            CANCOPY = 1,
            CANDELETE = 0x20,
            CANLINK = 4,
            CANMONIKER = 0x400000,
            CANMOVE = 2,
            CANRENAME = 0x10,
            CAPABILITYMASK = 0x177,
            COMPRESSED = 0x4000000,
            CONTENTSMASK = 0x80000000,
            DISPLAYATTRMASK = 0xfc000,
            DROPTARGET = 0x100,
            ENCRYPTED = 0x2000,
            FILESYSANCESTOR = 0x10000000,

            /// <summary>
            /// The specified folders are either file system folders or contain at least one
            /// descendant (child, grandchild, or later) that is a file system (SFGAO_FILESYSTEM) folder.
            /// </summary>
            FILESYSTEM = 0x40000000,

            FOLDER = 0x20000000,
            GHOSTED = 0x8000,
            HASPROPSHEET = 0x40,
            HASSTORAGE = 0x400000,

            /// <summary>
            /// The specified folders have subfolders. The SFGAO_HASSUBFOLDER attribute is only advisory
            /// and might be returned by Shell folder implementations even if they do not contain subfolders.
            /// Note, however, that the converse—failing to return SFGAO_HASSUBFOLDER—definitively states
            /// that the folder objects do not have subfolders.
            ///
            /// Returning SFGAO_HASSUBFOLDER is recommended whenever a significant amount of time is required
            /// to determine whether any subfolders exist. For example, the Shell always returns
            /// SFGAO_HASSUBFOLDER when a folder is located on a network drive.
            /// </summary>
            HASSUBFOLDER = 0x80000000,
            HIDDEN = 0x80000,
            ISSLOW = 0x4000,
            LINK = 0x10000,
            NEWCONTENT = 0x200000,
            NONENUMERATED = 0x100000,
            READONLY = 0x40000,
            REMOVABLE = 0x2000000,
            SHARE = 0x20000,
            STORAGE = 8,
            STORAGEANCESTOR = 0x800000,
            STORAGECAPMASK = 0x70c50008,
            STREAM = 0x400000,
            VALIDATE = 0x1000000
        }

        /// <summary>
        /// Determines the type of items included in an enumeration. 
        /// These values are used with the IShellFolder::EnumObjects method
        /// </summary>
        [Flags]
        internal enum SHCONTF
        {
            FOLDERS = 0x0020,
            NONFOLDERS = 0x0040,
            INCLUDEHIDDEN = 0x0080,
            INIT_ON_FIRST_NEXT = 0x0100,
            NETPRINTERSRCH = 0x0200,
            SHAREABLE = 0x0400,
            STORAGE = 0x0800,
        }
        #endregion
    }
}