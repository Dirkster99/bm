namespace Breadcrumb.ViewModels.Breadcrumbs
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
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

    internal class ExTreeNodeViewModel : ViewModelBase, ISupportTreeSelector<ExTreeNodeViewModel, FileSystemInfoEx>
	{
		#region fields
		public static ICompareHierarchy<FileSystemInfoEx> Comparer = new ExHierarchyComparer();

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
			this.Entries = new EntriesHelperViewModel<ExTreeNodeViewModel>();
			this.Selection =
				new TreeRootSelectorViewModel<ExTreeNodeViewModel, FileSystemInfoEx>(this.Entries)
				{
					Comparers = new[] { ExTreeNodeViewModel.Comparer }
				};

			// Find all entries below desktop (filter out recycle bin entry since its realy not that useful)
			this._dir = DirectoryInfoEx.DesktopDirectory;
			this.Entries.SetEntries(UpdateMode.Update, this._dir.GetDirectories()
							     .Where(d => !d.Equals(DirectoryInfoEx.RecycleBinDirectory))
							     .Select(d => new ExTreeNodeViewModel(d, this)).ToArray());

			this.Header = this._dir.Label;
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="parentNode"></param>
		internal ExTreeNodeViewModel(DirectoryInfoEx dir, ExTreeNodeViewModel parentNode)
		{
			this._dir = dir;

			// If parentNode == null => Root.
			this._rootNode = parentNode == null ? this : parentNode._rootNode;

			this._parentNode = parentNode;
			this.Header = this._dir.Label;

			this.Entries = new EntriesHelperViewModel<ExTreeNodeViewModel>((isLoaded, parameter) => Task.Run(() =>
			{
				try
				{
					return this._dir.GetDirectories().Select(d => new ExTreeNodeViewModel(d, this));
				}
				catch
				{
					return new List<ExTreeNodeViewModel>();
				}
			}));

			this.Selection = new TreeSelectorViewModel<ExTreeNodeViewModel, FileSystemInfoEx>(this._dir,
																																											  this,
																																											  this._parentNode.Selection,
																																											  this.Entries);
		}
		#endregion constructors

		#region properties
		public ITreeSelector<ExTreeNodeViewModel, FileSystemInfoEx> Selection { get; set; }

		public IEntriesHelper<ExTreeNodeViewModel> Entries { get; set; }

		public string Header
		{
			get
			{
				return this._header;
			}

			set
			{
				if (this._header != value)
				{
					this._header = value;
					this.NotifyOfPropertyChanged(() => this.Header);
				}
			}
		}

		public ImageSource Icon
		{
			get
			{
				if (!this._isIconLoaded)
				{
					this._isIconLoaded = true;
					this.loadIcon();
				}

				return this._icon;
			}

			set
			{
				if (this._icon != value)
				{
					this._icon = value;
					this.NotifyOfPropertyChanged(() => this.Icon);
				}
			}
		}

		private void loadIcon()
		{
			Bitmap bitmap = null;

			this._dir.RequestPIDL(pidl =>
			{
				bitmap = ExTreeNodeViewModel.iconExtractor.GetBitmap(IconSize.large, pidl.Ptr, true, false);

				if (bitmap != null)
					this.Icon = Breadcrumb.Utils.BitmapSourceUtils.CreateBitmapSourceFromBitmap(bitmap);
			});
		}
		#endregion properties
	}
}
