namespace Breadcrumb.ViewModels.Breadcrumbs
{
  using System.Collections.Generic;
  using System.Drawing;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Windows.Media;
  using Breadcrumb.ViewModels.Helpers;
  using Breadcrumb.ViewModels.Interfaces;
  using Breadcrumb.ViewModels.TreeSelectors;
  using BreadcrumbLib.Defines;
  using Breadcrumb.IconExtractors;
  using Breadcrumb.IconExtractors.Enums;
  using Breadcrumb.ViewModels.Base;
  using DirectoryInfoExLib.IO.FileSystemInfoExt;

  /// <summary>
  /// Class implements a ViewModel to manage a sub-tree of a Breadcrumb control.
  /// This sub-tree includes
  /// - a specific item (see Header property),
  /// - a SelectedItem (see Selection property), and
  /// - a list of items below this item.
  /// </summary>
  internal class ExTreeNodeViewModel : ViewModelBase, ISupportTreeSelector<ExTreeNodeViewModel, DirectoryInfoEx>
  {
    #region fields
    public static ICompareHierarchy<DirectoryInfoEx> Comparer = new ExHierarchyComparer();

    private static IconExtractor iconExtractor = new ExIconExtractor();

    private DirectoryInfoEx _dir;
    private ExTreeNodeViewModel _rootNode, _parentNode;

    private string _header;

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
        new TreeRootSelectorViewModel<ExTreeNodeViewModel, DirectoryInfoEx>(this.Entries)
        {
          Comparers = new[] { ExTreeNodeViewModel.Comparer }
        };

      // Find all entries below desktop (filter out recycle bin entry since its realy not that useful)
      _dir = DirectoryInfoExLib.Factory.DesktopDirectory;
      Entries.SetEntries(UpdateMode.Update,
                         _dir.GetDirectories()
                           .Where(d => !d.Equals(DirectoryInfoExLib.Factory.RecycleBinDirectory))
                           .Select(d => new ExTreeNodeViewModel(d, this)).ToArray());

      Header = _dir.Label;
    }

    /// <summary>
    /// Class constructor from an available parentNode and directory model
    /// to recurse down on a given structure.
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="parentNode"></param>
    internal ExTreeNodeViewModel(DirectoryInfoEx dir, ExTreeNodeViewModel parentNode)
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

      this.Selection = new TreeSelectorViewModel<ExTreeNodeViewModel, DirectoryInfoEx>
      (  _dir, this, this._parentNode.Selection, this.Entries);
    }
    #endregion constructors

    #region properties
    public ITreeSelector<ExTreeNodeViewModel, DirectoryInfoEx> Selection { get; set; }

    public IEntriesHelper<ExTreeNodeViewModel> Entries { get; set; }

    
    /// <summary>
    /// Gets/sets the name of the Breadcrumb node (item).
    /// </summary>
    public string Header
    {
      get
      {
        return _header;
      }

      set
      {
        if (_header != value)
        {
          _header = value;
          NotifyOfPropertyChanged(() => Header);
        }
      }
    }

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

      set
      {
        if (_icon != value)
        {
          _icon = value;
          NotifyOfPropertyChanged(() => Icon);
        }
      }
    }

    private void loadIcon()
    {
        this._dir.RequestPIDL(pidl =>
        {
            Bitmap bitmap = ExTreeNodeViewModel.iconExtractor.GetBitmap(IconSize.large, pidl.Ptr, true, false);

            if (bitmap != null)
                this.Icon = Breadcrumb.Utils.BitmapSourceUtils.CreateBitmapSourceFromBitmap(bitmap);
        });
    }
    #endregion properties
  }
}
