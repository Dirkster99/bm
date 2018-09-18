namespace BreadcrumbTestLib.ViewModels.TreeSelectors
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BreadcrumbTestLib.Utils;
    using BreadcrumbTestLib.ViewModels.Interfaces;

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
        private bool _isSelected = false;

        // Holds the location model of the selected child of this entry (if any)
        private T _SelectedChild = default(T);

        private T _Value = default(T);
        private VM _ViewModel;

        private bool _isRoot = false;
        private bool _isOverflowed;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="value">Is the location model object that is represented by the viewmodel object.</param>
        /// <param name="viewModel">Is the viewmodel object that represents an item in the viewmodel tree structure.</param>
        /// <param name="parentSelector"></param>
        /// <param name="entryHelper"></param>
        public TreeSelectorViewModel(T value,
                                     VM viewModel,
                                     ITreeSelector<VM, T> parentSelector,
                                     IBreadcrumbTreeItemHelperViewModel<VM> entryHelper)
        {
            _Value = value;
            _ViewModel = viewModel;

            RootSelector = parentSelector.RootSelector;
            ParentSelector = parentSelector;

            EntryHelper = entryHelper;
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
        /// Gets/sets whether current view model is selected or not.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged(() => IsSelected);
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
            get { return _SelectedChild != null; }
        }

        /// <summary>
        /// Gets the selected child of current view model.          
        /// </summary>
        public T SelectedChild
        {
            get
            {
                return _SelectedChild;
            }

            set
            {
                _SelectedChild = value;

                NotifyPropertyChanged(() => this.SelectedChild);
                NotifyPropertyChanged(() => this.IsChildSelected);
                NotifyPropertyChanged(() => this.IsRootAndIsChildSelected);
            }
        }

        /// <summary>
        /// Gets the instance of the location model object that represents this selection helper.
        /// The model backs the <see cref="ViewModel"/> property and should be in sync
        /// with it.
        /// </summary>
        public T Value
        {
            get { return _Value; }
        }

        /// <summary>
        /// Gets the owning ViewModel of this selection helper.
        /// </summary>
        public VM ViewModel
        {
            get { return _ViewModel; }
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
                    ////Debug.WriteLine("--> Item '{0}' is Overflowed: {1} -> {2}", this, _isOverflowed, value);

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
                _Value == null ? string.Empty : _Value.ToString(),
                _ViewModel == null ? string.Empty : _ViewModel.ToString());
        }

        /// <summary>
        /// Bubble up to TreeSelectionHelper for selection.
        /// </summary>
        /// <param name="path"></param>
        public virtual Task ReportChildSelectedAsync(Stack<ITreeSelector<VM, T>> path)
        {
            Logger.InfoFormat("_");

            return Task.Run(() => { });
        }
        #endregion
    }
}