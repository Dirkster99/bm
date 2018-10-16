namespace BmLib.Interfaces
{
    using BmLib.Models;

    public interface IEntryViewModel ////: INotifyPropertyChangedEx, IDraggable, ISelectable, IUIAware, IViewModelOf<IEntryModel>
    {
        IEntryModel EntryModel { get; }

        bool IsRenamable { get; set; }
        bool IsRenaming { get; set; }

        IEntryViewModel Clone();
    }
}
