namespace WSF.Shell.Interop.KnownFolders
{
    using WSF.IDs;
    using System;

    /// <summary>
    /// KnownFolders contain the known folder ids for windows.
    ///
    /// The KNOWNFOLDERID constants represent GUIDs that identify standard folders registered with the
    /// system as Known Folders. These folders are installed with Windows Vista and later operating
    /// systems, and a computer will have only folders appropriate to it installed. These values are
    /// defined in Knownfolders.h.
    /// </summary>
    /// <remarks>
    /// See:
    ///     http://msdn.microsoft.com/en-us/library/windows/desktop/dd378457.aspx
    /// For details on known folders.
    /// </remarks>
    public sealed class KnownFolderGuids
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Display Name         Network
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_NETWORK, CSIDL_COMPUTERSNEARME
        /// Legacy Display Name  My Network Places
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public static readonly Guid FOLDERID_NetworkFolder = new Guid(KF_ID.ID_FOLDERID_NetworkFolder);

        /// <summary>
        /// Display Name          Computer
        /// Folder Type           VIRTUAL
        /// Default Path          Not applicable—virtual folder
        /// CSIDL Equivalent      CSIDL_DRIVES
        /// Legacy Display Name   My Computer
        /// Legacy Default Path   Not applicable—virtual folder
        /// </summary>
        public static readonly Guid FOLDERID_ComputerFolder = new Guid(KF_ID.ID_FOLDERID_ComputerFolder);

        /// <summary>
        /// Display Name         Internet Explorer
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_INTERNET
        /// Legacy Display Name  Internet Explorer
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public static readonly Guid FOLDERID_InternetFolder = new Guid(KF_ID.ID_FOLDERID_InternetFolder);

        /// <summary>
        /// Display Name         Control Panel
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_CONTROLS
        /// Legacy Display Name  Control Panel
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public static readonly Guid FOLDERID_ControlPanelFolder = new Guid(KF_ID.ID_FOLDERID_ControlPanelFolder);

        /// <summary>
        /// Display Name         Printers
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_PRINTERS
        /// Legacy Display Name  Printers and Faxes
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public static readonly Guid FOLDERID_PrintersFolder = new Guid(KF_ID.ID_FOLDERID_PrintersFolder);

        /// <summary>
        /// Display Name         Sync Center
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_SyncManagerFolder = new Guid(KF_ID.ID_FOLDERID_SyncManagerFolder);

        /// <summary>
        /// Display Name         Sync Setup
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_SyncSetupFolder = new Guid(KF_ID.ID_FOLDERID_SyncSetupFolder);

        /// <summary>
        /// Display Name          Conflicts
        /// Folder Type           VIRTUAL
        /// Default Path          Not applicable—virtual folder
        /// CSIDL Equivalent      None, new in Windows Vista
        /// Legacy Display Name   Not applicable. This KNOWNFOLDERID refers to the Windows Vista Synchronization Manager. It is not the folder referenced by the older ISyncMgrConflictFolder.
        /// Legacy Default Path   Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_ConflictFolder = new Guid(KF_ID.ID_FOLDERID_ConflictFolder);

        /// <summary>
        /// Display Name         Control Panel
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_CONTROLS
        /// Legacy Display Name  Control Panel
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public static readonly Guid FOLDERID_SyncResultsFolder = new Guid(KF_ID.ID_FOLDERID_SyncResultsFolder);

        /// <summary>
        /// Display Name         Recycle Bin
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_BITBUCKET
        /// Legacy Display Name  Recycle Bin
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public static readonly Guid FOLDERID_RecycleBinFolder = new Guid(KF_ID.ID_FOLDERID_RecycleBinFolder);

        /// <summary>
        /// Display Name         Network Connections
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_CONNECTIONS
        /// Legacy Display Name  Network Connections
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public static readonly Guid FOLDERID_ConnectionsFolder = new Guid(KF_ID.ID_FOLDERID_ConnectionsFolder);

        /// <summary>
        /// Display Name         Fonts
        /// Folder Type          FIXED
        /// Default Path         %windir%\Fonts
        /// CSIDL Equivalent     CSIDL_FONTS
        /// Legacy Display Name  Fonts
        /// Legacy Default Path  %windir%\Fonts
        /// </summary>
        public static readonly Guid FOLDERID_Fonts = new Guid(KF_ID.ID_FOLDERID_Fonts);

