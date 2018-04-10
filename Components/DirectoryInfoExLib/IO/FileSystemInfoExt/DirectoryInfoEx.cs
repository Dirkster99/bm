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
    using DirectoryInfoExLib.IO.Tools;
    using System.Text.RegularExpressions;

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
        /// <summary>
        /// Defines the (CLSID Guid) path of the Desktop shell item.
        /// </summary>
        internal const string IID_Desktop = "::{00021400-0000-0000-C000-000000000046}";

        /// <summary>
        /// Defines the (CLSID Guid) path of the User Files shell item.
        /// </summary>
        internal const string IID_UserFiles = "::{59031A47-3F72-44A7-89C5-5595FE6b30EE}";

        /// <summary>
        /// Defines the (CLSID Guid) path of the Public shell item.
        /// </summary>
        internal const string IID_Public = "::{4336A54D-038B-4685-AB02-99BB52D3FB8B}";

        /// <summary>
        /// Defines the (PLID Guid) path of the Libraries shell item.
        /// </summary>
        internal const string IID_Library = "::{031E4825-7B94-4DC3-B131-E946B44C8DD5}";

        /// <summary>
        /// Defines the (CLSID Guid) path of the Recycle Bin shell item.
        /// </summary>
        private const string IDD_RecycleBin = "::{645FF040-5081-101B-9F08-00AA002F954E}";

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
            try
            {
                SharedDirectory = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.PublicDocuments));
            }
            catch { }

            NetworkDirectory = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Network));
            RecycleBinDirectory = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.RecycleBin));
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
            fullPath = ExpandPath(fullPath);
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
        /// <param name="parentShellFolder"></param>
        /// <param name="parentPIDL"></param>
        /// <param name="relPIDL"></param>
        /// <param name="parentIsLibrary"></param>
        protected DirectoryInfoEx(IShellFolder2 parentShellFolder,
                                  PIDL parentPIDL,
                                  PIDL relPIDL,
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

        private ShellFolder2 ShellFolder { get { return getIShellFolder(); } }
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

        /// <summary>
        /// Returns whether this directory item is
        /// the same as the <paramref name="other"/> or not.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
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
                if (dirObj.FullName.Equals(DirectoryInfoEx.IID_UserFiles))
                    if (this.FullName.Equals(DirectoryInfoEx.CurrentUserDirectory.FullName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                if (dirObj.FullName.Equals(DirectoryInfoEx.IID_Public))
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
            if (this.FullName.Equals(DirectoryInfoEx.IID_Desktop))
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
        #endregion

        #region Methods - GetSubItems
        /// <summary>
        /// Enumerates all items in this directory with the given search parameters.
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public IEnumerable<IDirectoryInfoEx> EnumerateDirectories(
            String searchPattern,
            SearchOption searchOption,
            CancelDelegate cancel)
        {
            const ShellAPI.SHCONTF flag = ShellAPI.SHCONTF.NONFOLDERS | ShellAPI.SHCONTF.FOLDERS | ShellAPI.SHCONTF.INCLUDEHIDDEN;
            IntPtr ptrEnum = IntPtr.Zero;
            IEnumIDList IEnum = null;
            PIDL parentPIDL = this.getPIDL();

            List<IntPtr> trashPtrList = new List<IntPtr>();
            using (ShellFolder2 sf = this.ShellFolder)
            {
                try
                {
                    if (sf.EnumObjects(IntPtr.Zero, flag, out ptrEnum) == ShellAPI.S_OK)
                    {
                        // Initialize query for sub-items below this item (folder)
                        IEnum = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(ptrEnum, typeof(IEnumIDList));
                        IntPtr pidlSubItemPtr;
                        int celtFetched;

                        // Process one item at a time sub-items below this item (folder)
                        while (!IsCancelTriggered(cancel) &&
                               IEnum.Next(1, out pidlSubItemPtr, out celtFetched) == ShellAPI.S_OK &&
                               celtFetched == 1)
                        {
                            trashPtrList.Add(pidlSubItemPtr); //0.24 : Large memory Leak here.                                

                            ShellAPI.SFGAO attribs = ShellAPI.SFGAO.FOLDER |
                                                     ShellAPI.SFGAO.FILESYSTEM |
                                                     ShellAPI.SFGAO.STREAM |
                                                     ShellAPI.SFGAO.FILESYSANCESTOR |
                                                     ShellAPI.SFGAO.NONENUMERATED;

                            sf.GetAttributesOf(1, new IntPtr[] { pidlSubItemPtr }, ref attribs);

                            bool isZip = ((attribs & ShellAPI.SFGAO.FOLDER) != 0 && (attribs & ShellAPI.SFGAO.STREAM) != 0);
                            bool isDir = ((attribs & ShellAPI.SFGAO.FOLDER) != 0);
                            //0.18 Added a check for NonEnumerated items so DirectoryInfoEx.EnumerateDirectories wont return some system directories (e.g. C:\MSOCache)
                            //bool isNonEnumerated = ((attribs & ShellAPI.SFGAO.NONENUMERATED) != 0);
                            bool isFileAncestor = ((attribs & ShellAPI.SFGAO.FILESYSANCESTOR) != 0);
                            bool includedFolder = false;

                            if (!isZip && !isFileAncestor) //0.14 : Added allowed folder list so Non-FileAncestor directory (e.g. recycle-bin) is listed.
                            {
                                string[] allowedPaths = new string[]{ IDD_RecycleBin };

                                string path = PtrToPath(pidlSubItemPtr);
                                foreach (string allowedPath in allowedPaths)
                                {
                                    if (allowedPath == path)
                                        includedFolder = true;
                                }
                            }

                            if (isDir && !isZip /*&& !isNonEnumerated*/ && (isFileAncestor || includedFolder))
                            {
                                PIDL subPidl = new PIDL(pidlSubItemPtr, true);
                                //DirectoryInfoEx di = new DirectoryInfoEx(this, subPidl);

                                //0.22: Fix illegal PIDL for Directory under Library.ms directory
                                bool isLibraryItem = Factory.IsLibraryItem(FullName);
                                DirectoryInfoEx di = new DirectoryInfoEx(sf, parentPIDL, subPidl, isLibraryItem);

                                if (this.MatchFileMask(di.Name, searchPattern))
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
                            Marshal.FreeCoTaskMem(ptr);    // 0.24 : Large memory Leak here.
                }
            }
        }

        /// <summary>
        /// Enumerates all items in this directory with the given search parameters.
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption)
        {
            return EnumerateDirectories(searchPattern, searchOption, null);
        }

        /// <summary>
        /// Enumerates all items in this directory with the given search parameters.
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern)
        {
            return EnumerateDirectories(searchPattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Enumerates all items in this directory with the given search parameters.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDirectoryInfoEx> EnumerateDirectories()
        {
            return EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
        }

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

        #region IDisposable Members
        /// <summary>
        /// Implements standard disposable method.
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
        }
        #endregion

        #region Match File Mask Filter
        /// <summary>
        /// Return whether PIDL match fileMask ( * and ? supported)
        /// </summary>
        /// <param name="pidl"></param>
        /// <param name="fileMask"></param>
        /// <returns></returns>
        public bool MatchFileMask(PIDL pidl, string fileMask)
        {
            string path = DirectoryInfoEx.PIDLToPath(pidl);
            string name = PathEx.GetFileName(path);
            return MatchFileMask(name, fileMask);
        }

        /// <summary>
        /// Return whether filename match fileMask ( * and ? supported)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileMask"></param>
        /// <returns></returns>
        public bool MatchFileMask(string fileName, string fileMask)
        {
            return MatchFileMask(fileName, fileMask, false);
        }

        /// <summary>
        /// http://stackoverflow.com/questions/725341/c-file-mask
        /// Return whether filename match fileMask ( * and ? supported)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileMask"></param>
        /// <returns></returns>
        protected bool MatchFileMask(string fileName, string fileMask, bool forceSlashCheck)
        {
            if (fileName.Equals(fileMask, StringComparison.InvariantCultureIgnoreCase))
                return true;
            if (fileMask == "*.*" || fileMask == "*" && !forceSlashCheck)
                return true;

            string pattern = constructFileMaskRegexPattern(fileMask, forceSlashCheck);

            return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(fileName);
        }

        public bool MatchFileMask(string fileName, string fileMask, bool forceSlashCheck, out Match match)
        {
            string pattern = constructFileMaskRegexPattern(fileMask, forceSlashCheck);
            match = new Regex(pattern, RegexOptions.IgnoreCase).Match(fileName);
            return match.Success;
        }

        public bool MatchFileMasks(string fileName, string fileMasks)
        {
            string[] fileMaskList = fileMasks.Split(new char[] { ',', ';' });
            foreach (string fileMask in fileMaskList)
                if (MatchFileMask(fileName, fileMask))
                    return true;
            return false;
        }

        protected string constructFileMaskRegexPattern(string fileMask, bool forceSlashCheck)
        {
            if (!forceSlashCheck)
            {
                return '^' +
                Regex.Escape(fileMask.Replace(".", "__DOT__")
                                .Replace("*", "__STAR__")
                                .Replace("?", "__QM__"))
                    .Replace("__DOT__", "[.]")
                    .Replace("__STAR__", ".*")
                    .Replace("__QM__", ".")
                + '$';
            }
            else
            {
                return '^' +
                 Regex.Escape(fileMask.Replace(".", "__DOT__")
                                 .Replace("\\", "__SLASH__")
                                 .Replace("**", "__DOUBLESTAR__")
                                 .Replace("*", "__STAR__")
                                 .Replace("#", "__VARIABLE__")
                                 .Replace("(", "__OPENQUOTE__")
                                 .Replace(")", "__CLOSEQUOTE__")
                                 .Replace("?", "__QM__"))
                     .Replace("__DOT__", "[.]")
                     .Replace("__DOUBLESTAR__", ".*")
                     .Replace("__STAR__", "[^\\\\]*")
                     .Replace("__SLASH__", "[\\\\]")
                     .Replace("__VARIABLE__", "?")
                     .Replace("__OPENQUOTE__", "(")
                     .Replace("__CLOSEQUOTE__", ")")
                     .Replace("__QM__", ".")
                 + '$';
            }
        }
        #endregion Match File Mask Filter

        protected override void checkProperties()
        {
            base.checkProperties();
            //if (Exists && (Attributes & FileAttributes.Directory) != FileAttributes.Directory && !HasSubFolder)
            //    throw new IOException(FullName + " is not a folder.");
        }

        private bool IsCancelTriggered(CancelDelegate cancel)
        {
            return cancel != null && cancel();
        }

        /// <summary>
        /// Expand environment path to parasable path.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private string ExpandPath(string fullPath)
        {
            fullPath = RemoveSlash(fullPath);

            if (fullPath.StartsWith("%"))
                fullPath = Environment.ExpandEnvironmentVariables(fullPath);

            return fullPath;
        }
    }
}
