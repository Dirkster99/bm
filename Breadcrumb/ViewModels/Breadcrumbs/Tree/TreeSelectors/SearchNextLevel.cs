namespace Breadcrumb.ViewModels.TreeSelectors
{
    using System.Threading;
    using System.Threading.Tasks;
    using Breadcrumb.ViewModels.Interfaces;
    using BreadcrumbLib.Defines;

    /// <summary>
    /// Lookup only next level of tree nodes, even if the lookupvalue is in deeper level, and process only the matched node.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class SearchNextLevel<VM, T> : ITreeLookup<VM, T>
	{
		/// <summary>
		/// Lookup only next level of tree nodes, even if the lookupvalue is in deeper level, and process only the matched node.
		/// Load subentries if not loaded.
		/// </summary>
		public static SearchNextLevel<VM, T> LoadSubentriesIfNotLoaded = new SearchNextLevel<VM, T>();

		public async Task LookupAsync(T value,
                                      ITreeSelector<VM, T> parentSelector,
				                      ICompareHierarchy<T> comparer,
                                      CancellationToken cancelToken,
                                      params ITreeLookupProcessor<VM, T>[] processors)
		{
			foreach (VM current in await parentSelector.EntryHelper.LoadAsync())
            {
                if (cancelToken != CancellationToken.None)
                    cancelToken.ThrowIfCancellationRequested();

                if (current is ISupportTreeSelector<VM, T>)
                {
                    var currentSelectionHelper = (current as ISupportTreeSelector<VM, T>).Selection;
                    var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, value);

                    switch (compareResult)
                    {
                        case HierarchicalResult.Current:
                        case HierarchicalResult.Child:
                            processors.Process(compareResult, parentSelector, currentSelectionHelper);
                            return;
                    }
                }
            }
        }
	}
}