        /// <summary>
        /// Display Name         Desktop
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Desktop
        /// CSIDL Equivalent     CSIDL_DESKTOP, CSIDL_DESKTOPDIRECTORY
        /// Legacy Display Name  Desktop
        /// Legacy Default Path  %USERPROFILE%\Desktop
        /// </summary>
        public static readonly Guid FOLDERID_Desktop = new Guid(KF_ID.ID_FOLDERID_Desktop);

        /// <summary>
        /// Display Name         Startup
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Start Menu\Programs\StartUp
        /// CSIDL Equivalent     CSIDL_STARTUP, CSIDL_ALTSTARTUP
        /// Legacy Display Name  Startup
        /// Legacy Default Path  %USERPROFILE%\Start Menu\Programs\StartUp
        /// </summary>
        public static readonly Guid FOLDERID_Startup = new Guid(KF_ID.ID_FOLDERID_Startup);

        /// <summary>
        /// Display Name         Programs
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Start Menu\Programs
        /// CSIDL Equivalent     CSIDL_PROGRAMS
        /// Legacy Display Name  Programs
        /// Legacy Default Path  %USERPROFILE%\Start Menu\Programs
        /// </summary>
        public static readonly Guid FOLDERID_Programs = new Guid(KF_ID.ID_FOLDERID_Programs);

        /// <summary>
        /// Display Name         Start Menu
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Start Menu
        /// CSIDL Equivalent     CSIDL_STARTMENU
        /// Legacy Display Name  Start Menu
        /// Legacy Default Path  %USERPROFILE%\Start Menu
        /// </summary>
        public static readonly Guid FOLDERID_StartMenu = new Guid(KF_ID.ID_FOLDERID_StartMenu);

        /// <summary>
        /// Display Name         Recent Items
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Recent
        /// CSIDL Equivalent     CSIDL_RECENT
        /// Legacy Display Name  My Recent Documents
        /// Legacy Default Path  %USERPROFILE%\Recent
        /// </summary>
        public static readonly Guid FOLDERID_Recent = new Guid(KF_ID.ID_FOLDERID_Recent);

        /// <summary>
        /// Display Name         SendTo
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\SendTo
        /// CSIDL Equivalent     CSIDL_SENDTO
        /// Legacy Display Name  SendTo
        /// Legacy Default Path  %USERPROFILE%\SendTo
        /// </summary>
        public static readonly Guid FOLDERID_SendTo = new Guid(KF_ID.ID_FOLDERID_SendTo);

        /// <summary>
        /// Display Name         Documents
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Documents
        /// CSIDL Equivalent     CSIDL_MYDOCUMENTS, CSIDL_PERSONAL
        /// Legacy Display Name  My Documents
        /// Legacy Default Path  %USERPROFILE%\My Documents
        /// </summary>
        public static readonly Guid FOLDERID_Documents = new Guid(KF_ID.ID_FOLDERID_Documents);

        /// <summary>
        /// Display Name         Favorites
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Favorites
        /// CSIDL Equivalent     CSIDL_FAVORITES, CSIDL_COMMON_FAVORITES
        /// Legacy Display Name  Favorites
        /// Legacy Default Path  %USERPROFILE%\Favorites
        /// </summary>
        public static readonly Guid FOLDERID_Favorites = new Guid(KF_ID.ID_FOLDERID_Favorites);

        /// <summary>
        /// Display Name         Network Shortcuts
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Network Shortcuts
        /// CSIDL Equivalent     CSIDL_NETHOOD
        /// Legacy Display Name  NetHood
        /// Legacy Default Path  %USERPROFILE%\NetHood
        /// </summary>
        public static readonly Guid FOLDERID_NetHood = new Guid(KF_ID.ID_FOLDERID_NetHood);

        /// <summary>
        /// Display Name         Printer Shortcuts
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Printer Shortcuts
        /// CSIDL Equivalent     CSIDL_PRINTHOOD
        /// Legacy Display Name  PrintHood
        /// Legacy Default Path  %USERPROFILE%\PrintHood
        /// </summary>
        public static readonly Guid FOLDERID_PrintHood = new Guid(KF_ID.ID_FOLDERID_PrintHood);

        /// <summary>
        /// Display Name         Templates
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Templates
        /// CSIDL Equivalent     CSIDL_TEMPLATES
        /// Legacy Display Name  Templates
        /// Legacy Default Path  %USERPROFILE%\Templates
        /// </summary>
        public static readonly Guid FOLDERID_Templates = new Guid(KF_ID.ID_FOLDERID_Templates);

