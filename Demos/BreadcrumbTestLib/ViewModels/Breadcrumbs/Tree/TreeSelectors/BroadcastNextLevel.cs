namespace BreadcrumbTestLib.ViewModels.TreeSelectors
{
    using System.Threading;
    using System.Threading.Tasks;
    using BmLib.Enums;
    using BreadcrumbTestLib.ViewModels.Interfaces;

    /// <summary>
    /// Lookup only next level of tree nodes, even if the lookupvalue is in deeper level, and process every node,
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    internal class BroadcastNextLevel<VM, T> : ITreeLookup<VM, T>
    {
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async Task LookupAsync(T targetLocation,
                                      ITreeSelector<VM, T> parentSelector,
                                      ICompareHierarchy<T> comparer,
                                      CancellationToken cancelToken,
                                      params ITreeLookupProcessor<VM, T>[] processors)
        {
            Logger.InfoFormat("target '{0}'", (targetLocation != null ? targetLocation.ToString() : "(null)"));

            foreach (VM current in await parentSelector.EntryHelper.LoadAsync())
            {
                if (cancelToken != CancellationToken.None)
                    cancelToken.ThrowIfCancellationRequested();

                if (current is ISupportBreadcrumbTreeItemViewModel<VM, T>)
                {
                    var currentSelectionHelper = (current as ISupportBreadcrumbTreeItemViewModel<VM, T>).Selection;
                    var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, targetLocation);

                    Process(processors, compareResult, parentSelector, currentSelectionHelper);
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
        private bool Process(ITreeLookupProcessor<VM, T>[] processors,
                             HierarchicalResult hr,
                             ITreeSelector<VM, T> parentSelector,
                             ITreeSelector<VM, T> selector)
        {
            foreach (var p in processors)
            {
                if (p.Process(hr, parentSelector, selector) == false)
                    return false;
            }

            return true;
        }
    }
}
