namespace BreadcrumbTestLib.ViewModels.TreeSelectors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BreadcrumbTestLib.Utils;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbTestLib.ViewModels.TreeLookupProcessors;
    using BmLib.Utils;
    using System.Threading;
    using System.Diagnostics;
    using BmLib.Enums;

    /// <summary>
    /// Base class of ITreeSelector, which implements Tree
    /// based structure and supports LookupProcessing.
    /// </summary>
    /// <typeparam name="VM">Reference to a type of viewmodel</typeparam>
    /// <typeparam name="T">reference to a type of model that is required by the viewmodel</typeparam>
    internal class TreeSelectorViewModel<VM, T> : Base.ViewModelBase, ITreeSelector<VM, T>
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        /// <summary>
        /// Class constructor
        /// </summary>
        public TreeSelectorViewModel(T currentValue, VM currentViewModel,
                                     ITreeSelector<VM, T> parentSelector,
                                     IBreadcrumbTreeItemHelperViewModel<VM> entryHelper)
        {
            RootSelector = parentSelector.RootSelector;
            ParentSelector = parentSelector;
            EntryHelper = entryHelper;

            _currentValue = currentValue;
            _currentViewModel = currentViewModel;
        }

        /// <summary>
        /// Internal base classe constructor for inheriting classes.
        /// </summary>
        protected TreeSelectorViewModel(IBreadcrumbTreeItemHelperViewModel<VM> entryHelper)
        {
            EntryHelper = entryHelper;
            RootSelector = this as ITreeRootSelector<VM, T>;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Whether current view model is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                Logger.InfoFormat("_");

                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged(() => IsSelected);
                    SelectedChild = default(T);

                    if (value)
                        ReportChildSelected(new Stack<ITreeSelector<VM, T>>());
                    else
                        ReportChildDeselected(new Stack<ITreeSelector<VM, T>>());
                }
            }
        }

        /// <summary>
        /// This is marked by TreeRootSelector, for overflow menu support.
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return _isRoot;
            }

            set
            {
                Logger.InfoFormat("_");

                _isRoot = value;
                NotifyPropertyChanged(() => this.IsRoot);
                NotifyPropertyChanged(() => this.IsOverflowedOrRoot);
                NotifyPropertyChanged(() => this.IsRootAndIsChildSelected);
            }
        }

        /// <summary>
        /// Based on IsRoot and IsChildSelected
        /// </summary>
        public virtual bool IsRootAndIsChildSelected
        {
            get { return IsRoot && IsChildSelected; }
        }

        /// <summary>
        /// Gets whether a child of current view model is selected.
        /// </summary>
        public virtual bool IsChildSelected
        {
            get { return _selectedValue != null; }
        }

        /// <summary>
        /// Gets the selected child of current view model.          
        /// </summary>
        public T SelectedChild
        {
            get
            {
                return _selectedValue;
            }

            set
            {
                Logger.InfoFormat("_");
                _selectedValue = value;

                NotifyPropertyChanged(() => this.SelectedChild);
                NotifyPropertyChanged(() => this.SelectedChildUI);
                NotifyPropertyChanged(() => this.IsChildSelected);
                NotifyPropertyChanged(() => this.IsRootAndIsChildSelected);
            }
        }

        /// <summary>
        /// Lycj: Removed SetSelectedChild and SetIsSelected.
        /// 
        /// The selected child of current view model, for use in UI only.          
        /// </summary>
        public T SelectedChildUI
        {
            get
            {
                return _selectedValue;
            }

            set
            {
                Logger.InfoFormat("_");
                IsSelected = false;
                NotifyPropertyChanged(() => this.IsSelected);

                if (_selectedValue == null || !_selectedValue.Equals(value))
                {
                    if (_prevSelected != null)
                    {
                        _prevSelected.IsSelected = false;
                    }

                    SelectedChild = value;

                    if (value != null)
                    {
                        AsyncUtils.RunAsync(async () => await LookupAsync
                        (
                            value,
                            new SearchNextLevel<VM, T>(),    // LoadSubentriesIfNotLoaded
                            CancellationToken.None,
                            new TreeLookupProcessor<VM, T>(HierarchicalResult.Related, (hr, p, c) =>
                            {
                                c.IsSelected = true;
                                _prevSelected = c;

                                return true;
                            })));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the instance of the model object that represents this selection helper.
        /// The model backs the <see cref="ViewModel"/> property and should be in sync
        /// with it.
        /// </summary>
        public T Value
        {
            get { return _currentValue; }
        }

        /// <summary>
        /// Gets the owning ViewModel of this selection helper.
        /// </summary>
        public VM ViewModel
        {
            get { return _currentViewModel; }
        }

        /// <summary>
        /// Gets the parent's ViewModel <see cref="ITreeSelector"/>.
        /// </summary>
        public ITreeSelector<VM, T> ParentSelector { get; }

        /// <summary>
        /// Gets the root's ViewModel <see cref="ITreeSelector"/>.
        /// </summary>
        public ITreeRootSelector<VM, T> RootSelector { get; }

        /// <summary>
        /// Gets All sub-entries of the current tree item
        /// to support loading tree items.
        /// </summary>
        public IBreadcrumbTreeItemHelperViewModel<VM> EntryHelper { get; }

        /// <summary>
        /// Gets whether this entry is currently overflowed (should be hidden
        /// because its to large for display) or a root element, or both.
        /// 
        /// This can be used by binding system to determine whether an element should
        /// be visble in the root drop down list, because overflowed or root
        /// items should be visible in the root drop down list for
        /// overflowed and root items.
        /// </summary>
        public bool IsOverflowedOrRoot
        {
            get { return IsOverflowed || IsRoot; }
        }

        /// <summary>
        /// Gets/sets whether the BreadCrumb Tree item is currently overflowed
        /// (does not fit into the view display area) or not.
        /// </summary>
        public bool IsOverflowed
        {
            get
            {
                return _isOverflowed;
            }

            set
            {
                if (_isOverflowed != value)
                {
                    Logger.InfoFormat("--> Item '{0}' is Overflowed: {1} -> {2}", this, _isOverflowed, value);
                    Debug.WriteLine("--> Item '{0}' is Overflowed: {1} -> {2}", this, _isOverflowed, value);

                    _isOverflowed = value;
                    NotifyPropertyChanged(() => this.IsOverflowed);
                    NotifyPropertyChanged(() => this.IsOverflowedOrRoot);
                }
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Model '{0}', ViewModel '{1}'",
                _currentValue == null ? string.Empty : _currentValue.ToString(),
                _currentViewModel == null ? string.Empty : _currentViewModel.ToString());
        }

        /// <summary>
        /// Bubble up to TreeSelectionHelper for selection.
        /// </summary>
        /// <param name="path"></param>
        public virtual void ReportChildSelected(Stack<ITreeSelector<VM, T>> path)
        {
            Logger.InfoFormat("_");

            if (path.Count() > 0)
            {
                _selectedValue = path.Peek().Value;

                NotifyPropertyChanged(() => this.SelectedChild);
                NotifyPropertyChanged(() => this.SelectedChildUI);
            }

            path.Push(this);

            if (ParentSelector != null)
                ParentSelector.ReportChildSelected(path);
        }

        public virtual void ReportChildDeselected(Stack<ITreeSelector<VM, T>> path)
        {
            Logger.InfoFormat("_");

            if (EntryHelper.IsLoaded)
            {
                // Clear child node selection.
                SelectedChild = default(T);

                // And just in case if the new selected value is child of this node.
                if (RootSelector.SelectedValue != null)
                {
                    AsyncUtils.RunAsync(() => LookupAsync
                    (
                      RootSelector.SelectedValue,
                      new SearchNextUsingReverseLookup<VM, T>(RootSelector.SelectedSelector),
                      CancellationToken.None,
                      new TreeLookupProcessor<VM, T>(HierarchicalResult.All, (hr, p, c) =>
                      {
                          SelectedChild = c == null ? default(T) : c.Value;

                          return true;
                      })));
                }

                // SetSelectedChild(lookupResult == null ? default(T) : lookupResult.Value);
                NotifyPropertyChanged(() => this.IsChildSelected);
                NotifyPropertyChanged(() => this.SelectedChild);
                NotifyPropertyChanged(() => this.SelectedChildUI);
            }

            path.Push(this);

            if (ParentSelector != null)
                ParentSelector.ReportChildDeselected(path);
        }

        /// <summary>
        /// Tunnel down to select the specified item.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentAction"></param>
        /// <returns></returns>
        public async Task LookupAsync(T value,
                                      ITreeLookup<VM, T> lookupProc,
                                      CancellationToken cancelToken,
                                      params ITreeLookupProcessor<VM, T>[] processors)
        {
            Logger.InfoFormat("'{0}'", (value != null ? value.ToString() : "(null)"));

            using (await _lookupLock.LockAsync())
            {
                await lookupProc.LookupAsync(value, this, this.RootSelector, cancelToken, processors);
            }
        }
        #endregion
    }
}