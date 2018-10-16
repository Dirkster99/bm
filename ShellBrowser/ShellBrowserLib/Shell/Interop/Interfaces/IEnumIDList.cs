﻿namespace ShellBrowserLib.SharpShell.Interop
{
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.Shell.Enums;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Exposes a standard set of methods used to enumerate the pointers to item identifier lists (PIDLs) of the items in a Shell folder.
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid(ShellIIDGuids.IEnumIDList)]
    public interface IEnumIDList
    {
        /// <summary>
        /// Retrieves the specified number of item identifiers in the 
        /// enumeration sequence and advances the current position by 
        /// the number of items retrieved.
        /// </summary>
        /// <param name="celt">Number of elements in the array pointed to by the rgelt parameter.</param>
        /// <param name="rgelt">
        /// Address of an array of ITEMIDLIST pointers that receives the item identifiers. The implementation must allocate these item identifiers
        /// using the Shell's allocator (retrieved by the SHGetMalloc function). The calling application is responsible for freeing the item 
        /// identifiers using the Shell's allocator.
        /// </param>
        /// <param name="pceltFetched">
        /// Address of a value that receives a count of the item identifiers actually returned in rgelt. The count can be smaller than the value
        /// specified in the celt parameter. This parameter can be NULL only if celt is one.
        /// </param>
        [PreserveSig]
        HRESULT Next(uint celt, out IntPtr rgelt, out uint pceltFetched);

        /// <summary>
        /// Skips over the specified number of elements in the enumeration sequence. 
        /// </summary>
        /// <param name="celt">Number of item identifiers to skip.</param>
        [PreserveSig]
        int Skip(uint celt);

        /// <summary>
        /// Returns to the beginning of the enumeration sequence. 
        /// </summary>
        [PreserveSig]
        int Reset();

        /// <summary>
        /// Creates a new item enumeration object with the same contents and state as the current one. 
        /// </summary>
        /// <param name="ppenum">
        /// Address of a pointer to the new enumeration object. The calling application must
        /// eventually free the new object by calling its Release member function. 
        /// </param>
        [PreserveSig]
        int Clone(out IEnumIDList ppenum);
    }
}