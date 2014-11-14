namespace Breadcrumb.ViewModels
{
	using System;
	using System.ComponentModel;
	using Breadcrumb.Defines;
	using Breadcrumb.ViewModels.Helpers;
	using Breadcrumb.ViewModels.Interfaces;

	public class TreeViewModel : INotifyPropertyChanged,
				ISupportTreeSelector<TreeNodeViewModel, string>, ICompareHierarchy<string>
	{
		public static string Format_DragDropItem = "DragDropItemVM";

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public TreeViewModel()
		{
			// Submodel is TreeNodeViewModel,
			this.Entries = new EntriesHelperViewModel<TreeNodeViewModel>();
			
			// Value is based on string
			this.Selection = new TreeRootSelectorViewModel<TreeNodeViewModel, string>(this.Entries)
			{
				Comparers = new[] { this }
			};

			this.Entries.SetEntries(UpdateMode.Update, new TreeNodeViewModel(string.Empty, "Root", this, null),
					                                       new TreeNodeViewModel("AA", "Root", this, null));            
		}
		#endregion constructors

		#region events
        public event PropertyChangedEventHandler PropertyChanged = (o, e) => { };
		#endregion events

		#region properties
		public ITreeSelector<TreeNodeViewModel, string> Selection { get; set; }

		public IEntriesHelper<TreeNodeViewModel> Entries { get; set; }
		#endregion properties

		#region methods
		public HierarchicalResult CompareHierarchy(string path1, string path2)
		{
			if (path1 == null || path2 == null)
				return HierarchicalResult.Unrelated;

			if (path1.Equals(path2, StringComparison.CurrentCultureIgnoreCase))
				return HierarchicalResult.Current;

			if (path1.StartsWith(path2, StringComparison.CurrentCultureIgnoreCase))
				return HierarchicalResult.Parent;

			if (path2.StartsWith(path1, StringComparison.CurrentCultureIgnoreCase))
				return HierarchicalResult.Child;

			return HierarchicalResult.Unrelated;
		}
		#endregion methods
	}
}
