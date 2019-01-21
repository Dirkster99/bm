namespace WSF.Shell.Interop.Interfaces.ShellItems
{
    using WSF.Shell.Enums;
    using WSF.Shell;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using WSF.IDs;
    using WSF.Shell.Interop.ShellItems;

    /// <summary>
    /// Extends <see cref="IShellItem"/> with methods that retrieve various property
    /// values of a shell item (file, folder, or drive).
    /// 
    /// <see cref="IShellItem"/> and <see cref="IShellItem2"/> are the preferred representations
    /// of items in any new code.
    /// 
    /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761130(v=vs.85).aspx"/> 
    /// </summary>
    [ComImport,
    Guid(ShellIIDGuids.IShellItem2),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItem2 : IShellItem
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
        new HRESULT BindToHandler(
            [In] IntPtr pbc,
            [In] ref Guid bhid,
            [In] ref Guid riid,
            out IntPtr ppv);

        /// <summary>
        /// Gets the parent of an IShellItem object.
        /// </summary>
        /// <param name="ppsi"></param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new HRESULT GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

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
            [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        /// <summary>
        /// IShellItemArray::GetAttributes
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761096(v=vs.85).aspx"/> 
        /// 
        /// Gets the attributes of the set of items contained in an IShellItemArray.
        /// If the array contains more than one item, the attributes retrieved by this
        /// method are not the attributes of single items, but a logical combination of
        /// all of the requested attributes of all of the items.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetAttributes([In] SFGAOF sfgaoMask, out SFGAOF psfgaoAttribs);

        /// <summary>
        /// Compares two IShellItem objects.
        /// </summary>
        /// <param name="psi"></param>
        /// <param name="hint"></param>
        /// <param name="piOrder"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Compare(
            [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi,
            [In] uint hint,
            out int piOrder);

        /// <summary>
        /// Gets a property store object for specified property store flags.
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761124(v=vs.85).aspx"/>
        /// 
        /// </summary>
        /// <param name="Flags"></param>
        /// <param name="riid"></param>
        /// <param name="ppv"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
        int GetPropertyStore(
            [In] GetPropertyStoreFlags Flags,
            [In] ref Guid riid,
            [Out, MarshalAs(UnmanagedType.Interface)] out IPropertyStore ppv);

        /// <summary>
        /// Uses the specified ICreateObject instead of CoCreateInstance
        /// to create an instance of the property handler associated with
        /// the Shell item on which this method is called.
        /// </summary>
        /// <param name="Flags"></param>
        /// <param name="punkCreateObject"></param>
        /// <param name="riid"></param>
        /// <param name="ppv"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyStoreWithCreateObject([In] GetPropertyStoreFlags Flags, [In, MarshalAs(UnmanagedType.IUnknown)] object punkCreateObject, [In] ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// Gets property store object for specified property keys.
        /// </summary>
        /// <param name="rgKeys"></param>
        /// <param name="cKeys"></param>
        /// <param name="Flags"></param>
        /// <param name="riid"></param>
        /// <param name="ppv"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyStoreForKeys([In] ref PropertyKey rgKeys, [In] uint cKeys, [In] GetPropertyStoreFlags Flags, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.IUnknown)] out IPropertyStore ppv);

        /// <summary>
        /// Gets a property description list object given a reference to a property key.
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="riid"></param>
        /// <param name="ppv"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyDescriptionList([In] ref PropertyKey keyType, [In] ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// Ensures that any cached information in this item is updated.
        /// </summary>
        /// <param name="pbc"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Update([In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc);

        /// <summary>
        /// Gets a PROPVARIANT structure from a specified property key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ppropvar"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetProperty([In] ref PropertyKey key, [Out] PropVariant ppropvar);

        /// <summary>
        /// Gets the CLSID value of specified property key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pclsid"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCLSID([In] ref PropertyKey key, out Guid pclsid);

        /// <summary>
        /// Gets the date and time value of a specified property key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pft"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileTime([In] ref PropertyKey key, out System.Runtime.InteropServices.ComTypes.FILETIME pft);

        /// <summary>
        /// Gets the Int32 value of specified property key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pi"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetInt32([In] ref PropertyKey key, out int pi);

        /// <summary>
        /// Gets the String value of specified property key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ppsz"></param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetString([In] ref PropertyKey key, [MarshalAs(UnmanagedType.LPWStr)] out string ppsz);

        /// <summary>
        /// Gets the UInt32 value of specified property key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pui"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetUInt32([In] ref PropertyKey key, out uint pui);

        /// <summary>
        /// Gets the UInt64 value of specified property key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pull"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetUInt64([In] ref PropertyKey key, out ulong pull);

        /// <summary>
        /// Gets the Boolean value of specified property key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pf"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetBool([In] ref PropertyKey key, out int pf);
    }
}
