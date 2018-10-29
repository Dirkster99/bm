namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbTestLib.ViewModels.Breadcrumbs.TreeSelectors;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Implemented in tree node view model, to provide selection support
    /// for the list of root drop down items.
    /// </summary>
    /// <typeparam name="VM">Sub-node viewmodel type.</typeparam>
    /// <typeparam name="M">Type to identify a node, commonly string.</typeparam>
    internal class TreeRootSelectorViewModel<VM, M> : TreeSelectorViewModel<VM, M>, ITreeRootSelector<VM, M>
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected new static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private M _selectedValue = default(M);
        private ITreeSelector<VM, M> _selectedSelector;
        private Stack<ITreeSelector<VM, M>> _prevPath = null;
        private ObservableCollection<VM> _OverflowedAndRootItems = null;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="entryHelper"></param>
        public TreeRootSelectorViewModel(IBreadcrumbTreeItemHelperViewModel<VM> entryHelper)
          : base(entryHelper)
        {
            this._OverflowedAndRootItems = new ObservableCollection<VM>();
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

        /// <summary>
        /// Gets the last item in the path of viewmodel items.
        /// 
        /// Example path is: 'This PC', 'C:', 'Program Files'
        /// -> This property should reference the 'Program Files' item.
        /// </summary>
        public ITreeSelector<VM, M> SelectedSelector
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
        /// Gets/sets the selected second level root item (eg. This PC, Library Desktop, or Desktop Folder).
        /// below the root desktop item.
        /// 
        /// This property usually changes:
        /// 1) When the user opens the drop down and selects 1 item in the dropdownlist of the RootDropDown button or
        /// 2) When the control navigates to a unrelated second level root address
        ///    (eg.: From 'This PC','C:\' to 'Libraries','Music')
        /// 
        /// Source:
        /// DropDownList Binding with SelectedValue="{Binding Selection.SelectedValue}"
        /// </summary>
        public M SelectedValue
        {
            get
            {
                return _selectedValue;
            }

            set
            {
                bool bHasChanged = true;

                if (_selectedValue == null && value == null)
                    bHasChanged = false;
                else
                {
                    if ((_selectedValue != null && value == null) ||
                        (_selectedValue == null && value != null))
                        bHasChanged = true;
                    else
                    {
                        bHasChanged = ! _selectedValue.Equals(value);
                    }
                }

                if (bHasChanged == true)
                {
                    _selectedValue = value;
                    NotifyPropertyChanged(() => this.SelectedValue);
                }
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
        public override async Task ReportChildSelectedAsync(Stack<ITreeSelector<VM, M>> path)
        {
            Logger.InfoFormat("_");

            try
            {
                ITreeSelector<VM, M> prevSelector = _selectedSelector;

                M prevSelectedValue = _selectedValue;
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

            if (path.Last().EntryHelper.IsLoaded == false)
                await path.Last().EntryHelper.LoadAsync();
        }
/***
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
***/
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
