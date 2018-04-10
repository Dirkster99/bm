///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2009 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Steven Roebert's work (http://www.codeproject.com/KB/miscctrl/FileBrowser.aspx)    //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace DirectoryInfoExLib.IO.FileSystemInfoExt
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Threading;
    using DirectoryInfoExLib.IO.Header.ShellDll;
    using DirectoryInfoExLib.Interfaces;
    using DirectoryInfoExLib.IO.Tools.Interface;
    using DirectoryInfoExLib.Tools;
    using System.IO;
    using DirectoryInfoExLib.IO.Header;
    using DirectoryInfoExLib.IO.Header.ShellDll.Interfaces;
    using DirectoryInfoExLib.IO.Header.KnownFolder;
    using DirectoryInfoExLib.IO.Header.KnownFolder.Enums;
    using DirectoryInfoExLib.Enums;

    /// <summary>
    /// Represents a directory in PIDL (Pointer to an ID List) system.
    ///
    /// https://www.codeproject.com/Articles/1649/The-Complete-Idiot-s-Guide-to-Writing-Namespace-Ex
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/cc144090(v=vs.85).aspx
    /// </summary>
    internal class DirectoryInfoEx : FileSystemInfoEx, IDirectoryInfoEx
    {
        #region fields
        private DirectoryTypeEnum _dirType;

        #region Static fields
        internal static readonly DirectoryInfoEx DesktopDirectory;
        internal static readonly DirectoryInfoEx MyComputerDirectory;
        internal static readonly DirectoryInfoEx CurrentUserDirectory;
        internal static readonly DirectoryInfoEx SharedDirectory;
        internal static readonly DirectoryInfoEx NetworkDirectory;
        internal static readonly DirectoryInfoEx RecycleBinDirectory;
        #endregion Static fields
        #endregion fields

        #region constructors
        /// <summary>
        /// Static constructor
        /// </summary>
        static DirectoryInfoEx()
        {
#if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                throw new Exception("This should not be executed when design time.");
#endif
            DesktopDirectory = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Desktop));
            MyComputerDirectory = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Computer));
            CurrentUserDirectory = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.UsersFiles));
            //0.17: Fixed some system cannot create shared directories. (by cwharmon)
            try { SharedDirectory = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.PublicDocuments)); }
            catch { }
            NetworkDirectory = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Network));
            RecycleBinDirectory = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.RecycleBin));

            //foreach (DirectoryInfoEx dir in DesktopDirectory.GetDirectories())
            //    if (dir.FullName.Equals(Helper.GetCurrentUserPath()))
            //    { CurrentUserDirectory = dir; break; }

            //foreach (DirectoryInfoEx dir in DesktopDirectory.GetDirectories())
            //    if (dir.FullName.Equals(Helper.GetSharedPath()))
            //    { SharedDirectory = dir; break; }
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        internal DirectoryInfoEx(PIDL fullPIDL)
        {
            init(fullPIDL);
            checkProperties();
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="fullPath"></param>
        public DirectoryInfoEx(string fullPath)
        {
            fullPath = IOTools.ExpandPath(fullPath);
            init(fullPath);
            checkProperties();
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="knownFolder"></param>
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

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
////        public DirectoryInfoEx(SerializationInfo info, StreamingContext context)
////            : base(info, context)
////        {
////            _dirType = (DirectoryTypeEnum)info.GetValue("DirectoryType", typeof(DirectoryTypeEnum));
////            IsBrowsable = info.GetBoolean("IsBrowsable");
////            IsFileSystem = info.GetBoolean("IsFileSystem");
////            HasSubFolder = info.GetBoolean("HasSubFolder");
////        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="parentShellFolder"></param>
        /// <param name="fullPIDL"></param>
        internal DirectoryInfoEx(IShellFolder2 parentShellFolder, PIDL fullPIDL)
        {
            init(parentShellFolder, fullPIDL);
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="parentShellFolder"></param>
        /// <param name="parentPIDL"></param>
        /// <param name="relPIDL"></param>
        /// <param name="parentIsLibrary"></param>
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

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="parentDir"></param>
        /// <param name="relPIDL"></param>
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

        /// <summary>
        /// Hidden paremeterless class constructor
        /// </summary>
        protected DirectoryInfoEx()
        {
        }

        ~DirectoryInfoEx()
        {
            ((IDisposable)this).Dispose();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the root of this item.
        /// </summary>
        public IDirectoryInfoEx Root { get { return getDirectoryRoot(this); } }

        /// <summary>
        /// Gets the folders type classification.
        /// </summary>
        public DirectoryTypeEnum DirectoryType
        {
            get { return _dirType; }
            protected set { _dirType = value; }
        }

        /// <summary>
        /// Gets the Windows known folder (similar to <see cref="Environment.SpecialFolder"/>
        /// but extensible and customizable at run-time) or null if this folder
        /// is not a special folder in Windows.
        /// </summary>
        /// <returns></returns>
        public KnownFolder KnownFolderType
        {
            get { return this.RequestPIDL(pidl => KnownFolder.FromPidl(pidl)); }
        }

        internal ShellFolder2 ShellFolder { get { return getIShellFolder(); } }
        #endregion

        #region Static Methods
        internal static PIDL KnownFolderToPIDL(KnownFolder knownFolder)
        {
            IntPtr ptrAddr;
            PIDL pidl;

            int RetVal = (knownFolder.KnownFolderInterface).GetIDList(KnownFolderRetrievalOptions.Create, out ptrAddr);
            if (ptrAddr != IntPtr.Zero)
            {
                pidl = new PIDL(ptrAddr, false);
                return pidl;
            }
            else throw new ArgumentException("Invalid knownFolder " + RetVal);
        }

        /// <summary>
        /// Takes a directoryInfoEx and return the first parent with directory type = desktop or drive.
        /// </summary>
        internal static IDirectoryInfoEx getDirectoryRoot(IDirectoryInfoEx lookup)
        {
            IDirectoryInfoEx dir = lookup.Parent;

            while (dir.DirectoryType != DirectoryTypeEnum.dtDesktop &&
                dir.DirectoryType != DirectoryTypeEnum.dtDrive &&
////                dir.DirectoryType != DirectoryTypeEnum.dtRoot &&
                dir != null)
                dir = dir.Parent;

            if (dir == null)
                throw new IOException("Internal exception in GetDirectoryRoot.");

            return dir;
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Implements the <see cref="ICloneable"/> interface.
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return new DirectoryInfoEx(this.FullName);
        }
        #endregion

        #region Methods
        public T RequestPIDL<T>(Func<PIDL, PIDL, T> pidlAndRelPidlFunc)
        {
            PIDL pidl = this.getPIDL();
            PIDL relPidl = this.getRelPIDL();
            try
            {
                return pidlAndRelPidlFunc(pidl, relPidl);
            }
            finally
            {
                pidl.Free();
                relPidl.Free();
            }
        }

        public T RequestPIDL<T>(Func<PIDL, T> pidlFuncOnly)
        {
            PIDL pidl = this.getPIDL();
            try
            {
                return pidlFuncOnly(pidl);
            }
            finally
            {
                pidl.Free();
            }
        }

        public void RequestPIDL(Action<PIDL> pidlFuncOnly)
        {
            PIDL pidl = this.getPIDL();
            try
            {
                pidlFuncOnly(pidl);
            }
            finally
            {
                pidl.Free();
            }
        }

        public T RequestRelativePIDL<T>(Func<PIDL, T> relPidlFuncOnly)
        {
            PIDL relPidl = this.getRelPIDL();
            try
            {
                return relPidlFuncOnly(relPidl);
            }
            finally
            {
                relPidl.Free();
            }
        }

        public bool Equals(IDirectoryInfoEx other)
        {
            return Equals(other as FileSystemInfoEx);
        }

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

        /// <summary>
        /// Delete this folder. (not move it to recycle bin)
        /// </summary>
        public override void Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Move this folder to specified directory (fullpath)
        /// </summary>
        public void MoveTo(string destDirName)
        {
            throw new NotImplementedException();
        }

        protected void checkExists()
        {
            //0.18: checkExists() ignore network directory.
            if (!FullName.StartsWith("\\") && !Exists)
                throw new DirectoryNotFoundException(FullName + " does not exist.");
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

        private ShellFolder2 getIShellFolderFromParent()
        {
            if (_parent != null)
                using (ShellFolder2 parentShellFolder = (_parent as DirectoryInfoEx).ShellFolder)
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

////    protected void initDirectoryType()
////    {
////        _dirType = DirectoryTypeEnum.dtFolder;   // default value
////
////        string path = FullName != null ? FullName : OriginalPath;
////
////        if (path != null)
////        {
////            if (path.Equals(IOTools.IID_Desktop))
////                _dirType = DirectoryTypeEnum.dtDesktop;
////            else
////            if (path.EndsWith(":\\"))
////            {
////                _dirType = DirectoryTypeEnum.dtDrive;
////            }
////            else
////            {
////                if (path.StartsWith("::"))
////                    _dirType = DirectoryTypeEnum.dtSpecial;
////                else
////                    _dirType = DirectoryTypeEnum.dtFolder;
////            }
////        }
////    }
        #endregion

        #region Methods - GetSubItems
        static ShellAPI.SHCONTF flag = ShellAPI.SHCONTF.NONFOLDERS | ShellAPI.SHCONTF.FOLDERS | ShellAPI.SHCONTF.INCLUDEHIDDEN;
////    //static ShellAPI.SHCONTF folderflag = ShellAPI.SHCONTF.FOLDERS | ShellAPI.SHCONTF.INCLUDEHIDDEN;
////    //static ShellAPI.SHCONTF fileflag = ShellAPI.SHCONTF.NONFOLDERS | ShellAPI.SHCONTF.INCLUDEHIDDEN;

////    private List<FileSystemInfoEx> _cachedDirList = new List<FileSystemInfoEx>();
////    //private List<FileSystemInfoEx> _cachedFileList = new List<FileSystemInfoEx>();
////
////    private static bool listContains(List<FileSystemInfoEx> list, PIDL pidl)
////    {
////        foreach (FileSystemInfoEx item in list)
////            if (item.RequestRelativePIDL((relPidl) => relPidl.Equals(pidl)))
////                return true;
////
////        return false;
////    }
////    private static void listRemove(List<FileSystemInfoEx> list, PIDL pidl)
////    {
////        for (int i = list.Count - 1; i >= 0; i--)
////            if (list[i].RequestRelativePIDL((relPidl) => relPidl.Equals(pidl)))
////            { list.RemoveAt(i); return; }
////    }

        public IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption, CancelDelegate cancel)
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
                                    IEnumerator<IDirectoryInfoEx> dirEnumerator = di.EnumerateDirectories(searchPattern, searchOption, cancel).GetEnumerator();

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
        public IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption) { return EnumerateDirectories(searchPattern, searchOption, null); }
        public IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern) { return EnumerateDirectories(searchPattern, SearchOption.TopDirectoryOnly); }
        public IEnumerable<IDirectoryInfoEx> EnumerateDirectories() { return EnumerateDirectories("*", SearchOption.TopDirectoryOnly); }

        #region GetXXX
        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        public IDirectoryInfoEx[] GetDirectories(String searchPattern, SearchOption searchOption)
        {
            checkExists();
            return new List<IDirectoryInfoEx>(EnumerateDirectories(searchPattern, searchOption)).ToArray();
        }

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        public IDirectoryInfoEx[] GetDirectories(String searchPattern)
        {
            checkExists();
            return new List<IDirectoryInfoEx>(EnumerateDirectories(searchPattern)).ToArray();
        }

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        public IDirectoryInfoEx[] GetDirectories()
        {
            checkExists();
            return new List<IDirectoryInfoEx>(EnumerateDirectories()).ToArray();
        }

        public Task<IDirectoryInfoEx[]> GetDirectoriesAsync(String searchPattern,
          SearchOption searchOption, CancellationToken ct)
        {
            checkExists();
            return Task.Run(() =>
                new List<IDirectoryInfoEx>(EnumerateDirectories(searchPattern, searchOption,
                    () => ct.IsCancellationRequested)).ToArray(), ct);
        }
        #endregion

        public static IDirectoryInfoEx FromString(string FullName)
        {
            return (DirectoryInfoEx.DirectoryExists(FullName) ?
                new DirectoryInfoEx(FullName) : null);
        }
        #endregion

////        #region Methods - Lookup
////        internal IDirectoryInfoEx GetSubDirectory(string name)
////        {
////            IDirectoryInfoEx[] subFSInfo = GetDirectories();
////            for (int i = 0; i < subFSInfo.Length; i++)
////                if (subFSInfo[i].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
////                    return (DirectoryInfoEx)subFSInfo[i];
////
////            return null;
////        }
////
////        internal IDirectoryInfoEx GetSubDirectory(IntPtr pidl)
////        {
////            IDirectoryInfoEx[] subFSInfo = GetDirectories();
////            for (int i = 0; i < subFSInfo.Length; i++)
////                if ((subFSInfo[i] as DirectoryInfoEx).RequestRelativePIDL(relPidl => relPidl.Equals(pidl)))
////                    return (DirectoryInfoEx)subFSInfo[i];
////
////            return null;
////        }
////        #endregion

        protected override void checkProperties()
        {
            base.checkProperties();
            //if (Exists && (Attributes & FileAttributes.Directory) != FileAttributes.Directory && !HasSubFolder)
            //    throw new IOException(FullName + " is not a folder.");
        }

        #region IDisposable Members
        public new void Dispose()
        {
            base.Dispose();
        }
        #endregion

////        #region ISerializable Members
////        protected override void getObjectData(SerializationInfo info, StreamingContext context)
////        {
////            base.getObjectData(info, context);
////            info.AddValue("DirectoryType", DirectoryType);
////            info.AddValue("IsBrowsable", IsBrowsable);
////            info.AddValue("IsFileSystem", IsFileSystem);
////            info.AddValue("HasSubFolder", HasSubFolder);
////        }
////
////        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
////        {
////            getObjectData(info, context);
////        }
////        #endregion ISerializable Members
    }
}
