namespace DirectoryInfoExLib.IO.Header.ShellDll
{
    using System;
    using System.Text;
    using System.Runtime.InteropServices;

    /// <summary>
    /// This class contains every method, enumeration, struct and constants from
    /// the Windows API, which are required by the FileBrowser
    /// </summary>
    internal static partial class ShellAPI
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
        /// Flags to specify which path is to be returned with SHGetFolderPath
        /// </summary>
        [Flags]
        internal enum SHGFP
        {
            TYPE_CURRENT = 0,
            TYPE_DEFAULT = 1
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

        // Determines the type of items included in an enumeration. 
        // These values are used with the IShellFolder::EnumObjects method
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

        // Specifies how the shortcut menu can be changed when calling IContextMenu::QueryContextMenu
        [Flags]
        internal enum CMF : uint
        {
            NORMAL = 0x00000000,
            DEFAULTONLY = 0x00000001,
            VERBSONLY = 0x00000002,
            EXPLORE = 0x00000004,
            NOVERBS = 0x00000008,
            CANRENAME = 0x00000010,
            NODEFAULT = 0x00000020,
            INCLUDESTATIC = 0x00000040,
            EXTENDEDVERBS = 0x00000100,
            RESERVED = 0xffff0000
        }

        // Flags specifying the information to return when calling IContextMenu::GetCommandString
        [Flags]
        internal enum GCS : uint
        {
            VERBA = 0,
            HELPTEXTA = 1,
            VALIDATEA = 2,
            VERBW = 4,
            HELPTEXTW = 5,
            VALIDATEW = 6
        }

        // Flags that specify the file information to retrieve with SHGetFileInfo
        [Flags]
        internal enum SHGFI : uint
        {
            ADDOVERLAYS = 0x20,
            ATTR_SPECIFIED = 0x20000,
            ATTRIBUTES = 0x800,
            DISPLAYNAME = 0x200,
            EXETYPE = 0x2000,
            ICON = 0x100,
            ICONLOCATION = 0x1000,
            LARGEICON = 0,
            LINKOVERLAY = 0x8000,
            OPENICON = 2,
            OVERLAYINDEX = 0x40,
            PIDL = 8,
            SELECTED = 0x10000,
            SHELLICONSIZE = 4,
            SMALLICON = 1,
            SYSICONINDEX = 0x4000,
            TYPENAME = 0x400,
            USEFILEATTRIBUTES = 0x10
        }

        // Flags that specify the file information to retrieve with SHGetFileInfo
        [Flags]
        internal enum FILE_ATTRIBUTE
        {
            READONLY = 0x00000001,
            HIDDEN = 0x00000002,
            SYSTEM = 0x00000004,
            DIRECTORY = 0x00000010,
            ARCHIVE = 0x00000020,
            DEVICE = 0x00000040,
            NORMAL = 0x00000080,
            TEMPORARY = 0x00000100,
            SPARSE_FILE = 0x00000200,
            REPARSE_POINT = 0x00000400,
            COMPRESSED = 0x00000800,
            OFFLINE = 0x00001000,
            NOT_CONTENT_INDEXED = 0x00002000,
            ENCRYPTED = 0x00004000
        }

        // Specifies how TrackPopupMenuEx positions the shortcut menu horizontally
        [Flags]
        internal enum TPM : uint
        {
            LEFTBUTTON = 0x0000,
            RIGHTBUTTON = 0x0002,
            LEFTALIGN = 0x0000,
            CENTERALIGN = 0x0004,
            RIGHTALIGN = 0x0008,
            TOPALIGN = 0x0000,
            VCENTERALIGN = 0x0010,
            BOTTOMALIGN = 0x0020,
            HORIZONTAL = 0x0000,
            VERTICAL = 0x0040,
            NONOTIFY = 0x0080,
            RETURNCMD = 0x0100,
            RECURSE = 0x0001,
            HORPOSANIMATION = 0x0400,
            HORNEGANIMATION = 0x0800,
            VERPOSANIMATION = 0x1000,
            VERNEGANIMATION = 0x2000,
            NOANIMATION = 0x4000,
            LAYOUTRTL = 0x8000
        }

