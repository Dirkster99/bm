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
        #endregion fields

        #region constructors
        /// <summary>
        /// Standard class constructor.
        /// Call the <see cref="InitRootAsync"/> method after construction to initialize
        /// root items collection outside of the scope of object construction.
        /// </summary>
        public BreadcrumbTreeItemViewModel()
        {
            Entries = new BreadcrumbTreeItemHelperViewModel<BreadcrumbTreeItemViewModel>();
            Selection =
              new TreeRootSelectorViewModel<BreadcrumbTreeItemViewModel, IDirectoryBrowser>(this.Entries)
              {
                  Comparers = new[] { BreadcrumbTreeItemViewModel.Comparer }
              };
        }

        /// <summary>
        /// Class constructor from an available parentNode and directory model
        /// to recurse down on a given structure.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="parentNode"></param>
        protected BreadcrumbTreeItemViewModel(IDirectoryBrowser dir, BreadcrumbTreeItemViewModel parentNode)
        {
            Logger.InfoFormat("'{0}'", dir.FullName);

            _dir = dir;

            // If parentNode == null => Parent of Root is this item itself.
            _rootNode = parentNode == null ? this : parentNode._rootNode;

            _parentNode = parentNode;
            Header = _dir.Label;

            Func<bool, object, Task<IEnumerable<BreadcrumbTreeItemViewModel>>> loadAsyncFunc = (isLoaded, parameter) => Task.Run(() =>
            {
                try
                {
                    return _dir.GetDirectories().Select(d => new BreadcrumbTreeItemViewModel(d, this));
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
        /// in the <see cref="Entries"/> collection.
        /// </summary>
        /// <returns></returns>
        public Task<FinalBrowseResult<IDirectoryBrowser>> InitRootAsync(
            BrowseRequest<string> initialRequest,
            IProgressViewModel progress = null)
        {
            try
            {
                // Find all entries below desktop
                _dir = DirectoryInfoExLib.Factory.DesktopDirectory;

                Logger.InfoFormat("'{0}' -> '{1}'", initialRequest.NewLocation, _dir.Label);

                // and insert desktop sub-entries into Entries property
                Entries.SetEntries(UpdateMode.Update,
                                   _dir.GetDirectories().Select(d => new BreadcrumbTreeItemViewModel(d, this)).ToArray());
                                     //(filter out recycle bin entry if its not that useful...)
                                     //.Where(d => !d.Equals(DirectoryInfoExLib.Factory.RecycleBinDirectory))

                Header = _dir.Label;

                var selector = this.Selection as ITreeRootSelector<BreadcrumbTreeItemViewModel, IDirectoryBrowser>;

                return selector.SelectAsync(
                    DirectoryInfoExLib.Factory.FromString(initialRequest.NewLocation),
                    initialRequest,
                    initialRequest.CancelTok,
                    progress);
            }
            catch
            {
                return null;
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