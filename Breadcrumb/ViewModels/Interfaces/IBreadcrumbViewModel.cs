namespace Breadcrumb.ViewModels.Interfaces
{
    using Breadcrumb.ViewModels.Breadcrumbs;
    using System.ComponentModel;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an inteface to manage the complete Breadcrumb control with
    /// a viewmodel that should implement these properties and methods.
    /// </summary>
    internal interface IBreadcrumbViewModel : INotifyPropertyChanged
    {
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

        /// <summary>
        /// Returns a task that is used to initialize the control with
        /// an initial path and corresponding selected items.
        /// </summary>
        /// <param name="initialPath"></param>
        /// <returns></returns>
        Task InitPath(string initialPath);
    }
}
