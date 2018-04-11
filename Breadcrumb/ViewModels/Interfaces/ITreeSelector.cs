namespace Breadcrumb.ViewModels.Interfaces
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Threading.Tasks;

	/// <summary>
	/// Implement Tree based structure and support LookupProcessing.
	/// </summary>
	/// <typeparam name="VM">Sub-node viewmodel type.</typeparam>
	/// <typeparam name="T">Type to identify a node, commonly string.</typeparam>
	public interface ITreeSelector<VM, T> : INotifyPropertyChanged
	{
		#region properties
		/// <summary>
		/// Whether current view model is selected.
		/// </summary>
		bool IsSelected { get; set; }

		/// <summary>
		/// This is marked by TreeRootSelector, for overflow menu support.
		/// </summary>
		bool IsRoot { get; set; }

		/// <summary>
		/// Whether a child of current view model is selected.
		/// </summary>
		bool IsChildSelected { get; }

		/// <summary>
		/// Based on IsRoot and IsChildSelected
		/// </summary>
		bool IsRootAndIsChildSelected { get; }

		/// <summary>
		/// The selected child of current view model.          
		/// </summary>
		T SelectedChild { get; set; }

		/// <summary>
		/// Lycj: Removed SetSelectedChild and SetIsSelected.
		/// 
		/// The selected child of current view model, for use in UI only.          
		/// </summary>
		T SelectedChildUI { get; set; }

		/// <summary>
		/// The owner view model of this selection helper.
		/// </summary>
		VM ViewModel { get; }

		/// <summary>
		/// The represented value of this selection helper.
		/// </summary>
		T Value { get; }

		/// <summary>
		/// Parent ViewModel's ITreeSelector.
		/// </summary>
		ITreeSelector<VM, T> ParentSelector { get; }

		/// <summary>
		/// Root ViewModel's ITreeSelector.
		/// </summary>
		ITreeRootSelector<VM, T> RootSelector { get; }

		/// <summary>
		/// For loading sub-entries of current tree.
		/// </summary>
		IEntriesHelper<VM> EntryHelper { get; }
		#endregion properties

		#region methods
		/// <summary>
		/// Used by a tree node to report to it's root it's selected.
		/// </summary>
		/// <param name="path"></param>
		void ReportChildSelected(Stack<ITreeSelector<VM, T>> path);

		/// <summary>
		/// Used by a tree node to report to it's parent it's deselected.
		/// </summary>
		/// <param name="path"></param>
		void ReportChildDeselected(Stack<ITreeSelector<VM, T>> path);

		/// <summary>        
		/// Find requested node using lookupProc using type T, after any HierarchicalResult,
        /// use processors to perform further action.
		/// </summary>
		/// <example>
		/// //This will select the C:\Temp Node.
		/// await LookupAsync(@"c:\temp", RecursiveSearch.LoadSubentriesIfNotLoaded, SetSelected.WhenSelected);
		/// </example>
		/// <param name="value"></param>
		/// <param name="lookupProc"></param>
		/// <param name="processors"></param>
		/// <returns></returns>
		Task LookupAsync(T value, ITreeLookup<VM, T> lookupProc,
				params ITreeLookupProcessor<VM, T>[] processors);
		#endregion methods
	}
}
