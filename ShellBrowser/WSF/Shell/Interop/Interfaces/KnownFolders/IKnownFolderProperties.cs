namespace WSF.Shell.Interop.Interfaces.Knownfolders
{
    using System;
    using System.IO;
    using WSF.Shell.Interop.Interfaces.KnownFolders;
    using WSF.Shell.Pidl;

    /// <summary>
    /// Gets property values for a known folder.
    ///
    /// This class holds the information returned in the KNOWNFOLDER_DEFINITION structure,
    /// and resources referenced by fields in NativeFolderDefinition, such as icon and tool tip.
    ///
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb773325(v=vs.85).aspx
    /// </summary>
    public interface IKnownFolderProperties
    {
        #region properties
        /// <summary>
        /// Gets a string to the non-localized, canonical name for the known folder,
        /// stored as a null-terminated Unicode string. If this folder is a common
        /// or per-user folder, this value is also used as the value name of the
        /// "User Shell Folders" registry settings. This name is meant to be a unique,
        /// human-readable name. Third parties are recommended to follow the format
        /// Company.Application.Name. The name given here should not be confused with the display name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a single value from the KF_CATEGORY constants that classifies the folder as:
        /// virtual, fixed, common, or per-user.
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb762512(v=vs.85).aspx
        /// </summary>
        FolderCategory Category { get; }

        /// <summary>
        /// Gets a string to the non-localized, canonical name for the known folder,
        /// stored as a null-terminated Unicode string. If this folder is a common
        /// or per-user folder, this value is also used as the value name of the
        /// "User Shell Folders" registry settings. This name is meant to be a unique,
        /// human-readable name. Third parties are recommended to follow the format
        /// Company.Application.Name. The name given here should not be confused with the display name.
        /// </summary>
        string CanonicalName { get; }

        /// <summary>
        /// Gets an optional One of more values from the KF_DEFINITION_FLAGS enumeration
        /// (https://msdn.microsoft.com/en-us/library/windows/desktop/bb762513(v=vs.85).aspx)
        ///
        /// that allow you to restrict redirection, allow PC-to-PC roaming, and control the time
        /// at which the known folder is created. Set to 0 if not needed.
        /// </summary>
        DefinitionOptions DefinitionOptions { get; }

        /// <summary>
        /// Gets a string to a short description of the known folder, stored as a null-terminated
        /// Unicode string. This description should include the folder's purpose and usage.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets 
        /// </summary>
        FileAttributes FileAttributes { get; }

        /// <summary>
        /// Type: KNOWNFOLDERID (https://msdn.microsoft.com/en-us/library/windows/desktop/dd378457(v=vs.85).aspx)
        /// 
        /// Gets the KNOWNFOLDERID of this folder.
        /// </summary>
        Guid FolderId { get; }

        /// <summary>
        /// Gets Type: FOLDERTYPEID (https://msdn.microsoft.com/en-us/library/windows/desktop/bb762581(v=vs.85).aspx)
        ///
        /// One of the FOLDERTYPEID values that identifies the known folder type based on its
        /// contents (such as documents, music, or photographs). This value is a GUID.
        /// </summary>
        Guid FolderTypeId { get; }

        /// <summary>
        /// Gets an optional pointer to the default icon resource used when the folder is created.
        /// This is a null-terminated Unicode string in this form:
        ///
        /// Module name, Resource ID
        /// When the folder is created, this string is stored in that folder's copy of Desktop.ini.
        /// It can be changed later by other Shell APIs.
        ///
        /// This information is not required for virtual folders.
        /// </summary>
        string IconResourceId { get; }

        /// <summary>
        /// Gets an optional string to the default localized name of this item.
        /// 
        /// When the folder is created, this string is stored in that folder's copy of Desktop.ini.
        /// It can be changed later by other Shell APIs.
        /// This information is not required for virtual folders.
        /// </summary>
        string LocalizedName { get; }

        /// <summary>
        /// Gets an optional string to the default localized name resource used when the folder is created.
        /// This is a null-terminated Unicode string in this form:
        /// Module name, Resource ID
        /// 
        /// When the folder is created, this string is stored in that folder's copy of Desktop.ini.
        /// It can be changed later by other Shell APIs.
        /// This information is not required for virtual folders.
        /// </summary>
        string LocalizedNameResourceId { get; }

        /// <summary>
        /// Gets a KNOWNFOLDERID value that names another known folder to serve as the parent folder.
        /// Applies to common and per-user folders only. This value is used in conjunction with pszRelativePath.
        /// See Remarks for more details.
        ///
        /// This value is optional if no value is provided for pszRelativePath.
        /// </summary>
        string Parent { get; }

        /// <summary>
        /// Type KNOWNFOLDERID (https://msdn.microsoft.com/en-us/library/windows/desktop/dd378457(v=vs.85).aspx)
        /// Gets a KNOWNFOLDERID value that names another known folder to serve as the parent folder.
        ///
        /// Applies to common and per-user folders only.
        /// This value is used in conjunction with pszRelativePath.
        /// See Remarks for more details.
        ///
        /// This value is optional if no value is provided for pszRelativePath.
        /// </summary>
        Guid ParentId { get; }

        /// <summary>
        /// Gets a string to the Shell namespace folder path of the folder,
        /// stored as a null-terminated Unicode string.
        ///
        /// Applies to virtual folders only.
        /// For example, Control Panel has a parsing name of ::%CLSID_MyComputer%\::%CLSID_ControlPanel%.
        /// </summary>
        string ParsingName { get; }

        /// <summary>
        /// Gets a path of the known folder representation in the file system if this folder
        /// has a representation in the file system (is not virtual).
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets whether the known folder object managed in this object is a folder
        /// or file objects that are part of the file system,
        /// that is, its a file, directory, or root directory.
        /// 
        /// A known folder with <see cref="IsExistsInFileSystem"/> = true also
        /// has <see cref="IsPathExists"/> = true but 
        /// <see cref="IsPathExists"/> = true does not require a particular value in
        /// <see cref="IsExistsInFileSystem"/>.
        /// </summary>
        bool IsExistsInFileSystem { get; }

        /// <summary>
        /// Gets whether the known folder has a path - that is, it has a path
        /// in the Path property but not necessarily a path in the file system.
        /// 
        /// <seealso cref="Path"/> and <see cref="IsExistsInFileSystem"/>
        /// </summary>
        bool IsPathExists { get; }

        /// <summary>
        /// Gets a value that states whether the known folder can have its path set to a new value
        /// or what specific restrictions or prohibitions are placed on that redirection.
        /// 
        /// Source: IKnownFolder::GetRedirectionCapabilities
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb761764(v=vs.85).aspx
        /// </summary>
        RedirectionCapability Redirection { get; }

        /// <summary>
        /// Gets an optional string to a path relative to the parent folder specified in fidParent.
        /// This is a null-terminated Unicode string, refers to the physical file system path, and
        /// is not localized.
        ///
        /// Applies to common and per-user folders only.
        /// See Remarks for more details.
        /// </summary>
        string RelativePath { get; }

        /// <summary>
        /// Gets an optional Security Descriptor Definition Language format string.
        /// This is a null-terminated Unicode string that describes the default security
        /// descriptor that the folder receives when it is created. If this parameter is NULL,
        /// the new folder inherits the security descriptor of its parent. This is particularly
        /// useful for common folders that are accessed by all users.
        /// </summary>
        string Security { get; }

        /// <summary>
        /// Gets an optional string to the default tooltip for this known folder
        /// when it is created. This is a null-terminated Unicode string in this form:
        /// 
        /// When the folder is created, this string is stored in that folder's copy of Desktop.ini.
        /// It can be changed later by other Shell APIs. This resource might be localized.
        /// This information is not required for virtual folders.
        /// </summary>
        string Tooltip { get; }

        /// <summary>
        /// Gets an optional string to the default tooltip resource used for this known folder
        /// when it is created. This is a null-terminated Unicode string in this form:
        /// 
        /// Module name, Resource ID
        /// 
        /// For example, @%_SYS_MOD_PATH%,-12688 is the tooltip for Common Pictures. When the folder is created, this string is stored in that folder's copy of Desktop.ini. It can be changed later by other Shell APIs. This resource might be localized.
        /// This information is not required for virtual folders.
        /// </summary>
        string TooltipResourceId { get; }

        /// <summary>
        /// Gets the Pidl Id List in the form of a standard .Net object.
        /// </summary>
        IdList PidlIdList { get; }

        /// <summary>
        /// Gets the standard ToString() representation of this object (for debugging only).
        /// </summary>
        string ToString();
        #endregion properties

        #region methods
        /// <summary>
        /// Determines whether the string in <paramref name="iconResourceId"/> contains
        /// a valid resource id reference of the sample form 'dll, -3'.
        /// 
        /// Call this method without parameter to determine this for
        /// the <see cref="IconResourceId"/> property contained in this object.
        /// </summary>
        /// <param name="iconResourceId"></param>
        /// <returns>True if resource id has more than zero characters and a ','
        /// character at an index larger 2, otherwise false.</returns>
        bool IsIconResourceIdValid(string iconResourceId = null);

        /// <summary>
        /// Resets an icons resource id. Use this property to overwrite available
        /// values or consider alternative options for retrieving the correct resource id string.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="index"></param>
        void ResetIconResourceId(string filename, int index);
        #endregion methods
    }
}