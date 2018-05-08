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
    using DirectoryInfoExLib.IO.Header.ShellDll;
    using DirectoryInfoExLib.IO.Tools.Interface;
    using DirectoryInfoExLib.IO.Header;
    using DirectoryInfoExLib.Enums;
    using DirectoryInfoExLib.IO.Header.KnownFolder;

    internal class FileSystemInfoEx : IDisposable
    {
        #region Enums
        /// <summary>
        /// Determines whether all properties or only parts of the property set
        /// are loaded at initialization time (refreshing properties is disabled)
        /// </summary>
        [Flags]
        public enum RefreshModeEnum : int
        {
            None = 1 << 0,
            BaseProps = 1 << 1,
            FullProps = 1 << 2,
            AllProps = BaseProps | FullProps
        }
        #endregion

        #region fields
        public static int counter = 0;
        protected string _name;
        private PIDL _pidlRel = null;
        protected PIDL _pidl = null;
        private FileAttributes _attributes;
        #endregion fields

        #region Constructors
        protected FileSystemInfoEx(string fullPath)
            : this()
        {
            init(fullPath);
        }

        protected FileSystemInfoEx()
            : base()
        {
            System.Threading.Interlocked.Increment(ref counter);
        }

        ~FileSystemInfoEx()
        {
            ((IDisposable)this).Dispose();
        }

        protected void init(PIDL fullPIDL)
        {
            PIDL relPIDL = null;
            //0.16: Fixed ShellFolder not freed
            using (ShellFolder2 parentShellFolder = getParentIShellFolder(fullPIDL, out relPIDL))
            {
                refresh(parentShellFolder, relPIDL, fullPIDL, RefreshModeEnum.BaseProps);
            }

            _pidl = new PIDL(fullPIDL, false); //0.14 : FileSystemInfoEx record the pidl when construct, as some path do not parasable (e.g. EntireNetwork)
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
            FullName = path;
            Refresh(RefreshModeEnum.BaseProps);
        }
        #endregion

        #region properties
        public RefreshModeEnum RefreshMode { get; private set; }

        public string Label { get; protected set; }

        public string FullName { get; protected set; }

        public FileAttributes Attributes
        {
            get
            {
                return _attributes;
            }

            set { _attributes = value; }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Refresh the file / directory info. Does not refresh directory contents 
        /// because it refresh every time GetFiles/Directories/FileSystemInfos is called.
        /// </summary>
        /// <param name="mode"></param>
        public void Refresh(RefreshModeEnum mode = RefreshModeEnum.AllProps)
        {
            RefreshMode |= mode; //0.23 : Delay loading some properties.

            PIDL relPIDL = null;
            if (getExists() == false)
                refresh(null, null, null, mode);
            else
                try
                {
                    //0.16: Fixed ShellFolder not freed
                    PIDL pidlLookup = this.getPIDL();
                    try
                    {
                        using (ShellFolder2 sf = getParentIShellFolder(pidlLookup, out relPIDL))
                        {
                            refresh(sf, relPIDL, pidlLookup, mode);
                        }
                    }
                    finally
                    {
                        pidlLookup.Free();
                    }
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

        public override string ToString()
        {
            return Label;
        }

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
                { /*Marshal.ThrowExceptionForHR(hr);*/ return null; }
                //Commented because this is part of init and it's too time consuming.
            }
            return new PIDL(pidlPtr, false);
        }

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
                    //Commented because this is part of init and it's too time consuming.
                    /*Marshal.ThrowExceptionForHR(hr);*/
                    return default(IntPtr);
                }
            }

            return pidlPtr;
        }

        //0.12: Fixed PIDL, PIDLRel, ShellFolder, Storage properties generated on demand to avoid x-thread issues.
        protected PIDL getRelPIDL()
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

        protected PIDL getPIDL()
        {
            if (_pidl != null)
                return new PIDL(_pidl, true);

            if (FullName == DirectoryInfoEx.IID_Desktop) // Desktop
                return DirectoryInfoEx.KnownFolderToPIDL(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Desktop));
            else
                return PathToPIDL(FullName);
        }

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

        protected static string PIDLToPath(IShellFolder2 iShellFolder, PIDL pidlRel)
        {
            return loadName(iShellFolder, pidlRel, ShellAPI.SHGNO.FORPARSING);
        }

        protected static PIDL getRelativePIDL(PIDL pidl)
        {
            if (pidl == null)
                return null;
            IntPtr pRelPIDL = PIDL.ILFindLastID(pidl.Ptr);
            if (pRelPIDL == IntPtr.Zero)
                throw new IOException("getRelativePIDL");
            return new PIDL(pRelPIDL, true); //0.21
        }

        protected static PIDL getParentPIDL(PIDL pidl, out PIDL relPIDL)
        {
            relPIDL = new PIDL(pidl, true); //0.21
            if (pidl.Size == 0)
                return pidl;

            IntPtr pParent = PIDL.ILClone(pidl.Ptr);

            relPIDL = getRelativePIDL(pidl);
            if (pParent == IntPtr.Zero || !PIDL.ILRemoveLastID2(ref pParent))
            {
                new PIDL(pParent, false).Free();

                return DirectoryInfoEx.KnownFolderToPIDL(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Desktop));
            }

            return new PIDL(pParent, false); //pParent will be freed by the PIDL.
        }

        protected static PIDL getParentPIDL(PIDL pidl)
        {
            PIDL relPIDL = null;
            try
            {
                return getParentPIDL(pidl, out relPIDL);
            }
            finally { if (relPIDL != null) relPIDL.Free(); }
        }

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

        private void refresh(IShellFolder2 parentShellFolder, PIDL relPIDL, PIDL fullPIDL, RefreshModeEnum mode)
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

        protected static string GetFileName(string path)
        {
            int idx = path.LastIndexOf('\\');

            if (idx == -1)
                return path;

            return path.Substring(idx + 1);
        }

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

        protected string loadName(IShellFolder2 iShellFolder, Header.ShellDll.ShellAPI.SHGNO uFlags)
        {
            return loadName(iShellFolder, new PIDL(IntPtr.Zero, false), uFlags);
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
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_pidlRel != null || _pidl != null)
                System.Threading.Interlocked.Decrement(ref counter);

            if (_pidlRel != null) _pidlRel.Free();

            if (_pidl != null) _pidl.Free();

            _pidlRel = null;
            _pidl = null;

        }
        #endregion
    }
}
