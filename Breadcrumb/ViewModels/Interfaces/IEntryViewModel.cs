namespace Breadcrumb.ViewModels.Interfaces
{
    using BreadcrumbLib.Models;
    using System.Windows.Media;

    public interface IEntryViewModel ////: INotifyPropertyChangedEx, IDraggable, ISelectable, IUIAware, IViewModelOf<IEntryModel>
    {
        IEntryModel EntryModel { get; }

        bool IsRenamable { get; set; }
        bool IsRenaming { get; set; }

        ImageSource Icon { get; }

        IEntryViewModel Clone();
    }
}