        /// <summary>
        /// Display Name          Startup
        /// Folder Type           COMMON
        /// Default Path          %ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs\StartUp
        /// CSIDL Equivalent      CSIDL_COMMON_STARTUP, CSIDL_COMMON_ALTSTARTUP
        /// Legacy Display Name   Startup
        /// Legacy Default Path   %ALLUSERSPROFILE%\Start Menu\Programs\StartUp
        /// </summary>
        public static readonly Guid FOLDERID_CommonStartup = new Guid(KF_ID.ID_FOLDERID_CommonStartup);

        /// <summary>
        /// Display Name          Programs
        /// Folder Type           COMMON
        /// Default Path          %ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs
        /// CSIDL Equivalent      CSIDL_COMMON_PROGRAMS
        /// Legacy Display Name   Programs
        /// Legacy Default Path   %ALLUSERSPROFILE%\Start Menu\Programs
        /// </summary>
        public static readonly Guid FOLDERID_CommonPrograms = new Guid(KF_ID.ID_FOLDERID_CommonPrograms);

        /// <summary>
        /// Display Name          Start Menu
        /// Folder Type           COMMON
        /// Default Path          %ALLUSERSPROFILE%\Microsoft\Windows\Start Menu
        /// CSIDL Equivalent      CSIDL_COMMON_STARTMENU
        /// Legacy Display Name   Start Menu
        /// Legacy Default Path   %ALLUSERSPROFILE%\Start Menu
        /// </summary>
        public static readonly Guid FOLDERID_CommonStartMenu = new Guid(KF_ID.ID_FOLDERID_CommonStartMenu);

        /// <summary>
        /// Display Name         Public Desktop
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Desktop
        /// CSIDL Equivalent     CSIDL_COMMON_DESKTOPDIRECTORY
        /// Legacy Display Name  Desktop
        /// Legacy Default Path  %ALLUSERSPROFILE%\Desktop
        /// </summary>
        public static readonly Guid FOLDERID_PublicDesktop = new Guid(KF_ID.ID_FOLDERID_PublicDesktop);

        /// <summary>
        /// Display Name         ProgramData
        /// Folder Type          FIXED
        /// Default Path         %ALLUSERSPROFILE% (%ProgramData%, %SystemDrive%\ProgramData)
        /// CSIDL Equivalent     CSIDL_COMMON_APPDATA
        /// Legacy Display Name  Application Data
        /// Legacy Default Path  %ALLUSERSPROFILE%\Application Data
        /// </summary>
        public static readonly Guid FOLDERID_ProgramData = new Guid(KF_ID.ID_FOLDERID_ProgramData);

        /// <summary>
        /// Display Name          Templates
        /// Folder Type           COMMON
        /// Default Path          %ALLUSERSPROFILE%\Templates
        /// CSIDL Equivalent      CSIDL_COMMON_TEMPLATES
        /// Legacy Display Name   Templates
        /// Legacy Default Path   %ALLUSERSPROFILE%\Templates
        /// </summary>
        public static readonly Guid FOLDERID_CommonTemplates = new Guid(KF_ID.ID_FOLDERID_CommonTemplates);

        /// <summary>
        /// Display Name         Public Documents
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Documents
        /// CSIDL Equivalent     CSIDL_COMMON_DOCUMENTS
        /// Legacy Display Name  Shared Documents
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents
        /// </summary>
        public static readonly Guid FOLDERID_PublicDocuments = new Guid(KF_ID.ID_FOLDERID_PublicDocuments);

        /// <summary>
        /// Display Name         Roaming
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA% (%USERPROFILE%\AppData\Roaming)
        /// CSIDL Equivalent     CSIDL_APPDATA
        /// Legacy Display Name  Application Data
        /// Legacy Default Path  %APPDATA% (%USERPROFILE%\Application Data)
        /// </summary>
        public static readonly Guid FOLDERID_RoamingAppData = new Guid(KF_ID.ID_FOLDERID_RoamingAppData);

        /// <summary>
        /// Display Name         Local
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA% (%USERPROFILE%\AppData\Local)
        /// CSIDL Equivalent     CSIDL_LOCAL_APPDATA
        /// Legacy Display Name  Application Data
        /// Legacy Default Path  %USERPROFILE%\Local Settings\Application Data
        /// </summary>
        public static readonly Guid FOLDERID_LocalAppData = new Guid(KF_ID.ID_FOLDERID_LocalAppData);

        /// <summary>
        /// Display Name         LocalLow
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\AppData\LocalLow
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_LocalAppDataLow = new Guid(KF_ID.ID_FOLDERID_LocalAppDataLow);

