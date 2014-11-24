namespace Breadcrumb.DirectoryInfoEx
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using Breadcrumb.Defines;
    using Breadcrumb.Viewmodels.Base;
    using Breadcrumb.ViewModels.Helpers;
    using Breadcrumb.ViewModels.Interfaces;
    using Breadcrumb.ViewModels.TreeSelectors;
    using QuickZip.Converters;
    using Breadcrumb.ViewModels.ResourceLoader;
    using System;
    using BreadcrumbLib.Interfaces;
    using System.Windows.Data;
    using BreadcrumbLib.Utils;
    using System.Threading;
    using System.IO.Utils;
    using ShellDll;

    public class ExTreeNodeViewModel : NotifyPropertyChanged, ISupportIconHelper, ISupportTreeSelector<ExTreeNodeViewModel, FileSystemInfoEx>
    {
        #region fields
        public static ICompareHierarchy<FileSystemInfoEx> Comparer = new ExHierarchyComparer();

        private static IconExtractor iconExtractor = new ExIconExtractor(); //To-Do: Remove this.
        private static IResourceLoader defaultIcon =
            ResourceLoader.FromResourceDictionary(String.Format("{0}_{1}", "SpecialFolder", "Default"));

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
            this.PathConverter = new LambdaValueConverter<FileSystemInfoEx, string>(
                fsi =>
                {
                    DirectoryInfoEx di = fsi as DirectoryInfoEx;
                    if (di != null && di.FullName.StartsWith("::"))
                    {
                        var kfId = di.KnownFolderId;
                        if (kfId.HasValue)
                        {
                            var attrib = EnumAttributeUtils<DisplayNameAttribute, ShellDll.KnownFolderIds>.FindAttribute(kfId.Value);
                            if (attrib != null)
                                return Environment.ExpandEnvironmentVariables(attrib.DisplayName);
                        }
                    }
                    return fsi.FullName;

                },
                path =>
                {
                    if (path.IndexOfAny(new char[] { '/', '\\' }) == -1)
                    {
                        foreach (var kfId in Enum.GetValues(typeof(KnownFolderIds)))
                        {
                            var attrib = EnumAttributeUtils<DisplayNameAttribute, ShellDll.KnownFolderIds>.FindAttribute(kfId);
                            if (attrib != null && attrib.DisplayName.Equals(path, StringComparison.CurrentCultureIgnoreCase))
                                return new DirectoryInfoEx((KnownFolderIds)kfId);
                        }
                    }
                    return new DirectoryInfoEx(path);

                });
            initIconHelper(_dir);


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

            this.Entries = new EntriesHelperViewModel<ExTreeNodeViewModel>((isLoaded, parameter) => Task.Run(async () =>
            {
                try
                {
                    return (await this._dir.GetDirectoriesAsync("*", SearchOption.TopDirectoryOnly,
                        CancellationToken.None))
                        .Select(d => new ExTreeNodeViewModel(d, this));
                }
                catch
                {
                    return new List<ExTreeNodeViewModel>();
                }
            }));

            this.Selection = new TreeSelectorViewModel<ExTreeNodeViewModel, FileSystemInfoEx>(
                this._dir, this, this._parentNode.Selection, this.Entries);
            initIconHelper(_dir);
        }

        private void initIconHelper(DirectoryInfoEx dir)
        {
            var kfType = dir.KnownFolderType;
            if (kfType != null)
            {
                var kfId = kfType.KnownFolderId;
                if (kfId != null)
                {
                    string resourceKey = String.Format("{0}_{1}", "SpecialFolder", kfId.Value.ToString());
                    Func<int, IResourceLoader> resourceLoaderFunc =
                        size => ResourceLoader.FromResourceDictionary(resourceKey,
                            new ExIconResourceLoader(dir, true, size));
                    this.Icons = ResourceIconHelperViewModel.FromResourceLoader(resourceLoaderFunc);
                }
                else this.Icons = ResourceIconHelperViewModel.FromResourceLoader(size => new ExIconResourceLoader(dir, true, size));
            }
            else
                if (dir.FullName.EndsWith(":\\"))
                    this.Icons = ResourceIconHelperViewModel.FromResourceLoader(size => new ExIconResourceLoader(dir, true, size));
                else this.Icons = ResourceIconHelperViewModel.FromResourceLoader(defaultIcon);
        }

        #endregion constructors

        #region properties
        public ITreeSelector<ExTreeNodeViewModel, FileSystemInfoEx> Selection { get; set; }
        public IValueConverter PathConverter { get; set; }
        public IEntriesHelper<ExTreeNodeViewModel> Entries { get; set; }
        public ISuggestSource SuggestSource { get { return new ExSuggestSource(); } }

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

        public IIconHelperViewModel Icons { get; set; }

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
