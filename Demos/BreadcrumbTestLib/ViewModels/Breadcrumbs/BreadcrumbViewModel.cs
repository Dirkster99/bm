namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BmLib.Enums;
    using BmLib.Utils;
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.ViewModels.Base;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbTestLib.ViewModels.TreeLookupProcessors;
    using BreadcrumbTestLib.ViewModels.TreeSelectors;
    using DirectoryInfoExLib;
    using DirectoryInfoExLib.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Class implements the viewmodel that manages the complete breadcrump control.
    /// </summary>
    internal class BreadcrumbViewModel : Base.ViewModelBase, IBreadcrumbViewModel,
                                                             IRoot<IDirectoryBrowser>
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
        private string _BreadcrumbSelectedPath;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public BreadcrumbViewModel()
        {
            Progressing = new ProgressViewModel();
            BreadcrumbSubTree = new BreadcrumbTreeItemViewModel(this);
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
        public IProgressViewModel Progressing { get; }

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

        public string BreadcrumbSelectedPath
        {
            get
            {
                return _BreadcrumbSelectedPath;
            }

            set
            {
                if (_BreadcrumbSelectedPath != value)
                {
                    _BreadcrumbSelectedPath = value;
                    NotifyPropertyChanged(() => BreadcrumbSelectedPath);
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

                        await this.NavigateTo(selectedFolder.GetModel());
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
        #endregion properties

        #region methods
        /// <summary>
        /// Method should be called after construction to initialize the viewmodel
        /// to view a default content.
        /// </summary>
        /// <param name="initialRequest"></param>
        public Task InitPathAsync()
        {
            return Task.Run(() => BreadcrumbSubTree.InitRootAsync());
        }

        public async Task<FinalBrowseResult<IDirectoryBrowser>> NavigateTo1Async(
            BrowseRequest<string> requestedLocation)
        {
            Logger.InfoFormat("'{0}'", requestedLocation.NewLocation);

            return await Task.Run(async () =>
            {
                var newLocation = requestedLocation.NewLocation;
                var result = await NavigateTo(Factory.CreateDirectoryInfoEx(newLocation));

                var locate = Factory.CreateDirectoryInfoEx(newLocation);

                return new FinalBrowseResult<IDirectoryBrowser>(locate);
            });
        }

        /// <summary>
        /// Navigates the viewmodel (and hopefully the bound control) to a new location
        /// and ensures correct <see cref="IsBrowsing"/> state and event handling towards
        /// listing objects for
        /// <see cref="ICanNavigate"/> events.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<BrowseResult> NavigateTo(IDirectoryBrowser location)
        {
            IsBrowsing = true;
            try
            {
                var root = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
                if (root == null)
                    return BrowseResult.InComplete;

                string[] pathSegments = DirectoryInfoExLib.Factory.GetFolderSegments(location.FullName);
                var request = new BrowseRequest<string>(location.FullName, pathSegments, CancellationToken.None);

//                BreadcrumbTreeItemViewModel selectedParentItem = null, selectedItem = null;
                var selector = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
                var items = await LookUpAndSelectAsync(BreadcrumbSubTree, location);

                if (items.Count > 0)
                {
                    var selectedItem = items.Peek();
                    if (selectedItem.Selection.IsSelected == true)
                    {
                        var path = new Stack<ITreeSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>>();
                        path.Push(selectedItem.Selection);

                        BreadcrumbSubTree.Selection.ReportChildSelected(path);
                    }
                }

//                foreach (var item in items)
//                {
//                    Console.WriteLine("Path Item '{0}' IsChildSelected {1} IsSelected {2}", 
//                        item.Header,
//                        item.Selection.IsChildSelected, item.Selection.IsSelected);
//                }
//                Console.WriteLine();

//               await selector.SelectAsync(location, request, CancellationToken.None, Progressing);

                // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
//                selectedParentItem = null;
//                selectedItem = null;
//                selectedParentItem = FindItemIsSelected(BreadcrumbSubTree, out selectedItem);
//
//                if (selectedParentItem != null && selectedItem != null)
//                {
//                    selectedItem.Selection.IsSelected = true;
//
//                    var path = new Stack<ITreeSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>>();
//                    path.Push(selectedItem.Selection);
//                    BreadcrumbSubTree.Selection.ReportChildSelected(path);
//                }

                UpdateBreadcrumbSelectedPath();

                if (BrowseEvent != null)
                    BrowseEvent(this, new BrowsingEventArgs(location, false, BrowseResult.Complete));

                return BrowseResult.Complete;
            }
            catch
            {
                if (BrowseEvent != null)
                    BrowseEvent(this, new BrowsingEventArgs(location, false, BrowseResult.InComplete));

                return BrowseResult.InComplete;
            }
            finally
            {
                IsBrowsing = false;
            }
        }

        private async Task<Stack<BreadcrumbTreeItemViewModel>> LookUpAndSelectAsync(
            BreadcrumbTreeItemViewModel root,
            IDirectoryBrowser newLocation)
        {
            Queue<Tuple<int, BreadcrumbTreeItemViewModel>> queue = new Queue<Tuple<int, BreadcrumbTreeItemViewModel>>();
            Stack<BreadcrumbTreeItemViewModel> path = new Stack<BreadcrumbTreeItemViewModel>();
            ICompareHierarchy<IDirectoryBrowser> Comparer = new ExHierarchyComparer();

            var ret = root;

            if (root != null)
                queue.Enqueue(new Tuple<int, BreadcrumbTreeItemViewModel>(0, root));

            while (queue.Count() > 0)
            {
                var queueItem = queue.Dequeue();
                int iLevel = queueItem.Item1;
                BreadcrumbTreeItemViewModel current = queueItem.Item2;

                var result = Comparer.CompareHierarchy(current.GetModel(), newLocation);

                // Found an item along the way?
                // Search on sub-tree items from an item that indicates selected children
                if (result == HierarchicalResult.Child)
                {
                    if (path.Count() > 0)
                        path.Peek().Selection.SelectedChild = current.Selection.Value;

                    path.Push(current);

                    if (current.Entries.IsLoaded == false)
                        await current.Entries.LoadAsync();

                    foreach (var item in current.Entries.All)
                        queue.Enqueue(new Tuple<int, BreadcrumbTreeItemViewModel>(iLevel + 1, item));
                }
                else  // Found what we where looking for?
                {
                    if (result == HierarchicalResult.Current)
                    {
                        if (path.Count() > 0)
                            path.Peek().Selection.SelectedChild = current.Selection.Value;

                        path.Push(current);
                        current.Selection.IsSelected = true;
                    }
                }
            }

            return path;
        }

        /// <summary>
        /// Searches the list of Entries.All items below root and returns the item with
        /// the Selection.IsSelected property - requires that each item towards the result
        /// item has the Selection.IsChildSelected property set.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="selectedChild">Returns item with Selection.IsSelected property set or null.</param>
        /// <returns>Last item with Selection.IsChildSelected property set or root</returns>
        public BreadcrumbTreeItemViewModel FindItemIsSelected(BreadcrumbTreeItemViewModel root,
                                                          out BreadcrumbTreeItemViewModel selectedChild)
        {
            selectedChild = null;
            var ret = root;
            Queue<Tuple<int, BreadcrumbTreeItemViewModel>> queue = new Queue<Tuple<int, BreadcrumbTreeItemViewModel>>();

            if (root != null)
                queue.Enqueue(new Tuple<int, BreadcrumbTreeItemViewModel>(0, root));

            while (queue.Count() > 0)
            {
                var queueItem = queue.Dequeue();
                int iLevel = queueItem.Item1;
                BreadcrumbTreeItemViewModel current = queueItem.Item2;

                // Search on sub-tree items from an item that indicates selected children
                if (current.Selection.IsChildSelected == true)
                {
                    ret = current;

                    foreach (var item in current.Entries.All)
                        queue.Enqueue(new Tuple<int, BreadcrumbTreeItemViewModel>(iLevel + 1, item));
                }

                if (current.Selection.IsSelected == true)
                    selectedChild = current;
            }

            return ret;
        }

        /// <summary>
        /// Method is executed to  change the navigation target of the currently
        /// selected location towards a new location. This method is typically
        /// executed when:
        /// 1) Any other than the root drop down triangle is opened,
        /// 2) An entry in the list drop down is selected and
        /// 3) The control is now deactivating its previous selection and
        /// 4) needs to navigate towards the new selected item.
        /// 
        /// Expected command parameter:
        /// Array of length 1 with an object of type <see cref="BreadcrumbTreeItemViewModel"/>
        /// object[1] = {new <see cref="BreadcrumbTreeItemViewModel"/>() }
        /// </summary>
        /// <param name="item">Is the tree item that represents the target location in the tree structure.</param>
        public void NavigateToChild(BreadcrumbTreeItemViewModel item,
                                    BreadcrumbTreeItemViewModel selectedLocationModel)
        {
            var prevSelector = BreadcrumbSubTree.Selection as TreeRootSelectorViewModel<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;

            IsBrowsing = true;
            try
            {
                var selector = item.Selection as TreeSelectorViewModel<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
                selector.NavigateToChild(selectedLocationModel.GetModel());

                var path = new Stack<ITreeSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>>();
                path.Push(selectedLocationModel.Selection);
                BreadcrumbSubTree.Selection.ReportChildSelected(path);

                UpdateBreadcrumbSelectedPath();

                if (BrowseEvent != null)
                    BrowseEvent(this, new BrowsingEventArgs(item.GetModel(), false, BrowseResult.Complete));
            }
            catch
            {
                if (BrowseEvent != null)
                    BrowseEvent(this, new BrowsingEventArgs(item.GetModel(), false, BrowseResult.InComplete));
            }
            finally
            {
                IsBrowsing = false;
            }
        }

        /// <summary>
        /// Method updates the BreadcrumbSelectedPath property for debugging purposes.
        /// </summary>
        private void UpdateBreadcrumbSelectedPath()
        {
            var selector = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;

            if (selector != null)
            {
                if (selector.SelectedViewModel != null)
                {
                    var pathItems = selector.SelectedViewModel.GetPathItems();

                    string output = "";
                    foreach (var item in pathItems)
                    {
                        var itemString = string.Format("{0} {1}", item.Header,
                            (item.Selection.IsSelected == false ? "" : "(Selected)"));

                        output = (string.IsNullOrEmpty(output) ?
                            itemString :
                            output + "/" + itemString);
                    }

                    // Gets the complete path string below root item selector.RootSelector.SelectedValue.FullName;

                    BreadcrumbSelectedPath = output;
                }
            }
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