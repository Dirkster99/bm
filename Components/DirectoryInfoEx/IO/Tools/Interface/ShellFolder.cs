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
    public class ShellFolder : IShellFolder, IDisposable
    {
        private IShellFolder _iShellFolder = null;
        private IntPtr _ptrShellFolder = IntPtr.Zero;
        private bool _disposed = false;

        internal ShellFolder(IntPtr ptrShellFolder)
        {
            _ptrShellFolder = ptrShellFolder;
            _iShellFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(ptrShellFolder, typeof(IShellFolder));
        }

        private void checkDisposed()
        {
            if (_disposed) throw new Exception("ShellFolder already disposed");
        }

        #region IShellFolder Members

        public int ParseDisplayName(IntPtr hwnd, IntPtr pbc, string pszDisplayName, ref uint pchEaten, out IntPtr ppidl, ref ShellAPI.SFGAO pdwAttributes)
        {
            checkDisposed();
            return _iShellFolder.ParseDisplayName(hwnd, pbc, pszDisplayName, ref pchEaten, out ppidl, ref pdwAttributes);
        }

        public int EnumObjects(IntPtr hwnd, ShellAPI.SHCONTF grfFlags, out IntPtr enumIDList)
        {
            checkDisposed();
            return _iShellFolder.EnumObjects(hwnd, grfFlags, out enumIDList);
        }

        public int BindToObject(IntPtr pidl, IntPtr pbc, ref Guid riid, out IntPtr ppv)
        {
            checkDisposed();
            return _iShellFolder.BindToObject(pidl, pbc, ref riid, out ppv);
        }

        public int BindToStorage(IntPtr pidl, IntPtr pbc, ref Guid riid, out IntPtr ppv)
        {
            checkDisposed();
            return _iShellFolder.BindToStorage(pidl, pbc, ref riid, out ppv);
        }

        public int CompareIDs(IntPtr lParam, IntPtr pidl1, IntPtr pidl2)
        {
            checkDisposed();
            return _iShellFolder.CompareIDs(lParam, pidl1, pidl2);
        }

        public int CreateViewObject(IntPtr hwndOwner, Guid riid, out IntPtr ppv)
        {
            checkDisposed();
            return _iShellFolder.CreateViewObject(hwndOwner, riid, out ppv);
        }

        public int GetAttributesOf(uint cidl, IntPtr[] apidl, ref ShellAPI.SFGAO rgfInOut)
        {
            checkDisposed();
            return _iShellFolder.GetAttributesOf(cidl, apidl, ref rgfInOut);
        }

        public int GetUIObjectOf(IntPtr hwndOwner, uint cidl, IntPtr[] apidl, ref Guid riid, IntPtr rgfReserved, out IntPtr ppv)
        {
            checkDisposed();
            return _iShellFolder.GetUIObjectOf(hwndOwner, cidl, apidl, ref riid, rgfReserved, out ppv);
        }

        public int GetDisplayNameOf(IntPtr pidl, ShellAPI.SHGNO uFlags, IntPtr lpName)
        {
            checkDisposed();
            return _iShellFolder.GetDisplayNameOf(pidl, uFlags, lpName);
        }

        public int SetNameOf(IntPtr hwnd, IntPtr pidl, string pszName, ShellAPI.SHGNO uFlags, out IntPtr ppidlOut)
        {
            checkDisposed();
            return _iShellFolder.SetNameOf(hwnd, pidl, pszName, uFlags, out ppidlOut);
        }

        #endregion

        #region IDisposable Members

        ~ShellFolder()
        {
            ((IDisposable)this).Dispose();
        }

        public void Dispose()
        {
            _disposed = true;
            if (_iShellFolder != null)
            {
                Marshal.ReleaseComObject(_iShellFolder);
                _iShellFolder = null;
            }

            if (_ptrShellFolder != IntPtr.Zero)
            {
                try
                {
                    Marshal.Release(_ptrShellFolder);
                }
                catch (Exception) { }
                finally
                {
                    _ptrShellFolder = IntPtr.Zero;
                }
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
