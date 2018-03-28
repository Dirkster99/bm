///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2009 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Steven Roebert's work (http://www.codeproject.com/KB/miscctrl/FileBrowser.aspx)    //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using ShellDll;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.ComponentModel;
using System.IO.Utils;
using System.Threading.Tasks;
using System.Threading;

namespace System.IO
{
    /// <summary>
    /// Represents a directory in PIDL system.
    /// </summary>
    [Serializable]
    public class DirectoryInfoEx : FileSystemInfoEx, IDisposable, ISerializable, ICloneable
    {
        public enum DirectoryTypeEnum { dtDesktop, dtSpecial, dtDrive, dtFolder, dtRoot }

        #region Static Variables
        public static readonly DirectoryInfoEx DesktopDirectory;
        public static readonly DirectoryInfoEx MyComputerDirectory;
        public static readonly DirectoryInfoEx CurrentUserDirectory;
        public static readonly DirectoryInfoEx SharedDirectory;
        public static readonly DirectoryInfoEx NetworkDirectory;
        public static readonly DirectoryInfoEx RecycleBinDirectory;

        static DirectoryInfoEx()
        {
#if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                throw new Exception("This should not be executed when design time.");
#endif
            DesktopDirectory = new DirectoryInfoEx(CSIDLtoPIDL(ShellAPI.CSIDL.CSIDL_DESKTOP));
            MyComputerDirectory = new DirectoryInfoEx(CSIDLtoPIDL(ShellAPI.CSIDL.CSIDL_DRIVES));
            CurrentUserDirectory = new DirectoryInfoEx(CSIDLtoPIDL(ShellAPI.CSIDL.CSIDL_PROFILE));
            //0.17: Fixed some system cannot create shared directories. (by cwharmon)
            try { SharedDirectory = new DirectoryInfoEx(CSIDLtoPIDL(ShellAPI.CSIDL.CSIDL_COMMON_DOCUMENTS)); }
            catch { }
            NetworkDirectory = new DirectoryInfoEx(CSIDLtoPIDL(ShellAPI.CSIDL.CSIDL_NETWORK));
            RecycleBinDirectory = new DirectoryInfoEx(CSIDLtoPIDL(ShellAPI.CSIDL.CSIDL_BITBUCKET));

            //foreach (DirectoryInfoEx dir in DesktopDirectory.GetDirectories())
            //    if (dir.FullName.Equals(Helper.GetCurrentUserPath()))
            //    { CurrentUserDirectory = dir; break; }

            //foreach (DirectoryInfoEx dir in DesktopDirectory.GetDirectories())
            //    if (dir.FullName.Equals(Helper.GetSharedPath()))
            //    { SharedDirectory = dir; break; }


        }
        #endregion

        #region Variables
        private DirectoryTypeEnum _dirType;
        private bool _isBrowsable, isFileSystem, _hasSubFolder;

        public ShellFolder2 ShellFolder { get { return getIShellFolder(); } }
        public Storage Storage { get { return getStorage(); } }
        public DirectoryInfoEx Root { get { return getDirectoryRoot(this); } }
        public bool IsBrowsable { get { checkRefresh(); return _isBrowsable; } set { _isBrowsable = value; } }
        public bool IsFileSystem { get { checkRefresh(); return isFileSystem; } set { isFileSystem = value; } }
        public bool HasSubFolder { get { checkRefresh(); return _hasSubFolder; } set { _hasSubFolder = value; } }
        public DirectoryTypeEnum DirectoryType { get { return _dirType; } protected set { _dirType = value; } }
        public Environment.SpecialFolder? ShellFolderType { get { var kf = KnownFolderType; return kf == null ? null : kf.SpecialFolder; } }
        public KnownFolder KnownFolderType { get { return this.RequestPIDL(pidl => KnownFolder.FromPidl(pidl)); } }
        public KnownFolderIds? KnownFolderId { get { var kf = KnownFolderType;  return kf == null ? null : kf.KnownFolderId; } }
        
        //0.12: Fixed PIDL, PIDLRel, ShellFolder, Storage properties generated on demand to avoid x-thread issues.
        private Storage getStorage()
        {
            Storage iStorage = null;
            //0.15: Fixed ShellFolder not freed correctly
            //if (ShellFolder2 != null)
            if (IOTools.getIStorage(this, out iStorage))
                return iStorage;
            return null;
        }


        private ShellFolder2 getIShellFolderFromParent()
        {
            if (_parent != null)
                using (ShellFolder2 parentShellFolder = _parent.ShellFolder)
                {
                    IntPtr ptrShellFolder = IntPtr.Zero;

                    int hr = this.RequestRelativePIDL(relPIDL =>
                        parentShellFolder.BindToObject(relPIDL.Ptr, IntPtr.Zero, ref ShellAPI.IID_IShellFolder2,
                        out ptrShellFolder));

                    if (ptrShellFolder != IntPtr.Zero && hr == ShellAPI.S_OK)
                        return new ShellFolder2(ptrShellFolder);
                }
            return null;
        }

