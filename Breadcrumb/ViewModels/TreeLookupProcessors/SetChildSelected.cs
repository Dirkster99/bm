namespace Breadcrumb.ViewModels.TreeLookupProcessors
{
    using Breadcrumb.Defines;
    using Breadcrumb.ViewModels.Interfaces;
    using BreadcrumbLib.Defines;

    /// <summary>
    /// Set Parent ViewModel's ITreeSelector.SelectedChild to Current ViewModel's Value.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class SetChildSelected<VM, T> : ITreeLookupProcessor<VM, T>
	{
		/// <summary>
		/// Set Parent ViewModel's ITreeSelector.SelectedChild to Current ViewModel's Value, if is child of lookup value of current lookup.
		/// </summary>
		public static SetChildSelected<VM, T> ToSelectedChild = new SetChildSelected<VM, T>();

		public bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
		{
			if (hr == HierarchicalResult.Child || hr == HierarchicalResult.Current)
				parentSelector.SelectedChild = selector.Value;

			return true;
		}
	}
}
