namespace Breadcrumb.ViewModels.TreeLookupProcessors
{
	using Breadcrumb.Defines;
	using Breadcrumb.ViewModels.Interfaces;

	/// <summary>
	/// Set ViewModel's Selector.IsSelected to false 
	/// </summary>
	/// <typeparam name="VM"></typeparam>
	/// <typeparam name="T"></typeparam>
	public class SetNotSelected<VM, T> : ITreeLookupProcessor<VM, T>
	{
		#region fields
		/// <summary>
		/// Set ViewModel's Selector.IsSelected to false  if it's lookupvalue of current lookup.
		/// </summary>
		public static SetNotSelected<VM, T> WhenCurrent = new SetNotSelected<VM, T>(HierarchicalResult.Current);

		/// <summary>
		/// Set ViewModel's Selector.IsSelected to false  if it's NOT lookupvalue of current lookup.
		/// (e.g. child, parent and unrelated node)
		/// </summary>
		public static SetNotSelected<VM, T> WhenNotCurrent = new SetNotSelected<VM, T>(
				HierarchicalResult.Child | HierarchicalResult.Parent | HierarchicalResult.Unrelated);

		private HierarchicalResult _hr;
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="hr"></param>
		public SetNotSelected(HierarchicalResult hr)
		{
			this._hr = hr;
		}
		#endregion constructors

		public bool Process(HierarchicalResult hr,
		                    ITreeSelector<VM, T> parentSelector,
												ITreeSelector<VM, T> selector)
		{
			if (this._hr.HasFlag(hr))
				selector.IsSelected = false;

			return true;
		}
	}
}
