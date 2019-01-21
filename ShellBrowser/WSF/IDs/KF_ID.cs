namespace WSF.IDs
{
    using System.Collections.Generic;

    /// <summary>
    /// KnownFolders contain the known folder ids for windows.
    ///
    /// The KNOWNFOLDERID constants represent GUIDs that identify standard folders registered with the
    /// system as Known Folders. These folders are installed with Windows Vista and later operating
    /// systems, and a computer will have only folders appropriate to it installed. These values are
    /// defined in Knownfolders.h.
    /// see also IKnownFolderNative.GetId() method.
    ///
    /// </summary>
    /// <remarks>
    /// See:
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/dd378457.aspx
    /// For details on known folders.
    /// </remarks>
    public sealed class KF_ID
    {
        /// <summary>
        /// Defines the exact length in characters for all IDs defined below.
        /// </summary>
        public const int ID_Length = 38;

        /// <summary>
        /// Display Name         Network
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_NETWORK, CSIDL_COMPUTERSNEARME
        /// Legacy Display Name  My Network Places
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public const string ID_FOLDERID_NetworkFolder = "{D20BEEC4-5CA8-4905-AE3B-BF251EA09B53}";

        /// <summary>
        /// Display Name          Computer
        /// Folder Type           VIRTUAL
        /// Default Path          Not applicable—virtual folder
        /// CSIDL Equivalent      CSIDL_DRIVES
        /// Legacy Display Name   My Computer
        /// Legacy Default Path   Not applicable—virtual folder
        /// </summary>
        public const string ID_FOLDERID_ComputerFolder = "{0AC0837C-BBF8-452A-850D-79D08E667CA7}";
////        public const string ID_FOLDERID_ComputerFolder_CLSID = "{20d04fe0-3aea-1069-a2d8-08002b30309d}"; // CLSID
        
        /// <summary>
        /// Display Name         Internet Explorer
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_INTERNET
        /// Legacy Display Name  Internet Explorer
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public const string ID_FOLDERID_InternetFolder = "{4D9F7874-4E0C-4904-967B-40B0D20C3E4B}";

        /// <summary>
        /// Display Name         Control Panel
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_CONTROLS
        /// Legacy Display Name  Control Panel
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public const string ID_FOLDERID_ControlPanelFolder = "{82A74AEB-AEB4-465C-A014-D097EE346D63}";

        /// <summary>
        /// Display Name         Printers
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_PRINTERS
        /// Legacy Display Name  Printers and Faxes
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public const string ID_FOLDERID_PrintersFolder = "{76FC4E2D-D6AD-4519-A663-37BD56068185}";

        /// <summary>
        /// Display Name         Sync Center
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_SyncManagerFolder = "{43668BF8-C14E-49B2-97C9-747784D784B7}";

        /// <summary>
        /// Display Name         Sync Setup
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_SyncSetupFolder = "{0F214138-B1D3-4a90-BBA9-27CBC0C5389A}";

        /// <summary>
        /// Display Name          Conflicts
        /// Folder Type           VIRTUAL
        /// Default Path          Not applicable—virtual folder
        /// CSIDL Equivalent      None, new in Windows Vista
        /// Legacy Display Name   Not applicable. This KNOWNFOLDERID refers to the Windows Vista Synchronization Manager. It is not the folder referenced by the older ISyncMgrConflictFolder.
        /// Legacy Default Path   Not applicable
        /// </summary>
        public const string ID_FOLDERID_ConflictFolder = "{4bfefb45-347d-4006-a5be-ac0cb0567192}";

        /// <summary>
        /// Display Name         Control Panel
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_CONTROLS
        /// Legacy Display Name  Control Panel
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public const string ID_FOLDERID_SyncResultsFolder = "{289a9a43-be44-4057-a41b-587a76d7e7f9}";

        /// <summary>
        /// Display Name         Recycle Bin
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_BITBUCKET
        /// Legacy Display Name  Recycle Bin
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public const string ID_FOLDERID_RecycleBinFolder = "{B7534046-3ECB-4C18-BE4E-64CD4CB7D6AC}";

        /// <summary>
        /// Display Name         Network Connections
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     CSIDL_CONNECTIONS
        /// Legacy Display Name  Network Connections
        /// Legacy Default Path  Not applicable—virtual folder
        /// </summary>
        public const string ID_FOLDERID_ConnectionsFolder = "{6F0CD92B-2E97-45D1-88FF-B0D186B8DEDD}";

        /// <summary>
        /// Display Name         Fonts
        /// Folder Type          FIXED
        /// Default Path         %windir%\Fonts
        /// CSIDL Equivalent     CSIDL_FONTS
        /// Legacy Display Name  Fonts
        /// Legacy Default Path  %windir%\Fonts
        /// </summary>
        public const string ID_FOLDERID_Fonts = "{FD228CB7-AE11-4AE3-864C-16F3910AB8FE}";

        /// <summary>
        /// Display Name         Desktop
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Desktop
        /// CSIDL Equivalent     CSIDL_DESKTOP, CSIDL_DESKTOPDIRECTORY
        /// Legacy Display Name  Desktop
        /// Legacy Default Path  %USERPROFILE%\Desktop
        /// </summary>
        public const string ID_FOLDERID_Desktop = "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}";

        /// <summary>
        /// Display Name         Startup
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Start Menu\Programs\StartUp
        /// CSIDL Equivalent     CSIDL_STARTUP, CSIDL_ALTSTARTUP
        /// Legacy Display Name  Startup
        /// Legacy Default Path  %USERPROFILE%\Start Menu\Programs\StartUp
        /// </summary>
        public const string ID_FOLDERID_Startup = "{B97D20BB-F46A-4C97-BA10-5E3608430854}";

        /// <summary>
        /// Display Name         Programs
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Start Menu\Programs
        /// CSIDL Equivalent     CSIDL_PROGRAMS
        /// Legacy Display Name  Programs
        /// Legacy Default Path  %USERPROFILE%\Start Menu\Programs
        /// </summary>
        public const string ID_FOLDERID_Programs = "{A77F5D77-2E2B-44C3-A6A2-ABA601054A51}";

        /// <summary>
        /// Display Name         Start Menu
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Start Menu
        /// CSIDL Equivalent     CSIDL_STARTMENU
        /// Legacy Display Name  Start Menu
        /// Legacy Default Path  %USERPROFILE%\Start Menu
        /// </summary>
        public const string ID_FOLDERID_StartMenu = "{625B53C3-AB48-4EC1-BA1F-A1EF4146FC19}";

        /// <summary>
        /// Display Name         Recent Items
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Recent
        /// CSIDL Equivalent     CSIDL_RECENT
        /// Legacy Display Name  My Recent Documents
        /// Legacy Default Path  %USERPROFILE%\Recent
        /// </summary>
        public const string ID_FOLDERID_Recent = "{AE50C081-EBD2-438A-8655-8A092E34987A}";

        /// <summary>
        /// Display Name         SendTo
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\SendTo
        /// CSIDL Equivalent     CSIDL_SENDTO
        /// Legacy Display Name  SendTo
        /// Legacy Default Path  %USERPROFILE%\SendTo
        /// </summary>
        public const string ID_FOLDERID_SendTo = "{8983036C-27C0-404B-8F08-102D10DCFD74}";

        /// <summary>
        /// Display Name         Documents
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Documents
        /// CSIDL Equivalent     CSIDL_MYDOCUMENTS, CSIDL_PERSONAL
        /// Legacy Display Name  My Documents
        /// Legacy Default Path  %USERPROFILE%\My Documents
        /// </summary>
        public const string ID_FOLDERID_Documents = "{FDD39AD0-238F-46AF-ADB4-6C85480369C7}";

        /// <summary>
        /// Display Name         Favorites
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Favorites
        /// CSIDL Equivalent     CSIDL_FAVORITES, CSIDL_COMMON_FAVORITES
        /// Legacy Display Name  Favorites
        /// Legacy Default Path  %USERPROFILE%\Favorites
        /// </summary>
        public const string ID_FOLDERID_Favorites = "{1777F761-68AD-4D8A-87BD-30B759FA33DD}";

        /// <summary>
        /// Display Name         Network Shortcuts
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Network Shortcuts
        /// CSIDL Equivalent     CSIDL_NETHOOD
        /// Legacy Display Name  NetHood
        /// Legacy Default Path  %USERPROFILE%\NetHood
        /// </summary>
        public const string ID_FOLDERID_NetHood = "{C5ABBF53-E17F-4121-8900-86626FC2C973}";

        /// <summary>
        /// Display Name         Printer Shortcuts
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Printer Shortcuts
        /// CSIDL Equivalent     CSIDL_PRINTHOOD
        /// Legacy Display Name  PrintHood
        /// Legacy Default Path  %USERPROFILE%\PrintHood
        /// </summary>
        public const string ID_FOLDERID_PrintHood = "{9274BD8D-CFD1-41C3-B35E-B13F55A758F4}";

        /// <summary>
        /// Display Name         Templates
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Templates
        /// CSIDL Equivalent     CSIDL_TEMPLATES
        /// Legacy Display Name  Templates
        /// Legacy Default Path  %USERPROFILE%\Templates
        /// </summary>
        public const string ID_FOLDERID_Templates = "{A63293E8-664E-48DB-A079-DF759E0509F7}";

        /// <summary>
        /// Display Name          Startup
        /// Folder Type           COMMON
        /// Default Path          %ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs\StartUp
        /// CSIDL Equivalent      CSIDL_COMMON_STARTUP, CSIDL_COMMON_ALTSTARTUP
        /// Legacy Display Name   Startup
        /// Legacy Default Path   %ALLUSERSPROFILE%\Start Menu\Programs\StartUp
        /// </summary>
        public const string ID_FOLDERID_CommonStartup = "{82A5EA35-D9CD-47C5-9629-E15D2F714E6E}";

        /// <summary>
        /// Display Name          Programs
        /// Folder Type           COMMON
        /// Default Path          %ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs
        /// CSIDL Equivalent      CSIDL_COMMON_PROGRAMS
        /// Legacy Display Name   Programs
        /// Legacy Default Path   %ALLUSERSPROFILE%\Start Menu\Programs
        /// </summary>
        public const string ID_FOLDERID_CommonPrograms = "{0139D44E-6AFE-49F2-8690-3DAFCAE6FFB8}";

        /// <summary>
        /// Display Name          Start Menu
        /// Folder Type           COMMON
        /// Default Path          %ALLUSERSPROFILE%\Microsoft\Windows\Start Menu
        /// CSIDL Equivalent      CSIDL_COMMON_STARTMENU
        /// Legacy Display Name   Start Menu
        /// Legacy Default Path   %ALLUSERSPROFILE%\Start Menu
        /// </summary>
        public const string ID_FOLDERID_CommonStartMenu = "{A4115719-D62E-491D-AA7C-E74B8BE3B067}";

        /// <summary>
        /// Display Name         Public Desktop
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Desktop
        /// CSIDL Equivalent     CSIDL_COMMON_DESKTOPDIRECTORY
        /// Legacy Display Name  Desktop
        /// Legacy Default Path  %ALLUSERSPROFILE%\Desktop
        /// </summary>
        public const string ID_FOLDERID_PublicDesktop = "{C4AA340D-F20F-4863-AFEF-F87EF2E6BA25}";

        /// <summary>
        /// Display Name         ProgramData
        /// Folder Type          FIXED
        /// Default Path         %ALLUSERSPROFILE% (%ProgramData%, %SystemDrive%\ProgramData)
        /// CSIDL Equivalent     CSIDL_COMMON_APPDATA
        /// Legacy Display Name  Application Data
        /// Legacy Default Path  %ALLUSERSPROFILE%\Application Data
        /// </summary>
        public const string ID_FOLDERID_ProgramData = "{62AB5D82-FDC1-4DC3-A9DD-070D1D495D97}";

        /// <summary>
        /// Display Name          Templates
        /// Folder Type           COMMON
        /// Default Path          %ALLUSERSPROFILE%\Templates
        /// CSIDL Equivalent      CSIDL_COMMON_TEMPLATES
        /// Legacy Display Name   Templates
        /// Legacy Default Path   %ALLUSERSPROFILE%\Templates
        /// </summary>
        public const string ID_FOLDERID_CommonTemplates = "{B94237E7-57AC-4347-9151-B08C6C32D1F7}";

        /// <summary>
        /// Display Name         Public Documents
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Documents
        /// CSIDL Equivalent     CSIDL_COMMON_DOCUMENTS
        /// Legacy Display Name  Shared Documents
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents
        /// </summary>
        public const string ID_FOLDERID_PublicDocuments = "{ED4824AF-DCE4-45A8-81E2-FC7965083634}";

        /// <summary>
        /// Display Name         Roaming
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA% (%USERPROFILE%\AppData\Roaming)
        /// CSIDL Equivalent     CSIDL_APPDATA
        /// Legacy Display Name  Application Data
        /// Legacy Default Path  %APPDATA% (%USERPROFILE%\Application Data)
        /// </summary>
        public const string ID_FOLDERID_RoamingAppData = "{3EB685DB-65F9-4CF6-A03A-E3EF65729F3D}";

        /// <summary>
        /// Display Name         Local
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA% (%USERPROFILE%\AppData\Local)
        /// CSIDL Equivalent     CSIDL_LOCAL_APPDATA
        /// Legacy Display Name  Application Data
        /// Legacy Default Path  %USERPROFILE%\Local Settings\Application Data
        /// </summary>
        public const string ID_FOLDERID_LocalAppData = "{F1B32785-6FBA-4FCF-9D55-7B8E7F157091}";

        /// <summary>
        /// Display Name         LocalLow
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\AppData\LocalLow
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_LocalAppDataLow = "{A520A1A4-1780-4FF6-BD18-167343C5AF16}";

        /// <summary>
        /// Display Name         Temporary Internet Files
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\Temporary Internet Files
        /// CSIDL Equivalent     CSIDL_INTERNET_CACHE
        /// Legacy Display Name  Temporary Internet Files
        /// Legacy Default Path  %USERPROFILE%\Local Settings\Temporary Internet Files
        /// </summary>
        public const string ID_FOLDERID_InternetCache = "{352481E8-33BE-4251-BA85-6007CAEDCF9D}";

        /// <summary>
        /// Display Name         Cookies
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Cookies
        /// CSIDL Equivalent     CSIDL_COOKIES
        /// Legacy Display Name  Cookies
        /// Legacy Default Path  %USERPROFILE%\Cookies
        /// </summary>
        public const string ID_FOLDERID_Cookies = "{2B0F765D-C0E9-4171-908E-08A611B84FF6}";

        /// <summary>
        /// Display Name         History
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\History
        /// CSIDL Equivalent     CSIDL_HISTORY
        /// Legacy Display Name  History
        /// Legacy Default Path  %USERPROFILE%\Local Settings\History
        /// </summary>
        public const string ID_FOLDERID_History = "{D9DC8A3B-B784-432E-A781-5A1130A75963}";

        /// <summary>
        /// Display Name         System32
        /// Folder Type          FIXED
        /// Default Path         %windir%\system32
        /// CSIDL Equivalent     CSIDL_SYSTEM
        /// Legacy Display Name  system32
        /// Legacy Default Path  %windir%\system32
        /// </summary>
        public const string ID_FOLDERID_System = "{1AC14E77-02E7-4E5D-B744-2EB1AE5198B7}";

        /// <summary>
        /// Display Name         System32
        /// Folder Type          FIXED
        /// Default Path         %windir%\system32
        /// CSIDL Equivalent     CSIDL_SYSTEMX86
        /// Legacy Display Name  system32
        /// Legacy Default Path  %windir%\system32
        /// </summary>
        public const string ID_FOLDERID_SystemX86 = "{D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27}";

        /// <summary>
        /// Display Name         Windows
        /// Folder Type          FIXED
        /// Default Path         %windir%
        /// CSIDL Equivalent     CSIDL_WINDOWS
        /// Legacy Display Name  WINDOWS
        /// Legacy Default Path  %windir%
        /// </summary>
        public const string ID_FOLDERID_Windows = "{F38BF404-1D43-42F2-9305-67DE0B28FC23}";

        /// <summary>
        /// Display Name         The user's username (%USERNAME%)
        /// Folder Type          FIXED
        /// Default Path         %USERPROFILE% (%SystemDrive%\Users\%USERNAME%)
        /// CSIDL Equivalent     CSIDL_PROFILE
        /// Legacy Display Name  The user's username (%USERNAME%)
        /// Legacy Default Path  %USERPROFILE% (%SystemDrive%\Documents and Settings\%USERNAME%)
        /// </summary>
        public const string ID_FOLDERID_Profile = "{5E6C858F-0E22-4760-9AFE-EA3317B67173}";

        /// <summary>
        /// Display Name         Pictures
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Pictures
        /// CSIDL Equivalent     CSIDL_MYPICTURES
        /// Legacy Display Name  My Pictures
        /// Legacy Default Path  %USERPROFILE%\My Documents\My Pictures
        /// </summary>
        public const string ID_FOLDERID_Pictures = "{33E28130-4E1E-4676-835A-98395C3BC3BB}";

        /// <summary>
        /// Display Name         Program Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles% (%SystemDrive%\Program Files)
        /// CSIDL Equivalent     CSIDL_PROGRAM_FILESX86
        /// Legacy Display Name  Program Files
        /// Legacy Default Path  %ProgramFiles% (%SystemDrive%\Program Files)
        /// </summary>
        public const string ID_FOLDERID_ProgramFilesX86 = "{7C5A40EF-A0FB-4BFC-874A-C0F2E0B9FA8E}";

        /// <summary>
        /// Display Name         Common Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles%\Common Files
        /// CSIDL Equivalent     CSIDL_PROGRAM_FILES_COMMONX86
        /// Legacy Display Name  Common Files
        /// Legacy Default Path  %ProgramFiles%\Common Files
        /// </summary>
        public const string ID_FOLDERID_ProgramFilesCommonX86 = "{DE974D24-D9C6-4D3E-BF91-F4455120B917}";

        /// <summary>
        /// Display Name         Program Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles% (%SystemDrive%\Program Files)
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Program Files
        /// Legacy Default Path  %ProgramFiles% (%SystemDrive%\Program Files)
        /// </summary>
        public const string ID_FOLDERID_ProgramFilesX64 = "{6D809377-6AF0-444b-8957-A3773F02200E}";

        /// <summary>
        /// Display Name         Common Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles%\Common Files
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Common Files
        /// Legacy Default Path  %ProgramFiles%\Common Files
        /// </summary>
        public const string ID_FOLDERID_ProgramFilesCommonX64 = "{6365D5A7-0F0D-45e5-87F6-0DA56B6A4F7D}";

        /// <summary>
        /// Display Name         Program Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles% (%SystemDrive%\Program Files)
        /// CSIDL Equivalent     CSIDL_PROGRAM_FILES
        /// Legacy Display Name  Program Files
        /// Legacy Default Path  %ProgramFiles% (%SystemDrive%\Program Files)
        /// </summary>
        public const string ID_FOLDERID_ProgramFiles = "{905e63b6-c1bf-494e-b29c-65b732d3d21a}";

        /// <summary>
        /// Display Name         Common Files
        /// Folder Type          FIXED
        /// Default Path         %ProgramFiles%\Common Files
        /// CSIDL Equivalent     CSIDL_PROGRAM_FILES_COMMON
        /// Legacy Display Name  Common Files
        /// Legacy Default Path  %ProgramFiles%\Common Files
        /// </summary>
        public const string ID_FOLDERID_ProgramFilesCommon = "{F7F1ED05-9F6D-47A2-AAAE-29D317C6F066}";

        /// <summary>
        /// UsersLibraries
        /// </summary>
        public const string ID_FOLDERID_UsersLibraries = "{A302545D-DEFF-464b-ABE8-61C8648D939B}";
        #region Win7 KnownFolders

        /// <summary>
        /// UserProgramFiles
        /// </summary>
        public const string ID_FOLDERID_UserProgramFiles = "{5cd7aee2-2219-4a67-b85d-6c9ce15660cb}";
        
        /// <summary>
        /// UserProgramFilesCommon
        /// </summary>
        public const string ID_FOLDERID_UserProgramFilesCommon = "{bcbd3057-ca5c-4622-b42d-bc56db0ae516}";

        /// <summary>
        /// Ringtones
        /// </summary>
        public const string ID_FOLDERID_Ringtones = "{C870044B-F49E-4126-A9C3-B52A1FF411E8}";

        /// <summary>
        /// PublicRingtones
        /// </summary>
        public const string ID_FOLDERID_PublicRingtones = "{E555AB60-153B-4D17-9F04-A5FE99FC15EC}";

        /// <summary>
        /// Display Name         Documents
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\Libraries\Documents.library-ms
        /// CSIDL Equivalent     None, value introduced in Windows 7
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_DocumentsLibrary = "{7b0db17d-9cd2-4a93-9733-46cc89022e7c}";

        /// <summary>
        /// MusicLibrary
        /// </summary>
        public const string ID_FOLDERID_MusicLibrary = "{2112AB0A-C86A-4ffe-A368-0DE96E47012E}";

        /// <summary>
        /// PicturesLibrary
        /// </summary>
        public const string ID_FOLDERID_PicturesLibrary = "{A990AE9F-A03B-4e80-94BC-9912D7504104}";

        /// <summary>
        /// VideosLibrary
        /// </summary>
        public const string ID_FOLDERID_VideosLibrary = "{491E922F-5643-4af4-A7EB-4E7A138D8174}";

        /// <summary>
        /// RecordedTVLibrary
        /// </summary>
        public const string ID_FOLDERID_RecordedTVLibrary = "{1A6FDBA2-F42D-4358-A798-B74D745926C5}";
        
        /// <summary>
        /// OtherUsers
        /// </summary>
        public const string ID_FOLDERID_HomeGroup = "{52528A6B-B9E3-4add-B60D-588C2DBA842D}";
        
        /// <summary>
        /// DeviceMetadataStore
        /// </summary>        
        public const string ID_FOLDERID_DeviceMetadataStore = "{5CE4A5E9-E4EB-479D-B89F-130C02886155}";

        /// <summary>
        /// Libraries
        /// </summary>
        public const string ID_FOLDERID_Libraries = "{1B3EA5DC-B587-4786-B4EF-BD1DC332AEAE}";

        /// <summary>
        /// Display Name        Libraries
        /// Folder Type         COMMON
        /// Default Path        %ALLUSERSPROFILE%\Microsoft\Windows\Libraries
        /// CSIDL Equivalent    None, value introduced in Windows 7
        /// Legacy Display Name Not applicable
        /// Legacy Default Path Not applicable
        /// </summary>
        public const string ID_FOLDERID_PublicLibraries = "{48daf80b-e6cf-4f4e-b800-0e69d84ee384}";
        
        /// <summary>
        /// UserPinned
        /// </summary>
        public const string ID_FOLDERID_UserPinned = "{9e3995ab-1f9c-4f13-b827-48b24b6c7174}";

        /// <summary>
        /// ImplicitAppShortcuts
        /// </summary>
        public const string ID_FOLDERID_ImplicitAppShortcuts = "{bcb5256f-79f6-4cee-b725-dc34e402fd46}";
        #endregion Win7 KnownFolders

        /// <summary>
        /// Display Name        Administrative Tools
        /// Folder Type         PERUSER
        /// Default Path        %APPDATA%\Microsoft\Windows\Start Menu\Programs\Administrative Tools
        /// CSIDL Equivalent    CSIDL_ADMINTOOLS
        /// Legacy Display Name Administrative Tools
        /// Legacy Default Path %USERPROFILE%\Start Menu\Programs\Administrative Tools
        /// </summary>
        public const string ID_FOLDERID_AdminTools = "{724EF170-A42D-4FEF-9F26-B60E846FBA4F}";

        /// <summary>
        /// Display Name            Administrative Tools
        /// Folder Type             COMMON
        /// Default Path            %ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs\Administrative Tools
        /// CSIDL Equivalent        CSIDL_COMMON_ADMINTOOLS
        /// Legacy Display Name     Administrative Tools
        /// Legacy Default Path     %ALLUSERSPROFILE%\Start Menu\Programs\Administrative Tools
        /// </summary>
        public const string ID_FOLDERID_CommonAdminTools = "{D0384E7D-BAC3-4797-8F14-CBA229B392B5}";

        /// <summary>
        /// Display Name         Music
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Music
        /// CSIDL Equivalent     CSIDL_MYMUSIC
        /// Legacy Display Name  My Music
        /// Legacy Default Path  %USERPROFILE%\My Documents\My Music
        /// </summary>
        public const string ID_FOLDERID_Music = "{4BD8D571-6D19-48D3-BE97-422220080E43}";

        /// <summary>
        /// Display Name         Videos
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Videos
        /// CSIDL                CSIDL_MYVIDEO
        /// Legacy Display Name  My Videos
        /// Legacy Default Path  %USERPROFILE%\My Documents\My Videos
        /// </summary>
        public const string ID_FOLDERID_Videos = "{18989B1D-99B5-455B-841C-AB7C74E4DDFC}";

        /// <summary>
        /// Display Name         Public Pictures
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Pictures
        /// CSIDL Equivalent     CSIDL_COMMON_PICTURES
        /// Legacy Display Name  Shared Pictures
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Pictures
        /// </summary>
        public const string ID_FOLDERID_PublicPictures = "{B6EBFB86-6907-413C-9AF7-4FC2ABF07CC5}";

        /// <summary>
        /// Display Name         Public Music
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Music
        /// CSIDL Equivalent     CSIDL_COMMON_MUSIC
        /// Legacy Display Name  Shared Music
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Music
        /// </summary>
        public const string ID_FOLDERID_PublicMusic = "{3214FAB5-9757-4298-BB61-92A9DEAA44FF}";

        /// <summary>
        /// Display Name         Public Videos
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Videos
        /// CSIDL Equivalent     CSIDL_COMMON_VIDEO
        /// Legacy Display Name  Shared Video
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Videos
        /// </summary>
        public const string ID_FOLDERID_PublicVideos = "{2400183A-6185-49FB-A2D8-4A392A602BA3}";

        /// <summary>
        /// Display Name         Resources
        /// Folder Type          FIXED
        /// Default Path         %windir%\Resources
        /// CSIDL Equivalent     CSIDL_RESOURCES
        /// Legacy Display Name  Resources
        /// Legacy Default Path  %windir%\Resources
        /// </summary>
        public const string ID_FOLDERID_ResourceDir = "{8AD10C31-2ADB-4296-A8F7-E4701232C972}";

        /// <summary>
        /// Display Name         None
        /// Folder Type          FIXED
        /// Default Path         %windir%\resources\0409 (code page)
        /// CSIDL Equivalent     CSIDL_RESOURCES_LOCALIZED
        /// Legacy Display Name  None
        /// Legacy Default Path  %windir%\resources\0409 (code page)
        /// </summary>
        public const string ID_FOLDERID_LocalizedResourcesDir = "{2A00375E-224C-49DE-B8D1-440DF7EF3DDC}";

        /// <summary>
        /// Display Name        OEM Links
        /// Folder Type         COMMON
        /// Default Path        %ALLUSERSPROFILE%\OEM Links
        /// CSIDL Equivalent    CSIDL_COMMON_OEM_LINKS
        /// Legacy Display Name OEM Links
        /// Legacy Default Path %ALLUSERSPROFILE%\OEM Links
        /// </summary>
        public const string ID_FOLDERID_CommonOEMLinks = "{C1BAE2D0-10DF-4334-BEDD-7AA20B227A9D}";

        /// <summary>
        /// Display Name        Temporary Burn Folder
        /// Folder Type         PERUSER
        /// Default Path        %LOCALAPPDATA%\Microsoft\Windows\Burn\Burn
        /// CSIDL Equivalent    CSIDL_CDBURN_AREA
        /// Legacy Display Name CD Burning
        /// Legacy Default Path %USERPROFILE%\Local Settings\Application Data\Microsoft\CD Burning
        /// </summary>
        public const string ID_FOLDERID_CDBurning = "{9E52AB10-F80D-49DF-ACB8-4330F5687855}";

        /// <summary>
        /// Display Name         Users
        /// Folder Type          FIXED
        /// Default Path         %SystemDrive%\Users
        /// CSIDL Equivalent     None, new for Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_UserProfiles = "{0762D272-C50A-4BB0-A382-697DCD729B80}";

        /// <summary>
        /// Display Name         Playlists
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Music\Playlists
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_Playlists = "{DE92C1C7-837F-4F69-A3BB-86E631204A23}";

        /// <summary>
        /// Display Name         Sample Playlists
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Music\Sample Playlists
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_SamplePlaylists = "{15CA69B3-30EE-49C1-ACE1-6B5EC372AFB5}";

        /// <summary>
        /// Display Name         Sample Music
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Music\Sample Music
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Sample Music
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Music\Sample Music
        /// </summary>
        public const string ID_FOLDERID_SampleMusic = "{B250C668-F57D-4EE1-A63C-290EE7D1AA1F}";

        /// <summary>
        /// Display Name         Sample Pictures
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Pictures\Sample Pictures
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Sample Pictures
        /// Legacy Default Path  %ALLUSERSPROFILE%\Documents\My Pictures\Sample Pictures
        /// </summary>
        public const string ID_FOLDERID_SamplePictures = "{C4900540-2379-4C75-844B-64E6FAF8716B}";

        /// <summary>
        /// Display Name         Sample Videos
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Videos\Sample Videos
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_SampleVideos = "{859EAD94-2E85-48AD-A71A-0969CB56A6CD}";

        /// <summary>
        /// Display Name         Slide Shows
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Pictures\Slide Shows
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_PhotoAlbums = "{69D2CF90-FC33-4FB7-9A0C-EBB0F0FCB43C}";

        /// <summary>
        /// Display Name         Public
        /// Folder Type          FIXED
        /// Default Path         %PUBLIC% (%SystemDrive%\Users\Public)
        /// CSIDL Equivalent     None, new for Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_Public = "{DFDF76A2-C82A-4D63-906A-5644AC457385}";

        /// <summary>
        /// Display Name        Programs and Features
        /// Folder Type         VIRTUAL
        /// Default Path        Not applicable—virtual folder
        /// CSIDL Equivalent    None
        /// Legacy Display Name Add or Remove Programs
        /// Legacy Default Path Not applicable—virtual folder
        /// </summary>
        public const string ID_FOLDERID_ChangeRemovePrograms = "{df7266ac-9274-4867-8d55-3bd661de872d}";

        /// <summary>
        /// Display Name        Installed Updates
        /// Folder Type         VIRTUAL
        /// Default Path        Not applicable—virtual folder
        /// CSIDL Equivalent    None
        /// Legacy Display Name None, new in Windows Vista. In earlier versions of Microsoft Windows, the information on this page was included in Add or Remove Programs if the Show updates box was checked.
        /// Legacy Default Path Not applicable
        /// </summary>
        public const string ID_FOLDERID_AppUpdates = "{a305ce99-f527-492b-8b1a-7e76fa98d6e4}";

        /// <summary>
        /// Display Name        Get Programs
        /// Folder Type         VIRTUAL
        /// Default Path        Not applicable—virtual folder
        /// CSIDL Equivalent    None
        /// Legacy Display Name Add New Programs (found in the Add or Remove Programs item in the Control Panel)
        /// Legacy Default Path Not applicable—virtual folder
        /// </summary>
        public const string ID_FOLDERID_AddNewPrograms = "{de61d971-5ebc-4f02-a3a9-6c82895e5c04}";

        /// <summary>
        /// Display Name         Downloads
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Downloads
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_Downloads = "{374DE290-123F-4565-9164-39C4925E467B}";

        /// <summary>
        /// Display Name         Public Downloads
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\Downloads
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_PublicDownloads = "{3D644C9B-1FB8-4f30-9B45-F670235F79C0}";

        /// <summary>
        /// Display Name         Searches
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Searches
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_SavedSearches = "{7d1d3a04-debb-4115-95cf-2f29da2920da}";

        /// <summary>
        /// Display Name         Quick Launch
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Internet Explorer\Quick Launch
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Quick Launch
        /// Legacy Default Path  %APPDATA%\Microsoft\Internet Explorer\Quick Launch
        /// </summary>
        public const string ID_FOLDERID_QuickLaunch = "{52a4f021-7b75-48a9-9f6b-4b87a210bc8f}";

        /// <summary>
        /// Display Name         Contacts
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Contacts
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_Contacts = "{56784854-C6CB-462b-8169-88E350ACB882}";

        /// <summary>
        /// Display Name         Gadgets
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows Sidebar\Gadgets
        /// CSIDL Equivalent     None, new for Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_SidebarParts = "{A75D362E-50FC-4fb7-AC2C-A8BEAA314493}";

        /// <summary>
        /// Display Name         Gadgets
        /// Folder Type          COMMON
        /// Default Path         %ProgramFiles%\Windows Sidebar\Gadgets
        /// CSIDL Equivalent     None, new for Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_SidebarDefaultParts = "{7B396E54-9EC5-4300-BE0A-2482EBAE1A26}";

        /// <summary>
        /// Display Name         GameExplorer
        /// Folder Type          COMMON
        /// Default Path         %ALLUSERSPROFILE%\Microsoft\Windows\GameExplorer
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_PublicGameTasks = "{DEBF2536-E1A8-4c59-B6A2-414586476AEA}";

        /// <summary>
        /// Display Name         GameExplorer
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\GameExplorer
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_GameTasks = "{054FAE61-4DD8-4787-80B6-090220C4B700}";

        /// <summary>
        /// Display Name         Saved Games
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Saved Games
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_SavedGames = "{4C5C32FF-BB9D-43b0-B5B4-2D72E54EAAA4}";

        /// <summary>
        /// Display Name         Saved Games
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Saved Games
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_Games = "{CAC52C1A-B53D-4edc-92D7-6B2E8AC19434}";

        /// <summary>
        /// Display Name         Microsoft Office Outlook
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_SEARCH_MAPI = "{98ec0e18-2098-4d44-8644-66979315a281}";

        /// <summary>
        /// Display Name         Offline Files
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_SEARCH_CSC = "{ee32e446-31ca-4aba-814f-a5ebd2fd6d5e}";

        /// <summary>
        /// Display Name         Links
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Links
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_Links = "{bfb9d5e0-c6a9-404c-b2b2-ae6db6af4968}";

        /// <summary>
        /// Display Name         The user's full name (for instance, Jean Philippe Bagel) entered when the user account was created.
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_UsersFiles = "{f3ce0f7c-4901-4acc-8648-d5d44b04ef8f}";

        /// <summary>
        /// Display Name         Search Results
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_SearchHome = "{190337d1-b8ca-4121-a639-6d472d16972a}";

        /// <summary>
        /// Display Name         Original Images
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows Photo Gallery\Original Images
        /// CSIDL Equivalent     None, new in Windows Vista
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_OriginalImages = "{2C36C0AA-5812-4b87-BFD0-4CD0DFB19B39}";

        /// <summary>
        /// Display Name         The user's username (%USERNAME%)
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_HomeGroupCurrentUser = "{9B74B6A3-0DFD-4f11-9E78-5F7800F2E772}";

        /// <summary>
        /// Display Name         Account Pictures
        /// Folder Type          PERUSER
        /// Default Path         %APPDATA%\Microsoft\Windows\AccountPictures
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_AccountPictures = "{008ca0b1-55b4-4c56-b8a8-4de4b299d3be}";

        /// <summary>
        /// Display Name         Public Account Pictures
        /// Folder Type          COMMON
        /// Default Path         %PUBLIC%\AccountPictures
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_PublicUserTiles = "{0482af6c-08f1-4c34-8c90-e17ec98b1e17}";

        /// <summary>
        /// Display Name         Applications
        /// Folder Type          VIRTUAL
        /// Default Path         Not applicable—virtual folder
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_AppsFolder = "{1e87508d-89c2-42f0-8a7e-645a0f50ca58}";

        /// <summary>
        /// Display Name         Application Shortcuts
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\Application Shortcuts
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_ApplicationShortcuts = "{A3918781-E5F2-4890-B3D9-A7E54332328C}";

        /// <summary>
        /// Display Name         RoamingTiles
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\RoamingTiles
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_RoamingTiles = "{00BCFC5A-ED94-4e48-96A1-3F6217F21990}";

        /// <summary>
        /// Display Name         RoamedTileImages
        /// Folder Type          PERUSER
        /// Default Path         %LOCALAPPDATA%\Microsoft\Windows\RoamedTileImages
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_RoamedTileImages = "{AAA8D5A5-F1D6-4259-BAA8-78E7EF60835E}";

        /// <summary>
        /// Display Name         Screenshots
        /// Folder Type          PERUSER
        /// Default Path         %USERPROFILE%\Pictures\Screenshots
        /// CSIDL Equivalent     None, value introduced in Windows 8
        /// Legacy Display Name  Not applicable
        /// Legacy Default Path  Not applicable
        /// </summary>
        public const string ID_FOLDERID_Screenshots = "{b7bede81-df94-4682-a7d8-57a52620b86f}";
        
        /// <summary>
        /// Returns a dictionary of all constants and their Ids in this class.
        /// </summary>
        public Dictionary<string,string> GetIdKnownFolders(string prefixId = null)
        {
          if (prefixId == null)      // Ensure that we do not add null + string below
            prefixId = string.Empty;
          
          var lstOfConstants = new Dictionary<string,string>();
          
          foreach (var constant in typeof(KF_ID).GetFields())
          {
              if (constant.IsLiteral && !constant.IsInitOnly)
              {
                  lstOfConstants.Add(prefixId + constant.Name, (string)constant.GetValue(null));
              }
          }
          
          return lstOfConstants;
        }
    }
}