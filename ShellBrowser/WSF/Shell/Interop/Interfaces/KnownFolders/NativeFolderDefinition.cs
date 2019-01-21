namespace WSF.Shell.Interop.Interfaces.KnownFolders
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Defines the specifics of a known folder.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeFolderDefinition
    {
        /// <summary>
        /// A single value from the KF_CATEGORY constants that classifies the folder
        /// as virtual, fixed, common, or per-user.
        /// </summary>
        internal FolderCategory category;

        /// <summary>
        /// The non-localized, canonical name for the known folder,
        /// stored as a null-terminated Unicode string.
        /// If this folder is a common or per-user folder, this value is also used
        /// as the value name of the "User Shell Folders" registry settings.
        /// This name is meant to be a unique, human-readable name.
        /// Third parties are recommended to follow the format Company.Application.Name.
        /// The name given here should not be confused with the display name.
        /// </summary>
        internal IntPtr name;

        /// <summary>
        /// A pointer to a short description of the known folder,
        /// stored as a null-terminated Unicode string.
        /// This description should include the folder's purpose and usage.
        /// </summary>
        internal IntPtr description;

        /// <summary>
        /// Type: KNOWNFOLDERID
        ///
        /// A KNOWNFOLDERID value that names another known folder to serve as the
        /// parent folder.Applies to common and per-user folders only.This value
        /// is used in conjunction with pszRelativePath.See Remarks for more details.
        ///
        /// This value is optional if no value is provided for pszRelativePath.
        /// </summary>
        internal Guid parentId;

        /// <summary>
        /// Type: LPWSTR
        /// Optional. A pointer to a path relative to the parent folder specified in
        /// fidParent.This is a null-terminated Unicode string, refers to the
        /// physical file system path, and is not localized. Applies to common and
        /// per-user folders only.See Remarks for more details.
        /// </summary>
        internal IntPtr relativePath;

        /// <summary>
        /// Type: LPWSTR
        ///
        /// A pointer to the Shell namespace folder path of the folder,
        /// stored as a null-terminated Unicode string. Applies to virtual folders
        /// only.For example, Control Panel has a parsing name
        /// of ::%CLSID_MyComputer%\::%CLSID_ControlPanel%.
        /// </summary>
        internal IntPtr parsingName;

        /// <summary>
        /// the default tooltip resource used for this known folder when it is created.
        /// This is a null-terminated Unicode string in this form:
        /// 
        /// Module name, Resource ID
        /// 
        /// For example, @%_SYS_MOD_PATH%,-12688 is the tooltip for Common Pictures.When the folder is created, this string is stored in that folder's copy of Desktop.ini. It can be changed later by other Shell APIs. This resource might be localized.
        /// 
        /// This information is not required for virtual folders.
        /// /// </summary>
        internal IntPtr tooltip;

        /// <summary>
        /// Type: LPWSTR
        /// 
        /// Optional. A pointer to the default localized name resource used when
        /// the folder is created.This is a null-terminated Unicode string in this
        /// form:
        /// 
        /// Module name, Resource ID
        /// 
        /// When the folder is created, this string is stored in that folder's copy
        /// of Desktop.ini. It can be changed later by other Shell APIs.
        /// 
        /// This information is not required for virtual folders.
        /// </summary>
        internal IntPtr localizedName;

        /// <summary>
        /// Type: LPWSTR
        /// 
        /// Optional. A pointer to the default icon resource used when the folder is created. This is a null-terminated Unicode string in this form:
        ///
        /// Module name, Resource ID
        ///
        /// When the folder is created, this string is stored in that folder's copy of Desktop.ini. It can be changed later by other Shell APIs.
        ///
        /// This information is not required for virtual folders.
        /// </summary>
        internal IntPtr icon;

        /// <summary>
        /// Type: LPWSTR
        /// 
        /// Optional. A pointer to a Security Descriptor Definition Language format string.
        /// This is a null-terminated Unicode string that describes the default security descriptor
        /// that the folder receives when it is created.
        /// 
        /// If this parameter is NULL, the new folder inherits the security descriptor of
        /// its parent. This is particularly useful for common folders that are accessed
        /// by all users.
        /// </summary>
        internal IntPtr security;

        /// <summary>
        /// Type: DWORD
        /// 
        /// Optional. Default file system attributes given to the folder when it is created.
        /// For example, the file could be hidden and read-only (FILE_ATTRIBUTE_HIDDEN and
        /// FILE_ATTRIBUTE_READONLY). For a complete list of possible values,
        /// see the dwFlagsAndAttributes parameter of the CreateFile function.
        /// 
        /// Set to -1 if not needed.
        /// </summary>
        internal UInt32 attributes;

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb762513(v=vs.85).aspx
        /// 
        /// Type: KF_DEFINITION_FLAGS
        /// 
        /// Optional. One of more values from the KF_DEFINITION_FLAGS enumeration that
        /// allow you to restrict redirection, allow PC-to-PC roaming, and control
        /// the time at which the known folder is created.Set to 0 if not needed.
        /// </summary>
        internal DefinitionOptions definitionOptions;

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb762581(v=vs.85).aspx
        /// Type: FOLDERTYPEID
        /// 
        /// One of the FOLDERTYPEID values that identifies the known folder type based
        /// on its contents(such as documents, music, or photographs). This value is a
        /// GUID.
        /// </summary>
        internal Guid folderTypeId;
    }
}
