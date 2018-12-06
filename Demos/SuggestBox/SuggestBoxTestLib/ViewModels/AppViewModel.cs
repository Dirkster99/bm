namespace SuggestBoxTestLib.ViewModels
{
    using SuggestLib;
    using SuggestLib.Interfaces;
    using SuggestLib.SuggestSource;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AppViewModel : Base.ViewModelBase
    {
        #region fields
        private bool _Processing;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        public AppViewModel()
        {
            SuggestBoxDummy_SuggestSources = new List<ISuggestSource>(new[] { new DummySuggestSource() });

            FakeViewModel fvm = new FakeViewModel();

            this.Processing = true;
            var t = ConstructHierarchy(fvm);
            t.ContinueWith((result) =>
            {
                this.Processing = false;
            });

            // Construct SuggestBoxAuto properties
            SuggestBoxAuto_RootItems = fvm;
            SuggestBoxAuto_HierarchyHelper =
                    new PathHierarchyHelper("Parent", "Value", "SubDirectories");

            // Construct SuggestBoxAuto2 properties
            SuggestBoxAuto2_HierarchyHelper
                = new PathHierarchyHelper("Parent", "Value", "SubDirectories");

            SuggestBoxAuto2_RootItems = FakeViewModel.GenerateFakeViewModels(TimeSpan.FromSeconds(0.5));

            SuggestBoxAuto2_SuggestSources = new List<ISuggestSource>(
                new[] {
                    //This is default value, suggest based on HierarchyLister.List()
                    new AutoSuggestSource()
                });
        }
        #endregion constructors

        #region properties
        public List<ISuggestSource> SuggestBoxDummy_SuggestSources { get; }

        #region SuggestBoxAuto
        public PathHierarchyHelper SuggestBoxAuto_HierarchyHelper { get; }

        public FakeViewModel SuggestBoxAuto_RootItems { get; }
        #endregion SuggestBoxAuto

        #region SuggestBoxAuto2
        public PathHierarchyHelper SuggestBoxAuto2_HierarchyHelper { get; }

        public FakeViewModel SuggestBoxAuto2_RootItems { get; }

        public List<ISuggestSource> SuggestBoxAuto2_SuggestSources { get; }

        public bool Processing
        {
            get { return _Processing; }
            private set
            {
                if (_Processing != value)
                {
                    _Processing = value;
                    NotifyPropertyChanged(() => Processing);
                }
            }
        }
        #endregion SuggestBoxAuto2
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
