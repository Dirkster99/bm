using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellDll
{
    //http://msdn.microsoft.com/en-us/library/windows/desktop/dd378457%28v=vs.85%29.aspx
    public enum KnownFolderIds
    {        
        [KnownFolderGuid("{008ca0b1-55b4-4c56-b8a8-4de4b299d3be}")]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\AccountPictures")]
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DisplayName("Account Pictures")]
        [MinVersion(OSVersions.Windows8)]        
        AccountPictures,

        [KnownFolderGuid("{de61d971-5ebc-4f02-a3a9-6c82895e5c04}")]
        [FolderCategory(KnownFolderCategory.Virtual)]
        [DisplayName("Get Programs")]
        AddNewPrograms,

        [KnownFolderGuid("{724EF170-A42D-4FEF-9F26-B60E846FBA4F}")]
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Start Menu\Programs\Administrative Tools")]
        [DisplayName("Administrative Tools")]
        [Csidl(ShellAPI.CSIDL.CSIDL_ADMINTOOLS)]
        [SpecialFolder(Environment.SpecialFolder.AdminTools)]
        AdminTools,

        [KnownFolderGuid("{A3918781-E5F2-4890-B3D9-A7E54332328C}")]
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Microsoft\Windows\Application Shortcuts")]
        [DisplayName("Application Shortcuts")]
        [MinVersion(OSVersions.Windows8)]
        ApplicationShortcuts,


        [KnownFolderGuid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}")]
        [FolderCategory(KnownFolderCategory.Virtual)]
        [DisplayName("Applications")]
        [MinVersion(OSVersions.Windows8)]
        AppsFolder,

        [KnownFolderGuid("{a305ce99-f527-492b-8b1a-7e76fa98d6e4}")]
        [FolderCategory(KnownFolderCategory.Virtual)]
        [DisplayName("Installed Updates")]
        AppUpdates,

        [KnownFolderGuid("{AB5FB87B-7CE2-4F83-915D-550846C9537B}")]
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Pictures\Camera Roll")]
        [DisplayName("Camera Roll")]
        [MinVersion(OSVersions.Windows81)]
        CameraRoll,

        [KnownFolderGuid("{9E52AB10-F80D-49DF-ACB8-4330F5687855}")]
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Microsoft\Windows\Burn\Burn")]
        [DisplayName("Temporary Burn Folder")]
        [Csidl(ShellAPI.CSIDL.CSIDL_CDBURN_AREA)]
        [SpecialFolder(Environment.SpecialFolder.CDBurning)]
        CDBurning,

        [KnownFolderGuid("{df7266ac-9274-4867-8d55-3bd661de872d}")]
        [FolderCategory(KnownFolderCategory.Virtual)]
        [DisplayName("Programs and Features")]
        ChangeRemovePrograms,

        [KnownFolderGuid("{D0384E7D-BAC3-4797-8F14-CBA229B392B5}")]
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs\Administrative Tools")]
        [DisplayName("Administrative Tools")]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_ADMINTOOLS)]
        [SpecialFolder(Environment.SpecialFolder.CommonAdminTools)]
        CommonAdminTools,

        [KnownFolderGuid("{C1BAE2D0-10DF-4334-BEDD-7AA20B227A9D}")]
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%ALLUSERSPROFILE%\OEM Links")]
        [DisplayName("OEM Links")]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_OEM_LINKS)]
        [SpecialFolder(Environment.SpecialFolder.CommonOemLinks)]
        CommonOEMLinks,

        [KnownFolderGuid("{0139D44E-6AFE-49F2-8690-3DAFCAE6FFB8}")]
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs")]
        [DisplayName("Programs")]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_PROGRAMS)]
        [SpecialFolder(Environment.SpecialFolder.CommonPrograms)]
        CommonPrograms,


        [KnownFolderGuid("{A4115719-D62E-491D-AA7C-E74B8BE3B067}")]
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%ALLUSERSPROFILE%\Microsoft\Windows\Start Menu")]
        [DisplayName("Start Menu")]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_STARTMENU)]
        [SpecialFolder(Environment.SpecialFolder.CommonStartMenu)]
        CommonStartMenu,

        [KnownFolderGuid("{82A5EA35-D9CD-47C5-9629-E15D2F714E6E}")]
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs\StartUp")]
        [DisplayName("Startup")]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_STARTUP)]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_ALTSTARTUP)]
        [SpecialFolder(Environment.SpecialFolder.CommonStartup)]        
        CommonStartup,

        [KnownFolderGuid("{B94237E7-57AC-4347-9151-B08C6C32D1F7}")]
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%ALLUSERSPROFILE%\Microsoft\Windows\Templates")]
        [DisplayName("Templates")]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_TEMPLATES)]
        [SpecialFolder(Environment.SpecialFolder.CommonTemplates)]
        CommonTemplates,

        [KnownFolderGuid("{0AC0837C-BBF8-452A-850D-79D08E667CA7}")]
        [FolderCategory(KnownFolderCategory.Virtual)]
        [DisplayName("Computer")]
        [Csidl(ShellAPI.CSIDL.CSIDL_DRIVES)]
        [SpecialFolder(Environment.SpecialFolder.MyComputer)]
        ComputerFolder,

        [KnownFolderGuid("{4bfefb45-347d-4006-a5be-ac0cb0567192}")]
        [FolderCategory(KnownFolderCategory.Virtual)]
        [DisplayName("Conflicts")]
        [MinVersion(OSVersions.WindowsVista)]
        ConflictFolder,

        [KnownFolderGuid("{6F0CD92B-2E97-45D1-88FF-B0D186B8DEDD}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("Network Connections")]
        [Csidl(ShellAPI.CSIDL.CSIDL_CONNECTIONS)]                
        ConnectionsFolder,

        [KnownFolderGuid("{56784854-C6CB-462b-8169-88E350ACB882}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Contacts")]
        [DisplayName("Contacts")]        
        [MinVersion(OSVersions.WindowsVista)]
        Contacts,

        [KnownFolderGuid("{82A74AEB-AEB4-465C-A014-D097EE346D63}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("Control Panel")]
        [Csidl(ShellAPI.CSIDL.CSIDL_CONTROLS)]                
        ControlPanelFolder,

        [KnownFolderGuid("{2B0F765D-C0E9-4171-908E-08A611B84FF6}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Cookies")]
        [DisplayName("Cookies")]
        [Csidl(ShellAPI.CSIDL.CSIDL_COOKIES)]
        [SpecialFolder(Environment.SpecialFolder.Cookies)]        
        Cookies,

        [KnownFolderGuid("{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Desktop")]
        [DisplayName("Desktop")]
        [Csidl(ShellAPI.CSIDL.CSIDL_DESKTOP)]
        [Csidl(ShellAPI.CSIDL.CSIDL_DESKTOPDIRECTORY)]
        [SpecialFolder(Environment.SpecialFolder.Desktop)]
        [SpecialFolder(Environment.SpecialFolder.DesktopDirectory)]        
        Desktop,

        [KnownFolderGuid("{5CE4A5E9-E4EB-479D-B89F-130C02886155}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"")]
        [DisplayName("DeviceMetadataStore")]
        [MinVersion(OSVersions.Windows7)]
        DeviceMetadataStore,

        [KnownFolderGuid("{FDD39AD0-238F-46AF-ADB4-6C85480369C7}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\My Documents")]
        [DisplayName("Documents")]
        [Csidl(ShellAPI.CSIDL.CSIDL_MYDOCUMENTS)]
        [Csidl(ShellAPI.CSIDL.CSIDL_PERSONAL)]
        [SpecialFolder(Environment.SpecialFolder.MyDocuments)]
        [SpecialFolder(Environment.SpecialFolder.Personal)]        
        Documents,

        [KnownFolderGuid("{7B0DB17D-9CD2-4A93-9733-46CC89022E7C}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Libraries\Documents.library-ms")]
        [DisplayName("Documents")]                
        [MinVersion(OSVersions.Windows7)]
        DocumentsLibrary,

        [KnownFolderGuid("{374DE290-123F-4565-9164-39C4925E467B}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Downloads")]
        [DisplayName("Downloads")]        
        Downloads,

        [KnownFolderGuid("{1777F761-68AD-4D8A-87BD-30B759FA33DD}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Favorites")]
        [DisplayName("Favorites")]
        [Csidl(ShellAPI.CSIDL.CSIDL_FAVORITES)]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_FAVORITES)]
        [SpecialFolder(Environment.SpecialFolder.Favorites)]        
        Favorites,

        [KnownFolderGuid("{FD228CB7-AE11-4AE3-864C-16F3910AB8FE}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%windir%\Fonts")]
        [DisplayName("Fonts")]
        [Csidl(ShellAPI.CSIDL.CSIDL_FONTS)]
        [SpecialFolder(Environment.SpecialFolder.Fonts)]        
        Fonts,

        [KnownFolderGuid("{CAC52C1A-B53D-4edc-92D7-6B2E8AC19434}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("Games")]
        Games,

        [KnownFolderGuid("{054FAE61-4DD8-4787-80B6-090220C4B700}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Microsoft\Windows\GameExplorer")]
        [DisplayName("GameExplorer")]
        [MinVersion(OSVersions.WindowsVista)]
        GameTasks,

        [KnownFolderGuid("{D9DC8A3B-B784-432E-A781-5A1130A75963}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Microsoft\Windows\History")]
        [DisplayName("History")]
        [Csidl(ShellAPI.CSIDL.CSIDL_HISTORY)]
        [SpecialFolder(Environment.SpecialFolder.History)]        
        History,

        [KnownFolderGuid("{52528A6B-B9E3-4ADD-B60D-588C2DBA842D}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]
        [DefaultPath(@"Homegroup")]
        [MinVersion(OSVersions.Windows7)]
        HomeGroup,

        [KnownFolderGuid("{9B74B6A3-0DFD-4f11-9E78-5F7800F2E772}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("%USERNAME%")]
        [MinVersion(OSVersions.Windows8)]
        HomeGroupCurrentUser,

        [KnownFolderGuid("{BCB5256F-79F6-4CEE-B725-DC34E402FD46}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]        
        [DisplayName("ImplicitAppShortcuts")]
        [MinVersion(OSVersions.Windows7)]
        ImplicitAppShortcuts,

        [KnownFolderGuid("{352481E8-33BE-4251-BA85-6007CAEDCF9D}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Microsoft\Windows\Temporary Internet Files")]
        [DefaultPath(@"%USERPROFILE%\Local Settings\Temporary Internet Files")]
        [DisplayName("Temporary Internet Files")]
        [Csidl(ShellAPI.CSIDL.CSIDL_INTERNET_CACHE)]
        [SpecialFolder(Environment.SpecialFolder.InternetCache)]        
        InternetCache,

        [KnownFolderGuid("{4D9F7874-4E0C-4904-967B-40B0D20C3E4B}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("The Internet")]
        [Csidl(ShellAPI.CSIDL.CSIDL_INTERNET)]                
        InternetFolder,

        [KnownFolderGuid("{1B3EA5DC-B587-4786-B4EF-BD1DC332AEAE}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Libraries")]
        [DisplayName("Libraries")]
        [MinVersion(OSVersions.Windows7)]
        Libraries,

        [KnownFolderGuid("{bfb9d5e0-c6a9-404c-b2b2-ae6db6af4968}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Links")]
        [DisplayName("Links")]
        Links,

        [KnownFolderGuid("{F1B32785-6FBA-4FCF-9D55-7B8E7F157091}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%")]
        [DefaultPath(@"%USERPROFILE%\AppData\Local")]
        [DisplayName("Local")]
        [Csidl(ShellAPI.CSIDL.CSIDL_LOCAL_APPDATA)]
        [SpecialFolder(Environment.SpecialFolder.LocalApplicationData)]        
        LocalAppData,

        [KnownFolderGuid("{A520A1A4-1780-4FF6-BD18-167343C5AF16}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\AppData\LocalLow")]
        [DisplayName("LocalLow")]
        LocalAppDataLow,

        
        [KnownFolderGuid("{2A00375E-224C-49DE-B8D1-440DF7EF3DDC}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%windir%\resources\0409")]        
        [Csidl(ShellAPI.CSIDL.CSIDL_RESOURCES_LOCALIZED)]
        [SpecialFolder(Environment.SpecialFolder.LocalizedResources)]
        LocalizedResourcesDir,

        [KnownFolderGuid("{4BD8D571-6D19-48D3-BE97-422220080E43}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Music")]
        [DefaultPath(@"%USERPROFILE%\My Documents\My Music")]
        [DisplayName("Music")]
        [Csidl(ShellAPI.CSIDL.CSIDL_MYMUSIC)]
        [SpecialFolder(Environment.SpecialFolder.MyMusic)]        
        Music,

        [KnownFolderGuid("{2112AB0A-C86A-4FFE-A368-0DE96E47012E}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Libraries\Music.library-ms")]
        [DisplayName("Music")]
        [MinVersion(OSVersions.Windows7)]
        MusicLibrary,

        [KnownFolderGuid("{C5ABBF53-E17F-4121-8900-86626FC2C973}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Network Shortcuts")]
        [DisplayName("Network Shortcuts")]
        [Csidl(ShellAPI.CSIDL.CSIDL_NETHOOD)]
        [SpecialFolder(Environment.SpecialFolder.NetworkShortcuts)]        
        NetHood,

        [KnownFolderGuid("{D20BEEC4-5CA8-4905-AE3B-BF251EA09B53}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("Network")]
        [Csidl(ShellAPI.CSIDL.CSIDL_NETWORK)]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMPUTERSNEARME)]                
        NetworkFolder,

        [KnownFolderGuid("{2C36C0AA-5812-4b87-BFD0-4CD0DFB19B39}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Microsoft\Windows Photo Gallery\Original Images")]
        [DisplayName("Original Images")]
        [MinVersion(OSVersions.WindowsVista)]
        OriginalImages,

        [KnownFolderGuid("{69D2CF90-FC33-4FB7-9A0C-EBB0F0FCB43C}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Pictures\Slide Shows")]
        [DisplayName("Slide Shows")]
        [MinVersion(OSVersions.WindowsVista)]
        PhotoAlbums,

        [KnownFolderGuid("{A990AE9F-A03B-4E80-94BC-9912D7504104}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Libraries\Pictures.library-ms")]
        [DisplayName("Pictures")]
        [MinVersion(OSVersions.Windows7)]
        PicturesLibrary,

        [KnownFolderGuid("{33E28130-4E1E-4676-835A-98395C3BC3BB}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Pictures")]
        [DefaultPath(@"%USERPROFILE%\My Documents\My Pictures")]
        [DisplayName("Pictures")]
        [Csidl(ShellAPI.CSIDL.CSIDL_MYPICTURES)]
        [SpecialFolder(Environment.SpecialFolder.MyPictures)]        
        Pictures,

        [KnownFolderGuid("{DE92C1C7-837F-4F69-A3BB-86E631204A23}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Music\Playlists")]
        [DisplayName("Playlists")]        
        Playlists,

        [KnownFolderGuid("{76FC4E2D-D6AD-4519-A663-37BD56068185}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("Printers")]
        [Csidl(ShellAPI.CSIDL.CSIDL_PRINTERS)]        
        PrintersFolder,

        [KnownFolderGuid("{9274BD8D-CFD1-41C3-B35E-B13F55A758F4}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Printer Shortcuts")]
        [DefaultPath(@"%USERPROFILE%\PrintHood")]
        [DisplayName("Printer Shortcuts")]
        [Csidl(ShellAPI.CSIDL.CSIDL_PRINTHOOD)]
        [SpecialFolder(Environment.SpecialFolder.PrinterShortcuts)]        
        [MinVersion(OSVersions.Windows81)]
        PrintHood,

        [KnownFolderGuid("{5E6C858F-0E22-4760-9AFE-EA3317B67173}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%USERPROFILE%")]
        [DefaultPath(@"%SystemDrive%\Users\%USERNAME%")]
        [DefaultPath(@"%SystemDrive%\Documents")]
        [DisplayName("%USERNAME%")]
        [Csidl(ShellAPI.CSIDL.CSIDL_PROFILE)]
        [SpecialFolder(Environment.SpecialFolder.UserProfile)]        
        Profile,

        [KnownFolderGuid("{62AB5D82-FDC1-4DC3-A9DD-070D1D495D97}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%ALLUSERSPROFILE%")]
        [DefaultPath(@"%ProgramData%")]
        [DefaultPath(@"%SystemDrive%\ProgramData")]            
        [DefaultPath(@"%ALLUSERSPROFILE%\Application Data")]   
        [DisplayName("ProgramData")]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_APPDATA)]
        [SpecialFolder(Environment.SpecialFolder.CommonApplicationData)]        
        ProgramData,

        [KnownFolderGuid("{905e63b6-c1bf-494e-b29c-65b732d3d21a}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%ProgramFiles%")]
        [DefaultPath(@"%SystemDrive%\Program Files")]
        [DisplayName("Program Files")]
        [Csidl(ShellAPI.CSIDL.CSIDL_PROGRAM_FILES)]
        [SpecialFolder(Environment.SpecialFolder.ProgramFiles)]        
        ProgramFiles,

        [KnownFolderGuid("{6D809377-6AF0-444b-8957-A3773F02200E}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%ProgramFiles%")]
        [DefaultPath(@"%SystemDrive%\Program Files")]
        [DisplayName("Program Files")]                
        ProgramFilesX64,

        [KnownFolderGuid("{7C5A40EF-A0FB-4BFC-874A-C0F2E0B9FA8E}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%ProgramFiles%")]
        [DefaultPath(@"%SystemDrive%\Program Files")]
        [DisplayName("Program Files")]       
        [Csidl(ShellAPI.CSIDL.CSIDL_PROGRAM_FILESX86)]
        [SpecialFolder(Environment.SpecialFolder.ProgramFilesX86)] 
        ProgramFilesX86,


        [KnownFolderGuid("{F7F1ED05-9F6D-47A2-AAAE-29D317C6F066}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%ProgramFiles%\Common Files")]
        [DisplayName("Common Files")]
        [Csidl(ShellAPI.CSIDL.CSIDL_PROGRAM_FILES_COMMON)]
        [SpecialFolder(Environment.SpecialFolder.CommonProgramFiles)]        
        ProgramFilesCommon,

        [KnownFolderGuid("{6365D5A7-0F0D-45E5-87F6-0DA56B6A4F7D}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%ProgramFiles%\Common Files")]
        [DisplayName("Common Files")]        
        ProgramFilesCommonX64,

        [KnownFolderGuid("{DE974D24-D9C6-4D3E-BF91-F4455120B917}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%ProgramFiles%\Common Files")]
        [DisplayName("Common Files")]
        [Csidl(ShellAPI.CSIDL.CSIDL_PROGRAM_FILES_COMMONX86)]
        [SpecialFolder(Environment.SpecialFolder.CommonProgramFilesX86)]
        ProgramFilesCommonX86,

        [KnownFolderGuid("{A77F5D77-2E2B-44C3-A6A2-ABA601054A51}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Start Menu\Programs")]
        [DisplayName("Programs")]
        [Csidl(ShellAPI.CSIDL.CSIDL_PROGRAMS)]
        [SpecialFolder(Environment.SpecialFolder.Programs)]        
        Programs,

        [KnownFolderGuid("{DFDF76A2-C82A-4D63-906A-5644AC457385}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%PUBLIC%")]
        [DefaultPath(@"%SystemDrive%\Users\Public")]        
        [DisplayName("Public")]
        [MinVersion(OSVersions.WindowsVista)]
        Public,

        [KnownFolderGuid("{C4AA340D-F20F-4863-AFEF-F87EF2E6BA25}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%PUBLIC%\Desktop")]
        [DefaultPath(@"%ALLUSERSPROFILE%\Desktop")]
        [DisplayName("Public Desktop")]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_DESKTOPDIRECTORY)]
        [SpecialFolder(Environment.SpecialFolder.CommonDesktopDirectory)]        
        PublicDesktop,

        [KnownFolderGuid("{ED4824AF-DCE4-45A8-81E2-FC7965083634}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%PUBLIC%\Documents")]
        [DefaultPath(@"%ALLUSERSPROFILE%\Documents")]
        [DisplayName("Public Documents")]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_DOCUMENTS)]
        [SpecialFolder(Environment.SpecialFolder.CommonDocuments)]        
        PublicDocuments,

        [KnownFolderGuid("{3D644C9B-1FB8-4f30-9B45-F670235F79C0}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%PUBLIC%\Downloads")]
        [DisplayName("Public Downloads")]
        [MinVersion(OSVersions.WindowsVista)]
        PublicDownloads,

        [KnownFolderGuid("{DEBF2536-E1A8-4c59-B6A2-414586476AEA}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%ALLUSERSPROFILE%\Microsoft\Windows\GameExplorer")]
        [DisplayName("GameExplorer")]
        [MinVersion(OSVersions.WindowsVista)]
        PublicGameTasks,

        [KnownFolderGuid("{48DAF80B-E6CF-4F4E-B800-0E69D84EE384}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%ALLUSERSPROFILE%\Microsoft\Windows\Libraries")]
        [DisplayName("Libraries")]
        [MinVersion(OSVersions.Windows7)]
        PublicLibraries,

        [KnownFolderGuid("{3214FAB5-9757-4298-BB61-92A9DEAA44FF}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%PUBLIC%\Music")]
        [DefaultPath(@"%ALLUSERSPROFILE%\Documents\My Music")]
        [DisplayName("Public Music")]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_MUSIC)]
        [SpecialFolder(Environment.SpecialFolder.CommonMusic)]        
        PublicMusic,

        [KnownFolderGuid("{B6EBFB86-6907-413C-9AF7-4FC2ABF07CC5}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%PUBLIC%\Pictures")]
        [DefaultPath(@"%ALLUSERSPROFILE%\Documents\My Pictures")]
        [DisplayName("Public Pictures")]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_PICTURES)]
        [SpecialFolder(Environment.SpecialFolder.CommonPictures)]        
        PublicPictures,

        [KnownFolderGuid("{E555AB60-153B-4D17-9F04-A5FE99FC15EC}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%ALLUSERSPROFILE%\Microsoft\Windows\Ringtones")]
        [DisplayName("Ringtones")]
        [MinVersion(OSVersions.Windows7)]
        PublicRingtones,

        [KnownFolderGuid("{0482af6c-08f1-4c34-8c90-e17ec98b1e17}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%PUBLIC%\AccountPictures")]
        [DisplayName("Public Account Pictures")]
        [MinVersion(OSVersions.Windows8)]
        PublicUserTiles,

        [KnownFolderGuid("{2400183A-6185-49FB-A2D8-4A392A602BA3}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%PUBLIC%\Videos")]
        [DefaultPath(@"%ALLUSERSPROFILE%\Documents\My Videos")]
        [DisplayName("Public Videos")]
        [Csidl(ShellAPI.CSIDL.CSIDL_COMMON_VIDEO)]
        [SpecialFolder(Environment.SpecialFolder.CommonVideos)]        
        PublicVideos,

        [KnownFolderGuid("{52a4f021-7b75-48a9-9f6b-4b87a210bc8f}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Internet Explorer\Quick Launch")]
        [DisplayName("Quick Launch")]
        QuickLaunch,

        [KnownFolderGuid("{AE50C081-EBD2-438A-8655-8A092E34987A}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Recent")]
        [DisplayName("Recent Items")]
        [Csidl(ShellAPI.CSIDL.CSIDL_RECENT)]
        [SpecialFolder(Environment.SpecialFolder.Recent)]        
        Recent,


        //[MinVersion(OSVersions.Windows7)]
        //RecordedTV,

        [KnownFolderGuid("{1A6FDBA2-F42D-4358-A798-B74D745926C5}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%PUBLIC%\RecordedTV.library-ms")]
        [DisplayName("Recorded TV")]
        [MinVersion(OSVersions.Windows7)]
        RecordedTVLibrary,

        [KnownFolderGuid("{B7534046-3ECB-4C18-BE4E-64CD4CB7D6AC}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]
        [DisplayName("Recycle Bin")]
        [Csidl(ShellAPI.CSIDL.CSIDL_BITBUCKET)]                
        RecycleBinFolder,

        [KnownFolderGuid("{8AD10C31-2ADB-4296-A8F7-E4701232C972}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%windir%\Resources")]        
        [DisplayName("Resources")]
        [Csidl(ShellAPI.CSIDL.CSIDL_RESOURCES)]
        [SpecialFolder(Environment.SpecialFolder.Resources)]        
        ResourceDir,

        [KnownFolderGuid("{C870044B-F49E-4126-A9C3-B52A1FF411E8}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Microsoft\Windows\Ringtones")]
        [DisplayName("Ringtones")]
        [MinVersion(OSVersions.Windows7)]
        Ringtones,

        [KnownFolderGuid("{3EB685DB-65F9-4CF6-A03A-E3EF65729F3D}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%")]
        [DefaultPath(@"%USERPROFILE%\AppData\Roaming")]
        [DefaultPath(@"%USERPROFILE%\Application Data")]
        [DisplayName("Roaming")]
        [Csidl(ShellAPI.CSIDL.CSIDL_APPDATA)]
        [SpecialFolder(Environment.SpecialFolder.ApplicationData)]        
        RoamingAppData,

        [KnownFolderGuid("{AAA8D5A5-F1D6-4259-BAA8-78E7EF60835E}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Microsoft\Windows\RoamedTileImages")]
        [DisplayName("RoamedTileImages")]
        [MinVersion(OSVersions.Windows8)]
        RoamedTileImages,

        [KnownFolderGuid("{00BCFC5A-ED94-4e48-96A1-3F6217F21990}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Microsoft\Windows\RoamingTiles")]
        [DisplayName("RoamingTiles")]
        [MinVersion(OSVersions.Windows8)]
        RoamingTiles,

        [KnownFolderGuid("{B250C668-F57D-4EE1-A63C-290EE7D1AA1F}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%PUBLIC%\Music\Sample Music")]
        [DisplayName("Sample Music")]
        SampleMusic,

        [KnownFolderGuid("{C4900540-2379-4C75-844B-64E6FAF8716B}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%PUBLIC%\Pictures\Sample Pictures")]
        [DisplayName("Sample Pictures")]
        SamplePictures,

        [KnownFolderGuid("{15CA69B3-30EE-49C1-ACE1-6B5EC372AFB5}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%PUBLIC%\Music\Sample Playlists")]
        [DisplayName("Sample Playlists")]        
        [MinVersion(OSVersions.WindowsVista)]
        SamplePlaylists,

        [KnownFolderGuid("{859EAD94-2E85-48AD-A71A-0969CB56A6CD}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%PUBLIC%\Videos\Sample Videos")]
        [DisplayName("Sample Videos")]
        SampleVideos,

        [KnownFolderGuid("{4C5C32FF-BB9D-43b0-B5B4-2D72E54EAAA4}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Saved Games")]
        [DisplayName("Saved Games")]
        [MinVersion(OSVersions.WindowsVista)]
        SavedGames,

        [KnownFolderGuid("{7d1d3a04-debb-4115-95cf-2f29da2920da}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Searches")]
        [DisplayName("Searches")]
        SavedSearches,

        [KnownFolderGuid("{b7bede81-df94-4682-a7d8-57a52620b86f}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Pictures\Screenshots")]
        [DisplayName("Screenshots")]
        [MinVersion(OSVersions.Windows8)]
        Screenshots,

        [KnownFolderGuid("{ee32e446-31ca-4aba-814f-a5ebd2fd6d5e}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("Offline Files")]
        SEARCH_CSC,

        [KnownFolderGuid("{0D4C3DB6-03A3-462F-A0E6-08924C41B5D4}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Microsoft\Windows\ConnectedSearch\History")]
        [DisplayName("History")]
        [MinVersion(OSVersions.Windows81)]
        SearchHistory,

        [KnownFolderGuid("{190337d1-b8ca-4121-a639-6d472d16972a}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("Search Results")]
        SearchHome,

        [KnownFolderGuid("{98ec0e18-2098-4d44-8644-66979315a281}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("Microsoft Office Outlook")]
        SEARCH_MAPI,

        [KnownFolderGuid("{7E636BFE-DFA9-4D5E-B456-D7B39851D8A9}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Microsoft\Windows\ConnectedSearch\Templates")]
        [DisplayName("Templates")]
        [MinVersion(OSVersions.Windows81)]
        SearchTemplates,

        [KnownFolderGuid("{8983036C-27C0-404B-8F08-102D10DCFD74}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\SendTo")]
        [DefaultPath(@"%USERPROFILE%\SendTo")]
        [DisplayName("SendTo")]
        [Csidl(ShellAPI.CSIDL.CSIDL_SENDTO)]
        [SpecialFolder(Environment.SpecialFolder.SendTo)]        
        SendTo,

        [KnownFolderGuid("{7B396E54-9EC5-4300-BE0A-2482EBAE1A26}")]  
        [FolderCategory(KnownFolderCategory.Common)]
        [DefaultPath(@"%ProgramFiles%\Windows Sidebar\Gadgets")]
        [DisplayName("Gadgets")]
        [MinVersion(OSVersions.Windows7)]
        SidebarDefaultParts,

        [KnownFolderGuid("{A75D362E-50FC-4fb7-AC2C-A8BEAA314493}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Microsoft\Windows Sidebar\Gadgets")]
        [DisplayName("Gadgets")]
        [MinVersion(OSVersions.Windows7)]
        SidebarParts,

        [KnownFolderGuid("{A52BBA46-E9E1-435f-B3D9-28DAA648C0F6}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\OneDrive")]
        [DisplayName("OneDrive")]
        [MinVersion(OSVersions.Windows81)]
        SkyDrive,

        [KnownFolderGuid("{767E6811-49CB-4273-87C2-20F355E1085B}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\OneDrive\Pictures\Camera Roll")]
        [DisplayName("Camera Roll")]
        [MinVersion(OSVersions.Windows81)]
        SkyDriveCameraRoll,

        [KnownFolderGuid("{24D89E24-2F19-4534-9DDE-6A6671FBB8FE}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\OneDrive\Documents")]
        [DisplayName("Documents")]
        [MinVersion(OSVersions.Windows81)]
        SkyDriveDocuments,

        [KnownFolderGuid("{339719B5-8C47-4894-94C2-D8F77ADD44A6}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\OneDrive\Pictures")]
        [DisplayName("Pictures")]
        [MinVersion(OSVersions.Windows81)]
        SkyDrivePictures,

        [KnownFolderGuid("{625B53C3-AB48-4EC1-BA1F-A1EF4146FC19}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Start Menu")]
        [DisplayName("Start Menu")]
        [Csidl(ShellAPI.CSIDL.CSIDL_STARTMENU)]
        [SpecialFolder(Environment.SpecialFolder.StartMenu)]        
        StartMenu,

        [KnownFolderGuid("{B97D20BB-F46A-4C97-BA10-5E3608430854}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Start Menu\Programs\StartUp")]
        [DefaultPath(@"%USERPROFILE%\Start Menu\Programs\StartUp")]
        [DisplayName("Startup")]
        [Csidl(ShellAPI.CSIDL.CSIDL_STARTUP)]
        [Csidl(ShellAPI.CSIDL.CSIDL_ALTSTARTUP)]
        [SpecialFolder(Environment.SpecialFolder.Startup)]        
        Startup,

        [KnownFolderGuid("{43668BF8-C14E-49B2-97C9-747784D784B7}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]
        [DisplayName("Sync Center")]
        [MinVersion(OSVersions.WindowsVista)]
        SyncManagerFolder,

        [KnownFolderGuid("{289a9a43-be44-4057-a41b-587a76d7e7f9}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("Sync Results")]
        [MinVersion(OSVersions.WindowsVista)]
        SyncResultsFolder,

        [KnownFolderGuid("{0F214138-B1D3-4a90-BBA9-27CBC0C5389A}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("Sync Setup")]
        [MinVersion(OSVersions.WindowsVista)]
        SyncSetupFolder,

        [KnownFolderGuid("{1AC14E77-02E7-4E5D-B744-2EB1AE5198B7}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%windir%\system32")]
        [DisplayName("system32")]
        [Csidl(ShellAPI.CSIDL.CSIDL_SYSTEM)]
        [SpecialFolder(Environment.SpecialFolder.System)]        
        System,

        [KnownFolderGuid("{D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%windir%\system32")]
        [DisplayName("system32")]
        [Csidl(ShellAPI.CSIDL.CSIDL_SYSTEMX86)]
        [SpecialFolder(Environment.SpecialFolder.SystemX86)]
        SystemX86,

        [KnownFolderGuid("{A63293E8-664E-48DB-A079-DF759E0509F7}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Templates")]
        [DefaultPath(@"%USERPROFILE%\Templates")]
        [DisplayName("Templates")]
        [Csidl(ShellAPI.CSIDL.CSIDL_TEMPLATES)]
        [SpecialFolder(Environment.SpecialFolder.Templates)]        
        Templates,
        
        //[MinVersion(OSVersions.WindowsVista)]
        //TreeProperties,

        [KnownFolderGuid("{9E3995AB-1F9C-4F13-B827-48B24B6C7174}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Internet Explorer\Quick Launch\User Pinned")]
        [DisplayName("User Pinned")]
        [MinVersion(OSVersions.Windows7)]
        UserPinned,

        [KnownFolderGuid("{0762D272-C50A-4BB0-A382-697DCD729B80}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%SystemDrive%\Users")]
        [DisplayName("Users")]
        [MinVersion(OSVersions.WindowsVista)]
        UserProfiles,

        [KnownFolderGuid("{5CD7AEE2-2219-4A67-B85D-6C9CE15660CB}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Programs")]
        [DisplayName("Programs")]
        [MinVersion(OSVersions.Windows7)]
        UserProgramFiles,

        [KnownFolderGuid("{BCBD3057-CA5C-4622-B42D-BC56DB0AE516}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%LOCALAPPDATA%\Programs\Common")]
        [DisplayName("Programs")]
        [MinVersion(OSVersions.Windows7)]
        UserProgramFilesCommon,

        [KnownFolderGuid("{f3ce0f7c-4901-4acc-8648-d5d44b04ef8f}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("%USERNAME%")]
        UsersFiles,

        [KnownFolderGuid("{A302545D-DEFF-464b-ABE8-61C8648D939B}")]  
        [FolderCategory(KnownFolderCategory.Virtual)]        
        [DisplayName("Libraries")]
        [MinVersion(OSVersions.Windows7)]
        UsersLibraries,

        [KnownFolderGuid("{18989B1D-99B5-455B-841C-AB7C74E4DDFC}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%USERPROFILE%\Videos")]
        [DefaultPath(@"%USERPROFILE%\My Documents\My Videos")]
        [DisplayName("Videos")]
        [Csidl(ShellAPI.CSIDL.CSIDL_MYVIDEO)]
        [SpecialFolder(Environment.SpecialFolder.MyVideos)]        
        Videos,

        [KnownFolderGuid("{491E922F-5643-4AF4-A7EB-4E7A138D8174}")]  
        [FolderCategory(KnownFolderCategory.PerUser)]
        [DefaultPath(@"%APPDATA%\Microsoft\Windows\Libraries\Videos.library-ms")]
        [DisplayName("Videos")]
        [MinVersion(OSVersions.Windows7)]
        VideosLibrary,

        [KnownFolderGuid("{F38BF404-1D43-42F2-9305-67DE0B28FC23}")]  
        [FolderCategory(KnownFolderCategory.Fixed)]
        [DefaultPath(@"%windir%")]
        [DisplayName("Windows")]
        [Csidl(ShellAPI.CSIDL.CSIDL_WINDOWS)]
        [SpecialFolder(Environment.SpecialFolder.Windows)]        
        Windows

        //[KnownFolderGuid("")]  
        //[FolderCategory(KnownFolderCategory.)]
        //[DefaultPath(@"")]
        //[DisplayName("")]
        //[Csidl(ShellAPI.CSIDL.)]
        //[SpecialFolder(Environment.SpecialFolder)]
        //[MinVersion(OSVersions.Windows81)]

    }
}
