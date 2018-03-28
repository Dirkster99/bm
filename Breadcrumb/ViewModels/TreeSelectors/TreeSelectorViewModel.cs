namespace Breadcrumb.ViewModels.TreeSelectors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Breadcrumb.Defines;
    using Breadcrumb.Utils;
    using Breadcrumb.ViewModels.Interfaces;
    using Breadcrumb.ViewModels.TreeLookupProcessors;
    using BreadcrumbLib.Utils;
    using BreadcrumbLiv.Viewmodels.Base;
    using BreadcrumbLib.Defines;

    /// <summary>
    /// Base class of ITreeSelector, which implement Tree based structure and support LookupProcessing.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class TreeSelectorViewModel<VM, T> : NotifyPropertyChanged, ITreeSelector<VM, T>
	{
		#region fields
		private readonly AsyncLock _lookupLock = new AsyncLock();
		private T _currentValue = default(T);
		private bool _isSelected = false;
		private T _selectedValue = default(T);
		private ITreeSelector<VM, T> _prevSelected = null;

		private VM _currentViewModel;
		private bool _isRoot = false;
		private bool _isOverflowed;
		#endregion fields

		#region constructors
		public TreeSelectorViewModel(T currentValue, VM currentViewModel,
																 ITreeSelector<VM, T> parentSelector,
																 IEntriesHelper<VM> entryHelper)
		{
			this.RootSelector = parentSelector.RootSelector;
			this.ParentSelector = parentSelector;
			this.EntryHelper = entryHelper;

			this._currentValue = currentValue;
			this._currentViewModel = currentViewModel;
		}

		protected TreeSelectorViewModel(IEntriesHelper<VM> entryHelper)
		{
			this.EntryHelper = entryHelper;
			this.RootSelector = this as ITreeRootSelector<VM, T>;
		}
		#endregion

		#region Public Properties
		public T Value
		{
			get { return this._currentValue; }
		}

		public VM ViewModel
		{
			get { return this._currentViewModel; }
		}

		public ITreeSelector<VM, T> ParentSelector { get; private set; }

		public ITreeRootSelector<VM, T> RootSelector { get; private set; }

		public IEntriesHelper<VM> EntryHelper { get; private set; }

		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}

			set
			{
				if (this._isSelected != value)
				{
					this._isSelected = value;
					this.NotifyOfPropertyChange(() => this.IsSelected);
					this.SelectedChild = default(T);

					if (value)
						this.ReportChildSelected(new Stack<ITreeSelector<VM, T>>());
					else
						this.ReportChildDeselected(new Stack<ITreeSelector<VM, T>>());
				}
			}
		}

		public bool IsRoot
		{
			get
			{
				return this._isRoot;
			}

			set
			{
				this._isRoot = value;
				this.NotifyOfPropertyChange(() => this.IsRoot);
				this.NotifyOfPropertyChange(() => this.IsRootAndIsChildSelected);
			}
		}

		public virtual bool IsChildSelected
		{
			get { return this._selectedValue != null; }
		}

		public virtual bool IsRootAndIsChildSelected
		{
			get { return this.IsRoot && this.IsChildSelected; }
		}

		public T SelectedChild
		{
			get
			{
				return this._selectedValue;
			}

			set
			{
				this._selectedValue = value;

				this.NotifyOfPropertyChange(() => this.SelectedChild);
				this.NotifyOfPropertyChange(() => this.SelectedChildUI);
				this.NotifyOfPropertyChange(() => this.IsChildSelected);
				this.NotifyOfPropertyChange(() => this.IsRootAndIsChildSelected);
			}
		}

		public T SelectedChildUI
		{
			get
			{
				return this._selectedValue;
			}

			set
			{
				this.IsSelected = false;
				this.NotifyOfPropertyChange(() => this.IsSelected);

				if (this._selectedValue == null || !this._selectedValue.Equals(value))
				{
					if (this._prevSelected != null)
					{
						this._prevSelected.IsSelected = false;
					}

					this.SelectedChild = value;

					if (value != null)
					{
						AsyncUtils.RunAsync(async () => await this.LookupAsync(value,
														SearchNextLevel<VM, T>.LoadSubentriesIfNotLoaded,
														new TreeLookupProcessor<VM, T>(HierarchicalResult.Related, (hr, p, c) =>
														{
															c.IsSelected = true;
															this._prevSelected = c;

															return true;
														})));
					}
				}
			}
		}

		public bool IsOverflowedOrRoot
		{
			get { return this._isOverflowed || this.IsRoot; }

			set { }
		}

		public bool IsOverflowed
		{
			get
			{
				return this._isOverflowed;
			}

			set
			{
				this._isOverflowed = value;
				this.NotifyOfPropertyChange(() => this.IsOverflowed);
				this.NotifyOfPropertyChange(() => this.IsOverflowedOrRoot);
			}
		}
		#endregion

		#region methods
		public override string ToString()
		{
			return this._currentValue == null ? string.Empty : this._currentValue.ToString();
		}

		/// <summary>
		/// Bubble up to TreeSelectionHelper for selection.
		/// </summary>
		/// <param name="path"></param>
		public virtual void ReportChildSelected(Stack<ITreeSelector<VM, T>> path)
		{
			if (path.Count() > 0)
			{
				this._selectedValue = path.Peek().Value;

				this.NotifyOfPropertyChange(() => this.SelectedChild);
				this.NotifyOfPropertyChange(() => this.SelectedChildUI);
			}

			path.Push(this);

			if (this.ParentSelector != null)
				this.ParentSelector.ReportChildSelected(path);
		}

		public virtual void ReportChildDeselected(Stack<ITreeSelector<VM, T>> path)
		{
			if (this.EntryHelper.IsLoaded)
			{
				// Clear child node selection.
				this.SelectedChild = default(T);

				// And just in case if the new selected value is child of this node.
				if (this.RootSelector.SelectedValue != null)
				{
					AsyncUtils.RunAsync(() => this.LookupAsync(
		this.RootSelector.SelectedValue,
		new SearchNextUsingReverseLookup<VM, T>(this.RootSelector.SelectedSelector),
		new TreeLookupProcessor<VM, T>(HierarchicalResult.All, (hr, p, c) =>
		{
			this.SelectedChild = c == null ? default(T) : c.Value;

			return true;
		})));
				}

				// SetSelectedChild(lookupResult == null ? default(T) : lookupResult.Value);
				this.NotifyOfPropertyChange(() => this.IsChildSelected);
				this.NotifyOfPropertyChange(() => this.SelectedChild);
				this.NotifyOfPropertyChange(() => this.SelectedChildUI);
			}

			path.Push(this);

			if (this.ParentSelector != null)
				this.ParentSelector.ReportChildDeselected(path);
		}

		/// <summary>
		/// Tunnel down to select the specified item.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="currentAction"></param>
		/// <returns></returns>
		public async Task LookupAsync(T value,
				ITreeLookup<VM, T> lookupProc,
				params ITreeLookupProcessor<VM, T>[] processors)
		{
			using (await this._lookupLock.LockAsync())
			{
				await lookupProc.LookupAsync(value, this, this.RootSelector, processors);
			}
		}
		#endregion
	}
}
