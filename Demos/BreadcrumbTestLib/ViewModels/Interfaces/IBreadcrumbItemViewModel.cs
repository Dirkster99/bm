namespace BreadcrumbTestLib.ViewModels.Interfaces
{
    using BmLib.Interfaces;

    public interface IBreadcrumbItemViewModel : IEntryViewModel ////, ISupportTreeSelector<IBreadcrumbItemViewModel, IEntryModel>
    {
        bool ShowCaption { get; set; }
        IEntriesHelper<IBreadcrumbItemViewModel> Entries { get; set; }
    }
}