        // Flags used with the CMINVOKECOMMANDINFOEX structure
        [Flags]
        internal enum CMIC : uint
        {
            HOTKEY = 0x00000020,
            ICON = 0x00000010,
            FLAG_NO_UI = 0x00000400,
            UNICODE = 0x00004000,
            NO_CONSOLE = 0x00008000,
            ASYNCOK = 0x00100000,
            NOZONECHECKS = 0x00800000,
            SHIFT_DOWN = 0x10000000,
            CONTROL_DOWN = 0x40000000,
            FLAG_LOG_USAGE = 0x04000000,
            PTINVOKE = 0x20000000
        }

        // Specifies how the window is to be shown
        [Flags]
        internal enum SW
        {
            HIDE = 0,
            SHOWNORMAL = 1,
            NORMAL = 1,
            SHOWMINIMIZED = 2,
            SHOWMAXIMIZED = 3,
            MAXIMIZE = 3,
            SHOWNOACTIVATE = 4,
            SHOW = 5,
            MINIMIZE = 6,
            SHOWMINNOACTIVE = 7,
            SHOWNA = 8,
            RESTORE = 9,
            SHOWDEFAULT = 10,
        }

        // Window message flags
        [Flags]
        internal enum WM : uint
        {
            ACTIVATE = 0x6,
            ACTIVATEAPP = 0x1C,
            AFXFIRST = 0x360,
            AFXLAST = 0x37F,
            APP = 0x8000,
            ASKCBFORMATNAME = 0x30C,
            CANCELJOURNAL = 0x4B,
            CANCELMODE = 0x1F,
            CAPTURECHANGED = 0x215,
            CHANGECBCHAIN = 0x30D,
            CHAR = 0x102,
            CHARTOITEM = 0x2F,
            CHILDACTIVATE = 0x22,
            CLEAR = 0x303,
            CLOSE = 0x10,
            COMMAND = 0x111,
            COMPACTING = 0x41,
            COMPAREITEM = 0x39,
            CONTEXTMENU = 0x7B,
            COPY = 0x301,
            COPYDATA = 0x4A,
            CREATE = 0x1,
            CTLCOLORBTN = 0x135,
            CTLCOLORDLG = 0x136,
            CTLCOLOREDIT = 0x133,
            CTLCOLORLISTBOX = 0x134,
            CTLCOLORMSGBOX = 0x132,
            CTLCOLORSCROLLBAR = 0x137,
            CTLCOLORSTATIC = 0x138,
            CUT = 0x300,
            DEADCHAR = 0x103,
            DELETEITEM = 0x2D,
            DESTROY = 0x2,
            DESTROYCLIPBOARD = 0x307,
            DEVICECHANGE = 0x219,
            DEVMODECHANGE = 0x1B,
            DISPLAYCHANGE = 0x7E,
            DRAWCLIPBOARD = 0x308,
            DRAWITEM = 0x2B,
            DROPFILES = 0x233,
            ENABLE = 0xA,
            ENDSESSION = 0x16,
            ENTERIDLE = 0x121,
            ENTERMENULOOP = 0x211,
            ENTERSIZEMOVE = 0x231,
            ERASEBKGND = 0x14,
            EXITMENULOOP = 0x212,
            EXITSIZEMOVE = 0x232,
            FONTCHANGE = 0x1D,
            GETDLGCODE = 0x87,
            GETFONT = 0x31,
            GETHOTKEY = 0x33,
            GETICON = 0x7F,
            GETMINMAXINFO = 0x24,
            GETOBJECT = 0x3D,
            GETSYSMENU = 0x313,
            GETTEXT = 0xD,
            GETTEXTLENGTH = 0xE,
            HANDHELDFIRST = 0x358,
            HANDHELDLAST = 0x35F,
            HELP = 0x53,
            HOTKEY = 0x312,
            HSCROLL = 0x114,
            HSCROLLCLIPBOARD = 0x30E,
            ICONERASEBKGND = 0x27,
            IME_CHAR = 0x286,
            IME_COMPOSITION = 0x10F,
            IME_COMPOSITIONFULL = 0x284,
            IME_CONTROL = 0x283,
            IME_ENDCOMPOSITION = 0x10E,
            IME_KEYDOWN = 0x290,
            IME_KEYLAST = 0x10F,
            IME_KEYUP = 0x291,
            IME_NOTIFY = 0x282,
            IME_REQUEST = 0x288,
            IME_SELECT = 0x285,
            IME_SETCONTEXT = 0x281,
            IME_STARTCOMPOSITION = 0x10D,
            INITDIALOG = 0x110,
            INITMENU = 0x116,
            INITMENUPOPUP = 0x117,
            INPUTLANGCHANGE = 0x51,
            INPUTLANGCHANGEREQUEST = 0x50,
            KEYDOWN = 0x100,
            KEYFIRST = 0x100,
            KEYLAST = 0x108,
            KEYUP = 0x101,
            KILLFOCUS = 0x8,
            LBUTTONDBLCLK = 0x203,
            LBUTTONDOWN = 0x201,
            LBUTTONUP = 0x202,
            LVM_GETEDITCONTROL = 0x1018,
            LVM_SETIMAGELIST = 0x1003,
            MBUTTONDBLCLK = 0x209,
            MBUTTONDOWN = 0x207,
            MBUTTONUP = 0x208,
            MDIACTIVATE = 0x222,
            MDICASCADE = 0x227,
            MDICREATE = 0x220,
            MDIDESTROY = 0x221,
            MDIGETACTIVE = 0x229,
            MDIICONARRANGE = 0x228,
            MDIMAXIMIZE = 0x225,
            MDINEXT = 0x224,
            MDIREFRESHMENU = 0x234,
            MDIRESTORE = 0x223,
            MDISETMENU = 0x230,
            MDITILE = 0x226,
            MEASUREITEM = 0x2C,
            MENUCHAR = 0x120,
            MENUCOMMAND = 0x126,
            MENUDRAG = 0x123,
            MENUGETOBJECT = 0x124,
            MENURBUTTONUP = 0x122,
            MENUSELECT = 0x11F,
            MOUSEACTIVATE = 0x21,
            MOUSEFIRST = 0x200,
            MOUSEHOVER = 0x2A1,
            MOUSELAST = 0x20A,
            MOUSELEAVE = 0x2A3,
            MOUSEMOVE = 0x200,
            MOUSEWHEEL = 0x20A,
            MOVE = 0x3,
            MOVING = 0x216,
            NCACTIVATE = 0x86,
            NCCALCSIZE = 0x83,
            NCCREATE = 0x81,
            NCDESTROY = 0x82,
            NCHITTEST = 0x84,
            NCLBUTTONDBLCLK = 0xA3,
            NCLBUTTONDOWN = 0xA1,
            NCLBUTTONUP = 0xA2,
            NCMBUTTONDBLCLK = 0xA9,
            NCMBUTTONDOWN = 0xA7,
            NCMBUTTONUP = 0xA8,
            NCMOUSEHOVER = 0x2A0,
            NCMOUSELEAVE = 0x2A2,
            NCMOUSEMOVE = 0xA0,
            NCPAINT = 0x85,
            NCRBUTTONDBLCLK = 0xA6,
            NCRBUTTONDOWN = 0xA4,
            NCRBUTTONUP = 0xA5,
            NEXTDLGCTL = 0x28,
            NEXTMENU = 0x213,
            NOTIFY = 0x4E,
            NOTIFYFORMAT = 0x55,
            NULL = 0x0,
            PAINT = 0xF,
            PAINTCLIPBOARD = 0x309,
            PAINTICON = 0x26,
            PALETTECHANGED = 0x311,
            PALETTEISCHANGING = 0x310,
            PARENTNOTIFY = 0x210,
            PASTE = 0x302,
            PENWINFIRST = 0x380,
            PENWINLAST = 0x38F,
            POWER = 0x48,
            PRINT = 0x317,
            PRINTCLIENT = 0x318,
            QUERYDRAGICON = 0x37,
            QUERYENDSESSION = 0x11,
            QUERYNEWPALETTE = 0x30F,
            QUERYOPEN = 0x13,
            QUEUESYNC = 0x23,
            QUIT = 0x12,
            RBUTTONDBLCLK = 0x206,
            RBUTTONDOWN = 0x204,
            RBUTTONUP = 0x205,
            RENDERALLFORMATS = 0x306,
            RENDERFORMAT = 0x305,
            SETCURSOR = 0x20,
            SETFOCUS = 0x7,
            SETFONT = 0x30,
            SETHOTKEY = 0x32,
            SETICON = 0x80,
            SETMARGINS = 0xD3,
            SETREDRAW = 0xB,
            SETTEXT = 0xC,
            SETTINGCHANGE = 0x1A,
            SHOWWINDOW = 0x18,
            SIZE = 0x5,
            SIZECLIPBOARD = 0x30B,
            SIZING = 0x214,
            SPOOLERSTATUS = 0x2A,
            STYLECHANGED = 0x7D,
            STYLECHANGING = 0x7C,
            SYNCPAINT = 0x88,
            SYSCHAR = 0x106,
            SYSCOLORCHANGE = 0x15,
            SYSCOMMAND = 0x112,
            SYSDEADCHAR = 0x107,
            SYSKEYDOWN = 0x104,
            SYSKEYUP = 0x105,
            TCARD = 0x52,
            TIMECHANGE = 0x1E,
            TIMER = 0x113,
            TVM_GETEDITCONTROL = 0x110F,
            TVM_SETIMAGELIST = 0x1109,            
            UNDO = 0x304,
            UNINITMENUPOPUP = 0x125,
            USER = 0x400,
            USERCHANGED = 0x54,
            VKEYTOITEM = 0x2E,
            VSCROLL = 0x115,
            VSCROLLCLIPBOARD = 0x30A,
            WINDOWPOSCHANGED = 0x47,
            WINDOWPOSCHANGING = 0x46,
            WININICHANGE = 0x1A,
            SH_NOTIFY = 0x0401
        }

