namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.ViewModels.Base;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using DirectoryInfoExLib.Interfaces;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Class implements the viewmodel that manages the complete breadcrump control.
    /// </summary>
    internal class BreadcrumbViewModel : Base.ViewModelBase, IBreadcrumbViewModel
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool _EnableBreadcrumb;
        private string _suggestedPath;

        private ICommand _RootDropDownSelectionChangedCommand;
        private bool _IsBrowsing;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public BreadcrumbViewModel()
        {
            Progressing = new ProgressViewModel();
            BreadcrumbSubTree = new BreadcrumbTreeItemViewModel();
            _EnableBreadcrumb = true;
            _IsBrowsing = false;
        }
        #endregion constructors

        #region browsing events
        /// <summary>
        /// Indicates when the viewmodel starts heading off somewhere else
        /// and when its done browsing to a new location.
        /// </summary>
        public event EventHandler<BrowsingEventArgs> BrowseEvent;
        #endregion browsing events

        #region properties
        /// <summary>
        /// Gets a viewmodel that manages the progress display that is shown to inform
        /// users of long running processings.
        /// </summary>
        public IProgressViewModel Progressing { get; private set; }

        /// <summary>
        /// Gets a viewmodel that manages the sub-tree brwosing and
        /// selection within the sub-tree component
        /// </summary>
        public BreadcrumbTreeItemViewModel BreadcrumbSubTree { get; }

        /// <summary>
        /// Gets/sets a property that determines whether a breadcrumb
        /// switch is turned on or off.
        /// 
        /// On false: A Breadcrumb switch turned off shows the text editable path
        ///  On true: A Breadcrumb switch turned  on shows the BreadcrumbSubTree for browsing
        /// </summary>
        public bool EnableBreadcrumb
        {
            get
            {
                return _EnableBreadcrumb;
            }

            set
            {
                if (_EnableBreadcrumb != value)
                {
                    _EnableBreadcrumb = value;
                    NotifyPropertyChanged(() => EnableBreadcrumb);
                }
            }
        }

        public string SuggestedPath
        {
            get { return _suggestedPath; }
            set
            {
                _suggestedPath = value;

                NotifyPropertyChanged(() => SuggestedPath);
                OnSuggestPathChanged();
            }
        }

        ////        /// <summary>
        ////        /// Contains a list of items that maps into the SuggestBox control.
        ////        /// </summary>
        ////        public IEnumerable<ISuggestSource> SuggestSources
        ////        {
        ////            get
        ////            {
        ////                return _suggestSources;
        ////            }
        ////            set
        ////            {
        ////                _suggestSources = value;
        ////                NotifyOfPropertyChange(() => SuggestSources);
        ////            }
        ////        }

        /// <summary>
        /// Gets a command that can change the navigation target of the currently
        /// selected location towards a new location.
        /// 
        /// Expected command parameter:
        /// Array of length 1 with an object of type <see cref="BreadcrumbTreeItemViewModel"/>
        /// object[1] = {new <see cref="BreadcrumbTreeItemViewModel"/>() }
        /// </summary>
        public ICommand RootDropDownSelectionChangedCommand
            {
                get
                {
                    if (_RootDropDownSelectionChangedCommand == null)
                    {
                        _RootDropDownSelectionChangedCommand = new RelayCommand<object>(async (p) =>
                        {
                            var parArray = p as object[];
                            if (parArray == null)
                                return;

                            if (parArray.Length <= 0)
                                return;

                            // Limitation of command is currently only 1 LOCATION PARAMETER being processed
                            var selectedFolder = parArray[0] as BreadcrumbTreeItemViewModel;

                            if (selectedFolder == null)
                                return;

                            var model = selectedFolder.GetModel();

                            await this.NavigateTo(model);
                        });
                    }

                return _RootDropDownSelectionChangedCommand;
            }
        }

        /// <summary>
        /// Gets whether the browser is currently processing
        /// a request for brwosing to a known location.
        /// 
        /// Can only be set by the control if user started browser process
        /// 
        /// Use IsBrowsing and IsExternallyBrowsing to lock the controls UI
        /// during browse operations or display appropriate progress bar(s).
        /// </summary>
        public bool IsBrowsing
        {
            get
            {
                return _IsBrowsing;
            }

            private set
            {
                if (_IsBrowsing != value)
                {
                    _IsBrowsing = value;
                    NotifyPropertyChanged(() => IsBrowsing);
                }
            }
        }

        /// <summary>
        /// Navigates the viewmodel (and hopefully the bound control) to a new location
        /// and ensures correct <see cref="IsBrowsing"/> state and event handling towards
        /// listing objects for
        /// <see cref="ICanNavigate"/> events.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private async Task NavigateTo(IDirectoryBrowser location)
        {
            IsBrowsing = true;
            try
            {
                var root = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
                if (root == null)
                    return;

                string[] pathSegments = DirectoryInfoExLib.Factory.GetFolderSegments(location.FullName);
                var request = new BrowseRequest<string>(location.FullName, pathSegments, CancellationToken.None);

                var selector = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
                await selector.SelectAsync(location, request, CancellationToken.None, Progressing);

                try
                {
                    if (BrowseEvent != null)
                        BrowseEvent(this, new BrowsingEventArgs(location, false, BrowseResult.Complete));
                }
                catch
                {
                }
            }
            finally
            {
                IsBrowsing = false;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Method should be called after construction to initialize the viewmodel
        /// to view a default content.
        /// </summary>
        /// <param name="initialRequest"></param>
        public async Task<FinalBrowseResult<IDirectoryBrowser>> InitPathAsync(
            BrowseRequest<string> initialRequest)
        {
            Logger.InfoFormat("'{0}'", initialRequest.NewLocation);

            return await Task.Run(() =>
            {
                return BreadcrumbSubTree.InitRootAsync(initialRequest, Progressing);
            });
        }

        /// <summary>
        /// Method executes when the text path portion in the
        /// Breadcrumb control has been edit.
        /// </summary>
        private void OnSuggestPathChanged()
        {
            Logger.InfoFormat("_");

            /***
            if (!ShowBreadcrumb)
            {
              Task.Run(async () =>
              {
                foreach (var p in _profiles)
                  if (p.MatchPathPattern(SuggestedPath))
                  {
                    if (String.IsNullOrEmpty(SuggestedPath) && Entries.AllNonBindable.Count() > 0)
                      SuggestedPath = Entries.AllNonBindable.First().EntryModel.FullPath;

                    var found = await p.ParseThenLookupAsync(SuggestedPath, CancellationToken.None);
                    if (found != null)
                    {
                      _sbox.Dispatcher.BeginInvoke(new System.Action(() => { SelectAsync(found); }));
                      ShowBreadcrumb = true;
                      BroadcastDirectoryChanged(EntryViewModel.FromEntryModel(found));
                    }
                    //else not found
                  }
              });//.Start();
            }
             ***/
        }
        #endregion methods
    }
}