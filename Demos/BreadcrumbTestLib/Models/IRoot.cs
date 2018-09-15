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
    internal interface IRoot<M>
    {
        /// <summary>
        /// Navigates the viewmodel (and hopefully the bound control) to a new location
        /// and ensures correct <see cref="IsBrowsing"/> state and event handling towards
        /// listing objects for
        /// <see cref="ICanNavigate"/> events.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        Task<BrowseResult> NavigateToAsync(M location);
    }
}