        // Specifies the content of the new menu item
        [Flags]
        internal enum MFT : uint
        {
            GRAYED = 0x00000003,
            DISABLED = 0x00000003,
            CHECKED = 0x00000008,
            SEPARATOR = 0x00000800,
            RADIOCHECK = 0x00000200,
            BITMAP = 0x00000004,
            OWNERDRAW = 0x00000100,
            MENUBARBREAK = 0x00000020,
            MENUBREAK = 0x00000040,
            RIGHTORDER = 0x00002000,
            BYCOMMAND = 0x00000000,
            BYPOSITION = 0x00000400,
            POPUP = 0x00000010,
            ////Added
            MFT_STRING = 0x000,
            MF_BYPOSITION = 0x400,
            MF_REMOVE = 0x1000
        }

        // Specifies the state of the new menu item
        [Flags]
        internal enum MFS : uint
        {
            GRAYED  = 0x00000003,
            DISABLED = 0x00000003,
            CHECKED = 0x00000008,
            HILITE = 0x00000080,
            ENABLED = 0x00000000,
            UNCHECKED = 0x00000000,
            UNHILITE = 0x00000000,
            DEFAULT = 0x00001000
        }

        // Specifies the content of the new menu item
        [Flags]
        internal enum MIIM : uint
        {
            BITMAP = 0x80,
            CHECKMARKS = 0x08,
            DATA = 0x20,
            FTYPE = 0x100,
            ID = 0x02,
            STATE = 0x01,
            STRING = 0x40,
            SUBMENU = 0x04,
            TYPE = 0x10
        }

