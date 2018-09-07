namespace BreadcrumbTestLib.ViewModels.TreeSelectors
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BmLib.Enums;

    /// <summary>
    /// Lookup until lookupvalue is found, and process only parent or matchednode.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    internal class RecursiveSearch<VM, T> : ITreeLookup<VM, T>
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool _loadSubEntries;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="loadSubEntries"></param>
        public RecursiveSearch(bool loadSubEntries)
        {
            Logger.InfoFormat("_");
            this._loadSubEntries = loadSubEntries;
        }
        #endregion constructors

        public async Task LookupAsync(T value,
                                      ITreeSelector<VM, T> parentSelector,
                                      ICompareHierarchy<T> comparer,
                                      CancellationToken cancelToken,
                                      params ITreeLookupProcessor<VM, T>[] processors)
        {
            Logger.InfoFormat("_");

            IEnumerable<VM> subentries = this._loadSubEntries ?
                    await parentSelector.EntryHelper.LoadAsync() :
                     parentSelector.EntryHelper.AllNonBindable;

            if (subentries != null)
            {
                foreach (VM current in subentries)
                {
                    if (cancelToken != CancellationToken.None)
                        cancelToken.ThrowIfCancellationRequested();

                    if (current is ISupportBreadcrumbTreeItemViewModel<VM, T> && current is ISupportBreadcrumbTreeItemHelperViewModel<VM>)
                    {
                        var currentSelectionHelper = (current as ISupportBreadcrumbTreeItemViewModel<VM, T>).Selection;
                        var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, value);

                        switch (compareResult)
                        {
                            case HierarchicalResult.Current:
                                Process(processors, compareResult, parentSelector, currentSelectionHelper);
                                return;

                            case HierarchicalResult.Child:
                                if (Process(processors, compareResult, parentSelector, currentSelectionHelper))
                                {
                                    await this.LookupAsync(value, currentSelectionHelper,
                                                           comparer, cancelToken, processors);

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
    }
}
