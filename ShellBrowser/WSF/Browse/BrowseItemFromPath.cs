namespace WSF.Browse
{
    using WSF.Enums;
    using WSF.IDs;
    using WSF.Shell.Interop.Dlls;
    using WSF.Shell.Interop.Interfaces.Knownfolders;
    using WSF.Shell.Interop.Interfaces.KnownFolders;
    using WSF.Shell.Interop.Interfaces.ShellFolders;
    using WSF.Shell.Interop.Knownfolders;
    using WSF.Shell.Interop.ShellFolders;
    using WSF.Shell.Pidl;
    using WSF.Shell.Enums;
    using WSF.Shell.Interop.Interfaces.ShellItems;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using WSF.Shell.Interop.ResourceIds;

    /// <summary>
    /// Implements a class that contains all API calls and methods necessary to initialize
    /// an <seealso cref="DirectoryBrowser"/> object through its constructor.
    /// </summary>
    internal class BrowseItemFromPath
    {
        #region constructors
        /// <summary>
        /// General purpose constructor to initialize only <see cref="Path_RAW"/>.
        /// </summary>
        /// <param name="rawPath"></param>
        /// <param name="pathType"></param>
        /// <param name="itemType"></param>
        public BrowseItemFromPath(string rawPath,
                                  PathHandler pathType,
                                  DirectoryItemFlags itemType = DirectoryItemFlags.Unknown)
            : this()
        {
            Path_RAW = rawPath;
            PathType = pathType;
            ItemType = itemType;
        }

        public BrowseItemFromPath(string path, string normPath, bool isSpecialID,
                                  string parseName,
                                  string name,
                                  string labelName,
                                  string pathSpecialItemId,
                                  IdList parentIdList, IdList relativeChildIdList,
                                  PathHandler pathType,
                                  DirectoryItemFlags itemType,
                                  IKnownFolderProperties props = null)
        {
            this.Path_RAW = path;
            this.Name = name;
            this.ParseName = parseName;
            this.LabelName = labelName;

            this.PathSpecialItemId = pathSpecialItemId;

            this.PathFileSystem = normPath;

            this.IsSpecialFolder = isSpecialID;
            this.ParentIdList = parentIdList;
            this.ChildIdList = relativeChildIdList;

            this.PathType = pathType;
            this.ItemType = itemType;

            if (props != null)
            {
                IconResourceId = props.IconResourceId;
            }
        }


        /// <summary>
        /// constructor to initialize only <see cref="Path_RAW"/> and
        /// <see cref="PathFileSystem"/> property or
        /// <see cref="PathSpecialItemId"/> property. 
        /// <paramref name="rawPath"/> can be either a real path
        /// or special folder path starting with '::{...}'
        /// </summary>
        /// <param name="rawPath"></param>
        /// <param name="parsingName"></param>
        /// <param name="pathType"></param>
        protected BrowseItemFromPath(string rawPath,
                                      string parsingName,
                                      PathHandler pathType)
            : this()
        {
            Path_RAW = rawPath;

            if (ShellHelpers.IsSpecialPath(parsingName) == ShellHelpers.SpecialPath.None)
                PathFileSystem = parsingName;
            else
                PathSpecialItemId = parsingName;

            ItemType = DirectoryItemFlags.Unknown;

            PathType = pathType;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected BrowseItemFromPath()
        {
            ChildIdList = null;
            ParentIdList = null;
        }
        #endregion constructors

        #region props
        public bool IsSpecialFolder { get; }

        /// <summary>
        /// The name of a folder that should be used for display
        /// (e.g. 'Windows' for 'c:\Windows' or Documents for '::{...}')
        /// </summary>
        public string Name { get; protected set; }

        public string ParseName { get; }

        public string LabelName { get; }

        /// <summary>
        /// Contains the special path (Guid) Id for a special known folder
        /// or null if this item is not special and cannot be identified
        /// through a seperate GUID.
        /// </summary>
        public string PathSpecialItemId { get; }

        /// <summary>
        /// Contains the file system path of this item or null if this
        /// item has no file system representation.
        /// </summary>
        public string PathFileSystem { get; }

        /// <summary>
        /// Gets the IdList (if available) that describes the parent
        /// shell item of this item. This property can be null if this
        /// shell item does not have a parent (is the Desktop).
        /// </summary>
        public IdList ParentIdList { get; }

        /// <summary>
        /// Gets the IdList (if available) that describes this item
        /// underneath the parent item.
        /// 
        /// This property cannot be null. The <see cref="ParentIdList"/>
        /// and <see cref="ChildIdList"/> must be processed together
        /// since this item is otherwise no fully described just using
        /// one of either properties.
        /// </summary>
        public IdList ChildIdList { get; }

        /// <summary>
        /// Gets an optional pointer to the default icon resource used when the folder is created.
        /// This is a null-terminated Unicode string in this form:
        ///
        /// Module name, Resource ID
        /// or null is this information is not available.
        /// </summary>
        public string IconResourceId { get; protected set; }

        //// <summary>
        //// Gets the folders type classification.
        //// </summary>
        public DirectoryItemFlags ItemType { get; protected set; }

        /// <summary>
        /// Gets a type of path handler that indicates the
        /// handling object that should be used to manipulate
        /// this item and its children.
        /// </summary>
        public PathHandler PathType { get; }

        /// <summary>
        /// The raw path string that was originally passed in (for debugging only).
        /// </summary>
        public string Path_RAW { get; }
        #endregion props

        #region methods
        /// <summary>
        /// Initializes the items type flags and path properties.
        /// </summary>
        /// <param name="path">Is either a path reference a la 'C:' or a
        /// special folder path reference a la '::{...}' <seealso cref="KF_IID"/>
        /// for more details.</param>
        /// <param name="bFindKF">Determines if known folder should be looked up
        /// even if given folder is a normal string such as (eg.: 'C:\Windows\').
        /// Set this parameter only if you are sure that you need it as it will
        /// have a performance impact on the time required to generate the object.
        /// </param>
        /// <returns>Returns a simple pojo type object to initialize
        /// the calling object members.</returns>
        internal static BrowseItemFromPath InitItemType(string path,
                                                        bool bFindKF = false)
        {
            if (string.IsNullOrEmpty(path) == true)   // return unknown references
            {
                var ret = new BrowseItemFromPath(path, PathHandler.InvalidPath);
                ret.Name = path;
                return ret;
            }

            // Return item for root desktop item
            if (string.Compare(path, KF_IID.ID_ROOT_Desktop,true) == 0)
            {
                var ret = InitDesktopRootItem();

                ret.ItemType |= DirectoryItemFlags.Special |
                                DirectoryItemFlags.DesktopRoot;

                return ret;
            }

            ShellHelpers.SpecialPath isSpecialID = ShellHelpers.IsSpecialPath(path);
            string normPath = null, SpecialPathId = null;
            bool hasPIDL = false;
            IdList parentIdList, relativeChildIdList;

            if (isSpecialID != ShellHelpers.SpecialPath.IsSpecialPath)
            {
                normPath = Browser.NormalizePath(path);
                hasPIDL = PidlManager.GetParentIdListFromPath(normPath, out parentIdList, out relativeChildIdList);
            }
            else
            {
                SpecialPathId = path;
                hasPIDL = PidlManager.GetParentIdListFromPath(path, out parentIdList, out relativeChildIdList);
            }

            if (hasPIDL == false)   // return references that cannot resolve with a PIDL
            {
                var ret = new BrowseItemFromPath(path, PathHandler.InvalidPath);
                ret.Name = path;
                return ret;
            }

            string parseName = normPath;
            string name = normPath;
            string labelName = null;
            IdList fullIdList = null;

            // Get the IShellFolder2 Interface for the original path item...
            IntPtr fullPidlPtr = default(IntPtr);
            IntPtr ptrShellFolder = default(IntPtr);
            IntPtr parentPIDLPtr = default(IntPtr);
            IntPtr relativeChildPIDLPtr = default(IntPtr);
            try
            {
                // We are asked to build the desktop root item here...
                if (parentIdList == null && relativeChildIdList == null)
                {
                    using (var shellFolder = new ShellFolderDesktop())
                    {
                        parseName = shellFolder.GetShellFolderName(fullPidlPtr, SHGDNF.SHGDN_FORPARSING);
                        name = shellFolder.GetShellFolderName(fullPidlPtr, SHGDNF.SHGDN_INFOLDER | SHGDNF.SHGDN_FOREDITING);
                        labelName = shellFolder.GetShellFolderName(fullPidlPtr, SHGDNF.SHGDN_NORMAL);
                    }
                }
                else
                {
                    fullIdList = PidlManager.CombineParentChild(parentIdList, relativeChildIdList);
                    fullPidlPtr = PidlManager.IdListToPidl(fullIdList);

                    if (fullPidlPtr == default(IntPtr))
                        return null;

                    HRESULT hr = HRESULT.False;

                    if (fullIdList.Size == 1) // Is this item directly under the desktop root?
                    {
                        hr = NativeMethods.SHGetDesktopFolder(out ptrShellFolder);

                        if (hr != HRESULT.S_OK)
                            return null;

                        using (var shellFolder = new ShellFolder(ptrShellFolder))
                        {
                            parseName = shellFolder.GetShellFolderName(fullPidlPtr, SHGDNF.SHGDN_FORPARSING);
                            name = shellFolder.GetShellFolderName(fullPidlPtr, SHGDNF.SHGDN_INFOLDER | SHGDNF.SHGDN_FOREDITING);
                            labelName = shellFolder.GetShellFolderName(fullPidlPtr, SHGDNF.SHGDN_NORMAL);
                        }
                    }
                    else
                    {
                        parentPIDLPtr = PidlManager.IdListToPidl(parentIdList);
                        relativeChildPIDLPtr = PidlManager.IdListToPidl(relativeChildIdList);

                        using (var desktopFolder = new ShellFolderDesktop())
                        {
                            hr = desktopFolder.Obj.BindToObject(parentPIDLPtr, IntPtr.Zero,
                                                                typeof(IShellFolder2).GUID, out ptrShellFolder);
                        }

                        if (hr != HRESULT.S_OK)
                            return null;

                        // This item is not directly under the Desktop root
                        using (var shellFolder = new ShellFolder(ptrShellFolder))
                        {
                            parseName = shellFolder.GetShellFolderName(relativeChildPIDLPtr, SHGDNF.SHGDN_FORPARSING);
                            name = shellFolder.GetShellFolderName(relativeChildPIDLPtr, SHGDNF.SHGDN_INFOLDER | SHGDNF.SHGDN_FOREDITING);
                            labelName = shellFolder.GetShellFolderName(relativeChildPIDLPtr, SHGDNF.SHGDN_NORMAL);
                        }
                    }
                }

                if (ShellHelpers.IsSpecialPath(parseName) == ShellHelpers.SpecialPath.None)
                    normPath = parseName;

                return InitItemType(path, normPath, parseName, name, labelName, SpecialPathId,
                                    parentIdList, relativeChildIdList, fullIdList, bFindKF);
            }
            finally
            {
                PidlManager.ILFree(parentPIDLPtr);
                PidlManager.ILFree(relativeChildPIDLPtr);

                if (fullPidlPtr != default(IntPtr))
                    NativeMethods.ILFree(fullPidlPtr);
            }
        }

        /// <summary>
        /// Class constructor from strings that are commonly exposed by
        /// <see cref="IShellFolder2"/> interfaces. Constructing from these
        /// items can speed up enumeration since we do not need to revisit
        /// each items <see cref="IShellFolder2"/> interfaces.
        /// </summary>
        /// <param name="parseName"></param>
        /// <param name="name"></param>
        /// <param name="labelName"></param>
        /// <param name="bFindKF">Determines if known folder should be looked up
        /// even if given folder is a normal string such as (eg.: 'C:\Windows\').
        /// Set this parameter only if you are sure that you need it as it will
        /// have a performance impact on the time required to generate the object.
        /// </param>
        /// <returns></returns>
        internal static BrowseItemFromPath InitItemType(string parseName,
                                                        string name,
                                                        string labelName,
                                                        bool bFindKF = false)
        {
            bool hasPIDL = false;
            IdList parentIdList = null;
            IdList relativeChildIdList = null;

            string path = parseName;
            string normPath = null, SpecialPathId = null;

            ShellHelpers.SpecialPath isSpecialID = ShellHelpers.IsSpecialPath(path);
            if (isSpecialID == ShellHelpers.SpecialPath.None)
                normPath = parseName;
            else
            {
                SpecialPathId = path;
            }

            hasPIDL = PidlManager.GetParentIdListFromPath(path, out parentIdList, out relativeChildIdList);

            if (hasPIDL == false)   // return references that cannot resolve with a PIDL
            {
                var ret = new BrowseItemFromPath(path, PathHandler.InvalidPath);
                ret.Name = path;
                return ret;
            }

            IdList fullIdList = null;

            // Get the IShellFolder2 Interface for the original path item...
            // We are asked to build the desktop root item here...
            if ((parentIdList == null && relativeChildIdList == null) == false)
                fullIdList = PidlManager.CombineParentChild(parentIdList, relativeChildIdList);

            return InitItemType(path, normPath, parseName, name, labelName, SpecialPathId,
                                parentIdList, relativeChildIdList, fullIdList, bFindKF);
        }

        private static BrowseItemFromPath InitItemType(string path, string normPath,
                                                       string parseName,
                                                       string name,
                                                       string labelName, string SpecialPathId,
                                                       IdList parentIdList,
                                                       IdList relativeChildIdList,
                                                       IdList fullIdList = null,
                                                       bool bFindKF = false
                                                )
        {
            PathHandler pathType = PathHandler.Unknown;
            DirectoryItemFlags itemType = DirectoryItemFlags.Unknown;

            ShellHelpers.SpecialPath specialPath = ShellHelpers.SpecialPath.None;
            if (string.IsNullOrEmpty(normPath) == false)
            {
                specialPath = ShellHelpers.IsSpecialPath(normPath);

                var pathIsTypeOf = Browser.IsTypeOf(normPath);

                if (pathIsTypeOf == Enums.PathType.FileSystemPath)
                {
                    // TODO XXX Always evaluate on NormPath???
                    try
                    {
                        bool pathExists = false;
                        try
                        {
                            pathExists = System.IO.File.Exists(normPath);
                        }
                        catch { }

                        if (pathExists && normPath.EndsWith(".zip"))
                        {
                            pathType = PathHandler.FileSystem;
                            itemType |= DirectoryItemFlags.DataFileContainer;
                        }
                    }
                    catch { }

                    // See if this is a directory if it was not a file...
                    if ((itemType & DirectoryItemFlags.DataFileContainer) == 0)
                    {
                        // Does this directory exist in file system ?
                        try
                        {
                            bool pathExists = false;
                            try
                            {
                                pathExists = System.IO.Directory.Exists(parseName);
                            }
                            catch { }

                            if (pathExists == true)
                            {
                                pathType = PathHandler.FileSystem;
                                itemType |= DirectoryItemFlags.FileSystemDirectory;

                                // This appears to be a reference to a drive
                                DirectoryInfo d = new DirectoryInfo(parseName);
                                if (d.Parent == null)
                                    itemType |= DirectoryItemFlags.Drive;
                            }
                            else
                            {
                                // Neither a regular directory nor a regular file
                                // -> Most likely a folder inside a zip file data container
                                if (string.IsNullOrEmpty(normPath) == false)
                                {
                                    if (normPath.Contains(".zip"))
                                    {
                                        pathType = PathHandler.ZipFileSystem;
                                        itemType |= DirectoryItemFlags.DataFileContainerFolder;
                                    }

                                    // -> Lets get its name for display if its more than empty
                                    string displayName = System.IO.Path.GetFileName(normPath);
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            Debug.WriteLine(exp.Message);
                        }
                    }
                }
            }

            // Attempt to find known folder for this item
            IKnownFolderProperties props = null;
            bool isSpecialFolder = false;
            string pathSpecialItemId = null;

            if (fullIdList == null)
            {
                if ((parentIdList == null && relativeChildIdList == null) == false)
                    PidlManager.CombineParentChild(parentIdList, relativeChildIdList);
            }

            if (SpecialPathId != null || bFindKF == true)
            {
                GetKnownFolder(fullIdList, ref pathType,
                                           ref itemType,
                                           ref props,
                                           ref isSpecialFolder,
                                           ref pathSpecialItemId);
            }

            // Initialize a normal file system path item or special folder item
            return new BrowseItemFromPath(path, normPath, isSpecialFolder,
                                          parseName, name, labelName, pathSpecialItemId,
                                          parentIdList, relativeChildIdList,
                                          pathType, itemType,
                                          props);
        }

        private static bool GetKnownFolder(IdList fullIdList,
                                           ref PathHandler pathType,
                                           ref DirectoryItemFlags itemType,
                                           ref IKnownFolderProperties props,
                                           ref bool isSpecialFolder,
                                           ref string pathSpecialItemId)
        {
            using (var knownFolder = KnownFolderHelper.FromPIDL(fullIdList))
            {
                if (knownFolder != null) // Initialize a known folder item
                {
                    isSpecialFolder = true;

                    if (fullIdList == null)
                        pathType = PathHandler.DesktopRoot;

                    itemType |= DirectoryItemFlags.Special;

                    props = KnownFolderHelper.GetFolderProperties(knownFolder.Obj);

                }
            }

            if (props != null) // Initialize a known folder item
            {
                pathSpecialItemId = string.Format("{0}{1}{2}{3}", KF_IID.IID_Prefix,
                                                "{", props.FolderId, "}");

                if (props.IsIconResourceIdValid() == false)
                {
                    string filename = null;
                    int index = -1;
                    if (IconHelper.GetIconResourceId(fullIdList, out filename, out index))
                    {
                        // Store filename and index for Desktop Root ResourceId
                        props.ResetIconResourceId(filename, index);
                    }
                }

                if (string.Compare(pathSpecialItemId, KF_IID.ID_FOLDERID_ComputerFolder, true) == 0)
                {
                    pathType = PathHandler.FileSystem;
                }

                if (props.Category == FolderCategory.Virtual)
                    itemType |= DirectoryItemFlags.Virtual;

                // Check for very common known special directory reference
                if (KF_IID.ID_FOLDERID_Desktop.Equals(pathSpecialItemId, StringComparison.InvariantCultureIgnoreCase))
                    itemType |= DirectoryItemFlags.Desktop;
                else
                if (KF_IID.ID_FOLDERID_Documents.Equals(pathSpecialItemId, StringComparison.InvariantCultureIgnoreCase))
                    itemType |= DirectoryItemFlags.Documents;
                else
                if (KF_IID.ID_FOLDERID_Downloads.Equals(pathSpecialItemId, StringComparison.InvariantCultureIgnoreCase))
                    itemType |= DirectoryItemFlags.Downloads;
                else
                if (KF_IID.ID_FOLDERID_Music.Equals(pathSpecialItemId, StringComparison.InvariantCultureIgnoreCase))
                    itemType |= DirectoryItemFlags.Music;
                else
                if (KF_IID.ID_FOLDERID_Pictures.Equals(pathSpecialItemId, StringComparison.InvariantCultureIgnoreCase))
                    itemType |= DirectoryItemFlags.Pictures;
                else
                if (KF_IID.ID_FOLDERID_Videos.Equals(pathSpecialItemId, StringComparison.InvariantCultureIgnoreCase))
                    itemType |= DirectoryItemFlags.Videos;

                return true;
            }

            return false;
        }

        internal static BrowseItemFromPath InitDesktopRootItem()
        {
            string root = KF_IID.ID_ROOT_Desktop;
            BrowseItemFromPath ret = new BrowseItemFromPath(root, root, PathHandler.DesktopRoot);

            // Use normal desktop special folder as template for naming and properties
            string specialPath = KF_IID.ID_FOLDERID_Desktop;
            IKnownFolderProperties props = null;
            using (var kf = KnownFolderHelper.FromPath(specialPath))
            {
                if (kf == null)
                    return null;

                IdList FullPidl = kf.KnownFolderToIdList();
                props = KnownFolderHelper.GetFolderProperties(kf.Obj);
            }

            // A directory we cannot find in file system is by definition VIRTUAL
            if (props.Category == FolderCategory.Virtual)
                ret.ItemType |= DirectoryItemFlags.Virtual;

            ret.Name = props.Name;

            string filename = null; // Get Resoure Id for desktop root item
            int index = -1;
            if (props.IsIconResourceIdValid() == false)
            {
                if (IconHelper.GetIconResourceId(IdList.Create(), out filename, out index))
                {
                    // Store filename and index for Desktop Root ResourceId
                    ret.IconResourceId = string.Format("{0},{1}", filename, index);
                }
            }
            else
            {
                ret.IconResourceId = props.IconResourceId;
            }

            return ret;
        }

        /// <summary>
        /// Method gets the display name from a file system path.
        /// </summary>
        /// <param name="path">Is either a path reference a la 'C:' or a
        /// special folder path reference a la '::{...}'</param>
        /// <returns>Returns the recommended display name for this item</returns>
        internal static string GetDisplayNameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (ShellHelpers.IsSpecialPath(path) == ShellHelpers.SpecialPath.IsSpecialPath)
            {
                // Get the KnownFolderId Guid for this special folder
                var kf_guid = new Guid(path.Substring(KF_IID.IID_Prefix.Length));

                IntPtr pidlKF_Path = default(IntPtr);
                IntPtr pShellPtr = default(IntPtr);
                IShellItem2 ppv = null;
                try
                {
                    NativeMethods.SHGetKnownFolderIDList(kf_guid, KNOWN_FOLDER_FLAG.KF_NO_FLAGS,
                                                         IntPtr.Zero, out pidlKF_Path);

                    if (pidlKF_Path != default(IntPtr))
                    {
                        HRESULT hr = NativeMethods.SHCreateItemFromIDList(pidlKF_Path, typeof(IShellItem2).GUID, out pShellPtr);

                        if (hr == HRESULT.S_OK)
                            ppv = (IShellItem2)Marshal.GetTypedObjectForIUnknown(pShellPtr, typeof(IShellItem2));

                        if (hr == HRESULT.S_OK && ppv != null)
                        {
                            using (var shellItem = new ShellItem2(ppv))      // Will dispose IShellItem
                            {
                                IntPtr ppszName = default(IntPtr);
                                try
                                {
                                    if (shellItem.Obj.GetDisplayName(SIGDN.NORMALDISPLAY, out ppszName) == HRESULT.S_OK)
                                        return Marshal.PtrToStringUni(ppszName);            // Get display name
                                }
                                finally
                                {
                                    if (ppszName != default(IntPtr))
                                        Marshal.FreeCoTaskMem(ppszName);
                                }
                            }
                        }
                    }

                }
                finally
                {
                    if (pShellPtr != default(IntPtr))
                        Marshal.Release(pShellPtr);

                    if (ppv != null)
                        Marshal.ReleaseComObject(ppv);

                    pidlKF_Path = PidlManager.ILFree(pidlKF_Path);
                }
            }

            IntPtr pidlPath = default(IntPtr);
            IntPtr pShellPtr1 = default(IntPtr);
            IShellItem2 ppv1 = null;
            try
            {
                pidlPath = PidlManager.GetPIDLFromPath(path);

                if (pidlPath != default(IntPtr))
                {
                    HRESULT hr = NativeMethods.SHCreateItemFromIDList(pidlPath, typeof(IShellItem2).GUID,
                                                                      out pShellPtr1);

                    if (hr == HRESULT.S_OK)
                        ppv1 = (IShellItem2)Marshal.GetTypedObjectForIUnknown(pShellPtr1, typeof(IShellItem2));

                    if (hr == HRESULT.S_OK && ppv1 != null)
                    {
                        using (var shellItem = new ShellItem2(ppv1))      // Will dispose IShellItem
                        {
                            IntPtr ppszName = default(IntPtr);
                            try
                            {
                                if (shellItem.Obj.GetDisplayName(SIGDN.NORMALDISPLAY, out ppszName) == HRESULT.S_OK)
                                    return Marshal.PtrToStringUni(ppszName);  // 2nd Option to get display name
                            }
                            finally
                            {
                                if (ppszName != default(IntPtr))
                                    Marshal.FreeCoTaskMem(ppszName);
                            }
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (pShellPtr1 != default(IntPtr))
                    Marshal.Release(pShellPtr1);

                if (ppv1 != null)
                    Marshal.ReleaseComObject(ppv1);

                pidlPath = PidlManager.ILFree(pidlPath);
            }

            return null;
        }
        #endregion methods
    }
}
