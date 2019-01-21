namespace WSF.Shell.Enums
{
    using System;

    /// <summary>
    /// SFGAOF => ShellFileGetAttributesOptions
    /// Attributes that can be retrieved on an item (file or folder) or set of items.
    ///
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb762589(v=vs.85).aspx
    /// </summary>
    [Flags]
    public enum SFGAOF
    {
        /// <summary>
        /// The specified items can be copied.
        /// </summary>
        CanCopy = 0x00000001,

        /// <summary>
        /// The specified items can be moved.
        /// </summary>
        CanMove = 0x00000002,

        /// <summary>
        /// Shortcuts can be created for the specified items. This flag has the same value as DROPEFFECT. 
        /// The normal use of this flag is to add a Create Shortcut item to the shortcut menu that is displayed 
        /// during drag-and-drop operations. However, SFGAO_CANLINK also adds a Create Shortcut item to the Microsoft 
        /// Windows Explorer's File menu and to normal shortcut menus. 
        /// If this item is selected, your application's IContextMenu::InvokeCommand is invoked with the lpVerb 
        /// member of the CMINVOKECOMMANDINFO structure set to "link." Your application is responsible for creating the link.
        /// </summary>
        CanLink = 0x00000004,

        /// <summary>
        /// The specified items can be bound to an IStorage interface through IShellFolder::BindToObject.
        /// </summary>
        Storage = 0x00000008,

        /// <summary>
        /// The specified items can be renamed.
        /// </summary>
        CanRename = 0x00000010,

        /// <summary>
        /// The specified items can be deleted.
        /// </summary>
        CanDelete = 0x00000020,

        /// <summary>
        /// The specified items have property sheets.
        /// </summary>
        HasPropertySheet = 0x00000040,

        /// <summary>
        /// The specified items are drop targets.
        /// </summary>
        DropTarget = 0x00000100,

        /// <summary>
        /// This flag is a mask for the capability flags.
        /// </summary>
        CapabilityMask = 0x00000177,

        /// <summary>
        /// Windows 7 and later. The specified items are system items.
        /// </summary>
        System = 0x00001000,

        /// <summary>
        /// The specified items are encrypted.
        /// </summary>
        Encrypted = 0x00002000,

        /// <summary>
        /// Indicates that accessing the object = through IStream or other storage interfaces, 
        /// is a slow operation. 
        /// Applications should avoid accessing items flagged with SFGAO_ISSLOW.
        /// </summary>
        IsSlow = 0x00004000,

        /// <summary>
        /// The specified items are ghosted icons.
        /// </summary>
        Ghosted = 0x00008000,

        /// <summary>
        /// The specified items are shortcuts.
        /// </summary>
        Link = 0x00010000,

        /// <summary>
        /// The specified folder objects are shared.
        /// </summary>    
        Share = 0x00020000,

        /// <summary>
        /// The specified items are read-only. In the case of folders, this means 
        /// that new items cannot be created in those folders.
        /// </summary>
        ReadOnly = 0x00040000,

        /// <summary>
        /// The item is hidden and should not be displayed unless the 
        /// Show hidden files and folders option is enabled in Folder Settings.
        /// </summary>
        Hidden = 0x00080000,

        /// <summary>
        /// This flag is a mask for the display attributes.
        /// </summary>
        DisplayAttributeMask = 0x000FC000,

        /// <summary>
        /// The specified folders contain one or more file system folders.
        /// </summary>
        FileSystemAncestor = 0x10000000,

        /// <summary>
        /// The specified items are folders.
        /// </summary>
        Folder = 0x20000000,

        /// <summary>
        /// The specified folders or file objects are part of the file system 
        /// that is, they are files, directories, or root directories).
        /// </summary>
        FileSystem = 0x40000000,

        /// <summary>
        /// The specified folders have subfolders = and are, therefore, 
        /// expandable in the left pane of Windows Explorer).
        /// </summary>
        HasSubFolder = unchecked((int)0x80000000),

        /// <summary>
        /// This flag is a mask for the contents attributes.
        /// </summary>
        ContentsMask = unchecked((int)0x80000000),

        /// <summary>
        /// When specified as input, SFGAO_VALIDATE instructs the folder to validate that the items 
        /// pointed to by the contents of apidl exist. If one or more of those items do not exist, 
        /// IShellFolder::GetAttributesOf returns a failure code. 
        /// When used with the file system folder, SFGAO_VALIDATE instructs the folder to discard cached 
        /// properties retrieved by clients of IShellFolder2::GetDetailsEx that may 
        /// have accumulated for the specified items.
        /// </summary>
        Validate = 0x01000000,

        /// <summary>
        /// The specified items are on removable media or are themselves removable devices.
        /// </summary>
        Removable = 0x02000000,

        /// <summary>
        /// The specified items are compressed.
        /// </summary>
        Compressed = 0x04000000,

        /// <summary>
        /// The specified items can be browsed in place.
        /// </summary>
        Browsable = 0x08000000,

        /// <summary>
        /// The items are nonenumerated items.
        /// </summary>
        Nonenumerated = 0x00100000,

        /// <summary>
        /// The objects contain new content.
        /// </summary>
        NewContent = 0x00200000,

        /// <summary>
        /// It is possible to create monikers for the specified file objects or folders.
        /// </summary>
        CanMoniker = 0x00400000,

        /// <summary>
        /// Not supported.
        /// </summary>
        HasStorage = 0x00400000,

        /// <summary>
        /// Indicates that the item has a stream associated with it that can be accessed 
        /// by a call to IShellFolder::BindToObject with IID_IStream in the riid parameter.
        /// </summary>
        Stream = 0x00400000,

        /// <summary>
        /// Children of this item are accessible through IStream or IStorage. 
        /// Those children are flagged with SFGAO_STORAGE or SFGAO_STREAM.
        /// </summary>
        StorageAncestor = 0x00800000,

        /// <summary>
        /// This flag is a mask for the storage capability attributes.
        /// </summary>
        StorageCapabilityMask = 0x70C50008,

        /// <summary>
        /// Mask used by PKEY_SFGAOFlags to remove certain values that are considered 
        /// to cause slow calculations or lack context. 
        /// Equal to SFGAO_VALIDATE | SFGAO_ISSLOW | SFGAO_HASSUBFOLDER.
        /// </summary>
        PkeyMask = unchecked((int)0x81044000),
    }
}
