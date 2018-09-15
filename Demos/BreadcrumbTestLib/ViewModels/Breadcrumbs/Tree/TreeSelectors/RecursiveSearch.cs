namespace BreadcrumbTestLib.ViewModels.TreeSelectors
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BmLib.Enums;
    using BreadcrumbTestLib.ViewModels.TreeLookupProcessors;
    using DirectoryInfoExLib.Interfaces;
    using BreadcrumbTestLib.ViewModels.Breadcrumbs;

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
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="loadSubEntries"></param>
        public RecursiveSearch(bool reloadSubEntries = true)
        {
            // XXX TODO FixMe to use existing sub-entries without reload, if requested
        }
        #endregion constructors

        public async Task LookupAsync(T targetLocation,
                                      ITreeSelector<VM, T> parentSelector,
                                      ICompareHierarchy<T> comparer,
                                      CancellationToken cancelToken,
                                      params ITreeLookupProcessor<VM, T>[] processors)
        {
            Logger.InfoFormat("target '{0}'", (targetLocation != null ? targetLocation.ToString() : "(null)"));

            // Always re-Load sub-entries below this node via parentSelector if requested
            // (XXX TODO FixMe to use existing entries without reload, if requested)
            IEnumerable<VM> subentries = await parentSelector.EntryHelper.LoadAsync();

            if (subentries != null)
            {
                // Go through each sub-entry and determine if any entry is:
                // 1) Same as targetLocation or
                // 2) targetLocation is child of this sub-entry (reload its children in turn)
                foreach (VM current in subentries)
                {
                    if (cancelToken != CancellationToken.None)
                        cancelToken.ThrowIfCancellationRequested();

                    if (current is ISupportBreadcrumbTreeItemViewModel<VM, T> && current is ISupportBreadcrumbTreeItemHelperViewModel<VM>)
                    {
                        var currentSelectionHelper = (current as ISupportBreadcrumbTreeItemViewModel<VM, T>).Selection;
                        var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, targetLocation);

                        switch (compareResult)
                        {
                            case HierarchicalResult.Current:
                                Process(processors, compareResult, parentSelector, currentSelectionHelper);
                                return;

                            case HierarchicalResult.Child:
                                if (Process(processors, compareResult, parentSelector, currentSelectionHelper))
                                {
                                    await this.LookupAsync(targetLocation, currentSelectionHelper,
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
        private bool Process(ITreeLookupProcessor<VM, T>[] processors,
                            HierarchicalResult hr,
                            ITreeSelector<VM, T> parentSelector,
                            ITreeSelector<VM, T> selector)
        {
            foreach (var p in processors)
            {
                bool retVal = p.Process(hr, parentSelector, selector);

                if (retVal == false)
                    return false;
            }

            return true;
        }
    }
}
