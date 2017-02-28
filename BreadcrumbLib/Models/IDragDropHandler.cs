namespace BreadcrumbLib.Models
{
    using BreadcrumbLib.Models.Helpers;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Owned by IProfile, for drag drop handling.
    /// </summary>
    public interface IDragDropHandler
    {
        Task<System.Windows.IDataObject> GetDataObject(IEnumerable<IEntryModel> entries);

        DragDropEffects QueryDrag(IEnumerable<IEntryModel> entries);

        void OnDragCompleted(IEnumerable<IEntryModel> entries, System.Windows.IDataObject da, DragDropEffects effect);

        IEnumerable<IEntryModel> GetEntryModels(System.Windows.IDataObject dataObject);

        bool QueryCanDrop(IEntryModel dest);
        QueryDropResult QueryDrop(IEnumerable<IEntryModel> entries, IEntryModel dest, DragDropEffects allowedEffects);
        DragDropEffects OnDropCompleted(IEnumerable<IEntryModel> entries, System.Windows.IDataObject da, IEntryModel dest, DragDropEffects allowedEffects);

    }
}
