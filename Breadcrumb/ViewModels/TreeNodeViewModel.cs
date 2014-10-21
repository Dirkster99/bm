namespace Breadcrumb.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Threading.Tasks;
	using Breadcrumb.ViewModels.Helpers;
	using Breadcrumb.ViewModels.Interfaces;
	using Breadcrumb.ViewModels.TreeSelectors;

	public class TreeNodeViewModel : INotifyPropertyChanged, ISupportTreeSelector<TreeNodeViewModel, string>
	{
		#region fields
		private string _header = "NotLoaded";
		private string _path = string.Empty;

		private TreeViewModel _root = null;
		private TreeNodeViewModel _parent = null;
		////private bool _isOverflowed = false;
		#endregion fields

		#region constructors
		public TreeNodeViewModel(string value, string header, TreeViewModel root, TreeNodeViewModel parentNode)
		{
			if (root == null || value == null)
				throw new ArgumentException();

			this._path = value;
			this._root = root;
			this._parent = parentNode;
			this._header = header;

			this.Entries = new EntriesHelperViewModel<TreeNodeViewModel>((ct) => Task.Run(() =>
			{
				return (IEnumerable<TreeNodeViewModel>)new List<TreeNodeViewModel>(
						from i in Enumerable.Range(1, 9)
						select new TreeNodeViewModel(
								(Path + "\\Sub" + i.ToString()).TrimStart('\\'),
								"Sub" + i.ToString(),
								this._root, this));
			}));

			this.Selection = new TreeSelectorViewModel<TreeNodeViewModel, string>(value, this,
																parentNode == null ? root.Selection : parentNode.Selection, this.Entries);
		}

		protected TreeNodeViewModel() // For DummyNode
		{
		}
		#endregion constructors

		#region events
		public event PropertyChangedEventHandler PropertyChanged = (o, e) => { };
		#endregion events

		#region properties
		public ITreeSelector<TreeNodeViewModel, string> Selection { get; set; }

		public IEntriesHelper<TreeNodeViewModel> Entries { get; set; }

		////Lycj: Moved to Selection.IsOverflowed/IsOverflowedOrRoot
		////public bool IsOverflowedOrRoot { get { return _isOverflowed || _parent == null; } set { } }
		////public bool IsOverflowed
		////{
		////  get { return _isOverflowed; }
		////  set
		////  {
		////    _isOverflowed = value;
		////    NotifyPropertyChanged("IsOverflowed"); NotifyPropertyChanged("IsOverflowedOrRoot");
		////  }
		////}

		public string Header
		{
			get
			{
				return this._header;
			}
			
			set
			{
				this._header = value;
				this.NotifyPropertyChanged("Header");
			}
		}

		public string Path
		{
			get
			{
				return this._path;
			}
			
			set
			{
				this._path = value;
				this.NotifyPropertyChanged("Path");
			}
		}
		#endregion

		#region Methods
		public void NotifyPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.Header))
				return "TreeNode - Dummy";
			else
				return "TreeNode - " + this.Path;
		}
		#endregion
	}
}