        private ShellFolder2 getIShellFolder()
        {
            if (this.FullName.Equals(IOTools.IID_Desktop))
                return getDesktopShellFolder();
            else
            {
                ShellFolder2 retVal = getIShellFolderFromParent();
                if (retVal != null)
                    return retVal;

                return this.RequestPIDL((pidl, relPIDL) =>
                    {
                        using (ShellFolder2 parentShellFolder = getParentIShellFolder(pidl, out relPIDL))
                        {
                            IntPtr ptrShellFolder;
                            int hr = parentShellFolder.BindToObject(relPIDL.Ptr, IntPtr.Zero, ref ShellAPI.IID_IShellFolder2,
                                out ptrShellFolder);
                            if (ptrShellFolder == IntPtr.Zero || hr != ShellAPI.S_OK) Marshal.ThrowExceptionForHR(hr);
                            return new ShellFolder2(ptrShellFolder);
                        }

                    });
            }
        }

        protected void initDirectoryType()
        {
            string path = FullName != null ? FullName : OriginalPath;

            if (path.Equals(IOTools.IID_Desktop))
                _dirType = DirectoryTypeEnum.dtDesktop;
            else
                if (path != null)
                    if (path.EndsWith(":\\"))
                        _dirType = DirectoryTypeEnum.dtDrive;
                    else if (path.StartsWith("::"))
                        _dirType = DirectoryTypeEnum.dtSpecial;
                    else _dirType = DirectoryTypeEnum.dtFolder;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Convert CSIDL to PIDL
        /// </summary>
        internal static PIDL CSIDLtoPIDL(ShellDll.ShellAPI.CSIDL csidl)
        {
            IntPtr ptrAddr;
            PIDL pidl;

            //if (csidl == ShellAPI.CSIDL.CSIDL_MYDOCUMENTS)
            //    return PathtoPIDL(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            {
                int RetVal = ShellAPI.SHGetSpecialFolderLocation(IntPtr.Zero, csidl, out ptrAddr);
                if (ptrAddr != IntPtr.Zero)
                {
                    pidl = new PIDL(ptrAddr, false);
                    return pidl;
                }
                else throw new ArgumentException("Invalid csidl " + RetVal);
            }
            //return null;
        }

        internal static PIDL KnownFolderToPIDL(KnownFolder knownFolder)
        {
            IntPtr ptrAddr;
            PIDL pidl;

            int RetVal = (knownFolder._knownFolder).GetIDList(KnownFolderRetrievalOptions.Create, out ptrAddr);
            if (ptrAddr != IntPtr.Zero)
            {
                pidl = new PIDL(ptrAddr, false);
                return pidl;
            }
            else throw new ArgumentException("Invalid knownFolder " + RetVal);
        }

        /// <summary>
        /// Gets the path to the system special folder identified by the specified enumeration.
        /// </summary>
        public static string GetFolderPath(ShellAPI.CSIDL folder)
        {
            PIDL pidlLookup = CSIDLtoPIDL(folder);
            try
            {
                return FileSystemInfoEx.PIDLToPath(pidlLookup);
            }
            finally { if (pidlLookup != null) pidlLookup.Free(); }

        }

        /// <summary>
        /// Convert a Parsable path to PIDL
        /// </summary>
        private static PIDL PathtoPIDL(string path)
        {
            IntPtr pidlPtr;
            uint pchEaten = 0;
            ShellAPI.SFGAO pdwAttributes = 0;
            using (ShellFolder2 _desktopShellFolder = getDesktopShellFolder())
                _desktopShellFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, path, ref pchEaten, out pidlPtr, ref pdwAttributes);
            PIDL pidl = new PIDL(pidlPtr, false);
            return pidl;
        }

        /// <summary>
        /// Takes a directoryInfoEx and return the first parent with directory type = desktop or drive.
        /// </summary>
        internal static DirectoryInfoEx getDirectoryRoot(DirectoryInfoEx lookup)
        {
            DirectoryInfoEx dir = lookup.Parent;
            while (dir.DirectoryType != DirectoryTypeEnum.dtDesktop &&
                dir.DirectoryType != DirectoryTypeEnum.dtDrive &&
                dir.DirectoryType != DirectoryTypeEnum.dtRoot &&
                dir != null)
                dir = dir.Parent;
            if (dir == null)
                throw new IOException("Internal exception in GetDirectoryRoot.");
            return dir;
        }
        #endregion

        #region Methods

