namespace WSF.Shell.Interop.Interfaces.ShellFolders
{
    using WSF.IDs;
    using WSF.Shell.Enums;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Exposed by all Shell namespace folder objects, its methods are used to manage folders.
    /// 
    /// https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nn-shobjidl_core-ishellfolder
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid(ShellIIDGuids.IShellFolder)]
    internal interface IShellFolder
    {
        /// <summary>
        /// Translates the display name of a file object or a folder into an item identifier list.
        /// </summary>
        /// <param name="hwnd">A window handle. The client should provide a window handle if it displays a dialog or message box. Otherwise set hwnd to NULL.</param>
        /// <param name="pbc">Optional. A pointer to a bind context used to pass parameters as inputs and outputs to the parsing function.</param>
        /// <param name="pszDisplayName">A null-terminated Unicode string with the display name.</param>
        /// <param name="pchEaten">A pointer to a ULONG value that receives the number of characters of the display name that was parsed. If your application does not need this information, set pchEaten to NULL, and no value will be returned.</param>
        /// <param name="ppidl">When this method returns, contains a pointer to the PIDL for the object.</param>
        /// <param name="pdwAttributes">The value used to query for file attributes. If not used, it should be set to NULL.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT ParseDisplayName(IntPtr hwnd,
                                 IntPtr pbc,
                                 [MarshalAs(UnmanagedType.LPWStr)]
                                 string pszDisplayName,
                                 ref uint pchEaten,
                                 out IntPtr ppidl,
                                 ref SFGAO pdwAttributes);

        /// <summary>
        /// Allows a client to determine the contents of a folder by creating an item identifier
        /// enumeration object and returning its <see cref="IEnumIDList"/> interface. 
        /// Return value: error code, if any
        /// </summary>
        /// <param name="hwnd">If user input is required to perform the enumeration,
        /// this window handle should be used by the enumeration object as the parent window to take user input.</param>
        /// <param name="grfFlags">Flags indicating which items to include in the  enumeration.
        /// For a list of possible values, see the <see cref="WSF.Shell.Enums.SHCONTF"/> enum.</param>
        /// <param name="ppenumIDList">Address that receives a pointer to the IEnumIDList interface of the enumeration object
        /// created by this method. </param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT EnumObjects(IntPtr hwnd, SHCONTF grfFlags, out IntPtr ppenumIDList);

        /// <summary>
        /// Retrieves an IShellFolder object for a subfolder.
        /// Return value: error code, if any
        /// </summary>
        /// <param name="pidl">Address of an ITEMIDLIST structure (PIDL) that identifies the subfolder.</param>
        /// <param name="pbc">Optional address of an IBindCtx interface on a bind context object to be used during this operation.</param>
        /// <param name="riid">Identifier of the interface to return. </param>
        /// <param name="ppv">Address that receives the interface pointer.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        Int32 BindToObject(IntPtr pidl, IntPtr pbc, [In] ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// Requests a pointer to an object's storage interface. 
        /// Return value: error code, if any
        /// </summary>
        /// <param name="pidl">Address of an ITEMIDLIST structure that identifies the subfolder relative to its parent folder. </param>
        /// <param name="pbc">Optional address of an IBindCtx interface on a bind context object to be  used during this operation.</param>
        /// <param name="riid">Interface identifier (IID) of the requested storage interface.</param>
        /// <param name="ppv"> Address that receives the interface pointer specified by riid.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        Int32 BindToStorage(IntPtr pidl, IntPtr pbc, [In] ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// Determines the relative order of two file objects or folders, given 
        /// their item identifier lists. Return value: If this method is 
        /// successful, the CODE field of the HRESULT contains one of the 
        /// following values (the code can be retrived using the helper function
        /// GetHResultCode): Negative A negative return value indicates that the first item should precede the second (pidl1 &lt; pidl2). 
        ///Positive A positive return value indicates that the first item should
        ///follow the second (pidl1 > pidl2).  Zero A return value of zero
        ///indicates that the two items are the same (pidl1 = pidl2). 
        /// </summary>
        /// <param name="lParam">Value that specifies how the comparison  should be performed. The lower Sixteen bits of lParam define the sorting  rule. 
        ///  The upper sixteen bits of lParam are used for flags that modify the sorting rule. values can be from  the SHCIDS enum
        /// </param>
        /// <param name="pidl1">Pointer to the first item's ITEMIDLIST structure.</param>
        /// <param name="pidl2"> Pointer to the second item's ITEMIDLIST structure.</param>
        /// <returns></returns>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        Int32 CompareIDs(IntPtr lParam, IntPtr pidl1, IntPtr pidl2);

        /// <summary>
        /// Requests an object that can be used to obtain information from or interact
        /// with a folder object.
        /// Return value: error code, if any
        /// </summary>
        /// <param name="hwndOwner">Handle to the owner window.</param>
        /// <param name="riid">Identifier of the requested interface.</param>
        /// <param name="ppv">Address of a pointer to the requested interface. </param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        Int32 CreateViewObject(IntPtr hwndOwner, [In] ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// Gets the attributes of one or more file or folder objects contained in the object represented by <see cref="IShellFolder"/>.
        /// </summary>
        /// <param name="cidl">The number of items from which to retrieve attributes.</param>
        /// <param name="apidl">Type: PCUITEMID_CHILD_ARRAY*
        /// 
        /// The address of an array of pointers to ITEMIDLIST structures, each of which uniquely identifies an
        /// item relative to the parent folder.Each ITEMIDLIST structure must contain exactly one SHITEMID structure
        /// followed by a terminating zero.</param>
        /// <param name="rgfInOut">Type: SFGAOF*
        /// 
        /// Pointer to a single ULONG value that, on entry, contains the bitwise SFGAO attributes that the calling application
        /// is requesting.On exit, this value contains the requested attributes that are common to all of the specified items.</param>
        /// <returns>Type: HRESULT
        /// 
        /// If this method succeeds, it returns S_OK.Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        int GetAttributesOf(UInt32 cidl,
                            [MarshalAs(UnmanagedType.LPArray)]
                            IntPtr[] apidl,
                            ref SFGAO rgfInOut);

        /// <summary>
        /// Retrieves an OLE interface that can be used to carry out actions on the 
        /// specified file objects or folders. Return value: error code, if any
        /// </summary>
        /// <param name="hwndOwner">Handle to the owner window that the client should specify if it displays a dialog box or message box.</param>
        /// <param name="cidl">Number of file objects or subfolders specified in the apidl parameter. </param>
        /// <param name="apidl">Address of an array of pointers to ITEMIDLIST  structures, each of which  uniquely identifies a file object or subfolder relative to the parent folder.</param>
        /// <param name="riid">Identifier of the COM interface object to return.</param>
        /// <param name="rgfReserved"> Reserved. </param>
        /// <param name="ppv">Pointer to the requested interface.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT GetUIObjectOf([In] IntPtr hwndOwner,
                              [In] UInt32 cidl,
                              [MarshalAs(UnmanagedType.LPArray)]
                              [In] IntPtr[] apidl,
                              [In] ref Guid riid,
                              [In, Out] IntPtr rgfReserved,
                              [Out] out IntPtr ppv);

        /// <summary>
        /// Retrieves the display name for the specified file object or subfolder. 
        /// Return value: error code, if any
        /// </summary>
        /// <param name="pidl">Address of an ITEMIDLIST structure (PIDL)  that uniquely identifies the file  object or subfolder relative to the parent  folder. </param>
        /// <param name="uFlags">Flags used to request the type of display name to return. For a list of possible values. </param>
        /// <param name="lpName"> Address of a STRRET structure in which to return the display name.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT GetDisplayNameOf(IntPtr pidl, SHGDNF uFlags, IntPtr lpName);

        /// <summary>
        /// Sets the display name of a file object or subfolder, changing the item
        /// identifier in the process.
        /// Return value: error code, if any
        /// </summary>
        /// <param name="hwnd"> Handle to the owner window of any dialog or message boxes that the client displays.</param>
        /// <param name="pidl"> Pointer to an ITEMIDLIST structure that uniquely identifies the file object or subfolder relative to the parent folder. </param>
        /// <param name="pszName"> Pointer to a null-terminated string that specifies the new display name.</param>
        /// <param name="uFlags">Flags indicating the type of name specified by  the lpszName parameter. For a list of possible values, see the description of the SHGNO enum.</param>
        /// <param name="ppidlOut"></param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        Int32 SetNameOf(IntPtr hwnd, IntPtr pidl,
                        [MarshalAs(UnmanagedType.LPWStr)]
                        String pszName, SHGDNF uFlags, out IntPtr ppidlOut);
    }
}