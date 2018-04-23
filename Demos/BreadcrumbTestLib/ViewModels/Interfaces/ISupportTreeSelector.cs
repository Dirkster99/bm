namespace BreadcrumbTestLib.ViewModels.Interfaces
{
    /// <summary>
    /// Implement by ViewModel that has Tree based structure and support LookupProcessing.
    /// </summary>
    /// <typeparam name="VM">Sub-node viewmodel type.</typeparam>
    /// <typeparam name="T">Type to identify a node, commonly string.</typeparam>
    public interface ISupportTreeSelector<VM, T> : ISupportEntriesHelper<VM>
    {
        ITreeSelector<VM, T> Selection { get; }
    }
}
