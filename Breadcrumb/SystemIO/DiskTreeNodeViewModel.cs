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
			Entries = new EntriesHelperViewModel<DiskTreeNodeViewModel>();
			Selection = new TreeRootSelectorViewModel<DiskTreeNodeViewModel, string>(this.Entries)
			{
				Comparers = new[] { DiskTreeNodeViewModel.Comparer }
			};

			Entries.SetEntries(UpdateMode.Update, dir.Select(d => new DiskTreeNodeViewModel(d, this)).ToArray());
			Header = string.Empty;
		}

		internal DiskTreeNodeViewModel(DirectoryInfo dir, DiskTreeNodeViewModel parentNode)
		{
			_dir = dir;

			// If parentNode == null => Root.
			_rootNode = parentNode == null ? this : parentNode._rootNode;

			_parentNode = parentNode;
			Header = _dir.Name;

			this.Entries = new EntriesHelperViewModel<DiskTreeNodeViewModel>((ct) => Task.Run(() =>
			{
				try
				{
					return _dir.GetDirectories().Select(d => new DiskTreeNodeViewModel(d, this));
				}
				catch
				{
					return new List<DiskTreeNodeViewModel>();
				}
			}));

			this.Selection = new TreeSelectorViewModel<DiskTreeNodeViewModel, string>
                (_dir.FullName,
                 this,
				 _parentNode.Selection,
				 Entries);
		}
        #endregion constructors

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
        ////        NotifyPropertyChanged(() => IsOverflowed);
        ////        NotifyPropertyChanged(() => IsOverflowedOrRoot);
        ////    }
        ////}
        public string Header
		{ get
			{
				return _header;
			}
			
			set
			{
				_header = value;
				NotifyPropertyChanged(() => this.Header);
			}
		}
	}
}
