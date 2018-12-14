namespace BmLib.Interfaces
{
    using System.Threading.Tasks;

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
    }
}
