namespace BreadcrumbTestLib.ViewModels.Interfaces
{
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.ViewModels.Breadcrumbs;
    using DirectoryInfoExLib.Interfaces;
    using System.ComponentModel;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an inteface to manage the complete Breadcrumb control with
    /// a viewmodel that should implement these properties and methods.
    /// </summary>
    internal interface IBreadcrumbViewModel : INotifyPropertyChanged
    {
        #region properties
        /// <summary>
        /// Gets the root tree of the breadcrumb control tree
        /// </summary>
        ExTreeNodeViewModel BreadcrumbSubTree { get; }

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
        IProgressViewModel Progressing { get; }

        string SuggestedPath { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Method should be called after construction to initialize the viewmodel
        /// to view a default content.
        /// </summary>
        /// <param name="initialRequest"></param>
        Task<FinalBrowseResult<IDirectoryInfoEx>> InitPathAsync(BrowseRequest<string> initialRequest);
        #endregion methods
    }
}
