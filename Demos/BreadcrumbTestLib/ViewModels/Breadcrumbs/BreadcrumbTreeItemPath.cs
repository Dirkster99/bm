namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using ShellBrowserLib;
    using ShellBrowserLib.Enums;
    using ShellBrowserLib.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class models a current path by keeping track of the involved
    /// viewmodel item objects that are also visible in selected path
    /// of the tree view portion in the Breadcrumb control.
    /// </summary>
    internal class BreadcrumbTreeItemPath : IBreadcrumbTreeItemPath
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
        /// Gets all items currently present in this list of path items.
        /// </summary>
        public IEnumerable<BreadcrumbTreeItemViewModel> Items
        {
            get
            {
                return _CurrentPath;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets an array of <see cref="IDirectoryBrowser"/> model objects
        /// that describe the current path managed in this object.
        /// </summary>
        /// <returns></returns>
        public IDirectoryBrowser[] GetPathModels()
        {
            // Skip showing the desktop in the string based path
            int idxSrc = 0, size = _CurrentPath.Count;
            if ((_CurrentPath[idxSrc].GetModel().ItemType & DirectoryItemFlags.Desktop) != 0)
            {
                idxSrc = 1;
                size = size - 1;
            }

            IDirectoryBrowser[] ret = new IDirectoryBrowser[size];
            for (int i = 0; i < size; i++)
                ret[i] = _CurrentPath[idxSrc++].GetModel().Clone() as IDirectoryBrowser;

            return ret;
        }

        /// <summary>
        /// Analyses the given path and returns:
        /// 1) a filesystem path ('X:\Data\'), if it exists, or
        /// 2) An empty string, if the current path cannot directly be mapped into the filesystem
        ///    (eg: 'Libraries/Music').
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public string GetFileSystemPath(out IDirectoryBrowser location)
        {
            location = null;
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
            {
                location = lastElement.GetModel().Clone() as IDirectoryBrowser;
                return fspath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets a path that contains either the real file system location
        /// or a location based on Named items along the current path (to avoid using SpecialPathIDs).
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public string GetWinShellPath(out IDirectoryBrowser location)
        {
            location = null;
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

            location = _CurrentPath[_CurrentPath.Count - 1].GetModel().Clone() as IDirectoryBrowser;

            return path;
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
        /// Peeks the last element in the list and returns it or
        /// raises an <see cref="System.InvalidOperationException"/> if the list is empty.
        /// </summary>
        /// <returns></returns>
        public BreadcrumbTreeItemViewModel Peek()
        {
            return _CurrentPath.Last();
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
