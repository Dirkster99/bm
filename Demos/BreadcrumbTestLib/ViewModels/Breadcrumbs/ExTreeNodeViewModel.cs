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

    /// <summary>
    /// Class implements a ViewModel to manage a sub-tree of a Breadcrumb control.
    /// 
    /// This sub-tree includes
    /// - a specific item (see Header property),
    /// - a SelectedItem (see Selection property), and
    /// - a list of items below this item.
    /// </summary>
    public class ExTreeNodeViewModel : ViewModelBase,
                                       ISupportTreeSelector<ExTreeNodeViewModel, IDirectoryBrowser>,
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
        private ExTreeNodeViewModel _rootNode, _parentNode;

        private bool _isIconLoaded = false;
        private ImageSource _icon = null;
        private bool _disposed = false;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public ExTreeNodeViewModel()
        {
            Entries = new EntriesHelperViewModel<ExTreeNodeViewModel>();
            Selection =
              new TreeRootSelectorViewModel<ExTreeNodeViewModel, IDirectoryBrowser>(this.Entries)
              {
                  Comparers = new[] { ExTreeNodeViewModel.Comparer }
              };

            // Find all entries below desktop
            _dir = DirectoryInfoExLib.Factory.DesktopDirectory;
            Entries.SetEntries(UpdateMode.Update,
                               _dir.GetDirectories()
                                 //(filter out recycle bin entry if its not that useful...)
                                 //.Where(d => !d.Equals(DirectoryInfoExLib.Factory.RecycleBinDirectory))
                                 .Select(d => new ExTreeNodeViewModel(d, this)).ToArray());

            Header = _dir.Label;
        }

        /// <summary>
        /// Class constructor from an available parentNode and directory model
        /// to recurse down on a given structure.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="parentNode"></param>
        internal ExTreeNodeViewModel(IDirectoryBrowser dir, ExTreeNodeViewModel parentNode)
        {
            Logger.InfoFormat("_");

            _dir = dir;

            // If parentNode == null => Root.
            _rootNode = parentNode == null ? this : parentNode._rootNode;

            _parentNode = parentNode;
            Header = _dir.Label;

            this.Entries = new EntriesHelperViewModel<ExTreeNodeViewModel>((isLoaded, parameter) => Task.Run(() =>
            {
                try
                {
                    return _dir.GetDirectories().Select(d => new ExTreeNodeViewModel(d, this));
                }
                catch (Exception exp)
                {
                    Logger.Error(exp);
                    return new List<ExTreeNodeViewModel>();
                }
            }));

            this.Selection = new TreeSelectorViewModel<ExTreeNodeViewModel, IDirectoryBrowser>
            (_dir, this, this._parentNode.Selection, this.Entries);
        }

        /// <summary>
        /// Class finalizer/destructor
        /// When the object is eligible for finalization,
        /// the garbage collector runs the Finalize method of the object. 
        /// </summary>
        ~ExTreeNodeViewModel()
        {
            Dispose();
        }
        #endregion constructors

        #region properties
        public ITreeSelector<ExTreeNodeViewModel, IDirectoryBrowser> Selection { get; protected set; }

        /// <summary>
        /// Gets all sub-tree entries that belong
        /// to the sub-tree represented by this item.
        /// </summary>
        public IEntriesHelper<ExTreeNodeViewModel> Entries { get; protected set; }

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
                    Logger.InfoFormat("_");
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
                    Logger.InfoFormat("_");
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