namespace Breadcrumb.ViewModels.TreeSelectors
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Breadcrumb.ViewModels.Interfaces;
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
		public async Task LookupAsync(T value, ITreeSelector<VM, T> parentSelector,
				ICompareHierarchy<T> comparer, params ITreeLookupProcessor<VM, T>[] processors)
		{
            await Task.FromResult(0);
			if (parentSelector.EntryHelper.IsLoaded)
			{
				foreach (VM current in parentSelector.EntryHelper.AllNonBindable)
				{
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