        // Particular clipboard format of interest. 
        // There are three types of formats recognized by OLE
        internal enum CF
        {
            BITMAP = 2,
            DIB = 8,
            DIF = 5,
            DSPBITMAP = 130,
            DSPENHMETAFILE = 0x8e,
            DSPMETAFILEPICT = 0x83,
            DSPTEXT = 0x81,
            ENHMETAFILE = 14,
            GDIOBJFIRST = 0x300,
            GDIOBJLAST = 0x3ff,
            HDROP = 15,
            LOCALE = 0x10,
            MAX = 0x11,
            METAFILEPICT = 3,
            OEMTEXT = 7,
            OWNERDISPLAY = 0x80,
            PALETTE = 9,
            PENDATA = 10,
            PRIVATEFIRST = 0x200,
            PRIVATELAST = 0x2ff,
            RIFF = 11,
            SYLK = 4,
            TEXT = 1,
            TIFF = 6,
            UNICODETEXT = 13,
            WAVE = 12
        }

        // Indicates the type of storage medium being used in a data transfer
        [Flags]
        internal enum TYMED
        {
            ENHMF = 0x40,
            FILE = 2,
            GDI = 0x10,
            HGLOBAL = 1,
            ISTORAGE = 8,
            ISTREAM = 4,
            MFPICT = 0x20,
            NULL = 0
        }

