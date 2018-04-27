namespace BreadcrumbTestLib.ViewModels.TreeLookupProcessors
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbLib.Enums;

    /// <summary>
    /// Set Parent ViewModel's ITreeSelector.SelectedChild to Current ViewModel's Value.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class SetChildSelected<VM, T> : ITreeLookupProcessor<VM, T>
    {
        public bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
        {
            if (hr == HierarchicalResult.Child || hr == HierarchicalResult.Current)
                parentSelector.SelectedChild = selector.Value;

            return true;
        }
    }
}
