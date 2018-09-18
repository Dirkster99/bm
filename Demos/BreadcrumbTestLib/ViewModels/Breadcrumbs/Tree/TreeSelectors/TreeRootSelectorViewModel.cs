namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbTestLib.ViewModels.TreeSelectors;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
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
        public TreeRootSelectorViewModel(IBreadcrumbTreeItemHelperViewModel<VM> entryHelper,
                                         IEnumerable<ICompareHierarchy<T>> inComparers)
          : base(entryHelper)
        {
            _OverflowedAndRootItems = new ObservableCollection<VM>();
            Comparers = inComparers;
        }
        #endregion constructors

        #region events
        /// <summary>
        /// Raises an event whenever the currently selected item has changed.
        /// </summary>
        public event EventHandler SelectionChanged;
        #endregion events

        #region properties
        /// <summary>
        /// Gets a list of viewmodel items that are shown at the root drop down
        /// list of the control (left most drop down list)
        /// </summary>
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

        /// <summary>
        /// Gets the currently selected viewmodel from the currently selected item.
        /// </summary>
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
            }
        }

        /// <summary>
        /// Gets the hierarichal comparer that can be used to determine
        /// the relation of two items in the tree structure.
        /// </summary>
        public IEnumerable<ICompareHierarchy<T>> Comparers
        {
            get
            {
                return _comparers;
            }

            protected set
            {
                _comparers = value;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Method is invoked at the end of each navigational change when the
        /// control's viewmodel has retrieved all required items and is about
        /// to change the selection to complete the cycle.
        /// </summary>
        /// <param name="path"></param>
        public override async Task ReportChildSelectedAsync(Stack<ITreeSelector<VM, T>> path)
        {
            Logger.InfoFormat("_");

            try
            {
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
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }

            if (this.SelectionChanged != null)
                this.SelectionChanged(this, EventArgs.Empty);

            await path.Last().EntryHelper.LoadAsync();
        }

        /// <summary>
        /// Method checks value1 and value2 with a set of comparers
        /// as stored in the <see cref="Comparers"/> property and
        /// returns when a relation is found.
        /// 
        /// The method cycles otherwise through all comparers and returns
        /// unrelated if no other relation was determined.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
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
                _OverflowedAndRootItems.Add(default(VM)); // Insert Separator between Root and Overflowed Items
            });

            foreach (var p in rootItems)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _OverflowedAndRootItems.Add(p);
                });
            }
        }
        #endregion
    }
}
