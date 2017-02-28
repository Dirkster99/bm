using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ShellDll;
using System.Security;
using System.IO;

namespace PreviewHandlerWPF
{
    public sealed class COMStream : IStream, IDisposable
    {
        Stream _stream;

        ~COMStream()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream.Dispose();
                _stream = null;
            }
        }

        private COMStream() { }

        public COMStream(Stream sourceStream)
        {
            _stream = sourceStream;
        }

        #region IStream Members

        public void Clone(out IStream ppstm)
        {
            throw new NotSupportedException();
        }

        public void Commit(int grfCommitFlags)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            throw new NotSupportedException();
        }

        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotSupportedException();
        }

        [SecurityCritical]
        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            int count = this._stream.Read(pv, 0, cb);
            if (pcbRead != IntPtr.Zero)
            {
                Marshal.WriteInt32(pcbRead, count);
            }
        }

        public void Revert()
        {
            throw new NotSupportedException();
        }

        [SecurityCritical]
        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            SeekOrigin origin = (SeekOrigin)dwOrigin;
            long pos = this._stream.Seek(dlibMove, origin);
            if (plibNewPosition != IntPtr.Zero)
            {
                Marshal.WriteInt64(plibNewPosition, pos);
            }
        }

        public void SetSize(long libNewSize)
        {
            this._stream.SetLength(libNewSize);
        }

        public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
            pstatstg.type = 2;
            pstatstg.cbSize = this._stream.Length;
            pstatstg.grfMode = 0;
            if (this._stream.CanRead && this._stream.CanWrite)
            {
                pstatstg.grfMode |= 2;
            }
            else if (this._stream.CanWrite && !_stream.CanRead)
            {
                pstatstg.grfMode |= 1;
            }
            else
            {
                throw new IOException();
            }

        }

        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotSupportedException();
        }

        [SecurityCritical]
        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            this._stream.Write(pv, 0, cb);
            if (pcbWritten != IntPtr.Zero)
            {
                Marshal.WriteInt32(pcbWritten, cb);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this._stream != null)
            {
                this._stream.Close();
                this._stream.Dispose();
                this._stream = null;
            }
        }

        #endregion
    }
   
}
