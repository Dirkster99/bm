namespace Breadcrumb.ViewModels.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Breadcrumb.Defines;
	using Breadcrumb.Utils;
	using Breadcrumb.Viewmodels.Base;
	using Breadcrumb.ViewModels.Interfaces;

	public class EntriesHelperViewModel<VM> : NotifyPropertyChanged, IEntriesHelper<VM>
	{
		#region fields
		////private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<EntriesHelper<VM>>();

		protected Func<bool, object, Task<IEnumerable<VM>>> _loadSubEntryFunc;

		private readonly AsyncLock _loadingLock = new AsyncLock();
		private CancellationTokenSource _lastCancellationToken = new CancellationTokenSource();
		private bool _clearBeforeLoad = false;
		////private bool _isLoading = false;
		private bool _isLoaded = false;
		private bool _isExpanded = false;
		private bool _isLoading = false;
		private IEnumerable<VM> _subItemList = new List<VM>();
		private ObservableCollection<VM> _subItems;
		private DateTime _lastRefreshTimeUtc = DateTime.MinValue;
		#endregion fields

		#region constructors
		public EntriesHelperViewModel(Func<bool, object, Task<IEnumerable<VM>>> loadSubEntryFunc)
		{
			this._loadSubEntryFunc = loadSubEntryFunc;

			this.All = new FastObservableCollection<VM>();
			this.All.Add(default(VM));
		}

		public EntriesHelperViewModel(Func<bool, Task<IEnumerable<VM>>> loadSubEntryFunc)
			: this((b, __) => loadSubEntryFunc(b))
		{
		}

		public EntriesHelperViewModel(Func<Task<IEnumerable<VM>>> loadSubEntryFunc)
			: this(_ => loadSubEntryFunc())
		{
		}

		public EntriesHelperViewModel(params VM[] entries)
		{
			this._isLoaded = true;
			this.All = new FastObservableCollection<VM>();
			this._subItemList = entries;

			(this.All as FastObservableCollection<VM>).AddItems(entries);
			////foreach (var entry in entries)
			////    All.Add(entry);
		}
		#endregion constructors

		#region events
		public event EventHandler EntriesChanged;
		#endregion events

		#region properties
		public bool ClearBeforeLoad
		{
			get { return this._clearBeforeLoad; }
			set { this._clearBeforeLoad = value; }
		}

		public bool IsExpanded
		{
			get
			{
				return this._isExpanded;
			}

			set
			{
				if (value && !this._isExpanded)
					this.LoadAsync();

				this._isExpanded = value;
				this.NotifyOfPropertyChanged(() => this.IsExpanded);
			}
		}

		public bool IsLoaded
		{
			get
			{
				return this._isLoaded;
			}

			set
			{
				this._isLoaded = value;
				this.NotifyOfPropertyChanged(() => this.IsLoaded);
			}
		}

		public bool IsLoading
		{
			get
			{
				return this._isLoading;
			}

			set
			{
				this._isLoading = value;
				this.NotifyOfPropertyChanged(() => this.IsLoading);
			}
		}

		public DateTime LastRefreshTimeUtc
		{
			get
			{
				return this._lastRefreshTimeUtc;
			}
		}

		public IEnumerable<VM> AllNonBindable
		{
			get
			{
				return this._subItemList;
			}
		}

		public ObservableCollection<VM> All
		{
			get
			{
				return this._subItems;
			}

			private set
			{
				this._subItems = value;
			}
		}

		public AsyncLock LoadingLock
		{
			get
			{
				return this._loadingLock;
			}
		}
		#endregion properties

		#region methods
		public async Task UnloadAsync()
		{
			// Cancel previous load.
			this._lastCancellationToken.Cancel();

			using (var releaser = await this._loadingLock.LockAsync())
			{
				this._subItemList = new List<VM>();
				this.All.Clear();
				this._isLoaded = false;
			}
		}

		public async Task<IEnumerable<VM>> LoadAsync(UpdateMode updateMode = UpdateMode.Replace, bool force = false, object parameter = null)
		{
			if (this._loadSubEntryFunc != null) // Ignore if contructucted using entries but not entries func
			{
				this._lastCancellationToken.Cancel(); // Cancel previous load.

				using (var releaser = await this._loadingLock.LockAsync())
				{
					this._lastCancellationToken = new CancellationTokenSource();

					if (!this._isLoaded || force)
					{
						if (this._clearBeforeLoad)
							this.All.Clear();

						try
						{
							var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
							this.IsLoading = true;

							await this._loadSubEntryFunc(this._isLoaded, parameter).ContinueWith((prevTask, _) =>
							{
								this.IsLoaded = true;
								this.IsLoading = false;

								if (!prevTask.IsCanceled && !prevTask.IsFaulted)
								{
									this.SetEntries(updateMode, prevTask.Result.ToArray());
									this._lastRefreshTimeUtc = DateTime.UtcNow;
								}
							},
							
							this._lastCancellationToken, scheduler);
						}
						catch ////(InvalidOperationException ex)
						{
							////logger.Error("Cannot obtain SynchronizationContext", ex);
						}
					}
				}
			}

			return this._subItemList;
		}

		public void SetEntries(UpdateMode updateMode = UpdateMode.Replace, params VM[] viewModels)
		{
			switch (updateMode)
			{
				case UpdateMode.Update:
					this.updateEntries(viewModels);
				break;
				
				case UpdateMode.Replace:
					this.setEntries(viewModels);
				break;

				default:
					throw new NotSupportedException("UpdateMode");
			}
		}

		private void updateEntries(params VM[] viewModels)
		{
			FastObservableCollection<VM> all = this.All as FastObservableCollection<VM>;
			all.SuspendCollectionChangeNotification();

			var removeItems = all.Where(vm => !viewModels.Contains(vm)).ToList();
			var addItems = viewModels.Where(vm => !all.Contains(vm)).ToList();

			foreach (var vm in removeItems)
				all.Remove(vm);

			foreach (var vm in addItems)
				all.Add(vm);

			this._subItemList = all.ToArray().ToList();
			all.NotifyChanges();

			if (this.EntriesChanged != null)
				this.EntriesChanged(this, EventArgs.Empty);
		}

		private void setEntries(params VM[] viewModels)
		{
			this._subItemList = viewModels.ToList();
			FastObservableCollection<VM> all = this.All as FastObservableCollection<VM>;

			all.SuspendCollectionChangeNotification();
			all.Clear();
			all.NotifyChanges();
			all.AddItems(viewModels);
			all.NotifyChanges();

			if (this.EntriesChanged != null)
				this.EntriesChanged(this, EventArgs.Empty);
		}
		#endregion methods
	}
}
