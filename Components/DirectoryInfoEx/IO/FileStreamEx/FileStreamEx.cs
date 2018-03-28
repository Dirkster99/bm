///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2009 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Steven Roebert's work (http://www.codeproject.com/KB/miscctrl/FileBrowser.aspx)    //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using ShellDll;
using System.Runtime.InteropServices;

namespace System.IO
{
    public class FileStreamEx : Stream, IDisposable
    {
        private IntPtr streamPtr; private IStream stream;
        private IStorage tempParentStorage; private IntPtr tempParentStoragePtr;
        private string _fileName;
        private bool _canRead, _canWrite;
        private bool _disposed = false;
        private long currentPos;
        private ShellAPI.STATSTG streamInfo;   

        protected virtual void closeStream()
        {
            base.Close();

            if (stream != null)
            {
                Marshal.ReleaseComObject(stream);
                Marshal.Release(streamPtr);
            }
        }


        protected virtual void init(string file, FileMode mode, FileAccess access)
        {
            _fileName = file;
            string dirPath = PathEx.GetDirectoryName(file);
            DirectoryInfoEx dir = new DirectoryInfoEx(dirPath);
            if (dir == null) throw new IOException(String.Format("Directory not exists: {0}", dirPath));
            if (!IOTools.getIStorage(dir, out tempParentStoragePtr, out tempParentStorage))
                throw new IOException(String.Format("Failed to obtaiin IStorage: {0}", dirPath));
            IOTools.openStream(tempParentStorage, file, ref mode, ref access, out streamPtr, out stream);

            this._canRead = (access == FileAccess.Read || access == FileAccess.ReadWrite);
            this._canWrite = (access == FileAccess.Write || access == FileAccess.ReadWrite);
            currentPos = 0;
            
            if (_canRead) stream.Stat(out streamInfo, ShellAPI.STATFLAG.NONAME);

            switch (mode)
            {
                case FileMode.Append :
                    Seek(0, SeekOrigin.End);                    
                    break;
                case FileMode.CreateNew:
                    
                    break;
            }
        }

        protected void init(string file, FileMode mode)
        {
            FileAccess access = FileAccess.Read;
            switch (mode)
            {
                case FileMode.Append : access = FileAccess.ReadWrite; break;
                case FileMode.Create : access = FileAccess.Write; break;
                case FileMode.CreateNew : access = FileAccess.Write; break;
                case FileMode.OpenOrCreate: access = FileAccess.Write; break;
                case FileMode.Truncate: access = FileAccess.Write; break;
            }
            init(file, mode, access);
        }

        protected void init(string file, FileAccess access)
        {
            switch (access)
            {
                case FileAccess.Read:
                    init(file, FileMode.Open, access);
                    break;
                default:
                    init(file, FileMode.OpenOrCreate, access);
                    break;
            }
        }

        public FileStreamEx(string file, FileAccess access)
        {
            init(file, access);
        }

        public FileStreamEx(string file, FileMode mode, FileAccess access)
        {
            init(file, mode, access);
        }

        public FileStreamEx(string file, FileMode mode)
        {
            init(file, mode);
        }

        protected FileStreamEx() //Used by child class, construct using init()
        {

        }

        public override void Close()
        {            
            this.Dispose();
            base.Close();
        }

        ~FileStreamEx()
        {
            closeStream();
            ((IDisposable)this).Dispose();
        }

        #region IDisposable Members

        public new void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (stream != null)
                {
                    Marshal.ReleaseComObject(stream);
                    stream = null;

                    Marshal.ReleaseComObject(tempParentStorage);
                    tempParentStorage = null;
                }

                if (streamPtr != IntPtr.Zero)
                {
                    Marshal.Release(streamPtr);
                    streamPtr = IntPtr.Zero;

                    Marshal.Release(tempParentStoragePtr);
                    tempParentStoragePtr = IntPtr.Zero;
                }

                GC.SuppressFinalize(this);
            }
        }


        #endregion

        #region Stream Members
        public override bool CanRead { get { return this._canRead; } }       
        public override bool CanSeek { get { return true; } }
        public override bool CanWrite { get { return this._canWrite; } }
        public override long Length { get { return _canRead ? streamInfo.cbSize : 0; } }
        public override long Position { get { return currentPos; } set { Seek(currentPos, SeekOrigin.Begin); } }


        public override void Flush() { stream.Commit(ShellAPI.STGC.DEFAULT); }
        public override void SetLength(long value) { if (CanWrite) stream.SetSize(value); }        
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (CanRead)
            {
                byte[] readBytes = new byte[count];

                IntPtr readNrPtr = Marshal.AllocCoTaskMem(32);
                stream.Read(readBytes, count, readNrPtr);

                int readNr = (int)Marshal.PtrToStructure(readNrPtr, typeof(Int32));
                Marshal.FreeCoTaskMem(readNrPtr);

                Array.Copy(readBytes, 0, buffer, offset, readNr);
                currentPos += readNr;
                return readNr;
            }
            else
                return 0;
        }            
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (CanWrite)
            {
                byte[] writeBytes = new byte[count];
                Array.Copy(buffer, offset, writeBytes, 0, count);

                IntPtr writtenNrPtr = Marshal.AllocCoTaskMem(64);
                stream.Write(writeBytes, count, writtenNrPtr);

                long writtenNr = (long)Marshal.PtrToStructure(writtenNrPtr, typeof(Int64));
                Marshal.FreeCoTaskMem(writtenNrPtr);

                currentPos += writtenNr;
            }
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (CanSeek)
            {
                IntPtr newPosPtr = Marshal.AllocCoTaskMem(64);
                stream.Seek(offset, origin, newPosPtr);

                long newPos = (long)Marshal.PtrToStructure(newPosPtr, typeof(Int64));
                Marshal.FreeCoTaskMem(newPosPtr);

                return (currentPos = newPos);
            }
            else
                return -1;
        }
        #endregion


    }
}
