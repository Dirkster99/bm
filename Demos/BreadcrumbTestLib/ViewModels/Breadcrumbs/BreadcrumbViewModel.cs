namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BmLib.Enums;
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.Tasks;
    using BreadcrumbTestLib.ViewModels.Base;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using ShellBrowserLib;
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.Interfaces;
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

        private OneTaskLimitedScheduler _OneTaskScheduler;
        private SemaphoreSlim _Semaphore;
        private bool _EnableBreadcrumb;
        private string _suggestedPath;

        private ICommand _RootDropDownSelectionChangedCommand;
        private bool _IsBrowsing;
        private string _BreadcrumbSelectedPath;

        private IDirectoryBrowser _RootLocation = ShellBrowser.DesktopDirectory;
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

            _Semaphore = new SemaphoreSlim(1, 1);
            _OneTaskScheduler = new OneTaskLimitedScheduler();

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
                        if (IsBrowsing == true)
                            return;

                        var parArray = p as object[];
                        if (parArray == null)
                            return;

                        if (parArray.Length <= 0)
                            return;

                        // Limitation of command is currently only 1 LOCATION PARAMETER being processed
                        var selectedFolder = parArray[0] as BreadcrumbTreeItemViewModel;

                        if (selectedFolder == null)
                            return;

                        Logger.InfoFormat("selectedFolder {0}", selectedFolder);

                        var request = new BrowseRequest<IDirectoryBrowser>(selectedFolder.GetModel());
                        if (selectedFolder.Selection.IsOverflowed)
                        {
                            await this.NavigateToAsync(request, "BreadcrumbViewModel.RootDropDownSelectionChangedCommand",
                                HintDirection.Up, selectedFolder);
                        }
                        else
                        {
                            await this.NavigateToAsync(request, "BreadcrumbViewModel.RootDropDownSelectionChangedCommand");
                        }
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

                //var rootSelector = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
                var request = new BrowseRequest<IDirectoryBrowser>(_RootLocation);
                await NavigateToAsync(request, "BreadcrumbViewModel.InitPathAsync");

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
        /// <param name="direction">Specifies whether the navigation direction
        /// is not specified or up or down relative to the current path or
        /// ihintLevel parameter</param>
        /// <param name="ihintLevel">This parameter is relevant for Down direction only.
        /// It specifies the level in the tree structure from which the next child
        /// in the current path should be searched.</param>
        /// <returns>Returns a result that informs whether the target was reached or not.</returns>
        public async Task<FinalBrowseResult<IDirectoryBrowser>> NavigateToAsync(
            BrowseRequest<IDirectoryBrowser> requestedLocation,
            string sourceHint,
            HintDirection direction = HintDirection.Unrelated,
            BreadcrumbTreeItemViewModel toBeSelectedLocation = null)
        {
            Logger.InfoFormat("Request '{0}' direction {1} source: {3} IsBrowsing {4}",
                requestedLocation.NewLocation, direction, sourceHint, IsBrowsing);

            try
            {
                IsBrowsing = true;
                return await Task.Run(async () =>
                {
                    BrowseResult result = BrowseResult.Unknown;

                    if (toBeSelectedLocation != null &&
                        (direction == HintDirection.Up || direction == HintDirection.Down ))
                        result = await InternalNavigateUpDownAsync(toBeSelectedLocation,
                                                                   requestedLocation, direction);
                    else
                        result = await InternalNavigateAsyncToNewLocationAsync(BreadcrumbSubTree,
                                                                             requestedLocation.NewLocation);

//                    result = await InternalNavigateToAsync(requestedLocation.NewLocation,
//                                                               direction, ihintLevel);

                    return FinalBrowseResult<IDirectoryBrowser>.FromRequest(requestedLocation,
                                                                            result);
                }).ContinueWith((result)=>
                {
                    IsBrowsing = false;
                    return result.Result;
                });
            }
            catch
            {
                return FinalBrowseResult<IDirectoryBrowser>.FromRequest(requestedLocation,
                                                                        BrowseResult.InComplete);
            }
        }

        /// <summary>
        /// Implements an optimized browse path behavior for browsing back to an item that is
        /// already part of the currently selected path.
        /// </summary>
        /// <param name="toBeSelectedLocation"></param>
        /// <param name="requestedLocation"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private async Task<BrowseResult> InternalNavigateUpDownAsync(
            BreadcrumbTreeItemViewModel toBeSelectedLocation,
            BrowseRequest<IDirectoryBrowser> requestedLocation,
            HintDirection direction)
        {
            // CurrentPath should contain at least a Desktop RootItem + Currently Selected Item
            // Simply refuse if this is requested to browse to the last item in the current path
//            if (direction == HintDirection.Up)
//            {
//                if (_CurrentPath.Count() > 0)
//                {
//                    if (_CurrentPath.Peek().Equals(toBeSelectedLocation)) // Requested Location is Current Location
//                        return BrowseResult.Complete;                    // There is no way up from here...
//                }
//            }

            bool isMatchAvailable=false;               // Make sure reuested location is really
            foreach (var item in _CurrentPath)        // part of current path (before edit current path...)
            {
                if (item.Equals(toBeSelectedLocation))
                {
                    isMatchAvailable = true;
                    break;
                }
            }

            if (isMatchAvailable == false)
                return BrowseResult.InComplete;

            if (_CurrentPath.Peek().Equals(toBeSelectedLocation) == false)
            {
                var lastPathItem = _CurrentPath.Pop();  // Pop current path items until we find the one we need
                while (lastPathItem != null)
                {
                    lastPathItem.Selection.SelectedChild = null;
                    lastPathItem.Selection.IsSelected = false;

                    var nextLastPathItem = _CurrentPath.Peek();

                    if (nextLastPathItem != null)
                    {
                        nextLastPathItem.Selection.SelectedChild = null;
                        nextLastPathItem.Selection.IsSelected = true;

                        if (nextLastPathItem.Equals(toBeSelectedLocation))
                            break;

                        lastPathItem = _CurrentPath.Pop();
                    }
                    else
                        break;
                }
            }

            var newSelectedLocation = toBeSelectedLocation;
            if (direction == HintDirection.Down)
            {
                if (toBeSelectedLocation.Entries.IsLoaded == false)
                    await toBeSelectedLocation.Entries.LoadAsync();

                foreach (var item in toBeSelectedLocation.Entries.All)
                {
                    if (item.EqualsLocation(requestedLocation.NewLocation))
                    {
                        var topItem = _CurrentPath.Peek();

                        topItem.Selection.IsSelected = false;
                        topItem.Selection.SelectedChild = item.GetModel();

                        item.Selection.IsSelected = true;
                        item.Selection.SelectedChild = null;

                        _CurrentPath.Push(item);
                        newSelectedLocation = item;
                        break;
                    }
                }
            }

            var path1 = new Stack<ITreeSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>>();
            path1.Push(newSelectedLocation.Selection);
            await BreadcrumbSubTree.Selection.ReportChildSelectedAsync(path1);

            var rootSelector1 = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
            UpdateListOfOverflowableRootItems(rootSelector1, _CurrentPath);
            UpdateBreadcrumbSelectedPath();

            if (BrowseEvent != null)
                BrowseEvent(this, new BrowsingEventArgs(newSelectedLocation.GetModel(),
                                                        false, BrowseResult.Complete));

            return BrowseResult.Complete;
        }
