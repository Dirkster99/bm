namespace BreadcrumbTestLib.ViewModels.TreeSelectors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BreadcrumbTestLib.Utils;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbTestLib.ViewModels.TreeLookupProcessors;
    using BreadcrumbLib.Utils;
    using System.Threading;
    using System.Windows;
    using System.Diagnostics;
    using BreadcrumbLib.Enums;

    /// <summary>
    /// Base class of ITreeSelector, which implements Tree
    /// based structure and supports LookupProcessing.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    internal class TreeSelectorViewModel<VM, T> : Base.ViewModelBase, ITreeSelector<VM, T>
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
        /// <summary>
        /// Class constructor
        /// </summary>
        public TreeSelectorViewModel(T currentValue, VM currentViewModel,
                                     ITreeSelector<VM, T> parentSelector,
                                     IEntriesHelper<VM> entryHelper)
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
        protected TreeSelectorViewModel(IEntriesHelper<VM> entryHelper)
        {
            EntryHelper = entryHelper;
            RootSelector = this as ITreeRootSelector<VM, T>;
        }
        #endregion

        #region Public Properties
        public T Value
        {
            get { return _currentValue; }
        }

        public VM ViewModel
        {
            get { return _currentViewModel; }
        }

        public ITreeSelector<VM, T> ParentSelector { get; private set; }

        public ITreeRootSelector<VM, T> RootSelector { get; private set; }

        public IEntriesHelper<VM> EntryHelper { get; private set; }

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
                    SelectedChild = default(T);

                    if (value)
                        ReportChildSelected(new Stack<ITreeSelector<VM, T>>());
                    else
                        ReportChildDeselected(new Stack<ITreeSelector<VM, T>>());
                }
            }
        }

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

        public virtual bool IsChildSelected
        {
            get { return _selectedValue != null; }
        }

        public virtual bool IsRootAndIsChildSelected
        {
            get { return IsRoot && IsChildSelected; }
        }

        public T SelectedChild
        {
            get
            {
                return _selectedValue;
            }

            set
            {
                _selectedValue = value;

                NotifyPropertyChanged(() => this.SelectedChild);
                NotifyPropertyChanged(() => this.SelectedChildUI);
                NotifyPropertyChanged(() => this.IsChildSelected);
                NotifyPropertyChanged(() => this.IsRootAndIsChildSelected);
            }
        }

        public T SelectedChildUI
        {
            get
            {
                return _selectedValue;
            }

            set
            {
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
                            SearchNextLevel<VM, T>.LoadSubentriesIfNotLoaded,
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
                    if (value == true)
                        Debug.WriteLine("--> Item is Overflowed: " + this);

                    _isOverflowed = value;
                    NotifyPropertyChanged(() => this.IsOverflowed);
                    NotifyPropertyChanged(() => this.IsOverflowedOrRoot);
                }
            }
        }
        #endregion

        #region methods
        public override string ToString()
        {
            return _currentValue == null ? string.Empty : _currentValue.ToString();
        }

        /// <summary>
        /// Bubble up to TreeSelectionHelper for selection.
        /// </summary>
        /// <param name="path"></param>
        public virtual void ReportChildSelected(Stack<ITreeSelector<VM, T>> path)
        {
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
            using (await _lookupLock.LockAsync())
            {
                await Application.Current.Dispatcher.Invoke(async () =>
                {
                    await lookupProc.LookupAsync(value, this, this.RootSelector, cancelToken, processors);
                });
            }
        }
        #endregion
    }
}