namespace BreadcrumbTestLib.ViewModels.TreeLookupProcessors
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbLib.Enums;

    /// <summary>
    /// Refresh current directory using EntryHelper.LoadAsync()
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class RefreshDirectory<VM, T> : ITreeLookupProcessor<VM, T>
    {
        /// <summary>
        /// Refresh current directory using EntryHelper.LoadAsync() if is matched current directory of current lookup.
        /// </summary>
        public static RefreshDirectory<VM, T> WhenFound = new RefreshDirectory<VM, T>(HierarchicalResult.Current);

        private HierarchicalResult _hr;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="hr"></param>
        public RefreshDirectory(HierarchicalResult hr)
        {
            this._hr = hr;
        }

        public bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
        {
            if (this._hr.HasFlag(hr))
                selector.EntryHelper.LoadAsync(UpdateMode.Replace, true);

            return true;
        }
    }
}