/***
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
        private async Task<BrowseResult> InternalNavigateToAsync(
            IDirectoryBrowser location,
            HintDirection direction = HintDirection.Unrelated,
            int ihintLevel = -1)
        {
            Logger.InfoFormat("'{0}'", location.FullName);

            await _Semaphore.WaitAsync();
            try
            {
                if (direction == HintDirection.Unrelated)
                    return await InternalNavigateAsyncToNewLocationAsync(BreadcrumbSubTree, location);

                if (ihintLevel == -1)
                {
                    if (_CurrentPath == null)
                        ihintLevel = 0;
                    else
                        ihintLevel = _CurrentPath.Count();
                }

                ICompareHierarchy<IDirectoryBrowser> Comparer = new DirectoryBrowserHierarchyComparer();
                var items = await BrowseItemsAsync(BreadcrumbSubTree, location,
                                                   direction, ihintLevel,
                                                   _CurrentPath, Comparer);

                if (items.Count > 0)
                {
                    var selectedItem = items.Peek();

                    if (_CurrentPath != null) // Get rid of old selected path
                    {
                        var lastSelectedItem = _CurrentPath.Peek();

                        var lastList = _CurrentPath.Reverse().ToArray();
                        var pathList = items.Reverse().ToArray();

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
                                pathList[i - 1].Selection.IsSelected = false;
                                pathList[i - 1].Selection.SelectedChild = pathList[i].GetModel();
                            }

                            if (i == pathList.Length - 1)
                            {
                                pathList[i].Selection.IsSelected = true;
                                pathList[i].Selection.SelectedChild = null;
                            }
                        }
                    }

                    _CurrentPath = items;

                    var path = new Stack<ITreeSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>>();
                    path.Push(selectedItem.Selection);
                    await BreadcrumbSubTree.Selection.ReportChildSelectedAsync(path);
                }

                var rootSelector = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
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
                _Semaphore.Release();
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
            int ihintLevel,
            Stack<BreadcrumbTreeItemViewModel> currentPath,
            ICompareHierarchy<IDirectoryBrowser> Comparer)
        {
            Queue<Tuple<int, BreadcrumbTreeItemViewModel>> queue = new Queue<Tuple<int, BreadcrumbTreeItemViewModel>>();
            Stack<BreadcrumbTreeItemViewModel> path = new Stack<BreadcrumbTreeItemViewModel>();

            var ret = root;
            int initLevel = 0;
            BreadcrumbTreeItemViewModel matchedItem = null;

            if (direction == HintDirection.Down || direction == HintDirection.Up)
            {
                // Put current path into output stack and continue searching below current path
                foreach (var item in currentPath.ToArray().Reverse())
                {
                    path.Push(item);
                    initLevel++;

                    if (direction == HintDirection.Up) // Found target item? -> return completed path
                    {
                        var cmpResult = Comparer.CompareHierarchy(item.GetModel(), destination);
                        if (cmpResult == HierarchicalResult.Current)
                            return path;
                    }

                    // Found level down hint ??? Lets search from here
                    if (direction == HintDirection.Down && initLevel == (ihintLevel+1))
                        break;
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
                if (ShellBrowser.DesktopDirectory.Equals(destination) == true)
                    return path;

                matchedItem = GetBestMatch(destination, Comparer, root);
            }

            if (matchedItem != null)
                queue.Enqueue(new Tuple<int, BreadcrumbTreeItemViewModel>(initLevel, matchedItem));
            else
            {
//// TODO XXX
////                throw new NotImplementedException("Attempt Refresh - Reload before giving up here...");
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
***/
        private async Task<BrowseResult> InternalNavigateAsyncToNewLocationAsync(
            BreadcrumbTreeItemViewModel desktopRootItem,
            IDirectoryBrowser location)
        {
            if (location == null)
                return BrowseResult.InComplete;

            BreadcrumbTreeItemViewModel secLevelRootItem = null;
            var secLevelRootItems = desktopRootItem.Entries.All.Where(i => i.EqualsLocation(location));
            if (secLevelRootItems.Count() > 0)
                secLevelRootItem = secLevelRootItems.First();

            List<string> locations = null;

            if (secLevelRootItem == null)
                locations = ShellBrowser.PathItemsAsParseNames(location);
            else
            {
                locations = new List<string>() { secLevelRootItem.GetModel().PathShell };
            }

            // Count=0: Indicates that Desktop root (This PC is selected) item is destination of selection
            // locationRootItem Indicates a 2nd Level root item directly under the Desktop
            // Either case: Clear Path, insert DesktopRoot + (ThisPC or 2ndLevelRootItem)
            if (locations.Count == 0  || secLevelRootItem != null)
            {
                await SelectSecLevelRootItem(desktopRootItem, secLevelRootItem);

                if (BrowseEvent != null)
                    BrowseEvent(this, new BrowsingEventArgs(location, false, BrowseResult.Complete));

                return BrowseResult.Complete;
            }

            var targetItems = ShellBrowser.PathItemsAsParseNames(location).ToArray().Reverse().ToArray();
            if (targetItems.Count() == 0)
                return BrowseResult.InComplete;   // No target to navigate to ???

            var firstTargetItem = targetItems.First();

            var firstSourceItems = BreadcrumbSubTree.Entries.All.Where(i => i.EqualsParseName(firstTargetItem));
            if (firstSourceItems.Count() == 0)
                return BrowseResult.InComplete;   // No source tree root to navigate ???

            var firstSourceItem = firstSourceItems.First();
            if (firstSourceItem == null)
                return BrowseResult.InComplete;   // No source tree root to navigate ???

            foreach (var item in _CurrentPath)    // Reset Selection states from last selection
            {
                item.Selection.IsSelected = false;
                item.Selection.SelectedChild = null;
            }

            // Do level order traversal on source root tree to find new target
            List<BreadcrumbTreeItemViewModel> targetPath = new List<BreadcrumbTreeItemViewModel>();
            Queue<Tuple<int, BreadcrumbTreeItemViewModel>> queue = new Queue<Tuple<int, BreadcrumbTreeItemViewModel>>();

            queue.Enqueue(new Tuple<int, BreadcrumbTreeItemViewModel>(0, firstSourceItem));

            while (queue.Count() > 0)
            {
                var deQueuedItem = queue.Dequeue();
                int iLevel = deQueuedItem.Item1;
                var current = deQueuedItem.Item2;

                // Process the node
                if (current.EqualsParseName(targetItems[iLevel]) == true)
                {
                    targetPath.Add(current);

                    if (current.Entries.IsLoaded == false)
                        await current.Entries.LoadAsync();

                    // Find next match in next level if any
                    if (targetItems.Length > (iLevel+1))
                    {
                        BreadcrumbTreeItemViewModel matchedItem = null;
                        foreach (var item in current.Entries.All)
                        {
                            if (item.EqualsParseName(targetItems[iLevel+1]))
                            {
                                matchedItem = item;
                                break;
                            }
                        }

                        if (matchedItem != null)
                        {
                            queue.Clear();
                            queue.Enqueue(new Tuple<int, BreadcrumbTreeItemViewModel>(iLevel + 1, matchedItem));
                        }
                    }
                }
            }

            // Chain new path items by their SelectedChild property
            for (int i = 0; i < targetPath.Count - 1; i++)
            {
                targetPath[i].Selection.IsSelected = false;
                targetPath[i].Selection.SelectedChild = targetPath[i + 1].GetModel();
            }

            // Select last item and make sure there is no further selected child
            targetPath[targetPath.Count-1].Selection.IsSelected = true;
            targetPath[targetPath.Count-1].Selection.SelectedChild = null;

            _CurrentPath.Clear();               // Push new path items into current path
            _CurrentPath.Push(desktopRootItem);

            foreach (var item in targetPath)
                _CurrentPath.Push(item);

            var path1 = new Stack<ITreeSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>>();
            path1.Push(targetPath[targetPath.Count-1].Selection);
            await BreadcrumbSubTree.Selection.ReportChildSelectedAsync(path1);

            var rootSelector1 = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
            UpdateListOfOverflowableRootItems(rootSelector1, _CurrentPath);
            UpdateBreadcrumbSelectedPath();

            if (BrowseEvent != null)
                BrowseEvent(this, new BrowsingEventArgs(location, false, BrowseResult.Complete));

            return BrowseResult.Complete;
        }

        /// <summary>
        /// Selects a given item that is directly under the root of the desktop root.
        /// 
        /// This can be a special case because
        /// 1) 'This PC' should always be selected by default
        ///    if there is a need for a fallback option from fatal errors.
        /// 
        /// 2) A Second Level Root item like a (folder) on the current user's desktop
        /// should be displayed with the path: '> (folder) >'
        ///             and not with the path: '> This PC > C: > Users > (User) > Desktop > (folder) >'
        /// </summary>
        /// <param name="desktopRootItem"></param>
        /// <param name="secLevelRootItem"></param>
        /// <returns></returns>
        private async Task SelectSecLevelRootItem(BreadcrumbTreeItemViewModel desktopRootItem,
                                                  BreadcrumbTreeItemViewModel secLevelRootItem)
        {
            if (secLevelRootItem == null)
            {
                var thisPC = desktopRootItem.Entries.All.Where(i => i.EqualsParseName(KF_IID.ID_FOLDERID_ComputerFolder));
                secLevelRootItem = thisPC.First();
            }

            foreach (var item in _CurrentPath)      // Reset Selection states from last selection
            {
                item.Selection.IsSelected = false;
                item.Selection.SelectedChild = null;
            }

            // Get thisPC item and set its selection states
            secLevelRootItem.Selection.IsSelected = true;
            secLevelRootItem.Selection.SelectedChild = null;
            BreadcrumbSubTree.Selection.SelectedChild = secLevelRootItem.GetModel();

            _CurrentPath.Clear();
            _CurrentPath.Push(desktopRootItem);          // set the current path to thisPC
            _CurrentPath.Push(secLevelRootItem); // set the current path to thisPC

            var path = new Stack<ITreeSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>>();
            path.Push(secLevelRootItem.Selection);
            await BreadcrumbSubTree.Selection.ReportChildSelectedAsync(path);

            var rootSelector = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
            UpdateListOfOverflowableRootItems(rootSelector, _CurrentPath);
            UpdateBreadcrumbSelectedPath();
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
/***
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
***/
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