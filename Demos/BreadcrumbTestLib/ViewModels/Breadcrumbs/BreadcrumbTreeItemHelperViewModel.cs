namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.Utils;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using BmLib.Enums;
    using System.Windows;

    /// <summary>
    /// Implements the viewmodel template that drives every BreadcrumbTreeItem control
    /// of the root item of a BreadcrumbTree control.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    internal class BreadcrumbTreeItemHelperViewModel<VM> : Base.ViewModelBase, IBreadcrumbTreeItemHelperViewModel<VM>
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected Func<bool, object, Task<IEnumerable<VM>>> _loadSubEntryFunc;

        private bool _clearBeforeLoad = false;

        private bool _isLoaded = false;
        private bool _isExpanded = false;
        private bool _isLoading = false;
        private DateTime _lastRefreshTimeUtc = DateTime.MinValue;

        private CancellationTokenSource _lastCancellationToken = new CancellationTokenSource();

        private readonly FastObservableCollection<VM> _All;
        private readonly AsyncLock _loadingLock = new AsyncLock();
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="loadSubEntryFunc"></param>
        public BreadcrumbTreeItemHelperViewModel(Func<bool, object, Task<IEnumerable<VM>>> loadSubEntryFunc)
            : this()
        {
            _loadSubEntryFunc = loadSubEntryFunc;

            _All = new FastObservableCollection<VM>
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
            : this()
        {
            _isLoaded = true;
            _All.AddItems(entries);
        }

        /// <summary>
        /// Hidden standard class constructor
        /// </summary>
        protected BreadcrumbTreeItemHelperViewModel()
        {
            _All = new FastObservableCollection<VM>();
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
        /// Gets/sets whether a breadcrumb drop down entry is
        /// 1) Expanded - Drop Down List is open or
        /// 2) not.
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
                    try
                    {
                        Logger.InfoFormat("{0} -> {1}", value, _isExpanded);
                        ////                        Console.WriteLine("{0}: {1} -> {2}", this.ToString(), value, _isExpanded);

                        ////if (value && _isExpanded == false)
                        ////    AsyncUtils.RunAsync(() => this.LoadAsync());
                    }
                    catch (Exception exc)
                    {
                        Logger.Error(exc);
                    }

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

        /// <summary>
        /// Gets a list of sub-entries that should be displayed below this entry
        /// in a tree view like display of path items.
        /// </summary>
        public IEnumerable<VM> All
        {
            get
            {
                return _All;
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
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _All.Clear();
                            });
                        }

                        await Application.Current.Dispatcher.Invoke(async () =>
                        {
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
                        });
                    }
                }
            }

            return _All;
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
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _All.Clear();
                });
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
        /// with the entries in the <param name="viewModels"/> array parameter.
        /// </summary>
        /// <param name="viewModels"></param>
        private void UpdateAllEntries(params VM[] viewModels)
        {
            Logger.InfoFormat("_");

            _All.SuspendCollectionChangeNotification();
            try
            {
                var removeItems = _All.Where(vm => !viewModels.Contains(vm)).ToList();
                var addItems = viewModels.Where(vm => !_All.Contains(vm)).ToList();

                foreach (var vm in removeItems)
                    _All.Remove(vm);

                foreach (var vm in addItems)
                    _All.Add(vm);
            }
            finally
            {
                _All.NotifyChanges();

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
