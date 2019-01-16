namespace SuggestBoxTestLib.ViewModels
{
    using SuggestBoxTestLib.AutoSuggest;
    using SuggestBoxTestLib.DataSources;
    using SuggestBoxTestLib.DataSources.Auto;
    using SuggestBoxTestLib.DataSources.Directory;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AppViewModel : Base.ViewModelBase
    {
        #region constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        public AppViewModel()
        {
            DummySuggestions = new DummySuggestions();

            // Construct SuggestBoxAuto properties
            FakeViewModel fvm = new FakeViewModel();
            SuggestBoxAuto_SuggestSources = new AutoSuggestSource(new LocationIndicator(fvm));
            SuggestBoxAuto_SuggestSources.SetProcessing(true);
            var t = ConstructHierarchy(fvm);
            t.ContinueWith((result) =>
            {
                SuggestBoxAuto_SuggestSources.SetProcessing(false);
            });

            // Construct SuggestBoxAuto2 properties
            var fakeViewModel = new LocationIndicator(FakeViewModel.GenerateFakeViewModels(TimeSpan.FromSeconds(0.5)));
            SuggestBoxAuto2_SuggestSources = new AutoSuggestSource(fakeViewModel);

            // Construct properties for diskpath suggestion demo
            SuggestDirectory = new DirectorySuggestSource();
            SuggestDirectoryWithRecentList = new DirectorySuggestSource();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a simple data provider object that will always suggest a list of 2 items
        /// based on any given input string (by adding a constant output to any given input).
        /// </summary>
        public DummySuggestions DummySuggestions { get; }

        /// <summary>
        /// Gets the datasource that is queried for the SuggestBoxAuto sample SuggestBox.
        /// </summary>
        public AutoSuggestSource SuggestBoxAuto_SuggestSources { get; }

        /// <summary>
        /// Gets the datasource that is queried for the SuggestBoxAuto2 sample SuggestBox.
        /// </summary>
        public AutoSuggestSource SuggestBoxAuto2_SuggestSources { get; }

        #region DiskPathSuggestBox
        /// <summary>
        /// Gets a suggestions data source that can produce file based suggestions.
        /// </summary>
        public DirectorySuggestSource SuggestDirectory { get; }

        /// <summary>
        /// Gets a suggestions data source that can produce file based suggestions
        /// and is also supported by a recent list of previously selected entries.
        /// </summary>
        public DirectorySuggestSource SuggestDirectoryWithRecentList { get; }
        #endregion DiskPathSuggestBox
        #endregion properties

        #region methods
        /// <summary>
        /// Returns a tree of <see cref="FakeViewModel"/> items with a depth
        /// of <paramref name="iLevels"/> and
        /// a number of <paramref name="iSubDirectories"/> per item.
        /// 
        /// Be careful when playing with these parameters because memory consumption
        /// grows exponentially if you increase both parameters or only one of them
        /// by a large amount :-(
        /// </summary>
        /// <param name="fvm"></param>
        /// <param name="iLevels"></param>
        /// <param name="iSubDirectories"></param>
        /// <returns></returns>
        private Task<FakeViewModel> ConstructHierarchy(
            FakeViewModel fvm,
            int iLevels = 9,
            int iSubDirectories = 5)
        {
            return Task.Run(() =>
            {
                List<FakeViewModel> models = new List<FakeViewModel>();
                int counter = 0;
                Queue<Tuple<int, FakeViewModel>> queue = new Queue<Tuple<int, FakeViewModel>>();

                if (fvm != null)
                    queue.Enqueue(new Tuple<int, FakeViewModel>(0, fvm));

                while (queue.Count() > 0)
                {
                    var queueItem = queue.Dequeue();
                    int iLevel = queueItem.Item1;
                    FakeViewModel current = queueItem.Item2;

                    if (iLevel < iLevels)
                    {
                        models.Clear();
                        for (int i = 0; i < iSubDirectories; i++, counter++)
                        {
                            string nextItem = "Sub" + (iLevel + 1) + "_" + counter;
                            var vm = new FakeViewModel()
                            {
                                Header = nextItem,
                                Value = (current.Value + "\\" + nextItem),
                                //Latency = TimeSpan.FromSeconds(0.1),
                                Parent = current
                            };

                            if (iLevel == 0)
                                vm.Value = nextItem;

                            current.AddSubDirectoryItem(vm);
                            models.Add(vm);
                        }

                        if (iLevel + 1 <= iLevels) // end this as soon as we reached the max level
                        {
                            foreach (var item in models)
                                queue.Enqueue(new Tuple<int, FakeViewModel>(iLevel + 1, item));
                        }
                    }
                }

                return fvm;
            });
        }
        #endregion methods
    }
}
