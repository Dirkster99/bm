namespace WSF
{
    using WSF.Browse;
    using WSF.IDs;
    using WSF.Interfaces;
    using WSF.Shell.Interop;
    using WSF.Shell.Interop.Dlls;
    using WSF.Shell.Interop.Interfaces.ShellFolders;
    using WSF.Shell.Interop.Knownfolders;
    using WSF.Shell.Interop.ShellFolders;
    using WSF.Shell.Pidl;
    using WSF.Shell.Enums;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using WSF.Enums;
    using System.Linq;

    /// <summary>
    /// Implements core API type methods and properties that are used to interact
    /// with Windows Shell System Items (folders and known folders).
    /// </summary>
    public static class Browser
    {
        #region ctors
        /// <summary>
        /// Static constructor
        /// </summary>
        static Browser()
        {
            KnownFileSystemFolders = new Dictionary<string, IDirectoryBrowser>();
        }
        #endregion  ctors

        #region properties
        /// <summary>
        /// Contains a collection of known folders with a file system folder.
        /// This collection is build on program start-up.
        /// </summary>
        private static Dictionary<string, IDirectoryBrowser> KnownFileSystemFolders { get; }

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
        /// Gets the interface for a user's 'This PC' (virtual folder).
        /// 
        /// This item usually lists Mounted drives (eg: 'C:\'),
        /// and frequently used special folders like: Desktop, Music, Video, Downloads etc..
        /// </summary>
        public static IDirectoryBrowser MyComputer
        {
            get
            {
                return Browser.Create(KF_IID.ID_FOLDERID_ComputerFolder);
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
        /// <param name="bFindKF"></param>
        /// <returns></returns>
        public static IDirectoryBrowser Create(string fullPath,
                                               bool bFindKF = false)
        {
            if (string.IsNullOrEmpty(fullPath) == true)
                throw new System.ArgumentNullException("path cannot be null or empty");

            try
            {
                // Try to locate a known folder by its directory path in the file system
                if (ShellHelpers.IsSpecialPath(fullPath) == ShellHelpers.SpecialPath.None)
                {
                    var item = Browser.FindKnownFolderByFileSystemPath(fullPath);

                    if (item != null)
                        return item;
                }

                var itemModel = BrowseItemFromPath.InitItemType(fullPath, bFindKF);

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
        public static IDirectoryBrowser Create(string parseName,
                                               string name,
                                               string labelName)
        {
            if (string.IsNullOrEmpty(parseName) == true)
                throw new System.ArgumentNullException("path cannot be null or empty");

            try
            {
                // Try to locate a known folder by its directory path in the file system
                if (ShellHelpers.IsSpecialPath(parseName) == ShellHelpers.SpecialPath.None)
                {
                    var item = Browser.FindKnownFolderByFileSystemPath(parseName);

                    if (item != null)
                        return item;
                }

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
        /// <param name="bFindKF">Determines if known folder should be looked up
        /// even if given folder is a normal string such as (eg.: 'C:\Windows\').
        /// Set this parameter only if you are sure that you need it as it will
        /// have a performance impact on the time required to generate the object.
        /// </param>
        /// <returns></returns>
        public static IDirectoryBrowser Create(IdList fullPidl,
                                               bool bFindKF = false)
        {
            if (fullPidl == null)
                return Create(KF_IID.ID_FOLDERID_Desktop);

            if (fullPidl.Size == 0)
                return Create(KF_IID.ID_FOLDERID_Desktop);

            string parseName = PidlManager.GetPathFromPIDL(fullPidl);

            if (string.IsNullOrEmpty(parseName) == true)
                return null;

            return Create(parseName, bFindKF);
        }

        /// <summary>
        /// Gets an enumeration of all childitems below the
        /// <paramref name="folderParseName"/> item.
        /// </summary>
        /// <param name="folderParseName">Is the parse name that should be used to emit child items for.</param>
        /// <param name="searchMask">Optional name of an item that should be filtered
        /// in case insensitive fashion when searching for a certain child rather than all children.</param>
        /// <param name="itemFilter">Specify wether to filter only on names or on names and ParseNames</param>
        /// <returns>returns each item as <see cref="IDirectoryBrowser"/> object</returns>
        public static IEnumerable<IDirectoryBrowser> GetChildItems(string folderParseName,
                                                                   string searchMask = null,
                                                                   SubItemFilter itemFilter = SubItemFilter.NameOnly)
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
                                kf.Obj.GetIDList((uint)KNOWN_FOLDER_FLAG.KF_NO_FLAGS, out pidlFull);
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
                if (searchMask != null)
                    filter = new FilterMask(searchMask);

                uint fetched, count = 0;
                IntPtr apidl = default(IntPtr);

                // Allocate memory to convert parsing names into .Net strings efficiently below
                ptrStr = Marshal.AllocCoTaskMem(NativeMethods.MAX_PATH * 2 + 4);
                Marshal.WriteInt32(ptrStr, 0, 0);
                StringBuilder strbuf = new StringBuilder(NativeMethods.MAX_PATH);
                var desktop = Browser.DesktopDirectory;

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
                        bool bFilter = false;
                        if (iFolder.GetDisplayNameOf(apidl, SHGDNF.SHGDN_INFOLDER | SHGDNF.SHGDN_FOREDITING, ptrStr) == HRESULT.S_OK)
                        {
                            NativeMethods.StrRetToBuf(ptrStr, default(IntPtr),
                                                      strbuf, NativeMethods.MAX_PATH);

                            name = strbuf.ToString();
                        }

                        // Skip this item if search parameter is set and this appears to be a non-match
                        if (filter != null)
                        {
                            if (filter.MatchFileMask(name) == false)
                            {
                                if (itemFilter == SubItemFilter.NameOnly) // Filter items on Names only
                                    continue;

                                bFilter = true;
                            }
                        }

                        string parseName = string.Empty;
                        if (iFolder.GetDisplayNameOf(apidl, SHGDNF.SHGDN_FORPARSING, ptrStr) == HRESULT.S_OK)
                        {
                            NativeMethods.StrRetToBuf(ptrStr, default(IntPtr),
                                                      strbuf, NativeMethods.MAX_PATH);

                            parseName = strbuf.ToString();
                        }

                        // Skip this item if search parameter is set and this appears to be a non-match
                        if (filter != null)
                        {
                            if (filter.MatchFileMask(parseName) == false && bFilter == true)
                                continue;
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

            folder = Browser.NormalizePath(folder);

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
        /// Attempts to re-root a path model item
        /// under the Desktop or ThisPC (filesystem) by looking up its PIDLS
        /// and building a sequence of path models from those PIDLS.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static IDirectoryBrowser[] FindRoot(IDirectoryBrowser location)
        {
            IDirectoryBrowser[] ret = null;

            try
            {
                var items = Browser.PathItemsAsIdList(location);
                IDirectoryBrowser[] pathItems = new IDirectoryBrowser[items.Count];

                for (int i = 0; i < items.Count; i++)
                    pathItems[i] = Browser.Create(items[i]);

                return pathItems;
            }
            catch
            {
                return ret;
            }
        }

        /// <summary>
        /// Attempts to re-root a sequence of path model items
        /// under the Desktop or ThisPC (filesystem).
        /// </summary>
        /// <param name="pathItems"></param>
        /// <param name="newPath"></param>
        /// <param name="pathIsRooted">Determines whether the output result
        /// was succesfully rooted or not. This parameter was introduced to
        /// support returning empty collection to indicate Desktop
        /// - NOT 100% sure if this is still required.</param>
        /// <returns></returns>
        public static IDirectoryBrowser[] FindRoot(IDirectoryBrowser[] pathItems,
                                                   string newPath,
                                                   out bool pathIsRooted)
        {
            pathIsRooted = false;
            bool foundRoot = false;
            List<IDirectoryBrowser> newRoot = new List<IDirectoryBrowser>();

            var desktop = Browser.DesktopDirectory;

            if (pathItems.Length >= 1)
            {
                // Is the desktop already part of the indicated location?
                for (int i = 0; i < pathItems.Length; i++)
                {
                    if (desktop.Equals(pathItems[i]) == true)
                    {
                        for (int j = i + 1; j < pathItems.Length; j++)
                            newRoot.Add(pathItems[j].Clone() as IDirectoryBrowser);

                        if (newRoot.Count == 0)
                        {
                            newRoot.Add(Browser.MyComputer);
                            newRoot.Add(Browser.DesktopDirectory);
                        }

                        pathIsRooted = true;
                        return newRoot.ToArray();
                    }
                }

                // Search this item under desktop root
                for (int i = pathItems.Length - 1; i >= 0; i--)
                {
                    var it = Browser.GetChildItems(KF_IID.ID_FOLDERID_Desktop, pathItems[i].Name);
                    if (it.Any())
                    {
                        // This is rooted so we just return it as is
                        if (pathItems[i].Equals(it.First()) == true)
                        {
                            for (int j = i; j < pathItems.Length; j++)
                                newRoot.Add(pathItems[j].Clone() as IDirectoryBrowser);

                            pathIsRooted = true;
                            return newRoot.ToArray();
                        }
                    }
                }

                // Can we find it under an item directly under the Destop (eg.: Under ThisPC)?
                // (then return it with eg.: ThisPC on top)
                foreach (var rootitem in Browser.GetChildItems(KF_IID.ID_FOLDERID_Desktop))
                {
                    // Evaluate only special items, such as, ThisPC, User etc.
                    if (string.IsNullOrEmpty(rootitem.SpecialPathId))
                        continue;

                    var itms = Browser.GetChildItems(rootitem.SpecialPathId, pathItems[0].Name);
                    if (itms.Any())
                    {
                        if (pathItems[0].Equals(itms.First()) == true)
                        {
                            newRoot.Add(rootitem);

                            for (int i = 0; i < pathItems.Length; i++)
                                newRoot.Add(pathItems[i].Clone() as IDirectoryBrowser);

                            pathIsRooted = true;
                            return newRoot.ToArray();
                        }
                    }
                }
            }

            if (Browser.IsTypeOf(newPath) == PathType.WinShellPath)
            {
                // Search root under desktop and return shortest possible number of items
                for (int idx = pathItems.Length - 1; idx >= 0; idx--)
                {
                    var dpItems = Browser.GetChildItems(KF_IID.ID_FOLDERID_Desktop, pathItems[idx].Name);
                    if (dpItems.Any())
                    {
                        for (int i = idx; i < pathItems.Length; i++)
                            newRoot.Add(pathItems[i].Clone() as IDirectoryBrowser);

                        pathIsRooted = true;
                        return newRoot.ToArray();
                    }
                }
            }

            string pathExt = null;
            if (Browser.IsParentPathOf(desktop.PathFileSystem, newPath, out pathExt))
            {
                int idx = -1;

                // Is this path under desktop but desktop burried in the middle of it?
                // Eg 'C:', 'Users', '<User>', 'Desktop', 'Folder', 'SubFolder'
                if (pathItems.Length > 1)
                {
                    for (int i = 0; i < pathItems.Length; i++)
                    {
                        if ((pathItems[i].ItemType & DirectoryItemFlags.Desktop) != 0)
                        {
                            idx = i;
                            break;
                        }
                    }

                    if (idx >= 0 && idx == pathItems.Length - 1)
                    {
                        // Requested location is desktop itself, so we return 'ThisPC','Desktop'
                        var retArr = new IDirectoryBrowser[2];
                        retArr[0] = Browser.MyComputer;

                        var dpItems = Browser.GetChildItems(retArr[0].SpecialPathId, desktop.Name);
                        if (dpItems.Any())
                        {
                            retArr[1] = dpItems.First();

                            return retArr;
                        }
                    }

                    if (idx >= 0)
                    {
                        // Return remaining path but skip desktop
                        // since items under desktop are ROOTED items
                        for (int i = idx + 1; i < pathItems.Length; i++)
                            newRoot.Add(pathItems[i].Clone() as IDirectoryBrowser);

                        pathIsRooted = true;
                        return newRoot.ToArray();
                    }
                }

                IDirectoryBrowser[] arrDesktop = new IDirectoryBrowser[1] { desktop };

                // Search root under desktop based on pathItems.Length == 1
                idx = Browser.FindCommonRoot(arrDesktop, newPath, out pathExt);

                if (idx >= 0)
                {
                    if ((pathItems[idx].ItemType & DirectoryItemFlags.Desktop) != 0)
                    {
                        if (idx < (pathItems.Length - 1))
                            idx++;
                        else
                            idx = -1; // Find Desktop under ThisPC/file system test below
                    }
                }

                if (idx >= 0)
                {
                    var dpItems = Browser.GetChildItems(KF_IID.ID_FOLDERID_Desktop, pathItems[idx].Name);
                    if (dpItems.Any())
                    {
                        for (int i = idx; i < pathItems.Length; i++)
                            newRoot.Add(pathItems[i].Clone() as IDirectoryBrowser);

                        pathIsRooted = true;
                        return newRoot.ToArray();
                    }
                }
            }

            // Second chance finding root under ThisPC
            var thisPC = Browser.MyComputer;
            foreach (var item in Browser.GetChildItems(KF_IID.ID_FOLDERID_ComputerFolder))
            {
                if (string.IsNullOrEmpty(item.PathFileSystem))
                    continue;

                pathExt = null;
                if (Browser.IsParentPathOf(item.PathFileSystem, newPath, out pathExt) == true)
                {
                    // Search root under ThisPC
                    int idx = Browser.FindCommonRoot(pathItems, item.PathFileSystem, out pathExt);

                    if (idx >= 0)
                    {
                        var dpItems = Browser.GetChildItems(KF_IID.ID_FOLDERID_ComputerFolder, pathItems[idx].Name);
                        if (dpItems.Any())
                        {
                            newRoot.Add(Browser.MyComputer);

                            for (int i = idx; i < pathItems.Length; i++)
                                newRoot.Add(pathItems[i].Clone() as IDirectoryBrowser);

                            pathIsRooted = true;
                            return newRoot.ToArray();
                        }
                    }
                }
            }

            // Third chance try finding root under ThisPC
            var items = Browser.GetChildItems(KF_IID.ID_FOLDERID_ComputerFolder, pathItems[0].Name);
            if (items.Any())
            {
                foundRoot = true;
                newRoot.Add(Browser.MyComputer);
            }

            // No rooted item found for re-mount
            if (foundRoot == false)
                return null;

            for (int i = 0; i < pathItems.Length; i++) //Join path to root and return to sender
                newRoot.Add(pathItems[i].Clone() as IDirectoryBrowser);

            pathIsRooted = true;
            return newRoot.ToArray();
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
        /// (path can be formatted as special path KF_IDD) and returns <paramref name="pathItems"/>
        /// if path was a sequence of Windows shell named items (eg 'Libraries\Music').
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathItems"></param>
        /// <param name="bFindKF">Determines if known folder should be looked up
        /// even if given folder is a normal string such as (eg.: 'C:\Windows\').
        /// Set this parameter only if you are sure that you need it as it will
        /// have a performance impact on the time required to generate the object.
        /// </param>
        /// <returns>Returns true if item has a filesystem path otherwise false.</returns>
        public static bool DirectoryExists(string path,
                                           out IDirectoryBrowser[] pathItems,
                                           bool bFindKF = false)
        {
            pathItems = null;

            if (string.IsNullOrEmpty(path))
                return false;

            if (ShellHelpers.IsSpecialPath(path) == ShellHelpers.SpecialPath.IsSpecialPath)
            {
                try
                {
                    // translate KF_IID into file system path and check if it exists
                    string fs_path = KnownFolderHelper.GetKnownFolderPath(path);

                    if (fs_path != null)
                    {
                        bool exists = System.IO.Directory.Exists(fs_path);

                        if (exists)
                            pathItems = GetFileSystemPathItems(fs_path, bFindKF);

                        return exists;
                    }
                }
                catch
                {
                    return false;
                }

                return false;
            }
            else
            {
                if (path.Length < 2)
                    return false;

                try
                {
                    if ((path[0] == '\\' && path[1] == '\\') || path[1] == ':')
                    {
                        bool exists = System.IO.Directory.Exists(path);

                        if (exists)
                            pathItems = GetFileSystemPathItems(path, bFindKF);

                        return exists;
                    }

                    if (path.Length > 1)
                        path = path.TrimEnd('\\');

                    // Try to resolve an abstract Windows Shell Space description like:
                    // 'Libraries/Documents' (valid in a localized fashion only)
                    pathItems = GetWinShellPathItems(path);

                    // This path exists as sequence of localized names of windows shell items
                    if (pathItems != null)
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
        /// Converts a given existing file system path string into a sequence
        /// of <see cref="IDirectoryBrowser"/> items or null if path cannot be resolved.
        /// </summary>
        /// <param name="fs_path">The file system path to be resolved.</param>
        /// <param name="bFindKF">Determines if known folder should be looked up
        /// even if given folder is a normal string such as (eg.: 'C:\Windows\').
        /// Set this parameter only if you are sure that you need it as it will
        /// have a performance impact on the time required to generate the object.
        /// </param>
        /// <returns></returns>
        public static IDirectoryBrowser[] GetFileSystemPathItems(string fs_path,
                                                                 bool bFindKF = false)
        {
            try
            {
                var dirs = Browser.GetDirectories(fs_path);
                var dirItems = new IDirectoryBrowser[dirs.Length];
                string currentPath = null;
                for (int i = 0; i < dirItems.Length; i++)
                {
                    if (currentPath == null)
                        currentPath = dirs[0];
                    else
                        currentPath = System.IO.Path.Combine(currentPath, dirs[i]);

                    dirItems[i] = Browser.Create(currentPath, bFindKF);
                }

                return dirItems;
            }
            catch
            {
                // Lets make sure we can recover from errors
                return null;
            }
        }

        /// <summary>
        /// Converts a given existing Windows shell Path string
        /// (eg 'Libraries\Music') into a sequence of <see cref="IDirectoryBrowser"/>
        /// items or null if path cannot be resolved.
        /// </summary>
        /// <param name="path">The Windows Shell Path to be resolved.</param>
        /// <returns></returns>
        public static IDirectoryBrowser[] GetWinShellPathItems(string path)
        {
            IDirectoryBrowser[] pathItems = null;
            try
            {
                string[] pathNames = GetDirectories(path);

                if (pathNames == null)
                    return null;

                if (pathNames.Length == 0)
                    return null;

                pathItems = new IDirectoryBrowser[pathNames.Length];

                string parentPath = KF_IID.ID_FOLDERID_Desktop;
                for (int i = 0; i < pathItems.Length; i++)
                {
                    if (i > 0)
                        parentPath = pathItems[i - 1].PathShell;

                    var subList = Browser.GetChildItems(parentPath, pathNames[i]);
                    if (subList.Any())
                    {
                        pathItems[i] = subList.First();
                    }
                    else
                        return null;
                }

                return pathItems;
            }
            catch
            {
                // Lets make sure we can recover from errors
                return null;
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
        /// Compares 2 to paths and indicates whether they match or not.
        /// Both strings should be normalized before calling this method.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static PathMatch IsCurrentPath(string inputPath, string path)
        {
            if (string.IsNullOrEmpty(inputPath) == true &&
                string.IsNullOrEmpty(path) == true)
                return PathMatch.CompleteMatch;

            if ((string.IsNullOrEmpty(inputPath) == true && string.IsNullOrEmpty(path) == false) ||
                (string.IsNullOrEmpty(inputPath) == false && string.IsNullOrEmpty(path) == true))
                return PathMatch.Unrelated;

            // Remove ending backslash to normalize both strings for comparison
            int idx = inputPath.LastIndexOf('\\');
            if (idx == (inputPath.Length - 1))
                inputPath = inputPath.Substring(0, inputPath.Length - 1);

            if (string.Compare(path, inputPath, true) == 0)
                return PathMatch.CompleteMatch;

            if (inputPath.Length > path.Length)
            {
                string tmpInputPath = inputPath.Substring(0, path.Length);
                if (string.Compare(path, tmpInputPath, true) == 0)
                    return PathMatch.PartialTarget;
            }
            else
            {
                if (path.Length > inputPath.Length)
                {
                    string tmpPath = path.Substring(0, inputPath.Length);
                    if (string.Compare(inputPath, tmpPath, true) == 0)
                        return PathMatch.PartialSource;
                }
            }

            return PathMatch.Unrelated;
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

            for (int i = currentPath.Length - 1; i >= 0; i--)
            {
                var model = currentPath[i];

                if (string.IsNullOrEmpty(model.PathFileSystem))
                    break;

                // found a common root item for path discription
                if (Browser.IsParentPathOf(model.PathFileSystem, navigateToThisLocation, out pathExtension))
                    return i;
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

                    var nxt = Browser.GetChildItems(currentRootParseName, altPathNames[i]);

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


        /// <summary>
        /// Make sure that a path reference does actually work with
        /// <see cref="System.IO.DirectoryInfo"/> by replacing 'C:' by 'C:\'.
        /// </summary>
        /// <param name="dirOrfilePath"></param>
        /// <returns></returns>
        public static string NormalizePath(string dirOrfilePath)
        {
            if (string.IsNullOrEmpty(dirOrfilePath) == true)
                return dirOrfilePath;

            // The dirinfo constructor will not work with 'C:' but does work with 'C:\'
            if (dirOrfilePath.Length < 2)
                return dirOrfilePath;

            if (dirOrfilePath.Length == 2)
            {
                if (dirOrfilePath[dirOrfilePath.Length - 1] == ':')
                    return dirOrfilePath + System.IO.Path.DirectorySeparatorChar;
            }

            if (dirOrfilePath.Length == 3)
            {
                if (dirOrfilePath[dirOrfilePath.Length - 2] == ':' &&
                    dirOrfilePath[dirOrfilePath.Length - 1] == System.IO.Path.DirectorySeparatorChar)
                    return dirOrfilePath;

                if (dirOrfilePath[1] == ':')
                    return "" + dirOrfilePath[0] + dirOrfilePath[1] +
                                System.IO.Path.DirectorySeparatorChar + dirOrfilePath[2];
                else
                    return dirOrfilePath;
            }

            // Insert a backslash in 3rd character position if not already present
            // C:Temp\myfile -> C:\Temp\myfile
            if (dirOrfilePath.Length >= 3)
            {
                if (char.ToUpper(dirOrfilePath[0]) >= 'A' && char.ToUpper(dirOrfilePath[0]) <= 'Z' &&
                    dirOrfilePath[1] == ':' &&
                    dirOrfilePath[2] != '\\')
                {
                    dirOrfilePath = dirOrfilePath.Substring(0, 2) + "\\" + dirOrfilePath.Substring(2);
                }
            }

            // This will normalize directory and drive references into 'C:' or 'C:\Temp'
            if (dirOrfilePath[dirOrfilePath.Length - 1] == System.IO.Path.DirectorySeparatorChar)
                dirOrfilePath = dirOrfilePath.Trim(System.IO.Path.DirectorySeparatorChar);

            return dirOrfilePath;
        }

        /// <summary>
        /// Gets an enumeration of all childitems below the
        /// <paramref name="folderParseName"/> item.
        /// </summary>
        /// <param name="folderParseName">Is the parse name that should be used to emit child items for.</param>
        /// <param name="searchMask">Optional name of an item that should be filtered
        /// in case insensitive fashion when searching for a certain child rather than all children.</param>
        /// <param name="itemFilter">Specify wether to filter only on names or on names and ParseNames</param>
        /// <returns>returns each item as <see cref="DirectoryBrowserSlim"/> object</returns>
        public static IEnumerable<DirectoryBrowserSlim> GetSlimChildItems(string folderParseName,
                                                                          string searchMask = null,
                                                                          SubItemFilter itemFilter = SubItemFilter.NameOnly)
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
                                kf.Obj.GetIDList((uint)KNOWN_FOLDER_FLAG.KF_NO_FLAGS, out pidlFull);
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
                if (searchMask != null)
                    filter = new FilterMask(searchMask);

                uint fetched, count = 0;
                IntPtr apidl = default(IntPtr);

                // Allocate memory to convert parsing names into .Net strings efficiently below
                ptrStr = Marshal.AllocCoTaskMem(NativeMethods.MAX_PATH * 2 + 4);
                Marshal.WriteInt32(ptrStr, 0, 0);
                StringBuilder strbuf = new StringBuilder(NativeMethods.MAX_PATH);
                int index = 0;

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
                        bool bFilter = false;
                        if (iFolder.GetDisplayNameOf(apidl, SHGDNF.SHGDN_INFOLDER | SHGDNF.SHGDN_FOREDITING, ptrStr) == HRESULT.S_OK)
                        {
                            NativeMethods.StrRetToBuf(ptrStr, default(IntPtr),
                                                      strbuf, NativeMethods.MAX_PATH);

                            name = strbuf.ToString();
                        }

                        // Skip this item if search parameter is set and this appears to be a non-match
                        if (filter != null)
                        {
                            if (filter.MatchFileMask(name) == false)
                            {
                                if (itemFilter == SubItemFilter.NameOnly) // Filter items on Names only
                                    continue;

                                bFilter = true;
                            }
                        }

                        string parseName = string.Empty;
                        if (iFolder.GetDisplayNameOf(apidl, SHGDNF.SHGDN_FORPARSING, ptrStr) == HRESULT.S_OK)
                        {
                            NativeMethods.StrRetToBuf(ptrStr, default(IntPtr),
                                                      strbuf, NativeMethods.MAX_PATH);

                            parseName = strbuf.ToString();
                        }

                        // Skip this item if search parameter is set and this appears to be a non-match
                        if (filter != null)
                        {
                            if (filter.MatchFileMask(parseName) == false && bFilter == true)
                                continue;
                        }

                        string labelName = string.Empty;
                        if (iFolder.GetDisplayNameOf(apidl, SHGDNF.SHGDN_NORMAL, ptrStr) == HRESULT.S_OK)
                        {
                            NativeMethods.StrRetToBuf(ptrStr, default(IntPtr),
                                                      strbuf, NativeMethods.MAX_PATH);

                            labelName = strbuf.ToString();
                        }

                        IdList apidlIdList = PidlManager.PidlToIdlist(apidl);

                        yield return new DirectoryBrowserSlim(index++, folderParseName, parseName, name, labelName);
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
        /// Gets a strongly-typed collection of all registered known folders that have
        /// an associated file system path.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, IDirectoryBrowser> GetAllKnownFolders()
        {
            // Should this method be thread-safe?? (It'll take a while
            // to get a list of all the known folders, create the managed wrapper
            // and return the read-only collection.
            var pathList = new Dictionary<string, IDirectoryBrowser>();
            uint count;
            IntPtr folders = IntPtr.Zero;

            try
            {
                KnownFolderManagerClass knownFolderManager = new KnownFolderManagerClass();
                var result = knownFolderManager.GetFolderIds(out folders, out count);

                if (count > 0 && folders != IntPtr.Zero)
                {
                    // Loop through all the KnownFolderID elements
                    for (int i = 0; i < count; i++)
                    {
                        // Read the current pointer
                        IntPtr current = new IntPtr(folders.ToInt64() + (Marshal.SizeOf(typeof(Guid)) * i));

                        // Convert to Guid
                        Guid knownFolderID = (Guid)Marshal.PtrToStructure(current, typeof(Guid));

                        try
                        {
                            var folder = Browser.Create("::" + knownFolderID.ToString("B"), true);

                            if (folder != null &&
                                string.IsNullOrEmpty(folder.PathFileSystem) == false)
                            {
                                // It is possible to have more than one known folder point at one
                                // file system location - but this implementation still handles
                                // unique file locations and associated folders
                                IDirectoryBrowser val = null;
                                if (pathList.TryGetValue(folder.PathFileSystem, out val) == false)
                                {
                                    if (string.IsNullOrEmpty(folder.PathFileSystem) == false)
                                        pathList.Add(folder.PathFileSystem, folder);
                                }
                            }

                            ////                            using (var nativeKF = KnownFolderHelper.FromKnownFolderGuid(knownFolderID))
                            ////                            {
                            ////                                var kf = KnownFolderHelper.GetFolderProperties(nativeKF.Obj);
                            ////
                            ////                                // Add to our collection if it's not null (some folders might not exist on the system
                            ////                                // or we could have an exception that resulted in the null return from above method call
                            ////                                if (kf != null)
                            ////                                {
                            ////                                    foldersList.Add(kf.FolderId, kf);
                            ////
                            ////                                    if (kf.IsExistsInFileSystem == true)
                            ////                                        pathList.Add(kf.Path, kf);
                            ////                                }
                            ////                            }
                        }
                        catch { }
                    }
                }
            }
            finally
            {
                if (folders != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(folders);
            }

            return pathList;
        }

        /// <summary>
        /// Tries to determine whether there is a known folder associated with this
        /// path or not.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IDirectoryBrowser FindKnownFolderByFileSystemPath(string path)
        {
            if (KnownFileSystemFolders.Count == 0)
            {
                foreach (var item in GetAllKnownFolders())
                    KnownFileSystemFolders.Add(item.Key, item.Value);
            }

            IDirectoryBrowser matchedItem = null;
            KnownFileSystemFolders.TryGetValue(path, out matchedItem);

            return matchedItem;
        }
        #endregion methods
    }
}
