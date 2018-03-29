namespace Breadcrumb.ViewModels.TreeSelectors
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Breadcrumb.ViewModels.Interfaces;

	/// <summary>
	/// Lookup until lookupvalue is found, and process all node.
	/// </summary>
	/// <typeparam name="VM"></typeparam>
	/// <typeparam name="T"></typeparam>
	public class RecrusiveBroadcast<VM, T> : ITreeLookup<VM, T>
	{
		#region fields
		/// <summary>
		///  Lookup until lookupvalue is found, and process all node.  Load entries if not loaded.
		/// </summary>
		public static RecrusiveBroadcast<VM, T> LoadSubentriesIfNotLoaded = new RecrusiveBroadcast<VM, T>(false);

		/// <summary>
		///  Lookup until lookupvalue is found, and process all node.  Skip if not loaded.
		/// </summary>
		public static RecrusiveBroadcast<VM, T> SkipIfNotLoaded = new RecrusiveBroadcast<VM, T>(false);

		private bool _loadSubEntries;
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="loadSubEntries"></param>
		public RecrusiveBroadcast(bool loadSubEntries)
		{
			this._loadSubEntries = loadSubEntries;
		}
		#endregion constructors

		#region methods
		public async Task LookupAsync(T value, ITreeSelector<VM, T> parentSelector,
			ICompareHierarchy<T> comparer, params ITreeLookupProcessor<VM, T>[] processors)
		{
			IEnumerable<VM> subentries = this._loadSubEntries ?
																		await parentSelector.EntryHelper.LoadAsync() :
																		 parentSelector.EntryHelper.AllNonBindable;

			foreach (VM current in subentries)
			{
				if (current is ISupportTreeSelector<VM, T> && current is ISupportEntriesHelper<VM>)
				{
					var currentSelectionHelper = (current as ISupportTreeSelector<VM, T>).Selection;
					var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, value);

					if (processors.Process(compareResult, parentSelector, currentSelectionHelper))
					{
						await this.LookupAsync(value, currentSelectionHelper, comparer, processors);
						return;
					}

					break;
				}
			}
		}
		#endregion methods
	}
}
