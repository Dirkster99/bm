namespace BreadcrumbTestLib.Models
{
    using ShellBrowserLib;
    using ShellBrowserLib.Enums;
    using ShellBrowserLib.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    internal class LocationIndicator
    {
        #region fields
        private readonly List<IDirectoryBrowser> _CurrentPath;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public LocationIndicator(IDirectoryBrowser[] path)
            : this()
        {
            for (int i = 0; i < path.Length; i++)
            {
                _CurrentPath.Add(path[i].Clone() as IDirectoryBrowser);
            }
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public LocationIndicator()
        {
            _CurrentPath = new List<IDirectoryBrowser>();
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
        public IEnumerable<IDirectoryBrowser> Items
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
            if ((_CurrentPath[idxSrc].ItemType & DirectoryItemFlags.Desktop) != 0)
            {
                idxSrc = 1;
                size = size - 1;
            }

            IDirectoryBrowser[] ret = new IDirectoryBrowser[size];
            for (int i = 0; i < size; i++)
                ret[i] = _CurrentPath[idxSrc++].Clone() as IDirectoryBrowser;

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
        public string GetFileSystemPath()
        {
            string fileSystemPath = string.Empty;

            if (_CurrentPath.Count == 0)
                return string.Empty;

            // Skip showing the desktop in the string based path
            int i = 0;
            if ((_CurrentPath[i].ItemType & DirectoryItemFlags.Desktop) != 0)
                i = 1;

            if (i > (_CurrentPath.Count - 1))
                return string.Empty;

            var lastElement = _CurrentPath[_CurrentPath.Count - 1];
            var fspath = lastElement.PathFileSystem;

            if (ShellBrowser.IsTypeOf(fspath) == PathType.FileSystemPath)
                return fspath;

            return string.Empty;
        }

        /// <summary>
        /// Gets a path that contains either the real file system location
        /// or a location based on Named items along the current path (to avoid using SpecialPathIDs).
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public string GetWinShellPath()
        {
            string path = string.Empty;

            // Skip showing the desktop in the string based path
            int i = 0;
            if ((_CurrentPath[i].ItemType & DirectoryItemFlags.Desktop) != 0)
                i = 1;

            for (; i < _CurrentPath.Count; i++)
            {
                path = path + (path.Length > 0 ? "\\" + _CurrentPath[i].Name :
                                               _CurrentPath[i].Name);
            }

            return path;
        }

        /// <summary>
        /// Gets a Shell Path (sequence of localized item names) or a
        /// file system path and indicates the type of path with <paramref name="pathTypeParam"/>.
        /// </summary>
        /// <param name="pathTypeParam"></param>
        /// <returns></returns>
        public string GetPath(out PathType pathTypeParam)
        {
            pathTypeParam = PathType.Unknown;
            string path = string.Empty;

            path = GetFileSystemPath();
            pathTypeParam = PathType.FileSystemPath;

            if (string.IsNullOrEmpty(path))
            {
                path = GetWinShellPath();
                pathTypeParam = PathType.WinShellPath;
            }

            return path;
        }

        /// <summary>
        /// Compares the given path with the path in this object
        /// and indicates whether they match or not.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <returns></returns>
        public bool IsCurrentPath(string inputPath)
        {
            PathType pathType = PathType.Unknown;
            string path = GetPath(out pathType);

            return IsCurrentPath(inputPath, path);
        }

        /// <summary>
        /// Compares 2 to paths and indicates whether they match or not.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsCurrentPath(string inputPath, string path)
        {
            if (string.IsNullOrEmpty(inputPath) == true &&
                string.IsNullOrEmpty(path) == true)
                return true;

            // Remove ending backslash to normalize both strings for comparison
            int idx = inputPath.LastIndexOf('\\');
            if (idx == (inputPath.Length - 1))
                inputPath = inputPath.Substring(0, inputPath.Length - 1);

            if (string.Compare(path, inputPath, true) == 0)
                return true;

            return false;
        }

        /// <summary>
        /// Adds another <paramref name="item"/> as the last item in the current path.
        /// </summary>
        /// <param name="item"></param>
        public void Add(IDirectoryBrowser item)
        {
            _CurrentPath.Add(item.Clone() as IDirectoryBrowser);
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
        public IDirectoryBrowser Peek()
        {
            return _CurrentPath.Last();
        }

        /// <summary>
        /// Pops the last element in the list and returns it or
        /// returns null if list is empty.
        /// </summary>
        /// <returns></returns>
        public IDirectoryBrowser Pop()
        {
            return Pop<IDirectoryBrowser>(_CurrentPath);
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
