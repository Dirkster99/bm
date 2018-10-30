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
    using SSCoreLib.Browse;
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
        private readonly Stack<BreadcrumbTreeItemViewModel> _CurrentPath;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public BreadcrumbViewModel()
        {
            _CurrentPath = new Stack<BreadcrumbTreeItemViewModel>() { };
            _EnableBreadcrumb = true;
            _IsBrowsing = false;

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
        /// Gets the <see cref="ITreeRootSelector{VM, M}"/> selector for this BreadCrumb ViewModel.
        /// </summary>
        internal ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser> RootSelector
        {
            get
            {
                return BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
            }
        }

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

        /// <summary>
        /// Gets a string representation of the currently selected path items.
        /// (mostly useful for debugging purposes).
        /// </summary>
        public string BreadcrumbSelectedPath
        {
            get
            {
                return _BreadcrumbSelectedPath;
            }

            protected set
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

                        var request = new BrowseRequest<IDirectoryBrowser>(selectedFolder.GetModel(), RequestType.Navigational);
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
                BreadcrumbTreeItemViewModel item = null;
                item = BreadcrumbSubTree.InitRoot(_RootLocation);

                if (item != null)
                {
                    _CurrentPath.Push(item);
                    await RootSelector.ReportChildSelectedAsync(item.Selection);

                    var request = new BrowseRequest<IDirectoryBrowser>(_RootLocation, RequestType.Navigational);
                    await NavigateToAsync(request, "BreadcrumbViewModel.InitPathAsync");
                }
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
            bool isMatchAvailable=false;               // Make sure requested location is really
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

            await RootSelector.ReportChildSelectedAsync(newSelectedLocation.Selection);
            UpdateListOfOverflowableRootItems(RootSelector, _CurrentPath);
            UpdateBreadcrumbSelectedPath();

            if (BrowseEvent != null)
                BrowseEvent(this, new BrowsingEventArgs(newSelectedLocation.GetModel(),
                                                        false, BrowseResult.Complete));

            return BrowseResult.Complete;
        }

        /// <summary>
        /// Find a location that may be completely unrelated to the current location.
        /// Load all sub-items beginning at the root and select the destination.
        /// </summary>
        /// <param name="desktopRootItem"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private async Task<BrowseResult> InternalNavigateAsyncToNewLocationAsync(
            BreadcrumbTreeItemViewModel desktopRootItem,
            IDirectoryBrowser location)
        {
            if (location == null)
                return BrowseResult.InComplete;

            // See if requested location is a second level root item (This PC, Desktop, Music or such)
            BreadcrumbTreeItemViewModel secLevelRootItem = null;
            var secLevelRootItems = desktopRootItem.Entries.All.Where(i => i.EqualsLocation(location));
            if (secLevelRootItems.Count() > 0)
                secLevelRootItem = secLevelRootItems.First();

            List<string> locations = null;

            if (secLevelRootItem == null)
                locations = ShellBrowser.PathItemsAsParseNames(location);
            else
                locations = new List<string>();

            // Count=0: Indicates that Desktop root (This PC is selected) item is destination of selection
            // locationRootItem Indicates a 2nd Level root item directly under the Desktop
            // Either case: Clear Path, insert DesktopRoot + (ThisPC or 2ndLevelRootItem)
            if (locations.Count == 0 || secLevelRootItem != null)
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

            var targetPath = await FindLoadTarget(targetItems, firstSourceItem);

            // Chain new path items by their SelectedChild property
            for (int i = 0; i < targetPath.Count - 1; i++)
            {
                targetPath[i].Selection.IsSelected = false;
                targetPath[i].Selection.SelectedChild = targetPath[i + 1].GetModel();
            }

            // Select last item and make sure there is no further selected child
            targetPath[targetPath.Count - 1].Selection.IsSelected = true;
            targetPath[targetPath.Count - 1].Selection.SelectedChild = null;

            _CurrentPath.Clear();               // Push new path items into current path
            _CurrentPath.Push(desktopRootItem);

            foreach (var item in targetPath)
                _CurrentPath.Push(item);

            await RootSelector.ReportChildSelectedAsync(targetPath[targetPath.Count - 1].Selection);

            var rootSelector1 = BreadcrumbSubTree.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;
            UpdateListOfOverflowableRootItems(rootSelector1, _CurrentPath);
            UpdateBreadcrumbSelectedPath();

            if (BrowseEvent != null)
                BrowseEvent(this, new BrowsingEventArgs(location, false, BrowseResult.Complete));

            return BrowseResult.Complete;
        }

        /// <summary>
        /// Do a level order traversal on source root tree to find new target
        /// and re-load missing sub-items as we go deeper into the tree...
        /// </summary>
        /// <param name="targetItems"></param>
        /// <param name="firstSourceItem"></param>
        /// <returns></returns>
        private static async Task<List<BreadcrumbTreeItemViewModel>> FindLoadTarget(string[] targetItems, BreadcrumbTreeItemViewModel firstSourceItem)
        {
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
                    if (targetItems.Length > (iLevel + 1))
                    {
                        BreadcrumbTreeItemViewModel matchedItem = null;
                        foreach (var item in current.Entries.All)
                        {
                            if (item.EqualsParseName(targetItems[iLevel + 1]))
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

            return targetPath;
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
            BreadcrumbSubTree.Selection.SelectedChild = secLevelRootItem.GetModel();

            _CurrentPath.Clear();
            _CurrentPath.Push(desktopRootItem);   // set the current path to thisPC
            _CurrentPath.Push(secLevelRootItem); // set the current path to thisPC

            await RootSelector.ReportChildSelectedAsync(secLevelRootItem.Selection);

            UpdateListOfOverflowableRootItems(RootSelector, _CurrentPath);
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

            var pathList = items.Reverse().ToArray();

            // select item in RootDropDownList if it is visible here
            if (pathList.Length >= 2)
                rootSelector.SelectedValue = pathList[1].Selection.Value;
            else
                rootSelector.SelectedValue = null;
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