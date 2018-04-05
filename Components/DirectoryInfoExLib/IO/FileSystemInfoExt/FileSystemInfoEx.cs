///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2009 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Steven Roebert's work (http://www.codeproject.com/KB/miscctrl/FileBrowser.aspx)        //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace DirectoryInfoExLib.IO.FileSystemInfoExt
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Runtime.Serialization;
    using DirectoryInfoExLib.IO.Header.ShellDll;
    using DirectoryInfoExLib.IO.Tools.Interface;
    using DirectoryInfoExLib.IO.Header;
    using DirectoryInfoExLib.Tools;
    using DirectoryInfoExLib.IO.Tools;
    using DirectoryInfoExLib.Interfaces;

    internal class FileSystemInfoEx : FileSystemInfo, IDisposable, ISerializable, ICloneable,
        IEquatable<FileSystemInfoEx>
    {
        #region Enums

        [Flags]
        public enum RefreshModeEnum : int { None = 1 << 0, BaseProps = 1 << 1, FullProps =  1 << 2, AllProps = BaseProps | FullProps }

        #endregion

        #region Variables and Properties
        public static int counter = 0;
        private string _name;
        internal IDirectoryInfoEx _parent;
        private bool _parentInited = false;
        private PIDL _pidlRel = null;
        private PIDL _pidl = null;
        private FileAttributes _attributes;
        private DateTime _lastWriteTime, _lastAccessTime, _creationTime;
  

        //public PIDL PIDLRel
        //{
        //    get { return getRelPIDL(); ; }
        //}
        //public PIDL PIDL
        //{
        //    get { return getPIDL(); }
        //}

        public RefreshModeEnum RefreshMode { get; private set; }
        public override string Name { get { return _name; } }
        public new string Extension { get { return Path.GetExtension(Name); } }
        public override bool Exists { get { return getExists(); } }

        public IDirectoryInfoEx Parent
        {
            get { initParent(); return _parent; }

            protected set
            {
                if (_parent != null)
                    throw new Exception("_parent cannot be reset.");

                _parent = value;
            }
        }

        public string Label { get; protected set; }
        public new string FullName { get; protected set; }
        public new FileAttributes Attributes { get { checkRefresh(); return _attributes; } set { _attributes = value; } }
        public new DateTime LastWriteTime { get { checkRefresh(); return _lastWriteTime; } set { _lastWriteTime = value; } }
        public new DateTime LastWriteTimeUtc { get { return LastWriteTime.ToUniversalTime(); } }
        public new DateTime LastAccessTime { get { checkRefresh(); return _lastAccessTime; } set { _lastAccessTime = value; } }
        public new DateTime LastAccessTimeUtc { get { return LastAccessTime.ToUniversalTime(); } }
        public new DateTime CreationTime { get { checkRefresh(); return _creationTime; } set { _creationTime = value; } }
        public new DateTime CreationTimeUtc { get { return CreationTime.ToUniversalTime(); } }
        public bool IsFolder { get { return (Attributes & FileAttributes.Directory) == FileAttributes.Directory; } }

        protected void initParent()
        {
            if (!_parentInited)
                if (Exists)
                {
                    if (FullName.Equals(DirectoryInfoExLib.Tools.IOTools.IID_Desktop)) return;

                    this.RequestPIDL((pidl) =>
                        {
////                            PIDL relPIDL;
                            _parent = new DirectoryInfoEx(getParentPIDL(pidl));                            
                            _parentInited = true;
                            
                        });
                }
                else
                {
                    _parent = new DirectoryInfoEx(PathEx.GetDirectoryName(FullName));
                    _parentInited = true;
                }
        }



        //0.12: Fixed PIDL, PIDLRel, ShellFolder, Storage properties generated on demand to avoid x-thread issues.
        internal PIDL getRelPIDL()
        {
            if (_pidlRel != null) //0.14 : FileSystemInfoEx now stored a copy of PIDL/Rel, will return copy of it when properties is called (to avoid AccessViolation). 
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

        internal PIDL getPIDL()
        {
            if (_pidl != null)
                return new PIDL(_pidl, true);

            if (FullName == "::{00021400-0000-0000-C000-000000000046}") //Desktop
                return DirectoryInfoEx.CSIDLtoPIDL(Header.ShellDll.ShellAPI.CSIDL.CSIDL_DESKTOP);
            else
                return PathToPIDL(FullName);
        }

        private bool getExists()
        {
            if (FullName == "::{00021400-0000-0000-C000-000000000046}") //Desktop
                return true;
            else if (FullName == null) return false;
            else
                try
                {
                    if (_pidl != null)
                    {
                        using (ShellFolder2 _desktopShellFolder = getDesktopShellFolder())
                            loadName(_desktopShellFolder, _pidl, Header.ShellDll.ShellAPI.SHGNO.FORPARSING);
                        return true;
                    }
                    else
                    {
                        PIDL pidlLookup = PathToPIDL(FullName);
                        try
                        {
                            return pidlLookup != null;
                        }
                        finally { if (pidlLookup != null) pidlLookup.Free(); }
                    }
                }
                catch (FileNotFoundException)
                {
                    return false;
                }
        }

        #endregion

        #region Static Methods
        /// <summary>
        /// Gets wether a directory exists at a given path or not.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool DirectoryExists(string path)
        {
            try
            {
                FileSystemInfoEx fsInfo = new FileSystemInfoEx(path);
                return fsInfo != null && fsInfo.IsFolder && fsInfo.Exists;
            }
            catch { return false; }
        }

        /// <summary>
        /// Gets wether a file exists at a given path or not.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool FileExists(string path)
        {
            try
            {
                FileSystemInfoEx fsInfo = new FileSystemInfoEx(path);
                return fsInfo != null && !fsInfo.IsFolder && fsInfo.Exists;
            }
            catch { return false; }
        }

        internal static ShellFolder2 getDesktopShellFolder()
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

        internal static PIDL PathToPIDL(string path)
        {
            path = Helper.RemoveSlash(path);
            IntPtr pidlPtr;
            uint pchEaten = 0;
            ShellAPI.SFGAO pdwAttributes = 0;

            using (ShellFolder2 _desktopShellFolder = getDesktopShellFolder())
            {
                int hr = _desktopShellFolder.ParseDisplayName(
                    IntPtr.Zero, IntPtr.Zero, path, ref pchEaten, out pidlPtr, ref pdwAttributes);

                if (pidlPtr == IntPtr.Zero || hr != ShellAPI.S_OK)
                { /*Marshal.ThrowExceptionForHR(hr);*/ return null; }
                //Commented because this is part of init and it's too time consuming.
            }
            return new PIDL(pidlPtr, false);
        }


        internal static string PtrToPath(IntPtr ptr)
        {            
            using (ShellFolder2 _desktopShellFolder = getDesktopShellFolder())
                return loadName(_desktopShellFolder, ptr, ShellAPI.SHGNO.FORPARSING);

        }

        internal static string PIDLToPath(PIDL pidlFull)
        {            
            if (DirectoryInfoEx.DesktopDirectory.RequestPIDL(desktopPIDL => pidlFull.Equals(desktopPIDL)))
                return "::{00021400-0000-0000-C000-000000000046}";
            using (ShellFolder2 _desktopShellFolder = getDesktopShellFolder())
                return loadName(_desktopShellFolder, pidlFull, ShellAPI.SHGNO.FORPARSING);
                
        }

        internal static string PIDLToPath(IShellFolder2 iShellFolder, PIDL pidlRel)
        {
            return loadName(iShellFolder, pidlRel, ShellAPI.SHGNO.FORPARSING);
        }

        internal static PIDL getRelativePIDL(PIDL pidl)
        {
            if (pidl == null)
                return null;
            IntPtr pRelPIDL = PIDL.ILFindLastID(pidl.Ptr);
            if (pRelPIDL == IntPtr.Zero)
                throw new IOException("getRelativePIDL");
            return new PIDL(pRelPIDL, true); //0.21
        }

        internal static PIDL getParentPIDL(PIDL pidl, out PIDL relPIDL)
        {
            relPIDL = new PIDL(pidl, true); //0.21
            if (pidl.Size == 0)
                return pidl;

            IntPtr pParent = PIDL.ILClone(pidl.Ptr);

            relPIDL = getRelativePIDL(pidl);
            if (pParent == IntPtr.Zero || !PIDL.ILRemoveLastID2(ref pParent))
            {
                new PIDL(pParent, false).Free();
                return DirectoryInfoEx.CSIDLtoPIDL(ShellAPI.CSIDL.CSIDL_DESKTOP);
            }

            return new PIDL(pParent, false); //pParent will be freed by the PIDL.
        }

        internal static PIDL getParentPIDL(PIDL pidl)
        {
            PIDL relPIDL = null;
            try
            {
                return getParentPIDL(pidl, out relPIDL);
            }
            finally { if (relPIDL != null) relPIDL.Free(); }
        }

        //internal static string getParentParseName(PIDL pidl)
        //{
        //    PIDL relPIDL;
        //    PIDL parentPIDL = getParentPIDL(pidl, out relPIDL);
        //    IShellFolder sf = getParentIShellFolder(parentPIDL, out relPIDL);
        //    if (relPIDL.Size == 0)
        //        return IOTools.IID_Desktop;
        //    return loadName(sf, relPIDL, ShellAPI.SHGNO.FORPARSING);
        //}

        internal static ShellFolder2 getParentIShellFolder(PIDL pidl, out PIDL relPIDL)
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

        protected static ShellAPI.SFGAO shGetFileAttribute(PIDL pidl, ShellAPI.SFGAO lookup)
        {
            ShellAPI.SHFILEINFO shfi = new ShellAPI.SHFILEINFO();
            shfi.dwAttributes = ShellAPI.SFGAO.READONLY | ShellAPI.SFGAO.HIDDEN | ShellAPI.SFGAO.BROWSABLE |
                ShellAPI.SFGAO.FILESYSTEM | ShellAPI.SFGAO.HASSUBFOLDER;
            ShellAPI.SHGFI dwFlag = ShellAPI.SHGFI.PIDL | ShellAPI.SHGFI.ATTRIBUTES | ShellAPI.SHGFI.ATTR_SPECIFIED | ShellAPI.SHGFI.USEFILEATTRIBUTES;
            ShellAPI.FILE_ATTRIBUTE dwAttr = 0;
            int cbFileInfo = Marshal.SizeOf(shfi.GetType());
            IntPtr retPtr = ShellAPI.SHGetFileInfo(pidl.Ptr, dwAttr, ref shfi, cbFileInfo, dwFlag);

            if (retPtr.ToInt32() != ShellAPI.S_OK && retPtr.ToInt32() != 1)
                Marshal.ThrowExceptionForHR(retPtr.ToInt32());
            return shfi.dwAttributes;
        }


        private static FileAttributes loadAttributes(IShellFolder2 iShellFolder, PIDL pidlFull, PIDL pidlRel)
        {
            FileAttributes retVal = new FileAttributes();


            //ShellAPI.SFGAO attribute = shGetFileAttribute(pidlFull, ShellAPI.SFGAO.READONLY |
            //    ShellAPI.SFGAO.FOLDER | ShellAPI.SFGAO.FILESYSTEM | ShellAPI.SFGAO.STREAM | ShellAPI.SFGAO.FILESYSANCESTOR |
            //    ShellAPI.SFGAO.HIDDEN);
            Header.ShellDll.ShellAPI.SFGAO attribute = Header.ShellDll.ShellAPI.SFGAO.READONLY | Header.ShellDll.ShellAPI.SFGAO.FOLDER | Header.ShellDll.ShellAPI.SFGAO.FILESYSTEM | Header.ShellDll.ShellAPI.SFGAO.STREAM | Header.ShellDll.ShellAPI.SFGAO.FILESYSANCESTOR;
            iShellFolder.GetAttributesOf(1, new IntPtr[] { pidlRel.Ptr }, ref attribute);

            if (!IOTools.IsZip(attribute) && (attribute & Header.ShellDll.ShellAPI.SFGAO.FOLDER) != 0) retVal |= FileAttributes.Directory;
            if ((attribute & Header.ShellDll.ShellAPI.SFGAO.HIDDEN) != 0) retVal |= FileAttributes.Hidden;
            if ((attribute & Header.ShellDll.ShellAPI.SFGAO.READONLY) != 0) retVal |= FileAttributes.ReadOnly;

            return retVal;
        }

        private static string loadName(IShellFolder2 iShellFolder, Header.ShellDll.ShellAPI.SHGNO uFlags)
        {
            return loadName(iShellFolder, new PIDL(IntPtr.Zero, false), uFlags);
        }

        private static string loadName(IShellFolder2 iShellFolder,IntPtr ptr, Header.ShellDll.ShellAPI.SHGNO uFlags)
        {
            if (iShellFolder == null) return null;

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

        private static string loadName(IShellFolder2 iShellFolder, PIDL relPidl, Header.ShellDll.ShellAPI.SHGNO uFlags)
        {
            return loadName(iShellFolder, relPidl.Ptr, uFlags);
        }


        #endregion

        #region Methods


        protected virtual void refresh(IShellFolder2 parentShellFolder, PIDL relPIDL, PIDL fullPIDL, RefreshModeEnum mode)
        {
            if (parentShellFolder != null && fullPIDL != null && relPIDL != null)
            {

                Attributes = loadAttributes(parentShellFolder, fullPIDL, relPIDL);
                string parseName = loadName(parentShellFolder, relPIDL, Header.ShellDll.ShellAPI.SHGNO.FORPARSING);
                FullName = "";

                //Console.WriteLine("relPIDL.size = {0}", relPIDL.Size);
                //Console.WriteLine("PIDL.size = {0}", _pidl.Size); 


                if (relPIDL.Size == 0)
                    FullName = IOTools.IID_Desktop;
                else
                //0.12: Fixed Fullname of User/Shared directory under desktop is now it's GUID instead of it's file path.
                //0.13: Fixed All subdirectory under User/Shared directory uses GUID now.
                {
                    if (DirectoryInfoEx.CurrentUserDirectory != null)
                    {
                        if (parseName == DirectoryInfoEx.CurrentUserDirectory.FullName &&
                        loadName(parentShellFolder, Header.ShellDll.ShellAPI.SHGNO.FORPARSING) == Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                        {
                            FullName = IOTools.IID_UserFiles;
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
                            FullName = IOTools.IID_Public;
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


                if (OriginalPath == null)
                    OriginalPath = FullName;
                if (parseName.StartsWith("::")) //Guid
                    parseName = loadName(parentShellFolder, relPIDL, Header.ShellDll.ShellAPI.SHGNO.NORMAL);

                _name = FullName.EndsWith("\\") ? FullName : PathEx.GetFileName(FullName);
                Label = loadName(parentShellFolder, relPIDL, Header.ShellDll.ShellAPI.SHGNO.NORMAL);


            }
            else
            {
                if (OriginalPath != null)
                {
                    string origPath = Helper.RemoveSlash(OriginalPath);
                    _name = Path.GetFileName(origPath);
                    Label = _name;
                    FullName = origPath;
                }
            }
        }

        protected void checkRefresh(RefreshModeEnum mode = RefreshModeEnum.AllProps)
        {
            if ((RefreshMode & mode) != mode)
                Refresh(mode);
        }

        /// <summary>
        /// Refresh the file / directory info. Does not refresh directory contents 
        /// because it refresh every time GetFiles/Directories/FileSystemInfos is called.
        /// </summary>
        /// <param name="mode"></param>
        public void Refresh(RefreshModeEnum mode = RefreshModeEnum.AllProps )
        {
            RefreshMode |= mode; //0.23 : Delay loading some properties.

            PIDL relPIDL = null;
            if (!Exists)
                refresh(null, null, null, mode);
            else
                try
                {                    
                    //0.16: Fixed ShellFolder not freed
                    this.RequestPIDL(pidlLookup =>
                        {
                            using (ShellFolder2 sf = getParentIShellFolder(pidlLookup, out relPIDL))
                                refresh(sf, relPIDL, pidlLookup, mode);
                        });
                }
                catch (NullReferenceException)
                {
                    refresh(null, null, null, mode);
                }
                finally
                {               
                    if (relPIDL != null)
                        relPIDL.Free();
                    relPIDL = null;
                }

        }

        /// <summary>
        /// Delete the current item.
        /// </summary>
        public override void Delete()
        {
            throw new NotImplementedException();
////            if (this is FileInfoEx)
////                (this as FileInfoEx).Delete();
////            else
////                (this as DirectoryInfoEx).Delete();
        }

        /// <summary>
        /// Return if two FileSystemInfoEx is equal (using PIDL if possible, otherwise Path)
        /// </summary>
        public virtual bool Equals(FileSystemInfoEx other)
        {            
            return this.RequestPIDL(thisPidl => 
                other.RequestPIDL(otherPidl => thisPidl.Equals(otherPidl)));
        }

        public override bool Equals(object obj)
        {
            if (obj is FileSystemInfoEx)
                return Equals((FileSystemInfoEx)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return IOTools.GetHashCode(this);
        }

        public override string ToString()
        {
            return Label;
        }

        #endregion

        #region Constructors
        protected virtual void checkProperties()
        {
        }

        protected void init(PIDL fullPIDL)
        {
            PIDL relPIDL = null;
            //0.16: Fixed ShellFolder not freed
            using (ShellFolder2 parentShellFolder = getParentIShellFolder(fullPIDL, out relPIDL))
                refresh(parentShellFolder, relPIDL, fullPIDL, RefreshModeEnum.BaseProps);
            _pidl = new PIDL(fullPIDL, false); //0.14 : FileSystemInfoEx record the pidl when construct, as some path do not parasable (e.g. EntireNetwork)
            _pidlRel = relPIDL;
        }

        protected void init(IShellFolder2 parentShellFolder, PIDL fullPIDL)
        {
            PIDL relPIDL = null;
            getParentPIDL(fullPIDL, out relPIDL);
            refresh(parentShellFolder, relPIDL, fullPIDL, RefreshModeEnum.BaseProps);
            _pidl = new PIDL(fullPIDL, false);
            _pidlRel = relPIDL;
        }

        protected void init(IShellFolder2 parentShellFolder, PIDL parentPIDl, PIDL relPIDL)
        {
            PIDL fullPIDL = new PIDL(PIDL.ILCombine(parentPIDl.Ptr, relPIDL.Ptr), false);

            refresh(parentShellFolder, relPIDL, fullPIDL, RefreshModeEnum.BaseProps);
            _pidl = fullPIDL;
            _pidlRel = relPIDL; // new PIDL(relPIDL, false);
        }


        protected void init(string path)
        {
            //0.22: Fix illegal PIDL for Directory under Library.ms directory
            _pidl = null;
            _pidlRel = null;
            OriginalPath = path;
            FullName = path;
            Refresh(RefreshModeEnum.BaseProps);
        }

        protected void init(SerializationInfo info, StreamingContext context)
        {
            OriginalPath = info.GetString("OriginalPath");
            Label = info.GetString("Label");
            _name = info.GetString("Name");
            FullName = info.GetString("FullName");
            Attributes = (FileAttributes)info.GetValue("Attributes", typeof(FileAttributes));
            LastWriteTime = info.GetDateTime("LastWriteTime");
            LastAccessTime = info.GetDateTime("LastAccessTime");
            CreationTime = info.GetDateTime("CreationTime");
        }

        internal FileSystemInfoEx(string fullPath)
            : this()
        {
            init(fullPath);
        }

        protected FileSystemInfoEx(SerializationInfo info, StreamingContext context)
            : this()
        {
            init(info, context);
        }


        protected FileSystemInfoEx()
            : base()
        {
            System.Threading.Interlocked.Increment(ref counter);
        }

        #endregion

        #region ISerializable Members
        protected virtual void getObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("OriginalPath", OriginalPath);
            info.AddValue("Label", Label);
            info.AddValue("Name", Name);
            info.AddValue("FullName", FullName);
            info.AddValue("Attributes", Attributes);
            info.AddValue("LastWriteTime", LastWriteTime);
            info.AddValue("LastAccessTime", LastAccessTime);
            info.AddValue("CreationTime", CreationTime);

        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            getObjectData(info, context);
        }

        #endregion

        #region IDisposable Members

        ~FileSystemInfoEx()
        {
            ((IDisposable)this).Dispose();
        }

        public void Dispose()
        {
            if (_pidlRel != null || _pidl != null)
                System.Threading.Interlocked.Decrement(ref counter);

            if (_pidlRel != null)            
                _pidlRel.Free();                
            if (_pidl != null) _pidl.Free();
            _pidlRel = null;
            _pidl = null;
            
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new FileSystemInfoEx(this.FullName);
        }

        #endregion
    }
}
