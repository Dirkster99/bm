namespace BreadcrumbTestLib.Models
{
    using BreadcrumbTestLib.ViewModels.Breadcrumbs;
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
        /// Schedules a navigational task and returns immediately
        /// </summary>
        /// <param name="selectedFolder"></param>
        /// <param name="sourceHint"></param>
        /// <param name="hintDirection"></param>
        /// <param name="toBeSelectedLocation"></param>
        Task NavigateToScheduledAsync(BreadcrumbTreeItemViewModel selectedFolder,
                                      string sourceHint,
                                      HintDirection hintDirection = HintDirection.Unrelated,
                                      BreadcrumbTreeItemViewModel toBeSelectedLocation = null
                                );
    }
}
