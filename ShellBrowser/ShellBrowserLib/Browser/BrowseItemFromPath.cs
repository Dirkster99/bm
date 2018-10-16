﻿namespace ShellBrowserLib.Browser
{
    using ShellBrowserLib.Enums;
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.SharpShell.Interop.Dlls;
    using ShellBrowserLib.SharpShell.Interop.Interfaces.Knownfolders;
    using ShellBrowserLib.SharpShell.Interop.Interfaces.KnownFolders;
    using ShellBrowserLib.SharpShell.Interop.Interfaces.ShellFolders;
    using ShellBrowserLib.SharpShell.Interop.Knownfolders;
    using ShellBrowserLib.SharpShell.Interop.ShellFolders;
    using ShellBrowserLib.SharpShell.Pidl;
    using ShellBrowserLib.Shell.Enums;
    using ShellBrowserLib.Shell.Interop.Interfaces;
    using ShellBrowserLib.Shell.Interop.Interfaces.ShellItems;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Implements a class that contains all API call and methods necessary to initialize
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

            this.PathFileSystem = (string.IsNullOrEmpty(parseName) ? normPath : parseName);

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
            ParentIdList = ChildIdList = null;
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
        /// <returns>Returns a simple pojo type object to initialize
        /// the calling object members.</returns>
        internal static BrowseItemFromPath InitItemType(string path)
        {
            if (string.IsNullOrEmpty(path) == true)   // return unknown references
            {
                var ret = new BrowseItemFromPath(path, PathHandler.InvalidPath);
                ret.Name = path;
                return ret;
            }

            if (path == KF_IID.ID_ROOT_Desktop)  // Return item for root desktop item
            {
                var ret = InitDesktopRootItem();

                ret.ItemType |= DirectoryItemFlags.Special |
                                DirectoryItemFlags.DesktopRoot;

                return ret;
            }

            ShellHelpers.SpecialPath isSpecialID = ShellHelpers.IsSpecialPath(path);
            string normPath = null;
            bool hasPIDL = false;
            IdList parentIdList, relativeChildIdList;

            if (isSpecialID != ShellHelpers.SpecialPath.IsSpecialPath)
            {
                normPath = DirectoryBrowser.NormalizePath(path);
                hasPIDL = PidlManager.GetParentIdListFromPath(normPath, out parentIdList, out relativeChildIdList);
            }
            else
                hasPIDL = PidlManager.GetParentIdListFromPath(path, out parentIdList, out relativeChildIdList);

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

                return InitItemType(path, normPath, parseName, name, labelName,
                                    parentIdList, relativeChildIdList, fullIdList);
            }
            finally
            {
                PidlManager.ILFree(parentPIDLPtr);
                PidlManager.ILFree(relativeChildPIDLPtr);

                if (fullPidlPtr != default(IntPtr))
                    NativeMethods.ILFree(fullPidlPtr);
            }
        }

        internal static BrowseItemFromPath InitItemType(string parseName,
                                                        string name,
                                                        string labelName)
        {
            bool hasPIDL = false;
            IdList parentIdList = null;
            IdList relativeChildIdList = null;

            string path = parseName;
            string normPath = null;

            ShellHelpers.SpecialPath isSpecialID = ShellHelpers.IsSpecialPath(path);
            if (isSpecialID == ShellHelpers.SpecialPath.None)
                normPath = parseName;

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

            return InitItemType(path, normPath, parseName, name, labelName,
                                parentIdList, relativeChildIdList, fullIdList);
        }

        internal static BrowseItemFromPath InitItemType(string path, string normPath,
                                                        string parseName,
                                                        string name,
                                                        string labelName,
                                                        IdList parentIdList,
                                                        IdList relativeChildIdList,
                                                        IdList fullIdList = null
                                                )
        {
            PathHandler pathType = PathHandler.Unknown;
            DirectoryItemFlags itemType = DirectoryItemFlags.Unknown;

            if (ShellHelpers.IsSpecialPath(parseName) == ShellHelpers.SpecialPath.IsSpecialPath)
                parseName = null;
            else
            {
                try
                {
                    bool pathExists = false;
                    try
                    {
                        pathExists = System.IO.File.Exists(parseName);
                    }
                    catch { }

                    if (pathExists && parseName.EndsWith(".zip"))
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

            // Attempt to find known folder for this item
            IKnownFolderProperties props = null;
            bool isSpecialFolder = false;
            string pathSpecialItemId = null;

            if (fullIdList == null)
            {
                if ((parentIdList == null && relativeChildIdList == null) == false)
                    PidlManager.CombineParentChild(parentIdList, relativeChildIdList);
            }

            GetKnownFolder(fullIdList, ref pathType,
                                       ref itemType,
                                       ref props,
                                       ref isSpecialFolder,
                                       ref pathSpecialItemId);

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
            var knownFolder = KnownFolderHelper.FromPIDL(fullIdList);
            try
            {
                if (knownFolder != null) // Initialize a known folder item
                {
                    isSpecialFolder = true;

                    if (fullIdList == null)
                        pathType = PathHandler.DesktopRoot;

                    itemType |= DirectoryItemFlags.Special;

                    props = KnownFolderHelper.GetFolderProperties(knownFolder.Obj);

                    pathSpecialItemId = string.Format("{0}{1}{2}{3}", KF_IID.IID_Prefix,
                                                    "{", props.FolderId, "}");

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
            }
            finally
            {
                if (knownFolder != null)
                    knownFolder.Dispose();
            }

            return false;
        }

        internal static BrowseItemFromPath InitDesktopRootItem()
        {
            string root = KF_IID.ID_ROOT_Desktop;
            BrowseItemFromPath ret = new BrowseItemFromPath(root, root, PathHandler.DesktopRoot);

            // Use normal desktop special folder as template for naming and properties
            string specialPath = KF_IID.ID_FOLDERID_Desktop;
            using (var kf = KnownFolderHelper.FromPath(specialPath))
            {
                if (kf == null)
                    return null;

                IdList FullPidl = kf.KnownFolderToIdList();

                var props = KnownFolderHelper.GetFolderProperties(kf.Obj);

                // A directory we cannot find in file system is by definition VIRTUAL
                if (props.Category == FolderCategory.Virtual)
                    ret.ItemType |= DirectoryItemFlags.Virtual;

                ret.Name = props.Name;

                string filename = null; // Get Resoure Id for desktop root item
                int index = -1;
                if (GetIconResourceId(IdList.Create(), out filename, out index))
                {
                    // Store filename and index for Desktop Root ResourceId
                    ret.IconResourceId = string.Format("{0},{1}", filename, index);
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets an icons reource id if available in the format:
        /// "filename, index"
        /// where the first par tis a string and the 2nd part is a negativ integer number).
        /// 
        /// This format is usally used by the Windows Shell libraries so we use it here
        /// to add missing ResourceIds.
        /// </summary>
        /// <param name="parentIdList"></param>
        /// <param name="filename"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static bool GetIconResourceId(IdList parentIdList,
                                               out string filename,
                                               out int index)
        {
            filename = null;
            index = -1;

            IntPtr parentPtr = default(IntPtr);
            IntPtr relChildPtr = default(IntPtr);
            IntPtr ptrShellFolder = default(IntPtr);
            IntPtr ptrExtractIcon = default(IntPtr);
            IntPtr smallHicon = default(IntPtr);
            IntPtr largeHicon = default(IntPtr);
            try
            {
                parentPtr = PidlManager.IdListToPidl(parentIdList);
                relChildPtr = PidlManager.IdListToPidl(IdList.Create());

                if (parentPtr == default(IntPtr) || relChildPtr == default(IntPtr))
                    return false;

                Guid guid = typeof(IShellFolder2).GUID;
                HRESULT hr = NativeMethods.SHBindToParent(parentPtr, guid,
                                                    out ptrShellFolder, ref relChildPtr);

                if (hr != HRESULT.S_OK)
                    return false;

                using (var shellFolder = new ShellFolder(ptrShellFolder))
                {
                    if (shellFolder == null)
                        return false;

                    guid = typeof(IExtractIcon).GUID;
                    var pidls = new IntPtr[] { relChildPtr };
                    hr = shellFolder.Obj.GetUIObjectOf(IntPtr.Zero, 1, pidls, guid,
                                                        IntPtr.Zero, out ptrExtractIcon);

                    if (hr != HRESULT.S_OK)
                        return false;

                    using (var extractIcon = new GenericCOMFolder<IExtractIcon>(ptrExtractIcon))
                    {
                        if (extractIcon == null)
                            return false;

                        var iconFile = new StringBuilder(NativeMethods.MAX_PATH);
                        uint pwFlags = 0;

                        hr = extractIcon.Obj.GetIconLocation(0, iconFile,
                                                            (uint)iconFile.Capacity,
                                                            ref index, ref pwFlags);

                        if (hr != HRESULT.S_OK)
                            return false;

                        if (string.IsNullOrEmpty(iconFile.ToString()))
                            return false;

                        filename = iconFile.ToString();

                        return true;
                    }
                }
            }
            finally
            {
                if (parentPtr != default(IntPtr))
                    NativeMethods.ILFree(parentPtr);

                ////if (relChildPtr != default(IntPtr))
                ////    Shell32.ILFree(relChildPtr);

                if (smallHicon != default(IntPtr))
                    NativeMethods.DestroyIcon(smallHicon);

                if (largeHicon != default(IntPtr))
                    NativeMethods.DestroyIcon(largeHicon);
            }
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