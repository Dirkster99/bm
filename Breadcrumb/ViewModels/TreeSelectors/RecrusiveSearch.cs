namespace Breadcrumb.ViewModels.TreeSelectors
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Breadcrumb.Defines;
	using Breadcrumb.ViewModels.Interfaces;

	/// <summary>
	/// Lookup until lookupvalue is found, and process only parent or matchednode.
	/// </summary>
	/// <typeparam name="VM"></typeparam>
	/// <typeparam name="T"></typeparam>
	public class RecrusiveSearch<VM, T> : ITreeLookup<VM, T>
	{
		#region fields
		/// <summary>
		/// Lookup until lookupvalue is found, and process only parent or matchednode.
		/// Load subentries if not loaded.
		/// </summary>
		public static RecrusiveSearch<VM, T> LoadSubentriesIfNotLoaded = new RecrusiveSearch<VM, T>(true);

		/// <summary>
		/// Lookup until lookupvalue is found, and process only parent or matchednode.  Skip if not loaded.
		/// </summary>
		public static RecrusiveSearch<VM, T> SkipIfNotLoaded = new RecrusiveSearch<VM, T>(false);

		private bool _loadSubEntries;
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="loadSubEntries"></param>
		public RecrusiveSearch(bool loadSubEntries)
		{
			this._loadSubEntries = loadSubEntries;
		}
		#endregion constructors

		public async Task LookupAsync(T value, ITreeSelector<VM, T> parentSelector,
			 ICompareHierarchy<T> comparer, params ITreeLookupProcessor<VM, T>[] processors)
		{
			IEnumerable<VM> subentries = this._loadSubEntries ?
					await parentSelector.EntryHelper.LoadAsync() :
					 parentSelector.EntryHelper.AllNonBindable;

			if (subentries != null)
			{
				foreach (VM current in subentries)
				{
					if (current is ISupportTreeSelector<VM, T> && current is ISupportEntriesHelper<VM>)
					{
						var currentSelectionHelper = (current as ISupportTreeSelector<VM, T>).Selection;
						var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, value);
						switch (compareResult)
						{
							case HierarchicalResult.Current:
								processors.Process(compareResult, parentSelector, currentSelectionHelper);
								return;

							case HierarchicalResult.Child:
								if (processors.Process(compareResult, parentSelector, currentSelectionHelper))
								{
									await this.LookupAsync(value, currentSelectionHelper, comparer, processors);

									return;
								}

								break;
						}
					}
				}
			}
		}
	}
}
