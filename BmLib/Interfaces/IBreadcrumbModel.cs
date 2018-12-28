namespace BmLib.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the core methods that are called when switching the SuggestBox on and off
    /// which requires synchronization and path re-mounting between TreeView and SuggestBox.
    /// </summary>
    public interface IBreadcrumbModel
    {
        /// <summary>
        /// This navigates the bound tree view model to the requested
        /// location when the user switches the display from:
        /// 
        /// - the string based and path oriented suggestbox back to
        /// - the tree view item based and path orient tree view.
        /// </summary>
        /// <param name="navigateToThisLocation"></param>
        /// <returns></returns>
        Task<bool> NavigateTreeViewModel(string navigateToThisLocation,
                                         bool goBackToPreviousLocation);

        /// <summary>
        /// Updates the bound text path property of the SuggestBox with the path of the
        /// currently selected item. This method should be called whenever the SuggestBox
        /// is switched from invisible to visible.
        /// </summary>
        /// <param name="locations"></param>
        /// <returns></returns>
        string UpdateSuggestPath(out object locations);
    }
}