        /// <summary>
        /// Display Name         Temporary Internet Files
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\Temporary Internet Files
        /// CSIDL Equivalent     CSIDL_INTERNET_CACHE
        /// Legacy Display Name  Temporary Internet Files
        /// Legacy Default Path  %USERPROFILE%\Local Settings\Temporary Internet Files
        /// </summary>
        public static readonly Guid FOLDERID_InternetCache = new Guid(KF_ID.ID_FOLDERID_InternetCache);

        /// <summary>
        /// Display Name         Cookies
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Cookies
        /// CSIDL Equivalent     CSIDL_COOKIES
        /// Legacy Display Name  Cookies
        /// Legacy Default Path  %USERPROFILE%\Cookies
        /// </summary>
        public static readonly Guid FOLDERID_Cookies = new Guid(KF_ID.ID_FOLDERID_Cookies);

        /// <summary>
        /// Display Name         History
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\History
        /// CSIDL Equivalent     CSIDL_HISTORY
        /// Legacy Display Name  History
        /// Legacy Default Path  %USERPROFILE%\Local Settings\History
        /// </summary>
        public static readonly Guid FOLDERID_History = new Guid(KF_ID.ID_FOLDERID_History);

        /// <summary>
        /// Display Name         System32
        /// Folder Type          FIXED
        /// Default Path         %windir%\system32
        /// CSIDL Equivalent     CSIDL_SYSTEM
        /// Legacy Display Name  system32
        /// Legacy Default Path  %windir%\system32
        /// </summary>
        public static readonly Guid FOLDERID_System = new Guid(KF_ID.ID_FOLDERID_System);

        /// <summary>
        /// Display Name         System32
        /// Folder Type          FIXED
        /// Default Path         %windir%\system32
        /// CSIDL Equivalent     CSIDL_SYSTEMX86
        /// Legacy Display Name  system32
        /// Legacy Default Path  %windir%\system32
        /// </summary>
        public static readonly Guid FOLDERID_SystemX86 = new Guid(KF_ID.ID_FOLDERID_SystemX86);

        /// <summary>
        /// Display Name         Windows
        /// Folder Type          FIXED
        /// Default Path         %windir%
        /// CSIDL Equivalent     CSIDL_WINDOWS
        /// Legacy Display Name  WINDOWS
        /// Legacy Default Path  %windir%
        /// </summary>
        public static readonly Guid FOLDERID_Windows = new Guid(KF_ID.ID_FOLDERID_Windows);

        /// <summary>
        /// Display Name         The user's username (%USERNAME%)
        /// Folder Type          FIXED
        /// Default Path         %USERPROFILE% (%SystemDrive%\Users\%USERNAME%)
        /// CSIDL Equivalent     CSIDL_PROFILE
        /// Legacy Display Name  The user's username (%USERNAME%)
        /// Legacy Default Path  %USERPROFILE% (%SystemDrive%\Documents and Settings\%USERNAME%)
        /// </summary>
        public static readonly Guid FOLDERID_Profile = new Guid(KF_ID.ID_FOLDERID_Profile);

        /// <summary>
        /// Display Name         Pictures
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Pictures
        /// CSIDL Equivalent     CSIDL_MYPICTURES
        /// Legacy Display Name  My Pictures
        /// Legacy Default Path  %USERPROFILE%\My Documents\My Pictures
        /// </summary>
        public static readonly Guid FOLDERID_Pictures = new Guid(KF_ID.ID_FOLDERID_Pictures);

        /// <summary>
        /// Display Name         Program Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles% (%SystemDrive%\Program Files)
        /// CSIDL Equivalent     CSIDL_PROGRAM_FILESX86
        /// Legacy Display Name  Program Files
        /// Legacy Default Path  %ProgramFiles% (%SystemDrive%\Program Files)
        /// </summary>
        public static readonly Guid FOLDERID_ProgramFilesX86 = new Guid(KF_ID.ID_FOLDERID_ProgramFilesX86);

        /// <summary>
        /// Display Name         Common Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles%\Common Files
        /// CSIDL Equivalent     CSIDL_PROGRAM_FILES_COMMONX86
        /// Legacy Display Name  Common Files
        /// Legacy Default Path  %ProgramFiles%\Common Files
        /// </summary>
        public static readonly Guid FOLDERID_ProgramFilesCommonX86 = new Guid(KF_ID.ID_FOLDERID_ProgramFilesCommonX86);

        /// <summary>
        /// Display Name         Program Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles% (%SystemDrive%\Program Files)
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Program Files
        /// Legacy Default Path  %ProgramFiles% (%SystemDrive%\Program Files)
        /// </summary>
        public static readonly Guid FOLDERID_ProgramFilesX64 = new Guid(KF_ID.ID_FOLDERID_ProgramFilesX64);

