namespace DirectoryInfoExLib.IO.Tools.Interface
{
    using System;
    using System.Runtime.InteropServices;
    using DirectoryInfoExLib.IO.Header.ShellDll.Interfaces;

    internal class Storage : IStorage, IDisposable
    {
        private IStorage _iStorage = null;
        private IntPtr _ptrStorage = IntPtr.Zero;

        internal Storage(IntPtr storagePtr)
        {
            _iStorage = (IStorage)Marshal.GetTypedObjectForIUnknown(storagePtr, typeof(IStorage));
        }

        #region IDisposable Members

        ~Storage()
        {
            ((IDisposable)this).Dispose();
        }

        public void Dispose()
        {
            if (_iStorage != null)
            {
                Marshal.ReleaseComObject(_iStorage);
                _iStorage = null;
            }

            if (_ptrStorage != IntPtr.Zero)
            {
                Marshal.Release(_ptrStorage);
                _ptrStorage = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);

        }

        #endregion

        #region IStorage Members

        public int CreateStream(string pwcsName, Header.ShellDll.ShellAPI.STGM grfMode, int reserved1, int reserved2, out IntPtr ppstm)
        {
            return _iStorage.CreateStream(pwcsName, grfMode, reserved1, reserved2, out ppstm);
        }

        public int OpenStream(string pwcsName, IntPtr reserved1, Header.ShellDll.ShellAPI.STGM grfMode, int reserved2, out IntPtr ppstm)
        {
            return _iStorage.OpenStream(pwcsName, reserved1, grfMode, reserved2, out ppstm);
        }

        public int CreateStorage(string pwcsName, Header.ShellDll.ShellAPI.STGM grfMode, int reserved1, int reserved2, out IntPtr ppstg)
        {
            return _iStorage.CreateStorage(pwcsName, grfMode, reserved1, reserved2, out ppstg);
        }

        public int OpenStorage(string pwcsName, IStorage pstgPriority, Header.ShellDll.ShellAPI.STGM grfMode, IntPtr snbExclude, int reserved, out IntPtr ppstg)
        {
            return _iStorage.OpenStorage(pwcsName, pstgPriority, grfMode, snbExclude, reserved, out ppstg);
        }

        public int CopyTo(int ciidExclude, ref Guid rgiidExclude, IntPtr snbExclude, IStorage pstgDest)
        {
            return _iStorage.CopyTo(ciidExclude, ref rgiidExclude, snbExclude, pstgDest);
        }

        public int MoveElementTo(string pwcsName, IStorage pstgDest, string pwcsNewName, Header.ShellDll.ShellAPI.STGMOVE grfFlags)
        {
            return _iStorage.MoveElementTo(pwcsName, pstgDest, pwcsName, grfFlags);
        }

        public int Commit(Header.ShellDll.ShellAPI.STGC grfCommitFlags)
        {
            return _iStorage.Commit(grfCommitFlags);
        }

        public int Revert()
        {
            return _iStorage.Revert();
        }

        public int EnumElements(int reserved1, IntPtr reserved2, int reserved3, out IntPtr ppenum)
        {
            return _iStorage.EnumElements(reserved1, reserved2, reserved3, out ppenum);
        }

        public int DestroyElement(string pwcsName)
        {
            return _iStorage.DestroyElement(pwcsName);
        }

        public int RenameElement(string pwcsOldName, string pwcsNewName)
        {
            return _iStorage.RenameElement(pwcsOldName, pwcsNewName);
        }

        public int SetElementTimes(string pwcsName, Header.ShellDll.ShellAPI.FILETIME pctime, Header.ShellDll.ShellAPI.FILETIME patime, Header.ShellDll.ShellAPI.FILETIME pmtime)
        {
            return _iStorage.SetElementTimes(pwcsName, pctime, patime, pmtime);
        }

        public int SetClass(ref Guid clsid)
        {
            return _iStorage.SetClass(ref clsid);
        }

        public int SetStateBits(int grfStateBits, int grfMask)
        {
            return _iStorage.SetStateBits(grfStateBits, grfMask);
        }

        public int Stat(out Header.ShellDll.ShellAPI.STATSTG pstatstg, Header.ShellDll.ShellAPI.STATFLAG grfStatFlag)
        {
            return _iStorage.Stat(out pstatstg, grfStatFlag);
        }

        #endregion
    }
}
