namespace BreadcrumbTestLib.ViewModels.TreeLookupProcessors
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BmLib.Enums;

    /// <summary>
    /// Set ViewModel's EntryHelper.IsExpanded to true.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class LoadSubEntries<VM, T> : ITreeLookupProcessor<VM, T>
    {
        /// <summary>
        /// Set ViewModel's EntryHelper.IsExpanded to true when it's unrelated node of current lookup.
        /// </summary>
////        public static LoadSubEntries<VM, T> WhenSelected(UpdateMode updateMode = UpdateMode.Replace, bool force = false,
////            object parameter = null)
////        { return new LoadSubEntries<VM, T>(HierarchicalResult.Current, updateMode, force, parameter); }

        private UpdateMode _updateMode;
        private bool _force;
        private object _parameter;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="matchResult"></param>
        public LoadSubEntries(HierarchicalResult matchResult, UpdateMode updateMode = UpdateMode.Replace,
                             bool force = false,
                             object parameter = null)
        {
            this.MatchResult = matchResult;
            _updateMode = updateMode;
            _force = force;
            _parameter = parameter;
        }

        private HierarchicalResult MatchResult { get; set; }

        public bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
        {
            if (this.MatchResult.HasFlag(hr))
                selector.EntryHelper.LoadAsync(_updateMode, _force, _parameter);

            // To-Do: Add a interface for IsBringIntoView.
            ////if (hr == FileExplorer.Defines.HierarchicalResult.Current)
            ////    (selector.ViewModel as IDirectoryNodeViewModel).IsBringIntoView = true;
            return true;
        }
    }
}
