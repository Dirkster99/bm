namespace WSF.Shell.Interop.ShellFolders
{
    using WSF.Shell.Interop.Interfaces.ShellFolders;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Class wraps COM interfaces to ensure correct memory management via
    /// <see cref="IDisposable"/> pattern. This class can be used with various
    /// COM interfaces such as IShellItem or IShellFolder and so forth...
    /// </summary>
    internal class GenericCOMFolder<T> : IDisposable
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool _disposed = false;
        private IntPtr _intPtrICOMInterface = IntPtr.Zero;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor from (eg: <see cref="IShellFolder2"/>) interface.
        /// </summary>
        /// <param name="iCOMInterface"></param>
        public GenericCOMFolder(T iCOMInterface)
            : this()
        {
            InitObject(IntPtr.Zero, iCOMInterface);
        }

        /// <summary>
        /// Class constructor from <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="intPtriCOMInterface"></param>
        public GenericCOMFolder(IntPtr intPtriCOMInterface)
            : this()
        {
            InitObject(intPtriCOMInterface);
        }

        /// <summary>
        /// Initializr object from <paramref name="intPtrICOMInterface"/>
        /// or <paramref name="ICOMInterface"/>. You should use only one
        /// parameter here and set the other to default if necessary.
        /// </summary>
        /// <param name="intPtrICOMInterface"></param>
        /// <param name="ICOMInterface"></param>
        protected void InitObject(IntPtr intPtrICOMInterface,
                                  T ICOMInterface = default(T))
        {
            _intPtrICOMInterface = intPtrICOMInterface;

            if (_intPtrICOMInterface != IntPtr.Zero)
            {
                Obj = (T)Marshal.GetTypedObjectForIUnknown(intPtrICOMInterface, typeof(T));
            }
            else
                Obj = ICOMInterface;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected GenericCOMFolder()
        {
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets an encapsulated (eg: <see cref="IShellFolder2"/>) interface
        /// to let clients invoke a method on this interface.
        /// </summary>
        public T Obj { get; protected set; }
        #endregion properties

        #region methods
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
                        catch (Exception exp)
                        {
                            Logger.Error(exp);
                        }
                        finally
                        {
                            Obj = default(T);
                        }
                    }

                    if (_intPtrICOMInterface != IntPtr.Zero)
                    {
                        try
                        {
                            Marshal.Release(_intPtrICOMInterface);
                        }
                        catch { }
                        finally
                        {
                            _intPtrICOMInterface = IntPtr.Zero;
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
