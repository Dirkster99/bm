namespace BreadcrumbTestLib.ViewModels.TreeSelectors
{
    using System.Threading;
    using System.Threading.Tasks;
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

        /// <summary>
        /// Lookup only next level of tree nodes, load subentries if not already loaded.
        /// </summary>
        public static BroadcastNextLevel<VM, T> LoadSubentriesIfNotLoaded = new BroadcastNextLevel<VM, T>();

        public async Task LookupAsync(T value,
                                      ITreeSelector<VM, T> parentSelector,
                                      ICompareHierarchy<T> comparer,
                                      CancellationToken cancelToken,
                                      params ITreeLookupProcessor<VM, T>[] processors)
        {
            Logger.InfoFormat("_");

            foreach (VM current in await parentSelector.EntryHelper.LoadAsync())
            {
                if (cancelToken != CancellationToken.None)
                    cancelToken.ThrowIfCancellationRequested();

                if (current is ISupportTreeSelector<VM, T>)
                {
                    var currentSelectionHelper = (current as ISupportTreeSelector<VM, T>).Selection;
                    var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, value);
                    processors.Process(compareResult, parentSelector, currentSelectionHelper);
                }
            }
        }
    }
}
