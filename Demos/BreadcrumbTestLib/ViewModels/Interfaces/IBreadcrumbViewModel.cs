namespace BreadcrumbTestLib.ViewModels.Interfaces
{
    using BmLib.Interfaces;
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.ViewModels.Breadcrumbs;
    using WSF.Interfaces;
    using SSCoreLib.Browse;
    using System.ComponentModel;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an inteface to manage the complete Breadcrumb control with
    /// a viewmodel that should implement these properties and methods.
    /// </summary>
    public interface IBreadcrumbViewModel : IRoot<IDirectoryBrowser>,
                                            INotifyPropertyChanged
    {
        #region properties
        /// <summary>
        /// Gets the root tree of the breadcrumb control tree
        /// </summary>
        BreadcrumbTreeItemViewModel BreadcrumbSubTree { get; }

        /// <summary>
        /// Gets/sets a property that determines whether a breadcrumb
        /// switch is turned on or off.
        /// 
        /// On false: A Breadcrumb switch turned off shows the text editable path
        ///  On true: A Breadcrumb switch turned  on shows the BreadcrumbSubTree for browsing
        /// </summary>
        bool EnableBreadcrumb { get; set; }

        /// <summary>
        /// Gets an interface to determine whether progress display should currenlty be visible or not.
        /// </summary>
        IProgress Progressing { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Method should be called after construction to initialize the viewmodel
        /// to view a default content.
        /// </summary>
        Task InitPathAsync();

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
        #endregion methods
    }
}
