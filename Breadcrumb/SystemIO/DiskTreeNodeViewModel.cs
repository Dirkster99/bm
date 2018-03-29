namespace Breadcrumb.SystemIO
{
    using Breadcrumb.ViewModels.Base;
    using Breadcrumb.ViewModels.Helpers;
    using Breadcrumb.ViewModels.Interfaces;
    using Breadcrumb.ViewModels.TreeSelectors;
    using BreadcrumbLib.Defines;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    internal class DiskTreeNodeViewModel : ViewModelBase, ISupportTreeSelector<DiskTreeNodeViewModel, string>
	{
		#region fields
		public static ICompareHierarchy<string> Comparer = new PathComparer(StringComparison.CurrentCultureIgnoreCase);

		private DirectoryInfo _dir;
		private DiskTreeNodeViewModel _rootNode, _parentNode;
		////private bool _isOverflowed;
		private string _header;
		#endregion fields

		#region constructors
		public DiskTreeNodeViewModel(params DirectoryInfo[] dir)
		{
			this.Entries = new EntriesHelperViewModel<DiskTreeNodeViewModel>();
			this.Selection = new TreeRootSelectorViewModel<DiskTreeNodeViewModel, string>(this.Entries)
			{
				Comparers = new[] { DiskTreeNodeViewModel.Comparer }
			};

			this.Entries.SetEntries(UpdateMode.Update, dir.Select(d => new DiskTreeNodeViewModel(d, this)).ToArray());
			this.Header = string.Empty;
		}
		#endregion constructors

		internal DiskTreeNodeViewModel(DirectoryInfo dir, DiskTreeNodeViewModel parentNode)
		{
			this._dir = dir;

			// If parentNode == null => Root.
			this._rootNode = parentNode == null ? this : parentNode._rootNode;

			this._parentNode = parentNode;
			this.Header = this._dir.Name;

			this.Entries = new EntriesHelperViewModel<DiskTreeNodeViewModel>((ct) => Task.Run(() =>
			{
				try
				{
					return this._dir.GetDirectories().Select(d => new DiskTreeNodeViewModel(d, this));
				}
				catch
				{
					return new List<DiskTreeNodeViewModel>();
				}
			}));

			this.Selection = new TreeSelectorViewModel<DiskTreeNodeViewModel, string>(this._dir.FullName,
			                                                                          this,
																																								this._parentNode.Selection,
																																								this.Entries);
		}

		public ITreeSelector<DiskTreeNodeViewModel, string> Selection { get; set; }

		public IEntriesHelper<DiskTreeNodeViewModel> Entries { get; set; }

		// Lycj: Moved to Selection.IsOverflowed/IsOverflowedOrRoot
		////public bool IsOverflowedOrRoot { get { return _isOverflowed || _parentNode == null; } set { } }
		////public bool IsOverflowed
		////{
		////    get { return _isOverflowed; }
		////    set
		////    {
		////        _isOverflowed = value;
		////        NotifyOfPropertyChanged(() => IsOverflowed);
		////        NotifyOfPropertyChanged(() => IsOverflowedOrRoot);
		////    }
		////}
		public string Header
		{ get
			{
				return this._header;
			}
			
			set
			{
				this._header = value;
				this.NotifyOfPropertyChanged(() => this.Header);
			}
		}
	}
}
