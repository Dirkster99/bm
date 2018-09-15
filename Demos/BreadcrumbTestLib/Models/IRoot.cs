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
        Task<BrowseResult> NavigateTo(M location);

        /// <summary>
        /// Method is executed to  change the navigation target of the currently
        /// selected location towards a new location. This method is typically
        /// executed when:
        /// 1) Any other than the root drop down triangle is opened,
        /// 2) An entry in the list drop down is selected and
        /// 3) The control is now deactivating its previous selection and
        /// 4) needs to navigate towards the new selected item.
        /// 
        /// Expected command parameter:
        /// Array of length 1 with an object of type <see cref="BreadcrumbTreeItemViewModel"/>
        /// object[1] = {new <see cref="BreadcrumbTreeItemViewModel"/>() }
        /// </summary>
        /// <param name="item">Is the tree item that represents the target location in the tree structure.</param>
        void NavigateToChild(BreadcrumbTreeItemViewModel item,
                             BreadcrumbTreeItemViewModel selectedLocationModel);
    }
}
