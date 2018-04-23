namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using DirectoryInfoExLib.Interfaces;
    using System.Threading.Tasks;

    /// <summary>
    /// Class implements the viewmodel that manages the complete breadcrump control.
    /// </summary>
    public class BreadcrumbViewModel : Base.ViewModelBase, IBreadcrumbViewModel
    {
        #region fields
        private bool _EnableBreadcrumb;
        private string _suggestedPath;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public BreadcrumbViewModel()
        {
            Progressing = new ProgressViewModel();
            BreadcrumbSubTree = new ExTreeNodeViewModel();
            _EnableBreadcrumb = true;
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
        public ExTreeNodeViewModel BreadcrumbSubTree { get; }

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

        /// <summary>
        /// Contains a list of items that maps into the SuggestBox control.
        /// </summary>
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
        public async Task<FinalBrowseResult<IDirectoryInfoEx>> InitPathAsync(
            BrowseRequest<string> initialRequest)
        {
            var selector = BreadcrumbSubTree.Selection as ITreeRootSelector<ExTreeNodeViewModel, IDirectoryInfoEx>;

            return await Task.Run(() => selector.SelectAsync(DirectoryInfoExLib.Factory.FromString(initialRequest.NewLocation),
                                              initialRequest.CancelTok,
                                              Progressing));
        }

        /// <summary>
        /// Method executes when the text path portion in the
        /// Breadcrumb control has been edit.
        /// </summary>
        private void OnSuggestPathChanged()
        {
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