namespace BreadcrumbTestLib.SystemIO
{
    using System;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbLib.Defines;

    /// <summary>
    /// Implements a string based <seealso ref="ICompareHierarchy"> object
    /// to compute the relationship of two string based paths to each other.
    /// </summary>
    public class PathComparer : ICompareHierarchy<string>
    {
        #region fields
        private StringComparison _stringComparsion = StringComparison.InvariantCultureIgnoreCase;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="comparsion"></param>
        public PathComparer(StringComparison comparsion = StringComparison.CurrentCultureIgnoreCase)
        {
            _stringComparsion = comparsion;
        }
        #endregion constructors

        #region Methods
        /// <summary>
        /// Method determines a <seealso cref="HierarchicalResult"> between
        /// <param ref="path1"> and <param ref="path2"> and indicates the relation-ship
        /// through its return value.
        /// </summary>
        public HierarchicalResult CompareHierarchy(string path1, string path2)
        {
            if (path1 == null || path2 == null)
                return HierarchicalResult.Unrelated;

            if (path1.Equals(path2, this._stringComparsion))
                return HierarchicalResult.Current;

            if (!path1.EndsWith("\\"))
                path1 += "\\";

            if (!path2.EndsWith("\\"))
                path2 += "\\";

            if (path1.StartsWith(path2, this._stringComparsion))
                return HierarchicalResult.Parent;

            if (path2.StartsWith(path1, this._stringComparsion))
                return HierarchicalResult.Child;

            return HierarchicalResult.Unrelated;
        }
        #endregion
    }
}
