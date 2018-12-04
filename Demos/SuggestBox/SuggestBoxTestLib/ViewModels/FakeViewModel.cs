namespace SuggestBoxTestLib.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;

    /// <summary>
    /// Implements a tree like viewmodel to demonstrate usage of the suggestion
    /// boxes with "Sub" string entries.
    /// </summary>
    public class FakeViewModel : Base.ViewModelBase
    {
        #region Fields
        private readonly ObservableCollection<FakeViewModel> _subDirectories;
        bool _loaded = false;
        #endregion Fields

        #region constructors
        private static void generate(FakeViewModel root, int level, string str = "")
        {
            if (level > 0)
                for (int i = 1; i < 5; i++)
                {
                    var vm = new FakeViewModel()
                    {
                        Header = "Sub" + str + i.ToString(),
                        Value = (root.Value + "\\Sub" + str + i.ToString()).TrimStart('\\'),
                        Latency = root.Latency,
                        Parent = root
                    };
                    generate(vm, level - 1, str + i.ToString());
                    root._subDirectories.Add(vm);
                }
        }

        public static FakeViewModel GenerateFakeViewModels(TimeSpan latency)
        {
            var root = new FakeViewModel() { Latency = latency };
            generate(root, 5);
            return root;
        }

        /// <summary>
        /// Class constructor for root item (which has same value and header)
        /// </summary>
        public FakeViewModel()
        {
            Header = "Root";
            Value = "";
            Latency = TimeSpan.FromSeconds(0);
            _subDirectories = new ObservableCollection<FakeViewModel>();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets/sets the parent node of this node in the tree.
        /// </summary>
        public FakeViewModel Parent { get; set; }

        /// <summary>
        /// Gets/sets the header (display string) of this node.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets/sets the actual path used for matching (eg: "Root\Sub1\Sub2").
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets sets a latency that will defer looking up items at retrieval time
        /// (this can also defer construction of a given tree).
        /// </summary>
        public TimeSpan Latency { get; set; }

        /// <summary>
        /// Gets all child nodes of this node.
        /// </summary>
        public ObservableCollection<FakeViewModel> SubDirectories
        {
            get
            {
                if (!_loaded)
                {
                    _loaded = true;
                    Thread.Sleep(Latency);
                }
                return _subDirectories;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Add another childitem into the collection.
        /// </summary>
        /// <param name="item"></param>
        public void AddSubDirectoryItem(FakeViewModel item)
        {
            _subDirectories.Add(item);
        }
        #endregion methods
    }
}
