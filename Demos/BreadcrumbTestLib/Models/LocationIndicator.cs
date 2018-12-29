﻿namespace BreadcrumbTestLib.Models
{
    using ShellBrowserLib;
    using ShellBrowserLib.Enums;
    using ShellBrowserLib.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class LocationIndicator
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