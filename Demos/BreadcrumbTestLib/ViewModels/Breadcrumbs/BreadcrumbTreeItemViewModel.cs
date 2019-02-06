namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbTestLib.ViewModels.Breadcrumbs.TreeSelectors;
    using BreadcrumbTestLib.ViewModels.Base;
    using WSF.Interfaces;
    using System;
    using BmLib.Interfaces;
    using BmLib.Enums;
    using BreadcrumbTestLib.Models;
    using System.Windows.Input;
    using WSF;
    using WSF.IDs;
    using WSF.Enums;
    using System.Windows;

    /// <summary>
    /// Class implements a ViewModel to manage a sub-tree of a Breadcrumb control.
    /// 
    /// This sub-tree includes
    /// - a specific item (see Header property),
    /// - a SelectedItem (see Selection property), and
    /// - a list of items below this item (<see cref="BreadcrumbTreeItemHelperViewModel{VM}"/>).
    /// </summary>
    public class BreadcrumbTreeItemViewModel : ViewModelBase,
                                               ISupportBreadcrumbTreeItemViewModel<BreadcrumbTreeItemViewModel, IDirectoryBrowser>,
                                               IBreadcrumbTreeItemViewModel
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IDirectoryBrowser _dir;
        private string _header;
        private BreadcrumbTreeItemViewModel _rootNode, _parentNode;

        // Instance of the root viewmodel - reference is used to invoke central tree related (navigational) methods
        private IRoot<IDirectoryBrowser> _Root;

        private ICommand _DropDownItemSelectedCommand;
        private ICommand _BreadcrumbTreeTreeItemClickCommand;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor from an available parentNode and directory model
        /// to recurse down on a given structure.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="parentNode"></param>
        internal BreadcrumbTreeItemViewModel(IDirectoryBrowser dir,
                                             BreadcrumbTreeItemViewModel parentNode,
                                             IRoot<IDirectoryBrowser> root
                                            )
        {
            Logger.InfoFormat("FullName '{0}'", (dir != null ? dir.FullName : "(null)"));

            _Root = root;
            _dir = dir;

            // If parentNode == null => Parent of Root is null
            _rootNode = parentNode == null ? this : parentNode._rootNode;
            _parentNode = parentNode;

            Header = (dir != null ? _dir.Label : string.Empty );

            Func<bool, object, Task<List<BreadcrumbTreeItemViewModel>>> loadAsyncFunc = (isLoaded, parameter) => Task.Run(() =>
            {
                try
                {
                    var subItemsList = Browser.GetChildItems(_dir.FullName, null, SubItemFilter.NameOnly, true);

                    //var viewmodels = subItemsList.Select(d => new BreadcrumbTreeItemViewModel(d, this, _Root));
                    List<BreadcrumbTreeItemViewModel> viewmodelItems = new List<BreadcrumbTreeItemViewModel>();
                    foreach (var item in subItemsList)
                    {
                        viewmodelItems.Add(new BreadcrumbTreeItemViewModel(item, this, _Root));
                    }

                    System.Console.WriteLine("{0} Retrieved {1:D8} items for '{2}'", DateTime.Now, subItemsList.Count(), _dir.FullName);

                    return viewmodelItems;
                }
                catch (Exception exp)
                {
                    Logger.Error(exp);
                    return new List<BreadcrumbTreeItemViewModel>();
                }
            });

            this.Entries = new BreadcrumbTreeItemHelperViewModel<BreadcrumbTreeItemViewModel>(loadAsyncFunc);
            this.Selection = new TreeSelectorViewModel<BreadcrumbTreeItemViewModel, IDirectoryBrowser>
                                                    (_dir, this, this.Entries);
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a property that holds all other properties on whether
        /// or not this entry is selected or not and so forth.
        /// </summary>
        public ITreeSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser> Selection { get; protected set; }

        /// <summary>
        /// Gets a structure that contains all root items that are located on level 0.
        /// See <see cref="Entries.All"/> collection for more details.
        /// </summary>
        public IBreadcrumbTreeItemHelperViewModel<BreadcrumbTreeItemViewModel> Entries { get; protected set; }

        /// <summary>
        /// Gets the name of the Breadcrumb node (item).
        /// </summary>
        public string Header
        {
            get
            {
                return _header;
            }

            protected set
            {
                if (_header != value)
                {
                    _header = value;
                    NotifyPropertyChanged(() => Header);
                }
            }
        }

        bool IOverflown.IsOverflown
        {
            get
            {
                if (Selection != null)
                    return Selection.IsOverflowed;

                return false;
            }
        }

        /// <summary>
        /// Gets a command that can change the navigation target of the currently
        /// selected location towards a new location. This command is typically
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
        public ICommand DropDownItemSelectedCommand
        {
            get
            {
                if (_DropDownItemSelectedCommand == null)
                {
                    _DropDownItemSelectedCommand = new RelayCommand<object>(async (param) =>
                    {
                        var parArray = param as object[];
                        if (parArray == null)
                            return;

                        if (parArray.Length <= 0)
                            return;

                        // Limitation of command is currently only 1 LOCATION PARAMETER being processed
                        var selectedFolder = parArray[0] as BreadcrumbTreeItemViewModel;

                        if (selectedFolder == null)
                            return;

                        if (_Root.IsBrowsing == true)   // Selection change originates from viewmodel
                            return;                    // So, let ignore this one since its browsing anyways...

                        Logger.InfoFormat("selectedFolder {0}", selectedFolder);

                        await _Root.NavigateToScheduledAsync(selectedFolder.GetModel(),
                                                             "BreadcrumbTreeItemViewModel.ItemSelectionChanged",
                                                             HintDirection.Down, this);
                    }
                    //// Partial rollback from commit on 2018-10-30:
                    //// https://github.com/Dirkster99/bm/commit/4ccb72b2ef6e500175cc99c63b37e2fa1d608e5a
                    //// Not needed and causes timing problems since other elements in the tree receive the event
                    //// ,(param) =>
                    //// {
                    ////     if (_Root.IsBrowsing == true)
                    ////         return false;
                    ////
                    ////     return true;
                    //// }
                    );
                }

                return _DropDownItemSelectedCommand;
            }
        }

        /// <summary>
        /// Gets a command that can be execute to make this item the currently
        /// selected item in the breadcrumb control.
        /// 
        /// Use Case: User clicks a visible item in the list of breadcrumbs.
        /// </summary>
        public ICommand BreadcrumbTreeTreeItemClickCommand
        {
            get
            {
                if (_BreadcrumbTreeTreeItemClickCommand == null)
                {
                    _BreadcrumbTreeTreeItemClickCommand = new RelayCommand<object>(async (param) =>
                    {
                        if (_Root.IsBrowsing == true)   // Selection change originates from viewmodel
                            return;                    // So, let ignore this one since its browsing anyways...

                        Logger.InfoFormat("selectedFolder {0}", this);

                        await _Root.NavigateToScheduledAsync(this.GetModel(),
                                                             "BreadcrumbTreeItemViewModel.BreadcrumbTreeTreeItemClickCommand",
                                                             HintDirection.Up, this);
                    }
                    //// Partial rollback from commit on 2018-10-30:
                    //// https://github.com/Dirkster99/bm/commit/4ccb72b2ef6e500175cc99c63b37e2fa1d608e5a
                    //// Not needed and causes timing problems since other elements in the tree receive the event
                    //// ,(param) =>
                    //// {
                    ////     if (_Root.IsBrowsing == true)
                    ////         return false;
                    ////
                    ////     return true;
                    //// }
                    );
                }

                return _BreadcrumbTreeTreeItemClickCommand;
            }
        }

        /// <summary>
        /// Get Known FolderId or file system Path for this folder.
        /// 
        /// That is:
        /// 1) A knownfolder GUID (if it exists) is shown
        ///    here as default preference over
        ///    
        /// 2) A storage location (if it exists) in the filesystem
        /// </summary>
        public string ItemPath
        {
            get
            {
                if (_dir == null)
                    return string.Empty;

                if (string.IsNullOrEmpty(_dir.SpecialPathId) == false)
                    return _dir.SpecialPathId;


                return _dir.PathFileSystem;
            }
        }

        /// <summary>
        /// Gets the name of this folder (without its root path component).
        /// </summary>
        public string ItemName
        {
            get
            {
                if (_dir != null)
                    return _dir.Name;

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets an optional pointer to the default icon resource used when the folder is created.
        /// This is a null-terminated Unicode string in this form:
        ///
        /// Module name, Resource ID
        /// or null is this information is not available.
        /// </summary>
        public string IconResourceId
        {
            get
            {
                if (_dir != null)
                    return _dir.IconResourceId;

                return null;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Output Header string to hint about the object instance of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("location '{0}' Header '{1}'",
                                (_dir != null ? _dir.FullName : "(null)"),
                                (this.Header != null ? this.Header : "(null)"));
        }

        /// <summary>
        /// Gets all root items below the desktop item and makes them available
        /// in the <see cref="Entries"/> collection. The first available item
        /// in the retrieved collection (eg: 'PC') is selected.
        /// 
        /// This method should only be called on a root item of the breadcrumb tree to
        /// initialize/construct a valid root of the tree(!!!)
        /// </summary>
        /// <param name="location"></param>
        /// <param name="rootSelector"></param>
        /// <returns></returns>
        internal BreadcrumbTreeItemViewModel InitRoot(IDirectoryBrowser location)
        {
            try
            {
                // Find all entries below desktop
                _dir = location;
                Header = _dir.Label;

                var models = Browser.GetChildItems(_dir.PathShell, null, SubItemFilter.NameOnly, true)
                                                // (filter out recycle bin and control panel entries since its not that useful...)
                                                .Where(d => string.Compare(d.SpecialPathId, KF_ParseName_IID.RecycleBinFolder, true) != 0)
                                                .Where(d => string.Compare(d.PathRAW, KF_ParseName_IID.ControlPanelFolder, true) != 0);

                var viewmodels = models.Select(d => new BreadcrumbTreeItemViewModel(d, this, _Root)).ToList();

                // and insert desktop sub-entries into Entries property
                Entries.SetEntries(viewmodels, UpdateMode.Replace);

                // All items imidiately under root are by definition root items
                foreach (var item in viewmodels)
                    item.Selection.IsRoot = true;

                var firstRootItem = viewmodels.First();
                firstRootItem.Selection.IsSelected = true;

                return firstRootItem;
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Gets the model that represents this item for internal navigational
        /// and debug puroses only(!).
        /// </summary>
        /// <returns></returns>
        internal IDirectoryBrowser GetModel()
        {
            return _dir;
        }

        /// <summary>
        /// Compares a given parse name with the parse names known in this object's model.
        /// 
        /// Considers case insensitive string matching for:
        /// 1> SpecialPathId
        ///   1.2> PathRAW (if SpecialPathId fails and CLSID may have been used to create this)
        ///
        /// 3> PathFileSystem
        /// </summary>
        /// <param name="parseName">True is a matching parse name was found and false if not.</param>
        /// <returns></returns>
        internal bool EqualsParseName(string parseName)
        {
            return _dir.EqualsParseName(parseName);
        }

        /// <summary>
        /// Goes through all Parent items of this item and gets a collection of viewmodel
        /// items that represent the path towards this item in the the tree.
        /// </summary>
        /// <returns></returns>
        internal IList<BreadcrumbTreeItemViewModel> GetPathItems(bool bReverseItems = true)
        {
            var parentNode = _parentNode;
            var list = new List<BreadcrumbTreeItemViewModel>();

            list.Add(this);

            while (parentNode != null)
            {
                list.Add(parentNode);
                parentNode = parentNode._parentNode;
            }

            if (bReverseItems == true)
                list.Reverse();

            return list;
        }

        internal bool EqualsLocation(IDirectoryBrowser location)
        {
            return _dir.Equals(location);
        }

        public IParent GetParent()
        {
            return _parentNode;
        }
        #endregion
    }
}