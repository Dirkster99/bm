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

    /// <summary>
    /// Class implements a ViewModel to manage a sub-tree of a Breadcrumb control.
    /// 
    /// This sub-tree includes
    /// - a specific item (see Header property),
    /// - a SelectedItem (see Selection property), and
    /// - a list of items below this item.
    /// </summary>
    public class ExTreeNodeViewModel : ViewModelBase,
                                       ISupportTreeSelector<ExTreeNodeViewModel, IDirectoryInfoEx>
    {
        #region fields
        public static ICompareHierarchy<IDirectoryInfoEx> Comparer = new ExHierarchyComparer();

        private IDirectoryInfoEx _dir;
        private string _header;
        private ExTreeNodeViewModel _rootNode, _parentNode;

        private bool _isIconLoaded = false;
        private ImageSource _icon = null;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public ExTreeNodeViewModel()
        {
            Entries = new EntriesHelperViewModel<ExTreeNodeViewModel>();
            Selection =
              new TreeRootSelectorViewModel<ExTreeNodeViewModel, IDirectoryInfoEx>(this.Entries)
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
        internal ExTreeNodeViewModel(IDirectoryInfoEx dir, ExTreeNodeViewModel parentNode)
        {
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
                catch
                {
                    return new List<ExTreeNodeViewModel>();
                }
            }));

            this.Selection = new TreeSelectorViewModel<ExTreeNodeViewModel, IDirectoryInfoEx>
            (_dir, this, this._parentNode.Selection, this.Entries);
        }
        #endregion constructors

        #region properties
        public ITreeSelector<ExTreeNodeViewModel, IDirectoryInfoEx> Selection { get; protected set; }

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
        /// Gets icon for this item when requested ...
        /// </summary>
        private void loadIcon()
        {
            Bitmap bitmap = _dir.RequestPIDL(pidl =>
                                GetBitmap(IconSize.large, pidl.Ptr, false));

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