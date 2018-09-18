namespace BreadcrumbTestLib.SystemIO
{
    using BreadcrumbTestLib.ViewModels.Base;
    using BreadcrumbTestLib.ViewModels.Breadcrumbs;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbTestLib.ViewModels.TreeSelectors;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using BmLib.Interfaces;
    using BmLib.Enums;

    public class DiskTreeNodeViewModel : ViewModelBase, ISupportBreadcrumbTreeItemViewModel<DiskTreeNodeViewModel, string>
    {
        #region fields
        public static ICompareHierarchy<string> Comparer = new PathComparer(StringComparison.CurrentCultureIgnoreCase);

        private DirectoryInfo _dir;
        private DiskTreeNodeViewModel _rootNode, _parentNode;
        ////private bool _isOverflowed;
        private string _header;
        #endregion fields

        #region constructors
        public DiskTreeNodeViewModel(params DirectoryInfo[] dir)
        {
            Entries = new BreadcrumbTreeItemHelperViewModel<DiskTreeNodeViewModel>();
            Selection = new TreeRootSelectorViewModel<DiskTreeNodeViewModel, string>(
                                this.Entries,
                                new[] { DiskTreeNodeViewModel.Comparer });

            Entries.SetEntries(UpdateMode.Update, dir.Select(d => new DiskTreeNodeViewModel(d, this)).ToArray());
            Header = string.Empty;
        }

        internal DiskTreeNodeViewModel(DirectoryInfo dir, DiskTreeNodeViewModel parentNode)
        {
            _dir = dir;

            // If parentNode == null => Root.
            _rootNode = parentNode == null ? this : parentNode._rootNode;

            _parentNode = parentNode;
            Header = _dir.Name;

            this.Entries = new BreadcrumbTreeItemHelperViewModel<DiskTreeNodeViewModel>((ct) => Task.Run(() =>
            {
                try
                {
                    return _dir.GetDirectories().Select(d => new DiskTreeNodeViewModel(d, this));
                }
                catch
                {
                    return new List<DiskTreeNodeViewModel>();
                }
            }));

            this.Selection = new TreeSelectorViewModel<DiskTreeNodeViewModel, string>
                      (_dir.FullName,
                       this,
               _parentNode.Selection,
               Entries);
        }
        #endregion constructors

        public ITreeSelector<DiskTreeNodeViewModel, string> Selection { get; set; }

        /// <summary>
        /// Gets all sub-tree entries that belong
        /// to the sub-tree represented by this item.
        /// </summary>
        public IBreadcrumbTreeItemHelperViewModel<DiskTreeNodeViewModel> Entries { get; set; }

        //// Lycj: Moved to Selection.IsOverflowed/IsOverflowedOrRoot
        ////public bool IsOverflowedOrRoot { get { return _isOverflowed || _parentNode == null; } set { } }
        ////public bool IsOverflowed
        ////{
        ////    get { return _isOverflowed; }
        ////    set
        ////    {
        ////        _isOverflowed = value;
        ////        NotifyPropertyChanged(() => IsOverflowed);
        ////        NotifyPropertyChanged(() => IsOverflowedOrRoot);
        ////    }
        ////}

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
                    NotifyPropertyChanged(() => this.Header);
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
    }
}