        // Specifies a group of flags for controlling the advisory connection
        [Flags]
        internal enum ADVF
        {
            CACHE_FORCEBUILTIN = 0x10,
            CACHE_NOHANDLER = 8,
            CACHE_ONSAVE = 0x20,
            DATAONSTOP = 0x40,
            NODATA = 1,
            ONLYONCE = 4,
            PRIMEFIRST = 2
        }

        // Flags indicating which mouse buttons are clicked and which modifier keys are pressed
        [Flags]
        internal enum MK
        {
            LBUTTON = 0x0001,
            RBUTTON = 0x0002,
            SHIFT = 0x0004,
            CONTROL = 0x0008,
            MBUTTON = 0x0010,
            ALT = 0x0020
        }

        // Are used in activation calls to indicate the execution contexts in which an object is to be run
        [Flags]
        internal enum CLSCTX : uint
        {
            INPROC_SERVER = 0x1,
            INPROC_HANDLER = 0x2,
            LOCAL_SERVER = 0x4,
            INPROC_SERVER16 = 0x8,
            REMOTE_SERVER = 0x10,
            INPROC_HANDLER16 = 0x20,
            RESERVED1 = 0x40,
            RESERVED2 = 0x80,
            RESERVED3 = 0x100,
            RESERVED4 = 0x200,
            NO_CODE_DOWNLOAD = 0x400,
            RESERVED5 = 0x800,
            NO_CUSTOM_MARSHAL = 0x1000,
            ENABLE_CODE_DOWNLOAD = 0x2000,
            NO_FAILURE_LOG = 0x4000,
            DISABLE_AAA = 0x8000,
            ENABLE_AAA = 0x10000,
            FROM_DEFAULT_CONTEXT = 0x20000,
            INPROC = INPROC_SERVER | INPROC_HANDLER,
            SERVER = INPROC_SERVER | LOCAL_SERVER | REMOTE_SERVER,
            ALL = SERVER | INPROC_HANDLER
        }

