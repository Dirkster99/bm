namespace DirectoryInfoExLib.IO.Header.KnownFolder
{
    using DirectoryInfoExLib.IO.Header.KnownFolder.Enums;
    using DirectoryInfoExLib.IO.Header.KnownFolder.Interfaces;
    using DirectoryInfoExLib.IO.Header.ShellDll;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    
    /// <summary>
    /// Windows Vista introduces new storage scenarios and a new user profile namespace.
    /// To address these new factors, the older system of referring to standard folders by
    /// a CSIDL value has been replaced. As of Windows Vista, those folders are referenced
    /// by a new set of GUID values called Known Folder IDs.
    ///
    /// The Known Folder system provides these advantages:
    ///
    /// Independent software vendors(ISVs) can extend the set of Known Folder IDs with their own.
    /// They can define folders, give them IDs, and register them with the system.
    /// CSIDL values could not be extended.
    /// 
    /// All Known Folders on a system can be enumerated. No API provided this functionality for
    /// CSIDL values. See IKnownFolderManager::GetFolderIds for more information.
    ///
    /// A known folder added by an ISV can add custom properties that allow it to explain its
    /// purpose and intended use.
    /// 
    /// Many known folders can be redirected to new locations, including network locations.
    /// Under the CSIDL system, only the My Documents folder could be redirected.
    /// 
    /// Known folders can have custom handlers for use during creation or deletion.
    ///
    /// The CSIDL system and APIs that make use of CSIDL values are still supported for compatibility.
    /// However, it is not recommended to use them in any new development.
    /// 
    /// https://msdn.microsoft.com/en-us/library/bb776911
    ///
    /// Microsoft.SDK.Samples.VistaBridge.Library.Shell
    /// https://blogs.msdn.microsoft.com/yvesdolc/2007/02/14/known-folders-the-return/
    /// </summary>
    internal class KnownFolder : IDisposable
    {
        #region fields
        /// <summary>
        /// Gets the <see cref="KnownFolderManager"/> for this system.
        /// </summary>
        private static IKnownFolderManager _FolderManager = (IKnownFolderManager)new KnownFolderManager();

        private IKnownFolder _knownFolder = null;
        #endregion

        #region constructor
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="knownFolder"></param>
        internal KnownFolder(IKnownFolder knownFolder)
        {
            _knownFolder = knownFolder;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~KnownFolder()
        {
            Dispose();
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets the path of a known folder.
        /// </summary>
        public string Path
        {
            get { return GetPath(0); }

            protected set { SetPath(value, 0); }
        }

        /// <summary>
        /// Gets the <see cref="Guid"/> id of the known folder based
        /// on the inner native known folder interface and folder manager.
        /// </summary>
        public Guid Id
        {
            get
            {
                Guid id;
                _knownFolder.GetId(out id);

                return id;
            }
        }

        /// <summary>
        /// Gets the folder type.
        /// 
        /// Returns a a GUID that identifies the known folder type.
        /// </summary>
        public Guid FolderType
        {
            get
            {
                Guid type;
                _knownFolder.GetFolderType(out type);

                return type;
            }
        }

        /// <summary>
        /// Retrieves the category—virtual,
        /// fixed, common,
        /// or per-user—of the selected folder.
        /// </summary>
        public KnownFolderCategory Category
        {
            get
            {
                KnownFolderCategory category;
                _knownFolder.GetCategory(out category);

                return category;
            }
        }

        /// <summary>
        /// Gets a value that states whether the known folder
        /// can have its path set to a new value or what specific restrictions
        /// or prohibitions are placed on that redirection.
        /// </summary>
        public KnownFolderRedirectionCapabilities RedirectionCapabilities
        {
            get
            {
                KnownFolderRedirectionCapabilities redirectionCapabilities;
                _knownFolder.GetRedirectionCapabilities(out redirectionCapabilities);

                return redirectionCapabilities;
            }
        }

        /// <summary>
        /// Retrieves a structure that contains the defining elements
        /// of a known folder, which includes the folder's category,
        /// name, path, description, tooltip, icon, and other properties.
        /// </summary>
        public KnownFolderDefinition Definition
        {
            get { return GetDefinition(); }
        }

        internal IKnownFolder KnownFolderInterface
        {
            get
            {
                return _knownFolder;
            }
        }
        #endregion

        #region static methods
        /// <summary>
        /// Creates a <see cref="KnownFolder"/> object from a <see cref="Guid"/>
        /// that represents a known shell folder.
        /// <seealso cref="DirectoryInfoExLib.Enums.KnownFolder_GUIDS"/>
        /// 
        /// https://msdn.microsoft.com/en-us/library/bb762584(VS.85).aspx
        /// </summary>
        /// <param name="knownFolderID"></param>
        /// <returns></returns>
        public static KnownFolder FromKnownFolderId(Guid knownFolderID)
        {
            IKnownFolder knowFolderInterface;
            KnownFolder._FolderManager.GetFolder(knownFolderID, out knowFolderInterface);

            return new KnownFolder(knowFolderInterface);
        }

        /// <summary>
        /// Gets the Windows known folder (similar to <see cref="Environment.SpecialFolder"/>
        /// but extensible and customizable at run-time) or null if the given <see cref="PIDL"/>
        /// does not refer to a special folder in Windows.
        /// </summary>
        /// <param name="pidl"></param>
        /// <returns></returns>
        internal static KnownFolder FromPidl(PIDL pidl)
        {
            IKnownFolder knowFolderInterface;
            try
            {
                int hr = KnownFolder._FolderManager.FindFolderFromIDList(pidl.Ptr, out knowFolderInterface);
                
                if (knowFolderInterface != null && hr == ShellAPI.S_OK)
                    return new KnownFolder(knowFolderInterface);
                else
                  return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets an object that represents a known folder based on a file
        /// system path. The object allows you to query certain folder
        /// properties, get the current path of the folder, redirect
        /// the folder to another location, and get the path of the folder
        /// as an ITEMIDLIST.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static KnownFolder FromDirectoryPath(
            string path,
            KnownFolderFindMode mode)
        {
            IKnownFolder knowFolderInterface;
            try
            {
                KnownFolder._FolderManager.FindFolderFromPath(path, mode, out knowFolderInterface);

                return new KnownFolder(knowFolderInterface);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Gets all known folders of the Shell and returns them as a list.
        /// </summary>
        /// <returns></returns>
        public static List<KnownFolder> GetKnownFolders()
        {
            IList<KnownFolder> foldersList = new List<KnownFolder>();
            uint count;
            IntPtr folders = IntPtr.Zero;

            try
            {
                // Microsoft.WindowsAPICodePack.Shell                
                KnownFolder._FolderManager.GetFolderIds(out folders, out count);

                if (count > 0 && folders != IntPtr.Zero)
                {
                    // Loop through all the KnownFolderID elements
                    for (int i = 0; i < count; i++)
                    {
                        // Read the current pointer
                        IntPtr current = new IntPtr(folders.ToInt64() + (Marshal.SizeOf(typeof(Guid)) * i));

                        // Convert to Guid
                        Guid knownFolderID = (Guid)Marshal.PtrToStructure(current, typeof(Guid));

                        KnownFolder kf = KnownFolder.FromKnownFolderId(knownFolderID);

                        // Add to our collection if it's not null (some folders might not exist on the system
                        // or we could have an exception that resulted in the null return from above method call
                        if (kf != null) { foldersList.Add(kf); }
                    }
                }
            }
            finally
            {
                if (folders != IntPtr.Zero) { Marshal.FreeCoTaskMem(folders); }
            }

            return foldersList.OrderBy(kf => kf.Category).ToList();
        }

        /// <summary>
        /// Gets the string based path of a real folder or null
        /// if the folder is virtual etc...
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public string GetPath(KnownFolderRetrievalOptions options)
        {
            IntPtr pointerToPath = IntPtr.Zero;
            string path;
            try
            {
                if (Category == KnownFolderCategory.Virtual)
                    return null;
                else
                {
                    _knownFolder.GetPath(options, out pointerToPath);
                    path = Marshal.PtrToStringUni(pointerToPath);
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                path = null;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                path = null;
            }
            finally
            {
                Marshal.FreeCoTaskMem(pointerToPath);
            }
            return path;
        }

        /// <summary>
        /// Implements standard disposable interface.
        /// </summary>
        public void Dispose()
        {
            if (_knownFolder != null)
            {
                Marshal.ReleaseComObject(_knownFolder);
                _knownFolder = null;
            }
        }

        /// <summary>
        /// Gets the string based path for this known folder.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="options"></param>
        protected void SetPath(string path, KnownFolderRetrievalOptions options)
        {
            _knownFolder.SetPath(options, path);
        }

        /// <summary>
        /// Gets the public definition for a known folder.
        /// 
        /// Retrieves a structure that contains the defining elements
        /// of a known folder, which includes the folder's category,
        /// name, path, description, tooltip, icon, and other properties.
        /// 
        /// This is not a small operation so let's make it a method
        /// </summary>
        /// <returns></returns>
        private KnownFolderDefinition GetDefinition()
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
        #endregion
    }
}
