///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace DirectoryInfoExLib.IO.Header
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Used by an IEnumExtraSearch enumerator object to return information
    /// on the search objects supported by a Shell Folder object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ExtraSearch
    {
        //http://msdn.microsoft.com/en-us/library/bb773283(VS.85).aspx
        // typedef struct EXTRASEARCH {
        //    GUID guidSearch;
        //    WCHAR wszFriendlyName[80];
        //    WCHAR wszUrl[2084];
        //} EXTRASEARCH, *LPEXTRASEARCH;

        Guid guidSearch;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        string wszFriendlyName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2084)]
        string wszUrl;
    }

    /// <summary>
    /// Specifies the FMTID/PID identifier that programmatically identifies a property.
    /// 
    /// Defines a property key which can be used to retrieve custom
    /// file properties from the file system.
    /// </summary>
    [StructLayout ( LayoutKind.Sequential, Pack = 4 )]
    public struct PropertyKey
    {
        /// <summary>
        /// A unique GUID for the property.
        /// </summary>
        public Guid fmtid;

        /// <summary>
        /// A property identifier (PID).
        /// This parameter is not used as in SHCOLUMNID.
        /// It is recommended that you set this value to PID_FIRST_USABLE.
        /// Any value greater than or equal to 2 is acceptable.
        /// </summary>
        public uint pid;
    }

    /// <summary>
    /// Reports detailed information on an item in a Shell folder.
    /// 
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb759781(v=vs.85).aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ShellDetails
    {
        /// <summary>
        /// The alignment of the column heading and the subitem text in the column.
        /// This member can be one of the following values.
        /// </summary>
        public Int32 fmt;

        /// <summary>
        /// The number of average-sized characters in the header.
        /// </summary>
        public Int32 cxChar;

        /// <summary>
        /// An STRRET structure
        /// http://msdn.microsoft.com/en-us/library/bb759820(VS.85).aspx
        /// 
        /// that includes a string with the requested information.
        /// To convert this structure to a string, use StrRetToBuf or StrRetToStr.
        /// </summary>
        public IntPtr str; //ShellAPI.StrRetToBuf(str, relPidl.Ptr, buf, ShellAPI.MAX_PATH);
    }

    /// <summary>
    /// A standard OLE enumerator used by a client to determine the available
    /// search objects for a folder.
    /// 
    /// http://msdn.microsoft.com/en-us/library/bb761992(VS.85).aspx
    /// </summary>
    [ComImport]
    [Guid("0E700BE1-9DB6-11D1-A1CE-00C04FD75D13")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumExtraSearch
    {
        /// <summary>
        /// Used to request a duplicate of the enumerator object to preserve its current state.
        /// </summary>
        /// <param name="ppEnum"></param>
        /// <returns></returns>
        Int32 Clone(out IEnumExtraSearch ppEnum);

        /// <summary>
        /// Used to request information on one or more search objects.
        /// </summary>
        /// <param name="celt"></param>
        /// <param name="regelt"></param>
        /// <param name="pceltFetched"></param>
        /// <returns></returns>
        Int32 Next(UInt64 celt, out ExtraSearch regelt, out Int64 pceltFetched);

        /// <summary>
        /// Used to reset the enumeration index to zero.
        /// </summary>
        /// <returns></returns>
        Int32 Reset();

        /// <summary>
        /// Skip a specified number of objects.
        /// </summary>
        /// <param name="celt"></param>
        /// <returns></returns>
        Int32 Skip(UInt64 celt);
    }

    /// <summary>
    /// Describes how a property should be treated.
    /// These values are defined in Shtypes.h.
    /// 
    /// http://msdn.microsoft.com/en-us/library/bb762538(VS.85).aspx
    /// </summary>
    [Flags]
    public enum SHCOLSTATEF : uint 
    {
        /// <summary>
        /// The value is displayed according to default settings for the column.
        /// </summary>
        SHCOLSTATE_DEFAULT = 0x00000000,

        /// <summary>
        /// The value is displayed as a string.
        /// </summary>
        SHCOLSTATE_TYPE_STR = 0x00000001,

        /// <summary>
        /// The value is displayed as an integer.
        /// </summary>
        SHCOLSTATE_TYPE_INT = 0x00000002,

        /// <summary>
        /// The value is displayed as a date/time.
        /// </summary>
        SHCOLSTATE_TYPE_DATE = 0x00000003,

        /// <summary>
        /// A mask for display type values SHCOLSTATE_TYPE_STR, SHCOLSTATE_TYPE_STR, and SHCOLSTATE_TYPE_DATE.
        /// </summary>
        SHCOLSTATE_TYPEMASK = 0x0000000f,

        /// <summary>
        /// The column should be on by default in Details view.
        /// </summary>
        SHCOLSTATE_ONBYDEFAULT = 0x00000010,

        /// <summary>
        /// Will be slow to compute. Perform on a background thread.
        /// </summary>
        SHCOLSTATE_SLOW = 0x00000020,

        /// <summary>
        /// Provided by a handler, not the folder.
        /// </summary>
        SHCOLSTATE_EXTENDED = 0x00000040,

        /// <summary>
        /// Not displayed in the context menu, but is listed in the More... dialog.
        /// </summary>
        SHCOLSTATE_SECONDARYUI = 0x00000080,

        /// <summary>
        /// Not displayed in the UI.
        /// </summary>
        SHCOLSTATE_HIDDEN = 0x00000100,

        /// <summary>
        /// VarCmp produces same result as IShellFolder::CompareIDs.
        /// </summary>
        SHCOLSTATE_PREFER_VARCMP = 0x00000200,

        /// <summary>
        /// PSFormatForDisplay produces same result as IShellFolder::CompareIDs.
        /// </summary>
        SHCOLSTATE_PREFER_FMTCMP = 0x00000400,

        /// <summary>
        /// Do not sort folders separately.
        /// </summary>
        SHCOLSTATE_NOSORTBYFOLDERNESS = 0x00000800,

        /// <summary>
        /// Only displayed in the UI.
        /// </summary>
        SHCOLSTATE_VIEWONLY = 0x00010000,

        /// <summary>
        /// Marks columns with values that should be read in a batch.
        /// </summary>
        SHCOLSTATE_BATCHREAD = 0x00020000,

        /// <summary>
        /// Grouping is disabled for this column.
        /// </summary>
        PSHCOLSTATE_NO_GROUPBY = 0x00040000,

        /// <summary>
        /// Can't resize the column.
        /// </summary>
        SHCOLSTATE_FIXED_WIDTH = 0x00001000,

        /// <summary>
        /// //The width is the same in all dots per inch (dpi)s.
        /// </summary>
        SHCOLSTATE_NODPISCALE = 0x00002000,

        /// <summary>
        /// Fixed width and height ratio.
        /// </summary>
        SHCOLSTATE_FIXED_RATIO = 0x00004000,

        /// <summary>
        /// Filters out new display flags.
        /// </summary>
        SHCOLSTATE_DISPLAYMASK = 0x0000F000
    }

    /// <summary>
    /// Extends the capabilities of IShellFolder.
    /// Its methods provide a variety of information about the contents of a Shell folder.
    /// 
    /// http://msdn.microsoft.com/en-us/library/bb775055(VS.85).aspx
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("93F2F68C-1D1B-11D3-A30E-00C04F79ABD1")]
    internal interface IShellFolder2
    {
        #region IShellFolder
        /// <summary>
        /// Translates a file object's or folder's display name into an item identifier list.
        /// Return value: error code, if any
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="pbc"></param>
        /// <param name="pszDisplayName"></param>
        /// <param name="pchEaten"></param>
        /// <param name="ppidl"></param>
        /// <param name="pdwAttributes"></param>
        /// <returns></returns>
        [PreserveSig]
        Int32 ParseDisplayName(
            IntPtr hwnd,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPWStr)] 
            string pszDisplayName,
            ref uint pchEaten,
            out IntPtr ppidl,
            ref DirectoryInfoExLib.IO.Header.ShellDll.ShellAPI.SFGAO pdwAttributes);

        /// <summary>
        /// Allows a client to determine the contents of a folder by creating an item
        /// identifier enumeration object and returning its IEnumIDList interface.
        /// Return value: error code, if any
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="grfFlags"></param>
        /// <param name="enumIDList"></param>
        /// <returns></returns>
        [PreserveSig]
        Int32 EnumObjects(
            IntPtr hwnd,
            DirectoryInfoExLib.IO.Header.ShellDll.ShellAPI.SHCONTF grfFlags,
            out IntPtr enumIDList);

        /// <summary>
        /// Retrieves an IShellFolder object for a subfolder.
        /// Return value: error code, if any
        /// </summary>
        /// <param name="pidl"></param>
        /// <param name="pbc"></param>
        /// <param name="riid"></param>
        /// <param name="ppv"></param>
        /// <returns></returns>
        [PreserveSig]
        Int32 BindToObject(
            IntPtr pidl,
            IntPtr pbc,
            ref Guid riid,
            out IntPtr ppv);

        /// <summary>
        /// Requests a pointer to an object's storage interface. 
        /// Return value: error code, if any
        /// </summary>
        /// <param name="pidl"></param>
        /// <param name="pbc"></param>
        /// <param name="riid"></param>
        /// <param name="ppv"></param>
        /// <returns></returns>
        [PreserveSig]
        Int32 BindToStorage(
            IntPtr pidl,
            IntPtr pbc,
            ref Guid riid,
            out IntPtr ppv);

        /// <summary>
        /// Determines the relative order of two file objects or folders, given their
        /// item identifier lists. Return value: If this method is successful, the
        /// CODE field of the HRESULT contains one of the following values (the code
        /// can be retrived using the helper function GetHResultCode): Negative A
        /// negative return value indicates that the first item should precede
        /// the second (pidl1 &lt; pidl2). 
        ///
        /// Positive A positive return value indicates that the first item should
        /// follow the second (pidl1 > pidl2).  Zero A return value of zero
        /// indicates that the two items are the same (pidl1 = pidl2). 
        /// </summary>
        /// <param name="lParam"></param>
        /// <param name="pidl1"></param>
        /// <param name="pidl2"></param>
        /// <returns></returns>
        [PreserveSig]
        Int32 CompareIDs(
            IntPtr lParam,
            IntPtr pidl1,
            IntPtr pidl2);

        /// <summary>
        /// Requests an object that can be used to obtain information from or interact
        /// with a folder object.
        /// Return value: error code, if any
        /// </summary>
        /// <param name="hwndOwner"></param>
        /// <param name="riid"></param>
        /// <param name="ppv"></param>
        /// <returns></returns>
        [PreserveSig]
        Int32 CreateViewObject(
            IntPtr hwndOwner,
            Guid riid,
            out IntPtr ppv);

        /// <summary>
        /// Retrieves the attributes of one or more file objects or subfolders. 
        /// Return value: error code, if any
        /// </summary>
        /// <param name="cidl"></param>
        /// <param name="apidl"></param>
        /// <param name="rgfInOut"></param>
        /// <returns></returns>
        [PreserveSig]
        Int32 GetAttributesOf(
            uint cidl,
            [MarshalAs(UnmanagedType.LPArray)]
            IntPtr[] apidl,
            ref DirectoryInfoExLib.IO.Header.ShellDll.ShellAPI.SFGAO rgfInOut);

        /// <summary>
        /// Retrieves an OLE interface that can be used to carry out actions on the
        /// specified file objects or folders.
        /// Return value: error code, if any
        /// </summary>
        /// <param name="hwndOwner"></param>
        /// <param name="cidl"></param>
        /// <param name="apidl"></param>
        /// <param name="riid"></param>
        /// <param name="rgfReserved"></param>
        /// <param name="ppv"></param>
        /// <returns></returns>
        [PreserveSig]
        Int32 GetUIObjectOf(
            IntPtr hwndOwner,
            uint cidl,
            [MarshalAs(UnmanagedType.LPArray)]
            IntPtr[] apidl,
            ref Guid riid,
            IntPtr rgfReserved,
            out IntPtr ppv);

        /// <summary>
        /// Retrieves the display name for the specified file object or subfolder. 
        /// Return value: error code, if any
        /// </summary>
        /// <param name="pidl"></param>
        /// <param name="uFlags"></param>
        /// <param name="lpName"></param>
        /// <returns></returns>
        [PreserveSig()]
        Int32 GetDisplayNameOf(
            IntPtr pidl,
            DirectoryInfoExLib.IO.Header.ShellDll.ShellAPI.SHGNO uFlags,
            IntPtr lpName);

        /// <summary>
        /// Sets the display name of a file object or subfolder, changing the item
        /// identifier in the process.
        /// Return value: error code, if any
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="pidl"></param>
        /// <param name="pszName"></param>
        /// <param name="uFlags"></param>
        /// <param name="ppidlOut"></param>
        /// <returns></returns>
        [PreserveSig]
        Int32 SetNameOf(
            IntPtr hwnd,
            IntPtr pidl,
            [MarshalAs(UnmanagedType.LPWStr)] 
            string pszName,
            DirectoryInfoExLib.IO.Header.ShellDll.ShellAPI.SHGNO uFlags,
            out IntPtr ppidlOut);
        #endregion

        #region IShellFolder2
        //HRESULT EnumSearches(IEnumExtraSearch **ppEnum);
        //HRESULT GetDefaultColumn(DWORD dwReserved, ULONG* pSort, ULONG* pDisplay); 
        //HRESULT GetDefaultColumnState(UINT iColumn, SHCOLSTATEF *pcsFlags);
        //HRESULT GetDefaultSearchGUID(GUID *pguid);
        //HRESULT GetDetailsEx(PCUITEMID_CHILD pidl, const SHCOLUMNID *pscid, VARIANT *pv);
        //HRESULT GetDetailsOf(PCUITEMID_CHILD pidl, UINT iColumn, SHELLDETAILS *psd);
        //HRESULT MapColumnToSCID(UINT iColumn, SHCOLUMNID *pscid);

        [PreserveSig]
        Int32 GetDefaultSearchGUID(out Guid pguid);

        [PreserveSig]
        Int32 EnumSearches(ref IEnumExtraSearch ppEnum);
        
        [PreserveSig]
        Int32 GetDefaultColumn(UInt32 dwReserved, out UInt64 pSort, out UInt64 pDisplay);

        [PreserveSig]
        Int32 GetDefaultColumnState(UInt32 iColumn, out SHCOLSTATEF pcsFlags);
                       
        [PreserveSig]
        Int32 GetDetailsEx(IntPtr pidl, ref PropertyKey pscid, out object pv);
        
        [PreserveSig]
        Int32 GetDetailsOf(IntPtr pidl, UInt32 iColumn, out ShellDetails psd);
        
        [PreserveSig]
        Int32 MapColumnToSCID(UInt32 iColumn, out PropertyKey pscid);
        #endregion
    }
}
