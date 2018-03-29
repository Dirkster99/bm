using BreadcrumbLib.Interfaces;

namespace Breadcrumb.ViewModels.Interfaces
{
	public interface IBreadcrumbItemViewModel : IEntryViewModel ////, ISupportTreeSelector<IBreadcrumbItemViewModel, IEntryModel>
    {
        bool ShowCaption { get; set; }
        IEntriesHelper<IBreadcrumbItemViewModel> Entries { get; set; }
    }
}
