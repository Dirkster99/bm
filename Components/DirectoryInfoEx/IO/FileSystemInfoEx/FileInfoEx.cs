///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2009 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Steven Roebert's work (http://www.codeproject.com/KB/miscctrl/FileBrowser.aspx)    //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using ShellDll;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.IO
{
    /// <summary>
    /// Represent a file in PIDL system
    /// </summary>
    public class FileInfoEx : FileSystemInfoEx, ISerializable, ICloneable
    {
        #region Variables
        private Int64 _length;
        private bool _isReadOnly;


        public Int64 Length { get { checkRefresh(); return _length; } set { _length = value; } } //Size of the file
        public bool IsReadOnly { get { checkRefresh(); return _isReadOnly; } set { _isReadOnly = value; } }//Is the file readonly?
        public DirectoryInfoEx Directory { get { return Parent; } } //Owner of the file.
        public string DirectoryName { get { return Directory.FullName; } } //Full path of the owner.
        #endregion

        #region Methods
        protected void checkExists()
        {
            if (!Exists)
                throw new FileNotFoundException(FullName + " is not exists.");
        }

        protected override void refresh(IShellFolder2 parentShellFolder, PIDL relPIDL, PIDL fullPIDL, RefreshModeEnum mode)
        {
            base.refresh(parentShellFolder, relPIDL, fullPIDL, mode);

            if ((mode & RefreshModeEnum.FullProps) != 0 &&
                parentShellFolder != null && relPIDL != null && fullPIDL != null)
            {

                if (!FullName.StartsWith("::") && File.Exists(FullName))
                    try
                    {
                        FileInfo fi = new FileInfo(FullName);
                        IsReadOnly = fi.IsReadOnly;
                        Attributes = fi.Attributes;
                        Length = fi.Length;
                        LastAccessTime = fi.LastAccessTime;
                        LastWriteTime = fi.LastWriteTime;
                        CreationTime = fi.CreationTime;
                    }
                    catch { }
                else //0.18: Uses File to return FileInfo by default
                {
                    ShellAPI.SFGAO attribute = shGetFileAttribute(fullPIDL, ShellAPI.SFGAO.READONLY);
                    IsReadOnly = (attribute & ShellAPI.SFGAO.READONLY) != 0;

                    Length = 0;
                }
            }
        }

        public new void Delete()
        {
            checkExists();
            int hr = Parent.Storage.DestroyElement(Name);
            if (hr != ShellAPI.S_OK) Marshal.ThrowExceptionForHR(hr);
            Refresh();
        }

        public void MoveTo(string destFileName)
        {
            checkExists();
            destFileName = IOTools.ExpandPath(destFileName);
            IOTools.Move(FullName, destFileName);
            FullName = destFileName;
            OriginalPath = FullName;
            Refresh();
        }

        public FileStreamEx OpenRead()
        {
            return Open(FileMode.Open, FileAccess.Read);
        }

        public StreamWriter AppendText()
        {
            return new StreamWriter(Open(FileMode.Append, FileAccess.ReadWrite));
        }

        public StreamReader OpenText()
        {
            return new StreamReader(OpenRead());
        }

        public FileStreamEx Open(FileMode mode, FileAccess access)
        {
            return new FileStreamEx(FullName, mode, access);
        }

        public FileStreamEx Open(FileMode mode)
        {
            return new FileStreamEx(FullName, mode);
        }

        public FileStreamEx Open()
        {
            return new FileStreamEx(FullName, FileAccess.ReadWrite);
        }

        /// <summary>
        /// Create a file, returns it's filestream.
        /// </summary>
        public FileStreamEx Create()
        {
            return new FileStreamEx(FullName, FileMode.Create, FileAccess.ReadWrite);
        }
        #endregion

        #region Constructors
        protected override void checkProperties()
        {
            base.checkProperties();
            if (Exists && (Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                throw new IOException(FullName + " is not a file.");
        }

        internal FileInfoEx(PIDL fullPIDL, bool freeFullPIDL)
        {
            init(fullPIDL);
            checkProperties();
            if (freeFullPIDL)
                fullPIDL.Free();
        }

        public FileInfoEx(string fullPath)
        {
            fullPath = IOTools.ExpandPath(fullPath);
            init(fullPath);
            checkProperties();
        }

        public FileInfoEx(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IsReadOnly = info.GetBoolean("IsReadOnly");
            Length = info.GetInt64("Length");
        }

        internal FileInfoEx(IShellFolder2 parentShellFolder, PIDL fullPIDL)
        {
            init(parentShellFolder, fullPIDL);
        }

        internal FileInfoEx(IShellFolder2 parentShellFolder, PIDL parentPIDL, PIDL relPIDL)
        {
            init(parentShellFolder, parentPIDL, relPIDL);
        }

        internal FileInfoEx(IShellFolder2 parentShellFolder, DirectoryInfoEx parentDir, PIDL relPIDL)
        {
            Parent = parentDir;
            parentDir.RequestPIDL(parentPIDL =>
                {
                    init(parentShellFolder, parentPIDL, relPIDL);
                });            
        }

        internal FileInfoEx(DirectoryInfoEx parentDir, PIDL relPIDL)
        {
            Parent = parentDir;
            //0.15: Fixed ShellFolder not freed.
            using (ShellFolder2 parentShellFolder = parentDir.ShellFolder)
                parentDir.RequestRelativePIDL(parentRelPidl =>
                    init(parentShellFolder, parentRelPidl, relPIDL));
        }

        protected FileInfoEx()
        {

        }
        #endregion

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("IsReadOnly", IsReadOnly);
            info.AddValue("Length", Length);
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return new FileInfoEx(this.FullName);
        }

        #endregion
    }
}
