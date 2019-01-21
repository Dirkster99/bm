namespace WSF.Shell.Interop.Interfaces.ShellFolders
{
    using WSF.IDs;
    using WSF.Shell.Enums;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Extends the capabilities of IShellFolder.
    /// Its methods provide a variety of information about the contents of a Shell folder.
    /// 
    /// https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nn-shobjidl_core-ishellfolder2
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid(ShellIIDGuids.IShellFolder2)]
    internal interface IShellFolder2 : IShellFolder
    {
        #region IShellFolder overrides for COM/C# compatibility.
        [PreserveSig]
        new HRESULT ParseDisplayName(IntPtr hwnd,
                                   IntPtr pbc,
                                   [MarshalAs(UnmanagedType.LPWStr)]
                                   string pszDisplayName,
                                   ref uint pchEaten,
                                   out IntPtr ppidl,
                                   ref SFGAO pdwAttributes);

        [PreserveSig]
        new HRESULT EnumObjects(IntPtr hwnd, SHCONTF grfFlags, out IntPtr ppenumIDList);

        [PreserveSig]
        new HRESULT BindToObject(IntPtr pidl, IntPtr pbc, [In] ref Guid riid, out IntPtr ppv);

        [PreserveSig]
        new Int32 BindToStorage(IntPtr pidl, IntPtr pbc, [In] ref Guid riid, out IntPtr ppv);

        [PreserveSig]
        new Int32 CompareIDs(IntPtr lParam, IntPtr pidl1, IntPtr pidl2);

        [PreserveSig]
        new Int32 CreateViewObject(IntPtr hwndOwner, [In] ref Guid riid, out IntPtr ppv);

        [PreserveSig]
        new int GetAttributesOf(UInt32 cidl,
                                [MarshalAs(UnmanagedType.LPArray)]
                                IntPtr[] apidl,
                                ref SFGAO rgfInOut);

        [PreserveSig]
        new HRESULT GetUIObjectOf([In] IntPtr hwndOwner,
                                  [In] UInt32 cidl,
                                  [MarshalAs(UnmanagedType.LPArray)]
                                  [In] IntPtr[] apidl,
                                  [In] ref Guid riid,
                                  [In, Out] IntPtr rgfReserved,
                                  [Out] out IntPtr ppv);

        [PreserveSig]
        new HRESULT GetDisplayNameOf(IntPtr pidl, SHGDNF uFlags, IntPtr lpName);

        [PreserveSig]
        new Int32 SetNameOf(IntPtr hwnd, IntPtr pidl,
                          [MarshalAs(UnmanagedType.LPWStr)]
                          String pszName, SHGDNF uFlags, out IntPtr ppidlOut);
        #endregion

        /// <summary>
        /// Returns the globally unique identifier (GUID) of the default search object for the folder.
        /// </summary>
        /// <param name="pguid">The GUID of the default search object.</param>
        /// <returns>Returns S_OK if successful, or a COM error value otherwise.</returns>
        [PreserveSig]
        Int32 GetDefaultSearchGUID(out Guid pguid);

        /// <summary>
        /// Requests a pointer to an interface that allows a client to enumerate the available search objects.
        /// </summary>
        /// <param name="ppenum">The address of a pointer to an enumerator object's IEnumExtraSearch interface.</param>
        /// <returns>Returns S_OK if successful, or a COM error value otherwise.</returns>
        [PreserveSig]
        Int32 EnumSearches(out IEnumExtraSearch ppenum);

        /// <summary>
        /// Gets the default sorting and display columns.
        /// </summary>
        /// <param name="dwRes">Reserved. Set to zero.</param>
        /// <param name="pSort">A pointer to a value that receives the index of the default sorted column.</param>
        /// <param name="pDisplay">A pointer to a value that receives the index of the default display column.</param>
        /// <returns>Returns S_OK if successful, or a COM error value otherwise.</returns>
        [PreserveSig]
        Int32 GetDefaultColumn(UInt32 dwRes, out UInt64 pSort, out UInt64 pDisplay);

        /// <summary>
        /// Gets the default state for a specified column.
        /// </summary>
        /// <param name="iColumn">An integer that specifies the column number.</param>
        /// <param name="pcsFlags">A pointer to a value that contains flags that indicate the default column state. This parameter can include a combination of the following flags.</param>
        /// <returns>Returns S_OK if successful, or a COM error value otherwise.</returns>
        [PreserveSig]
        Int32 GetDefaultColumnState(UInt32 iColumn, out SHCOLSTATEF pcsFlags);

        /// <summary>
        /// Gets detailed information, identified by a property set identifier (FMTID) and a property identifier (PID), on an item in a Shell folder.
        /// </summary>
        /// <param name="pidl">A PIDL of the item, relative to the parent folder. This method accepts only single-level PIDLs. The structure must contain exactly one SHITEMID structure followed by a terminating zero. This value cannot be NULL.</param>
        /// <param name="pscid">A pointer to an SHCOLUMNID structure that identifies the column.</param>
        /// <param name="pv">A pointer to a VARIANT with the requested information. The value is fully typed. The value returned for properties from the property system must conform to the type specified in that property definition's typeInfo as the legacyType attribute.</param>
        /// <returns>Returns S_OK if successful, or a COM error value otherwise.</returns>
        [PreserveSig]
        Int32 GetDetailsEx(IntPtr pidl, PROPERTYKEY pscid, out object pv);

        /// <summary>
        /// Gets detailed information, identified by a column index, on an item in a Shell folder.
        /// </summary>
        /// <param name="pidl">PIDL of the item for which you are requesting information. This method accepts only single-level PIDLs. The structure must contain exactly one SHITEMID structure followed by a terminating zero. If this parameter is set to NULL, the title of the information field specified by iColumn is returned.</param>
        /// <param name="iColumn">The zero-based index of the desired information field. It is identical to the column number of the information as it is displayed in a Windows Explorer Details view.</param>
        /// <param name="psd">The zero-based index of the desired information field. It is identical to the column number of the information as it is displayed in a Windows Explorer Details view.</param>
        /// <returns>Returns S_OK if successful, or a COM error value otherwise.</returns>
        [PreserveSig]
        HRESULT GetDetailsOf(IntPtr pidl, UInt32 iColumn, out SHELLDETAILS psd);

        /// <summary>
        /// Converts a column to the appropriate property set ID (FMTID) and property ID (PID).
        /// </summary>
        /// <param name="iColumn">The column ID.</param>
        /// <param name="pscid">A pointer to an SHCOLUMNID structure containing the FMTID and PID.</param>
        /// <returns>Returns S_OK if successful, or a COM error value otherwise.</returns>
        [PreserveSig]
        Int32 MapColumnToSCID(UInt32 iColumn, out PROPERTYKEY pscid);
    };
}