namespace DirectoryInfoExLib.IO.Header.KnownFolder.Enums
{
    using System;

    /// <summary>
    /// Describes the public definition for a known folder.
    /// </summary>
    public struct KnownFolderDefinition
    {
        /// <summary>
        /// Gets the category by which a folder
        /// registered with the Known Folder system can be classified.
        /// </summary>
        public KnownFolderCategory Category;

        /// <summary>
        /// Gets the non-localized, canonical name for the known folder,
        /// stored as a null-terminated Unicode string. If this folder is
        /// a common or per-user folder, this value is also used as the value
        /// name of the "User Shell Folders" registry settings.
        /// 
        /// This name is meant to be a unique, human-readable name.
        /// Third parties are recommended to follow the format Company.Application.Name.
        /// 
        /// The name given here should not be confused with the display name.
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets a short description of the known folder,
        /// stored as a null-terminated Unicode string.
        /// 
        /// This description should include the folder's purpose and usage.
        /// </summary>
        public string Description;

        /// <summary>
        /// Gets a KNOWNFOLDERID value that names another known folder
        /// to serve as the parent folder. Applies to common and per-user
        /// folders only. This value is used in conjunction with pszRelativePath.
        /// 
        /// This value is optional if no value is provided for pszRelativePath.
        /// </summary>
        public Guid ParentID;

        /// <summary>
        /// Optional. Gets a path relative to the parent folder specified
        /// in fidParent. This is a null-terminated Unicode string,
        /// refers to the physical file system path, and is not localized.
        /// 
        /// Applies to common and per-user folders only.
        /// </summary>
        public string RelativePath;

        /// <summary>
        /// Gets a string to the Shell namespace folder path of the folder,
        /// stored as a null-terminated Unicode string. Applies to virtual
        /// folders only.
        /// 
        /// For example, Control Panel has a parsing name of ::%CLSID_MyComputer%\::%CLSID_ControlPanel%.
        /// </summary>
        public string ParsingName;

        /// <summary>
        /// Optional. Gets a default tooltip resource used for this
        /// known folder when it is created. This is a
        /// null-terminated Unicode string in this form: 
        /// 
        /// Module name, Resource ID
        /// 
        /// For example, @%_SYS_MOD_PATH%,-12688 is the tooltip for
        /// Common Pictures. When the folder is created, this string
        /// is stored in that folder's copy of Desktop.ini.
        /// 
        /// It can be changed later by other Shell APIs.
        /// This resource might be localized.
        /// 
        /// This information is not required for virtual folders.
        /// </summary>
        public string Tooltip;

        /// <summary>
        /// Optional. Gets a string to the default localized name resource
        /// used when the folder is created. This is a null-terminated
        /// Unicode string in this form:
        /// 
        /// Module name, Resource ID
        /// 
        /// When the folder is created, this string is stored in that folder's copy of Desktop.ini. It can be changed later by other Shell APIs.
        /// 
        /// This information is not required for virtual folders.
        /// </summary>
        public string LocalizedName;

        /// <summary>
        /// Optional.
        /// A string Uri to the default icon resource used when the
        /// folder is created. This is a null-terminated Unicode string
        /// in this form:
        /// 
        /// Module name, Resource ID
        /// 
        /// When the folder is created, this string is stored in that
        /// folder's copy of Desktop.ini. It can be changed later
        /// by other Shell APIs.
        /// 
        /// This information is not required for virtual folders.
        /// </summary>
        public string Icon;

        /// <summary>
        /// Optional. A Uri string to a Security Descriptor
        /// Definition Language format string.
        /// 
        /// This is a null-terminated Unicode string that describes the
        /// default security descriptor that the folder receives when
        /// it is created.
        /// 
        /// If this parameter is NULL, the new folder inherits the
        /// security descriptor of its parent. This is particularly
        /// useful for common folders that are accessed by all users.
        /// </summary>
        public string Security;

        /// <summary>
        /// Optional. Default file system attributes given to the folder
        /// when it is created. For example, the file could be hidden
        /// and read-only (FILE_ATTRIBUTE_HIDDEN and FILE_ATTRIBUTE_READONLY).
        /// 
        /// For a complete list of possible values, see the dwFlagsAndAttributes
        /// parameter of the CreateFile function. Set to -1 if not needed.
        /// </summary>
        public UInt32 Attributes;

        /// <summary>
        /// Optional. One of more values from the KF_DEFINITION_FLAGS
        /// enumeration that allow you to restrict redirection,
        /// allow PC-to-PC roaming, and control the time at which the
        /// known folder is created. Set to 0 if not needed.
        /// </summary>
        public KnownFolderDefinitionFlags DefinitionFlags;

        /// <summary>
        /// One of the FOLDERTYPEID values that identifies the known folder
        /// type based on its contents (such as documents, music,
        /// or photographs).
        /// 
        /// This value is a GUID.
        /// </summary>
        public Guid FolderTypeID;
    }
}