        /// <summary>
        /// Display Name         Common Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles%\Common Files
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Common Files
        /// Legacy Default Path  %ProgramFiles%\Common Files
        /// </summary>
        public static readonly Guid FOLDERID_ProgramFilesCommonX64 = new Guid(KF_ID.ID_FOLDERID_ProgramFilesCommonX64);

        /// <summary>
        /// Display Name         Program Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles% (%SystemDrive%\Program Files)
        /// CSIDL Equivalent     CSIDL_PROGRAM_FILES
        /// Legacy Display Name  Program Files
        /// Legacy Default Path  %ProgramFiles% (%SystemDrive%\Program Files)
        /// </summary>
        public static readonly Guid FOLDERID_ProgramFiles = new Guid(KF_ID.ID_FOLDERID_ProgramFiles);

        /// <summary>
        /// Display Name         Common Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles%\Common Files
        /// CSIDL Equivalent     CSIDL_PROGRAM_FILES_COMMON
        /// Legacy Display Name  Common Files
        /// Legacy Default Path  %ProgramFiles%\Common Files
        /// </summary>
        public static readonly Guid FOLDERID_ProgramFilesCommon = new Guid(KF_ID.ID_FOLDERID_ProgramFilesCommon);

        /// <summary>
        /// UsersLibraries
        /// </summary>
        public static readonly Guid FOLDERID_UsersLibraries = new Guid(KF_ID.ID_FOLDERID_UsersLibraries);
        #region Win7 KnownFolders

        /// <summary>
        /// UserProgramFiles
        /// </summary>
        public static readonly Guid FOLDERID_UserProgramFiles = new Guid(KF_ID.ID_FOLDERID_UserProgramFiles);
        
        /// <summary>
        /// UserProgramFilesCommon
        /// </summary>
        public static readonly Guid FOLDERID_UserProgramFilesCommon = new Guid(KF_ID.ID_FOLDERID_UserProgramFilesCommon);

        /// <summary>
        /// Ringtones
        /// </summary>
        public static readonly Guid FOLDERID_Ringtones = new Guid(KF_ID.ID_FOLDERID_Ringtones);

        /// <summary>
        /// PublicRingtones
        /// </summary>
        public static readonly Guid FOLDERID_PublicRingtones = new Guid(KF_ID.ID_FOLDERID_PublicRingtones);
        
        /// <summary>
        /// DocumentsLibrary
        /// </summary>
        public static readonly Guid FOLDERID_DocumentsLibrary = new Guid(KF_ID.ID_FOLDERID_DocumentsLibrary);

        /// <summary>
        /// MusicLibrary
        /// </summary>
        public static readonly Guid FOLDERID_MusicLibrary = new Guid(KF_ID.ID_FOLDERID_MusicLibrary);

        /// <summary>
        /// PicturesLibrary
        /// </summary>
        public static readonly Guid FOLDERID_PicturesLibrary = new Guid(KF_ID.ID_FOLDERID_PicturesLibrary);

        /// <summary>
        /// VideosLibrary
        /// </summary>
        public static readonly Guid FOLDERID_VideosLibrary = new Guid(KF_ID.ID_FOLDERID_VideosLibrary);

        /// <summary>
        /// RecordedTVLibrary
        /// </summary>
        public static readonly Guid FOLDERID_RecordedTVLibrary = new Guid(KF_ID.ID_FOLDERID_RecordedTVLibrary);
        
        /// <summary>
        /// OtherUsers
        /// </summary>
        public static readonly Guid FOLDERID_HomeGroup = new Guid(KF_ID.ID_FOLDERID_HomeGroup);
        
        /// <summary>
        /// DeviceMetadataStore
        /// </summary>        
        public static readonly Guid FOLDERID_DeviceMetadataStore = new Guid(KF_ID.ID_FOLDERID_DeviceMetadataStore);

        /// <summary>
        /// Libraries
        /// </summary>
        public static readonly Guid FOLDERID_Libraries = new Guid(KF_ID.ID_FOLDERID_Libraries);

        /// <summary>
        /// Display Name        Libraries
        /// Folder Type         COMMON
        /// Default Path        %ALLUSERSPROFILE%\Microsoft\Windows\Libraries
        /// CSIDL Equivalent    None, value introduced in Windows 7
        /// Legacy Display Name Not applicable
        /// Legacy Default Path Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_PublicLibraries = new Guid(KF_ID.ID_FOLDERID_PublicLibraries);
        
        /// <summary>
        /// UserPinned
        /// </summary>
        public static readonly Guid FOLDERID_UserPinned = new Guid(KF_ID.ID_FOLDERID_UserPinned);

