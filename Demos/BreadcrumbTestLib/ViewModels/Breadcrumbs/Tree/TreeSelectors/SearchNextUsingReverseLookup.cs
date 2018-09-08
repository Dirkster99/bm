namespace BreadcrumbTestLib.ViewModels.TreeSelectors
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BmLib.Enums;

    /// <summary>
    /// Given a child selector (ITreeSelector), lookup only next level of tree nodes for the selector, and process only the child or matched node.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    internal class SearchNextUsingReverseLookup<VM, T> : ITreeLookup<VM, T>
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Stack<ITreeSelector<VM, T>> _hierarchy;
        private ITreeSelector<VM, T> _targetSelector;
        #endregion fields

        #region constructors
        public SearchNextUsingReverseLookup(ITreeSelector<VM, T> targetSelector)
        {
            _targetSelector = targetSelector;
            _hierarchy = new Stack<ITreeSelector<VM, T>>();

            var current = targetSelector;

            while (current != null)
            {
                _hierarchy.Push(current);
                current = current.ParentSelector;
            }
        }
        #endregion constructors

        #region methods
        /// <summary>
        /// Lookup an item from a multi-level tree based ViewModels, and run processors on the way.
        /// </summary>
        /// <param name="value">Value of the lookup node.</param>
        /// <param name="parentSelector">Where to start lookup.</param>
        /// <param name="comparer">Compare two value and return it's hierarchy.</param>
        /// <param name="cancelToken"></param>
        /// <param name="processors">Processors's Process() method is run whether it's parent, child, current or unrelated node of lookup node.</param>
        /// <returns></returns>
        public async Task LookupAsync(T value,
                                      ITreeSelector<VM, T> parentSelector,
                                      ICompareHierarchy<T> comparer,
                                      CancellationToken cancelToken,
                                      params ITreeLookupProcessor<VM, T>[] processors)
        {
            Logger.InfoFormat("_");

            await Task.FromResult(0);

            if (cancelToken != CancellationToken.None)
                cancelToken.ThrowIfCancellationRequested();

            if (parentSelector.EntryHelper.IsLoaded)
            {
                foreach (VM current in parentSelector.EntryHelper.All)
                {
                    if (cancelToken != CancellationToken.None)
                        cancelToken.ThrowIfCancellationRequested();

                    if (current is ISupportBreadcrumbTreeItemViewModel<VM, T> && current is ISupportBreadcrumbTreeItemHelperViewModel<VM>)
                    {
                        var currentSelectionHelper = (current as ISupportBreadcrumbTreeItemViewModel<VM, T>).Selection;
                        var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, value);

                        switch (compareResult)
                        {
                            case HierarchicalResult.Child:
                            case HierarchicalResult.Current:
                                if (this._hierarchy.Contains(currentSelectionHelper))
                                {
                                    Process(processors, compareResult, parentSelector, currentSelectionHelper);
                                    return;
                                }
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called by ITreeSelector.LookupAsync() to process returned HierarchicalResult from ITreeLookup.
        /// </summary>
        /// <param name="hr">Returned hierarchicalresult</param>
        /// <param name="parentSelector">Parent viewmodel's ITreeSelector.</param>
        /// <param name="selector">Current viewmodel's ITreeSelector.</param>
        /// <returns>Stop process and return false if any processor return false.</returns>
        public bool Process(ITreeLookupProcessor<VM, T>[] processors,
                            HierarchicalResult hr,
                            ITreeSelector<VM, T> parentSelector,
                            ITreeSelector<VM, T> selector)
        {
            foreach (var p in processors)
            {
                if (!p.Process(hr, parentSelector, selector))
                    return false;
            }

            return true;
        }
        #endregion methods
    }
}
