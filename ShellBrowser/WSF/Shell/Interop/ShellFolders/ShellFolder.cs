namespace WSF.Shell.Interop.ShellFolders
{
    using WSF.Shell.Interop.Dlls;
    using WSF.Shell.Interop.Interfaces.ShellFolders;
    using WSF.Shell.Enums;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using WSF.Shell.Pidl;
    using WSF.Shell.Interop.Interfaces;

    /// <summary>
    /// Class wraps <see cref="IShellFolder2"/> COM interface to ensure
    /// correct memory management via <see cref="IDisposable"/> pattern.
    /// </summary>
    internal class ShellFolder : IDisposable
    {
        #region fields
        private bool _disposed;
        private IntPtr _intPtrShellFolder2 = IntPtr.Zero;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor from <see cref="IShellFolder2"/> interface.
        /// </summary>
        /// <param name="iShellFolder2"></param>
        public ShellFolder(IShellFolder2 iShellFolder2)
            : this()
        {
            InitObject(IntPtr.Zero, iShellFolder2);
        }

        /// <summary>
        /// Class constructor from <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="intPtrShellFolder2"></param>
        public ShellFolder(IntPtr intPtrShellFolder2)
            : this()
        {
            InitObject(intPtrShellFolder2);
        }

        /// <summary>
        /// Initialize object from <paramref name="intPtrShellFolder2"/>
        /// or <paramref name="iShellFolder2"/>. You should use only one
        /// parameter here and set the other to default if necessary.
        /// </summary>
        /// <param name="intPtrShellFolder2"></param>
        /// <param name="iShellFolder2"></param>
        protected void InitObject(IntPtr intPtrShellFolder2,
                                  IShellFolder2 iShellFolder2 = null)
        {
            _intPtrShellFolder2 = intPtrShellFolder2;

            if (_intPtrShellFolder2 != IntPtr.Zero)
            {
                Obj = (IShellFolder2)Marshal.GetTypedObjectForIUnknown(intPtrShellFolder2, typeof(IShellFolder2));
            }
            else
            {
                Obj = iShellFolder2;
            }
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected ShellFolder()
        {
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the encapsulated <see cref="IShellFolder2"/> interface
        /// to let clients invoke a method on this interface.
        /// </summary>
        public IShellFolder2 Obj { get; protected set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets parse and display name items for a ShellFolder2 object.
        /// 
        /// Tpically using either values for
        /// <see cref="SHGDNF.SHGDN_FORPARSING"/>
        /// <see cref="SHGDNF.SHGDN_NORMAL"/> <paramref name="uFlags"/>
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        public string GetShellFolderName(IntPtr ptr,
                                         SHGDNF uFlags)
        {
            if (Obj == null)
            {
                return null;
            }

            IntPtr ptrStr = Marshal.AllocCoTaskMem(NativeMethods.MAX_PATH * 2 + 4);
            Marshal.WriteInt32(ptrStr, 0, 0);
            StringBuilder buf = new StringBuilder(NativeMethods.MAX_PATH);

            try
            {
                if (Obj.GetDisplayNameOf(ptr, uFlags, ptrStr) == HRESULT.S_OK)
                {
                    NativeMethods.StrRetToBuf(ptrStr, ptr, buf, NativeMethods.MAX_PATH);
                }
            }
            finally
            {
                if (ptrStr != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(ptrStr);
                }

                ptrStr = IntPtr.Zero;
            }

            return buf.ToString();
        }
        #region Disposable Interfaces
        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    if (Obj != null)
                    {
                        try
                        {
                            Marshal.ReleaseComObject(Obj);
                        }
                        catch
                        {
                            // Ignoring this on exit of lifetime
                        }
                        finally
                        {
                            Obj = null;
                        }
                    }

                    if (_intPtrShellFolder2 != IntPtr.Zero)
                    {
                        try
                        {
                            Marshal.Release(_intPtrShellFolder2);
                        }
                        catch { }
                        finally
                        {
                            _intPtrShellFolder2 = IntPtr.Zero;
                        }
                    }

                    GC.SuppressFinalize(this);
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            _disposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }
        #endregion Disposable Interfaces
        #endregion methods
    }
}
