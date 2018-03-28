namespace BreadcrumbLib.UIEventHub
{
    /// <summary>
    /// Indicate an item is draggable.
    /// </summary>
    public interface IDraggable : IUIAware
    {
        /// <summary>
        /// Whether the item is dragging, set by UIEventHub.
        /// </summary>
        bool IsDragging { get; set; }
    }
}
