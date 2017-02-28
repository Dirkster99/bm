namespace BreadcrumbLib.UIEventHub
{
    /// <summary>
    /// Indicate the item is a view model participate in UIEventHub's processing.
    /// </summary>
    public interface IUIAware
    {
        string DisplayName { get; }
    }
}
