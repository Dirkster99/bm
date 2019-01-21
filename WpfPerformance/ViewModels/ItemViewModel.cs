namespace WpfPerformance.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using WSF;
    using WSF.Browse;
    using WSF.Interfaces;

    public class ItemViewModel : Base.ViewModelBase, IItemViewModel
    {
        #region fields
        private readonly DirectoryBrowserSlim _ModelStage1;
        private DateTime _lastRefreshTimeUtc;
        private IDirectoryBrowser _dir;
        private bool _isLoaded;
        private bool _IsLoading;
        #endregion fields

        #region ctors
        /// <summary>
        /// parameterized class constructor
        /// </summary>
        public ItemViewModel(DirectoryBrowserSlim model)
            : this()
        {
            this._ModelStage1 = model;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected ItemViewModel()
        {
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets a unqie ID within the collection.
        /// </summary>
        public int ID
        {
            get
            {
                return _ModelStage1.ID;
            }
        }

        /// <summary>
        /// Gets the name of the Breadcrumb node (item).
        /// </summary>
        public string Header
        {
            get
            {
                return (_dir != null ? _dir.Label : string.Empty);
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
                if (IsLoaded)
                {
                    if (_dir != null)
                        return _dir.Name;
                }
                else
                {
                    return _ModelStage1.ParseName;
                }

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

        /// <summary>
        /// Gets whether current items are already loaded or not.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return _IsLoading;
            }

            private set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyPropertyChanged(() => IsLoading);
                }
            }
        }

        /// <summary>
        /// Gets whether current items are already loaded or not.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }

            private set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    NotifyPropertyChanged(() => IsLoaded);
                }
            }
        }

        /// <summary>
        /// Gets the Coordinated Universal Time (UTC) of the last load processing
        /// at the ALL/AllNonBindable items collection.
        /// </summary>
        public DateTime LastRefreshTimeUtc
        {
            get
            {
                return _lastRefreshTimeUtc;
            }

            private set
            {
                if (_lastRefreshTimeUtc != value)
                {
                    _lastRefreshTimeUtc = value;
                    NotifyPropertyChanged(() => IsLoaded);
                }
            }
        }
        #endregion properties

        #region methods
        public Task<bool> LoadModel()
        {
            return Task.Run<bool>(() =>
            {
                IsLoaded = false;
                IsLoading = true;
                try
                {
                    string pathFileName = string.Format("{0}{1}{2}", _ModelStage1.ItemPath
                                                                   , System.IO.Path.DirectorySeparatorChar
                                                                   , _ModelStage1.ParseName );

                    _dir = Browser.Create(pathFileName, _ModelStage1.LabelName,
                                               _ModelStage1.Name);

                    if (_dir != null)
                    {
                        NotifyPropertyChanged(() => ItemName);
                        NotifyPropertyChanged(() => ItemPath);
                        NotifyPropertyChanged(() => IconResourceId);
                        NotifyPropertyChanged(() => Header);

                        LastRefreshTimeUtc = DateTime.Now;
                        IsLoaded = true;

                        //System.Console.WriteLine("Model {0} loaded.", _ModelStage1.ID);
                    }
                }
                catch
                {
                    IsLoaded = false;
                    IsLoading = false;
                    return false;
                }

                IsLoading = false;
                IsLoaded = true;
                return IsLoaded;
            });
        }

        public Task<bool> UnlodLoadModel()
        {
            return Task.Run<bool>(() =>
            {
                if (IsLoaded == true)
                {
                    IsLoading = true;
                    try
                    {
                        _dir = null;
                    }
                    finally
                    {
                        IsLoaded = IsLoading = false;
                    }

                    return true;
                }

                return false;
            });
        }
        #endregion methods
    }
}
