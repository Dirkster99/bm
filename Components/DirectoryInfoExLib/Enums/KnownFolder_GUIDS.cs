namespace DirectoryInfoExLib.Enums
{
    using System;

    /// <summary>
    /// The KNOWNFOLDERID constants represent GUIDs that identify standard folders registered with the
    /// system as Known Folders. These folders are installed with Windows Vista and later operating
    /// systems, and a computer will have only folders appropriate to it installed. These values are
    /// defined in Knownfolders.h.
    /// 
    /// https://msdn.microsoft.com/en-us/library/bb762584(VS.85).aspx
    ///
    /// See also Win10-SDK
    /// https://github.com/tpn/winsdk-10/blob/master/Include/10.0.10240.0/um/KnownFolders.h
    /// Last Updated: 2018-04-09
    /// </summary>
    public class KnownFolder_GUIDS
    {
        #region KnownFolder Guids
        /// <summary>
        /// Display Name          Computer
        /// Folder Type           VIRTUAL
        /// Default Path          Not applicable—virtual folder
        /// CSIDL Equivalent      CSIDL_DRIVES
        /// Legacy Display Name   My Computer
        /// Legacy Default Path   Not applicable—virtual folder
        /// </summary>
        public static Guid Computer = new Guid(0x0AC0837C, 0xBBF8, 0x452A, 0x85, 0x0D, 0x79, 0xD0, 0x8E, 0x66, 0x7C, 0xA7);

        /// <summary>
        /// Display Name          Conflicts
        /// Folder Type           VIRTUAL
        /// Default Path          Not applicable—virtual folder
        /// CSIDL Equivalent      None, new in Windows Vista
        /// Legacy Display Name   Not applicable. This KNOWNFOLDERID refers to the Windows Vista Synchronization Manager. It is not the folder referenced by the older ISyncMgrConflictFolder.
        /// Legacy Default Path   Not applicable
        /// </summary>
        public static Guid Conflict = new Guid(0x4bfefb45, 0x347d, 0x4006, 0xa5, 0xbe, 0xac, 0x0c, 0xb0, 0x56, 0x71, 0x92);

        /// <summary>
        /// Display Name         Control Panel
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_CONTROLS
        /// Legacy Display Name  Control Panel
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public static Guid ControlPanel = new Guid(0x82A74AEB, 0xAEB4, 0x465C, 0xA0, 0x14, 0xD0, 0x97, 0xEE, 0x34, 0x6D, 0x63);

        /// <summary>
        /// Display Name         Desktop
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Desktop
        /// CSIDL Equivalent     CSIDL_DESKTOP, CSIDL_DESKTOPDIRECTORY
        /// Legacy Display Name  Desktop
        /// Legacy Default Path  %USERPROFILE%\Desktop
        /// </summary>
        public static Guid Desktop = new Guid(0xB4BFCC3A, 0xDB2C, 0x424C, 0xB0, 0x29, 0x7F, 0xE9, 0x9A, 0x87, 0xC6, 0x41);

        /// <summary>
        /// Display Name         Internet Explorer
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_INTERNET
        /// Legacy Display Name  Internet Explorer
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public static Guid Internet = new Guid(0x4D9F7874, 0x4E0C, 0x4904, 0x96, 0x7B, 0x40, 0xB0, 0xD2, 0x0C, 0x3E, 0x4B);

        /// <summary>
        /// Display Name         Network
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_NETWORK, CSIDL_COMPUTERSNEARME
        /// Legacy Display Name  My Network Places
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public static Guid Network = new Guid(0xD20BEEC4, 0x5CA8, 0x4905, 0xAE, 0x3B, 0xBF, 0x25, 0x1E, 0xA0, 0x9B, 0x53);

        /// <summary>
        /// Display Name         Printers
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_PRINTERS
        /// Legacy Display Name  Printers and Faxes
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public static Guid Printers = new Guid(0x76FC4E2D, 0xD6AD, 0x4519, 0xA6, 0x63, 0x37, 0xBD, 0x56, 0x06, 0x81, 0x85);

        /// <summary>
        /// Display Name         Sync Center
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid SyncManager = new Guid(0x43668BF8, 0xC14E, 0x49B2, 0x97, 0xC9, 0x74, 0x77, 0x84, 0xD7, 0x84, 0xB7);

        /// <summary>
        /// Display Name         Network Connections
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_CONNECTIONS
        /// Legacy Display Name  Network Connections
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public static Guid Connections = new Guid(0x6F0CD92B, 0x2E97, 0x45D1, 0x88, 0xFF, 0xB0, 0xD1, 0x86, 0xB8, 0xDE, 0xDD);

        /// <summary>
        /// Display Name         Sync Setup
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid SyncSetup = new Guid(0xf214138, 0xb1d3, 0x4a90, 0xbb, 0xa9, 0x27, 0xcb, 0xc0, 0xc5, 0x38, 0x9a);

        /// <summary>
        /// Display Name         Sync Results
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid SyncResults = new Guid(0x289a9a43, 0xbe44, 0x4057, 0xa4, 0x1b, 0x58, 0x7a, 0x76, 0xd7, 0xe7, 0xf9);

        /// <summary>
        /// Display Name         Recycle Bin
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_BITBUCKET
        /// Legacy Display Name  Recycle Bin
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public static Guid RecycleBin = new Guid(0xB7534046, 0x3ECB, 0x4C18, 0xBE, 0x4E, 0x64, 0xCD, 0x4C, 0xB7, 0xD6, 0xAC);

        /// <summary>
        /// Display Name         Fonts
        /// Folder Type          FIXED
        /// Default Path         %windir%\Fonts
        /// CSIDL Equivalent     CSIDL_FONTS
        /// Legacy Display Name  Fonts
        /// Legacy Default Path  %windir%\Fonts
        /// </summary>
        public static Guid Fonts = new Guid(0xFD228CB7, 0xAE11, 0x4AE3, 0x86, 0x4C, 0x16, 0xF3, 0x91, 0x0A, 0xB8, 0xFE);

        /// <summary>
        /// Display Name         Startup
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Start Menu\Programs\StartUp
        /// CSIDL Equivalent     CSIDL_STARTUP, CSIDL_ALTSTARTUP
        /// Legacy Display Name  Startup
        /// Legacy Default Path  %USERPROFILE%\Start Menu\Programs\StartUp
        /// </summary>
        public static Guid Startup = new Guid(0xB97D20BB, 0xF46A, 0x4C97, 0xBA, 0x10, 0x5E, 0x36, 0x08, 0x43, 0x08, 0x54);

        /// <summary>
        /// Display Name         Programs
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Start Menu\Programs
        /// CSIDL Equivalent     CSIDL_PROGRAMS
        /// Legacy Display Name  Programs
        /// Legacy Default Path  %USERPROFILE%\Start Menu\Programs
        /// </summary>
        public static Guid Programs = new Guid(0xA77F5D77, 0x2E2B, 0x44C3, 0xA6, 0xA2, 0xAB, 0xA6, 0x01, 0x05, 0x4A, 0x51);

        /// <summary>
        /// Display Name         Start Menu
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Start Menu
        /// CSIDL Equivalent     CSIDL_STARTMENU
        /// Legacy Display Name  Start Menu
        /// Legacy Default Path  %USERPROFILE%\Start Menu
        /// </summary>
        public static Guid StartMenu = new Guid(0x625B53C3, 0xAB48, 0x4EC1, 0xBA, 0x1F, 0xA1, 0xEF, 0x41, 0x46, 0xFC, 0x19);

        /// <summary>
        /// Display Name         Recent Items
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Recent
        /// CSIDL Equivalent     CSIDL_RECENT
        /// Legacy Display Name  My Recent Documents
        /// Legacy Default Path  %USERPROFILE%\Recent
        /// </summary>
        public static Guid Recent = new Guid(0xAE50C081, 0xEBD2, 0x438A, 0x86, 0x55, 0x8A, 0x09, 0x2E, 0x34, 0x98, 0x7A);

        /// <summary>
        /// Display Name         SendTo
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\SendTo
        /// CSIDL Equivalent     CSIDL_SENDTO
        /// Legacy Display Name  SendTo
        /// Legacy Default Path  %USERPROFILE%\SendTo
        /// </summary>
        public static Guid SendTo = new Guid(0x8983036C, 0x27C0, 0x404B, 0x8F, 0x08, 0x10, 0x2D, 0x10, 0xDC, 0xFD, 0x74);

        /// <summary>
        /// Display Name         Documents
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Documents
        /// CSIDL Equivalent     CSIDL_MYDOCUMENTS, CSIDL_PERSONAL
        /// Legacy Display Name  My Documents
        /// Legacy Default Path  %USERPROFILE%\My Documents
        /// </summary>
        public static Guid Documents = new Guid(0xFDD39AD0, 0x238F, 0x46AF, 0xAD, 0xB4, 0x6C, 0x85, 0x48, 0x03, 0x69, 0xC7);

        /// <summary>
        /// Display Name         Favorites
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Favorites
        /// CSIDL Equivalent     CSIDL_FAVORITES, CSIDL_COMMON_FAVORITES
        /// Legacy Display Name  Favorites
        /// Legacy Default Path  %USERPROFILE%\Favorites
        /// </summary>
        public static Guid Favorites = new Guid(0x1777F761, 0x68AD, 0x4D8A, 0x87, 0xBD, 0x30, 0xB7, 0x59, 0xFA, 0x33, 0xDD);

        /// <summary>
        /// Display Name         Network Shortcuts
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Network Shortcuts
        /// CSIDL Equivalent     CSIDL_NETHOOD
        /// Legacy Display Name  NetHood
        /// Legacy Default Path  %USERPROFILE%\NetHood
        /// </summary>
        public static Guid NetHood = new Guid(0xC5ABBF53, 0xE17F, 0x4121, 0x89, 0x00, 0x86, 0x62, 0x6F, 0xC2, 0xC9, 0x73);

        /// <summary>
        /// Display Name         Printer Shortcuts
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Printer Shortcuts
        /// CSIDL Equivalent     CSIDL_PRINTHOOD
        /// Legacy Display Name  PrintHood
        /// Legacy Default Path  %USERPROFILE%\PrintHood
        /// </summary>
        public static Guid PrintHood = new Guid(0x9274BD8D, 0xCFD1, 0x41C3, 0xB3, 0x5E, 0xB1, 0x3F, 0x55, 0xA7, 0x58, 0xF4);

        /// <summary>
        /// Display Name         Templates
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Templates
        /// CSIDL Equivalent     CSIDL_TEMPLATES
        /// Legacy Display Name  Templates
        /// Legacy Default Path  %USERPROFILE%\Templates
        /// </summary>
        public static Guid Templates = new Guid(0xA63293E8, 0x664E, 0x48DB, 0xA0, 0x79, 0xDF, 0x75, 0x9E, 0x05, 0x09, 0xF7);

        /// <summary>
        /// Display Name          Startup
        /// Folder Type           COMMON
        /// Default Path          %ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs\StartUp
        /// CSIDL Equivalent      CSIDL_COMMON_STARTUP, CSIDL_COMMON_ALTSTARTUP
        /// Legacy Display Name   Startup
        /// Legacy Default Path   %ALLUSERSPROFILE%\Start Menu\Programs\StartUp
        /// </summary>
        public static Guid CommonStartup = new Guid(0x82A5EA35, 0xD9CD, 0x47C5, 0x96, 0x29, 0xE1, 0x5D, 0x2F, 0x71, 0x4E, 0x6E);

        /// <summary>
        /// Display Name          Programs
        /// Folder Type           COMMON
        /// Default Path          %ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs
        /// CSIDL Equivalent      CSIDL_COMMON_PROGRAMS
        /// Legacy Display Name   Programs
        /// Legacy Default Path   %ALLUSERSPROFILE%\Start Menu\Programs
        /// </summary>
        public static Guid CommonPrograms = new Guid(0x0139D44E, 0x6AFE, 0x49F2, 0x86, 0x90, 0x3D, 0xAF, 0xCA, 0xE6, 0xFF, 0xB8);

        /// <summary>
        /// Display Name          Start Menu
        /// Folder Type           COMMON
        /// Default Path          %ALLUSERSPROFILE%\Microsoft\Windows\Start Menu
        /// CSIDL Equivalent      CSIDL_COMMON_STARTMENU
        /// Legacy Display Name   Start Menu
        /// Legacy Default Path   %ALLUSERSPROFILE%\Start Menu
        /// </summary>
        public static Guid CommonStartMenu = new Guid(0xA4115719, 0xD62E, 0x491D, 0xAA, 0x7C, 0xE7, 0x4B, 0x8B, 0xE3, 0xB0, 0x67);

        /// <summary>
        /// Display Name         Public Desktop
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Desktop
        /// CSIDL Equivalent     CSIDL_COMMON_DESKTOPDIRECTORY
        /// Legacy Display Name  Desktop
        /// Legacy Default Path  %ALLUSERSPROFILE%\Desktop
        /// </summary>
        public static Guid PublicDesktop = new Guid(0xC4AA340D, 0xF20F, 0x4863, 0xAF, 0xEF, 0xF8, 0x7E, 0xF2, 0xE6, 0xBA, 0x25);

        /// <summary>
        /// Display Name         ProgramData
        /// Folder Type          FIXED
        /// Default Path         %ALLUSERSPROFILE% (%ProgramData%, %SystemDrive%\ProgramData)
        /// CSIDL Equivalent     CSIDL_COMMON_APPDATA
        /// Legacy Display Name  Application Data
        /// Legacy Default Path  %ALLUSERSPROFILE%\Application Data
        /// </summary>
        public static Guid ProgramData = new Guid(0x62AB5D82, 0xFDC1, 0x4DC3, 0xA9, 0xDD, 0x07, 0x0D, 0x1D, 0x49, 0x5D, 0x97);

        /// <summary>
        /// Display Name          Templates
        /// Folder Type           COMMON
        /// Default Path          %ALLUSERSPROFILE%\Templates
        /// CSIDL Equivalent      CSIDL_COMMON_TEMPLATES
        /// Legacy Display Name   Templates
        /// Legacy Default Path   %ALLUSERSPROFILE%\Templates
        /// </summary>
        public static Guid CommonTemplates = new Guid(0xB94237E7, 0x57AC, 0x4347, 0x91, 0x51, 0xB0, 0x8C, 0x6C, 0x32, 0xD1, 0xF7);

        /// <summary>
        /// Display Name         Public Documents
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Documents
        /// CSIDL Equivalent     CSIDL_COMMON_DOCUMENTS
        /// Legacy Display Name  Shared Documents
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents
        /// </summary>
        public static Guid PublicDocuments = new Guid(0xED4824AF, 0xDCE4, 0x45A8, 0x81, 0xE2, 0xFC, 0x79, 0x65, 0x08, 0x36, 0x34);

        /// <summary>
        /// Display Name         Roaming
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA% (%USERPROFILE%\AppData\Roaming)
        /// CSIDL Equivalent     CSIDL_APPDATA
        /// Legacy Display Name  Application Data
        /// Legacy Default Path  %APPDATA% (%USERPROFILE%\Application Data)
        /// </summary>
        public static Guid RoamingAppData = new Guid(0x3EB685DB, 0x65F9, 0x4CF6, 0xA0, 0x3A, 0xE3, 0xEF, 0x65, 0x72, 0x9F, 0x3D);

        /// <summary>
        /// Display Name         Local
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA% (%USERPROFILE%\AppData\Local)
        /// CSIDL Equivalent     CSIDL_LOCAL_APPDATA
        /// Legacy Display Name  Application Data
        /// Legacy Default Path  %USERPROFILE%\Local Settings\Application Data
        /// </summary>
        public static Guid LocalAppData = new Guid(0xF1B32785, 0x6FBA, 0x4FCF, 0x9D, 0x55, 0x7B, 0x8E, 0x7F, 0x15, 0x70, 0x91);

        /// <summary>
        /// Display Name         LocalLow
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\AppData\LocalLow
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid LocalAppDataLow = new Guid(0xA520A1A4, 0x1780, 0x4FF6, 0xBD, 0x18, 0x16, 0x73, 0x43, 0xC5, 0xAF, 0x16);

        /// <summary>
        /// Display Name         Temporary Internet Files
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\Temporary Internet Files
        /// CSIDL Equivalent     CSIDL_INTERNET_CACHE
        /// Legacy Display Name  Temporary Internet Files
        /// Legacy Default Path  %USERPROFILE%\Local Settings\Temporary Internet Files
        /// </summary>
        public static Guid InternetCache = new Guid(0x352481E8, 0x33BE, 0x4251, 0xBA, 0x85, 0x60, 0x07, 0xCA, 0xED, 0xCF, 0x9D);

        /// <summary>
        /// Display Name         Cookies
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Cookies
        /// CSIDL Equivalent     CSIDL_COOKIES
        /// Legacy Display Name  Cookies
        /// Legacy Default Path  %USERPROFILE%\Cookies
        /// </summary>
        public static Guid Cookies = new Guid(0x2B0F765D, 0xC0E9, 0x4171, 0x90, 0x8E, 0x08, 0xA6, 0x11, 0xB8, 0x4F, 0xF6);

        /// <summary>
        /// Display Name         History
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\History
        /// CSIDL Equivalent     CSIDL_HISTORY
        /// Legacy Display Name  History
        /// Legacy Default Path  %USERPROFILE%\Local Settings\History
        /// </summary>
        public static Guid History = new Guid(0xD9DC8A3B, 0xB784, 0x432E, 0xA7, 0x81, 0x5A, 0x11, 0x30, 0xA7, 0x59, 0x63);

        /// <summary>
        /// Display Name         System32
        /// Folder Type          FIXED
        /// Default Path         %windir%\system32
        /// CSIDL Equivalent     CSIDL_SYSTEM
        /// Legacy Display Name  system32
        /// Legacy Default Path  %windir%\system32
        /// </summary>
        public static Guid System = new Guid(0x1AC14E77, 0x02E7, 0x4E5D, 0xB7, 0x44, 0x2E, 0xB1, 0xAE, 0x51, 0x98, 0xB7);

        /// <summary>
        /// Display Name         System32
        /// Folder Type          FIXED
        /// Default Path         %windir%\system32
        /// CSIDL Equivalent     CSIDL_SYSTEMX86
        /// Legacy Display Name  system32
        /// Legacy Default Path  %windir%\system32
        /// </summary>
        public static Guid SystemX86 = new Guid(0xD65231B0, 0xB2F1, 0x4857, 0xA4, 0xCE, 0xA8, 0xE7, 0xC6, 0xEA, 0x7D, 0x27);

        /// <summary>
        /// Display Name         Windows
        /// Folder Type          FIXED
        /// Default Path         %windir%
        /// CSIDL Equivalent     CSIDL_WINDOWS
        /// Legacy Display Name  WINDOWS
        /// Legacy Default Path  %windir%
        /// </summary>
        public static Guid Windows = new Guid(0xF38BF404, 0x1D43, 0x42F2, 0x93, 0x05, 0x67, 0xDE, 0x0B, 0x28, 0xFC, 0x23);

        /// <summary>
        /// Display Name         The user's username (%USERNAME%)
        /// Folder Type          FIXED
        /// Default Path         %USERPROFILE% (%SystemDrive%\Users\%USERNAME%)
        /// CSIDL Equivalent     CSIDL_PROFILE
        /// Legacy Display Name  The user's username (%USERNAME%)
        /// Legacy Default Path  %USERPROFILE% (%SystemDrive%\Documents and Settings\%USERNAME%)
        /// </summary>
        public static Guid Profile = new Guid(0x5E6C858F, 0x0E22, 0x4760, 0x9A, 0xFE, 0xEA, 0x33, 0x17, 0xB6, 0x71, 0x73);

        /// <summary>
        /// Display Name         Pictures
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Pictures
        /// CSIDL Equivalent     CSIDL_MYPICTURES
        /// Legacy Display Name  My Pictures
        /// Legacy Default Path  %USERPROFILE%\My Documents\My Pictures
        /// </summary>
        public static Guid Pictures = new Guid(0x33E28130, 0x4E1E, 0x4676, 0x83, 0x5A, 0x98, 0x39, 0x5C, 0x3B, 0xC3, 0xBB);

        /// <summary>
        /// Display Name         Program Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles% (%SystemDrive%\Program Files)
        /// CSIDL Equivalent     CSIDL_PROGRAM_FILESX86
        /// Legacy Display Name  Program Files
        /// Legacy Default Path  %ProgramFiles% (%SystemDrive%\Program Files)
        /// </summary>
        public static Guid ProgramFilesX86 = new Guid(0x7C5A40EF, 0xA0FB, 0x4BFC, 0x87, 0x4A, 0xC0, 0xF2, 0xE0, 0xB9, 0xFA, 0x8E);

        /// <summary>
        /// Display Name         Common Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles%\Common Files
        /// CSIDL Equivalent     CSIDL_PROGRAM_FILES_COMMONX86
        /// Legacy Display Name  Common Files
        /// Legacy Default Path  %ProgramFiles%\Common Files
        /// </summary>
        public static Guid ProgramFilesCommonX86 = new Guid(0xDE974D24, 0xD9C6, 0x4D3E, 0xBF, 0x91, 0xF4, 0x45, 0x51, 0x20, 0xB9, 0x17);

        /// <summary>
        /// Display Name         Program Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles% (%SystemDrive%\Program Files)
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Program Files
        /// Legacy Default Path  %ProgramFiles% (%SystemDrive%\Program Files)
        /// </summary>
        public static Guid ProgramFilesX64 = new Guid(0x6d809377, 0x6af0, 0x444b, 0x89, 0x57, 0xa3, 0x77, 0x3f, 0x02, 0x20, 0x0e);

        /// <summary>
        /// Display Name         Common Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles%\Common Files
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Common Files
        /// Legacy Default Path  %ProgramFiles%\Common Files
        /// </summary>
        public static Guid ProgramFilesCommonX64 = new Guid(0x6365d5a7, 0xf0d, 0x45e5, 0x87, 0xf6, 0xd, 0xa5, 0x6b, 0x6a, 0x4f, 0x7d);

        /// <summary>
        /// Display Name         Program Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles% (%SystemDrive%\Program Files)
        /// CSIDL Equivalent     CSIDL_PROGRAM_FILES
        /// Legacy Display Name  Program Files
        /// Legacy Default Path  %ProgramFiles% (%SystemDrive%\Program Files)
        /// </summary>
        public static Guid ProgramFiles = new Guid(0x905e63b6, 0xc1bf, 0x494e, 0xb2, 0x9c, 0x65, 0xb7, 0x32, 0xd3, 0xd2, 0x1a);

        /// <summary>
        /// Display Name         Common Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles%\Common Files
        /// CSIDL Equivalent     CSIDL_PROGRAM_FILES_COMMON
        /// Legacy Display Name  Common Files
        /// Legacy Default Path  %ProgramFiles%\Common Files
        /// </summary>
        public static Guid ProgramFilesCommon = new Guid(0xF7F1ED05, 0x9F6D, 0x47A2, 0xAA, 0xAE, 0x29, 0xD3, 0x17, 0xC6, 0xF0, 0x66);

        /// <summary>
        /// Display Name        Administrative Tools
        /// Folder Type         PERUSER
        /// Default Path        %APPDATA%\Microsoft\Windows\Start Menu\Programs\Administrative Tools
        /// CSIDL Equivalent    CSIDL_ADMINTOOLS
        /// Legacy Display Name Administrative Tools
        /// Legacy Default Path %USERPROFILE%\Start Menu\Programs\Administrative Tools
        /// </summary>
        public static Guid AdminTools = new Guid(0x724EF170, 0xA42D, 0x4FEF, 0x9F, 0x26, 0xB6, 0x0E, 0x84, 0x6F, 0xBA, 0x4F);

        /// <summary>
        /// Display Name            Administrative Tools
        /// Folder Type             COMMON
        /// Default Path            %ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs\Administrative Tools
        /// CSIDL Equivalent        CSIDL_COMMON_ADMINTOOLS
        /// Legacy Display Name     Administrative Tools
        /// Legacy Default Path     %ALLUSERSPROFILE%\Start Menu\Programs\Administrative Tools
        /// </summary>
        public static Guid CommonAdminTools = new Guid(0xD0384E7D, 0xBAC3, 0x4797, 0x8F, 0x14, 0xCB, 0xA2, 0x29, 0xB3, 0x92, 0xB5);

        /// <summary>
        /// Display Name         Music
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Music
        /// CSIDL Equivalent     CSIDL_MYMUSIC
        /// Legacy Display Name  My Music
        /// Legacy Default Path  %USERPROFILE%\My Documents\My Music
        /// </summary>
        public static Guid Music = new Guid(0x4BD8D571, 0x6D19, 0x48D3, 0xBE, 0x97, 0x42, 0x22, 0x20, 0x08, 0x0E, 0x43);

        /// <summary>
        /// Display Name         Videos
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Videos
        /// CSIDL                CSIDL_MYVIDEO
        /// Legacy Display Name  My Videos
        /// Legacy Default Path  %USERPROFILE%\My Documents\My Videos
        /// </summary>
        public static Guid Videos = new Guid(0x18989B1D, 0x99B5, 0x455B, 0x84, 0x1C, 0xAB, 0x7C, 0x74, 0xE4, 0xDD, 0xFC);

        /// <summary>
        /// Display Name         Public Pictures
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Pictures
        /// CSIDL Equivalent     CSIDL_COMMON_PICTURES
        /// Legacy Display Name  Shared Pictures
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Pictures
        /// </summary>
        public static Guid PublicPictures = new Guid(0xB6EBFB86, 0x6907, 0x413C, 0x9A, 0xF7, 0x4F, 0xC2, 0xAB, 0xF0, 0x7C, 0xC5);

        /// <summary>
        /// Display Name         Public Music
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Music
        /// CSIDL Equivalent     CSIDL_COMMON_MUSIC
        /// Legacy Display Name  Shared Music
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Music
        /// </summary>
        public static Guid PublicMusic = new Guid(0x3214FAB5, 0x9757, 0x4298, 0xBB, 0x61, 0x92, 0xA9, 0xDE, 0xAA, 0x44, 0xFF);

        /// <summary>
        /// Display Name         Public Videos
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Videos
        /// CSIDL Equivalent     CSIDL_COMMON_VIDEO
        /// Legacy Display Name  Shared Video
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Videos
        /// </summary>
        public static Guid PublicVideos = new Guid(0x2400183A, 0x6185, 0x49FB, 0xA2, 0xD8, 0x4A, 0x39, 0x2A, 0x60, 0x2B, 0xA3);

        /// <summary>
        /// Display Name         Resources
        /// Folder Type          FIXED
        /// Default Path         %windir%\Resources
        /// CSIDL Equivalent     CSIDL_RESOURCES
        /// Legacy Display Name  Resources
        /// Legacy Default Path  %windir%\Resources
        /// </summary>
        public static Guid ResourceDir = new Guid(0x8AD10C31, 0x2ADB, 0x4296, 0xA8, 0xF7, 0xE4, 0x70, 0x12, 0x32, 0xC9, 0x72);

        /// <summary>
        /// Display Name         None
        /// Folder Type          FIXED
        /// Default Path         %windir%\resources\0409 (code page)
        /// CSIDL Equivalent     CSIDL_RESOURCES_LOCALIZED
        /// Legacy Display Name  None
        /// Legacy Default Path  %windir%\resources\0409 (code page)
        /// </summary>
        public static Guid LocalizedResourcesDir = new Guid(0x2A00375E, 0x224C, 0x49DE, 0xB8, 0xD1, 0x44, 0x0D, 0xF7, 0xEF, 0x3D, 0xDC);

        /// <summary>
        /// Display Name        OEM Links
        /// Folder Type         COMMON
        /// Default Path        %ALLUSERSPROFILE%\OEM Links
        /// CSIDL Equivalent    CSIDL_COMMON_OEM_LINKS
        /// Legacy Display Name OEM Links
        /// Legacy Default Path %ALLUSERSPROFILE%\OEM Links
        /// </summary>
        public static Guid CommonOEMLinks = new Guid(0xC1BAE2D0, 0x10DF, 0x4334, 0xBE, 0xDD, 0x7A, 0xA2, 0x0B, 0x22, 0x7A, 0x9D);

        /// <summary>
        /// Display Name        Temporary Burn Folder
        /// Folder Type         PERUSER
        /// Default Path        %LOCALAPPDATA%\Microsoft\Windows\Burn\Burn
        /// CSIDL Equivalent    CSIDL_CDBURN_AREA
        /// Legacy Display Name CD Burning
        /// Legacy Default Path %USERPROFILE%\Local Settings\Application Data\Microsoft\CD Burning
        /// </summary>
        public static Guid CDBurning = new Guid(0x9E52AB10, 0xF80D, 0x49DF, 0xAC, 0xB8, 0x43, 0x30, 0xF5, 0x68, 0x78, 0x55);

        /// <summary>
        /// Display Name         Users
        /// Folder Type          FIXED
        /// Default Path         %SystemDrive%\Users
        /// CSIDL Equivalent     None, new for Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid UserProfiles = new Guid(0x0762D272, 0xC50A, 0x4BB0, 0xA3, 0x82, 0x69, 0x7D, 0xCD, 0x72, 0x9B, 0x80);

        /// <summary>
        /// Display Name         Playlists
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Music\Playlists
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid Playlists = new Guid(0xDE92C1C7, 0x837F, 0x4F69, 0xA3, 0xBB, 0x86, 0xE6, 0x31, 0x20, 0x4A, 0x23);

        /// <summary>
        /// Display Name         Sample Playlists
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Music\Sample Playlists
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid SamplePlaylists = new Guid(0x15CA69B3, 0x30EE, 0x49C1, 0xAC, 0xE1, 0x6B, 0x5E, 0xC3, 0x72, 0xAF, 0xB5);

        /// <summary>
        /// Display Name         Sample Music
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Music\Sample Music
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Sample Music
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Music\Sample Music
        /// </summary>
        public static Guid SampleMusic = new Guid(0xB250C668, 0xF57D, 0x4EE1, 0xA6, 0x3C, 0x29, 0x0E, 0xE7, 0xD1, 0xAA, 0x1F);

        /// <summary>
        /// Display Name         Sample Pictures
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Pictures\Sample Pictures
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Sample Pictures
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Pictures\Sample Pictures
        /// </summary>
        public static Guid SamplePictures = new Guid(0xC4900540, 0x2379, 0x4C75, 0x84, 0x4B, 0x64, 0xE6, 0xFA, 0xF8, 0x71, 0x6B);

        /// <summary>
        /// Display Name         Sample Videos
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Videos\Sample Videos
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid SampleVideos = new Guid(0x859EAD94, 0x2E85, 0x48AD, 0xA7, 0x1A, 0x09, 0x69, 0xCB, 0x56, 0xA6, 0xCD);

        /// <summary>
        /// Display Name         Slide Shows
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Pictures\Slide Shows
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid PhotoAlbums = new Guid(0x69D2CF90, 0xFC33, 0x4FB7, 0x9A, 0x0C, 0xEB, 0xB0, 0xF0, 0xFC, 0xB4, 0x3C);

        /// <summary>
        /// Display Name         Public
        /// Folder Type          FIXED
        /// Default Path         %PUBLIC% (%SystemDrive%\Users\Public)
        /// CSIDL Equivalent     None, new for Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid Public = new Guid(0xDFDF76A2, 0xC82A, 0x4D63, 0x90, 0x6A, 0x56, 0x44, 0xAC, 0x45, 0x73, 0x85);

        /// <summary>
        /// Display Name        Programs and Features
        /// Folder Type         VIRTUAL
        /// Default Path        Not applicable—virtual folder
        /// CSIDL Equivalent    None
        /// Legacy Display Name Add or Remove Programs
        /// Legacy Default Path Not applicable—virtual folder
        /// </summary>
        public static Guid ChangeRemovePrograms = new Guid(0xdf7266ac, 0x9274, 0x4867, 0x8d, 0x55, 0x3b, 0xd6, 0x61, 0xde, 0x87, 0x2d);

        /// <summary>
        /// Display Name        Installed Updates
        /// Folder Type         VIRTUAL
        /// Default Path        Not applicable—virtual folder
        /// CSIDL Equivalent    None
        /// Legacy Display Name None, new in Windows Vista. In earlier versions of Microsoft Windows, the information on this page was included in Add or Remove Programs if the Show updates box was checked.
        /// Legacy Default Path Not applicable
        /// </summary>
        public static Guid AppUpdates = new Guid(0xa305ce99, 0xf527, 0x492b, 0x8b, 0x1a, 0x7e, 0x76, 0xfa, 0x98, 0xd6, 0xe4);

        /// <summary>
        /// Display Name        Get Programs
        /// Folder Type         VIRTUAL
        /// Default Path        Not applicable—virtual folder
        /// CSIDL Equivalent    None
        /// Legacy Display Name Add New Programs (found in the Add or Remove Programs item in the Control Panel)
        /// Legacy Default Path Not applicable—virtual folder
        /// </summary>
        public static Guid AddNewPrograms = new Guid(0xde61d971, 0x5ebc, 0x4f02, 0xa3, 0xa9, 0x6c, 0x82, 0x89, 0x5e, 0x5c, 0x04);

        /// <summary>
        /// Display Name         Downloads
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Downloads
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid Downloads = new Guid(0x374de290, 0x123f, 0x4565, 0x91, 0x64, 0x39, 0xc4, 0x92, 0x5e, 0x46, 0x7b);

        /// <summary>
        /// Display Name         Public Downloads
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Downloads
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid PublicDownloads = new Guid(0x3d644c9b, 0x1fb8, 0x4f30, 0x9b, 0x45, 0xf6, 0x70, 0x23, 0x5f, 0x79, 0xc0);

        /// <summary>
        /// Display Name         Searches
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Searches
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid SavedSearches = new Guid(0x7d1d3a04, 0xdebb, 0x4115, 0x95, 0xcf, 0x2f, 0x29, 0xda, 0x29, 0x20, 0xda);

        /// <summary>
        /// Display Name         Quick Launch
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Internet Explorer\Quick Launch
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Quick Launch
        /// Legacy Default Path  %APPDATA%\Microsoft\Internet Explorer\Quick Launch
        /// </summary>
        public static Guid QuickLaunch = new Guid(0x52a4f021, 0x7b75, 0x48a9, 0x9f, 0x6b, 0x4b, 0x87, 0xa2, 0x10, 0xbc, 0x8f);

        /// <summary>
        /// Display Name         Contacts
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Contacts
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid Contacts = new Guid(0x56784854, 0xc6cb, 0x462b, 0x81, 0x69, 0x88, 0xe3, 0x50, 0xac, 0xb8, 0x82);

        /// <summary>
        /// Display Name         Gadgets
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows Sidebar\Gadgets
        /// CSIDL Equivalent     None, new for Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid SidebarParts = new Guid(0xa75d362e, 0x50fc, 0x4fb7, 0xac, 0x2c, 0xa8, 0xbe, 0xaa, 0x31, 0x44, 0x93);

        /// <summary>
        /// Display Name         Gadgets
        /// Folder Type          COMMON
        /// Default Path         %ProgramFiles%\Windows Sidebar\Gadgets
        /// CSIDL Equivalent     None, new for Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid SidebarDefaultParts = new Guid(0x7b396e54, 0x9ec5, 0x4300, 0xbe, 0xa, 0x24, 0x82, 0xeb, 0xae, 0x1a, 0x26);

        /// <summary>
        /// FOLDERID_TreeProperties
        /// Not used.
        ///
        /// Tree property value folder
        /// </summary>
        public static Guid TreeProperties = new Guid(0x5b3749ad, 0xb49f, 0x49c1, 0x83, 0xeb, 0x15, 0x37, 0x0f, 0xbd, 0x48, 0x82);

        /// <summary>
        /// Display Name         GameExplorer
        /// Folder Type          COMMON
        /// Default Path         %ALLUSERSPROFILE%\Microsoft\Windows\GameExplorer
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid PublicGameTasks = new Guid(0xdebf2536, 0xe1a8, 0x4c59, 0xb6, 0xa2, 0x41, 0x45, 0x86, 0x47, 0x6a, 0xea);

        /// <summary>
        /// Display Name         GameExplorer
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\GameExplorer
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid GameTasks = new Guid(0x54fae61, 0x4dd8, 0x4787, 0x80, 0xb6, 0x9, 0x2, 0x20, 0xc4, 0xb7, 0x0);

        /// <summary>
        /// Display Name         Saved Games
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Saved Games
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid SavedGames = new Guid(0x4c5c32ff, 0xbb9d, 0x43b0, 0xb5, 0xb4, 0x2d, 0x72, 0xe5, 0x4e, 0xaa, 0xa4);

        /// <summary>
        /// Display Name         Games
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid Games = new Guid(0xcac52c1a, 0xb53d, 0x4edc, 0x92, 0xd7, 0x6b, 0x2e, 0x8a, 0xc1, 0x94, 0x34);

        /// <summary>
        /// Recorded TV
        /// </summary>
        public static Guid RecordedTV = new Guid(0xbd85e001, 0x112e, 0x431e, 0x98, 0x3b, 0x7b, 0x15, 0xac, 0x09, 0xff, 0xf1);

        /// <summary>
        /// Display Name         Microsoft Office Outlook
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid SearchMapi = new Guid(0x98ec0e18, 0x2098, 0x4d44, 0x86, 0x44, 0x66, 0x97, 0x93, 0x15, 0xa2, 0x81);

        /// <summary>
        /// Display Name         Offline Files
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid SearchCsc = new Guid(0xee32e446, 0x31ca, 0x4aba, 0x81, 0x4f, 0xa5, 0xeb, 0xd2, 0xfd, 0x6d, 0x5e);

        /// <summary>
        /// Display Name         Links
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Links
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid Links = new Guid(0xbfb9d5e0, 0xc6a9, 0x404c, 0xb2, 0xb2, 0xae, 0x6d, 0xb6, 0xaf, 0x49, 0x68);

        /// <summary>
        /// Display Name         The user's full name (for instance, Jean Philippe Bagel) entered when the user account was created.
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid UsersFiles = new Guid(0xf3ce0f7c, 0x4901, 0x4acc, 0x86, 0x48, 0xd5, 0xd4, 0x4b, 0x04, 0xef, 0x8f);

        /// <summary>
        /// Display Name         Search Results
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid SearchHome = new Guid(0x190337d1, 0xb8ca, 0x4121, 0xa6, 0x39, 0x6d, 0x47, 0x2d, 0x16, 0x97, 0x2a);

        /// <summary>
        /// Display Name         Original Images
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows Photo Gallery\Original Images
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static Guid OriginalImages = new Guid(0x2C36C0AA, 0x5812, 0x4b87, 0xbf, 0xd0, 0x4c, 0xd0, 0xdf, 0xb1, 0x9b, 0x39);
        #endregion

        #region Win7 KnownFolders Guids
        /// <summary>
        /// UserProgramFiles
        /// </summary>
        public static Guid UserProgramFiles = new Guid(0x5cd7aee2, 0x2219, 0x4a67, 0xb8, 0x5d, 0x6c, 0x9c, 0xe1, 0x56, 0x60, 0xcb);

        /// <summary>
        /// UserProgramFilesCommon
        /// </summary>
        public static Guid UserProgramFilesCommon = new Guid(0xbcbd3057, 0xca5c, 0x4622, 0xb4, 0x2d, 0xbc, 0x56, 0xdb, 0x0a, 0xe5, 0x16);

        /// <summary>
        /// Ringtones
        /// </summary>
        public static Guid Ringtones = new Guid(0xC870044B, 0xF49E, 0x4126, 0xA9, 0xC3, 0xB5, 0x2A, 0x1F, 0xF4, 0x11, 0xE8);

        /// <summary>
        /// PublicRingtones
        /// </summary>
        public static Guid PublicRingtones = new Guid(0xE555AB60, 0x153B, 0x4D17, 0x9F, 0x04, 0xA5, 0xFE, 0x99, 0xFC, 0x15, 0xEC);

        /// <summary>
        /// UsersLibraries
        /// </summary>
        public static Guid UsersLibraries = new Guid(0xa302545d, 0xdeff, 0x464b, 0xab, 0xe8, 0x61, 0xc8, 0x64, 0x8d, 0x93, 0x9b);

        /// <summary>
        /// DocumentsLibrary
        /// </summary>
        public static Guid DocumentsLibrary = new Guid(0x7b0db17d, 0x9cd2, 0x4a93, 0x97, 0x33, 0x46, 0xcc, 0x89, 0x02, 0x2e, 0x7c);

        /// <summary>
        /// MusicLibrary
        /// </summary>
        public static Guid MusicLibrary = new Guid(0x2112ab0a, 0xc86a, 0x4ffe, 0xa3, 0x68, 0xd, 0xe9, 0x6e, 0x47, 0x1, 0x2e);

        /// <summary>
        /// PicturesLibrary
        /// </summary>
        public static Guid PicturesLibrary = new Guid(0xa990ae9f, 0xa03b, 0x4e80, 0x94, 0xbc, 0x99, 0x12, 0xd7, 0x50, 0x41, 0x4);

        /// <summary>
        /// VideosLibrary
        /// </summary>
        public static Guid VideosLibrary = new Guid(0x491e922f, 0x5643, 0x4af4, 0xa7, 0xeb, 0x4e, 0x7a, 0x13, 0x8d, 0x81, 0x74);

        /// <summary>
        /// RecordedTVLibrary
        /// </summary>
        public static Guid RecordedTVLibrary = new Guid(0x1a6fdba2, 0xf42d, 0x4358, 0xa7, 0x98, 0xb7, 0x4d, 0x74, 0x59, 0x26, 0xc5);

        /// <summary>
        /// OtherUsers
        /// </summary>
        public static Guid OtherUsers = new Guid(0x52528a6b, 0xb9e3, 0x4add, 0xb6, 0xd, 0x58, 0x8c, 0x2d, 0xba, 0x84, 0x2d);

        /// <summary>
        /// DeviceMetadataStore
        /// </summary>
        public static Guid DeviceMetadataStore = new Guid(0x5ce4a5e9, 0xe4eb, 0x479d, 0xb8, 0x9f, 0x13, 0x0c, 0x02, 0x88, 0x61, 0x55);

        /// <summary>
        /// Libraries
        /// </summary>
        public static Guid Libraries = new Guid(0x1b3ea5dc, 0xb587, 0x4786, 0xb4, 0xef, 0xbd, 0x1d, 0xc3, 0x32, 0xae, 0xae);

        /// <summary>
        /// UserPinned
        /// </summary>
        public static Guid UserPinned = new Guid(0x9e3995ab, 0x1f9c, 0x4f13, 0xb8, 0x27, 0x48, 0xb2, 0x4b, 0x6c, 0x71, 0x74);

        /// <summary>
        /// ImplicitAppShortcuts
        /// </summary>
        public static Guid ImplicitAppShortcuts = new Guid(0xbcb5256f, 0x79f6, 0x4cee, 0xb7, 0x25, 0xdc, 0x34, 0xe4, 0x2, 0xfd, 0x46);
        #endregion
    }
}
