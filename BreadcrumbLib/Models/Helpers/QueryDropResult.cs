namespace BreadcrumbLib.Models.Helpers
{
    using BreadcrumbLib.UIEventHub;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    public class QueryDropResult
    {
        public QueryDropResult(DragDropEffects supportedEffects, DragDropEffects preferredEffects)
        {
            SupportedEffects = supportedEffects;
            PreferredEffect = preferredEffects;
        }

        public static QueryDropResult None = new QueryDropResult(DragDropEffects.None, DragDropEffects.None);

        public static QueryDropResult CreateNew(DragDropEffects supportedEffects, DragDropEffects preferredEffects)
        {
            return new QueryDropResult(supportedEffects, preferredEffects);
        }

        public static QueryDropResult CreateNew(DragDropEffects supportedEffects)
        {
            return new QueryDropResult(supportedEffects, supportedEffects);
        }


        public DragDropEffects SupportedEffects { get; set; }
        public DragDropEffects PreferredEffect { get; set; }
    }

    public interface ISupportDropHelper
    {
        ISupportDrop DropHelper { get; }
    }

    public interface ISupportDrop
    {
        bool IsDraggingOver { set; }
        bool IsDroppable { get; }
        string DropTargetLabel { get; }
        QueryDropResult QueryDrop(IDataObject da, DragDropEffects allowedEffects);
        IEnumerable<IDraggable> QueryDropDraggables(IDataObject da);
        DragDropEffects Drop(IEnumerable<IDraggable> draggables, IDataObject da, DragDropEffects allowedEffects);
    }

    public class NullSupportDrop : ISupportDrop
    {
        public static ISupportDrop Instance = new NullSupportDrop();
        public bool IsDraggingOver { set { } }
        public bool IsDroppable { get { return false; } }
        public string DropTargetLabel { get { return null; } }
        public QueryDropResult QueryDrop(IDataObject da, DragDropEffects allowedEffects)
        {
            return QueryDropResult.None;
        }

        public IEnumerable<IDraggable> QueryDropDraggables(IDataObject da)
        {
            return new List<IDraggable>();
        }

        public DragDropEffects Drop(IEnumerable<IDraggable> draggables, IDataObject da, DragDropEffects allowedEffects)
        {
            return DragDropEffects.None;
        }
    }
}
