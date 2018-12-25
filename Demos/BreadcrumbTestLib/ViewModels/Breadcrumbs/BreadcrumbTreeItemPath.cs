namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using ShellBrowserLib;
    using ShellBrowserLib.Enums;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class models a current path by keeping track of the involved
    /// viewmodel item objects that are also viaible in the tree view.
    /// </summary>
    internal class BreadcrumbTreeItemPath : Base.ViewModelBase
    {
        #region fields
        private readonly List<BreadcrumbTreeItemViewModel> _CurrentPath;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public BreadcrumbTreeItemPath()
        {
            _CurrentPath = new List<BreadcrumbTreeItemViewModel>();
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets all items currently present in this list of path items.
        /// </summary>
        public IEnumerable<BreadcrumbTreeItemViewModel> Items
        {
            get
            {
                return _CurrentPath;
            }
        }

        /// <summary>
        /// Gets the length of the current path.
        /// </summary>
        public int Count
        {
            get
            {
                return _CurrentPath.Count;
            }
        }

        /// <summary>
        /// Gets the Windows Shell Path which is the literal sequence of
        /// the items names that make up a path
        /// (minus Desktop since its always given as root):
        /// 'Libraries\Music'
        /// </summary>
        public string WinShellPath
        {
            get
            {
                if (_CurrentPath.Count == 0)
                    return string.Empty;

                var winShellPath = GetWinShellPath();

                return winShellPath;
            }
        }

        /// <summary>
        /// Gets the current path as
        /// 1) a filesystem path ('X:\Data\'), if it exists, or
        /// 2) An empty string, if the current path cannot directly
        ///    be mapped into the filesystem (eg: 'Libraries/Music').
        /// </summary>
        public string FileSystemPath
        {
            get
            {
                if (_CurrentPath.Count == 0)
                    return string.Empty;

                return GetFileSystemPath();
            }
        }

        public string RootFileSystemPath
        {
            get
            {
                if (_CurrentPath.Count == 0)
                    return string.Empty;

                return GetRootFileSystemPath();
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets a path that contains either the real file system location
        /// or a location based on Named items along the current path (to avoid using SpecialPathIDs).
        /// </summary>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        public string GetWinShellPath()
        {
            string path = string.Empty;

            // Skip showing the desktop in the string based path
            int i = 0;
            if ((_CurrentPath[i].GetModel().ItemType & DirectoryItemFlags.Desktop) != 0)
                i = 1;

            for (; i < _CurrentPath.Count; i++)
            {
                path = path + (path.Length > 0 ? "\\" + _CurrentPath[i].ItemName :
                                               _CurrentPath[i].ItemName);
            }

            return path;
        }

        /// <summary>
        /// Analyses the given path and returns:
        /// 1) a filesystem path ('X:\Data\'), if it exists, or
        /// 2) An empty string, if the current path cannot directly be mapped into the filesystem
        ///    (eg: 'Libraries/Music').
        /// </summary>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        public string GetFileSystemPath()
        {
            string fileSystemPath = string.Empty;

            if (_CurrentPath.Count == 0)
                return string.Empty;

            // Skip showing the desktop in the string based path
            int i = 0;
            if ((_CurrentPath[i].GetModel().ItemType & DirectoryItemFlags.Desktop) != 0)
                i = 1;

            if (i > (_CurrentPath.Count - 1))
                return string.Empty;

            var lastElement = _CurrentPath[_CurrentPath.Count - 1];
            var fspath = lastElement.GetModel().PathFileSystem;

            if (ShellBrowser.IsTypeOf(fspath) == PathType.FileSystemPath)
                return fspath;

            return string.Empty;
        }

        public string GetRootFileSystemPath()
        {
            string fileSystemPath = string.Empty;

            if (_CurrentPath.Count == 0)
                return string.Empty;

////            // Skip showing the desktop in the string based path
////            int i = 0;
////            if ((_CurrentPath[i].GetModel().ItemType & DirectoryItemFlags.Desktop) != 0)
////                i = 1;
////
////            if (i > (_CurrentPath.Count - 1))
////                return string.Empty;

            int lastIdx = _CurrentPath.Count - 1;
            var lastElement = _CurrentPath[lastIdx];
            var fspath = lastElement.GetModel().PathFileSystem;

            if (ShellBrowser.IsTypeOf(fspath) != PathType.FileSystemPath)
                return string.Empty;

            for (int idx = _CurrentPath.Count - 2; idx >= 0; idx--)
            {
                var thisElement = _CurrentPath[idx];
                var thisFsPath = thisElement.GetModel().PathFileSystem;

                if (string.IsNullOrEmpty(thisFsPath))
                    return fspath;

                if (ShellBrowser.IsParentPathOf(thisFsPath, fspath))
                {
                    lastIdx = idx;
                    lastElement = _CurrentPath[lastIdx];
                    fspath = thisFsPath;
                }
            }

            return fspath;
        }

        /// <summary>
        /// Pops the last element in the list and returns it or
        /// returns null if list is empty.
        /// </summary>
        /// <returns></returns>
        public BreadcrumbTreeItemViewModel Pop()
        {
            return Pop<BreadcrumbTreeItemViewModel>(_CurrentPath);
        }

        /// <summary>
        /// Peeks the last element in the list and returns it or
        /// raises an <see cref="System.InvalidOperationException"/> if the list is empty.
        /// </summary>
        /// <returns></returns>
        public BreadcrumbTreeItemViewModel Peek()
        {
            return _CurrentPath.Last();
        }

        /// <summary>
        /// Adds another <paramref name="item"/> as the last item in the current path.
        /// </summary>
        /// <param name="item"></param>
        public void Add(BreadcrumbTreeItemViewModel item)
        {
            _CurrentPath.Add(item);
        }

        /// <summary>
        /// Removes all elements from the current path collection.
        /// </summary>
        public void Clear()
        {
            _CurrentPath.Clear();
        }

        /// <summary>
        /// Pops the last element from a list and returns it if it exists,
        /// or returns null if the list was emtpy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public T Pop<T>(IList<T> list)
        {
            if (list.Any() == false)
                return default(T);

            // first assign the  last value to a seperate string 
            T extractedElement = list.Last();

            // then remove it from list
            list.RemoveAt(list.Count - 1);

            // then return the value 
            return extractedElement;
        }
        #endregion methods
    }
}
