using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Utils;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShellDll
{
    public class KnownFolder : IDisposable
    {
        #region fields

        public static IKnownFolderManager FolderManager = (IKnownFolderManager)new KnownFolderManager();

        #endregion

        internal IKnownFolder _knownFolder = null;

        internal KnownFolder(IKnownFolder knownFolder)
        {
            _knownFolder = knownFolder;
        }

        ~KnownFolder()
        {
            Dispose();
        }


        #region properties

        public string Path
        {
            get { return GetPath(0); }
            set { SetPath(value, 0); }
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

        public ShellAPI.CSIDL? CSIDL
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

        #endregion

        #region static methods

        public static KnownFolder FromCsidl(ShellAPI.CSIDL csidl)
        {
            var folderManager = (IKnownFolderManager)new KnownFolderManager();

            Guid knownFolderID;
            folderManager.FolderIdFromCsidl((int)csidl, out knownFolderID);
            IKnownFolder knowFolderInterface;
            folderManager.GetFolder(knownFolderID, out knowFolderInterface);
            return new KnownFolder(knowFolderInterface);
        }

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

        public static KnownFolder FromPidl(PIDL pidl)
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

        // This is not a small operation so let's make it a method
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
