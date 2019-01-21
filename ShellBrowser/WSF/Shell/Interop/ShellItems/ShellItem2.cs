namespace WSF.Shell.Interop.ShellFolders
{
    using WSF.Shell.Enums;
    using WSF.Shell.Interop.Interfaces.ShellItems;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Class wraps <see cref="ShellItem2"/> COM interface to ensure
    /// correct memory management via <see cref="IDisposable"/> pattern.
    /// </summary>
    public class ShellItem2 : IDisposable
    {
        #region fields
        private bool _disposed = false;
        private IntPtr _intPtrShellFolder2 = IntPtr.Zero;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor from <see cref="ShellItem2"/> interface.
        /// </summary>
        /// <param name="iShellItem2"></param>
        public ShellItem2(IShellItem2 iShellItem2)
            : this()
        {
            InitObject(IntPtr.Zero, iShellItem2);
        }

        /// <summary>
        /// Class constructor from <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="intPtrShellFolder2"></param>
        public ShellItem2(IntPtr intPtrShellFolder2)
            : this()
        {
            InitObject(intPtrShellFolder2);
        }

        /// <summary>
        /// Initializr objrct from <paramref name="intPtrShellFolder2"/>
        /// or <paramref name="iShellItem2"/>. You should use only one
        /// parameter here and set the other to default if necessary.
        /// </summary>
        /// <param name="intPtrShellFolder2"></param>
        /// <param name="iShellItem2"></param>
        protected void InitObject(IntPtr intPtrShellFolder2,
                                  IShellItem2 iShellItem2 = null)
        {
            _intPtrShellFolder2 = intPtrShellFolder2;

            if (_intPtrShellFolder2 != IntPtr.Zero)
                Obj = (IShellItem2)Marshal.GetTypedObjectForIUnknown(intPtrShellFolder2, typeof(IShellItem2));
            else
                Obj = iShellItem2;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected ShellItem2()
        {
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the encapsulated <see cref="ShellItem2"/> interface
        /// to let clients invoke a method on this interface.
        /// </summary>
        public IShellItem2 Obj { get; protected set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets the display name of the shell item or null if name is not available.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public string GetDisplayName(SIGDN flags)
        {
            IntPtr ppszName = default(IntPtr);
            string result = null;
            try
            {

                if (this.Obj.GetDisplayName(flags, out ppszName) == HRESULT.S_OK)
                    result = Marshal.PtrToStringUni(ppszName);
            }
            finally
            {
                if (ppszName != default(IntPtr))
                    Marshal.FreeCoTaskMem(ppszName);
            }

            return result;
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
                        catch{}
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
