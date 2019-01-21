namespace WSF.Shell.Interop.Interfaces.KnownFolders
{
    using WSF.Shell.Enums;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport,
    Guid(KnownFoldersIIDGuid.IKnownFolderManager),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IKnownFolderManager
    {
        [MethodImpl(MethodImplOptions.InternalCall,
            MethodCodeType = MethodCodeType.Runtime)]
        void FolderIdFromCsidl(int csidl,
           [Out] out Guid knownFolderID);

        [MethodImpl(MethodImplOptions.InternalCall,
            MethodCodeType = MethodCodeType.Runtime)]
        void FolderIdToCsidl([In, MarshalAs(UnmanagedType.LPStruct)] Guid id,
          [Out] out int csidl);

        [MethodImpl(MethodImplOptions.InternalCall,
            MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetFolderIds([Out] out IntPtr folders,
          [Out] out UInt32 count);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall,
            MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetFolder([In, MarshalAs(UnmanagedType.LPStruct)] Guid id,
          [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolderNative knownFolder);

        [MethodImpl(MethodImplOptions.InternalCall,
            MethodCodeType = MethodCodeType.Runtime)]
        void GetFolderByName(string canonicalName,
          [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolderNative knownFolder);

        [MethodImpl(MethodImplOptions.InternalCall,
            MethodCodeType = MethodCodeType.Runtime)]
        void RegisterFolder(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid knownFolderGuid,
            [In] ref NativeFolderDefinition knownFolderDefinition);

        [MethodImpl(MethodImplOptions.InternalCall,
            MethodCodeType = MethodCodeType.Runtime)]
        void UnregisterFolder(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid knownFolderGuid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FindFolderFromPath(
            [In, MarshalAs(UnmanagedType.LPWStr)] string path,
            [In] int mode,
            [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolderNative knownFolder);

        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall,
            MethodCodeType = MethodCodeType.Runtime)]
        HRESULT FindFolderFromIDList(IntPtr pidl, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolderNative knownFolder);

        [MethodImpl(MethodImplOptions.InternalCall,
            MethodCodeType = MethodCodeType.Runtime)]
        void Redirect();
    }
}