        /// <summary>
        /// ImplicitAppShortcuts
        /// </summary>
        public static readonly Guid FOLDERID_ImplicitAppShortcuts = new Guid(KF_ID.ID_FOLDERID_ImplicitAppShortcuts);
        #endregion Win7 KnownFolders

        /// <summary>
        /// Display Name        Administrative Tools
        /// Folder Type         PERUSER
        /// Default Path        %APPDATA%\Microsoft\Windows\Start Menu\Programs\Administrative Tools
        /// CSIDL Equivalent    CSIDL_ADMINTOOLS
        /// Legacy Display Name Administrative Tools
        /// Legacy Default Path %USERPROFILE%\Start Menu\Programs\Administrative Tools
        /// </summary>
        public static readonly Guid FOLDERID_AdminTools = new Guid(KF_ID.ID_FOLDERID_AdminTools);

        /// <summary>
        /// Display Name            Administrative Tools
        /// Folder Type             COMMON
        /// Default Path            %ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs\Administrative Tools
        /// CSIDL Equivalent        CSIDL_COMMON_ADMINTOOLS
        /// Legacy Display Name     Administrative Tools
        /// Legacy Default Path     %ALLUSERSPROFILE%\Start Menu\Programs\Administrative Tools
        /// </summary>
        public static readonly Guid FOLDERID_CommonAdminTools = new Guid(KF_ID.ID_FOLDERID_CommonAdminTools);

        /// <summary>
        /// Display Name         Music
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Music
        /// CSIDL Equivalent     CSIDL_MYMUSIC
        /// Legacy Display Name  My Music
        /// Legacy Default Path  %USERPROFILE%\My Documents\My Music
        /// </summary>
        public static readonly Guid FOLDERID_Music = new Guid(KF_ID.ID_FOLDERID_Music);

        /// <summary>
        /// Display Name         Videos
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Videos
        /// CSIDL                CSIDL_MYVIDEO
        /// Legacy Display Name  My Videos
        /// Legacy Default Path  %USERPROFILE%\My Documents\My Videos
        /// </summary>
        public static readonly Guid FOLDERID_Videos = new Guid(KF_ID.ID_FOLDERID_Videos);

        /// <summary>
        /// Display Name         Public Pictures
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Pictures
        /// CSIDL Equivalent     CSIDL_COMMON_PICTURES
        /// Legacy Display Name  Shared Pictures
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Pictures
        /// </summary>
        public static readonly Guid FOLDERID_PublicPictures = new Guid(KF_ID.ID_FOLDERID_PublicPictures);

        /// <summary>
        /// Display Name         Public Music
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Music
        /// CSIDL Equivalent     CSIDL_COMMON_MUSIC
        /// Legacy Display Name  Shared Music
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Music
        /// </summary>
        public static readonly Guid FOLDERID_PublicMusic = new Guid(KF_ID.ID_FOLDERID_PublicMusic);

        /// <summary>
        /// Display Name         Public Videos
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Videos
        /// CSIDL Equivalent     CSIDL_COMMON_VIDEO
        /// Legacy Display Name  Shared Video
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Videos
        /// </summary>
        public static readonly Guid FOLDERID_PublicVideos = new Guid(KF_ID.ID_FOLDERID_PublicVideos);

        /// <summary>
        /// Display Name         Resources
        /// Folder Type          FIXED
        /// Default Path         %windir%\Resources
        /// CSIDL Equivalent     CSIDL_RESOURCES
        /// Legacy Display Name  Resources
        /// Legacy Default Path  %windir%\Resources
        /// </summary>
        public static readonly Guid FOLDERID_ResourceDir = new Guid(KF_ID.ID_FOLDERID_ResourceDir);

        /// <summary>
        /// Display Name         None
        /// Folder Type          FIXED
        /// Default Path         %windir%\resources\0409 (code page)
        /// CSIDL Equivalent     CSIDL_RESOURCES_LOCALIZED
        /// Legacy Display Name  None
        /// Legacy Default Path  %windir%\resources\0409 (code page)
        /// </summary>
        public static readonly Guid FOLDERID_LocalizedResourcesDir = new Guid(KF_ID.ID_FOLDERID_LocalizedResourcesDir);

        /// <summary>
        /// Display Name        OEM Links
        /// Folder Type         COMMON
        /// Default Path        %ALLUSERSPROFILE%\OEM Links
        /// CSIDL Equivalent    CSIDL_COMMON_OEM_LINKS
        /// Legacy Display Name OEM Links
        /// Legacy Default Path %ALLUSERSPROFILE%\OEM Links
        /// </summary>
        public static readonly Guid FOLDERID_CommonOEMLinks = new Guid(KF_ID.ID_FOLDERID_CommonOEMLinks);

