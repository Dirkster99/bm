namespace WSF.Shell.Interop.Interfaces.KnownFolders
{
    using WSF.Shell.Enums;
    using WSF.Shell.Interop.Interfaces.ShellItems;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Exposes methods that allow an application to retrieve information about a known folder's
    /// category, type, GUID, pointer to an item identifier list (PIDL) value, redirection capabilities,
    /// and definition. It provides a method for the retrival of a known folder's IShellItem object.
    /// It also provides methods to get or set the path of the known folder.
    /// 
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb761768(v=vs.85).aspx
    /// </summary>
    [ComImport,
     Guid(KnownFoldersIIDGuid.IKnownFolder), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IKnownFolderNative
    {
        /// <summary>
        /// Gets the KnownFolderID (Guid) of the selected folder.
        /// When this method returns, returns the KNOWNFOLDERID value of the known folder.
        /// Note, KNOWNFOLDERID values are GUIDs:
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/dd378457(v=vs.85).aspx
        /// 
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb761758(v=vs.85).aspx
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Guid GetId();

        /// <summary>
        /// Retrieves the category—virtual, fixed, common, or per-user—of the selected folder.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        FolderCategory GetCategory();

        /// <summary>
        /// Retrieves the location of a known folder in the Shell namespace in the form
        /// of a Shell item (IShellItem or derived interface).
        /// </summary>
        /// <param name="i"></param>
        /// <param name="interfaceGuid"></param>
        /// <param name="shellItem"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        HRESULT GetShellItem([In] int i,
             ref Guid interfaceGuid,
             [Out, MarshalAs(UnmanagedType.Interface)] out IShellItem2 shellItem);

        /// <summary>
        /// Retrieves the path of a known folder as a string.
        /// 
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761762(v=vs.85).aspx"/>
        /// 
        /// </summary>
        /// <param name="option">Flags that specify special retrieval options. This value can be 0;
        /// 
        /// otherwise, one or more of the KNOWN_FOLDER_FLAG values.
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd378447(v=vs.85).aspx"/> 
        /// </param>
        /// <param name="path"></param>
        /// <returns>When this method returns, contains the address of a pointer to a
        /// null-terminated buffer that contains the path. The calling application is
        /// responsible for calling CoTaskMemFree to free this resource
        /// when it is no longer needed.</returns>
        [return: MarshalAs(UnmanagedType.LPWStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPath([In] int option, [Out] out IntPtr path);

        /// <summary>
        /// Assigns a new path to a known folder.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetPath([In] int i, [In] string path);

        /// <summary>
        /// Gets the location of the Shell namespace folder in the IDList (ITEMIDLIST) form.
        /// 
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb761760(v=vs.85).aspx
        /// </summary>
        /// <param name="flag">Flags that specify special retrieval options. This value can be 0; otherwise, one or more of the KNOWN_FOLDER_FLAG values.</param>
        /// <param name="itemIdentifierListPointer">Type: PIDLIST_ABSOLUTE*
        /// When this method returns, contains the address of an absolute PIDL. This parameter is passed uninitialized.
        /// The caller is responsible for freeing the returned PIDL when it is no longer needed by calling ILFree.</param>
        /// <returns>f this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        /// <remarks>Equivalent to Shell32.SHGetKnownFolderIDList.</remarks>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetIDList([In] uint flag,
            [Out] out IntPtr itemIdentifierListPointer);

        /// <summary>
        /// Retrieves the folder type.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Guid GetFolderType();

        /// <summary>
        /// Gets a value that states whether the known folder can have its path set
        /// to a new value or
        /// what specific restrictions or prohibitions are placed on that redirection.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        RedirectionCapability GetRedirectionCapabilities();

        /// <summary>
        /// Retrieves a structure that contains the defining elements of a known folder,
        /// which includes the folder's category, name, path, description, tooltip, icon, and other properties.
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761754(v=vs.85).aspx"/>
        /// 
        /// </summary>
        /// <param name="definition">When this method returns, contains a pointer to the KNOWNFOLDER_DEFINITION structure.
        /// 
        /// When no longer needed, the calling application is responsible for calling
        /// FreeKnownFolderDefinitionFields
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb776424(v=vs.85).aspx"/>
        /// 
        /// to free this resource.</param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolderDefinition([Out, MarshalAs(UnmanagedType.Struct)]
                                 out NativeFolderDefinition definition);
    }
}
