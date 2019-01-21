namespace WSF.Shell.Interop.Interfaces.ShellItems
{
    using WSF.Shell.Enums;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Exposes enumeration of IShellItem interfaces.
    /// This interface is typically obtained by calling the IEnumShellItems method.
    /// 
    /// <see href="https://docs.microsoft.com/en-us/windows/desktop/api/shobjidl_core/nn-shobjidl_core-ienumshellitems"/>
    /// </summary>
    [ComImport]
    [Guid("70629033-e363-4a28-a567-0db78006e6d7")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumShellItems
    {
        /// <summary>
        /// Gets an array of one or more IShellItem interfaces from the enumeration.
        /// </summary>
        /// <param name="celt"></param>
        /// <param name="rgelt"></param>
        /// <param name="pceltFetched"></param>
        /// <returns></returns>
        HRESULT Next(uint celt, out IShellItem rgelt, out uint pceltFetched);

        /// <summary>
        /// Skips a given number of IShellItem interfaces in the enumeration.
        /// Used when retrieving interfaces.
        /// </summary>
        /// <param name="celt"></param>
        /// <returns></returns>
        HRESULT Skip(uint celt);

        /// <summary>
        /// Resets the internal count of retrieved IShellItem interfaces in the enumeration.
        /// </summary>
        /// <returns></returns>
        HRESULT Reset();

        /// <summary>
        /// Gets a copy of the current enumeration.
        /// </summary>
        /// <param name="ppenum"></param>
        /// <returns></returns>
        HRESULT Clone(out IEnumShellItems ppenum);
    }
}
