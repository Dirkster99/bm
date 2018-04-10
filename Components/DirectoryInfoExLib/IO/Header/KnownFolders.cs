namespace DirectoryInfoExLib.IO.Header
{
    using DirectoryInfoExLib.Enums;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Microsoft.SDK.Samples.VistaBridge.Library.Shell
    /// https://blogs.msdn.microsoft.com/yvesdolc/2007/02/14/known-folders-the-return/
    /// </summary>
    public enum KnownFolderFindMode : int
    {
        ExactMatch = 0,
        NearestParentMatch = ExactMatch + 1
    };

    /// <summary>
    /// Microsoft.SDK.Samples.VistaBridge.Library.Shell
    /// https://blogs.msdn.microsoft.com/yvesdolc/2007/02/14/known-folders-the-return/
    /// </summary>
    public class KnownFolders
    {
    #region enums
        public enum KnownFolderCategory
        {
            Virtual = 1,
            Fixed = 2,
            Common = 3,
            PerUser = 4
        }

        public enum KnownFolderRetrievalOptions
        {
            Create = 0x00008000,
            DontVerify = 0x00004000,
            DontUnexpand = 0x00002000,
            NoAlias = 0x00001000,
            Init = 0x00000800,
            DefaultPath = 0x00000400,
            NotParentRelative = 0x00000200
        }

        public enum KnownFolderRedirectionCapabilities
        {
            AllowAll = 0xff,
            Redirectable = 0x1,
            DenyAll = 0xfff00,
            DenyPolicyRedirected = 0x100,
            DenyPolicy = 0x200,
            DenyPermissions = 0x400
        }

        public enum KnownFolderDefinitionFlags
        {
            LocalRedirectOnly = 0x2,
            Roamable = 0x4,
            Precreate = 0x8
        }
    #endregion enums

       public struct KnownFolderDefinition
       {
          public KnownFolderCategory Category;
          public string Name;
          public string Description;
          public Guid ParentID;
          public string RelativePath;
          public string ParsingName;
          public string Tooltip;
          public string LocalizedName;
          public string Icon;
          public string Security;
          public UInt32 Attributes;
          public KnownFolderDefinitionFlags DefinitionFlags;
          public Guid FolderTypeID;
      }

        public static string Desktop { get { return GetPath(KnownFolder_GUIDS.Desktop); } }
        public static string Fonts { get { return GetPath(KnownFolder_GUIDS.Fonts); } }
        public static string Startup { get { return GetPath(KnownFolder_GUIDS.Startup); } }
        public static string Programs { get { return GetPath(KnownFolder_GUIDS.Programs); } }
        public static string StartMenu { get { return GetPath(KnownFolder_GUIDS.StartMenu); } }
        public static string Recent { get { return GetPath(KnownFolder_GUIDS.Recent); } }
        public static string SendTo { get { return GetPath(KnownFolder_GUIDS.SendTo); } }
        public static string Documents { get { return GetPath(KnownFolder_GUIDS.Documents); } }
        public static string Favorites { get { return GetPath(KnownFolder_GUIDS.Favorites); } }
        public static string NetHood { get { return GetPath(KnownFolder_GUIDS.NetHood); } }
        public static string PrintHood { get { return GetPath(KnownFolder_GUIDS.PrintHood); } }
        public static string Templates { get { return GetPath(KnownFolder_GUIDS.Templates); } }
        public static string CommonStartup { get { return GetPath(KnownFolder_GUIDS.CommonStartup); } }
        public static string CommonPrograms { get { return GetPath(KnownFolder_GUIDS.CommonPrograms); } }
        public static string CommonStartMenu { get { return GetPath(KnownFolder_GUIDS.CommonStartMenu); } }
        public static string PublicDesktop { get { return GetPath(KnownFolder_GUIDS.PublicDesktop); } }
        public static string ProgramData { get { return GetPath(KnownFolder_GUIDS.ProgramData); } }
        public static string CommonTemplates { get { return GetPath(KnownFolder_GUIDS.CommonTemplates); } }
        public static string PublicDocuments { get { return GetPath(KnownFolder_GUIDS.PublicDocuments); } }
        public static string RoamingAppData { get { return GetPath(KnownFolder_GUIDS.RoamingAppData); } }
        public static string LocalAppData { get { return GetPath(KnownFolder_GUIDS.LocalAppData); } }
        public static string LocalAppDataLow { get { return GetPath(KnownFolder_GUIDS.LocalAppDataLow); } }
        public static string InternetCache { get { return GetPath(KnownFolder_GUIDS.InternetCache); } }
        public static string Cookies { get { return GetPath(KnownFolder_GUIDS.Cookies); } }
        public static string History { get { return GetPath(KnownFolder_GUIDS.History); } }
        public static string System { get { return GetPath(KnownFolder_GUIDS.System); } }
        public static string SystemX86 { get { return GetPath(KnownFolder_GUIDS.SystemX86); } }
        public static string Windows { get { return GetPath(KnownFolder_GUIDS.Windows); } }
        public static string Profile { get { return GetPath(KnownFolder_GUIDS.Profile); } }
        public static string Pictures { get { return GetPath(KnownFolder_GUIDS.Pictures); } }
        public static string ProgramFilesX86 { get { return GetPath(KnownFolder_GUIDS.ProgramFilesX86); } }
        public static string ProgramFilesCommonX86 { get { return GetPath(KnownFolder_GUIDS.ProgramFilesCommonX86); } }
        public static string ProgramFilesX64 { get { return GetPath(KnownFolder_GUIDS.ProgramFilesX64); } }
        public static string ProgramFilesCommonX64 { get { return GetPath(KnownFolder_GUIDS.ProgramFilesCommonX64); } }
        public static string ProgramFiles { get { return GetPath(KnownFolder_GUIDS.ProgramFiles); } }
        public static string ProgramFilesCommon { get { return GetPath(KnownFolder_GUIDS.ProgramFilesCommon); } }
        public static string AdminTools { get { return GetPath(KnownFolder_GUIDS.AdminTools); } }
        public static string CommonAdminTools { get { return GetPath(KnownFolder_GUIDS.CommonAdminTools); } }
        public static string Music { get { return GetPath(KnownFolder_GUIDS.Music); } }
        public static string Videos { get { return GetPath(KnownFolder_GUIDS.Videos); } }
        public static string PublicPictures { get { return GetPath(KnownFolder_GUIDS.PublicPictures); } }
        public static string PublicMusic { get { return GetPath(KnownFolder_GUIDS.PublicMusic); } }
        public static string PublicVideos { get { return GetPath(KnownFolder_GUIDS.PublicVideos); } }
        public static string ResourceDir { get { return GetPath(KnownFolder_GUIDS.ResourceDir); } }
        public static string LocalizedResourcesDir { get { return GetPath(KnownFolder_GUIDS.LocalizedResourcesDir); } }
        public static string CommonOEMLinks { get { return GetPath(KnownFolder_GUIDS.CommonOEMLinks); } }
        public static string CDBurning { get { return GetPath(KnownFolder_GUIDS.CDBurning); } }
        public static string UserProfiles { get { return GetPath(KnownFolder_GUIDS.UserProfiles); } }
        public static string Public { get { return GetPath(KnownFolder_GUIDS.Public); } }
        public static string Downloads { get { return GetPath(KnownFolder_GUIDS.Downloads); } }
        public static string PublicDownloads { get { return GetPath(KnownFolder_GUIDS.PublicDownloads); } }
        public static string SavedSearches { get { return GetPath(KnownFolder_GUIDS.SavedSearches); } }
        public static string QuickLaunch { get { return GetPath(KnownFolder_GUIDS.QuickLaunch); } }
        public static string Contacts { get { return GetPath(KnownFolder_GUIDS.Contacts); } }
        public static string SidebarParts { get { return GetPath(KnownFolder_GUIDS.SidebarParts); } }
        public static string SidebarDefaultParts { get { return GetPath(KnownFolder_GUIDS.SidebarDefaultParts); } }
        public static string PublicGameTasks { get { return GetPath(KnownFolder_GUIDS.PublicGameTasks); } }
        public static string GameTasks { get { return GetPath(KnownFolder_GUIDS.GameTasks); } }
        public static string SavedGames { get { return GetPath(KnownFolder_GUIDS.SavedGames); } }
        public static string Links { get { return GetPath(KnownFolder_GUIDS.Links); } }
      
        /// <summary>
        /// Exposes methods that allow an application to retrieve information about
        /// a known folder's category, type, GUID, pointer to an item identifier list (PIDL) value,
        /// redirection capabilities, and definition. It provides a method for the retrieval of
        /// a known folder's IShellItem object. It also provides methods to get or set the path
        /// of the known folder.
        ///
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb761768(v=vs.85).aspx
        /// </summary>
        [Guid("8BE2D872-86AA-4d47-B776-32CCA40C7018"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IKnownFolderManager
        {
            void FolderIdFromCsidl(int Csidl, [Out] out Guid knownFolderID);
            void FolderIdToCsidl([In, MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out] out int Csidl);
            //    HRESULT GetFolderIds( [out, size_is( , *pCount)] KnownFolderID ** ppKFId, [in, out] UINT *pCount);
            void GetFolderIds();
            void GetFolder([In, MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out, MarshalAs(UnmanagedType.Interface)] out KnownFolder.IKnownFolder knownFolder);
            void GetFolderByName(string canonicalName, [Out, MarshalAs(UnmanagedType.Interface)] out KnownFolder.IKnownFolder knowFolder);
            void RegisterFolder();  //([in] REFKNOWNFOLDERID rfid,[in] KNOWNFOLDER_DEFINITION const *pKFD);
            void UnregisterFolder(); //([in] REFKNOWNFOLDERID rfid);
            void FindFolderFromPath([In, MarshalAs(UnmanagedType.LPWStr)] string path, [In] KnownFolderFindMode mode, [Out, MarshalAs(UnmanagedType.Interface)] out KnownFolder.IKnownFolder knownFolder);
            void FindFolderFromIDList(); //( [in] PCIDLIST_ABSOLUTE pidl, [out] IKnownFolder **ppkf);
            void Redirect(); //( [in] REFKNOWNFOLDERID rfid, [in, unique] HWND hwnd, [in] KF_REDIRECT_FLAGS flags,[in, unique, string] LPCWSTR pszTargetPath, [in] UINT cFolders, [in, size_is(cFolders), unique] KnownFolderID const *pExclusion,[out, string] LPWSTR* ppszError);
        }

        [ComImport, Guid("4df0c730-df9d-4ae3-9153-aa6b82e9795a")]
        internal class NativeKnownFolderManager 
        {
        }

        /// <summary>
        /// Gets the <see cref="NativeKnownFolderManager "/> for this system.
        /// </summary>
        static IKnownFolderManager _knownFolderManager = (IKnownFolderManager)new NativeKnownFolderManager();

        public static KnownFolder GetKnownFolder(Guid knownFolderID)
        {
            KnownFolder.IKnownFolder knowFolderInterface;
            _knownFolderManager.GetFolder(knownFolderID, out knowFolderInterface);
            return new KnownFolder(knowFolderInterface);
        }

        public static KnownFolder GetKnownFolder(string canonicalName)
        {
            KnownFolder.IKnownFolder knowFolderInterface;
            _knownFolderManager.GetFolderByName(canonicalName, out knowFolderInterface);
            return new KnownFolder(knowFolderInterface);
        }

        public static KnownFolder FindFolderFromPath(string path, KnownFolderFindMode mode)
        {
            KnownFolder.IKnownFolder knowFolderInterface;
            _knownFolderManager.FindFolderFromPath(path, mode, out knowFolderInterface);
            return new KnownFolder(knowFolderInterface);
        }

        private static string GetPath(Guid knownFolderID)
        {
            IntPtr pointerToPath = IntPtr.Zero;
            string path;
            try
            {
                KnownFolder.IKnownFolder knowFolderInterface;
                _knownFolderManager.GetFolder(knownFolderID, out knowFolderInterface);
                knowFolderInterface.GetPath(0, out pointerToPath);
                path = Marshal.PtrToStringUni(pointerToPath);
            }
            finally
            {
                Marshal.FreeCoTaskMem(pointerToPath);
            }
            return path;
        }
    }

    public class KnownFolder
    {
        [ComImport, Guid("3AA7AF7E-9B36-420c-A8E3-F77D4674A488"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IKnownFolder
        {
            void GetId([Out] out Guid Id);
            void GetCategory([Out] out KnownFolderCategory category);
            void GetShellItem(KnownFolderRetrievalOptions retrievalOptions,
                              [MarshalAs(UnmanagedType.LPStruct)] Guid interfaceGuid,
                              [Out, MarshalAs(UnmanagedType.IUnknown)] out object shellItem);
            void GetPath(KnownFolderRetrievalOptions retrievalOptions, [Out] out IntPtr path);
            void SetPath(KnownFolderRetrievalOptions retrievalOptions, string path);
            void GetIDList(KnownFolderRetrievalOptions retrievalOptions, [Out] out IntPtr itemIdentifierListPointer);
            void GetFolderType([Out, MarshalAs(UnmanagedType.LPStruct)] out Guid folderTypeID);
            void GetRedirectionCapabilities([Out] out KnownFolderRedirectionCapabilities redirectionCapabilities);
            void GetFolderDefinition([Out, MarshalAs(UnmanagedType.Struct)] out InternalKnownFolderDefinition definition);
        }

        IKnownFolder _knownFolder = null;

        internal KnownFolder(IKnownFolder knownFolder)
        {
            _knownFolder = knownFolder;
        }

        public string Path
        {
            get { return GetPath(0); }
            set { SetPath(value, 0); }
        }

        public string GetPath(KnownFolderRetrievalOptions options)
        {
            IntPtr pointerToPath = IntPtr.Zero;
            string path;
            try
            {
                _knownFolder.GetPath(options, out pointerToPath);
                path = Marshal.PtrToStringUni(pointerToPath);
            }
            finally
            {
                Marshal.FreeCoTaskMem(pointerToPath);
            }
            return path;
        }

        public void SetPath(string path, KnownFolderRetrievalOptions options)
        {
            _knownFolder.SetPath(options, path);
        }

        public Guid Id
        {
            get
            {
                Guid id;
                _knownFolder.GetId(out id);
                return id;
            }
        }

        public Guid FolderType
        {
            get
            {
                Guid type;
                _knownFolder.GetFolderType(out type);
                return type;
            }
        }
        public KnownFolderCategory Category
        {
            get
            {
                KnownFolderCategory category;
                _knownFolder.GetCategory(out category);
                return category;
            }
        }
        public KnownFolderRedirectionCapabilities RedirectionCapabilities
        {
            get
            {
                KnownFolderRedirectionCapabilities redirectionCapabilities;
                _knownFolder.GetRedirectionCapabilities(out redirectionCapabilities);
                return redirectionCapabilities;
            }
        }

#pragma warning disable 0649
        internal struct InternalKnownFolderDefinition
        {
            internal KnownFolderCategory Category;
            internal IntPtr pszName;
            internal IntPtr pszDescription;
            internal Guid ParentID;
            internal IntPtr pszRelativePath;
            internal IntPtr pszParsingName;
            internal IntPtr pszTooltip;
            internal IntPtr pszLocalizedName;
            internal IntPtr pszIcon;
            internal IntPtr pszSecurity;
            internal UInt32 dwAttributes;
            internal KnownFolderDefinitionFlags DefinitionFlags;
            internal Guid FolderTypeID;
        }
#pragma warning restore 0649

        // This is not a small operation so let's make it a method
        public KnownFolderDefinition GetDefinition()
        {
            InternalKnownFolderDefinition internalDefinition;
            KnownFolderDefinition definition = new KnownFolderDefinition();
            _knownFolder.GetFolderDefinition(out internalDefinition);
            try
            {
                definition.Category = internalDefinition.Category;
                definition.Name = Marshal.PtrToStringUni(internalDefinition.pszName);
                definition.Description = Marshal.PtrToStringUni(internalDefinition.pszDescription);
                definition.ParentID = internalDefinition.ParentID;
                definition.ParsingName = Marshal.PtrToStringUni(internalDefinition.pszParsingName);
                definition.Tooltip = Marshal.PtrToStringUni(internalDefinition.pszTooltip);
                definition.LocalizedName = Marshal.PtrToStringUni(internalDefinition.pszLocalizedName);
                definition.Icon = Marshal.PtrToStringUni(internalDefinition.pszIcon);
                definition.Security = Marshal.PtrToStringUni(internalDefinition.pszSecurity);
                definition.Attributes = internalDefinition.dwAttributes;
                definition.DefinitionFlags = internalDefinition.DefinitionFlags;
                definition.FolderTypeID = internalDefinition.FolderTypeID;
            }
            finally
            {
                Marshal.FreeCoTaskMem(internalDefinition.pszName);
                Marshal.FreeCoTaskMem(internalDefinition.pszDescription);
                Marshal.FreeCoTaskMem(internalDefinition.pszRelativePath);
                Marshal.FreeCoTaskMem(internalDefinition.pszParsingName);
                Marshal.FreeCoTaskMem(internalDefinition.pszTooltip);
                Marshal.FreeCoTaskMem(internalDefinition.pszLocalizedName);
                Marshal.FreeCoTaskMem(internalDefinition.pszIcon);
                Marshal.FreeCoTaskMem(internalDefinition.pszSecurity);
            }
            return definition;
        }
    }

} // namespace