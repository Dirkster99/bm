///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2009 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Steven Roebert's work (http://www.codeproject.com/KB/miscctrl/FileBrowser.aspx)        //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace DirectoryInfoExLib.IO
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
    using System.IO;
    using DirectoryInfoExLib.IO.Header;
    using DirectoryInfoExLib.IO.Header.ShellDll.Interfaces;
    using DirectoryInfoExLib.IO.Header.KnownFolder;
    using DirectoryInfoExLib.IO.Header.KnownFolder.Enums;
    using DirectoryInfoExLib.Enums;
    using System.Text.RegularExpressions;
    using System.Text;

    /// <summary>
    /// Represents a directory in PIDL (Pointer to an ID List) system.
    ///
    /// https://www.codeproject.com/Articles/1649/The-Complete-Idiot-s-Guide-to-Writing-Namespace-Ex
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/cc144090(v=vs.85).aspx
    /// </summary>
    internal class DirectoryInfoEx : IDirectoryInfoEx, IEquatable<IDirectoryInfoEx>
    {
        #region fields
        public static int counter = 0;

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
        internal const string IDD_RecycleBin = "::{645FF040-5081-101B-9F08-00AA002F954E}";

        internal static readonly DirectoryInfoEx DesktopDirectory;
        internal static readonly DirectoryInfoEx MyComputerDirectory;
        internal static readonly DirectoryInfoEx CurrentUser;
        internal static readonly DirectoryInfoEx SharedDirectory;
        internal static readonly DirectoryInfoEx NetworkDirectory;
        internal static readonly DirectoryInfoEx RecycleBinDirectory;
        #endregion Static fields

        private string _name;
        private PIDL _pidlRel = null;
        private PIDL _pidl = null;

        private IDirectoryBrowser _parent;     // Get the parent obejct of this object
        private bool _parentInited = false;    // Is parent property initialized?

        private bool _disposed = false;        // Implement IDisposable interface
        #endregion fields

        #region constructors
        /// <summary>
        /// Static constructor
        /// </summary>
        static DirectoryInfoEx()
        {
#if DEBUG
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                throw new Exception("This should not be executed at design time.");
#endif

            var desktopId = KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Desktop);
            CurrentUser = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.UsersFiles));
            RecycleBinDirectory = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.RecycleBin));
            DesktopDirectory = new DirectoryInfoEx(desktopId);
            MyComputerDirectory = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Computer));

            //0.17: Fixed some system cannot create shared directories. (by cwharmon)
            try
            {
                SharedDirectory = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.PublicDocuments));
            }
            catch { }

            NetworkDirectory = new DirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Network));
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        public DirectoryInfoEx(PIDL fullPIDL)
            : this()
        {
            init(fullPIDL);
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="fullPath"></param>
        public DirectoryInfoEx(string fullPath)
            : this()
        {
            fullPath = ExpandPath(fullPath);
            init(fullPath);
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="knownFolder"></param>
        public DirectoryInfoEx(KnownFolder knownFolder)
            : this()
        {
            PIDL pidlLookup = KnownFolderToPIDL(knownFolder);
            try
            {
                init(pidlLookup);
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
            : this()
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
            System.Threading.Interlocked.Increment(ref counter);
        }

        /// <summary>
        /// Class finalizer/destructor
        /// When the object is eligible for finalization,
        /// the garbage collector runs the Finalize method of the object. 
        /// </summary>
        ~DirectoryInfoEx()
        {
            Dispose();
        }

        protected void init(PIDL fullPIDL)
        {
            PIDL relPIDL = null;
            //0.16: Fixed ShellFolder not freed
            using (ShellFolder2 parentShellFolder = getParentIShellFolder(fullPIDL, out relPIDL))
            {
                refresh(parentShellFolder, relPIDL, fullPIDL);
            }

            _pidl = new PIDL(fullPIDL, false); //0.14 : FileSystemInfoEx record the pidl when construct, as some path do not parasable (e.g. EntireNetwork)
            _pidlRel = relPIDL;
        }

        protected void init(IShellFolder2 parentShellFolder, PIDL parentPIDl, PIDL relPIDL)
        {
            PIDL fullPIDL = new PIDL(PIDL.ILCombine(parentPIDl.Ptr, relPIDL.Ptr), false);

            refresh(parentShellFolder, relPIDL, fullPIDL);
            _pidl = fullPIDL;
            _pidlRel = relPIDL; // new PIDL(relPIDL, false);
        }

        protected void init(string path)
        {
            //0.22: Fix illegal PIDL for Directory under Library.ms directory
            _pidl = null;
            _pidlRel = null;
            FullName = path;
            Refresh();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the Label of this object. A label is a descriptive but short text
        /// that specifies an item in more detail. Typically, local drives or mapped
        /// drives have a label to specify the contents of that drive (e.g.: 'Data').
        /// </summary>
        public string Label { get; protected set; }

        /// <summary>
        /// Gets the name including path and extensions of this object.
        /// </summary>
        public string FullName { get; protected set; }


        /// <summary>
        /// Gets the file system attributes for this object.
        /// </summary>
        public FileAttributes Attributes
        {
            get; protected set;
        }

        /// <summary>
        /// Gets/sets the parent folder item of this item.
        /// </summary>
        public IDirectoryBrowser Parent
        {
            get
            {
                return initParent();
            }

            protected set
            {
                if (_parent != null)
                    throw new Exception("_parent cannot be reset.");

                _parent = value;
            }
        }
        /// <summary>
        /// Gets the name of this item's path.
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Gets the extension of this item's name.
        /// </summary>
        public string Extension { get { return Path.GetExtension(Name); } }

        /// <summary>
        /// Gets whether this item represents a folder or not.
        /// </summary>
        public bool IsFolder
        {
            get { return (Attributes & FileAttributes.Directory) == FileAttributes.Directory; }
        }

        public bool Exists { get { return getExists(); } }

        /// <summary>
        /// Gets the root of this item.
        /// </summary>
        public IDirectoryInfoEx Root { get { return getDirectoryRoot(this) as IDirectoryInfoEx; } }

        /// <summary>
        /// Gets the folders type classification.
        /// </summary>
        public DirectoryTypeEnum DirectoryType { get; protected set; }

        /// <summary>
        /// Gets the Windows known folder (similar to <see cref="Environment.SpecialFolder"/>
        /// but extensible and customizable at run-time) or null if this folder
        /// is not a special folder in Windows.
        /// </summary>
        /// <returns></returns>
        protected KnownFolder KnownFolderType
        {
            get
            {
                PIDL pidl = this.getPIDL();
                try
                {
                    return KnownFolder.FromPidl(pidl);
                }
                finally
                {
                    pidl.Free();
                }
            }
        }
        #endregion

        #region Methods
        #region Static Methods
        /// <summary>
        /// Gets whether a directory exists at a given path or not.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool DirectoryExists(string path)
        {
            try
            {
                DirectoryInfoEx dirInfo = new DirectoryInfoEx(path);

                return dirInfo != null && dirInfo.IsFolder && dirInfo.Exists;
            }
            catch { return false; }
        }

        /// <summary>
        /// Gets the <see cref="IDirectoryInfoEx"/> interface
        /// for the string representation in <paramref name="path"/>
        /// or null if directory does not exist.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IDirectoryInfoEx FromString(string path)
        {
            return (DirectoryInfoEx.DirectoryExists(path) ? new DirectoryInfoEx(path) : null);
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

        #region IDisposable Members
        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Implements standard disposable method.
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    if (_pidlRel != null || _pidl != null)
                        System.Threading.Interlocked.Decrement(ref counter);

                    if (_pidlRel != null) _pidlRel.Free();

                    if (_pidl != null) _pidl.Free();

                    _pidlRel = null;
                    _pidl = null;
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            _disposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }
        #endregion

        /// <summary>
        /// Refresh the file / directory info. Does not refresh directory contents 
        /// because it refresh every time GetFiles/Directories is called.
        /// </summary>
        public void Refresh()
        {
            PIDL relPIDL = null;
            if (getExists() == false)
                refresh(null, null, null);
            else
                try
                {
                    //0.16: Fixed ShellFolder not freed
                    PIDL pidlLookup = this.getPIDL();
                    try
                    {
                        using (ShellFolder2 sf = getParentIShellFolder(pidlLookup, out relPIDL))
                        {
                            refresh(sf, relPIDL, pidlLookup);
                        }
                    }
                    finally
                    {
                        pidlLookup.Free();
                    }
                }
                catch (NullReferenceException)
                {
                    refresh(null, null, null);
                }
                finally
                {
                    if (relPIDL != null)
                        relPIDL.Free();
                    relPIDL = null;
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
            PIDL thisPidl = this.getPIDL();
            PIDL otherPidl = (other as DirectoryInfoEx).getPIDL();
            try
            {
                return thisPidl.Equals(otherPidl);
            }
            finally
            {
                thisPidl.Free();
                otherPidl.Free();
            }
        }

        public bool Equals(DirectoryInfoEx other)
        {
            if (this.FullName.Equals(other.FullName, StringComparison.InvariantCultureIgnoreCase))
                return true;

            DirectoryInfoEx dirObj = other as DirectoryInfoEx;
            if (dirObj.FullName.Equals(DirectoryInfoEx.IID_UserFiles))
                if (this.FullName.Equals(DirectoryInfoEx.CurrentUser.FullName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            if (dirObj.FullName.Equals(DirectoryInfoEx.IID_Public))
                if (this.FullName.Equals(DirectoryInfoEx.SharedDirectory.FullName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return FullName.ToLower().GetHashCode();
        }

        public IntPtr GetPIDLIntPtr()
        {
            if (_pidl != null)
                return PIDL.ILClone(_pidl.Ptr);

            if (FullName == DirectoryInfoEx.IID_Desktop) // Desktop
                return DirectoryInfoEx.KnownFolderToPIDLIntPtr(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Desktop));
            else
                return PathToPIDLIntPtr(FullName);
        }

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
            using (ShellFolder2 sf = this.getIShellFolder())
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
                                string[] allowedPaths = new string[] { IDD_RecycleBin };

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
        public IEnumerable<IDirectoryBrowser> EnumerateDirectories()
        {
            return EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
        }

        #region GetXXX
        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        public IDirectoryBrowser[] GetDirectories(String searchPattern, SearchOption searchOption)
        {
            try
            {
                checkExists();
                return new List<IDirectoryBrowser>(EnumerateDirectories(searchPattern, searchOption)).ToArray();
            }
            catch
            {
                // Return empty list if something did not work as expected
                return new List<IDirectoryBrowser>().ToArray();
            }
        }

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        public IDirectoryInfoEx[] GetDirectories(String searchPattern)
        {
            try
            {
                checkExists();
                return new List<IDirectoryInfoEx>(EnumerateDirectories(searchPattern)).ToArray();
            }
            catch
            {
                // Return empty list if something did not work as expected
                return new List<IDirectoryInfoEx>().ToArray();
            }
        }

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        public IDirectoryBrowser[] GetDirectories()
        {
            try
            {
                checkExists();
                return new List<IDirectoryBrowser>(EnumerateDirectories()).ToArray();
            }
            catch
            {
                // Return empty list if something did not work as expected
                return new List<IDirectoryBrowser>().ToArray();
            }
        }

        /// <summary>
        /// Returns a task that gets a list of sub directories.
        /// </summary>
        public Task<IDirectoryInfoEx[]> GetDirectoriesAsync(String searchPattern,
          SearchOption searchOption, CancellationToken ct)
        {
            try
            {
                checkExists();
                return Task.Run(() =>
                    new List<IDirectoryInfoEx>(EnumerateDirectories(searchPattern, searchOption,
                        () => ct.IsCancellationRequested)).ToArray(), ct);
            }
            catch
            {
                // Return empty list if something did not work as expected
                return Task.Run(() => new List<IDirectoryInfoEx>().ToArray());
            }
        }
        #endregion
        #endregion

        /// <summary>
        /// Gets the label of this item to show meaningful information in the debugger.
        /// </summary>
        public override string ToString()
        {
            return Label;
        }

        /// <summary>
        /// Gets a PIDL for the Knownfolder parameter object.
        /// </summary>
        protected static PIDL KnownFolderToPIDL(KnownFolder knownFolder)
        {
            IntPtr ptrAddr;
            PIDL pidl;

            int RetVal = (knownFolder.KnownFolderInterface).GetIDList(KnownFolderRetrievalOptions.Create, out ptrAddr);
            if (ptrAddr != IntPtr.Zero)
            {
                pidl = new PIDL(ptrAddr, false);
                return pidl;
            }
            else
                throw new ArgumentException("Invalid knownFolder " + RetVal);
        }

        protected static IntPtr KnownFolderToPIDLIntPtr(KnownFolder knownFolder)
        {
            IntPtr ptrAddr;

            int RetVal = (knownFolder.KnownFolderInterface).GetIDList(KnownFolderRetrievalOptions.Create, out ptrAddr);
            if (ptrAddr != IntPtr.Zero)
            {
                return ptrAddr;
            }
            else
                throw new ArgumentException("Invalid knownFolder " + RetVal);
        }

        /// <summary>
        /// Takes a directoryInfoEx and return the first parent with directory type = desktop or drive.
        /// </summary>
        protected static IDirectoryBrowser getDirectoryRoot(IDirectoryInfoEx lookup)
        {
            IDirectoryBrowser dir = lookup.Parent;

            while (dir.DirectoryType != DirectoryTypeEnum.dtDesktop &&
                dir.DirectoryType != DirectoryTypeEnum.dtDrive &&
                ////                dir.DirectoryType != DirectoryTypeEnum.dtRoot &&
                dir != null)
                dir = dir.Parent;

            if (dir == null)
                throw new IOException("Internal exception in GetDirectoryRoot.");

            return dir;
        }

        /// <summary>
        /// Gets an object that implements the IShellFolder2 interface of the Windows Desktop object.
        /// The desktop object is an important anchor point in the Windows Shell since it is considered
        /// to be the root of all items below it.
        ///
        /// This type of object can be used to determine internals of a Windows shell folder.
        ///
        /// MSDN: https://msdn.microsoft.com/en-us/library/windows/desktop/bb775055(v=vs.85).aspx
        /// </summary>
        /// <returns>The object that implement the IShellFolder2 interface or null.</returns>
        protected static ShellFolder2 getDesktopShellFolder()
        {
            IntPtr ptrShellFolder;
            int hr = ShellAPI.SHGetDesktopFolder(out ptrShellFolder);
            if (hr == ShellAPI.S_OK && ptrShellFolder != IntPtr.Zero)
            {
                ShellFolder2 sf = new ShellFolder2(ptrShellFolder);
                //0.13: Fixed? Desktop ShellFolder not released. (may be a cause of AccessViolationException.)
                //GC.SuppressFinalize(sf);
                return sf;
            }
            else Marshal.ThrowExceptionForHR(hr);

            return null; //mute error.
        }

        /// <summary>
        /// Gets an object that represents an ABSOLUTE PIDL of the given path parameter.
        ///
        /// MSDN: https://msdn.microsoft.com/en-us/library/windows/desktop/bb775055(v=vs.85).aspx
        /// </summary>
        /// <returns>The PIDL object that wraps the pidl <seealso cref="IntPtr"/> or null
        /// if the PIDL cannot be determined at this time.</returns>
        protected static PIDL PathToPIDL(string path)
        {
            path = RemoveSlash(path);
            IntPtr pidlPtr;
            uint pchEaten = 0;
            ShellAPI.SFGAO pdwAttributes = 0;

            using (ShellFolder2 _desktopShellFolder = getDesktopShellFolder())
            {
                int hr = _desktopShellFolder.ParseDisplayName(
                    IntPtr.Zero, IntPtr.Zero, path, ref pchEaten, out pidlPtr, ref pdwAttributes);

                if (pidlPtr == IntPtr.Zero || hr != ShellAPI.S_OK)
                {
                    // Commented because this is part of init and it's too time consuming.
                    ////Marshal.ThrowExceptionForHR(hr);
                    return null;
                }
            }

            return new PIDL(pidlPtr, false);
        }

        /// <summary>
        /// Gets an <seealso cref="IntPtr"/> that represents an ABSOLUTE PIDL of the given path parameter.
        ///
        /// MSDN: https://msdn.microsoft.com/en-us/library/windows/desktop/cc144090(v=vs.85).aspx#ID_Lists_PIDL
        /// </summary>
        /// <returns>The <seealso cref="IntPtr"/> or default(<seealso cref="IntPtr"/>)
        /// if the PIDL cannot be determined at this time.</returns>
        protected static IntPtr PathToPIDLIntPtr(string path)
        {
            path = RemoveSlash(path);
            IntPtr pidlPtr;
            uint pchEaten = 0;
            ShellAPI.SFGAO pdwAttributes = 0;

            using (ShellFolder2 _desktopShellFolder = getDesktopShellFolder())
            {
                int hr = _desktopShellFolder.ParseDisplayName(
                    IntPtr.Zero, IntPtr.Zero, path, ref pchEaten, out pidlPtr, ref pdwAttributes);

                if (pidlPtr == IntPtr.Zero || hr != ShellAPI.S_OK)
                {
                    // Commented because this is part of init and it's too time consuming.
                    ////Marshal.ThrowExceptionForHR(hr);
                    return default(IntPtr);
                }
            }

            return pidlPtr;
        }

        /// <summary>
        /// Gets a path for a given ABSOLUTE/Full <seealso cref="PIDL"/> object parameter.
        ///
        /// MSDN: https://msdn.microsoft.com/en-us/library/windows/desktop/cc144090(v=vs.85).aspx#ID_Lists_PIDL
        /// </summary>
        /// <returns>The path (e.g.: 'C:\Windows' or '::{...}').</returns>
        protected static string PIDLToPath(PIDL pidlFull)
        {
            PIDL desktopPIDL = DirectoryInfoEx.DesktopDirectory.getPIDL();
            try
            {
                if (pidlFull.Equals(desktopPIDL))
                    return DirectoryInfoEx.IID_Desktop;
            }
            finally
            {
                desktopPIDL.Free();
            }

            using (ShellFolder2 _desktopShellFolder = getDesktopShellFolder())
            {
                return loadName(_desktopShellFolder, pidlFull, ShellAPI.SHGNO.FORPARSING);
            }
        }

        /// <summary>
        /// Gets a path for a given Relative <seealso cref="PIDL"/> object parameter.
        /// The relative PIDL must be relative to the given IShellFolder2 interface parameter.
        ///
        /// MSDN: https://msdn.microsoft.com/en-us/library/windows/desktop/cc144090(v=vs.85).aspx#ID_Lists_PIDL
        /// </summary>
        /// <returns>The path (e.g.: 'C:\Windows' or '::{...}').</returns>
        protected static string PIDLToPath(IShellFolder2 iShellFolder, PIDL pidlRel)
        {
            return loadName(iShellFolder, pidlRel, ShellAPI.SHGNO.FORPARSING);
        }

        /// <summary>
        /// Gets a Relative <seealso cref="PIDL"/> that represents this item's parent (if any).
        /// The relative PIDL is the last item in the ITEMIDLIST structure
        /// if this item is a container (directory or virtual directory).
        ///
        /// MSDN: https://msdn.microsoft.com/en-us/library/windows/desktop/cc144090(v=vs.85).aspx#ID_Lists_PIDL
        /// </summary>
        /// <returns>The path (e.g.: 'C:\Windows' or '::{...}').</returns>
        protected static PIDL getRelativePIDL(PIDL pidl)
        {
            if (pidl == null)
                return null;

            IntPtr pRelPIDL = PIDL.ILFindLastID(pidl.Ptr);

            if (pRelPIDL == IntPtr.Zero)
                throw new IOException("getRelativePIDL");

            return new PIDL(pRelPIDL, true); //0.21
        }

        /// <summary>
        /// Gets a Relative <seealso cref="PIDL"/> object that represents
        /// the parent of the item descried by the <param ref="pidl"/> parameter.
        ///
        /// The relative PIDL is the last item in the ITEMIDLIST structure
        /// if this item is a container (directory or virtual directory).
        ///
        /// MSDN: https://msdn.microsoft.com/en-us/library/windows/desktop/cc144090(v=vs.85).aspx#ID_Lists_PIDL
        /// <param name="pidl">Represents the item whos relative parent pidl is to be returned.</param>
        /// <param name="relPIDL">Returns the relative parent pidl via out.</param>
        /// <returns>The relative PIDL object of the parent item.</returns>
        /// </summary>
        protected static PIDL getParentPIDL(PIDL pidl, out PIDL relPIDL)
        {
            relPIDL = new PIDL(pidl, true); //0.21
            if (pidl.Size == 0)
                return pidl;

            IntPtr pParent = PIDL.ILClone(pidl.Ptr);

            relPIDL = getRelativePIDL(pidl);
            if (pParent == IntPtr.Zero || PIDL.ILRemoveLastID2(ref pParent) == false)
            {
                Marshal.FreeCoTaskMem(pParent); //new PIDL(pParent, false).Free();
                pParent = IntPtr.Zero;

                return DirectoryInfoEx.KnownFolderToPIDL(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Desktop));
            }

            return new PIDL(pParent, false); //pParent will be freed by the PIDL.
        }

        /// <summary>
        /// Gets a Relative <seealso cref="PIDL"/> object that represents
        /// the parent of the item descried by the <param ref="pidl"/> parameter.
        ///
        /// The relative PIDL is the last item in the ITEMIDLIST structure
        /// if this item is a container (directory or virtual directory).
        ///
        /// MSDN: https://msdn.microsoft.com/en-us/library/windows/desktop/cc144090(v=vs.85).aspx#ID_Lists_PIDL
        /// </summary>
        /// <param name="pidl">Represents the item whos relative parent pidl is to be returned.</param>
        /// <returns>The relative PIDL object of the parent item.</returns>
        protected static PIDL getParentPIDL(PIDL pidl)
        {
            PIDL relPIDL = null;
            try
            {
                return getParentPIDL(pidl, out relPIDL);
            }
            finally
            {
                if (relPIDL != null)
                    relPIDL.Free();
            }
        }

        /// <summary>
        /// Gets the name of the item represented by the given path parameter.
        /// (e.g. returns 'Windows' for 'C:\Windows\' or 'C:\Windows')
        /// </summary>
        /// <param name="path">Path to the item whos name is to be determined.</param>
        /// <returns>The name of the item addressed by the parameter</returns>
        protected static string GetFileName(string path)
        {
            int idx = path.LastIndexOf('\\');

            if (idx == -1)
                return path;

            return path.Substring(idx + 1);
        }

        /// <summary>
        /// Gets the name of a shell folder item based on its <seealso cref="IntPtr"/>.
        /// </summary>
        /// <param name="iShellFolder"></param>
        /// <param name="ptr"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        protected static string loadName(IShellFolder2 iShellFolder,
                                         IntPtr ptr,
                                         Header.ShellDll.ShellAPI.SHGNO uFlags)
        {
            if (iShellFolder == null)
                return null;

            IntPtr ptrStr = Marshal.AllocCoTaskMem(Header.ShellDll.ShellAPI.MAX_PATH * 2 + 4);
            Marshal.WriteInt32(ptrStr, 0, 0);
            StringBuilder buf = new StringBuilder(Header.ShellDll.ShellAPI.MAX_PATH);

            try
            {
                if (iShellFolder.GetDisplayNameOf(ptr, uFlags, ptrStr) == Header.ShellDll.ShellAPI.S_OK)
                    Header.ShellDll.ShellAPI.StrRetToBuf(ptrStr, ptr, buf, Header.ShellDll.ShellAPI.MAX_PATH);
            }
            finally
            {
                if (ptrStr != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(ptrStr);
                ptrStr = IntPtr.Zero;
            }

            return buf.ToString();
        }

        protected static string loadName(IShellFolder2 iShellFolder,
                                  PIDL relPidl,
                                  Header.ShellDll.ShellAPI.SHGNO uFlags)
        {
            return loadName(iShellFolder, relPidl.Ptr, uFlags);
        }

        /// <summary>
        /// Remove slash end of input.
        /// </summary>
        protected static string RemoveSlash(string input)
        {
            if (!input.EndsWith(@":\") && input.EndsWith(@"\"))
            {
                return input.Substring(0, input.Length - 1);
            }
            else
                return input;
        }

        /// <summary>
        /// Gets a PIDL object that represents a Relative PIDL of the parent item
        /// in relation to this item. The parent item can be used to navigate the
        /// Windows shell tree towards the root.
        ///
        /// 0.12: Fixed PIDL, PIDLRel, ShellFolder
        /// Storage properties generated on demand to avoid x-thread issues.
        /// </summary>
        /// <returns>The <seealso cref="IntPtr"/> or default(<seealso cref="IntPtr"/>)
        /// if the PIDL cannot be determined at this time.</returns>
        protected PIDL getRelPIDL()
        {
            //0.14 : FileSystemInfoEx now stored a copy of PIDL/Rel, will return copy of it when properties is called (to avoid AccessViolation).
            if (_pidlRel != null)
                return new PIDL(_pidlRel, true);

            //0.16: Fixed getRelPIDL() cannot return correct value if File/DirInfoEx construct with string. (attemp to return a freed up pointer).                       
            PIDL pidlLookup = getPIDL();
            try
            {
                return getRelativePIDL(pidlLookup);
            }
            finally
            {
                if (pidlLookup != null)
                    pidlLookup.Free();

                pidlLookup = null;
            }
        }

        /// <summary>
        /// Gets an ABSOLUTE PIDL object for this item.
        /// </summary>
        /// <returns>The <seealso cref="IntPtr"/> or default(<seealso cref="IntPtr"/>)
        /// if the PIDL cannot be determined at this time.</returns>
        protected PIDL getPIDL()
        {
            if (_pidl != null)
                return new PIDL(_pidl, true);

            // Path equal Desktop ID -> return desktop's PIDL
            if (FullName == DirectoryInfoEx.IID_Desktop)
                return DirectoryInfoEx.KnownFolderToPIDL(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Desktop));
            else
                return PathToPIDL(FullName);
        }

        /// <summary>
        /// Gets an IShellFolder2 object and a RELATIVE PIDL for the parent of the
        /// given <param ref="pidl"/> parameter.
        ///
        /// MSDN:
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/cc144090(v=vs.85).aspx#ID_Lists_PIDL
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb775055(v=vs.85).aspx
        /// </summary>
        /// <param name="pidl">Represents the item whos
        /// relative parent pidl and IshellFolder interface is to be returned.</param>
        /// <param name="relPIDL">Returns the relative parent pidl via out.</param>
        /// <returns>The <seealso cref="IntPtr"/> or default(<seealso cref="IntPtr"/>)
        /// if the PIDL cannot be determined at this time.</returns>
        protected ShellFolder2 getParentIShellFolder(PIDL pidl, out PIDL relPIDL)
        {
            int hr;
            IntPtr ptrShellFolder = IntPtr.Zero;

            if (pidl.Size == 0 || PIDL.ILFindLastID(pidl.Ptr) == pidl.Ptr || //is root or parent is root
                PIDLToPath(pidl) == Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
            {

                hr = ShellAPI.SHGetDesktopFolder(out ptrShellFolder);
                relPIDL = new PIDL(pidl, true);
            }
            else
            {
                PIDL parentPIDL = getParentPIDL(pidl, out relPIDL);

                //Console.WriteLine("ParentPIDL.Size = {0}", parentPIDL.Size);
                System.Guid guid = ShellAPI.IID_IShellFolder2;
                using (ShellFolder2 _desktopShellFolder = getDesktopShellFolder())
                    hr = _desktopShellFolder.BindToObject(parentPIDL.Ptr, IntPtr.Zero,
                        ref guid, out ptrShellFolder);

                if (parentPIDL != null) parentPIDL.Free();
            }

            if (hr == ShellAPI.S_OK && ptrShellFolder != IntPtr.Zero)
                return new ShellFolder2(ptrShellFolder);
            else Marshal.ThrowExceptionForHR(hr);

            return null; //mute error.
        }

        /// <summary>
        /// Gets a path for a given <seealso cref="IntPtr"/> representing an
        /// ABSOLUTE/Full <seealso cref="PIDL"/>.
        ///
        /// MSDN: https://msdn.microsoft.com/en-us/library/windows/desktop/cc144090(v=vs.85).aspx#ID_Lists_PIDL
        /// <returns>The path (e.g.: 'C:\Windows' or '::{...}').</returns>
        /// </summary>
        protected string PtrToPath(IntPtr ptr)
        {
            using (ShellFolder2 _desktopShellFolder = getDesktopShellFolder())
            {
                return loadName(_desktopShellFolder, ptr, ShellAPI.SHGNO.FORPARSING);
            }
        }

        protected void checkExists()
        {
            //0.18: checkExists() ignore network directory.
            if (!FullName.StartsWith("\\") && !Exists)
                throw new DirectoryNotFoundException(FullName + " does not exist.");
        }

        /// <summary>
        /// Initializes the parent item of this item.
        /// </summary>
        protected IDirectoryInfoEx initParent()
        {
            if (!_parentInited)
            {
                if (Exists)
                {
                    if (FullName.Equals(DirectoryInfoEx.IID_Desktop))
                        return _parent as IDirectoryInfoEx;

                    PIDL pidl = this.getPIDL();
                    try
                    {
                        _parent = new DirectoryInfoEx(getParentPIDL(pidl));
                        _parentInited = true;
                    }
                    finally
                    {
                        pidl.Free();
                    }
                }
                else
                {
                    _parent = new DirectoryInfoEx(GetDirectoryName(FullName));
                    _parentInited = true;
                }
            }

            return _parent as IDirectoryInfoEx;
        }

        /// <summary>
        /// Gets the name of the item represented by the given path parameter.
        /// (e.g. returns 'Windows' for 'C:\Windows\' or 'C:\Windows')
        /// </summary>
        /// <param name="path">Path to the item whos name is to be determined.</param>
        /// <returns>The name of the item addressed by the parameter</returns>
        protected string GetDirectoryName(string path)
        {
            if (path.EndsWith("\\"))
                path = path.Substring(0, path.Length - 1); // Remove ending slash.

            int idx = path.LastIndexOf('\\');

            if (idx == -1)
                return "";

            return path.Substring(0, idx);
        }

        /// <summary>
        /// Gets the name of the IShellFolder2 object given in the parameters list.
        /// </summary>
        /// <param name="iShellFolder">The object that implements IShellFolder2</param>
        /// <param name="uFlags">A list of flags that determines the type of name to get</param>
        /// <returns>The name of the shell item addressed by the parameter</returns>
        protected string loadName(IShellFolder2 iShellFolder, Header.ShellDll.ShellAPI.SHGNO uFlags)
        {
            PIDL pidl = new PIDL(IntPtr.Zero, false); // Memory Leak Fix
            try
            {
                return loadName(iShellFolder, new PIDL(IntPtr.Zero, false), uFlags);
            }
            finally
            {
                pidl.Free();
            }
        }

        /// <summary>
        /// Gets whether this item exists in the Windows Shell namespace or not.
        /// This includes virtual folders like Control Panel and other items whos
        /// existence is all but trivial.
        /// </summary>
        /// <returns>true if the item exists, otherwise false.</returns>
        protected bool getExists()
        {
            if (FullName == DirectoryInfoEx.IID_Desktop) // Desktop
                return true;
            else
            {
                if (FullName == null)
                    return false;
                else
                {
                    try
                    {
                        if (_pidl != null)
                        {
                            using (ShellFolder2 _desktopShellFolder = getDesktopShellFolder())
                            {
                                loadName(_desktopShellFolder, _pidl, Header.ShellDll.ShellAPI.SHGNO.FORPARSING);
                            }

                            return true;
                        }
                        else
                        {
                            PIDL pidlLookup = PathToPIDL(FullName);
                            try
                            {
                                return pidlLookup != null;
                            }
                            finally
                            {
                                if (pidlLookup != null)
                                    pidlLookup.Free();

                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        return false;
                    }
                }
            }
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
        /// Gets the Windows Shell file system properties for this object.
        /// </summary>
        private static FileAttributes loadAttributes(IShellFolder2 iShellFolder, PIDL pidlFull, PIDL pidlRel)
        {
            FileAttributes retVal = new FileAttributes();

            //ShellAPI.SFGAO attribute = shGetFileAttribute(pidlFull, ShellAPI.SFGAO.READONLY |
            //    ShellAPI.SFGAO.FOLDER | ShellAPI.SFGAO.FILESYSTEM | ShellAPI.SFGAO.STREAM | ShellAPI.SFGAO.FILESYSANCESTOR |
            //    ShellAPI.SFGAO.HIDDEN);
            Header.ShellDll.ShellAPI.SFGAO attribute = Header.ShellDll.ShellAPI.SFGAO.READONLY |
                                                       Header.ShellDll.ShellAPI.SFGAO.FOLDER |
                                                       Header.ShellDll.ShellAPI.SFGAO.FILESYSTEM |
                                                       Header.ShellDll.ShellAPI.SFGAO.STREAM |
                                                       Header.ShellDll.ShellAPI.SFGAO.FILESYSANCESTOR;

            iShellFolder.GetAttributesOf(1, new IntPtr[] { pidlRel.Ptr }, ref attribute);

            if ((attribute & Header.ShellDll.ShellAPI.SFGAO.FOLDER) != 0)
                retVal |= FileAttributes.Directory;

            if ((attribute & Header.ShellDll.ShellAPI.SFGAO.HIDDEN) != 0)
                retVal |= FileAttributes.Hidden;

            if ((attribute & Header.ShellDll.ShellAPI.SFGAO.READONLY) != 0)
                retVal |= FileAttributes.ReadOnly;

            return retVal;
        }

        /// <summary>
        /// Gets an object that implements the IShellFolder2 interface.
        /// This type of object can be used to determine internals of a Windows shell folder.
        ///
        /// MSDN: https://msdn.microsoft.com/en-us/library/windows/desktop/bb775055(v=vs.85).aspx
        /// </summary>
        private ShellFolder2 getIShellFolder()
        {
            if (this.FullName.Equals(DirectoryInfoEx.IID_Desktop))
                return getDesktopShellFolder();
            else
            {
                ShellFolder2 retVal = getIShellFolderFromParent();
                if (retVal != null)
                    return retVal;

                PIDL pidl = this.getPIDL();
                PIDL relPidl = this.getRelPIDL();
                try
                {
                    using (ShellFolder2 parentShellFolder = getParentIShellFolder(pidl, out relPidl))
                    {
                        IntPtr ptrShellFolder;
                        int hr = parentShellFolder.BindToObject(relPidl.Ptr, IntPtr.Zero, ref ShellAPI.IID_IShellFolder2,
                            out ptrShellFolder);
                        if (ptrShellFolder == IntPtr.Zero || hr != ShellAPI.S_OK) Marshal.ThrowExceptionForHR(hr);

                        return new ShellFolder2(ptrShellFolder);
                    }
                }
                finally
                {
                    pidl.Free();
                    relPidl.Free();
                }
            }
        }

        /// <summary>
        /// Gets an object that implements the IShellFolder2 interface of the Parent object
        /// or null if there is no known parent for this item. The parent object can be used
        /// to determine further details for items contained below the parent item.
        ///
        /// This type of object can be used to determine internals of a Windows shell folder.
        ///
        /// MSDN: https://msdn.microsoft.com/en-us/library/windows/desktop/bb775055(v=vs.85).aspx
        /// </summary>
        private ShellFolder2 getIShellFolderFromParent()
        {
            if (_parent == null)
                return null;

            using (ShellFolder2 parentShellFolder = (_parent as DirectoryInfoEx).getIShellFolder())
            {
                IntPtr ptrShellFolder = IntPtr.Zero;

                int hr = ShellAPI.S_FALSE;
                PIDL relPidl = this.getRelPIDL();
                try
                {
                    hr = parentShellFolder.BindToObject(relPidl.Ptr, IntPtr.Zero, ref ShellAPI.IID_IShellFolder2,
                                                        out ptrShellFolder);
                }
                finally
                {
                    relPidl.Free();
                }

                if (ptrShellFolder != IntPtr.Zero && hr == ShellAPI.S_OK)
                    return new ShellFolder2(ptrShellFolder);
            }

            return null;
        }

        #region Match File Mask Filter
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

        /// <summary>
        /// http://stackoverflow.com/questions/725341/c-file-mask
        /// Return whether filename match fileMask ( * and ? supported)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileMask"></param>
        /// <param name="forceSlashCheck"></param>
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

        /// <summary>
        /// Method implements an extension of the init() method which is called upon construction.
        /// This method can be called to initialize/refresh file system properties of this object.
        /// </summary>
        private void refresh(IShellFolder2 parentShellFolder, PIDL relPIDL, PIDL fullPIDL)
        {
            if (parentShellFolder != null && fullPIDL != null && relPIDL != null)
            {
                Attributes = loadAttributes(parentShellFolder, fullPIDL, relPIDL);
                string parseName = loadName(parentShellFolder, relPIDL, Header.ShellDll.ShellAPI.SHGNO.FORPARSING);
                FullName = "";

                //Console.WriteLine("relPIDL.size = {0}", relPIDL.Size);
                //Console.WriteLine("PIDL.size = {0}", _pidl.Size); 

                if (relPIDL.Size == 0)
                    FullName = DirectoryInfoEx.IID_Desktop;
                else
                //0.12: Fixed Fullname of User/Shared directory under desktop is now it's GUID instead of it's file path.
                //0.13: Fixed All subdirectory under User/Shared directory uses GUID now.
                {
                    if (DirectoryInfoEx.CurrentUser != null)
                    {
                        if (parseName == DirectoryInfoEx.CurrentUser.FullName &&
                            loadName(parentShellFolder, Header.ShellDll.ShellAPI.SHGNO.FORPARSING) == Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                        {
                            FullName = DirectoryInfoEx.IID_UserFiles;
                        }
                        //else if (
                        //    (parseName.StartsWith(DirectoryInfoEx.CurrentUserDirectory.FullName) &&
                        //    _parent != null && _parent.FullName.StartsWith(IOTools.IID_UserFiles))
                        //    ||
                        //    (OriginalPath != null && OriginalPath.StartsWith(IOTools.IID_UserFiles))
                        //    )
                        //{                            
                        //    FullName = parseName.Replace(DirectoryInfoEx.CurrentUserDirectory.FullName, IOTools.IID_UserFiles);
                        //}
                    }

                    if (DirectoryInfoEx.SharedDirectory != null)
                    {
                        if (parseName == DirectoryInfoEx.SharedDirectory.FullName &&
                        loadName(parentShellFolder, Header.ShellDll.ShellAPI.SHGNO.FORPARSING) == Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                            FullName = DirectoryInfoEx.IID_Public;
                        //else if (
                        //    (parseName.StartsWith(DirectoryInfoEx.SharedDirectory.FullName) &&
                        //    _parent != null && _parent.FullName.StartsWith(IOTools.IID_Public))
                        //    ||
                        //    (OriginalPath != null && OriginalPath.StartsWith(IOTools.IID_Public))
                        //    )
                        //    FullName = parseName.Replace(DirectoryInfoEx.SharedDirectory.FullName, IOTools.IID_Public);
                    }

                    //if (_parent != null && _parent.FullName.StartsWith(IOTools.IID_Library)
                    //    && !parseName.StartsWith(IOTools.IID_Library))
                    //    FullName = PathEx.Combine(_parent.FullName, PathEx.GetFileName(parseName));

                    if (FullName == "")
                        FullName = parseName;
                }
                //if (DirectoryInfoEx.CurrentUserDirectory != null && parseName == DirectoryInfoEx.CurrentUserDirectory.FullName &&
                //loadName(parentShellFolder, ShellAPI.SHGNO.FORPARSING) == Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                //    FullName = IOTools.IID_UserFiles;
                //else

                //if (DirectoryInfoEx.SharedDirectory != null && parseName == DirectoryInfoEx.SharedDirectory.FullName &&
                //    loadName(parentShellFolder, ShellAPI.SHGNO.FORPARSING) == Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                //    FullName = IOTools.IID_Public;
                //else

                ////                if (OriginalPath == null)
                ////                    OriginalPath = FullName;

                if (parseName.StartsWith("::")) //Guid
                    parseName = loadName(parentShellFolder, relPIDL, Header.ShellDll.ShellAPI.SHGNO.NORMAL);

                _name = FullName.EndsWith("\\") ? FullName : GetFileName(FullName);
                Label = loadName(parentShellFolder, relPIDL, Header.ShellDll.ShellAPI.SHGNO.NORMAL);
            }
            else
            {
                ////                if (OriginalPath != null)
                ////                {
                ////                    string origPath = RemoveSlash(OriginalPath);
                ////                    _name = Path.GetFileName(origPath);
                ////                    Label = _name;
                ////                    FullName = origPath;
                ////                }
            }
        }

        /// <summary>
        /// Determines via delegate whether processing was meanwhile cancelled or not.
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
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
        #endregion
    }
}