        /// <summary>
        /// Display Name        Temporary Burn Folder
        /// Folder Type         PERUSER
        /// Default Path        %LOCALAPPDATA%\Microsoft\Windows\Burn\Burn
        /// CSIDL Equivalent    CSIDL_CDBURN_AREA
        /// Legacy Display Name CD Burning
        /// Legacy Default Path %USERPROFILE%\Local Settings\Application Data\Microsoft\CD Burning
        /// </summary>
        public static readonly Guid FOLDERID_CDBurning = new Guid(KF_ID.ID_FOLDERID_CDBurning);

        /// <summary>
        /// Display Name         Users
        /// Folder Type          FIXED
        /// Default Path         %SystemDrive%\Users
        /// CSIDL Equivalent     None, new for Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_UserProfiles = new Guid(KF_ID.ID_FOLDERID_UserProfiles);

        /// <summary>
        /// Display Name         Playlists
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Music\Playlists
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_Playlists = new Guid(KF_ID.ID_FOLDERID_Playlists);

        /// <summary>
        /// Display Name         Sample Playlists
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Music\Sample Playlists
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_SamplePlaylists = new Guid(KF_ID.ID_FOLDERID_SamplePlaylists);

        /// <summary>
        /// Display Name         Sample Music
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Music\Sample Music
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Sample Music
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Music\Sample Music
        /// </summary>
        public static readonly Guid FOLDERID_SampleMusic = new Guid(KF_ID.ID_FOLDERID_SampleMusic);

        /// <summary>
        /// Display Name         Sample Pictures
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Pictures\Sample Pictures
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Sample Pictures
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Pictures\Sample Pictures
        /// </summary>
        public static readonly Guid FOLDERID_SamplePictures = new Guid(KF_ID.ID_FOLDERID_SamplePictures);

        /// <summary>
        /// Display Name         Sample Videos
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Videos\Sample Videos
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_SampleVideos = new Guid(KF_ID.ID_FOLDERID_SampleVideos);

        /// <summary>
        /// Display Name         Slide Shows
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Pictures\Slide Shows
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_PhotoAlbums = new Guid(KF_ID.ID_FOLDERID_PhotoAlbums);

        /// <summary>
        /// Display Name         Public
        /// Folder Type          FIXED
        /// Default Path         %PUBLIC% (%SystemDrive%\Users\Public)
        /// CSIDL Equivalent     None, new for Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_Public = new Guid(KF_ID.ID_FOLDERID_Public);

        /// <summary>
        /// Display Name        Programs and Features
        /// Folder Type         VIRTUAL
        /// Default Path        Not applicable—virtual folder
        /// CSIDL Equivalent    None
        /// Legacy Display Name Add or Remove Programs
        /// Legacy Default Path Not applicable—virtual folder
        /// </summary>
        public static readonly Guid FOLDERID_ChangeRemovePrograms = new Guid(KF_ID.ID_FOLDERID_ChangeRemovePrograms);

        /// <summary>
        /// Display Name        Installed Updates
        /// Folder Type         VIRTUAL
        /// Default Path        Not applicable—virtual folder
        /// CSIDL Equivalent    None
        /// Legacy Display Name None, new in Windows Vista. In earlier versions of Microsoft Windows, the information on this page was included in Add or Remove Programs if the Show updates box was checked.
        /// Legacy Default Path Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_AppUpdates = new Guid(KF_ID.ID_FOLDERID_AppUpdates);

        /// <summary>
        /// Display Name        Get Programs
        /// Folder Type         VIRTUAL
        /// Default Path        Not applicable—virtual folder
        /// CSIDL Equivalent    None
        /// Legacy Display Name Add New Programs (found in the Add or Remove Programs item in the Control Panel)
        /// Legacy Default Path Not applicable—virtual folder
        /// </summary>
        public static readonly Guid FOLDERID_AddNewPrograms = new Guid(KF_ID.ID_FOLDERID_AddNewPrograms);

        /// <summary>
        /// Display Name         Downloads
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Downloads
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_Downloads = new Guid(KF_ID.ID_FOLDERID_Downloads);

        /// <summary>
        /// Display Name         Public Downloads
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Downloads
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_PublicDownloads = new Guid(KF_ID.ID_FOLDERID_PublicDownloads);

