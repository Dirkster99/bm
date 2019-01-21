namespace WSF.Shell.Interop.Dlls
{
    using WSF.Shell.Enums;
    using WSF.Shell.Interop.Interfaces.ShellItems;
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;

    /// <summary>
    /// Methods imported from Shell32.dll.
    /// 
    /// Shell32.DLL Versions:
    /// <see href = "https://msdn.microsoft.com/en-us/library/windows/desktop/bb776779(v=vs.85).aspx"/>
    /// </summary>
    internal partial class NativeMethods
    {
        /// <summary>
        /// Defines the maximum support string length path for a valid path below .Net 4.7.2.
        /// </summary>
        public const int MAX_PATH = 260;

        /// <summary>
        /// Performs an operation on a specified file.
        /// </summary>
        /// <param name="pExecInfo">A pointer to a SHELLEXECUTEINFO structure that contains and receives information about the application being executed.</param>
        /// <returns>Returns TRUE if successful; otherwise, FALSE. Call GetLastError for extended error information.</returns>
        [DllImport("shell32.dll", EntryPoint = "ShellExecuteEx", SetLastError = true)]
        internal static extern int ShellExecuteEx(ref SHELLEXECUTEINFO pExecInfo);

        /// <summary>
        /// Retrieves the path of a known folder as an ITEMIDLIST structure.
        /// </summary>
        /// <param name="rfid">A reference to the KNOWNFOLDERID that identifies the folder. The folders associated with the known folder IDs might not exist on a particular system.</param>
        /// <param name="dwFlags">Flags that specify special retrieval options. This value can be 0; otherwise, it is one or more of the KNOWN_FOLDER_FLAG values.</param>
        /// <param name="hToken">An access token used to represent a particular user. This parameter is usually set to NULL, in which case the function tries to access the current user's instance of the folder.</param>
        /// <param name="ppidl">When this method returns, contains a pointer to the PIDL of the folder. This parameter is passed uninitialized. The caller is responsible for freeing the returned PIDL when it is no longer needed by calling ILFree.</param>
        /// <returns>Returns S_OK if successful, or an error value otherwise, including the following:</returns>
        /// <remarks>See also IKnownFolderNative.GetIDList</remarks>
        [DllImport("shell32.dll", SetLastError = true)]
        internal static extern int SHGetKnownFolderIDList([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, KNOWN_FOLDER_FLAG dwFlags, IntPtr hToken, out IntPtr ppidl);

        /// <summary>
        /// Retrieves the full path of a known folder identified by the folder's KNOWNFOLDERID.
        /// </summary>
        /// <param name="rfid">A reference to the KNOWNFOLDERID that identifies the folder. The folders associated with the known folder IDs might not exist on a particular system.</param>
        /// <param name="dwFlags">Flags that specify special retrieval options. This value can be 0; otherwise, it is one or more of the KNOWN_FOLDER_FLAG values.</param>
        /// <param name="hToken">An access token used to represent a particular user. This parameter is usually set to NULL, in which case the function tries to access the current user's instance of the folder.</param>
        /// <param name="pszPath">When this method returns, contains the address of a pointer to a null-terminated Unicode string that specifies the path of the known folder. The calling process is responsible for freeing this resource once it is no longer needed by calling CoTaskMemFree. The returned path does not include a trailing backslash. For example, "C:\Users" is returned rather than "C:\Users\".</param>
        /// <returns>Returns S_OK if successful, or an error value otherwise, including the following:</returns>
        [DllImport("shell32.dll")]
        internal static extern HRESULT SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, KNOWN_FOLDER_FLAG dwFlags, IntPtr hToken, out IntPtr pszPath);

        /// <summary>
        /// Frees an ITEMIDLIST structure allocated by the Shell.
        /// </summary>
        /// <param name="pidl">A pointer to the ITEMIDLIST structure to be freed. This parameter can be NULL.</param>
        [DllImport("shell32.dll")]
        internal static extern void ILFree(IntPtr pidl);

        /// <summary>
        /// Translates a Shell namespace object's display name into an item identifier list and
        /// returns the attributes of the object. This function is the preferred method to convert
        /// a string to a pointer to an item identifier list (PIDL).
        /// </summary>
        /// <param name="pszName">Type: LPCWSTR
        /// A pointer to a zero-terminated wide string that contains the display name to parse.</param>
        /// <param name="pbc">Type: IBindCtx*
        /// A bind context that controls the parsing operation. This parameter is normally set to NULL.</param>
        /// <param name="ppidl">Type: PIDLIST_ABSOLUTE*
        /// The address of a pointer to a variable of type ITEMIDLIST that receives the item identifier list for the object.
        /// If an error occurs, then this parameter is set to NULL.
        /// The caller is responsible for freeing the returned PIDL when it is no longer needed by calling ILFree.</param>
        /// <param name="sfgaoIn">Type: SFGAOF
        /// A ULONG value that specifies the attributes to query. To query for one or more attributes,
        /// initialize this parameter with the flags that represent the attributes of interest.
        /// For a list of available SFGAO flags, see IShellFolder::GetAttributesOf.</param>
        /// <param name="psfgaoOut">Type: SFGAOF*
        /// A pointer to a ULONG. On return, those attributes that are true for the object and were requested in sfgaoIn are set.
        /// An object's attribute flags can be zero or a combination of SFGAO flags. For a list of available SFGAO flags, see IShellFolder::GetAttributesOf.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        /// <remarks>You should call this function from a background thread. Failure to do so could cause the UI to stop responding.</remarks>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT SHParseDisplayName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName,
                                                          IntPtr pbc,
                                                          out IntPtr ppidl,
                                                          SFGAOF sfgaoIn,
                                                          out SFGAOF psfgaoOut);

        /// <summary>
        /// Creates and initializes a Shell item object from a pointer to
        /// an item identifier list (PIDL). The resulting shell item object
        /// can support, for example, the IShellItem interface.
        /// 
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb762133(v=vs.85).aspx
        /// </summary>
        /// <param name="pidl">
        /// Type: PCIDLIST_ABSOLUTE
        /// The source PIDL.</param>
        /// <param name="riid">
        /// Type: REFIID (Guid)
        /// A reference to the IID of the requested interface.
        /// </param>
        /// <param name="ppv"></param>
        /// <returns>When this function returns, contains the interface pointer
        /// requested in riid. This will typically be IShellItem or IShellItem2.</returns>
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT SHCreateItemFromIDList(IntPtr pidl,
                                                              [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                                                              out IntPtr ppv);

        /// <summary>
        /// Retrieves the IShellFolder interface for the desktop folder, which is the root of the Shell's namespace.
        /// </summary>
        /// <param name="ppshf">When this method returns, receives an IShellFolder interface pointer for the desktop folder.
        /// The calling application is responsible for eventually freeing the interface by calling its IUnknown::Release method.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise,
        /// it returns an HRESULT error code.</returns>
        [DllImport("shell32.dll")]
        internal static extern HRESULT SHGetDesktopFolder(out IntPtr ppshf);

        /// <summary>
        /// Converts an item identifier list to a file system path.
        /// </summary>
        /// <param name="pidl">The address of an item identifier list that specifies a file or directory location relative to the root of the namespace (the desktop).</param>
        /// <param name="pszPath">The address of a buffer to receive the file system path. This buffer must be at least MAX_PATH characters in size.</param>
        /// <returns>Returns TRUE if successful; otherwise, FALSE.</returns>
        [DllImport("shell32.dll", EntryPoint = "SHGetPathFromIDListW", CharSet = CharSet.Unicode)]
        internal static extern bool SHGetPathFromIDList(IntPtr pidl,
                                                        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPath);

        /// <summary>
        /// Makes a copy of a string in newly allocated memory.
        /// </summary>
        /// <param name="pszSource">A pointer to the null-terminated string to be copied.</param>
        /// <param name="ppwsz">A pointer to an allocated Unicode string that contains the result. SHStrDup allocates memory for this string with CoTaskMemAlloc. You should free the string with CoTaskMemFree when it is no longer needed. In the case of failure, this value is NULL.</param>
        /// <returns>Returns S_OK if successful, or a COM error value otherwise.</returns>
        [DllImport("shlwapi.dll", EntryPoint = "SHStrDupW")]
        internal static extern int SHStrDup([MarshalAs(UnmanagedType.LPWStr)] string pszSource, out IntPtr ppwsz);

        /// <summary>
        /// Tests whether two ITEMIDLIST structures are equal in a binary comparison.
        /// </summary>
        /// <param name="pidl1">The first ITEMIDLIST structure.</param>
        /// <param name="pidl2">The second ITEMIDLIST structure.</param>
        /// <returns>Returns TRUE if the two structures are equal, FALSE otherwise.</returns>
        [DllImport("shell32.dll")]
        internal static extern bool ILIsEqual(IntPtr pidl1, IntPtr pidl2);

        /// <summary>
        /// Combines two ITEMIDLIST structures.
        /// </summary>
        /// <param name="pidl1">A pointer to the first ITEMIDLIST structure.</param>
        /// <param name="pidl2">A pointer to the second ITEMIDLIST structure. This structure is appended to the structure pointed to by pidl1.</param>
        /// <returns>Returns an ITEMIDLIST containing the combined structures. If you set either pidl1 or pidl2 to NULL, the returned ITEMIDLIST structure is a clone of the non-NULL parameter.
        /// Returns NULL if pidl1 and pidl2 are both set to NULL.</returns>
        [DllImport("shell32.dll")]
        internal static extern IntPtr ILCombine(IntPtr pidl1, IntPtr pidl2);

        /// <summary>
        /// Clones an ITEMIDLIST structure.
        /// </summary>
        /// <param name="pidl">A pointer to the ITEMIDLIST structure to be cloned.</param>
        /// <returns>Returns a pointer to a copy of the ITEMIDLIST structure pointed to by pidl.</returns>
        [DllImport("shell32.dll")]
        internal static extern IntPtr ILClone(IntPtr pidl);

        /// <summary>
        /// Retrieves information about an object in the file system, such as a file, folder, directory, or drive root.
        /// </summary>
        /// <param name="pszPath">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path and file name. Both absolute and relative paths are valid.
        /// If the uFlags parameter includes the SHGFI_PIDL flag, this parameter must be the address of an ITEMIDLIST (PIDL) structure that contains the list of item identifiers that uniquely identifies the file within the Shell's namespace. The PIDL must be a fully qualified PIDL. Relative PIDLs are not allowed.
        /// If the uFlags parameter includes the SHGFI_USEFILEATTRIBUTES flag, this parameter does not have to be a valid file name. The function will proceed as if the file exists with the specified name and with the file attributes passed in the dwFileAttributes parameter. This allows you to obtain information about a file type by passing just the extension for pszPath and passing FILE_ATTRIBUTE_NORMAL in dwFileAttributes.
        /// This string can use either short (the 8.3 form) or long file names.</param>
        /// <param name="dwFileAttribs">A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</param>
        /// <param name="psfi">A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</param>
        /// <param name="cbFileInfo">A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</param>
        /// <param name="uFlags">A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</param>
        /// <returns>A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SHGetFileInfo([MarshalAs(UnmanagedType.LPWStr)] string pszPath,
                                                    uint dwFileAttribs,
                                                    out SHFILEINFO psfi,
                                                    uint cbFileInfo,
                                                    SHGFI uFlags);

        /// <summary>
        /// Retrieves information about an object in the file system, such as a file, folder, directory, or drive root.
        /// </summary>
        /// <param name="pszPath">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path and file name. Both absolute and relative paths are valid.
        /// If the uFlags parameter includes the SHGFI_PIDL flag, this parameter must be the address of an ITEMIDLIST (PIDL) structure that contains the list of item identifiers that uniquely identifies the file within the Shell's namespace. The PIDL must be a fully qualified PIDL. Relative PIDLs are not allowed.
        /// If the uFlags parameter includes the SHGFI_USEFILEATTRIBUTES flag, this parameter does not have to be a valid file name. The function will proceed as if the file exists with the specified name and with the file attributes passed in the dwFileAttributes parameter. This allows you to obtain information about a file type by passing just the extension for pszPath and passing FILE_ATTRIBUTE_NORMAL in dwFileAttributes.
        /// This string can use either short (the 8.3 form) or long file names.</param>
        /// <param name="dwFileAttribs">A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</param>
        /// <param name="psfi">A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</param>
        /// <param name="cbFileInfo">A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</param>
        /// <param name="uFlags">A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</param>
        /// <returns>A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</returns>
        [DllImport("shell32.dll")]
        internal static extern IntPtr SHGetFileInfo(IntPtr pszPath, uint dwFileAttribs, out SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb762179(v=vs.85).aspx
        /// </summary>
        /// <param name="pszPath">
        /// 
        /// Type: LPCTSTR
        /// A pointer to a null-terminated string of maximum length MAX_PATH that contains
        /// the path and file name.Both absolute and relative paths are valid.
        /// 
        /// If the uFlags parameter includes the SHGFI_PIDL flag, this parameter must be
        /// the address of an ITEMIDLIST (PIDL) structure that contains the list of item
        /// identifiers that uniquely identifies the file within the Shell's namespace.
        /// The PIDL must be a fully qualified PIDL. Relative PIDLs are not allowed.
        /// 
        /// If the uFlags parameter includes the SHGFI_USEFILEATTRIBUTES flag, this parameter
        /// does not have to be a valid file name. The function will proceed as if the file
        /// exists with the specified name and with the file attributes passed in the
        /// dwFileAttributes parameter.This allows you to obtain information about a
        /// file type by passing just the extension for pszPath and passing FILE_ATTRIBUTE_NORMAL
        /// in dwFileAttributes.
        /// This string can use either short (the 8.3 form) or long file names.</param>
        /// <param name="dwFileAttributes">Type: DWORD
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/gg258117(v=vs.85).aspx"/>
        /// 
        /// A combination of one or more file attribute flags(FILE_ATTRIBUTE_ values as defined
        /// in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this
        /// parameter is ignored.</param>
        /// <param name="psfi">
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb759792(v=vs.85).aspx"/>
        /// </param>
        /// <param name="cbSizeFileInfo"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        [DllImport("shell32.dll", CharSet = CharSet.Ansi)]
        internal static extern IntPtr SHGetFileInfo(IntPtr pszPath, UInt32 dwFileAttributes,
                                                    ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        /// <summary>
        /// Creates a data object in a parent folder.
        /// </summary>
        /// <param name="pidlFolder">A pointer to an ITEMIDLIST (PIDL) of the parent folder that contains the data object.</param>
        /// <param name="cidl">The number of file objects or subfolders specified in the apidl parameter.</param>
        /// <param name="apidl">An array of pointers to constant ITEMIDLIST structures, each of which uniquely identifies a file object or subfolder relative to the parent folder. Each item identifier list must contain exactly one SHITEMID structure followed by a terminating zero.</param>
        /// <param name="pdtInner">A pointer to interface IDataObject. This parameter can be NULL. Specify pdtInner only if the data object created needs to support additional FORMATETC clipboard formats beyond the default formats it is assigned at creation. Alternatively, provide support for populating the created data object using non-default clipboard formats by calling method IDataObject::SetData and specifying the format in the FORMATETC structure passed in parameter pFormatetc.</param>
        /// <param name="riid">A reference to the IID of the interface to retrieve through ppv. This must be IID_IDataObject.</param>
        /// <param name="ppv">When this method returns successfully, contains the IDataObject interface pointer requested in riid.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport("shell32.dll")]
        internal static extern int SHCreateDataObject(IntPtr pidlFolder, uint cidl, IntPtr apidl, IDataObject pdtInner, Guid riid,
                                                      out IntPtr ppv);

        /// <summary>
        /// This function takes the fully-qualified pointer to an item
        /// identifier list (PIDL) of a namespace object, and returns a specified
        /// interface pointer on the parent object.
        /// 
        /// https://www.codeproject.com/articles/3551/c-does-shell-part
        /// </summary>
        /// <param name="pidl">The item's PIDL.</param>
        /// <param name="riid">The REFIID of one of the interfaces exposed by
        /// the item's parent object.</param>
        /// <param name="ppv">A pointer to the interface specified by riid.
        /// You must release the object when  you are finished.</param>
        /// <param name="ppidlLast">The item's PIDL relative to the parent folder.
        /// This PIDL can be used with many of the methods supported by the parent
        /// folder's interfaces. If you set ppidlLast to NULL, the PIDL will not
        /// be returned.</param>
        /// <returns></returns>
        [DllImport("shell32.dll")]
        internal static extern HRESULT SHBindToParent(IntPtr pidl,
                                                      [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                                                      out IntPtr ppv,
                                                      ref IntPtr ppidlLast);

        /// <summary>
        /// Creates and initializes a Shell item object from a parsing name.
        /// 
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb762134(v=vs.85).aspx
        /// </summary>
        /// <param name="path">Type: PCWSTR
        /// A pointer to a display name.</param>
        /// <param name="pbc">Type: IBindCtx*
        /// Optional.A pointer to a bind context used to pass parameters as
        /// inputs and outputs to the parsing function.These passed parameters
        /// are often specific to the data source and are documented by the
        /// data source owners. For example, the file system data source
        /// accepts the name being parsed (as a WIN32_FIND_DATA structure),
        /// using the STR_FILE_SYS_BIND_DATA bind context parameter.
        /// STR_PARSE_PREFER_FOLDER_BROWSING can be passed to indicate that URLs are parsed using the file system data source when possible.Construct a bind context object using CreateBindCtx and populate the values using IBindCtx::RegisterObjectParam. See Bind Context String Keys for a complete list of these.See the Parsing With Parameters Sample for an example of the use of this parameter.
        /// 
        /// If no data is being passed to or received from the parsing function, this value can be NULL.</param>
        /// <param name="riid">Type: REFIID
        /// A reference to the IID of the interface to retrieve through ppv, typically IID_IShellItem or IID_IShellItem2.</param>
        /// <param name="shellItem">Type: void**
        /// When this method returns successfully, contains the interface
        /// pointer requested in riid.This is typically IShellItem or IShellItem2.</param>
        /// <returns>ype: HRESULT
        /// If this function succeeds, it returns S_OK.
        /// Otherwise, it returns an HRESULT error code.</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string path,
            // The following parameter is not used - binding context.
            IntPtr pbc,
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem2 shellItem);

        /// <summary>
        /// Retrieves the pointer to an item identifier list (PIDL) of an object.
        /// 
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb762184(v=vs.85).aspx
        /// </summary>
        /// <param name="pUnk">Type: IUnknown* https://msdn.microsoft.com/en-us/library/windows/desktop/ms680509(v=vs.85).aspx
        /// 
        /// A pointer to the IUnknown of the object from which to get the PIDL</param>
        /// <param name="pidl">Type: PIDLIST_ABSOLUTE*
        /// When this function returns, contains a pointer to the PIDL of the given object.
        /// </param>
        /// <returns></returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHGetIDListFromObject(
            IntPtr pUnk,
            out IntPtr pidl
            );
    }
}