namespace SuggestBoxTestLib.AutoSuggest
{
    using SuggestBoxTestLib.DataSources.Auto;
    using SuggestBoxTestLib.DataSources.Auto.Interfaces;

    /// <summary>
    /// Implements an object that keeps track of the root of the
    /// browsing hierarchy and provides helper methods towards browsing
    /// it (finding children for a given node etc).
    /// </summary>
    public class LocationIndicator
    {
        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public LocationIndicator(object rootItemParam, IHierarchyHelper hierarchyHelperParam)
        {
            this.RootItem = rootItemParam;
            this.HierarchyHelper = hierarchyHelperParam;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public LocationIndicator(object rootItemParam)
        {
            this.RootItem = rootItemParam;
            this.HierarchyHelper = new PathHierarchyHelper("Parent", "Value", "SubDirectories");
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected LocationIndicator()
        {
        }
        #endregion ctors

        /// <summary>
        /// Gets the root of the current browsing hierarchy.
        /// </summary>
        public object RootItem { get; }

        /// <summary>
        /// Gets an helper object to browse the current hierarchy.
        /// </summary>
        public IHierarchyHelper HierarchyHelper { get; }
    }
}
