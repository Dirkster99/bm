namespace WSF.Shell.Interop.Interfaces
{
    using WSF.Shell.Enums;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Exposes methods that allow a client to retrieve the icon that is associated with one of the objects in a folder.
    /// 
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb761854(v=vs.85).aspx
    /// </summary>
    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214FA-0000-0000-C000-000000000046")]
    public interface IExtractIcon
    {
        /// <summary>
        /// Gets the location and index of an icon.
        /// </summary>
        /// <param name="uFlags"></param>
        /// <param name="pszIconFile"></param>
        /// <param name="cchMax"></param>
        /// <param name="piIndex"></param>
        /// <param name="pwFlags"></param>
        /// <returns></returns>
        [PreserveSig]
        HRESULT GetIconLocation(
            uint uFlags,
            /*[MarshalAs(UnmanagedType.LPTStr)]*/
            StringBuilder pszIconFile,
            uint cchMax,
            ref int piIndex,
            ref uint pwFlags);

        /// <summary>
        /// Extracts an icon image from the specified location.
        /// </summary>
        /// <param name="pzsFile"></param>
        /// <param name="nIconIndex"></param>
        /// <param name="phiconLarge"></param>
        /// <param name="phiconSmall"></param>
        /// <param name="nIconSize"></param>
        /// <returns></returns>
        [PreserveSig]
        HRESULT Extract(
            string pzsFile,
            int nIconIndex,
            ref IntPtr phiconLarge,
            ref IntPtr phiconSmall,
            uint nIconSize);
    }
}
