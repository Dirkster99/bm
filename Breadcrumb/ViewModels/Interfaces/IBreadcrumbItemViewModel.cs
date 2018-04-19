namespace Breadcrumb.ViewModels.Interfaces
{
    using BreadcrumbLib.Interfaces;

    public interface IBreadcrumbItemViewModel : IEntryViewModel ////, ISupportTreeSelector<IBreadcrumbItemViewModel, IEntryModel>
    {
        bool ShowCaption { get; set; }
        IEntriesHelper<IBreadcrumbItemViewModel> Entries { get; set; }
    }
}
