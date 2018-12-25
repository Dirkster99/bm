namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BmLib.Interfaces;
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.Tasks;
    using BreadcrumbTestLib.ViewModels.Base;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using ShellBrowserLib;
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.Interfaces;
    using SSCoreLib.Browse;
    using SuggestLib.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Class implements the viewmodel that manages the complete breadcrump control.
    /// </summary>
    internal class BreadcrumbViewModel : Base.ViewModelBase, IBreadcrumbViewModel,
                                         BmLib.Interfaces.IBreadcrumbModel
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

        private bool _IsBrowsing;

        private BreadcrumbTreeItemViewModel _SelectedRootViewModel;
        private BreadcrumbTreeItemViewModel _BreadcrumbSelectedItem;
        private IDirectoryBrowser _SelectedRootValue;

        private ICommand _RootDropDownSelectionChangedCommand;
        private readonly INavigationController _NavigationController;
        private readonly ObservableCollection<BreadcrumbTreeItemViewModel> _OverflowedAndRootItems = null;
        private readonly IBreadcrumbTreeItemPath _CurrentPath;
        private readonly IDirectoryBrowser _RootLocation = ShellBrowser.DesktopDirectory;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="taskQueue"></param>
        public BreadcrumbViewModel(INavigationController navigationController)
            : this()
        {
            _NavigationController = navigationController;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected BreadcrumbViewModel()
        {
            _OverflowedAndRootItems = new ObservableCollection<BreadcrumbTreeItemViewModel>();
            _CurrentPath = new BreadcrumbTreeItemPath();
            _EnableBreadcrumb = true;
            _IsBrowsing = false;

            _Semaphore = new SemaphoreSlim(1, 1);
            _OneTaskScheduler = new OneTaskLimitedScheduler();

            Progressing = new ProgressViewModel();
            BreadcrumbSubTree = new BreadcrumbTreeItemViewModel(null, null, this);

            SuggestSources = new List<ISuggestSource>(new[]
                                { new SuggestSourceDirectory() });
        }
        #endregion constructors

        #region browsing events
        /// <summary>
        /// Indicates when the viewmodel starts heading off somewhere else
        /// and when its done browsing to a new location.
        /// </summary>
        public event EventHandler<BrowsingEventArgs> BrowseEvent;

        /// <summary>
        /// Raised when a node is selected, use SelectedValue/ViewModel to return the selected item.
        /// </summary>
        event EventHandler SelectionChanged;
        #endregion browsing events

        #region properties
        /// <summary>
        /// Gets a list of suggestion sources that are queries when the <see cref="SuggestionBox"/>
        /// is visible and the user can enter a string based path.
        /// </summary>
        public List<ISuggestSource> SuggestSources { get; }

        /// <summary>
        /// Gets a viewmodel that manages the progress display that is shown to inform
        /// users of long running processings.
        /// </summary>
        public IProgress Progressing { get; }

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

        /// <summary>
        /// Gets a seperate viewmodel object that keeps track of the current 
        /// path and all its viewmodel object items.
        /// </summary>
        public IBreadcrumbTreeItemPath CurrentPath
        {
            get
            {
                return _CurrentPath;
            }
        }

        /// <summary>
        /// Gets a currently selected item that is at the end
        /// of the currently selected path.
        /// </summary>
        public BreadcrumbTreeItemViewModel BreadcrumbSelectedItem
        {
            get
            {
                return _BreadcrumbSelectedItem;
            }

            protected set
            {
                if (_BreadcrumbSelectedItem != value)
                {
                    _BreadcrumbSelectedItem = value;
                    NotifyPropertyChanged(() => BreadcrumbSelectedItem);
                    NotifyPropertyChanged(() => CurrentPath);
                }
            }
        }

        /// <summary>
        /// Gets/Sets the string based path that binds to the SuggestBox and changes
        /// when it is visible and the user enters queries...
        /// </summary>
        public string SuggestedPath
        {
            get { return _suggestedPath; }
            set
            {
                if (_suggestedPath != value)
                {
                    _suggestedPath = value;
                    NotifyPropertyChanged(() => SuggestedPath);
                }
            }
        }

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

                        await RootDropDownSelectionChangedCommand_Executed(selectedFolder);
                    },
                    (param) =>
                    {
                        if (IsBrowsing == true)
                            return false;

                        return true;
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
        /// Gets a list of viewmodel items that are shown at the root drop down
        /// list of the control (left most drop down list)
        /// </summary>
        public IEnumerable<BreadcrumbTreeItemViewModel> OverflowedAndRootItems
        {
            get
            {
                return _OverflowedAndRootItems;
            }
        }

        /// <summary>
        /// Gets/sets the selected second level root item VIEWMODEL
        /// (eg. This PC, Library Desktop, or Desktop Folder).
        /// below the root desktop item.
        /// 
        /// This property usually changes:
        /// 1) When the user opens the drop down and selects 1 item in the dropdownlist of the RootDropDown button or
        /// 2) When the control navigates to a unrelated second level root address
        ///    (eg.: From 'This PC','C:\' to 'Libraries','Music')
        /// </summary>
        public BreadcrumbTreeItemViewModel SelectedRootViewModel
        {
            get
            {
                return _SelectedRootViewModel;
            }

            protected set
            {
                _SelectedRootViewModel = value;
                NotifyPropertyChanged(() => SelectedRootViewModel);
            }
        }

        /// <summary>
        /// Gets/sets the selected second level root item MODEL
        /// (eg. This PC, Library Desktop, or Desktop Folder)
        /// below the root desktop item.
        /// 
        /// This property usually changes:
        /// 1) When the user opens the drop down and selects 1 item in the dropdownlist of the RootDropDown button or
        /// 2) When the control navigates to a unrelated second level root address
        ///    (eg.: From 'This PC','C:\' to 'Libraries','Music')
        /// 
        /// Source:
        /// DropDownList Binding with SelectedValue="{Binding Selection.SelectedValue}"
        /// </summary>
        public IDirectoryBrowser SelectedRootValue
        {
            get
            {
                return _SelectedRootValue;
            }

            set
            {
                bool bHasChanged = true;

                if (_SelectedRootValue == null && value == null)
                    bHasChanged = false;
                else
                {
                    if ((_SelectedRootValue != null && value == null) ||
                        (_SelectedRootValue == null && value != null))
                        bHasChanged = true;
                    else
                    {
                        bHasChanged = !_SelectedRootValue.Equals(value);
                    }
                }

                if (bHasChanged == true)
                {
                    _SelectedRootValue = value;
                    NotifyPropertyChanged(() => this.SelectedRootValue);
                }
            }
        }
        #endregion properties

        #region methods
        #region IBreadcrumbModel
        /// <summary>
        /// This navigates the bound tree view model to the requested
        /// location when the user switches the display from:
        /// 
        /// - the string based and path oriented suggestbox back to
        /// - the tree view item based and path orient tree view.
        /// </summary>
        /// <param name="navigateToThisLocation"></param>
        /// <returns></returns>
        Task<bool> IBreadcrumbModel.NavigateTreeViewModel(string navigateToThisLocation,
                                                          bool goBackToPreviousLocation)
        {
            return Task.Run<bool>(async () =>
            {
                bool isPathValid = true;

                try
                {
                    // Navigation to new location was cancelled of given path is obviously empty
                    // Lets try and rollback to previously active location
                    if (string.IsNullOrEmpty(navigateToThisLocation) || goBackToPreviousLocation)
                    {
                        // Lets just go back to this without further processing
                        if (BreadcrumbSelectedItem != null)
                            return true;

                        isPathValid = false;
                        var previousLocation = BreadcrumbSelectedItem.GetModel();

                        // Previous location should be valid if we can re-create the model
                        isPathValid = (ShellBrowser.Create(previousLocation.PathShell) != null);

                        if (isPathValid)
                        {
                            await NavigateToScheduledAsync(previousLocation, "BreadcrumbViewModel.NavigateTreeViewModel");
                            return true;
                        }
                    }
                    else
                    {
                        // Attempt re-mounting if we find a common root for the given path
                        var currentPath = _CurrentPath.GetPathModels();
                        string extendPath = null;
                        int idxCommonRoot = ShellBrowser.FindCommonRoot(currentPath, navigateToThisLocation, out extendPath);

                        if (idxCommonRoot > 0 && extendPath != null)
                        {
                            List<IDirectoryBrowser> pathList = new List<IDirectoryBrowser>();
                            for (int i = 0; i <= idxCommonRoot; i++)
                            {
                                pathList.Add(currentPath[i].Clone() as IDirectoryBrowser);
                            }

                            bool joinSuccess = true;
                            if (string.IsNullOrEmpty(extendPath) == false)
                                joinSuccess = ShellBrowser.ExtendPath(ref pathList, extendPath);

                            if (joinSuccess) // path joined successfully -> lets go where no one has been before...
                            {
                                await NavigateToScheduledAsync(pathList.ToArray(), "BreadcrumbViewModel.NavigateTreeViewModel 4");
                                return true;
                            }
                        }

                        IDirectoryBrowser[] pathItems = null;
                        isPathValid = ShellBrowser.DirectoryExists(navigateToThisLocation, out pathItems);

                        if (isPathValid == true)
                        {
                            // The path is valid but we do not have any objects for it, yet.
                            // So, lets update the tree view based on the string representation.
                            if (pathItems == null)
                            {
                                var location = ShellBrowser.Create(navigateToThisLocation);
                                await NavigateToScheduledAsync(location, "BreadcrumbViewModel.NavigateTreeViewModel 0");
                            }
                            else
                            {
                                // We already have the objects representing the path
                                // so lets navigate the tree to this location
                                await NavigateToScheduledAsync(pathItems, "BreadcrumbViewModel.NavigateTreeViewModel 1");
                            }
                        }
                    }
                }
                catch
                {
                    isPathValid = false;
                }

                return isPathValid;
            });
        }

        /// <summary>
        /// Updates the bound text path property of the SuggestBox with the path of the
        /// currently selected item. This method should be called whenever the SuggestBox
        /// is switched from invisible to visible.
        /// </summary>
        /// <returns></returns>
        string IBreadcrumbModel.UpdateSuggestPath()
        {
            string path = string.Empty;

            if (_CurrentPath.Count > 0)
            {
                path = _CurrentPath.GetFileSystemPath();

                if (string.IsNullOrEmpty(path))
                    path = _CurrentPath.GetWinShellPath();
            }

            // Update path in bound textBox with value from currently selected item
            SuggestedPath = path;

            return SuggestedPath;
        }
        #endregion IBreadcrumbModel

        /// <summary>
        /// Method should be called after construction to initialize the viewmodel
        /// to view a default content.
        /// </summary>
        /// <param name="initialRequest"></param>
        public async Task InitPathAsync()
        {
            BreadcrumbTreeItemViewModel item = null;
            item = BreadcrumbSubTree.InitRoot(_RootLocation);

            if (item != null)
            {
                _CurrentPath.Add(item);
                await UpdateListOfOverflowableRootItemsAsync(_CurrentPath.Items, item);

                var request = new BrowseRequest<IDirectoryBrowser>(_RootLocation, RequestType.Navigational);
                await NavigateToAsync(request, "BreadcrumbViewModel.InitPathAsync");
            }
        }

        /// <summary>
        /// Schedules a navigational task and returns immediately
        /// </summary>
        /// <param name="targetLocation"></param>
        /// <param name="sourceHint"></param>
        /// <param name="hintDirection"></param>
        /// <param name="toBeSelectedLocation"></param>
        public async Task NavigateToScheduledAsync(
            IDirectoryBrowser targetLocation,
            string sourceHint,
            HintDirection hintDirection = HintDirection.Unrelated,
            BreadcrumbTreeItemViewModel toBeSelectedLocation = null
        )
        {
            if (_NavigationController != null)
            {
                var cancelTokenSrc = _NavigationController.GetCancelToken();
                var token = CancellationToken.None;

                var request = new BrowseRequest<IDirectoryBrowser>(targetLocation,
                                                                   RequestType.Navigational,
                                                                   token, cancelTokenSrc, null);

                var NaviTask = Task.Factory.StartNew(async (s) =>
                {
                    await this.NavigateToAsync(request, sourceHint, hintDirection, toBeSelectedLocation);

                }, token, TaskCreationOptions.LongRunning);

                request.SetTask(NaviTask);
                _NavigationController.QueueTask(request);
            }
            else
            {
                var request = new BrowseRequest<IDirectoryBrowser>(targetLocation, RequestType.Navigational);
                await NavigateToAsync(request,
                    "BreadcrumbTreeItemViewModel.BreadcrumbTreeTreeItemClickCommand",
                    hintDirection, toBeSelectedLocation);
            }
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
            Logger.InfoFormat("Request '{0}' direction {1} source: {2} IsBrowsing {3}",
                requestedLocation.NewLocation, direction, sourceHint, IsBrowsing);

            try
            {
                Progressing.ShowIndeterminatedProgress();
                IsBrowsing = true;
                try
                {
                    BrowseResult result = BrowseResult.Unknown;

                    // Optimize browsing up or down in current path
                    if (toBeSelectedLocation != null &&
                        (direction == HintDirection.Up || direction == HintDirection.Down))
                        result = await InternalNavigateUpDownAsync(toBeSelectedLocation,
                                                                   requestedLocation, direction);
                    else
                    {
                        if (requestedLocation.PathConfirmed == true)
                        {
                            // We have a verified path of model items - lets get the viewmodel items for this
                            result = await InternalNavigateAsyncToNewLocationAsync(BreadcrumbSubTree,
                                                                                   requestedLocation.LocationsPath);
                        }
                        else
                        {
                            // Unwind the string location based on the available SubTree ViewModel Items
                            result = await InternalNavigateAsyncToNewLocationAsync(BreadcrumbSubTree,
                                                                                   requestedLocation.NewLocation);
                        }
                    }

                    return FinalBrowseResult<IDirectoryBrowser>.FromRequest(requestedLocation,
                                                                            result);
                }
                finally
                {
                    Progressing.ProgressDisplayOff();
                    IsBrowsing = false;
                }
            }
            catch
            {
                return FinalBrowseResult<IDirectoryBrowser>.FromRequest(requestedLocation,
                                                                        BrowseResult.InComplete);
            }
        }

        /// <summary>
        /// Schedules a navigational request from a list of path model items and
        /// executes the request via asynchronous task execution.
        /// </summary>
        /// <param name="targetLocations"></param>
        /// <param name="sourceHint"></param>
        /// <returns></returns>
        private async Task NavigateToScheduledAsync(IDirectoryBrowser[] targetLocations,
                                                    string sourceHint)
        {
            if (_NavigationController != null)
            {
                var cancelTokenSrc = _NavigationController.GetCancelToken();
                var token = CancellationToken.None;

                var request = new BrowseRequest<IDirectoryBrowser>(targetLocations,
                                                                   RequestType.Navigational,
                                                                   token, cancelTokenSrc, null);

                var NaviTask = Task.Factory.StartNew(async (s) =>
                {
                    await this.NavigateToAsync(request, sourceHint);

                }, token, TaskCreationOptions.LongRunning);

                request.SetTask(NaviTask);
                _NavigationController.QueueTask(request);
            }
            else
            {
                var location = targetLocations[targetLocations.Length - 1];
                var request = new BrowseRequest<IDirectoryBrowser>(location, RequestType.Navigational);
                await NavigateToAsync(request,
                    "BreadcrumbTreeItemViewModel.BreadcrumbTreeTreeItemClickCommand");
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
            bool isMatchAvailable = false;            // Make sure requested location is really
            foreach (var item in _CurrentPath.Items) // part of current path (before edit current path...)
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
                // Pop current path items until we find the one we need
                var lastPathItem = _CurrentPath.Pop();
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

                        _CurrentPath.Add(item);
                        newSelectedLocation = item;
                        break;
                    }
                }
            }

            await UpdateListOfOverflowableRootItemsAsync(_CurrentPath.Items, newSelectedLocation);

            if (BrowseEvent != null)
                BrowseEvent(this, new BrowsingEventArgs(newSelectedLocation.GetModel(),
                                                        false, BrowseResult.Complete));

            return BrowseResult.Complete;
        }

        private async Task<BrowseResult> InternalNavigateAsyncToNewLocationAsync(
            BreadcrumbTreeItemViewModel desktopRootItem,
            IDirectoryBrowser[] modelLocations)
        {
            if (modelLocations == null)
                return BrowseResult.InComplete;

            if (modelLocations.Length <= 0)
                return BrowseResult.InComplete;

            // First location needs to be a second level root item (This PC, Desktop, Music or such)
            BreadcrumbTreeItemViewModel secLevelRootItem = null;
            var secLevelRootItems = desktopRootItem.Entries.All.Where(i => i.EqualsLocation(modelLocations[0]));
            if (secLevelRootItems.Count() > 0)
                secLevelRootItem = secLevelRootItems.First();
            else
                return BrowseResult.InComplete;

            // Count=0: Indicates that Desktop root (This PC is selected) item is destination of selection
            // locationRootItem Indicates a 2nd Level root item directly under the Desktop
            // Either case: Clear Path, insert DesktopRoot + (ThisPC or 2ndLevelRootItem)
            if (modelLocations.Length == 1)
            {
                await SelectSecLevelRootItem(desktopRootItem, secLevelRootItem);

                if (BrowseEvent != null)
                    BrowseEvent(this, new BrowsingEventArgs(modelLocations[0], false, BrowseResult.Complete));

                return BrowseResult.Complete;
            }

            foreach (var item in _CurrentPath.Items) // Reset Selection states from last selection
            {
                item.Selection.IsSelected = false;
                item.Selection.SelectedChild = null;
            }

            var targetPath = await FindLoadTargetTreeViewModelItems(modelLocations, secLevelRootItem);
            if (targetPath.Count == 0)
                return BrowseResult.InComplete;

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
            _CurrentPath.Add(desktopRootItem);

            foreach (var item in targetPath)
                _CurrentPath.Add(item);

            await UpdateListOfOverflowableRootItemsAsync(_CurrentPath.Items,
                                                         targetPath[targetPath.Count - 1]);

            if (BrowseEvent != null)
                BrowseEvent(this, new BrowsingEventArgs(modelLocations[modelLocations.Length - 1], false, BrowseResult.Complete));

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

            var targetItems = locations.ToArray();
            if (targetItems.Count() == 0)
                return BrowseResult.InComplete;   // No target to navigate to ???

            var firstTargetItem = targetItems.First();

            var firstSourceItems = BreadcrumbSubTree.Entries.All.Where(i => i.EqualsParseName(firstTargetItem));
            if (firstSourceItems.Count() == 0)
                return BrowseResult.InComplete;   // No source tree root to navigate ???

            var firstSourceItem = firstSourceItems.First();
            if (firstSourceItem == null)
                return BrowseResult.InComplete;   // No source tree root to navigate ???

            foreach (var item in _CurrentPath.Items) // Reset Selection states from last selection
            {
                item.Selection.IsSelected = false;
                item.Selection.SelectedChild = null;
            }

            var targetPath = await FindLoadTargetTreeViewModelItems(targetItems, firstSourceItem);

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
            _CurrentPath.Add(desktopRootItem);

            foreach (var item in targetPath)
                _CurrentPath.Add(item);

            await UpdateListOfOverflowableRootItemsAsync(_CurrentPath.Items, targetPath[targetPath.Count - 1]);

            if (BrowseEvent != null)
                BrowseEvent(this, new BrowsingEventArgs(location, false, BrowseResult.Complete));

            return BrowseResult.Complete;
        }

        /// <summary>
        /// Do a level order traversal on source root tree to find new target
        /// and re-load missing sub-items as we go deeper into the tree...
        /// 
        /// Update TreeViewModel of TreeItemViewModels along the given
        /// locations in <paramref name="modelLocations"/>. The <paramref name="firstSourceItem"/>
        /// represents the root item along which this tree is to be updated/reloaded.
        /// 
        /// Use of this method is recommended only if a previous method has confirmed the
        /// path in <paramref name="modelLocations"/> to be valid - there is no second verification
        /// on existance in this method.
        /// </summary>
        /// <param name="modelLocations"></param>
        /// <param name="firstSourceItem"></param>
        /// <returns></returns>
        private static async Task<List<BreadcrumbTreeItemViewModel>> FindLoadTargetTreeViewModelItems(
                                        IDirectoryBrowser[] modelLocations,
                                        BreadcrumbTreeItemViewModel firstSourceItem)
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
                if (current.EqualsLocation(modelLocations[iLevel]) == true)
                {
                    targetPath.Add(current);

                    if (current.Entries.IsLoaded == false)
                        await current.Entries.LoadAsync();

                    // Find next match in next level if any
                    if (modelLocations.Length > (iLevel + 1))
                    {
                        BreadcrumbTreeItemViewModel matchedItem = null;
                        foreach (var item in current.Entries.All)
                        {
                            if (item.EqualsLocation(modelLocations[iLevel + 1]))
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
        /// Do a level order traversal on source root tree to find new target
        /// and re-load missing sub-items as we go deeper into the tree...
        /// </summary>
        /// <param name="targetItems"></param>
        /// <param name="firstSourceItem"></param>
        /// <returns></returns>
        private static async Task<List<BreadcrumbTreeItemViewModel>> FindLoadTargetTreeViewModelItems(
                                        string[] targetItems,
                                        BreadcrumbTreeItemViewModel firstSourceItem)
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

            foreach (var item in _CurrentPath.Items)      // Reset Selection states from last selection
            {
                item.Selection.IsSelected = false;
                item.Selection.SelectedChild = null;
            }

            // Get thisPC item and set its selection states
            secLevelRootItem.Selection.IsSelected = true;
            BreadcrumbSubTree.Selection.SelectedChild = secLevelRootItem.GetModel();

            _CurrentPath.Clear();
            _CurrentPath.Add(desktopRootItem);   // set the current path to thisPC
            _CurrentPath.Add(secLevelRootItem); // set the current path to thisPC

            await UpdateListOfOverflowableRootItemsAsync(_CurrentPath.Items, secLevelRootItem);
        }

        /// <summary>
        /// Updates the source of the root drop down list by merging the current
        /// root items with the new list of overflowable path items.
        /// </summary>
        /// <param name="items">Is the list of new pathitems to be include in OverflowedAndRootItems</param>
        /// <param name="selectedItem"></param>
        private async Task UpdateListOfOverflowableRootItemsAsync(
            IEnumerable<BreadcrumbTreeItemViewModel> items,
            BreadcrumbTreeItemViewModel selectedItem)
        {
            // Update list of overflowable items for bindings from converter on rootdropdownlist
            // 1) Get all rootitems minus seperator minus overflowable pathitems
            List<BreadcrumbTreeItemViewModel> rootItems = new List<BreadcrumbTreeItemViewModel>();
            if (_OverflowedAndRootItems.Count() > 0)
            {
                foreach (var item in _OverflowedAndRootItems)
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
            UpdateOverflowedItems(rootItems, overflowedItems);

            // Update selection of currently selected item and second level root item
            await ReportChildSelectedAsync(selectedItem.Selection);

            // Update last selected item in chain of selected items and SuggestPath
            BreadcrumbSelectedItem = selectedItem;
            var model = selectedItem.GetModel();

            // select second level root item in RootDropDownList (if available)
            if (items.Count() >= 2)
            {
                SelectedRootValue = items.ElementAt(1).Selection.Value;
            }
            else
                SelectedRootValue = null;
        }

        /// <summary>
        /// Update the root drop down list with the list of root items
        /// and overflowable (non-root) items.
        /// </summary>
        /// <param name="rootItems"></param>
        /// <param name="pathItems"></param>
        private void UpdateOverflowedItems(IEnumerable<BreadcrumbTreeItemViewModel> rootItems,
                                          IEnumerable<BreadcrumbTreeItemViewModel> pathItems)
        {
            Logger.InfoFormat("_");

            Application.Current.Dispatcher.Invoke(() =>
            {
                _OverflowedAndRootItems.Clear();
            });

            // Get all items that belong to the current path and add them into the
            // OverflowedAndRootItems collection
            // The item.IsOverflowed property is (re)-set by OverflowableStackPanel
            // when the UI changes - a converter in the binding shows only those entries
            // in the root drop down list with item.IsOverflowed == true
            foreach (var p in pathItems)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _OverflowedAndRootItems.Add(p);
                });
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                // Insert Separator between Root and Overflowed Items
                _OverflowedAndRootItems.Add(null);
            });

            foreach (var p in rootItems)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _OverflowedAndRootItems.Add(p);
                });
            }
        }

        /// <summary>
        /// Method is invoked at the end of each navigational change when the
        /// control's viewmodel has retrieved all required items and is about
        /// to change the selection to complete the cycle.
        /// </summary>
        /// <param name="pathItem"></param>
        private async Task ReportChildSelectedAsync(
            ITreeSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser> pathItem)
        {
            Logger.InfoFormat("{0}", pathItem);

            try
            {
                if (pathItem != null)
                {
                    SelectedRootViewModel = pathItem.ViewModel;
                    SelectedRootValue = pathItem.Value;
                }
                else
                {
                    SelectedRootViewModel = null;
                    SelectedRootValue = null;
                }
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }

            if (this.SelectionChanged != null)
                this.SelectionChanged(this, EventArgs.Empty);

            if (pathItem.EntryHelper.IsLoaded == false)
                await pathItem.EntryHelper.LoadAsync();
        }

        private async Task RootDropDownSelectionChangedCommand_Executed(BreadcrumbTreeItemViewModel selectedFolder)
        {
            var hintDirection = HintDirection.Unrelated;
            BreadcrumbTreeItemViewModel toBeSelectedLocation = null;

            if (selectedFolder.Selection.IsOverflowed) // We can opimize browsing if target is in current path
            {
                // The selected root item is an overflowed root item so we move up towards the item that
                // is already part of the path
                hintDirection = HintDirection.Up;      // Hint optimization here ...
                toBeSelectedLocation = selectedFolder;
            }

            if (_NavigationController != null)
            {
                var cancelTokenSrc = _NavigationController.GetCancelToken();
                var token = CancellationToken.None;

                var request = new BrowseRequest<IDirectoryBrowser>(selectedFolder.GetModel(), RequestType.Navigational,
                                                                    token, cancelTokenSrc, null);

                var NaviTask = Task.Factory.StartNew(async (s) =>
                {
                    await this.NavigateToAsync(request, "BreadcrumbViewModel.RootDropDownSelectionChangedCommand",
                        hintDirection, toBeSelectedLocation);

                }, token, TaskCreationOptions.LongRunning);

                request.SetTask(NaviTask);
                _NavigationController.QueueTask(request);
            }
            else
            {
                var request = new BrowseRequest<IDirectoryBrowser>(selectedFolder.GetModel(), RequestType.Navigational);

                await this.NavigateToAsync(request, "BreadcrumbViewModel.RootDropDownSelectionChangedCommand",
                    hintDirection, toBeSelectedLocation);
            }
        }
        #endregion methods
    }
}