namespace WpfPerformance.ViewModels
{
    using WSF;
    using WSF.IDs;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using WpfPerformance.ViewModels.Base;

    public class AppViewModel : Base.ViewModelBase
    {
        #region fields
        public const int _NonVirtualizationMaximumSize = 1024;

        private readonly FastObservableCollection<ItemViewModel> _ListItems = null;

        private bool _IsProcessing;
        private string _CurrentPath;
        private string _GoToPath;
        private ICommand _GoToPathCommand;
        private ICommand _CleanUpItemCommand;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public AppViewModel()
        {
            _ListItems = new FastObservableCollection<ItemViewModel>();
        }
        #endregion ctors

        #region properties
        public ICommand GoToPathCommand
        {
            get
            {
                if (_GoToPathCommand == null)
                {
                    _GoToPathCommand = new RelayCommand<object>(async (p) =>
                    {
                        string path = p as string;
                        if (string.IsNullOrEmpty(path))
                            return;

                        await InitLoadAsync(path);
                    });
                }

                return _GoToPathCommand;
            }
        }

        public ICommand CleanUpItemCommand
        {
            get
            {
                if (_CleanUpItemCommand == null)
                {
                    _CleanUpItemCommand = new RelayCommand<object>((p) =>
                    {
                        var item = p as ItemViewModel;

                        if (item != null && _ListItems.Count >= _NonVirtualizationMaximumSize)
                        {
                            item.UnlodLoadModel();
                        }
                    });
                }

                return _CleanUpItemCommand;
            }
        }

        public string GoToPath
        {
            get { return _GoToPath; }
            set
            {
                if (_GoToPath != value)
                {
                    _GoToPath = value;
                    NotifyPropertyChanged(() => GoToPath);
                }
            }
        }

        public string CurrentPath
        {
            get { return _CurrentPath; }
            set
            {
                if (_CurrentPath != value)
                {
                    _CurrentPath = value;
                    NotifyPropertyChanged(() => CurrentPath);
                }
            }
        }

        public bool IsProcessing
        {
            get { return _IsProcessing; }
            set
            {
                if (_IsProcessing != value)
                {
                    _IsProcessing = value;
                    NotifyPropertyChanged(() => IsProcessing);
                }
            }
        }

        public IEnumerable<ItemViewModel> ListItems
        {
            get
            {
                return _ListItems;
            }
        }
        #endregion properties

        #region methods
        public async Task InitLoadAsync(string dirPath = null)
        {
            var result = await this.LoadItems(dirPath);

            var startTime = DateTime.Now;
            Console.WriteLine("{0} adding items into ObservableCollection...\n", startTime);

            if (result.Count < _NonVirtualizationMaximumSize)
            {
                foreach (var item in result)
                    await item.LoadModel();
            }

            _ListItems.Clear();
            _ListItems.AddItems(result);

            var endTime = DateTime.Now;
            Console.WriteLine("{0} done adding items after {1:n2} seconds..\n",
                              endTime, (endTime - startTime).TotalSeconds);
        }

        private Task<List<ItemViewModel>> LoadItems(string dirPath = null)
        {
            return Task.Run<List<ItemViewModel>>(() =>
            {
                List<ItemViewModel> result = null;
                result = new List<ItemViewModel>();

                IsProcessing = true;
                try
                {
                    try
                    {
                        // Load default demo directory if path was not supplied
                        if (string.IsNullOrEmpty(dirPath))
                        {
                            var windowsFolder = Browser.Create(KF_IID.ID_FOLDERID_Windows, true);
                            dirPath = System.IO.Path.Combine(windowsFolder.PathFileSystem, "WinSxs");
                        }

                        if (System.IO.Directory.Exists(dirPath) == false)
                            dirPath = Browser.SysDefault.PathFileSystem;

                        // Clip of last seperator if any
                        int idx = dirPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                        if (idx == dirPath.Length - 1)
                            dirPath = dirPath.Substring(0, dirPath.Length - 1);
                    }
                    catch
                    {
                        dirPath = Browser.SysDefault.PathFileSystem;
                    }

                    GoToPath = CurrentPath = dirPath;
                    Console.WriteLine("Retrieving all StageZero sub-directories from '{0}'...\n", dirPath);

                    // List all known folders
                    var startTime = DateTime.Now;
                    Console.WriteLine("...{0} working on it...\n", startTime);

                    int i = 0;
                    foreach (var item in Browser.GetSlimChildItems(dirPath))
                    {
                        result.Add(new ItemViewModel(item));
                        i++;
                    }

                    // List all known folders
                    var endTime = DateTime.Now;
                    Console.WriteLine();
                    Console.WriteLine("{0} Done retrieving {1} entries.\n", endTime, result.Count);
                    Console.WriteLine("After {0:n2} minutes or {1:n2} seconds.\n",
                        (endTime - startTime).TotalMinutes,
                        (endTime - startTime).TotalSeconds,
                        result.Count);
                }
                finally
                {
                    IsProcessing = false;
                }

                return result;
            });
        }
        #endregion methods
    }
}