        // Describes the event that has occurred
        [Flags]
        internal enum SHCNE : uint
        {
            RENAMEITEM = 0x00000001,
            CREATE = 0x00000002,
            DELETE = 0x00000004,
            MKDIR = 0x00000008,
            RMDIR = 0x00000010,
            MEDIAINSERTED = 0x00000020,
            MEDIAREMOVED = 0x00000040,
            DRIVEREMOVED = 0x00000080,
            DRIVEADD = 0x00000100,
            NETSHARE = 0x00000200,
            NETUNSHARE = 0x00000400,
            ATTRIBUTES = 0x00000800,
            UPDATEDIR = 0x00001000,
            UPDATEITEM = 0x00002000,
            SERVERDISCONNECT = 0x00004000,
            UPDATEIMAGE = 0x00008000,
            DRIVEADDGUI = 0x00010000,
            RENAMEFOLDER = 0x00020000,
            FREESPACE = 0x00040000,
            EXTENDED_EVENT = 0x04000000,
            ASSOCCHANGED = 0x08000000,
            DISKEVENTS = 0x0002381F,
            GLOBALEVENTS = 0x0C0581E0,
            ALLEVENTS = 0x7FFFFFFF,
            INTERRUPT = 0x80000000
        }

        // Flags that indicate the meaning of the dwItem1 and dwItem2 parameters
        [Flags]
        internal enum SHCNF
        {
            IDLIST = 0x0000,
            PATHA = 0x0001,
            PRINTERA = 0x0002,
            DWORD = 0x0003,
            PATHW = 0x0005,
            PRINTERW = 0x0006,
            TYPE = 0x00FF,
            FLUSH = 0x1000,
            FLUSHNOWAIT = 0x2000
        }

        // Indicate the type of events for which to receive notifications
        [Flags]
        internal enum SHCNRF
        {
            InterruptLevel = 0x0001,
            ShellLevel = 0x0002,
            RecursiveInterrupt = 0x1000,
            NewDelivery = 0x8000
        }

        // Indicate whether the method should try to return a name in the pwcsName member of the STATSTG structure
        [Flags]
        internal enum STATFLAG
        {
            DEFAULT = 0,
            NONAME = 1,
            NOOPEN = 2
        }

        // Indicate the type of locking requested for the specified range of bytes
        [Flags]
        internal enum LOCKTYPE
        {
            WRITE = 1,
            EXCLUSIVE = 2,
            ONLYONCE = 4
        }

        // Used in the type member of the STATSTG structure to indicate the type of the storage element
        internal enum STGTY
        {
            STORAGE = 1,
            STREAM = 2,
            LOCKBYTES = 3,
            PROPERTY = 4
        }

        // Indicate conditions for creating and deleting the object and access modes for the object
        [Flags]
        internal enum STGM
        {
            DIRECT = 0x00000000,
            TRANSACTED = 0x00010000,
            SIMPLE = 0x08000000,
            READ = 0x00000000,
            WRITE = 0x00000001,
            READWRITE = 0x00000002,
            SHARE_DENY_NONE = 0x00000040,
            SHARE_DENY_READ = 0x00000030,
            SHARE_DENY_WRITE = 0x00000020,
            SHARE_EXCLUSIVE = 0x00000010,
            PRIORITY = 0x00040000,
            DELETEONRELEASE = 0x04000000,
            NOSCRATCH = 0x00100000,
            CREATE = 0x00001000,
            CONVERT = 0x00020000,
            FAILIFTHERE = 0x00000000,
            NOSNAPSHOT = 0x00200000,
            DIRECT_SWMR = 0x00400000,
        }

        // Indicate whether a storage element is to be moved or copied
        internal enum STGMOVE
        {
            MOVE = 0,
            COPY = 1,
            SHALLOWCOPY = 2
        }

        // Specify the conditions for performing the commit operation in the IStorage::Commit and IStream::Commit methods
        [Flags]
        internal enum STGC
        {
            DEFAULT = 0,
            OVERWRITE = 1,
            ONLYIFCURRENT = 2,
            DANGEROUSLYCOMMITMERELYTODISKCACHE = 4,
            CONSOLIDATE = 8
        }

        // Directing the handling of the item from which you're retrieving the info tip text
        [Flags]
        internal enum QITIPF
        {
            DEFAULT = 0x00000,
            USENAME = 0x00001,
            LINKNOTARGET = 0x00002,
            LINKUSETARGET = 0x00004,
            USESLOWTIP = 0x00008
        }

        #endregion
    }
}