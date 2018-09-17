namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbTestLib.ViewModels.TreeLookupProcessors;
    using BreadcrumbTestLib.ViewModels.TreeSelectors;
    using BmLib.Utils;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using BmLib.Enums;
    using System.Windows;

    internal class TreeRootSelectorViewModel<VM, T> : TreeSelectorViewModel<VM, T>, ITreeRootSelector<VM, T>
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected new static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private T _selectedValue = default(T);
        private ITreeSelector<VM, T> _selectedSelector;
        private Stack<ITreeSelector<VM, T>> _prevPath = null;
        private IEnumerable<ICompareHierarchy<T>> _comparers;
        private ObservableCollection<VM> _OverflowedAndRootItems = null;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="entryHelper"></param>
        public TreeRootSelectorViewModel(IBreadcrumbTreeItemHelperViewModel<VM> entryHelper) ////int rootLevel = 0,
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
        /// 
        /// Source:
        /// DropDownList Binding with SelectedValue="{Binding Selection.SelectedValue}"
        /// </summary>
        public T SelectedValue
        {
            get
            {
                return _selectedValue;
            }

            set
            {
                if (value == null && _selectedSelector == null)
                    return;

                if (value != null && _selectedSelector != null)
                {
                    if (value.Equals(_selectedSelector) == true)
                        return;
                }

                //// Refactored this into BreadcrumbViewModel.RootDropDownSelectionChangedCommand
                //// AsyncUtils.RunAsync(() => this.SelectAsync(value, null, CancellationToken.None));
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
        /// <summary>
        /// Method is invoked via IsSelected property in TreeSelectorViewModel when
        /// user clicks on one Breadcrumb and control navigates back to the given location.
        /// 
        ///  Sample Use Case: Breadcrumb shows 'C:'> 'Windows'> 'System32'> and user clicks on 'Windows'
        /// Expectedt Result: Breadcrumb shows 'C:'> 'Windows'>
        /// 
        /// This behavior is mostly implemented in this method.
        /// </summary>
        /// <param name="path"></param>
        public override async Task ReportChildSelectedAsync(Stack<ITreeSelector<VM, T>> path)
        {
            Logger.InfoFormat("_");

            ITreeSelector<VM, T> prevSelector = _selectedSelector;

            T prevSelectedValue = _selectedValue;
            _prevPath = path;

            _selectedSelector = path.Last();
            _selectedValue = path.Last().Value;

            if (prevSelectedValue != null && prevSelectedValue.Equals(path.Last().Value) == false)
            {
                prevSelector.IsSelected = false;
            }

            this.NotifyPropertyChanged(() => this.SelectedValue);
            this.NotifyPropertyChanged(() => this.SelectedViewModel);

            if (this.SelectionChanged != null)
                this.SelectionChanged(this, EventArgs.Empty);

            this.updateRootItems(path);

            await path.Last().EntryHelper.LoadAsync();
        }

        public HierarchicalResult CompareHierarchy(T value1, T value2)
        {
            Logger.InfoFormat("_");

            foreach (var c in this.Comparers)
            {
                var retVal = c.CompareHierarchy(value1, value2);
                if (retVal != HierarchicalResult.Unrelated)
                    return retVal;
            }
            return HierarchicalResult.Unrelated;
        }

        /// <summary>
        /// Update the root drop down list with the list of root items
        /// and overflowable (non-root) items.
        /// </summary>
        /// <param name="rootItems"></param>
        /// <param name="pathItems"></param>
        public void UpdateOverflowedItems(IEnumerable<VM> rootItems,
                                          IEnumerable<VM> pathItems)
        {
            Logger.InfoFormat("_");

            Application.Current.Dispatcher.Invoke(() =>
            {
                _OverflowedAndRootItems.Clear();
            });

            if (pathItems != null && pathItems.Count() > 0)
            {
                // Get all items that belong to the current path and add them into the
                // OverflowedAndRootItems collection
                // The item.IsOverflowed property is (re)-set by OverflowableStackPanel
                // when the UI changes - a converter in the binding shows only those entries
                // in the root drop down list with item.IsOverflowed == true
                foreach (var p in pathItems)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _OverflowedAndRootItems.Add(p);
                    });
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _OverflowedAndRootItems.Add(default(VM)); // Separator
                });

                foreach (var p in rootItems)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _OverflowedAndRootItems.Add(p);
                    });
                }
            }
        }

        /// <summary>
        /// Method is executed when the control navigates to a new location -
        /// it re-computes the rootitems along with the overflowed items displayed
        /// in the root items drop down collection.
        /// </summary>
        /// <param name="path"></param>
        private void updateRootItems(Stack<ITreeSelector<VM, T>> path)
        {
            Logger.InfoFormat("_");

            Application.Current.Dispatcher.Invoke(() =>
            {
                _OverflowedAndRootItems.Clear();
            });

            if (path != null && path.Count() > 0)
            {
                // Get all items that belong to the current path and add them into the
                // OverflowedAndRootItems collection
                // The item.IsOverflowed property is (re)-set by OverflowableStackPanel
                // when the UI changes - a converter in the binding shows only those entries
                // in the root drop down list with item.IsOverflowed == true
                foreach (var p in path.Reverse())
                {
                    if (!(this.EntryHelper.All.Contains(p.ViewModel)))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _OverflowedAndRootItems.Add(p.ViewModel);
                        });
                    }
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _OverflowedAndRootItems.Add(default(VM)); // Separator
                });
            }

////            Console.WriteLine();
////            Console.WriteLine("Adding Root Items:");

            // Get all items for display in the root drop down list
            AsyncUtils.RunAsync(() => this.updateRootItemsAsync(this, _OverflowedAndRootItems, 1));
        }

        private async Task updateRootItemsAsync(ITreeSelector<VM, T> selector,
                                                ObservableCollection<VM> rootItems,
                                                int level)
        {
            Logger.InfoFormat("_");

            if (level == 0)
                return;

            List<ITreeSelector<VM, T>> rootTreeSelectors = new List<ITreeSelector<VM, T>>();

            // Perform a lookup and for all directories in next level of current directory (load asynchronously if not loaded), 
            // add directories's Selector to rootTreeSelectors.
            await selector.LookupAsync(default(T),
                                       new BroadcastNextLevel<VM, T>(),    // LoadSubentriesIfNotLoaded
                                       CancellationToken.None,
                                       new TreeLookupProcessor<VM, T>(HierarchicalResult.All, (hr, p, c) =>
                                       {
                                           rootTreeSelectors.Add(c);
                                           return true;
                                       }));

            // Then foreach rootTreeSelectors, add to rootItems and preferm updateRootItemAsync.
            foreach (var c in rootTreeSelectors)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    rootItems.Add(c.ViewModel);
                });

////                Console.WriteLine("Level {0} Root item: {1}", level, c.Value.ToString());
                c.IsRoot = true;

                await this.updateRootItemsAsync(c, rootItems, level - 1);
            }
        }
        #endregion
    }
}