        /// <summary>
        /// Display Name         Searches
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Searches
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_SavedSearches = new Guid(KF_ID.ID_FOLDERID_SavedSearches);

        /// <summary>
        /// Display Name         Quick Launch
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Internet Explorer\Quick Launch
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Quick Launch
        /// Legacy Default Path  %APPDATA%\Microsoft\Internet Explorer\Quick Launch
        /// </summary>
        public static readonly Guid FOLDERID_QuickLaunch = new Guid(KF_ID.ID_FOLDERID_QuickLaunch);

        /// <summary>
        /// Display Name         Contacts
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Contacts
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_Contacts = new Guid(KF_ID.ID_FOLDERID_Contacts);

        /// <summary>
        /// Display Name         Gadgets
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows Sidebar\Gadgets
        /// CSIDL Equivalent     None, new for Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_SidebarParts = new Guid(KF_ID.ID_FOLDERID_SidebarParts);

        /// <summary>
        /// Display Name         Gadgets
        /// Folder Type          COMMON
        /// Default Path         %ProgramFiles%\Windows Sidebar\Gadgets
        /// CSIDL Equivalent     None, new for Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_SidebarDefaultParts = new Guid(KF_ID.ID_FOLDERID_SidebarDefaultParts);

        /// <summary>
        /// Display Name         GameExplorer
        /// Folder Type          COMMON
        /// Default Path         %ALLUSERSPROFILE%\Microsoft\Windows\GameExplorer
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_PublicGameTasks = new Guid(KF_ID.ID_FOLDERID_PublicGameTasks);

        /// <summary>
        /// Display Name         GameExplorer
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\GameExplorer
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_GameTasks = new Guid(KF_ID.ID_FOLDERID_GameTasks);

        /// <summary>
        /// Display Name         Saved Games
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Saved Games
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_SavedGames = new Guid(KF_ID.ID_FOLDERID_SavedGames);

        /// <summary>
        /// Display Name         Saved Games
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Saved Games
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_Games = new Guid(KF_ID.ID_FOLDERID_Games);

        /// <summary>
        /// Display Name         Microsoft Office Outlook
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_SEARCH_MAPI = new Guid(KF_ID.ID_FOLDERID_SEARCH_MAPI);

        /// <summary>
        /// Display Name         Offline Files
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_SEARCH_CSC = new Guid(KF_ID.ID_FOLDERID_SEARCH_CSC);

        /// <summary>
        /// Display Name         Links
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Links
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_Links = new Guid(KF_ID.ID_FOLDERID_Links);

        /// <summary>
        /// Display Name         The user's full name (for instance, Jean Philippe Bagel) entered when the user account was created.
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_UsersFiles = new Guid(KF_ID.ID_FOLDERID_UsersFiles);

        /// <summary>
        /// Display Name         Search Results
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_SearchHome = new Guid(KF_ID.ID_FOLDERID_SearchHome);

        /// <summary>
        /// Display Name         Original Images
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows Photo Gallery\Original Images
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_OriginalImages = new Guid(KF_ID.ID_FOLDERID_OriginalImages);

        /// <summary>
        /// Display Name         The user's username (%USERNAME%)
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_HomeGroupCurrentUser = new Guid(KF_ID.ID_FOLDERID_HomeGroupCurrentUser);

        /// <summary>
        /// Display Name         Account Pictures
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\AccountPictures
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_AccountPictures = new Guid(KF_ID.ID_FOLDERID_AccountPictures);

        /// <summary>
        /// Display Name         Public Account Pictures
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\AccountPictures
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_PublicUserTiles = new Guid(KF_ID.ID_FOLDERID_PublicUserTiles);

        /// <summary>
        /// Display Name         Applications
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_AppsFolder = new Guid(KF_ID.ID_FOLDERID_AppsFolder);

        /// <summary>
        /// Display Name         Application Shortcuts
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\Application Shortcuts
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_ApplicationShortcuts = new Guid(KF_ID.ID_FOLDERID_ApplicationShortcuts);

        /// <summary>
        /// Display Name         RoamingTiles
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\RoamingTiles
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_RoamingTiles = new Guid(KF_ID.ID_FOLDERID_RoamingTiles);

        /// <summary>
        /// Display Name         RoamedTileImages
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\RoamedTileImages
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_RoamedTileImages = new Guid(KF_ID.ID_FOLDERID_RoamedTileImages);

        /// <summary>
        /// Display Name         Screenshots
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Pictures\Screenshots
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public static readonly Guid FOLDERID_Screenshots = new Guid(KF_ID.ID_FOLDERID_Screenshots);

        // ReSharper restore InconsistentNaming
    }
}