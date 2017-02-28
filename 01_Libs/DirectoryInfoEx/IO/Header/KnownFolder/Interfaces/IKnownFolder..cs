using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShellDll
{
    [ComImport, Guid("3AA7AF7E-9B36-420c-A8E3-F77D4674A488"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IKnownFolder
    {
        void GetId([Out] out Guid Id);
        void GetCategory([Out] out KnownFolderCategory category);
        void GetShellItem(KnownFolderRetrievalOptions retrievalOptions,
                          [MarshalAs(UnmanagedType.LPStruct)] Guid interfaceGuid,
                          [Out, MarshalAs(UnmanagedType.IUnknown)] out object shellItem);
        void GetPath(KnownFolderRetrievalOptions retrievalOptions, [Out] out IntPtr path);
        void SetPath(KnownFolderRetrievalOptions retrievalOptions, string path);
        int GetIDList(KnownFolderRetrievalOptions retrievalOptions, [Out] out IntPtr itemIdentifierListPointer);
        void GetFolderType([Out, MarshalAs(UnmanagedType.LPStruct)] out Guid folderTypeID);
        void GetRedirectionCapabilities([Out] out KnownFolderRedirectionCapabilities redirectionCapabilities);
        void GetFolderDefinition([Out, MarshalAs(UnmanagedType.Struct)] out InternalKnownFolderDefinition definition);
    }
}
