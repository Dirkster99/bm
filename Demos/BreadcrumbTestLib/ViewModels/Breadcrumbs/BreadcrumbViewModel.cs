namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using DirectoryInfoExLib.Interfaces;
    using System.Threading.Tasks;

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
        private bool _IsRootOverflowed;
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
            _IsRootOverflowed = false;
        }
        #endregion constructors

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
                    Logger.InfoFormat("_");

                    _EnableBreadcrumb = value;
                    NotifyPropertyChanged(() => EnableBreadcrumb);
                }
            }
        }

        public bool IsRootOverflowed
        {
            get
            {
                return _IsRootOverflowed;
            }

            set
            {
                if (_IsRootOverflowed != value)
                {
                    Logger.InfoFormat("_");

                    _IsRootOverflowed = value;
                    NotifyPropertyChanged(() => IsRootOverflowed);
                }
            }
        }

        public string SuggestedPath
        {
            get { return _suggestedPath; }
            set
            {
                Logger.InfoFormat("_");

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
            Logger.InfoFormat("_");

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