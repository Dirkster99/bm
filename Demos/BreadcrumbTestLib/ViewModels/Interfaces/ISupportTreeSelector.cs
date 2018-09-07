namespace BreadcrumbTestLib.ViewModels.Interfaces
{
    using BmLib.Interfaces;

    /// <summary>
    /// Implement by ViewModel that has Tree based structure and support LookupProcessing.
    /// </summary>
    /// <typeparam name="VM">Sub-node viewmodel type.</typeparam>
    /// <typeparam name="T">Type to identify a node, commonly string.</typeparam>
    public interface ISupportBreadcrumbTreeItemViewModel<VM, T> : ISupportBreadcrumbTreeItemHelperViewModel<VM>,
                                                                  IOverflown
    {
        /// <summary>
        /// Tree based structure and LookupProcessing support.
        /// </summary>
        ITreeSelector<VM, T> Selection { get; }
    }


    /// <summary>
    /// Defines an interface to an object that contains an
    /// <see cref="IBreadcrumbTreeItemHelperViewModel{VM}"/> Entries
    /// get property.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    public interface ISupportBreadcrumbTreeItemHelperViewModel<VM>
    {
        IBreadcrumbTreeItemHelperViewModel<VM> Entries { get; }
    }
}
