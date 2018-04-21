namespace DirectoryInfoExLib.IO.Header.KnownFolder.Interfaces
{
    using DirectoryInfoExLib.IO.Header.KnownFolder.Enums;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Exposes methods that create, enumerate or
    /// manage existing known folders.
    /// </summary>
    [Guid("8BE2D872-86AA-4d47-B776-32CCA40C7018"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IKnownFolderManager
    {
        /// <summary>
        /// Gets the KNOWNFOLDERID that is the equivalent of
        /// a legacy CSIDL value.
        /// </summary>
        /// <param name="Csidl"></param>
        /// <param name="knownFolderID"></param>
        void FolderIdFromCsidl(int Csidl, [Out] out Guid knownFolderID);

        /// <summary>
        /// Gets the legacy CSIDL value that is
        /// the equivalent of a given KNOWNFOLDERID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Csidl"></param>
        void FolderIdToCsidl([In, MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out] out int Csidl);

        /// <summary>
        /// Gets an array of all registered known folder IDs.
        /// This can be used in enumerating all known folders.
        /// </summary>
        /// <param name="folders"></param>
        /// <param name="count"></param>
        void GetFolderIds([Out] out IntPtr folders, [Out] out UInt32 count);

        /// <summary>
        /// Gets an object that represents a known folder identified
        /// by its KNOWNFOLDERID. The object allows you to query certain
        /// folder properties, get the current path of the folder,
        /// redirect the folder to another location, and get the
        /// path of the folder as an ITEMIDLIST.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="knownFolder"></param>
        void GetFolder([In, MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder knownFolder);

        /// <summary>
        /// Gets an object that represents a known folder identified by its
        /// canonical name. The object allows you to query certain folder
        /// properties, get the current path of the folder, redirect the
        /// folder to another location, and get the path of the folder as an
        /// ITEMIDLIST.
        /// </summary>
        /// <param name="canonicalName"></param>
        /// <param name="knowFolder"></param>
        void GetFolderByName(string canonicalName, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder knowFolder);

        /// <summary>
        /// Adds a new known folder to the registry. Used particularly by ISVs that are adding one
        /// of their own folders to the known folder system.
        /// </summary>
        void RegisterFolder();  //([in] REFKNOWNFOLDERID rfid,[in] KNOWNFOLDER_DEFINITION const *pKFD);

        /// <summary>
        /// Remove a known folder from the registry, which makes it
        /// unknown to the known folder system. This method does not
        /// remove the folder itself.
        /// </summary>
        void UnregisterFolder(); //([in] REFKNOWNFOLDERID rfid);

        /// <summary>
        /// Gets an object that represents a known folder based on a file
        /// system path. The object allows you to query certain folder
        /// properties, get the current path of the folder, redirect
        /// the folder to another location, and get the path of the folder
        /// as an ITEMIDLIST.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="knownFolder"></param>
        void FindFolderFromPath([In, MarshalAs(UnmanagedType.LPWStr)] string path, [In] KnownFolderFindMode mode, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder knownFolder);

        //void FindFolderFromIDList(IntPtr pidl, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder knownFolder);

        /// <summary>
        /// Gets an object that represents a known folder based on an IDList.
        /// The object allows you to query certain folder properties,
        /// get the current path of the folder, redirect the folder to
        /// another location, and get the path of the folder as an ITEMIDLIST.
        /// </summary>
        /// <param name="pidl"></param>
        /// <param name="knownFolder"></param>
        /// <returns></returns>
        [PreserveSig]
        int FindFolderFromIDList(IntPtr pidl, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder knownFolder);

        /// <summary>
        /// Redirects folder requests for common and per-user folders.
        /// </summary>
        void Redirect(); //( [in] REFKNOWNFOLDERID rfid, [in, unique] HWND hwnd, [in] KF_REDIRECT_FLAGS flags,[in, unique, string] LPCWSTR pszTargetPath, [in] UINT cFolders, [in, size_is(cFolders), unique] KnownFolderID const *pExclusion,[out, string] LPWSTR* ppszError);
    }
}
