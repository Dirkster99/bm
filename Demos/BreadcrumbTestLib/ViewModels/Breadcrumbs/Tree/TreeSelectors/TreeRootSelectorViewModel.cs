namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbTestLib.ViewModels.TreeLookupProcessors;
    using BreadcrumbTestLib.ViewModels.TreeSelectors;
    using BreadcrumbLib.Utils;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using BreadcrumbLib.Enums;

    internal class TreeRootSelectorViewModel<VM, T> : TreeSelectorViewModel<VM, T>, ITreeRootSelector<VM, T>
    {
        #region fields
        private T _selectedValue = default(T);
        private ITreeSelector<VM, T> _selectedSelector;
        private Stack<ITreeSelector<VM, T>> _prevPath = null;
        private IEnumerable<ICompareHierarchy<T>> _comparers;
        private ObservableCollection<VM> _OverflowedAndRootItems = null;
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

            _OverflowedAndRootItems = new ObservableCollection<VM>();
        }

        #endregion constructors

        #region events
        public event EventHandler SelectionChanged;
        #endregion events

        #region properties
        public IEnumerable<VM> OverflowedAndRootItems
        {
            get
            {
                return _OverflowedAndRootItems;
            }
        }

        public ITreeSelector<VM, T> SelectedSelector
        {
            get { return _selectedSelector; }
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
                return _selectedValue;
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
                return _comparers;
            }

            set
            {
                _comparers = value;
            }
        }
        #endregion properties

        #region methods
        public override void ReportChildSelected(Stack<ITreeSelector<VM, T>> path)
        {
            ITreeSelector<VM, T> prevSelector = _selectedSelector;

            T prevSelectedValue = _selectedValue;
            _prevPath = path;

            _selectedSelector = path.Last();
            _selectedValue = path.Last().Value;

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
                        SetChildSelected<VM, T> toSelectedChild = new SetChildSelected<VM, T>();

                        await this.LookupAsync(value,
                                               RecrusiveSearch<VM, T>.LoadSubentriesIfNotLoaded,
                                               cancelToken,
                                               SetSelected<VM, T>.WhenSelected,
                                               toSelectedChild,
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

        /// <summary>
        /// Method is executed when the control navigates to a new location -
        /// it re-computes the rootitems along with the overflowed items displayed
        /// in the root items drop down collection.
        /// </summary>
        /// <param name="path"></param>
        private void updateRootItems(Stack<ITreeSelector<VM, T>> path)
        {
            _OverflowedAndRootItems.Clear();

            if (path != null && path.Count() > 0)
            {
                // Get all items that belong to the current path and add them into the
                // OverflowedAndRootItems collection
                // The item.IsOverflowed property is (re)-set by OverflowableStackPanel
                // when the UI changes - a converter in the binding shows only those entries
                // in the root drop down list with item.IsOverflowed == true
                foreach (var p in path.Reverse())
                {
                    if (!(this.EntryHelper.AllNonBindable.Contains(p.ViewModel)))
                    {
                        _OverflowedAndRootItems.Add(p.ViewModel);

                    }
                }

                _OverflowedAndRootItems.Add(default(VM)); // Separator
            }

            // Get all items for display in the root drop down list
            AsyncUtils.RunAsync(() => this.updateRootItemsAsync(this, _OverflowedAndRootItems, 1));
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
        #endregion
    }
}
