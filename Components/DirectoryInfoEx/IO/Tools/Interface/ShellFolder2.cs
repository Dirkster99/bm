///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2009 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Steven Roebert's work (http://www.codeproject.com/KB/miscctrl/FileBrowser.aspx)    //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace DirectoryInfoExLib.IO.Tools.Interface
{
    using System;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using DirectoryInfoExLib.IO.Header;

    public class ShellFolder2 : IShellFolder2, IDisposable
    {
        private IShellFolder2 _iShellFolder2 = null;
        private IntPtr _ptrShellFolder2 = IntPtr.Zero;
        private bool _disposed = false;

        internal ShellFolder2(IntPtr ptrShellFolder2)
        {
            _ptrShellFolder2 = ptrShellFolder2;
            _iShellFolder2 = (IShellFolder2)Marshal.GetTypedObjectForIUnknown(ptrShellFolder2, typeof(IShellFolder2));
        }

        private void checkDisposed()
        {
            if (_disposed) throw new Exception("ShellFolder2 already disposed");
        }

        #region IShellFolder Members

        public int ParseDisplayName(IntPtr hwnd, IntPtr pbc, string pszDisplayName, ref uint pchEaten, out IntPtr ppidl, ref Header.ShellDll.ShellAPI.SFGAO pdwAttributes)
        {
            checkDisposed();
            return _iShellFolder2.ParseDisplayName(hwnd, pbc, pszDisplayName, ref pchEaten, out ppidl, ref pdwAttributes);
        }

        public int EnumObjects(IntPtr hwnd, Header.ShellDll.ShellAPI.SHCONTF grfFlags, out IntPtr enumIDList)
        {
            checkDisposed();
            return _iShellFolder2.EnumObjects(hwnd, grfFlags, out enumIDList);
        }

        public int BindToObject(IntPtr pidl, IntPtr pbc, ref Guid riid, out IntPtr ppv)
        {
            checkDisposed();
            return _iShellFolder2.BindToObject(pidl, pbc, ref riid, out ppv);
        }

        public int BindToStorage(IntPtr pidl, IntPtr pbc, ref Guid riid, out IntPtr ppv)
        {
            checkDisposed();
            return _iShellFolder2.BindToStorage(pidl, pbc, ref riid, out ppv);
        }

        public int CompareIDs(IntPtr lParam, IntPtr pidl1, IntPtr pidl2)
        {
            checkDisposed();
            return _iShellFolder2.CompareIDs(lParam, pidl1, pidl2);
        }

        public int CreateViewObject(IntPtr hwndOwner, Guid riid, out IntPtr ppv)
        {
            checkDisposed();
            return _iShellFolder2.CreateViewObject(hwndOwner, riid, out ppv);
        }

        public int GetAttributesOf(uint cidl, IntPtr[] apidl, ref Header.ShellDll.ShellAPI.SFGAO rgfInOut)
        {
            checkDisposed();
            return _iShellFolder2.GetAttributesOf(cidl, apidl, ref rgfInOut);
        }

        public int GetUIObjectOf(IntPtr hwndOwner, uint cidl, IntPtr[] apidl, ref Guid riid, IntPtr rgfReserved, out IntPtr ppv)
        {
            checkDisposed();
            return _iShellFolder2.GetUIObjectOf(hwndOwner, cidl, apidl, ref riid, rgfReserved, out ppv);
        }

        public int GetDisplayNameOf(IntPtr pidl, Header.ShellDll.ShellAPI.SHGNO uFlags, IntPtr lpName)
        {
            checkDisposed();
            return _iShellFolder2.GetDisplayNameOf(pidl, uFlags, lpName);
        }

        public int SetNameOf(IntPtr hwnd, IntPtr pidl, string pszName, Header.ShellDll.ShellAPI.SHGNO uFlags, out IntPtr ppidlOut)
        {
            checkDisposed();
            return _iShellFolder2.SetNameOf(hwnd, pidl, pszName, uFlags, out ppidlOut);
        }

        #endregion

        #region IShellFolder2 Members

        public int EnumSearches(ref IEnumExtraSearch ppEnum)
        {
            checkDisposed();
            return _iShellFolder2.EnumSearches(ref ppEnum);
        }

        public int GetDefaultColumn(uint dwReserved, out ulong pSort, out ulong pDisplay)
        {
            checkDisposed();
            return _iShellFolder2.GetDefaultColumn(dwReserved, out pSort, out pDisplay);
        }

        public int GetDefaultColumnState(uint iColumn, out SHCOLSTATEF pcsFlags)
        {
            checkDisposed();
            return _iShellFolder2.GetDefaultColumnState(iColumn, out pcsFlags);
        }

        public int GetDefaultSearchGUID(out Guid pguid)
        {
            checkDisposed();
            return _iShellFolder2.GetDefaultSearchGUID(out pguid);
        }

        public int GetDetailsEx(IntPtr pidl, ref PropertyKey pscid, out object pv)
        {
            checkDisposed();
            return _iShellFolder2.GetDetailsEx(pidl, ref pscid, out pv);
        }

        public int GetDetailsOf(IntPtr pidl, uint iColumn, out ShellDetails psd)
        {
            checkDisposed();
            return _iShellFolder2.GetDetailsOf(pidl, iColumn, out psd);
        }

        public int MapColumnToSCID(uint iColumn, out PropertyKey pscid)
        {
            checkDisposed();
            return _iShellFolder2.MapColumnToSCID(iColumn, out pscid);
        }

        #endregion

        #region IDisposable Members

        ~ShellFolder2()
        {
            ((IDisposable)this).Dispose();
        }

        public void Dispose()
        {
            _disposed = true;
            if (_iShellFolder2 != null)
            {
                try
                {
                    Marshal.ReleaseComObject(_iShellFolder2);
                    _iShellFolder2 = null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine("Exception thorown when releasing IShellFolder2.");
                }
            }

            if (_ptrShellFolder2 != IntPtr.Zero)
            {
                try
                {
                    Marshal.Release(_ptrShellFolder2);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception {0} thrown when releasing IShellFolder2's Ptr.", ex.Message);
                }
                finally
                {
                    _ptrShellFolder2 = IntPtr.Zero;
                }
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
