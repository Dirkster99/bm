namespace BreadcrumbTestLib.Models
{
    using BreadcrumbTestLib.ViewModels.Breadcrumbs;
    using DirectoryInfoExLib.Interfaces;
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
        /// <param name="direction"></param>
        /// <returns></returns>
        Task<FinalBrowseResult<M>> NavigateToAsync(BrowseRequest<M> requestedLocation,
                                                   HintDirection direction = HintDirection.Unrelated);
    }
}
