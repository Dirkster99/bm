namespace WpfPerformance.ViewModels
{
    using ShellBrowserLib;
    using ShellBrowserLib.IDs;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class AppViewModel : Base.ViewModelBase
    {
        #region fields
        private readonly FastObservableCollection<ItemViewModel> _ListItems = null;

        private bool _IsProcessing;
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
        public async Task InitLoadAsync()
        {
            var result = await this.LoadItems();

            var startTime = DateTime.Now;
            Console.WriteLine("{0} adding items into ObservableCollection...\n", startTime);
            _ListItems.AddItems(result);

            var endTime = DateTime.Now;
            Console.WriteLine("{0} done adding items after {1:n2} seconds..\n",
                              endTime, (endTime - startTime).TotalSeconds );
        }

        private Task<List<ItemViewModel>> LoadItems()
        {
            return Task.Run<List<ItemViewModel>>(() =>
            {
                List<ItemViewModel> result = null;
                result = new List<ItemViewModel>();

                IsProcessing = true;
                try
                {
                    var windowsFolder = ShellBrowser.Create(KF_IID.ID_FOLDERID_Windows, true);

                    string dirPath = System.IO.Path.Combine(windowsFolder.PathFileSystem, "WinSxs");

                    Console.WriteLine("Retrieving all StageZero sub-directories from '{0}'...\n", dirPath);

                    // List all known folders
                    var startTime = DateTime.Now;
                    Console.WriteLine("...{0} working on it...\n", startTime);

                    int i = 0;
                    foreach (var item in ShellBrowser.GetStageZeroChildItems(dirPath))
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
