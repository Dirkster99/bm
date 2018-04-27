namespace BreadcrumbTestLib.ViewModels.TreeLookupProcessors
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbLib.Enums;

    /// <summary>
    /// Set ViewModel's EntryHelper.IsExpanded to false.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class SetCollapsed<VM, T> : ITreeLookupProcessor<VM, T>
    {
        /// <summary>
        /// Set ViewModel's EntryHelper.IsExpanded to false if it's child of current lookup.
        /// </summary>
        public static SetCollapsed<VM, T> WhenChildSelected = new SetCollapsed<VM, T>(HierarchicalResult.Child);

        /// <summary>
        /// Set ViewModel's EntryHelper.IsExpanded to false when it's unrelated node of current lookup.
        /// </summary>
        public static SetCollapsed<VM, T> WhenNotRelated = new SetCollapsed<VM, T>(HierarchicalResult.Unrelated);

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="matchResult"></param>
        public SetCollapsed(HierarchicalResult matchResult)
        {
            this.MatchResult = matchResult;
        }

        private HierarchicalResult MatchResult { get; set; }

        public bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
        {
            if (this.MatchResult.HasFlag(hr))
                selector.EntryHelper.IsExpanded = false;

            return true;
        }
    }
}