        public override bool Equals(FileSystemInfoEx other)
        {
            if (other is DirectoryInfoEx)
            {
                if (this.FullName.Equals(other.FullName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

                DirectoryInfoEx dirObj = other as DirectoryInfoEx;
                if (dirObj.FullName.Equals(IOTools.IID_UserFiles))
                    if (this.FullName.Equals(DirectoryInfoEx.CurrentUserDirectory.FullName, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                if (dirObj.FullName.Equals(IOTools.IID_Public))
                    if (this.FullName.Equals(DirectoryInfoEx.SharedDirectory.FullName, StringComparison.InvariantCultureIgnoreCase))
                        return true;
            }

            return base.Equals(other);
        }

        protected void checkExists()
        {
            //0.18: checkExists() ignore network directory.
            if (!FullName.StartsWith("\\") && !Exists)
                throw new DirectoryNotFoundException(FullName + " is not exists.");
        }

        /// <summary>
        /// Create the directory.
        /// </summary>
        public void Create()
        {
            if (Exists)
                throw new IOException("Directory already exists.");
            if (Parent == null)
                throw new IOException("Cannot construct parent.");
            if (!Parent.Exists)
                Parent.Create();

            IntPtr outPtr;
            int hr = Parent.Storage.CreateStorage(Name, ShellDll.ShellAPI.STGM.FAILIFTHERE |
                ShellDll.ShellAPI.STGM.CREATE, 0, 0, out outPtr);
            Storage storage = new Storage(outPtr);

            if (hr != ShellAPI.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            Refresh();
        }
        /// <summary>
        /// Delete this folder. (not move it to recycle bin)
        /// </summary>
        public override void Delete()
        {
            checkExists();
            //iStorage = null;
            //iShellFolder = null;
            int hr = Parent.Storage.DestroyElement(Name);
            if (hr != ShellAPI.S_OK) Marshal.ThrowExceptionForHR(hr);
            Refresh();
        }
        /// <summary>
        /// Move this folder to specified directory (fullpath)
        /// </summary>
        public void MoveTo(string destDirName)
        {
            checkExists();
            //iStorage = null;
            //iShellFolder = null;
            destDirName = IOTools.ExpandPath(destDirName);
            IOTools.Move(FullName, destDirName);
            FullName = destDirName;
            OriginalPath = FullName;
            Refresh();
        }

        protected override void refresh(IShellFolder2 parentShellFolder, PIDL relPIDL, PIDL fullPIDL, RefreshModeEnum mode)
        {
            base.refresh(parentShellFolder, relPIDL, fullPIDL, mode);

            if ((mode & RefreshModeEnum.FullProps) != 0 &&
                parentShellFolder != null && relPIDL != null && fullPIDL != null)
            {
                ShellAPI.SFGAO attribute = shGetFileAttribute(fullPIDL, ShellAPI.SFGAO.BROWSABLE |
                    ShellAPI.SFGAO.FILESYSTEM | ShellAPI.SFGAO.HASSUBFOLDER);
                IsBrowsable = (attribute & ShellAPI.SFGAO.BROWSABLE) != 0 || (attribute & ShellAPI.SFGAO.CONTENTSMASK) != 0;
                IsFileSystem = (attribute & ShellAPI.SFGAO.FILESYSTEM) != 0;
                HasSubFolder = (attribute & ShellAPI.SFGAO.HASSUBFOLDER) != 0;

                if (!FullName.StartsWith("::") && Directory.Exists(FullName))
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(FullName);
                        Attributes = di.Attributes;
                        LastAccessTime = di.LastAccessTime;
                        LastWriteTime = di.LastWriteTime;
                        CreationTime = di.CreationTime;
                    }
                    catch { }

                initDirectoryType();
            }
        }

        /// <summary>
        /// Create a subdirectory
        /// </summary>
        /// <param name="path"> directory name.</param>
        public DirectoryInfoEx CreateDirectory(string path)
        {
            checkExists();
            return DirectoryEx.CreateDirectory(PathEx.Combine(FullName, path));
        }


        #endregion

        #region Methods - GetSubItems
        static ShellAPI.SHCONTF flag = ShellAPI.SHCONTF.NONFOLDERS | ShellAPI.SHCONTF.FOLDERS | ShellAPI.SHCONTF.INCLUDEHIDDEN;
        //static ShellAPI.SHCONTF folderflag = ShellAPI.SHCONTF.FOLDERS | ShellAPI.SHCONTF.INCLUDEHIDDEN;
        //static ShellAPI.SHCONTF fileflag = ShellAPI.SHCONTF.NONFOLDERS | ShellAPI.SHCONTF.INCLUDEHIDDEN;

        private List<FileSystemInfoEx> _cachedDirList = new List<FileSystemInfoEx>();
        //private List<FileSystemInfoEx> _cachedFileList = new List<FileSystemInfoEx>();

        private static bool listContains(List<FileSystemInfoEx> list, PIDL pidl)
        {
            foreach (FileSystemInfoEx item in list)
                if (item.RequestRelativePIDL((relPidl) => relPidl.Equals(pidl)))
                    return true;

            return false;
        }
        private static void listRemove(List<FileSystemInfoEx> list, PIDL pidl)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (list[i].RequestRelativePIDL((relPidl) => relPidl.Equals(pidl)))
                { list.RemoveAt(i); return; }
        }


        //0.17: Added DirectoryInfoEx.EnumerateFiles/EnumerateDirectories/EnumerateFileSystemInfos() methods which work similar as the one in .Net4
        public IEnumerable<FileInfoEx> EnumerateFiles(String searchPattern, SearchOption searchOption, CancelDelegate cancel)
        {
            IntPtr ptrEnum = IntPtr.Zero;
            IEnumIDList IEnum = null;
            PIDL parentPIDL = this.getPIDL();

            List<IntPtr> trashPtrList = new List<IntPtr>();
            using (ShellFolder2 sf = this.ShellFolder)
                try
                {
                    if (sf.EnumObjects(IntPtr.Zero, flag, out ptrEnum) == ShellAPI.S_OK)
                    {
                        IEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(ptrEnum, typeof(IEnumIDList));
                        IntPtr pidlSubItemPtr;
                        int celtFetched;

                        while (!IOTools.IsCancelTriggered(cancel) && IEnum.Next(1, out pidlSubItemPtr, out celtFetched) == ShellAPI.S_OK && celtFetched == 1)
                        {
                            trashPtrList.Add(pidlSubItemPtr); //0.24 : Large memory Leak here.    

                            ShellAPI.SFGAO attribs = ShellAPI.SFGAO.FOLDER | ShellAPI.SFGAO.FILESYSTEM | ShellAPI.SFGAO.STREAM;
                            sf.GetAttributesOf(1, new IntPtr[] { pidlSubItemPtr }, ref attribs);
                            //http://www.eggheadcafe.com/aspnet_answers/platformsdkshell/Mar2006/post26165601.asp
                            bool isZip = ((attribs & ShellAPI.SFGAO.FOLDER) != 0 && (attribs & ShellAPI.SFGAO.STREAM) != 0);
                            bool isDir = ((attribs & ShellAPI.SFGAO.FOLDER) != 0);
                            if (isZip || !isDir)
                            {
                                PIDL subRelPidl = new PIDL(pidlSubItemPtr, true);
                                //FileInfoEx fi = new FileInfoEx(sf, this, subRelPidl);
                                FileInfoEx fi = new FileInfoEx(sf, parentPIDL, subRelPidl);
                                if (IOTools.MatchFileMask(fi.Name, searchPattern))
                                    yield return fi;
                                //0.18: Fixed DirectoryInfoEx.EnumerateFiles, SearchPattern is ignored.
                            }
                        }

                        if (searchOption == SearchOption.AllDirectories)
                        {
                            IEnumerator<DirectoryInfoEx> dirEnumerator = EnumerateDirectories("*", SearchOption.TopDirectoryOnly, cancel).GetEnumerator();

                            while (!IOTools.IsCancelTriggered(cancel) && dirEnumerator.MoveNext())
                            {
                                IEnumerator<FileInfoEx> fileEnumerator = dirEnumerator.Current.EnumerateFiles(searchPattern, searchOption, cancel).GetEnumerator();

                                while (fileEnumerator.MoveNext())
                                {
                                    //Debug.Assert(!fileEnumerator.Current.IsFolder);
                                    yield return fileEnumerator.Current;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (parentPIDL != null)
                    {
                        parentPIDL.Free();
                        parentPIDL = null;
                    }

                    if (IEnum != null)
                    {
                        Marshal.ReleaseComObject(IEnum);
                        Marshal.Release(ptrEnum);
                    }


                    if (trashPtrList != null)
                        foreach (var ptr in trashPtrList)
                            Marshal.FreeCoTaskMem(ptr); //0.24 : Large memory Leak here.  
                }
        }
        public IEnumerable<FileInfoEx> EnumerateFiles(String searchPattern, SearchOption searchOption) { return EnumerateFiles(searchPattern, searchOption, null); }
        public IEnumerable<FileInfoEx> EnumerateFiles(String searchPattern) { return EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly); }
        public IEnumerable<FileInfoEx> EnumerateFiles() { return EnumerateFiles("*", SearchOption.TopDirectoryOnly); }

        public IEnumerable<DirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption, CancelDelegate cancel)
        {
            IntPtr ptrEnum = IntPtr.Zero;
            IEnumIDList IEnum = null;
            PIDL parentPIDL = this.getPIDL();

            List<IntPtr> trashPtrList = new List<IntPtr>();
            using (ShellFolder2 sf = this.ShellFolder)
                try
                {
                    if (sf.EnumObjects(IntPtr.Zero, flag, out ptrEnum) == ShellAPI.S_OK)
                    {
                        IEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(ptrEnum, typeof(IEnumIDList));
                        IntPtr pidlSubItemPtr;
                        int celtFetched;


                        while (!IOTools.IsCancelTriggered(cancel) && IEnum.Next(1, out pidlSubItemPtr, out celtFetched) == ShellAPI.S_OK && celtFetched == 1)
                        {
                            trashPtrList.Add(pidlSubItemPtr); //0.24 : Large memory Leak here.                                

                            ShellAPI.SFGAO attribs = ShellAPI.SFGAO.FOLDER | ShellAPI.SFGAO.FILESYSTEM | ShellAPI.SFGAO.STREAM |
                                ShellAPI.SFGAO.FILESYSANCESTOR | ShellAPI.SFGAO.NONENUMERATED;
                            sf.GetAttributesOf(1, new IntPtr[] { pidlSubItemPtr }, ref attribs);
                            bool isZip = ((attribs & ShellAPI.SFGAO.FOLDER) != 0 && (attribs & ShellAPI.SFGAO.STREAM) != 0);
                            bool isDir = ((attribs & ShellAPI.SFGAO.FOLDER) != 0);
                            //0.18 Added a check for NonEnumerated items so DirectoryInfoEx.EnumerateDirectories wont return some system directories (e.g. C:\MSOCache)
                            //bool isNonEnumerated = ((attribs & ShellAPI.SFGAO.NONENUMERATED) != 0);
                            bool isFileAncestor = ((attribs & ShellAPI.SFGAO.FILESYSANCESTOR) != 0);
                            bool includedFolder = false;

                            if (!isZip && !isFileAncestor) //0.14 : Added allowed folder list so Non-FileAncestor directory (e.g. recycle-bin) is listed.
                            {
                                string[] allowedPaths = new string[]
                                {
                                    "::{645FF040-5081-101B-9F08-00AA002F954E}"
                                };

                                string path = PtrToPath(pidlSubItemPtr);
                                foreach (string allowedPath in allowedPaths)
                                    if (allowedPath == path)
                                        includedFolder = true;
                                //if (!includedFolder)
                                //    if (IOTools.HasParent(this, NetworkDirectory))
                                //        includedFolder = true;

                            }
                            if (isDir && !isZip /*&& !isNonEnumerated*/ && (isFileAncestor || includedFolder))
                            {
                                PIDL subPidl = new PIDL(pidlSubItemPtr, true);
                                //DirectoryInfoEx di = new DirectoryInfoEx(this, subPidl);

                                //0.22: Fix illegal PIDL for Directory under Library.ms directory
                                bool isLibraryItem = IOTools.IsLibraryItem(FullName);
                                DirectoryInfoEx di = new DirectoryInfoEx(sf, parentPIDL, subPidl, isLibraryItem);

                                if (IOTools.MatchFileMask(di.Name, searchPattern))
                                    yield return di;
                                if (searchOption == SearchOption.AllDirectories)
                                {
                                    IEnumerator<DirectoryInfoEx> dirEnumerator = di.EnumerateDirectories(searchPattern, searchOption, cancel).GetEnumerator();

                                    while (dirEnumerator.MoveNext())
                                    {
                                        //Debug.Assert(dirEnumerator.Current.IsFolder);
                                        yield return dirEnumerator.Current;
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {

                    if (parentPIDL != null)
                    {
                        parentPIDL.Free();
                        parentPIDL = null;
                    }

                    if (IEnum != null)
                    {
                        Marshal.ReleaseComObject(IEnum);
                        Marshal.Release(ptrEnum);
                    }

                    if (trashPtrList != null)
                        foreach (var ptr in trashPtrList)
                            Marshal.FreeCoTaskMem(ptr); //0.24 : Large memory Leak here.                                

                }
        }
        public IEnumerable<DirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption) { return EnumerateDirectories(searchPattern, searchOption, null); }
        public IEnumerable<DirectoryInfoEx> EnumerateDirectories(String searchPattern) { return EnumerateDirectories(searchPattern, SearchOption.TopDirectoryOnly); }
        public IEnumerable<DirectoryInfoEx> EnumerateDirectories() { return EnumerateDirectories("*", SearchOption.TopDirectoryOnly); }

        public IEnumerable<FileSystemInfoEx> EnumerateFileSystemInfos(String searchPattern, SearchOption searchOption, CancelDelegate cancel)
        {
            IEnumerator<DirectoryInfoEx> dirEnumerator = EnumerateDirectories(searchPattern, SearchOption.TopDirectoryOnly).GetEnumerator();
            while (!IOTools.IsCancelTriggered(cancel) && dirEnumerator.MoveNext())
            {
                yield return dirEnumerator.Current;

                if (searchOption == SearchOption.AllDirectories)
                {
                    IEnumerator<FileSystemInfoEx> fsEnumerator =
                        dirEnumerator.Current.EnumerateFileSystemInfos(searchPattern, searchOption, cancel).GetEnumerator();
                    while (fsEnumerator.MoveNext())
                        yield return fsEnumerator.Current;
                }
            }

            IEnumerator<FileInfoEx> fileEnumerator = EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly, cancel).GetEnumerator();
            while (!IOTools.IsCancelTriggered(cancel) && fileEnumerator.MoveNext())
                yield return fileEnumerator.Current;
        }
        public IEnumerable<FileSystemInfoEx> EnumerateFileSystemInfos(String searchPattern, SearchOption searchOption) { return EnumerateFileSystemInfos(searchPattern, searchOption, null); }
        public IEnumerable<FileSystemInfoEx> EnumerateFileSystemInfos(String searchPattern) { return EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly); }
        public IEnumerable<FileSystemInfoEx> EnumerateFileSystemInfos() { return EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly); }


        #region Obsolute

        //protected virtual bool EnumDirs(out List<FileSystemInfoEx> dirList)
        //{
        //    List<FileSystemInfoEx> _newDirList = new List<FileSystemInfoEx>(); //New items since last cache
        //    List<FileSystemInfoEx> _remDirList = new List<FileSystemInfoEx>(); //Items pending to remove from cache 
        //    _remDirList.AddRange(_cachedDirList);  //Add cache to remove list.

        //    dirList = new List<FileSystemInfoEx>(); //The list to be returned
        //    dirList.AddRange(_cachedDirList); //Add cache to return value list.

        //    IntPtr ptrEnum = IntPtr.Zero;
        //    IEnumIDList IEnum = null;

        //    using (ShellFolder2 sf = this.ShellFolder)
        //        try
        //        {
        //            if (sf.EnumObjects(IntPtr.Zero, folderflag, out ptrEnum) == ShellAPI.S_OK)
        //            {
        //                IEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(ptrEnum, typeof(IEnumIDList));
        //                IntPtr pidlSubItem;
        //                int celtFetched;

        //                while (IEnum.Next(1, out pidlSubItem, out celtFetched) == ShellAPI.S_OK && celtFetched == 1)
        //                {
        //                    ShellAPI.SFGAO attribs = ShellAPI.SFGAO.FOLDER | ShellAPI.SFGAO.FILESYSTEM | ShellAPI.SFGAO.STREAM | ShellAPI.SFGAO.FILESYSANCESTOR;
        //                    sf.GetAttributesOf(1, new IntPtr[] { pidlSubItem }, ref attribs);
        //                    bool isZip = ((attribs & ShellAPI.SFGAO.FOLDER) != 0 && (attribs & ShellAPI.SFGAO.STREAM) != 0);
        //                    bool isFileAncestor = ((attribs & ShellAPI.SFGAO.FILESYSANCESTOR) != 0);
        //                    bool includedFolder = false;
        //                    if (!isZip && !isFileAncestor) //0.14 : Added allowed folder list so Non-FileAncestor directory (e.g. recycle-bin) is listed.
        //                    {
        //                        string[] allowedPaths = new string[]
        //                        {
        //                            "::{645FF040-5081-101B-9F08-00AA002F954E}"
        //                        };
        //                        string path = PIDLToPath(new PIDL(pidlSubItem, false));
        //                        foreach (string allowedPath in allowedPaths)
        //                            if (allowedPath == path)
        //                                includedFolder = true;
        //                        if (!includedFolder)
        //                            if (IOTools.HasParent(this, NetworkDirectory))
        //                                includedFolder = true;

        //                    }
        //                    if (!isZip & (isFileAncestor || includedFolder))
        //                    {
        //                        PIDL subPidl = new PIDL(pidlSubItem, false);

        //                        if (!listContains(_cachedDirList, subPidl)) //if not cache contains the pidl                                                                    
        //                            _newDirList.Add(new DirectoryInfoEx(this, subPidl)); //Add it to pending to add list.

        //                        listRemove(_remDirList, subPidl); //Remove it from pending to remove list.
        //                    }
        //                }
        //                foreach (DirectoryInfoEx dir in _remDirList)
        //                    listRemove(dirList, dir.PIDLRel); //Remove items from pending to remove list.
        //                foreach (DirectoryInfoEx dir in _newDirList)
        //                    dirList.Add(dir); //Add items from pending to add list.
        //                _cachedDirList.Clear();
        //                _cachedDirList.AddRange(dirList); //update cache.

        //                if (dirList.Count > 0 && !HasSubFolder)
        //                    HasSubFolder = true;
        //            }
        //            else return false;
        //        }
        //        //catch (AccessViolationException)
        //        //{
        //        //    if (System.Diagnostics.Debugger.IsAttached)
        //        //        System.Diagnostics.Debugger.Break();
        //        //}
        //        finally
        //        {
        //            if (IEnum != null)
        //            {
        //                Marshal.ReleaseComObject(IEnum);
        //                Marshal.Release(ptrEnum);
        //            }
        //        }
        //    return true;
        //}

        //protected virtual bool EnumFiles(out List<FileSystemInfoEx> fileList)
        //{
        //    //Please refer to EnumDirs for how it works.
        //    List<FileSystemInfoEx> _newFileList = new List<FileSystemInfoEx>();
        //    List<FileSystemInfoEx> _remFileList = new List<FileSystemInfoEx>();
        //    //_remFileList.AddRange(_cachedFileList);

        //    fileList = new List<FileSystemInfoEx>();
        //    //0.17: Removed DirectoryInfoEx file list caching (_cachedFileList) as it slow down if too many files.
        //    //fileList.AddRange(_cachedFileList);

        //    IntPtr ptrEnum = IntPtr.Zero;
        //    IEnumIDList IEnum = null;

        //    using (ShellFolder2 sf = this.ShellFolder)
        //        try
        //        {
        //            if (sf.EnumObjects(IntPtr.Zero, flag, out ptrEnum) == ShellAPI.S_OK)
        //            {
        //                IEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(ptrEnum, typeof(IEnumIDList));
        //                IntPtr pidlSubItem;
        //                int celtFetched;

        //                while (IEnum.Next(1, out pidlSubItem, out celtFetched) == ShellAPI.S_OK && celtFetched == 1)
        //                {

        //                    ShellAPI.SFGAO attribs = ShellAPI.SFGAO.FOLDER | ShellAPI.SFGAO.FILESYSTEM | ShellAPI.SFGAO.STREAM;
        //                    sf.GetAttributesOf(1, new IntPtr[] { pidlSubItem }, ref attribs);
        //                    //http://www.eggheadcafe.com/aspnet_answers/platformsdkshell/Mar2006/post26165601.asp
        //                    bool isZip = ((attribs & ShellAPI.SFGAO.FOLDER) != 0 && (attribs & ShellAPI.SFGAO.STREAM) != 0);
        //                    bool isDir = ((attribs & ShellAPI.SFGAO.FOLDER) != 0);
        //                    if (isZip || !isDir)
        //                    {
        //                        PIDL subRelPidl = new PIDL(pidlSubItem, false);

        //                        //if (!listContains(_cachedFileList, subRelPidl))
        //                        fileList.Add(new FileInfoEx(sf, this, subRelPidl));

        //                        //if (!listContains(_cachedFileList, subRelPidl))
        //                        //    _newFileList.Add(new FileInfoEx(sf, this, subRelPidl));
        //                        ////v0.6 somehow the pidlSubItem does not work as full pidl
        //                        ////     so new FileInfoEx(subRelPidl) doesnt work (return incorrect parent)
        //                        //listRemove(_remFileList, subRelPidl);
        //                    }
        //                }
        //                //foreach (FileInfoEx file in _remFileList)
        //                //    listRemove(fileList, file.PIDLRel);
        //                //foreach (FileInfoEx file in _newFileList)
        //                //    fileList.Add(file);
        //                //_cachedFileList.Clear();
        //                //_cachedFileList.AddRange(fileList);

        //            }

        //            else return false;
        //        }
        //        finally
        //        {
        //            if (IEnum != null)
        //            {
        //                Marshal.ReleaseComObject(IEnum);
        //                Marshal.Release(ptrEnum);
        //            }
        //        }

        //    return true;
        //}
        #endregion

        #region GetXXX
        /// <summary>
        /// Return a list of sub directories and files
        /// </summary>
        public FileSystemInfoEx[] GetFileSystemInfos(String searchPattern, SearchOption searchOption)
        {
            checkExists();
            return new List<FileSystemInfoEx>(EnumerateFileSystemInfos(searchPattern, searchOption)).ToArray();
        }

        public Task<FileSystemInfoEx[]> GetFileSystemInfosAsync(String searchPattern, 
            SearchOption searchOption, CancellationToken ct)
        {
            checkExists();
            return Task.Run(() =>
                new List<FileSystemInfoEx>(EnumerateFileSystemInfos(searchPattern, searchOption, 
                    () => ct.IsCancellationRequested)).ToArray(), ct);
        }

        /// <summary>
        /// Return a list of sub directories and files
        /// </summary>
        public FileSystemInfoEx[] GetFileSystemInfos(String searchPattern)
        {
            checkExists();
            return new List<FileSystemInfoEx>(EnumerateFileSystemInfos(searchPattern)).ToArray();
        }

        /// <summary>
        /// Return a list of sub directories and files
        /// </summary>
        public FileSystemInfoEx[] GetFileSystemInfos()
        {
            checkExists();
            return new List<FileSystemInfoEx>(EnumerateFileSystemInfos()).ToArray();
        }

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        public DirectoryInfoEx[] GetDirectories(String searchPattern, SearchOption searchOption)
        {
            checkExists();
            return new List<DirectoryInfoEx>(EnumerateDirectories(searchPattern, searchOption)).ToArray();
        }

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        public DirectoryInfoEx[] GetDirectories(String searchPattern)
        {
            checkExists();
            return new List<DirectoryInfoEx>(EnumerateDirectories(searchPattern)).ToArray();
        }

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        public DirectoryInfoEx[] GetDirectories()
        {
            checkExists();
            return new List<DirectoryInfoEx>(EnumerateDirectories()).ToArray();
        }

        public Task<DirectoryInfoEx[]> GetDirectoriesAsync(String searchPattern,
          SearchOption searchOption, CancellationToken ct)
        {
            checkExists();
            return Task.Run(() =>
                new List<DirectoryInfoEx>(EnumerateDirectories(searchPattern, searchOption,
                    () => ct.IsCancellationRequested)).ToArray(), ct);
        }

        /// <summary>
        /// Return a list of files in that directory
        /// </summary>
        public FileInfoEx[] GetFiles(String searchPattern, SearchOption searchOption)
        {
            checkExists();
            return new List<FileInfoEx>(EnumerateFiles(searchPattern, searchOption)).ToArray();
        }

        /// <summary>
        /// Return a list of files in that directory
        /// </summary>
        public FileInfoEx[] GetFiles(String searchPattern)
        {
            checkExists();
            return new List<FileInfoEx>(EnumerateFiles(searchPattern)).ToArray();
        }

        /// <summary>
        /// Return a list of files in that directory
        /// </summary>
        public FileInfoEx[] GetFiles()
        {
            checkExists();
            return new List<FileInfoEx>(EnumerateFiles()).ToArray();
        }

        public Task<FileInfoEx[]> GetFilesAsync(String searchPattern,
         SearchOption searchOption, CancellationToken ct)
        {
            checkExists();
            return Task.Run(() =>
                new List<FileInfoEx>(EnumerateFiles(searchPattern, searchOption,
                    () => ct.IsCancellationRequested)).ToArray(), ct);
        }
        #endregion

        private FileSystemInfoEx this[string name, bool lookupDir, bool lookupFile]
        {
            get
            {
                if (lookupDir)
                    foreach (FileSystemInfoEx fsi in GetDirectories())
                        if (fsi.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                            return fsi;
                if (lookupFile)
                    foreach (FileSystemInfoEx fsi in GetFiles())
                        if (fsi.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                            return fsi;
                return null;
            }
        }

        public FileSystemInfoEx this[string name, bool isFile]
        {
            get
            {
                return this[name, !isFile, isFile];
            }
        }

        public FileSystemInfoEx this[string name]
        {
            get
            {
                return this[name, true, true];
            }
        }

        #endregion

        #region Methods - Lookup

        internal bool Contains(string name, out bool isDirectory)
        {
            return IndexOf(name, out isDirectory) != -1;
        }

        internal bool Contains(IntPtr pidl, out bool isDirectory)
        {
            return IndexOf(pidl, out isDirectory) != -1;
        }

        internal int IndexOf(string name, out bool isDirectory)
        {
            isDirectory = true;
            if (!Exists) return -1;

            FileSystemInfoEx[] subFSInfo = GetFileSystemInfos();
            for (int i = 0; i < subFSInfo.Length; i++)
                if (subFSInfo[i].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    isDirectory = subFSInfo[i] is DirectoryInfoEx;
                    return i;
                }

            return -1;
        }

        internal int IndexOf(IntPtr pidl, out bool isDirectory)
        {
            isDirectory = true;
            if (!Exists) return -1;

            FileSystemInfoEx[] subFSInfo = GetFileSystemInfos();
            for (int i = 0; i < subFSInfo.Length; i++)
                if (subFSInfo[i].RequestRelativePIDL(relPidl => relPidl.Equals(pidl)))
                {
                    isDirectory = subFSInfo[i] is DirectoryInfoEx;
                    return i;
                }

            return -1;
        }

        internal DirectoryInfoEx GetSubDirectory(string name)
        {
            DirectoryInfoEx[] subFSInfo = GetDirectories();
            for (int i = 0; i < subFSInfo.Length; i++)
                if (subFSInfo[i].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return (DirectoryInfoEx)subFSInfo[i];

            return null;
        }

        internal DirectoryInfoEx GetSubDirectory(IntPtr pidl)
        {
            DirectoryInfoEx[] subFSInfo = GetDirectories();
            for (int i = 0; i < subFSInfo.Length; i++)
                if (subFSInfo[i].RequestRelativePIDL(relPidl => relPidl.Equals(pidl)))
                    return (DirectoryInfoEx)subFSInfo[i];

            return null;
        }

        internal FileInfoEx GetSubFile(string name)
        {
            FileInfoEx[] subFSInfo = GetFiles();
            for (int i = 0; i < subFSInfo.Length; i++)
                if (subFSInfo[i].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return (FileInfoEx)subFSInfo[i];

            return null;
        }

        internal FileInfoEx GetSubFile(IntPtr pidl)
        {
            FileInfoEx[] subFSInfo = GetFiles();
            for (int i = 0; i < subFSInfo.Length; i++)
                if (subFSInfo[i].RequestRelativePIDL(relPidl => relPidl.Equals(pidl)))
                    return (FileInfoEx)subFSInfo[i];

            return null;
        }
        #endregion

        #region Constructors
        protected override void checkProperties()
        {
            base.checkProperties();
            //if (Exists && (Attributes & FileAttributes.Directory) != FileAttributes.Directory && !HasSubFolder)
            //    throw new IOException(FullName + " is not a folder.");
        }

        internal DirectoryInfoEx(PIDL fullPIDL)
        {
            init(fullPIDL);
            checkProperties();
        }

        public DirectoryInfoEx(string fullPath)
        {
            fullPath = IOTools.ExpandPath(fullPath);
            init(fullPath);
            checkProperties();
        }

        public DirectoryInfoEx(KnownFolder knownFolder)
        {
            PIDL pidlLookup = KnownFolderToPIDL(knownFolder);
            try
            {
                init(pidlLookup);
                checkProperties();
            }
            finally
            {
                //if (pidlLookup != null) pidlLookup.Free();
                pidlLookup = null;
            }
        }

        public DirectoryInfoEx(KnownFolderIds knownFolderId)
            : this(KnownFolder.FromKnownFolderId(
                EnumAttributeUtils<KnownFolderGuidAttribute, KnownFolderIds>.FindAttribute(knownFolderId).Guid))
        {
        }

        public DirectoryInfoEx(ShellAPI.CSIDL csidl)
        {
            PIDL pidlLookup = CSIDLtoPIDL(csidl);
            try
            {
                init(pidlLookup);
                checkProperties();
            }
            finally
            {
                //if (pidlLookup != null) pidlLookup.Free();
                pidlLookup = null;
            }
        }

        public DirectoryInfoEx(Environment.SpecialFolder shellFolder)
        {
            var sf = IOTools.ShellFolderToCSIDL(shellFolder);
            if (!sf.HasValue)
                throw new ArgumentException("Cannot find CSIDL from this shell folder.");
            PIDL pidlLookup = CSIDLtoPIDL(sf.Value);
            try
            {
                init(pidlLookup);
                checkProperties();
            }
            finally
            {
                //if (pidlLookup != null) pidlLookup.Free();
                pidlLookup = null;
            }
        }

        public DirectoryInfoEx(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _dirType = (DirectoryTypeEnum)info.GetValue("DirectoryType", typeof(DirectoryTypeEnum));
            IsBrowsable = info.GetBoolean("IsBrowsable");
            IsFileSystem = info.GetBoolean("IsFileSystem");
            HasSubFolder = info.GetBoolean("HasSubFolder");
        }

        internal DirectoryInfoEx(IShellFolder2 parentShellFolder, PIDL fullPIDL)
        {
            init(parentShellFolder, fullPIDL);
        }

        internal DirectoryInfoEx(IShellFolder2 parentShellFolder, PIDL parentPIDL, PIDL relPIDL,
            bool parentIsLibrary)
        {
            init(parentShellFolder, parentPIDL, relPIDL);

            //0.22: Fix illegal PIDL for Directory under Library.ms directory
            if (parentIsLibrary)
            {
                init(FullName);
            }
        }

        internal DirectoryInfoEx(DirectoryInfoEx parentDir, PIDL relPIDL)
        {
            Parent = parentDir;
            parentDir.RequestPIDL(parentPIDL =>
                {
                    //0.15: Fixed ShellFolder not freed.
                    using (ShellFolder2 parentShellFolder = parentDir.ShellFolder)
                        init(parentShellFolder, parentPIDL, relPIDL);
                });
        }

        protected DirectoryInfoEx()
        {

        }
        #endregion

        #region IDisposable Members

        ~DirectoryInfoEx()
        {
            ((IDisposable)this).Dispose();
        }

        public new void Dispose()
        {
            base.Dispose();
            //if (iShellFolder != null) iShellFolder.Dispose();
            //if (iStorage != null) iStorage.Dispose();
            //iShellFolder = null;
            //iStorage = null;
        }

        #endregion

        #region ISerializable Members
        protected override void getObjectData(SerializationInfo info, StreamingContext context)
        {
            base.getObjectData(info, context);
            info.AddValue("DirectoryType", DirectoryType);
            info.AddValue("IsBrowsable", IsBrowsable);
            info.AddValue("IsFileSystem", IsFileSystem);
            info.AddValue("HasSubFolder", HasSubFolder);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            getObjectData(info, context);
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return new DirectoryInfoEx(this.FullName);
        }

        #endregion
    }
}
