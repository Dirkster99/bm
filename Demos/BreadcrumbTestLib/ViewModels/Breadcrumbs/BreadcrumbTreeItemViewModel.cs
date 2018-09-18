namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbTestLib.ViewModels.TreeSelectors;
    using BreadcrumbTestLib.ViewModels.Base;
    using DirectoryInfoExLib.Interfaces;
    using System;
    using BmLib.IconExtractors.Enums;
    using BmLib.IconExtractors.IconExtracts;
    using BmLib.Interfaces;
    using BmLib.Enums;
    using System.Runtime.InteropServices;
    using BreadcrumbTestLib.Models;
    using System.Windows.Input;
    using DirectoryInfoExLib;

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
                                       IDisposable
    {
        #region fields
        public static ICompareHierarchy<IDirectoryBrowser> Comparer = new ExHierarchyComparer();

        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IDirectoryBrowser _dir;
        private string _header;
        private BreadcrumbTreeItemViewModel _rootNode, _parentNode;

        private bool _isIconLoaded = false;
        private ImageSource _icon = null;
        private bool _disposed = false;

        // Instance of the root viewmodel - reference is used to invoke central tree related (navigational) methods
        private IRoot<IDirectoryBrowser> _Root;

        private ICommand _ItemSelectionChangedCommand;
        private ICommand _BreadcrumbTreeTreeItemClickCommand;
        #endregion fields

        #region constructors
        /// <summary>
        /// Standard class constructor.
        /// Call the <see cref="InitRootAsync"/> method after construction to initialize
        /// root items collection outside of the scope of object construction.
        /// </summary>
        internal BreadcrumbTreeItemViewModel(IRoot<IDirectoryBrowser> root)
        {
            _Root = root;
            Entries = new BreadcrumbTreeItemHelperViewModel<BreadcrumbTreeItemViewModel>();
            Selection = new TreeRootSelectorViewModel<BreadcrumbTreeItemViewModel, IDirectoryBrowser>
                (this.Entries,
                  new[] { BreadcrumbTreeItemViewModel.Comparer });
        }

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
            Logger.InfoFormat("'{0}'", dir.FullName);

            _Root = root;
            _dir = dir;

            // If parentNode == null => Parent of Root is this item itself.
            _rootNode = parentNode == null ? this : parentNode._rootNode;

            _parentNode = parentNode;
            Header = _dir.Label;

            Func<bool, object, Task<IEnumerable<BreadcrumbTreeItemViewModel>>> loadAsyncFunc = (isLoaded, parameter) => Task.Run(() =>
            {
                try
                {
                    return _dir.GetDirectories().Select(d => new BreadcrumbTreeItemViewModel(d, this, _Root));
                }
                catch (Exception exp)
                {
                    Logger.Error(exp);
                    return new List<BreadcrumbTreeItemViewModel>();
                }
            });

            this.Entries = new BreadcrumbTreeItemHelperViewModel<BreadcrumbTreeItemViewModel>(loadAsyncFunc);
            this.Selection = new TreeSelectorViewModel<BreadcrumbTreeItemViewModel, IDirectoryBrowser>
                                                    (_dir, this, this._parentNode.Selection, this.Entries);
        }

        /// <summary>
        /// Class finalizer/destructor
        /// When the object is eligible for finalization,
        /// the garbage collector runs the Finalize method of the object. 
        /// </summary>
        ~BreadcrumbTreeItemViewModel()
        {
            Dispose();
        }
        #endregion constructors

        #region properties
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

        /// <summary>
        /// Gets a lazy loaded image source for an icon that
        /// can be used to reresent this item in the bound view.
        /// </summary>
        public ImageSource Icon
        {
            get
            {
                if (_isIconLoaded == false)
                {
                    _isIconLoaded = true;
                    loadIcon();
                }

                return _icon;
            }

            protected set
            {
                if (_icon != value)
                {
                    _icon = value;
                    NotifyPropertyChanged(() => Icon);
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
        public ICommand ItemSelectionChangedCommand
        {
            get
            {
                if (_ItemSelectionChangedCommand == null)
                {
                    _ItemSelectionChangedCommand = new RelayCommand<object>(async (param) =>
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

                        var request = new BrowseRequest<IDirectoryBrowser>(selectedFolder.GetModel());
                        await _Root.NavigateToAsync(request, HintDirection.Down);
                    });
                }

                return _ItemSelectionChangedCommand;
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

                        var request = new BrowseRequest<IDirectoryBrowser>(this.GetModel());
                        await _Root.NavigateToAsync(request, HintDirection.Up);
                    });
                }

                return _BreadcrumbTreeTreeItemClickCommand;
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
            return (this.Header != null ? this.Header : "(Header==null)");
        }

        /// <summary>
        /// Gets all root items below the desktop item and makes them available
        /// in the <see cref="Entries"/> collection. The first available item
        /// in the retrieved collection (eg: 'PC') is selected.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task InitRootAsync(IDirectoryBrowser location)
        {
            try
            {
                // Find all entries below desktop
                _dir = location;
                Header = _dir.Label;

                // and insert desktop sub-entries into Entries property
                Entries.SetEntries(UpdateMode.Update,
                                   _dir.GetDirectories().Select(d => new BreadcrumbTreeItemViewModel(d, this, _Root)).ToArray());
                                     //(filter out recycle bin entry if its not that useful...)
                                     //.Where(d => !d.Equals(DirectoryInfoExLib.Factory.RecycleBinDirectory))

                var selector = this.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;

                if (Entries.All.Count() > 0)
                {
                    // All items imidiately under root are by definition root items
                    foreach (var item in Entries.All)
                        item.Selection.IsRoot = true;

                    var firstRootItem = Entries.All.First();
                    firstRootItem.Selection.IsSelected = true;

                    var path = new Stack<ITreeSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>>();
                    path.Push(firstRootItem.Selection);


                    await this.Selection.ReportChildSelectedAsync(path);
                }
            }
            catch
            {
            }
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
        /// Gets a collection of viewmodel items that represent
        /// the path towards the currently selected item in the the tree.
        /// </summary>
        /// <returns></returns>
        public IList<BreadcrumbTreeItemViewModel> GetPathItems(bool bReverseItems = true)
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

        #region Disposable Interfaces
        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Implements standard disposable method.
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    // Dispose of the model that defines this viemodel
                    if (_dir != null)
                        _dir.Dispose();

                    _dir = null;
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            _disposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }
        #endregion Disposable Interfaces

        /// <summary>
        /// Gets icon for this item when requested ...
        /// </summary>
        private void loadIcon()
        {
            IntPtr pidl = _dir.GetPIDLIntPtr();
            Bitmap bitmap = null;

            try
            {
                bitmap = GetBitmap(IconSize.large, pidl, false);
            }
            finally
            {
                Marshal.FreeCoTaskMem(pidl);
            }

            if (bitmap != null)
                this.Icon = BreadcrumbTestLib.Utils.BitmapSourceUtils.CreateBitmapSourceFromBitmap(bitmap);
        }

        private Bitmap GetBitmap(IconSize size,
                                 IntPtr ptr,
                                 bool forceLoad)
        {
            Bitmap retVal = null;

            using (var imgList = new SystemImageList(size))
                retVal = imgList[ptr, true, forceLoad];

            return retVal;
        }
        #endregion
    }
}