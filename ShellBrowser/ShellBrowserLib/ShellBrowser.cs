namespace ShellBrowserLib
{
    using ShellBrowserLib.Browser;
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.Interfaces;
    using ShellBrowserLib.Shell.Interop;
    using ShellBrowserLib.Shell.Interop.Dlls;
    using ShellBrowserLib.Shell.Interop.Interfaces.ShellFolders;
    using ShellBrowserLib.Shell.Interop.Knownfolders;
    using ShellBrowserLib.Shell.Interop.ShellFolders;
    using ShellBrowserLib.Shell.Pidl;
    using ShellBrowserLib.Shell.Enums;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using ShellBrowserLib.Enums;
    using System.Linq;

    /// <summary>
    /// Implements core API type methods and properties that are used to interact
    /// with Windows Shell Items (folders and known folders).
    /// </summary>
    public static class ShellBrowser
    {
        #region properties
        /// <summary>
        /// Gets the default system drive - usually 'C:\'.
        /// </summary>
        /// <returns></returns>
        public static IDirectoryBrowser SysDefault
        {
            get
            {
                try
                {
                    var drive = new DirectoryInfo(Environment.SystemDirectory).Root.Name;
                    return Create(drive);
                }
                catch
                {
                    return Create(@"C:\");
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="IDirectoryBrowser"/> interface
        /// for a user's desktop folder.
        /// </summary>
        public static IDirectoryBrowser DesktopDirectory
        {
            get
            {
                return Create(KF_IID.ID_FOLDERID_Desktop);
            }
        }

        /// <summary>
        /// Gets the <see cref="IDirectoryBrowser"/> interface
        /// for a current system user's directory and known folder item.
        /// </summary>
        public static IDirectoryBrowser CurrentUserDirectory
        {
            get
            {
                return Create(KF_IID.ID_FOLDERID_Profile);
            }
        }

        /// <summary>
        /// Gets the interface for a user's MyComputer (virtual folder).
        /// </summary>
        public static IDirectoryBrowser MyComputer
        {
            get
            {
                return ShellBrowser.Create(KF_IID.ID_FOLDERID_ComputerFolder);
            }
        }

        /// <summary>
        /// Gets the <see cref="IDirectoryBrowser"/> interface
        /// for the Public Documents folder (%PUBLIC%\Documents).
        /// </summary>
        public static IDirectoryBrowser PublicDocuments
        {
            get
            {
                try
                {
                    return Create(KF_IID.ID_FOLDERID_PublicDocuments);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the interface for the Network
        /// (virtual folder, Legacy: My Network Places).
        /// </summary>
        public static IDirectoryBrowser Network
        {
            get
            {
                return Create(KF_IID.ID_FOLDERID_NetworkFolder);
            }
        }

        /// <summary>
        /// Gets the interface
        /// for the Rycycle Bin (virtual folder).
        /// </summary>
        public static IDirectoryBrowser RecycleBin
        {
            get
            {
                return Create(KF_IID.ID_FOLDERID_RecycleBinFolder);
            }
        }
        #endregion properties

        #region methods
        /// <summary>Creates a new object that implements the
        /// <see cref="IDirectoryBrowser"/> interface. The created
        /// instance is based on the given parameter, which can be
        /// a path based string (e.g: 'c:\') or a special folder
        /// based string (e.g: '::{...}').
        /// 
        /// The <see cref="IDirectoryBrowser"/> object is always created
        /// and returned unless the given string parameter is null or empty
        /// which results in an <see cref="System.ArgumentNullException"/>
        /// Exception being thrown.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static IDirectoryBrowser Create(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath) == true)
                throw new System.ArgumentNullException("path cannot be null or empty");

            try
            {
                var itemModel = BrowseItemFromPath.InitItemType(fullPath);

                return new DirectoryBrowser(itemModel);
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Exception in Create method '{0}' on '{1}'", exc.Message, fullPath);

                return null;
            }
        }

        /// <summary>Creates a new object that implements the
        /// <see cref="IDirectoryBrowser"/> interface from a
        /// <paramref name="parseName"/>, <paramref name="name"/>,
        /// and <paramref name="labelName"/>.
        /// 
        /// This method is a short-cut to by-pass additional Windows Shell API
        /// queries for this items. An enumeration of Windows Shell items, for
        /// instance, can already result into all three items, so we by-pass
        /// the IShellFolder query stuff to speed up processing in this situation.
        /// 
        /// The created instance is based on the given <paramref name="parseName"/>
        /// parameter, which can be a path based string (e.g: 'c:\') or
        /// a special folder based string (e.g: '::{...}').
        /// 
        /// The <see cref="IDirectoryBrowser"/> object is always created
        /// and returned unless the given <paramref name="parseName"/>
        /// parameter is null or empty which results in an <see cref="System.ArgumentNullException"/>
        /// Exception being thrown.
        /// </summary>
        /// <param name="parseName"></param>
        /// <param name="labelName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static IDirectoryBrowser Create(string parseName,
                                               string name,
                                               string labelName)
        {
            if (string.IsNullOrEmpty(parseName) == true)
                throw new System.ArgumentNullException("path cannot be null or empty");

            try
            {
                var itemModel = BrowseItemFromPath.InitItemType(parseName, name, labelName);

                return new DirectoryBrowser(itemModel);
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Exception in Create method '{0}' on '{1}'", exc.Message, parseName);

                return null;
            }
        }

        /// <summary>Creates a new object that implements the
        /// <see cref="IDirectoryBrowser"/> interface from a
        /// <paramref name="fullPidl"/> that represents an item in the browsing structure.
        /// 
        /// Returns null if a parseName could not be determined.
        /// </summary>
        /// <param name="fullPidl"></param>
        /// <returns></returns>
        public static IDirectoryBrowser Create(IdList fullPidl)
        {
            if (fullPidl == null)
                return Create(KF_IID.ID_FOLDERID_Desktop);

            if (fullPidl.Size == 0)
                return Create(KF_IID.ID_FOLDERID_Desktop);

            string parseName = PidlManager.GetPathFromPIDL(fullPidl);

            if (string.IsNullOrEmpty(parseName) == true)
                return null;

            return Create(parseName);
        }

        /// <summary>
        /// Gets an enumeration of all childitems below the
        /// <paramref name="folderParseName"/> item.
        /// </summary>
        /// <param name="folderParseName">Is the parse name that should be used to emit child items for.</param>
        /// <param name="searchName">Optional name of an item that should be filtered
        /// in case insensitive fashion when searching for a certain child rather than all children.</param>
        /// <returns>returns each item as <see cref="IDirectoryBrowser"/> object</returns>
        public static IEnumerable<IDirectoryBrowser> GetChildItems(string folderParseName,
                                                                   string searchName = null)
        {
            if (string.IsNullOrEmpty(folderParseName) == true)
                yield break;

            // Defines the type of items that we want to retieve below the item passed in
            const SHCONTF flags = SHCONTF.FOLDERS | SHCONTF.INCLUDEHIDDEN | SHCONTF.FASTITEMS;

            //  Get the desktop root folder.
            IntPtr enumerator = default(IntPtr);
            IntPtr pidlFull = default(IntPtr);
            IntPtr ptrFolder = default(IntPtr);
            IShellFolder2 iFolder = null;
            IEnumIDList enumIDs = null;
            IntPtr ptrStr = default(IntPtr);      // Fetch parse name for this item
            try
            {
                HRESULT hr;

                if (KF_IID.ID_FOLDERID_Desktop.Equals(folderParseName, StringComparison.InvariantCultureIgnoreCase))
                    hr = NativeMethods.SHGetDesktopFolder(out ptrFolder);
                else
                {
                    pidlFull = ShellHelpers.PidlFromParsingName(folderParseName);

                    if (pidlFull == default(IntPtr)) // 2nd chance try known folders
                    {
                        using (var kf = KnownFolderHelper.FromPath(folderParseName))
                        {
                            if (kf != null)
                                kf.Obj.GetIDList(KNOWN_FOLDER_FLAG.KF_NO_FLAGS, out pidlFull);
                        }
                    }

                    if (pidlFull == default(IntPtr))
                        yield break;

                    using (var desktopFolder = new ShellFolderDesktop())
                    {
                        hr = desktopFolder.Obj.BindToObject(pidlFull, IntPtr.Zero,
                                                            typeof(IShellFolder2).GUID,
                                                            out ptrFolder);
                    }
                }

                if (hr != HRESULT.S_OK)
                    yield break;

                if (ptrFolder != IntPtr.Zero)
                    iFolder = (IShellFolder2)Marshal.GetTypedObjectForIUnknown(ptrFolder, typeof(IShellFolder2));

                if (iFolder == null)
                    yield break;

                //  Create an enumerator and enumerate over each item.
                hr = iFolder.EnumObjects(IntPtr.Zero, flags, out enumerator);

                if (hr != HRESULT.S_OK)
                    yield break;

                // Convert enum IntPtr to interface
                enumIDs = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(enumerator, typeof(IEnumIDList));

                if (enumIDs == null)
                    yield break;

                FilterMask filter = null;
                if (searchName != null)
                    filter = new FilterMask();

                uint fetched, count = 0;
                IntPtr apidl = default(IntPtr);

                // Allocate memory to convert parsing names into .Net strings efficiently below
                ptrStr = Marshal.AllocCoTaskMem(NativeMethods.MAX_PATH * 2 + 4);
                Marshal.WriteInt32(ptrStr, 0, 0);
                StringBuilder strbuf = new StringBuilder(NativeMethods.MAX_PATH);

                // Get one item below root item at a time and process by getting its display name
                // PITEMID_CHILD: The ITEMIDLIST is an allocated child ITEMIDLIST relative to
                // a parent folder, such as a result of IEnumIDList::Next.
                // It contains exactly one SHITEMID structure (https://docs.microsoft.com/de-de/windows/desktop/api/shtypes/ns-shtypes-_itemidlist)
                for (; enumIDs.Next(1, out apidl, out fetched) == HRESULT.S_OK; count++)
                {
                    if (fetched <= 0)  // End this loop if no more items are available
                        break;

                    try
                    {
                        string name = string.Empty;
                        if (iFolder.GetDisplayNameOf(apidl, SHGDNF.SHGDN_INFOLDER | SHGDNF.SHGDN_FOREDITING, ptrStr) == HRESULT.S_OK)
                        {
                            NativeMethods.StrRetToBuf(ptrStr, default(IntPtr),
                                                      strbuf, NativeMethods.MAX_PATH);

                            name = strbuf.ToString();
                        }

                        // Skip this item if search parameter is set and this appears to be a non-match
                        if (filter != null)
                        {
                            if (filter.MatchFileMask(name, searchName) == false)
                                continue;
                        }

                        string parseName = string.Empty;
                        if (iFolder.GetDisplayNameOf(apidl, SHGDNF.SHGDN_FORPARSING, ptrStr) == HRESULT.S_OK)
                        {
                            NativeMethods.StrRetToBuf(ptrStr, default(IntPtr),
                                                      strbuf, NativeMethods.MAX_PATH);

                            parseName = strbuf.ToString();
                        }

                        string labelName = string.Empty;
                        if (iFolder.GetDisplayNameOf(apidl, SHGDNF.SHGDN_NORMAL, ptrStr) == HRESULT.S_OK)
                        {
                            NativeMethods.StrRetToBuf(ptrStr, default(IntPtr),
                                                      strbuf, NativeMethods.MAX_PATH);

                            labelName = strbuf.ToString();
                        }

                        IdList apidlIdList = PidlManager.PidlToIdlist(apidl);

                        yield return Create(parseName, name, labelName);
                    }
                    finally
                    {
                        apidl = PidlManager.FreeCoTaskMem(apidl);
                    }
                }
            }
            finally
            {
                if (enumIDs != null)
                    Marshal.ReleaseComObject(enumIDs);

                if (enumerator != default(IntPtr))
                    Marshal.Release(enumerator);

                if (iFolder != null)
                    Marshal.ReleaseComObject(iFolder);

                if (ptrFolder != default(IntPtr))
                    Marshal.Release(ptrFolder);

                ptrStr = PidlManager.FreeCoTaskMem(ptrStr);
            }
        }

        /// <summary>
        /// Split the current folder in an array of sub-folder names and return it.
        /// </summary>
        /// <returns>Returns a string array of su-folder names (including drive) or null if there are no sub-folders.</returns>
        public static string[] GetDirectories(string folder)
        {
            if (string.IsNullOrEmpty(folder) == true)
                return null;

            folder = DirectoryBrowser.NormalizePath(folder);

            string[] dirs = null;

            try
            {
                dirs = folder.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                if (dirs != null)
                {
                    if (dirs[0].Length == 2)       // Normalizing Drive representation
                    {                             // from 'C:' to 'C:\'
                        if (dirs[0][1] == ':')   // to ensure correct processing
                            dirs[0] += '\\';    // since 'C:' is technically invalid(!)
                    }
                }
            }
            catch
            {
            }

            return dirs;
        }

        /// <summary>
        /// Determines if a directory (special or not) exists at the givem path
        /// (path can be a formatted as special path KF_IDD).
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Returns true if item has a filesystem path otherwise false.</returns>
        public static bool DirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            try
            {
                if (ShellHelpers.IsSpecialPath(path) == ShellHelpers.SpecialPath.IsSpecialPath)
                {
                    // translate KF_IID into file system path and check if it exists
                    string fs_path = KnownFolderHelper.GetKnownFolderPath(path);

                    if (fs_path != null)
                        return System.IO.Directory.Exists(fs_path);

                    return false;
                }
                else
                    return System.IO.Directory.Exists(path);
            }
            catch (System.ArgumentException)
            {
                // Is thrown by System.IO.Directory.Exists(...) if path contains illegal caharacter (eg: '<')
                return false;
            }
        }

        /// <summary>
        /// Parses the first two characters of a given path to determine its type.
        /// Paths that are shorter than 2 characters are classified <see cref="PathType.Unknown"/>.
        /// 
        /// Paths with 2 or more characters having no File Fystem or Special Folder signature
        /// are clasified as <seealso cref="PathType.WinShellPath"/>.
        /// 
        /// Returns false for strings 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static PathType IsTypeOf(string input)
        {
            if (string.IsNullOrEmpty(input))
                return PathType.Unknown;

            if (input.Length < 2)
                return PathType.Unknown;

            // Could be drive based like 'c:\Windows' or a network share like '\\MyServer\share'
            if ((char.ToLower(input[0]) >= 'a' && char.ToLower(input[0]) <= 'z' &&   // Drive based file system path
                 input[1] == ':') ||
                 (char.ToLower(input[0]) == '\\' && char.ToLower(input[1]) <= '\\'))  // UNC file system path
                return PathType.FileSystemPath;

            // Could be something like '::{Guid}' which is usually a known folder's path
            if (input[0] == ':' && input[1] == ':')
                return PathType.SpecialFolder;

            // Could be something like 'Libraries\Music'
            return PathType.WinShellPath;
        }

        /// <summary>
        /// Determines if a directory (special or not) exists at the givem path
        /// (path can be a formatted as special path KF_IDD).
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathItems"></param>
        /// <returns>Returns true if item has a filesystem path otherwise false.</returns>
        public static bool DirectoryExists(string path, out IDirectoryBrowser[] pathItems)
        {
            pathItems = null;
                
            if (string.IsNullOrEmpty(path))
                return false;

            if (ShellHelpers.IsSpecialPath(path) == ShellHelpers.SpecialPath.IsSpecialPath)
            {
                // translate KF_IID into file system path and check if it exists
                string fs_path = KnownFolderHelper.GetKnownFolderPath(path);

                if (fs_path != null)
                    return System.IO.Directory.Exists(fs_path);

                return false;
            }
            else
            {
                if (path.Length < 2)
                    return false;

                if ((path[0] == '\\' && path[1] == '\\') || path[1] == ':')
                    return System.IO.Directory.Exists(path);

                try
                {
                    if (path.Length > 1)
                        path = path.TrimEnd('\\');

                    // Try to resolve an abstract Windows Shell Space description like:
                    // 'Libraries/Documents' (in a localized fashion)
                    string[] pathNames = GetDirectories(path);

                    if (pathNames == null)
                        return false;

                    if (pathNames.Length == 0)
                        return false;

                    pathItems = new IDirectoryBrowser[pathNames.Length];

                    string parentPath = KF_IID.ID_FOLDERID_Desktop;
                    for (int i = 0; i < pathItems.Length; i++)
                    {
                        if (i > 0)
                            parentPath = pathItems[i - 1].PathShell;

                        foreach (var item in ShellBrowser.GetChildItems(parentPath))
                        {
                            if (string.Compare(item.Name, pathNames[i], true) == 0)
                            {
                                pathItems[i] = item;
                                break;
                            }
                        }
                    }

                    // This path exists as sequence of localized names of windows shell items
                    if (pathItems[pathItems.Length - 1] != null)
                        return true;
                }
                catch
                {
                    // Something went wrong so we signal that we cannot resolve this one...
                    return false;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a list of fullids that represent a path to the given <paramref name="location"/>.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static List<IdList> PathItemsAsIdList(IDirectoryBrowser location)
        {
            List<IdList> pathItems = new List<IdList>();

            // Desktop has no parents and no child to point at
            if (location.ParentIdList == null && location.ChildIdList == null)
                return pathItems;

            var fullIdList = PidlManager.CombineParentChild(location.ParentIdList,
                                                            location.ChildIdList);

            if (fullIdList.Size <= 1)
            {
                pathItems.Add(fullIdList); // Reference to 'This PC'(?) directly under desktop
                return pathItems;
            }

            IdList parentItem, childItem;
            while (PidlManager.GetParentChildIdList(fullIdList,
                                                    out parentItem, out childItem) == true)
            {
                // Reverse order since parent lookup would otherwise produce wrong order
                pathItems.Insert(0, fullIdList);

                if (fullIdList.Size <= 1)
                    break;

                fullIdList = parentItem;
            }

            return pathItems;
        }

        /// <summary>
        /// Gets a list of ParseNames that (taken together) represent a path to
        /// a given <paramref name="location"/>.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static List<string> PathItemsAsParseNames(IDirectoryBrowser location)
        {
            var pathItems = new List<string>();

            var idLists = PathItemsAsIdList(location);

            foreach (var item in idLists)
            {
                string parseName = PidlManager.IdListFullToName(item, SHGDNF.SHGDN_FORPARSING);

                if (string.IsNullOrEmpty(parseName) == false)
                    pathItems.Add(parseName);
                else
                    return pathItems;  // ParseName cannot be determined so we return what we got :-(
            }

            return pathItems;
        }

        /// <summary>
        /// Determines if the given <paramref name="childPath"/> could be mounted
        /// somewhere into the path of the <paramref name="parentPath"/> and returns
        /// true if thats the case.
        /// </summary>
        /// <param name="parentPath"></param>
        /// <param name="childPath"></param>
        /// <param name="pathExtension">Contains the part of the <paramref name="childPath"/>
        /// that could be used to extend the <paramref name="parentPath"/> in order to find
        /// an alternative path representation.</param>
        /// <returns></returns>
        public static bool IsParentPathOf(string parentPath,
                                          string childPath,
                                          out string pathExtension)
        {
            pathExtension = null;

            if (string.IsNullOrEmpty(parentPath) == true || string.IsNullOrEmpty(childPath) == true)
                return false;

            if (parentPath.Length > childPath.Length)
                return false;

            string childRoot = childPath.Substring(0, parentPath.Length);

            bool ret = string.Compare(childRoot, parentPath, true) == 0;

            if (ret == true)
            {
                pathExtension = childPath.Substring(childRoot.Length);

                if (pathExtension.Length > 0)       // Skip seperator at beginning of string
                {
                    if (pathExtension[0] == '\\')
                        pathExtension = pathExtension.Substring(1);
                }

                if (pathExtension.Length > 0)      // Skip last seperator char if present
                {
                    int idx = pathExtension.LastIndexOf('\\');
                    if (idx > 0 && idx == pathExtension.Length - 1)
                        pathExtension = pathExtension.Substring(0, idx);
                }
            }

            return ret;
        }

        /// <summary>
        /// Determines if 2 string based paths describe the same location or not.
        /// The method performs parsing of the given strings only.
        /// 
        /// Both paths should be normalized ('C:' vs. 'C:\') befores this is invoked.
        /// Method returns false if any of the given paths is empty.
        /// </summary>
        /// <param name="parentPath"></param>
        /// <param name="childPath"></param>
        /// <returns></returns>
        public static bool IsPathSameLocation(string parentPath, string childPath)
        {
            if (string.IsNullOrEmpty(parentPath) == true || string.IsNullOrEmpty(childPath) == true)
                return false;

            int idx = parentPath.LastIndexOf('\\');         // Remove last seperator char if present
            if (idx > 0 && idx == parentPath.Length - 1)
                parentPath = parentPath.Substring(0, idx);

            idx = childPath.LastIndexOf('\\');              // Remove last seperator char if present
            if (idx > 0 && idx == childPath.Length - 1)
                childPath = childPath.Substring(0, idx);

            return (string.Compare(childPath, parentPath, true) == 0);
        }

        /// <summary>
        /// Attempts to find a common root between the given path of <see cref="IDirectoryBrowser"/>
        /// model items and the string based path and returns the part of the alternative path
        /// that could be joined on to the element with the indicated index.
        /// 
        /// Returns the maximum common root (eg.: 'C:\Windows' is returned instead of 'C:\').
        /// </summary>
        /// <param name="currentPath"></param>
        /// <param name="navigateToThisLocation"></param>
        /// <param name="pathExtension"></param>
        /// <returns></returns>
        public static int FindCommonRoot(IDirectoryBrowser[] currentPath,
                                         string navigateToThisLocation,
                                         out string pathExtension)
        {
            pathExtension = null;

            if (currentPath.Length > 0)
            {
                for (int i = currentPath.Length - 1; i >= 0; i--)
                {
                    var model = currentPath[i];

                    if (string.IsNullOrEmpty(model.PathFileSystem))
                        break;

                    // found a common root item for path discription
                    if (ShellBrowser.IsParentPathOf(model.PathFileSystem, navigateToThisLocation, out pathExtension))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Extends a list of <see cref="IDirectoryBrowser"/> path models with the
        /// given path extension if these items can be verified in the file system structure.
        /// </summary>
        /// <param name="pathList"></param>
        /// <param name="pathExtension"></param>
        /// <returns></returns>
        public static bool ExtendPath(ref List<IDirectoryBrowser> pathList,
                                      string pathExtension)
        {
            string[] altPathNames = GetDirectories(pathExtension);
            for (int i = 0; i < altPathNames.Length; i++)
            {
                try
                {
                    var currentRootParseName = pathList[pathList.Count - 1].PathShell;

                    var nxt = ShellBrowser.GetChildItems(currentRootParseName, altPathNames[i]);

                    if (nxt.Any() == false)
                        return false;

                    pathList.Add(nxt.First());
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        #endregion methods
    }
}
