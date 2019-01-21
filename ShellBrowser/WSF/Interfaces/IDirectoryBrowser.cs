namespace WSF.Interfaces
{
    using WSF.Enums;
    using WSF.Shell.Pidl;
    using System;

    /// <summary>
    /// Represents a directory in a PIDL Shell system using an abstract .Net
    /// PIDL representation as <see cref="IdList"/> objects.
    /// https://docs.microsoft.com/en-us/windows/desktop/api/shtypes/ns-shtypes-_itemidlist
    /// </summary>
    public interface IDirectoryBrowser : ICloneable, IEquatable<IDirectoryBrowser>
    {
        #region properties
        /// <summary>
        /// Gets the name of a directory.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a label string that may differ from other naming
        /// strings if the item (eg Drive) supports labeling.
        /// 
        /// String is suitable for display only and should
        /// not be used as index as it is not unique.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Get Known FolderId or file system Path for this folder.
        /// 
        /// That is:
        /// 1) A knownfolder GUID (if it exists) is shown
        ///    here as default preference over
        ///    
        /// 2) A storage location (if it exists) in the filesystem
        /// </summary>
        string PathShell { get; }

        /// <summary>
        /// Contains the special path GUID if this item is a special shell space item.
        /// </summary>
        string SpecialPathId { get; }

        /// <summary>
        /// Gets the filesystem path (e.g. 'C:\') if this item has a dedicated
        /// or associated storage location in the file system.
        /// </summary>
        string PathFileSystem { get; }


        /// <summary>
        /// Gets the IdList (if available) that describes the full
        /// shell path for this item)
        /// </summary>
        IdList ParentIdList { get; }

        /// <summary>
        /// Gets the IdList (if available) that describes the full
        /// shell path for this item)
        /// </summary>
        IdList ChildIdList { get; }

        /// <summary>
        /// Gets an optional pointer to the default icon resource used when the folder is created.
        /// This is a null-terminated Unicode string in this form:
        ///
        /// Module name, Resource ID
        /// or null is this information is not available.
        /// </summary>
        string IconResourceId { get; }

        /// <summary>
        /// Gets the folders type classification.
        /// </summary>
        DirectoryItemFlags ItemType { get; }

        /// <summary>
        /// Gets a type of path handler that indicates the
        /// handling object that should be used to manipulate
        /// this item and its children.
        /// </summary>
        PathHandler PathType { get; }

        /// <summary>
        /// Get Known file system Path  or FolderId for this folder.
        /// 
        /// That is:
        /// 1) A storage location (if it exists) in the filesystem
        /// 
        /// 2) A knownfolder GUID (if it exists) is shown
        ///    here as default preference over
        ///    
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the raw string that was used to construct this object.
        /// This property is for debug/filter purposes only and should not be
        /// visible in any UI or ViewModel.
        /// </summary>
        string PathRAW { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Determines if this item refers to an existing path in the filesystem or not.
        /// </summary>
        /// <returns></returns>
        bool DirectoryPathExists();

        /// <summary>
        /// Compares a given parse name with the parse names known in this object.
        /// 
        /// Considers case insensitive string matching for:
        /// 1> SpecialPathId
        ///   1.2> PathRAW (if SpecialPathId fails and CLSID may have been used to create this)
        ///
        /// 3> PathFileSystem
        /// </summary>
        /// <param name="parseName">True is a matching parse name was found and false if not.</param>
        /// <returns></returns>
        bool EqualsParseName(string parseName);
        #endregion methods
    }
}
