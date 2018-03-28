///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ShellDll;

namespace ShellDll
{
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

    [StructLayout ( LayoutKind.Sequential, Pack = 4 )]
    public struct PropertyKey
    {
        public Guid fmtid;
        public uint pid;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ShellDetails
    {
        //http://msdn.microsoft.com/en-us/library/bb759820(VS.85).aspx
        public Int32 fmt;
        public Int32 cxChar;
        public IntPtr str; //ShellAPI.StrRetToBuf(str, relPidl.Ptr, buf, ShellAPI.MAX_PATH);
    }

    [ComImport]
    [Guid("0E700BE1-9DB6-11D1-A1CE-00C04FD75D13")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumExtraSearch
    {
        //http://msdn.microsoft.com/en-us/library/bb761992(VS.85).aspx

        //HRESULT Clone(IEnumExtraSearch **ppEnum);
        //HRESULT Next(ULONG celt, EXTRASEARCH* rgelt, ULONG* pceltFetched);
        //HRESULT Reset(VOID);
        //HRESULT Skip(ULONG celt);
        Int32 Clone(out IEnumExtraSearch ppEnum);        
        Int32 Next(UInt64 celt, out ExtraSearch regelt, out Int64 pceltFetched);
        Int32 Reset();
        Int32 Skip(UInt64 celt);
    }

    [Flags]
    public enum SHCOLSTATEF : uint 
    {
        //http://msdn.microsoft.com/en-us/library/bb762538(VS.85).aspx
        SHCOLSTATE_DEFAULT = 0x00000000, //The value is displayed according to default settings for the column.
        SHCOLSTATE_TYPE_STR = 0x00000001, //The value is displayed as a string.
        SHCOLSTATE_TYPE_INT = 0x00000002, //The value is displayed as an integer.
        SHCOLSTATE_TYPE_DATE = 0x00000003, //The value is displayed as a date/time.
        SHCOLSTATE_TYPEMASK = 0x0000000f, //A mask for display type values SHCOLSTATE_TYPE_STR, SHCOLSTATE_TYPE_STR, and SHCOLSTATE_TYPE_DATE.
        SHCOLSTATE_ONBYDEFAULT = 0x00000010, //The column should be on by default in Details view.
        SHCOLSTATE_SLOW = 0x00000020, //Will be slow to compute. Perform on a background thread.
        SHCOLSTATE_EXTENDED = 0x00000040, //Provided by a handler, not the folder.
        SHCOLSTATE_SECONDARYUI = 0x00000080, //Not displayed in the context menu, but is listed in the More... dialog.
        SHCOLSTATE_HIDDEN = 0x00000100, //Not displayed in the UI.
        SHCOLSTATE_PREFER_VARCMP = 0x00000200, //VarCmp produces same result as IShellFolder::CompareIDs.
        SHCOLSTATE_PREFER_FMTCMP = 0x00000400, //PSFormatForDisplay produces same result as IShellFolder::CompareIDs.
        SHCOLSTATE_NOSORTBYFOLDERNESS = 0x00000800, //Do not sort folders separately.
        SHCOLSTATE_VIEWONLY = 0x00010000, //Only displayed in the UI.
        SHCOLSTATE_BATCHREAD = 0x00020000, //Marks columns with values that should be read in a batch.
        PSHCOLSTATE_NO_GROUPBY = 0x00040000, //Grouping is disabled for this column.
        SHCOLSTATE_FIXED_WIDTH = 0x00001000, //Can't resize the column.
        SHCOLSTATE_NODPISCALE = 0x00002000, //The width is the same in all dots per inch (dpi)s.
        SHCOLSTATE_FIXED_RATIO = 0x00004000, //Fixed width and height ratio.
        SHCOLSTATE_DISPLAYMASK = 0x0000F000 //Filters out new display flags.
    }


    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("93F2F68C-1D1B-11D3-A30E-00C04F79ABD1")]
    public interface IShellFolder2
    {
        #region IShellFolder
        // Translates a file object's or folder's display name into an item identifier list.
        // Return value: error code, if any
        [PreserveSig]
        Int32 ParseDisplayName(
            IntPtr hwnd,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPWStr)] 
            string pszDisplayName,
            ref uint pchEaten,
            out IntPtr ppidl,
            ref ShellAPI.SFGAO pdwAttributes);

        // Allows a client to determine the contents of a folder by creating an item
        // identifier enumeration object and returning its IEnumIDList interface.
        // Return value: error code, if any
        [PreserveSig]
        Int32 EnumObjects(
            IntPtr hwnd,
            ShellAPI.SHCONTF grfFlags,
            out IntPtr enumIDList);

        // Retrieves an IShellFolder object for a subfolder.
        // Return value: error code, if any
        [PreserveSig]
        Int32 BindToObject(
            IntPtr pidl,
            IntPtr pbc,
            ref Guid riid,
            out IntPtr ppv);

        // Requests a pointer to an object's storage interface. 
        // Return value: error code, if any
        [PreserveSig]
        Int32 BindToStorage(
            IntPtr pidl,
            IntPtr pbc,
            ref Guid riid,
            out IntPtr ppv);

        // Determines the relative order of two file objects or folders, given their
        // item identifier lists. Return value: If this method is successful, the
        // CODE field of the HRESULT contains one of the following values (the code
        // can be retrived using the helper function GetHResultCode): Negative A
        // negative return value indicates that the first item should precede
        // the second (pidl1 < pidl2). 

        // Positive A positive return value indicates that the first item should
        // follow the second (pidl1 > pidl2).  Zero A return value of zero
        // indicates that the two items are the same (pidl1 = pidl2). 
        [PreserveSig]
        Int32 CompareIDs(
            IntPtr lParam,
            IntPtr pidl1,
            IntPtr pidl2);

        // Requests an object that can be used to obtain information from or interact
        // with a folder object.
        // Return value: error code, if any
        [PreserveSig]
        Int32 CreateViewObject(
            IntPtr hwndOwner,
            Guid riid,
            out IntPtr ppv);

        // Retrieves the attributes of one or more file objects or subfolders. 
        // Return value: error code, if any
        [PreserveSig]
        Int32 GetAttributesOf(
            uint cidl,
            [MarshalAs(UnmanagedType.LPArray)]
            IntPtr[] apidl,
            ref ShellAPI.SFGAO rgfInOut);

        // Retrieves an OLE interface that can be used to carry out actions on the
        // specified file objects or folders.
        // Return value: error code, if any
        [PreserveSig]
        Int32 GetUIObjectOf(
            IntPtr hwndOwner,
            uint cidl,
            [MarshalAs(UnmanagedType.LPArray)]
            IntPtr[] apidl,
            ref Guid riid,
            IntPtr rgfReserved,
            out IntPtr ppv);

        // Retrieves the display name for the specified file object or subfolder. 
        // Return value: error code, if any
        [PreserveSig()]
        Int32 GetDisplayNameOf(
            IntPtr pidl,
            ShellAPI.SHGNO uFlags,
            IntPtr lpName);

        // Sets the display name of a file object or subfolder, changing the item
        // identifier in the process.
        // Return value: error code, if any
        [PreserveSig]
        Int32 SetNameOf(
            IntPtr hwnd,
            IntPtr pidl,
            [MarshalAs(UnmanagedType.LPWStr)] 
            string pszName,
            ShellAPI.SHGNO uFlags,
            out IntPtr ppidlOut);
        #endregion

        #region IShellFolder2
        //http://msdn.microsoft.com/en-us/library/bb775055(VS.85).aspx

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
