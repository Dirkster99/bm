namespace ShellBrowserLib.Browser
{
    using ShellBrowserLib.Enums;
    using ShellBrowserLib.Interfaces;
    using System;
    using ShellBrowserLib.Shell.Pidl;

    /// <summary>
    /// Implements a light weight Windows Shell Browser class that can be used
    /// to model and browse the shell tree structure of the Windows (7-10) Shell.
    /// </summary>
    internal sealed partial class DirectoryBrowser : IDirectoryBrowser
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DirectoryBrowser(BrowseItemFromPath itemModel)
        {
            PathRAW = itemModel.Path_RAW;

            ItemType = itemModel.ItemType;
            PathType = itemModel.PathType;
            Name = itemModel.Name;
            Label = itemModel.LabelName;
            SpecialPathId = itemModel.PathSpecialItemId;
            PathFileSystem = itemModel.PathFileSystem;

            ParentIdList = itemModel.ParentIdList;
            ChildIdList = itemModel.ChildIdList;

            IconResourceId = itemModel.IconResourceId;

            // Get PathShell
            if (string.IsNullOrEmpty(itemModel.PathSpecialItemId) == false)
                PathShell = itemModel.PathSpecialItemId;
            else
            {
                if (string.IsNullOrEmpty(itemModel.PathFileSystem) == false)
                    PathShell = itemModel.PathFileSystem;
                else
                {
                    PathShell = itemModel.Name;
                }
            }

            // Get FullName
            if (string.IsNullOrEmpty(itemModel.PathFileSystem) == false)
                FullName = itemModel.PathFileSystem;
            else
            {
                if (string.IsNullOrEmpty(itemModel.PathSpecialItemId) == false)
                    FullName = itemModel.PathSpecialItemId;
                else
                {
                    FullName = itemModel.Name;
                }
            }

        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copyThis"></param>
        public DirectoryBrowser(DirectoryBrowser copyThis)
        {
            if (copyThis == null)
                return;

            ItemType = copyThis.ItemType;
            Name = copyThis.Name;
            Label = copyThis.Label;
            PathRAW = copyThis.PathRAW;
            PathShell = copyThis.PathShell;

            ChildIdList = copyThis.ChildIdList;
            ParentIdList = copyThis.ParentIdList;

            IconResourceId = copyThis.IconResourceId;
            SpecialPathId = copyThis.SpecialPathId;
            PathFileSystem = copyThis.PathFileSystem;
            PathType = copyThis.PathType;

            FullName = copyThis.FullName;
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the (localized) name of an item.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a label string that may differ from other naming
        /// strings if the item (eg Drive) supports labeling.
        /// 
        /// String is suitable for display only and should
        /// not be used as index as it is not unique.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Gets the logical path of a directory item.
        /// The logical path can differ from the physical storage location path
        /// because there are some special folder items
        /// that have no storage location (e.g. 'This PC')
        /// and a special folder can be identified:
        /// 
        /// 1) with GUID based strings (see KF_IID) or
        /// 2) via its path ('C:\Windows')
        /// 
        /// 3) via its name ('G:\')
        ///    A device that is not ready (eg: DVD drive without DVD)
        ///    will have neither a special id nor a filesystem path.
        /// </summary>
        public string PathShell { get; }

        /// <summary>
        /// Contains the special path GUID if this item is a special shell space item.
        /// </summary>
        public string SpecialPathId { get; }

        /// <summary>
        /// Gets the filesystem path (e.g. 'C:\') if this item has a dedicated
        /// or associated storage location in the file system.
        /// </summary>
        public string PathFileSystem { get; }

        /// <summary>
        /// Gets the IdList (if available) that describes the full
        /// shell path for this item)
        /// </summary>
        public IdList ParentIdList { get; }


        /// <summary>
        /// Gets the IdList (if available) that describes the full
        /// shell path for this item)
        /// </summary>
        public IdList ChildIdList { get; }

        /// <summary>
        /// Gets an optional pointer to the default icon resource used when the folder is created.
        /// This is a null-terminated Unicode string in this form:
        ///
        /// Module name, Resource ID
        /// or null is this information is not available.
        /// </summary>
        public string IconResourceId { get; }

        //// <summary>
        //// Gets the folders type classification.
        //// </summary>
        public DirectoryItemFlags ItemType { get; }

        /// <summary>
        /// Gets a type of path handler that indicates the
        /// handling object that should be used to manipulate
        /// this item and its children.
        /// </summary>
        public PathHandler PathType { get; }

        /// <summary>
        /// Gets the raw string that was used to construct this object.
        /// This property is for debug purposes only and should not be
        /// visible in any UI or ViewModel.
        /// </summary>
        public string PathRAW { get; }

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
        public string FullName { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Determines if this item refers to an existing path in the filesystem or not.
        /// </summary>
        /// <returns></returns>
        public bool DirectoryPathExists()
        {
            if (string.IsNullOrEmpty(PathFileSystem) == true)
                return false;

            // strings empty and Name with same value usually indicates non-existing item
            if (string.IsNullOrEmpty(PathFileSystem) == true &&
                string.IsNullOrEmpty(SpecialPathId) == true && Name == PathRAW)
                return false;

            // Check if this folder or drive item exists in file system
            if ((ItemType & DirectoryItemFlags.FileSystemDirectory) != 0)
            {
                bool isPath = false;

                try
                {
                    isPath = System.IO.Directory.Exists(PathFileSystem);
                }
                catch
                {
                }

                return isPath;
            }

            return false;
        }

        /// <summary>
        /// Gets a parent <see cref="IDirectoryBrowser"/> object if a parent can be determent
        /// or null if item does not have a parent.
        /// </summary>
        /// <returns></returns>
        public IDirectoryBrowser GetParent()
        {
            var itemPath = (string.IsNullOrEmpty(SpecialPathId) == false ? SpecialPathId : PathFileSystem);

            var parentPath = ShellBrowser.GetParentFolder(itemPath, TypOfPath.LogicalPath);

            if (parentPath != null)
                return ShellBrowser.Create(parentPath);

            return null;
        }

        #region ICloneable
        public object Clone()
        {
            return new DirectoryBrowser(this);
        }
        #endregion ICloneable

        /// <summary>
        /// Standard ToString() function to support internal debugging.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string itemFlags = string.Empty;

            Array values = Enum.GetValues(typeof(DirectoryItemFlags));

            foreach (DirectoryItemFlags val in values)
            {
                if ((ItemType & val) != 0)
                    itemFlags += (string.IsNullOrEmpty(itemFlags) ? val.ToString() : " | " + val.ToString());
            }

            return string.Format("Name: '{0}',PathRAW: '{1}', SpecialPathId: '{2}', PathFileSystem '{3}', Flags: '{4}'",
                Name, PathRAW, SpecialPathId, PathFileSystem, itemFlags);
        }

        /// <summary>
        /// Make sure that a path reference does actually work with
        /// <see cref="System.IO.DirectoryInfo"/> by replacing 'C:' by 'C:\'.
        /// </summary>
        /// <param name="dirOrfilePath"></param>
        /// <returns></returns>
        internal static string NormalizePath(string dirOrfilePath)
        {
            if (string.IsNullOrEmpty(dirOrfilePath) == true)
                return null;

            // The dirinfo constructor will not work with 'C:' but does work with 'C:\'
            if (dirOrfilePath.Length < 2)
                return null;

            if (dirOrfilePath.Length == 2)
            {
                if (dirOrfilePath[dirOrfilePath.Length - 1] == ':')
                    return dirOrfilePath + System.IO.Path.DirectorySeparatorChar;
            }

            if (dirOrfilePath.Length == 3)
            {
                if (dirOrfilePath[dirOrfilePath.Length - 2] == ':' &&
                    dirOrfilePath[dirOrfilePath.Length - 1] == System.IO.Path.DirectorySeparatorChar)
                    return dirOrfilePath;

                return "" + dirOrfilePath[0] + dirOrfilePath[1] +
                            System.IO.Path.DirectorySeparatorChar + dirOrfilePath[2];
            }

            // Insert a backslash in 3rd character position if not already present
            // C:Temp\myfile -> C:\Temp\myfile
            if (dirOrfilePath.Length >= 3)
            {
                if (char.ToUpper(dirOrfilePath[0]) >= 'A' && char.ToUpper(dirOrfilePath[0]) <= 'Z' &&
                    dirOrfilePath[1] == ':' &&
                    dirOrfilePath[2] != '\\')
                {
                    dirOrfilePath = dirOrfilePath.Substring(0, 2) + "\\" + dirOrfilePath.Substring(2);
                }
            }

            // This will normalize directory and drive references into 'C:' or 'C:\Temp'
            if (dirOrfilePath[dirOrfilePath.Length - 1] == System.IO.Path.DirectorySeparatorChar)
                dirOrfilePath = dirOrfilePath.Trim(System.IO.Path.DirectorySeparatorChar);

            return dirOrfilePath;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(IDirectoryBrowser other)
        {
            if (other == null)
                return false;

            if (string.Compare(this.SpecialPathId, other.SpecialPathId, true) != 0 ||
                string.Compare(this.PathFileSystem, other.PathFileSystem, true) != 0)
                return false;

            return true;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var otherbrowser = obj as IDirectoryBrowser;

            if (otherbrowser == null)
                return false;

            return Equals(otherbrowser);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            if (PathShell == null)
                return 0;

            return PathShell.GetHashCode();
        }

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
        public bool EqualsParseName(string parseName)
        {
            if (string.IsNullOrEmpty(parseName) == true)
                return false;

            bool KF_SpecialId = false;
            if (string.IsNullOrEmpty(SpecialPathId) == false)
            {
                KF_SpecialId = string.Compare(SpecialPathId, parseName, true) == 0;

                if (KF_SpecialId == false)
                {
                    // There are some corner cases where knownfolder special id is sometimes based on
                    // a CLSID (as in case of this PC) and the resulting knownfolder_id is different
                    // from the initial CLSID -> Lets try to cover this one here
                    if (string.Compare(PathRAW, parseName, true) == 0)
                        KF_SpecialId = true;
                }
            }

            if (KF_SpecialId == false)
            {
                if (string.IsNullOrEmpty(PathFileSystem) == false)
                    return string.Compare(PathFileSystem, parseName, true) == 0;
            }
            else
                return true;

            return false;
        }
        #endregion methods
    }
}
