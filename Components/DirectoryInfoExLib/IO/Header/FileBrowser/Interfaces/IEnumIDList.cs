namespace DirectoryInfoExLib.IO.Header.ShellDll.Interfaces
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Exposes a standard set of methods used to enumerate the pointers to item
    /// identifier lists (<see cref="PIDL"/>s) of the items in a Shell folder. When a folder's
    /// IShellFolder::EnumObjects method is called, it creates an enumeration object
    /// and passes a pointer to the object's IEnumIDList interface back to the
    /// calling application.
    /// </summary>
    [ComImportAttribute()]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F2-0000-0000-C000-000000000046")]
    public interface IEnumIDList
    {
        /// <summary>
        /// Retrieves the specified number of item identifiers in the enumeration 
        /// sequence and advances the current position by the number of items retrieved
        /// </summary>
        /// <param name="celt"></param>
        /// <param name="rgelt"></param>
        /// <param name="pceltFetched"></param>
        /// <returns></returns>
        [PreserveSig()]
        Int32 Next(
            int celt,
            out IntPtr rgelt,
            out int pceltFetched);

        /// <summary>
        /// Skips over the specified number of elements in the enumeration sequence
        /// </summary>
        /// <param name="celt"></param>
        /// <returns></returns>
        [PreserveSig()]
        Int32 Skip(
            int celt);

        /// <summary>
        /// Returns to the beginning of the enumeration sequence
        /// </summary>
        /// <returns></returns>
        [PreserveSig()]
        Int32 Reset();

        /// <summary>
        /// Creates a new item enumeration object with the same contents and state as the current one
        /// </summary>
        /// <param name="ppenum"></param>
        /// <returns></returns>
        [PreserveSig()]
        Int32 Clone(
            out IEnumIDList ppenum);
    }
}
