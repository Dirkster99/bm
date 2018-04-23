namespace BreadcrumbTestLib.ViewModels.TreeSelectors
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbLib.Defines;

    /// <summary>
    /// Given a child selector (ITreeSelector), lookup only next level of tree nodes for the selector, and process only the child or matched node.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class SearchNextUsingReverseLookup<VM, T> : ITreeLookup<VM, T>
    {
        #region fields
        private Stack<ITreeSelector<VM, T>> _hierarchy;
        private ITreeSelector<VM, T> _targetSelector;
        #endregion fields

        #region constructors
        public SearchNextUsingReverseLookup(ITreeSelector<VM, T> targetSelector)
        {
            this._targetSelector = targetSelector;
            this._hierarchy = new Stack<ITreeSelector<VM, T>>();

            var current = targetSelector;

            while (current != null)
            {
                this._hierarchy.Push(current);
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
            await Task.FromResult(0);

            if (cancelToken != CancellationToken.None)
                cancelToken.ThrowIfCancellationRequested();

            if (parentSelector.EntryHelper.IsLoaded)
            {
                foreach (VM current in parentSelector.EntryHelper.AllNonBindable)
                {
                    if (cancelToken != CancellationToken.None)
                        cancelToken.ThrowIfCancellationRequested();

                    if (current is ISupportTreeSelector<VM, T> && current is ISupportEntriesHelper<VM>)
                    {
                        var currentSelectionHelper = (current as ISupportTreeSelector<VM, T>).Selection;
                        var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, value);

                        switch (compareResult)
                        {
                            case HierarchicalResult.Child:
                            case HierarchicalResult.Current:
                                if (this._hierarchy.Contains(currentSelectionHelper))
                                {
                                    processors.Process(compareResult, parentSelector, currentSelectionHelper);
                                    return;
                                }
                                break;
                        }
                    }
                }
            }
        }
        #endregion methods
    }
}
