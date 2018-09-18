namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BmLib.Enums;
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.ViewModels.Base;
    using BreadcrumbTestLib.ViewModels.Interfaces;
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
    internal class BreadcrumbViewModel : Base.ViewModelBase, IBreadcrumbViewModel                                                             
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SemaphoreSlim _SlowStuffSemaphore;

        private bool _EnableBreadcrumb;
        private string _suggestedPath;

        private ICommand _RootDropDownSelectionChangedCommand;
        private bool _IsBrowsing;
        private string _BreadcrumbSelectedPath;

        private IDirectoryBrowser _RootLocation = DirectoryInfoExLib.Factory.DesktopDirectory;
        private Stack<BreadcrumbTreeItemViewModel> _CurrentPath;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public BreadcrumbViewModel()
        {
            _EnableBreadcrumb = true;
            _IsBrowsing = false;
            _CurrentPath = null;
            _SlowStuffSemaphore = new SemaphoreSlim(1, 1);
            Progressing = new ProgressViewModel();
            BreadcrumbSubTree = new BreadcrumbTreeItemViewModel(this);
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

                        if (IsBrowsing == true)
                            return;

                        Logger.InfoFormat("selectedFolder {0}", selectedFolder);

                        var request = new BrowseRequest<IDirectoryBrowser>(selectedFolder.GetModel());
                        await this.NavigateToAsync(request);
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
            return Task.Run(async () =>
            {
                await BreadcrumbSubTree.InitRootAsync(_RootLocation);

                _CurrentPath = new Stack<BreadcrumbTreeItemViewModel>() { };
                _CurrentPath.Push(BreadcrumbSubTree.Entries.All.First());

                var rootSelector = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
                var request = new BrowseRequest<IDirectoryBrowser>(_RootLocation);
                await NavigateToAsync(request);

                //var items = await BrowseItemsAsync(BreadcrumbSubTree, initLocation);
                //UpdateListOfOverlowableRootItems(rootSelector, items);
            });
        }

        /// <summary>
        /// Navigates the viewmodel (and hopefully the bound control) to a new location
        /// and ensures correct <see cref="IsBrowsing"/> state and event handling towards
        /// listing objects for <see cref="ICanNavigate"/> events.
        /// </summary>
        /// <param name="requestedLocation"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public async Task<FinalBrowseResult<IDirectoryBrowser>> NavigateToAsync(
            BrowseRequest<IDirectoryBrowser> requestedLocation,
            HintDirection direction = HintDirection.Unrelated)
        {
            Logger.InfoFormat("'{0}'", requestedLocation.NewLocation);

            return await Task.Run(async () =>
            {
                await _SlowStuffSemaphore.WaitAsync();
                try
                {
                    var result = await NavigateToAsync(requestedLocation.NewLocation, direction);

                    return FinalBrowseResult<IDirectoryBrowser>.FromRequest(requestedLocation,
                                                                            result);
                }
                catch
                {
                    return FinalBrowseResult<IDirectoryBrowser>.FromRequest(requestedLocation,
                                                                            BrowseResult.InComplete);
                }
                finally
                {
                    _SlowStuffSemaphore.Release();
                }
            });
        }

        /// <summary>
        /// Implements the internal version of the navigational request method.
        /// This method should always be called through a method that ensures
        /// correct synchronization using the correct semaphore.
        /// 
        /// Method should result in resetting _CurrentPath to new path targeted in request.
        ///
        /// Items in _CurrentPath should always adhere to these requirements:
        /// The last item in the stack should have   LastItem.Selection.IsSelected      == true and
        /// all other items in the stack should have OtherItem.Selection.IsChildSelected == true
        ///                                          OtherItem.Selection.SelectedChild   != null
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private async Task<BrowseResult> NavigateToAsync(IDirectoryBrowser location,
                                                         HintDirection direction = HintDirection.Unrelated)
        {
            Logger.InfoFormat("'{0}'", location.FullName);

            IsBrowsing = true;
            try
            {
                var root = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
                if (root == null)
                    return BrowseResult.InComplete;

                var rootSelector = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
                var items = await BrowseItemsAsync(BreadcrumbSubTree, location,
                                                   direction, _CurrentPath);

                if (items.Count > 0)
                {
                    var selectedItem = items.Peek();

                    if (_CurrentPath != null) // Get rid of old selected path
                    {
                        var lastSelectedItem = _CurrentPath.Peek();

                        var lastList = _CurrentPath.Reverse().ToArray();
                        var pathList = items.Reverse().ToArray();
                        ICompareHierarchy<IDirectoryBrowser> Comparer = new ExHierarchyComparer();

                        for (int i = 0; i < lastList.Length; i++)
                        {
                            if (i < pathList.Length)
                            {
                                var result = Comparer.CompareHierarchy(lastList[i].GetModel(), pathList[i].GetModel());

                                if (result != HierarchicalResult.Current)
                                {
                                    lastList[i].Selection.SelectedChild = null;
                                    lastList[i].Selection.IsSelected = false;
                                }
                                else
                                {
                                    // Special case if we look at last item in chain
                                    // there cannot be a remaining child in the list
                                    if (i == pathList.Length - 1)
                                        lastList[i].Selection.SelectedChild = null;
                                }

                                if (selectedItem.GetModel().Equals(lastSelectedItem.GetModel()) == false)
                                    lastList[i].Selection.IsSelected = false;
                            }
                            else
                            {
                                lastList[i].Selection.SelectedChild = null;
                                lastList[i].Selection.IsSelected = false;
                            }
                        }

                        for (int i = 0; i < pathList.Length; i++)
                        {
                            if (i > 0 && i < pathList.Length)
                            {
                                pathList[i].Selection.IsSelected = false;
                                pathList[i - 1].Selection.SelectedChild = pathList[i].GetModel();
                            }

                            if (i == pathList.Length - 1)
                            {
                                pathList[i].Selection.IsSelected = true;
                                pathList[0].Selection.SelectedChild = null;
                            }
                        }
                    }

                    _CurrentPath = items;

                    var path = new Stack<ITreeSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>>();
                    path.Push(selectedItem.Selection);
                    await BreadcrumbSubTree.Selection.ReportChildSelectedAsync(path);
                }

                UpdateListOfOverflowableRootItems(rootSelector, items);

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

        /// <summary>
        /// Attempts to find the <paramref name="destination"/> underneath the given
        /// root and returns a stack containing the path to the selected item including
        /// items that were populated so far.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        private async Task<Stack<BreadcrumbTreeItemViewModel>> BrowseItemsAsync(
            BreadcrumbTreeItemViewModel root,
            IDirectoryBrowser destination,
            HintDirection direction,
            Stack<BreadcrumbTreeItemViewModel> currentPath)
        {
            Queue<Tuple<int, BreadcrumbTreeItemViewModel>> queue = new Queue<Tuple<int, BreadcrumbTreeItemViewModel>>();
            Stack<BreadcrumbTreeItemViewModel> path = new Stack<BreadcrumbTreeItemViewModel>();
            ICompareHierarchy<IDirectoryBrowser> Comparer = new ExHierarchyComparer();

            var ret = root;
            int initLevel = 0;
            BreadcrumbTreeItemViewModel matchedItem = null;
            
            if (direction == HintDirection.Down || direction == HintDirection.Up)
            {
                // Put current path into output stack and continue searching below current path
                foreach(var item in currentPath.ToArray().Reverse())
                {
                  path.Push(item);
                    initLevel++;

                  if (direction == HintDirection.Up) // Found target item? -> return completed path
                  {
                    var cmpResult = Comparer.CompareHierarchy(item.GetModel(), destination);
                    if (cmpResult == HierarchicalResult.Current)
                        return path;
                  }
                }

                // This should always result in finding the next Child item
                // and the next match should be matched with Current (end of retrieval)
                matchedItem = GetBestMatch(destination, Comparer, path.Peek());
            }

            // But lets consider rolling back to generic algorithm if something went wrong before
            if (direction != HintDirection.Unrelated)
            {
                if (matchedItem == null)
                {
                    path.Clear();
                    initLevel = 0;
                    direction = HintDirection.Unrelated;
                }
            }

            // Initialize generic search from root ...
            if (direction == HintDirection.Unrelated)
            {
                // Level 0 Root should always be pushed since everything's under DesktopRoot item
                path.Push(root);

                // Check if destination is root because this means we are done here
                if (Factory.DesktopDirectory.Equals(destination) == true)
                    return path;

                matchedItem = GetBestMatch(destination, Comparer, root);
            }

            if (matchedItem != null)
                queue.Enqueue(new Tuple<int, BreadcrumbTreeItemViewModel>(initLevel, matchedItem));
            else
            {
                throw new NotImplementedException("Attempt Refresh - Reload before giving up here...");
            }

            while (queue.Count() > 0)
            {
                var queueItem = queue.Dequeue();
                int iLevel = queueItem.Item1;
                BreadcrumbTreeItemViewModel current = queueItem.Item2;

                var result = Comparer.CompareHierarchy(current.GetModel(), destination);

                // Found an item along the way?
                // Search on sub-tree items from an item that indicates selected children
                if (result == HierarchicalResult.Child)
                {
                    path.Push(current);

                    if (current.Entries.IsLoaded == false)
                        await current.Entries.LoadAsync();

                    queue.Clear();
                    matchedItem = GetBestMatch(destination, Comparer, current);

                    if (matchedItem != null)
                        queue.Enqueue(new Tuple<int, BreadcrumbTreeItemViewModel>(iLevel + 1, matchedItem));
                    else
                    {
                        throw new NotImplementedException("Attempt Refresh - Reload before giving up here...");
                    }
                }
                else  // Found what we where looking for?
                {
                    if (result == HierarchicalResult.Current)
                    {
                        path.Push(current);
                        return path;
                    }
                }
            }

            return path;
        }

        private BreadcrumbTreeItemViewModel GetBestMatch(
            IDirectoryBrowser destination,
            ICompareHierarchy<IDirectoryBrowser> Comparer,
            BreadcrumbTreeItemViewModel currentItem)
        {
            BreadcrumbTreeItemViewModel ret = null;

            foreach (var item in currentItem.Entries.All)
            {
                var itemResult = Comparer.CompareHierarchy(item.GetModel(), destination);
                if (itemResult == HierarchicalResult.Child)
                    ret = item;
                else
                {
                    if (itemResult == HierarchicalResult.Current)
                        return item;
                }
            }

            return ret;
        }

        /// <summary>
        /// Updates the source of the root drop down list by merging the current
        /// root items with the new list of overflowable path items.
        /// </summary>
        /// <param name="rootSelector">Is the <see cref="ITreeRootSelector{VM,M}"/> that contains
        /// the OverflowedAndRootItems list to be updated.</param>
        /// <param name="items">Is the list of new pathitems to be include in OverflowedAndRootItems</param>
        private void UpdateListOfOverflowableRootItems(
            ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser> rootSelector,
            Stack<BreadcrumbTreeItemViewModel> items)
        {
            // Update list of overflowable items for bindings from converter on rootdropdownlist
            // 1) Get all rootitems minus seperator minus overflowable pathitems
            List<BreadcrumbTreeItemViewModel> rootItems = new List<BreadcrumbTreeItemViewModel>();
            if (rootSelector.OverflowedAndRootItems.Count() > 0)
            {
                foreach (var item in rootSelector.OverflowedAndRootItems)
                {
                    if (item != null)
                    {
                        if (item.Selection != null)
                        {
                            if (item.Selection.IsRoot == true)
                            {
                                rootItems.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var item in BreadcrumbSubTree.Entries.All)
                {
                    if (item.Selection.IsRoot == true)
                        rootItems.Add(item);
                }
            }

            // 2) Get new list of overflowable path items that are not root items
            //    (assuming that all root items are already in first list)
            var overflowedItems = items.Where(i => i.Selection.IsRoot == false);

            // 3) merge both lists from 1) and 2) into updated overflowable list
            rootSelector.UpdateOverflowedItems(rootItems, overflowedItems);
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