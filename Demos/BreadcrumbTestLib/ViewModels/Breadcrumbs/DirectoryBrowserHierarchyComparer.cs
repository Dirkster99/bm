namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.SystemIO;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using ShellBrowserLib.Interfaces;
    using BmLib.Enums;
    using System;
    using ShellBrowserLib;

    /// <summary>
    /// Implements a <see cref="IDirectoryBrowser"/> based <seealso ref="ICompareHierarchy"> object
    /// to compute the relationship of two <see cref="IDirectoryBrowser"/> based paths to each other.
    /// </summary>
    public class DirectoryBrowserHierarchyComparer : ICompareHierarchy<IDirectoryBrowser>
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private PathComparer _pathComparer = new PathComparer();
        #endregion fields

        #region constructors
        /// <summary>
        /// Class cosntructor
        /// </summary>
        public DirectoryBrowserHierarchyComparer()
        {
        }
        #endregion constructors

        #region methods
        /// <summary>
        /// Method is invoked to compute the relationship of 2 items in a tree.
        /// (Parent, Current, Child <see cref="HierarchicalResult"/> enumeration)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public HierarchicalResult CompareHierarchy(IDirectoryBrowser a, IDirectoryBrowser b)
        {
            HierarchicalResult retVal = this.CompareHierarchyInner(a, b);

            Logger.InfoFormat("IDirectoryBrowser a '{0}' IDirectoryBrowser b '{1}' retVal {2}",
                (a != null ? a.FullName : "(null)"), (b != null ? b.FullName : "(null)"), retVal);

            return retVal;
        }

        private HierarchicalResult CompareHierarchyInner(IDirectoryBrowser a, IDirectoryBrowser b)
        {
            if (a == null || b == null)
                return HierarchicalResult.Unrelated;

            if (string.IsNullOrEmpty(a.PathFileSystem) == false &&
                string.IsNullOrEmpty(b.PathFileSystem) == false)
                return this._pathComparer.CompareHierarchy(a.PathFileSystem, b.PathFileSystem);

            if (a.Equals(b))
                return HierarchicalResult.Current;  // a and b refer to the same location

            //            string key = string.Format("{0}-compare-{1}", a.FullName, b.FullName);

////            if (ShellBrowser.HasParent(b, a.FullName) != ShellBrowser.HasParent(b, a))
////            {
////                var test = ShellBrowser.HasParent(b, a.FullName);
////                ShellBrowser.HasParent(b, a);
////            }

            if (ShellBrowser.HasParent(b, a.FullName) == true)
            {
                return HierarchicalResult.Child;
            }
            else
            {
////                if (ShellBrowser.HasParent(a, b.FullName) != ShellBrowser.HasParent(a, b))
////                {
////                    var test = ShellBrowser.HasParent(a, b.FullName);
////                    ShellBrowser.HasParent(a, b);
////                }

                if (ShellBrowser.HasParent(a, b.FullName) == true)
                {
                    return HierarchicalResult.Parent;
                }
                else
                {
                    if (a.FullName.Contains("::") == false || b.FullName.Contains("::") == false)
                    {
                        Console.WriteLine("Comparing to Unrelated: {0} - {1}", a.Name, b.Name);
                    }

                    return HierarchicalResult.Unrelated;
                }
            }
        }
        #endregion methods
    }
}
