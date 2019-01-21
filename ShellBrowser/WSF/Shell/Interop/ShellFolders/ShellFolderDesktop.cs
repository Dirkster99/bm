namespace WSF.Shell.Interop.ShellFolders
{
    using WSF.Shell.Interop.Dlls;
    using WSF.Shell.Interop.Interfaces.ShellFolders;
    using WSF.Shell.Enums;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Class wraps <see cref="IShellFolder2"/> COM interface of the
    /// Desktop Windows Shell object to ensure correct memory management
    /// via <see cref="IDisposable"/> pattern.
    ///
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb775075(v=vs.85).aspx
    /// </summary>
    internal class ShellFolderDesktop : ShellFolder
    {
        /// <summary>
        /// Standard class constructor.
        /// </summary>
        public ShellFolderDesktop() : base()
        {
            IntPtr intPtrShellFolder;
            HRESULT hr = NativeMethods.SHGetDesktopFolder(out intPtrShellFolder);

            if (hr == HRESULT.S_OK && intPtrShellFolder != IntPtr.Zero)
            {
                InitObject(intPtrShellFolder);

                Obj = (IShellFolder2)Marshal.GetTypedObjectForIUnknown(intPtrShellFolder,
                                                                       typeof(IShellFolder2));
            }
        }
    }
}
