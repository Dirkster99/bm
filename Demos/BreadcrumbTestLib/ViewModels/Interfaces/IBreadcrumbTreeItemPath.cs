namespace BreadcrumbTestLib.ViewModels.Interfaces
{
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.ViewModels.Breadcrumbs;
    using WSF.Enums;
    using WSF.Interfaces;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a current path by keeping track of the involved
    /// properties and objects that are also visible in the tree view.
    /// </summary>
    internal interface IBreadcrumbTreeItemPath
    {
        #region properties
        /// <summary>
        /// Gets the length of the current path.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets all items currently present in this list of path items.
        /// </summary>
        IEnumerable<BreadcrumbTreeItemViewModel> Items { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets an object that describes a current path with path model items.
        /// </summary>
        /// <returns></returns>
        LocationIndicator GetLocation();

        /// <summary>
        /// Gets an array of <see cref="IDirectoryBrowser"/> model objects
        /// that describe the current path managed in this object.
        /// </summary>
        /// <returns></returns>
        IDirectoryBrowser[] GetPathModels();

        /// <summary>
        /// Analyses the given path and returns:
        /// 1) a filesystem path ('X:\Data\'), if it exists, or
        /// 2) An empty string, if the current path cannot directly be mapped into the filesystem
        ///    (eg: 'Libraries/Music').
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        string GetFileSystemPath();

        /// <summary>
        /// Gets a path that contains either the real file system location
        /// or a location based on Named items along the current path (to avoid using SpecialPathIDs).
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        string GetWinShellPath();

        /// <summary>
        /// Gets a Shell Path (sequence of localized item names) or a
        /// file system path and indicates the type of path with <paramref name="pathTypeParam"/>.
        /// </summary>
        /// <param name="pathTypeParam"></param>
        /// <returns></returns>
        string GetPath(out PathType pathTypeParam);

        /// <summary>
        /// Adds another <paramref name="item"/> as the last item in the current path.
        /// </summary>
        /// <param name="item"></param>
        void Add(BreadcrumbTreeItemViewModel item);

        /// <summary>
        /// Removes all elements from the current path collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Peeks the last element in the list and returns it or
        /// raises an <see cref="System.InvalidOperationException"/> if the list is empty.
        /// </summary>
        /// <returns></returns>
        BreadcrumbTreeItemViewModel Peek();

        /// <summary>
        /// Pops the last element in the list and returns it or
        /// returns null if list is empty.
        /// </summary>
        /// <returns></returns>
        BreadcrumbTreeItemViewModel Pop();

        /// <summary>
        /// Pops the last element from a list and returns it if it exists,
        /// or returns null if the list was emtpy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        T Pop<T>(IList<T> list);
        #endregion methods
    }
}