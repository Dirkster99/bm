namespace BreadcrumbTestLib.Models
{
    using BreadcrumbTestLib.ViewModels.Breadcrumbs;
    using ShellBrowserLib.Interfaces;
    using SSCoreLib.Browse;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a set of methods and properties to be implemented by the root viewmodel
    /// of a Breadcrumb Tree Viewmodel structure.
    ///
    /// These properties and methods can be used within the beardcrumb tree
    /// to initialize the execution of (navigational) methods that influence
    /// the tree structure as a whole and ensure correct state for the whole tree.
    /// </summary>
    public interface IRoot<M> : ICanNavigate
    {
        /// <summary>
        /// Navigates the viewmodel (and hopefully the bound control) to a new location
        /// and ensures correct <see cref="IsBrowsing"/> state and event handling towards
        /// listing objects for
        /// <see cref="ICanNavigate"/> events.
        /// </summary>
        /// <param name="requestedLocation"></param>
        /// <param name="direction">Specifies whether the navigation direction
        /// is not specified or up or down relative to the current path or
        /// ihintLevel parameter</param>
        /// <param name="ihintLevel">This parameter is relevant for Down direction only.
        /// It specifies the level in the tree structure from which the next child
        /// in the current path should be searched.</param>
        /// <returns>Returns a result that informs whether the target was reached or not.</returns>
        Task<FinalBrowseResult<IDirectoryBrowser>> NavigateToAsync(
            BrowseRequest<IDirectoryBrowser> requestedLocation,
            string sourceHint,
            HintDirection direction = HintDirection.Unrelated,
            BreadcrumbTreeItemViewModel toBeSelectedLocation = null
            );
    }
}
