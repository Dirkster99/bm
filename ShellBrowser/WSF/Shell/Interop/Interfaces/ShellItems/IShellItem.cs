namespace WSF.Shell.Interop.Interfaces.ShellItems
{
    using WSF.Shell.Enums;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using WSF.IDs;

    /// <summary>
    /// Exposes methods that retrieve information about a Shell item.
    /// IShellItem and IShellItem2 are the preferred representations of items in any new code.
    /// 
    /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761144(v=vs.85).aspx"/> 
    /// </summary>
    [ComImport,
    Guid(ShellIIDGuids.IShellItem),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItem
    {
        /// <summary>
        /// Binds to a handler for an item as specified by the handler ID value (BHID).
        /// </summary>
        /// <param name="pbc"></param>
        /// <param name="bhid"></param>
        /// <param name="riid"></param>
        /// <param name="ppv"></param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT BindToHandler(
            [In] IntPtr pbc,
            [In] ref Guid bhid,
            [In] ref Guid riid,
            out IntPtr ppv);

        /// <summary>
        /// Gets the parent of an IShellItem object.
        /// </summary>
        /// <param name="ppsi"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// Gets the display name of the IShellItem object.
        /// </summary>
        /// <param name="sigdnName"></param>
        /// <param name="ppszName"></param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetDisplayName(
            [In] SIGDN sigdnName,
            out IntPtr ppszName);

        /// <summary>
        /// Gets a requested set of attributes of the IShellItem object.
        /// </summary>
        /// <param name="sfgaoMask"></param>
        /// <param name="psfgaoAttribs"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAttributes([In] SFGAOF sfgaoMask, out SFGAOF psfgaoAttribs);

        /// <summary>
        /// Compares two IShellItem objects.
        /// </summary>
        /// <param name="psi"></param>
        /// <param name="hint"></param>
        /// <param name="piOrder"></param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Compare(
            [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi,
            [In] SICHINTF hint,
            out int piOrder);
    }
}
