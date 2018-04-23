namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbTestLib.ViewModels.TreeLookupProcessors;
    using BreadcrumbTestLib.ViewModels.TreeSelectors;
    using BreadcrumbLib.Defines;
    using BreadcrumbLib.Utils;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    internal class TreeRootSelectorViewModel<VM, T> : TreeSelectorViewModel<VM, T>, ITreeRootSelector<VM, T>
    {
        #region fields
        private T _selectedValue = default(T);
        private ITreeSelector<VM, T> _selectedSelector;
        private Stack<ITreeSelector<VM, T>> _prevPath = null;
        private IEnumerable<ICompareHierarchy<T>> _comparers;
        private ObservableCollection<VM> _rootItems = null;
        #endregion fields

        #region constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entryHelper"></param>
        /// <param name="compareFunc"></param>
        /// <param name="rootLevel">Level of TreeItem to consider as root, root items should shown in expander 
        /// (e.g. in OverflowedAndRootItems) and have caption and expander hidden when the path is longer than it.</param>
        public TreeRootSelectorViewModel(IEntriesHelper<VM> entryHelper) ////int rootLevel = 0,
                                                                         ////params Func<T, T, HierarchicalResult>[] compareFuncs)
          : base(entryHelper)
        {
            ////_rootLevel = rootLevel;
            ////_compareFuncs = compareFuncs;
            ////Comparers = new [] { PathComparer.LocalDefault };
        }

        #endregion constructors

        #region events
        public event EventHandler SelectionChanged;
        #endregion events

        #region properties
        public ObservableCollection<VM> OverflowedAndRootItems
        {
            get
            {
                if (this._rootItems == null)
                    this.updateRootItems();

                return this._rootItems;
            }

            set
            {
                this._rootItems = value;
                this.NotifyPropertyChanged(() => this.OverflowedAndRootItems);
            }
        }

        public ITreeSelector<VM, T> SelectedSelector
        {
            get { return this._selectedSelector; }
        }

        public VM SelectedViewModel
        {
            get
            {
                return (this.SelectedSelector == null ? default(VM) : this.SelectedSelector.ViewModel);
            }
        }

        /// <summary>
        /// Gets/sets the select item when the user opens the drop down and selects 1 item
        /// in the dropdownlist of the RootDropDown button.
        /// </summary>
        public T SelectedValue
        {
            get
            {
                return this._selectedValue;
            }

            set
            {
                AsyncUtils.RunAsync(() => this.SelectAsync(value, CancellationToken.None));
            }
        }

        ////public int RootLevel { get { return _rootLevel; } set { _rootLevel = value; } }

        public IEnumerable<ICompareHierarchy<T>> Comparers
        {
            get
            {
                return this._comparers;
            }

            set
            {
                this._comparers = value;
            }
        }
        #endregion properties

        #region methods
        public override void ReportChildSelected(Stack<ITreeSelector<VM, T>> path)
        {
            ITreeSelector<VM, T> prevSelector = this._selectedSelector;

            T prevSelectedValue = this._selectedValue;
            this._prevPath = path;

            this._selectedSelector = path.Last();
            this._selectedValue = path.Last().Value;

            if (prevSelectedValue != null && !prevSelectedValue.Equals(path.Last().Value))
            {
                prevSelector.IsSelected = false;
            }

            this.NotifyPropertyChanged(() => this.SelectedValue);
            this.NotifyPropertyChanged(() => this.SelectedViewModel);

            if (this.SelectionChanged != null)
                this.SelectionChanged(this, EventArgs.Empty);

            this.updateRootItems(path);

            path.Last().EntryHelper.LoadAsync();
        }

        public override void ReportChildDeselected(Stack<ITreeSelector<VM, T>> path)
        {
        }

        /// <summary>
        /// Method can be invoked on the tree root to select a tree node by value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="progress"></param>
        /// <returns>Returns a task that selects the requested tree node.</returns>
        public async Task<FinalBrowseResult<T>> SelectAsync(
            T value,
            CancellationToken cancelToken = default(CancellationToken),
            IProgressViewModel progress = null)
        {
            if (_selectedValue == null ||
                CompareHierarchy(_selectedValue, value) != HierarchicalResult.Current)
            {
                try
                {
                    if (progress != null)                     // Show indeterminate progress
                        progress.ShowIndeterminatedProgress();

                    try
                    {
                        await this.LookupAsync(value,
                                               RecrusiveSearch<VM, T>.LoadSubentriesIfNotLoaded,
                                               cancelToken,
                                               SetSelected<VM, T>.WhenSelected,
                                               SetChildSelected<VM, T>.ToSelectedChild,
                                               LoadSubEntries<VM, T>.WhenSelected(UpdateMode.Replace,
                                               false, null));

                        return new FinalBrowseResult<T>(value, default(System.Guid), BrowseResult.Complete);
                    }
                    catch (Exception)
                    {
                        return new FinalBrowseResult<T>(value, default(System.Guid), BrowseResult.InComplete);
                    }
                }
                finally
                {
                    if (progress != null)              // Remove progress display
                        progress.ProgressDisplayOff();
                }
            }

            return new FinalBrowseResult<T>(value, default(System.Guid), BrowseResult.InComplete);
        }

        public HierarchicalResult CompareHierarchy(T value1, T value2)
        {
            foreach (var c in this.Comparers)
            {
                var retVal = c.CompareHierarchy(value1, value2);
                if (retVal != HierarchicalResult.Unrelated)
                    return retVal;
            }
            return HierarchicalResult.Unrelated;
        }

        private async Task updateRootItemsAsync(ITreeSelector<VM, T> selector, ObservableCollection<VM> rootItems, int level)
        {
            if (level == 0)
                return;

            List<ITreeSelector<VM, T>> rootTreeSelectors = new List<ITreeSelector<VM, T>>();

            // Perform a lookup and for all directories in next level of current directory (load asynchronously if not loaded), 
            // add directories's Selector to rootTreeSelectors.
            await selector.LookupAsync(default(T),
                                       BroadcastNextLevel<VM, T>.LoadSubentriesIfNotLoaded,
                                       CancellationToken.None,
                                       new TreeLookupProcessor<VM, T>(HierarchicalResult.All, (hr, p, c) =>
                                       {
                                           rootTreeSelectors.Add(c);
                                           return true;
                                       }));

            // Then foreach rootTreeSelectors, add to rootItems and preferm updateRootItemAsync.
            foreach (var c in rootTreeSelectors)
            {
                rootItems.Add(c.ViewModel);
                c.IsRoot = true;

                await this.updateRootItemsAsync(c, rootItems, level - 1);
            }
        }

        /// <summary>
        /// Method is executed when the user clicks the RootDropDown button.
        /// </summary>
        /// <param name="path"></param>
        private void updateRootItems(Stack<ITreeSelector<VM, T>> path = null)
        {
            ////if (_rootItems == null)
            this._rootItems = new ObservableCollection<VM>();
            ////else _rootItems.Clear();

            if (path != null && path.Count() > 0)
            {
                foreach (var p in path.Reverse())
                {
                    if (!(this.EntryHelper.AllNonBindable.Contains(p.ViewModel)))
                        this._rootItems.Add(p.ViewModel);
                }

                this._rootItems.Add(default(VM)); // Separator
            }

            // Get all items for display in the root drop down list
            AsyncUtils.RunAsync(() => this.updateRootItemsAsync(this, this._rootItems, 1));
        }
        #endregion
    }
}
