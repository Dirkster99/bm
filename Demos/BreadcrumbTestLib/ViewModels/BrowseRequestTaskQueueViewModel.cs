namespace BreadcrumbTestLib.ViewModels
{
    using BmLib.Interfaces;
    using SSCoreLib.ViewModels;

    /// <summary>
    /// This viewmodel extends the <see cref="TaskQueueViewModel"/> and ensures that all
    /// required properties are available for bind against the Breadcrumb control by
    /// also inheriting the <see cref="IBrowseRequestTaskQueue"/> interface.
    /// 
    /// Ideally, alle required properties in <see cref="IBrowseRequestTaskQueue"/> are
    /// allready implemented in <see cref="TaskQueueViewModel"/> - this inheratence step
    /// makes sure the required properties are implemented as required.
    /// </summary>
    public class BrowseRequestTaskQueueViewModel : TaskQueueViewModel, IBrowseRequestTaskQueue
    {
    }
}
