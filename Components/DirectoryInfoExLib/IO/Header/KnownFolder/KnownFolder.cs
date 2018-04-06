namespace DirectoryInfoExLib.IO.Header.KnownFolder
{
    using DirectoryInfoExLib.IO.Header.KnownFolder.Attributes;
    using DirectoryInfoExLib.IO.Header.KnownFolder.Enums;
    using DirectoryInfoExLib.IO.Header.KnownFolder.Interfaces;
    using DirectoryInfoExLib.IO.Header.ShellDll;
    using DirectoryInfoExLib.Tools;
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
    /// </summary>
    public class KnownFolder : IDisposable
    {
        #region fields
        /// <summary>
        /// Gets the <see cref="KnownFolderManager"/> for this system.
        /// </summary>
        public static IKnownFolderManager FolderManager = (IKnownFolderManager)new KnownFolderManager();

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
        /// on the inner known folder interface and folder manager.
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
        /// Gets the <see cref="Guid"/> id of the known folder,
        /// if it is present in the enumeration of known folder,
        /// or null, if the Id is not present in the enumeration.
        /// </summary>
        public KnownFolderIds? KnownFolderId
        {
            get
            {
                Guid id = Id;
                foreach (var kfId in Enum.GetValues(typeof(KnownFolderIds)))
                {
                    var attribute = EnumAttributeUtils<KnownFolderGuidAttribute, KnownFolderIds>.FindAttribute(kfId);

                    if (attribute != null && attribute.Guid.Equals(id))
                        return (KnownFolderIds)kfId;
                }

                return null;
            }
        }

        public Environment.SpecialFolder? SpecialFolder
        {
            get
            {
                var kfId = KnownFolderId;
                if (kfId != null)
                {
                    var attribute = EnumAttributeUtils<SpecialFolderAttribute, KnownFolderIds>.FindAttribute(kfId);
                    if (attribute != null)
                        return attribute.SpecialFolder;
                }
                return null;
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

        internal ShellAPI.CSIDL? CSIDL
        {
            get
            {
                try
                {
                    int csidl;
                    FolderManager.FolderIdToCsidl(this.Id, out csidl);
                    ShellAPI.CSIDL result;
                    if (Enum.TryParse<ShellAPI.CSIDL>(csidl.ToString(), out result))
                        return result;
                }
                catch (ArgumentException) { }

                return null;
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
            KnownFolder.FolderManager.GetFolder(knownFolderID, out knowFolderInterface);
            return new KnownFolder(knowFolderInterface);
        }

        public static KnownFolder FromKnownFolderId(KnownFolderIds kfId)
        {
            var guidAtb = EnumAttributeUtils<KnownFolderGuidAttribute, KnownFolderIds>.FindAttribute(kfId);
            return FromKnownFolderId(guidAtb.Guid);
        }

        public static KnownFolder FromKnownFolderName(string canonicalName)
        {
            IKnownFolder knowFolderInterface;
            KnownFolder.FolderManager.GetFolderByName(canonicalName, out knowFolderInterface);
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
                int hr = KnownFolder.FolderManager.FindFolderFromIDList(pidl.Ptr, out knowFolderInterface);
                if (knowFolderInterface != null && hr == ShellAPI.S_OK)
                    return new KnownFolder(knowFolderInterface);
                else return null;
            }
            catch
            {
                return null;
            }
        }

        public static KnownFolder FromDirectoryPath(string path, KnownFolderFindMode mode)
        {
            IKnownFolder knowFolderInterface;
            try
            {
                KnownFolder.FolderManager.FindFolderFromPath(path, mode, out knowFolderInterface);
                return new KnownFolder(knowFolderInterface);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region methods

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

        public void SetPath(string path, KnownFolderRetrievalOptions options)
        {
            _knownFolder.SetPath(options, path);
        }

        public void Dispose()
        {
            if (_knownFolder != null)
            {
                Marshal.ReleaseComObject(_knownFolder);
                _knownFolder = null;
            }
        }

        /// <summary>
        /// Gets the definition for a known folder.
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

        public static List<KnownFolder> GetKnownFolders()
        {
            IList<KnownFolder> foldersList = new List<KnownFolder>();
            uint count;
            IntPtr folders = IntPtr.Zero;

            try
            {
                //Microsoft.WindowsAPICodePack.Shell                
                KnownFolder.FolderManager.GetFolderIds(out folders, out count);

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
        #endregion
    }
}
