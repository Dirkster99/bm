namespace WSF.Shell.Interop.Dlls
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    /// <summary>
    /// Methods imported from Shlwapi.dll.
    /// 
    /// Shlwapi.DLL Versions:
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb776779(v=vs.85).aspx
    /// </summary>
    [SuppressUnmanagedCodeSecurity, ComVisible(false)]
    internal partial class NativeMethods
    {
        /// <summary>
        /// Converts an STRRET structure
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb759820(v=vs.85).aspx
        /// returned by IShellFolder::GetDisplayNameOf
        /// to a string, and places the result in a buffer.
        /// 
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb773426(v=vs.85).aspx
        /// </summary>
        /// <param name="pstr">A pointer to the STRRET structure. When the function returns, this pointer will no longer be valid.</param>
        /// <param name="pidl">A pointer to the item's ITEMIDLIST structure.</param>
        /// <param name="pszBuf">A buffer to hold the display name. It will be returned as a null-terminated string. If cchBuf is too small, the name will be truncated to fit.</param>
        /// <param name="cchBuf">The size of pszBuf, in characters. If cchBuf is too small, the string will be truncated to fit.</param>
        /// <returns></returns>
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Int32 StrRetToBuf(IntPtr pstr, IntPtr pidl,
                                               [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszBuf,
                                               int cchBuf);
    }
}