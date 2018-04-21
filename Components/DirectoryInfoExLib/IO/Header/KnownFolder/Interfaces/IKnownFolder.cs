namespace DirectoryInfoExLib.IO.Header.KnownFolder.Interfaces
{
    using DirectoryInfoExLib.IO.Header.KnownFolder.Enums;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Exposes methods that allow an application to retrieve information
    /// about a known folder's category, type, GUID, pointer to an item
    /// identifier list (PIDL) value, redirection capabilities,
    /// and definition. It provides a method for the retrival
    /// of a known folder's IShellItem object. It also provides methods
    /// to get or set the path of the known folder.
    /// </summary>
    [ComImport, Guid("3AA7AF7E-9B36-420c-A8E3-F77D4674A488"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IKnownFolder
    {
        /// <summary>
        /// Gets the ID of the selected folder.
        /// </summary>
        /// <param name="Id"></param>
        void GetId([Out] out Guid Id);

        /// <summary>
        /// Retrieves the category—virtual,
        /// fixed, common,
        /// or per-user—of the selected folder.
        /// </summary>
        /// <param name="category"></param>
        void GetCategory([Out] out KnownFolderCategory category);

        /// <summary>
        /// Retrieves the location of a known folder in the Shell namespace
        /// in the form of a Shell item (IShellItem or derived interface).
        /// </summary>
        /// <param name="retrievalOptions"></param>
        /// <param name="interfaceGuid"></param>
        /// <param name="shellItem"></param>
        void GetShellItem(KnownFolderRetrievalOptions retrievalOptions,
                          [MarshalAs(UnmanagedType.LPStruct)] Guid interfaceGuid,
                          [Out, MarshalAs(UnmanagedType.IUnknown)] out object shellItem);

        /// <summary>
        /// Retrieves the path of a known folder as a string.
        /// </summary>
        /// <param name="retrievalOptions"></param>
        /// <param name="path"></param>
        void GetPath(KnownFolderRetrievalOptions retrievalOptions, [Out] out IntPtr path);

        /// <summary>
        /// Assigns a new path to a known folder.
        /// </summary>
        /// <param name="retrievalOptions"></param>
        /// <param name="path"></param>
        void SetPath(KnownFolderRetrievalOptions retrievalOptions, string path);

        /// <summary>
        /// Gets the location of the Shell namespace folder in the IDList (ITEMIDLIST) form.
        /// </summary>
        /// <param name="retrievalOptions"></param>
        /// <param name="itemIdentifierListPointer"></param>
        /// <returns></returns>
        int GetIDList(KnownFolderRetrievalOptions retrievalOptions, [Out] out IntPtr itemIdentifierListPointer);

        /// <summary>
        /// Retrieves the folder type.
        /// </summary>
        /// <param name="folderTypeID">When this returns, contains a pointer
        /// to a FOLDERTYPEID (a GUID) that identifies the known folder type.</param>
        void GetFolderType([Out, MarshalAs(UnmanagedType.LPStruct)] out Guid folderTypeID);

        /// <summary>
        /// Gets a value that states whether the known folder
        /// can have its path set to a new value or what specific restrictions
        /// or prohibitions are placed on that redirection.
        /// </summary>
        /// <param name="redirectionCapabilities"></param>
        void GetRedirectionCapabilities([Out] out KnownFolderRedirectionCapabilities redirectionCapabilities);

        /// <summary>
        /// Retrieves a structure that contains the defining elements
        /// of a known folder, which includes the folder's category,
        /// name, path, description, tooltip, icon, and other properties.
        /// </summary>
        /// <param name="definition"></param>
        void GetFolderDefinition([Out, MarshalAs(UnmanagedType.Struct)] out InternalKnownFolderDefinition definition);
    }
}
