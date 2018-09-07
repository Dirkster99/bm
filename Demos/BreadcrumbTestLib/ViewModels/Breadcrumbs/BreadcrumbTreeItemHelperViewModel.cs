namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.Utils;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using BmLib.Enums;

    /// <summary>
    /// Implements the viewmodel template that drives every BreadcrumbTreeItem control
    /// of the root item of a BreadcrumbTree control.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    internal class BreadcrumbTreeItemHelperViewModel<VM> : Base.ViewModelBase, IBreadcrumbTreeItemViewModel<VM>
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected Func<bool, object, Task<IEnumerable<VM>>> _loadSubEntryFunc;

        private readonly AsyncLock _loadingLock = new AsyncLock();
        private CancellationTokenSource _lastCancellationToken = new CancellationTokenSource();
        private bool _clearBeforeLoad = false;
        ////private bool _isLoading = false;
        private bool _isLoaded = false;
        private bool _isExpanded = false;
        private bool _isLoading = false;
        private IEnumerable<VM> _subItemList = new List<VM>();
        private ObservableCollection<VM> _All;                      // _subItems
        private DateTime _lastRefreshTimeUtc = DateTime.MinValue;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="loadSubEntryFunc"></param>
        public BreadcrumbTreeItemHelperViewModel(Func<bool, object, Task<IEnumerable<VM>>> loadSubEntryFunc)
        {
            _loadSubEntryFunc = loadSubEntryFunc;

            All = new FastObservableCollection<VM>
            {
                default(VM)
            };

            this.CancelCommand = new RelayCommand(obj =>
            {
                var token = _lastCancellationToken;

                if (token != null)
                    token.Cancel();
            });
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="loadSubEntryFunc"></param>
        public BreadcrumbTreeItemHelperViewModel(Func<bool, Task<IEnumerable<VM>>> loadSubEntryFunc)
          : this((b, __) => loadSubEntryFunc(b))
        {
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="loadSubEntryFunc"></param>
        public BreadcrumbTreeItemHelperViewModel(Func<Task<IEnumerable<VM>>> loadSubEntryFunc)
          : this(_ => loadSubEntryFunc())
        {
        }

        /// <summary>
        /// Class constructor from entries parameters.
        /// </summary>
        /// <param name="entries"></param>
        public BreadcrumbTreeItemHelperViewModel(params VM[] entries)
        {
            _isLoaded = true;
            All = new FastObservableCollection<VM>();
            _subItemList = entries;

            (this.All as FastObservableCollection<VM>).AddItems(entries);
            ////foreach (var entry in entries)
            ////    All.Add(entry);
        }
        #endregion constructors

        #region events
        /// <summary>
        /// Implements an event that is fired when the ALL items collection has been changed.
        /// </summary>
        public event EventHandler EntriesChanged;
        #endregion events

        #region properties
        /// <summary>
        /// Gets/sets whether the entries in the All property should be
        /// reset before loading new entries or not.
        /// </summary>
        public bool ClearBeforeLoad
        {
            get { return _clearBeforeLoad; }
            set { _clearBeforeLoad = value; }
        }

        /// <summary>
        /// Gets/sets whether a breadcrumb entry is expanded or not.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }

            set
            {
                if (_isExpanded != value)
                {
                    ////if (value && !_isExpanded)
                    ////  AsyncUtils.RunAsync(() => this.LoadAsync());

                    _isExpanded = value;
                    NotifyPropertyChanged(() => IsExpanded);
                }
            }
        }

        /// <summary>
        /// Gets whether current items are already loaded or not.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }

            private set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    NotifyPropertyChanged(() => IsLoaded);
                }
            }
        }

        /// <summary>
        /// Gets whether the control is currently loading items or not.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }

            private set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyPropertyChanged(() => IsLoading);
                }
            }
        }

        /// <summary>
        /// Gets the command that can cancel the current LoadASync() operation.
        /// </summary>
        public ICommand CancelCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Coordinated Universal Time (UTC) of the last load processing
        /// at the ALL/AllNonBindable items collection.
        /// </summary>
        public DateTime LastRefreshTimeUtc
        {
            get
            {
                return _lastRefreshTimeUtc;
            }
        }

        public IEnumerable<VM> AllNonBindable
        {
            get
            {
                return _subItemList;
            }
        }

        public ObservableCollection<VM> All
        {
            get
            {
                return _All;
            }

            private set
            {
                _All = value;
            }
        }

        public AsyncLock LoadingLock
        {
            get
            {
                return _loadingLock;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Call to load sub-entries.
        /// </summary>
        /// <param name="force">Load sub-entries even if it's already loaded.</param>
        /// <returns></returns>
        public async Task<IEnumerable<VM>> LoadAsync(UpdateMode updateMode = UpdateMode.Replace,
                                                     bool force = false,
                                                     object parameter = null)
        {
            Logger.InfoFormat("_");

            // Ignore if constructed using entries but not entries func
            if (_loadSubEntryFunc != null)
            {
                _lastCancellationToken.Cancel(); // Cancel previous load.

                using (var releaser = await _loadingLock.LockAsync())
                {
                    _lastCancellationToken = new CancellationTokenSource();

                    if (!_isLoaded || force)
                    {
                        if (_clearBeforeLoad)
                            this.All.Clear();

                        try
                        {
                            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                            IsLoading = true;

                            await _loadSubEntryFunc(_isLoaded, parameter).ContinueWith((prevTask, _) =>
                            {
                                IsLoaded = true;
                                IsLoading = false;

                                if (!prevTask.IsCanceled && !prevTask.IsFaulted)
                                {
                                    this.SetEntries(updateMode, prevTask.Result.ToArray());
                                    _lastRefreshTimeUtc = DateTime.UtcNow;
                                }
                            },

                            _lastCancellationToken, scheduler);
                        }
                        catch (InvalidOperationException ex)
                        {
                            Logger.Error("Cannot obtain SynchronizationContext", ex);
                        }
                    }
                }
            }

            return _subItemList;
        }

        /// <summary>
        /// Call to unload sub-entries.
        /// Method can also be used to cancel current load processings.
        /// </summary>
        /// <returns></returns>
        public async Task UnloadAsync()
        {
            Logger.InfoFormat("_");

            // Cancel previous load.
            _lastCancellationToken.Cancel();

            using (var releaser = await _loadingLock.LockAsync())
            {
                _subItemList = new List<VM>();
                this.All.Clear();
                _isLoaded = false;
            }
        }

        /// <summary>
        /// Updates or resets the entries in the ALL items collection
        /// with the entries in the <param name="viewModels"/> parameter.
        /// </summary>
        /// <param name="updateMode">Specifies whether the ALL items collection is updated or reset.</param>
        /// <param name="viewModels"></param>
        public void SetEntries(UpdateMode updateMode = UpdateMode.Replace,
                               params VM[] viewModels)
        {
            Logger.InfoFormat("_");

            switch (updateMode)
            {
                case UpdateMode.Update:
                    UpdateAllEntries(viewModels);
                    break;

                case UpdateMode.Replace:
                    ResetAllEntries(viewModels);
                    break;

                default:
                    throw new NotSupportedException("UpdateMode");
            }
        }

        /// <summary>
        /// Updates the entries (via Remove and Add) in the ALL items collection
        /// with the entries in the <param name="viewModels"/> parameter.
        /// </summary>
        /// <param name="viewModels"></param>
        private void UpdateAllEntries(params VM[] viewModels)
        {
            Logger.InfoFormat("_");

            FastObservableCollection<VM> all = this.All as FastObservableCollection<VM>;
            all.SuspendCollectionChangeNotification();
            try
            {
                var removeItems = all.Where(vm => !viewModels.Contains(vm)).ToList();
                var addItems = viewModels.Where(vm => !all.Contains(vm)).ToList();

                foreach (var vm in removeItems)
                    all.Remove(vm);

                foreach (var vm in addItems)
                    all.Add(vm);

                _subItemList = all.ToArray().ToList();
            }
            finally
            {
                all.NotifyChanges();

                if (this.EntriesChanged != null)
                    this.EntriesChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Resets the entries (via Clear and Add) in the ALL items collection
        /// with the entries in the <param name="viewModels"/> parameter.
        /// </summary>
        /// <param name="viewModels"></param>
        private void ResetAllEntries(params VM[] viewModels)
        {
            Logger.InfoFormat("_");

            _subItemList = viewModels.ToList();
            FastObservableCollection<VM> all = this.All as FastObservableCollection<VM>;

            all.SuspendCollectionChangeNotification();
            try
            {
                all.Clear();
                all.NotifyChanges();
                all.AddItems(viewModels);
            }
            finally
            {
                all.NotifyChanges();

                if (this.EntriesChanged != null)
                    this.EntriesChanged(this, EventArgs.Empty);
            }
        }
        #endregion methods
    }
}
