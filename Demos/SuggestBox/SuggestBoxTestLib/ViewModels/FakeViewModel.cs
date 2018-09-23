namespace SuggestBoxTestLib.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;

    public class FakeViewModel : Base.ViewModelBase
    {
        #region Fields
        private ObservableCollection<FakeViewModel> _subDirectories;
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

        public FakeViewModel(params FakeViewModel[] rootModels)
        {
            Header = "Root";
            Value = "";
            Latency = TimeSpan.FromSeconds(0);
            _subDirectories = new ObservableCollection<FakeViewModel>();
            foreach (var rm in rootModels)
            {
                rm.Parent = this;
                _subDirectories.Add(rm);
            }
        }

        public FakeViewModel(string header, params string[] subHeaders)
        {
            Header = header;
            Value = header;
            SubDirectories = new ObservableCollection<FakeViewModel>();
            foreach (var sh in subHeaders)
                _subDirectories.Add(new FakeViewModel(sh) { Value = header + "\\" + sh, Parent = this });
        }
        #endregion constructors

        #region properties
        public FakeViewModel Parent { get; set; }
        public string Header { get; set; }
        public string Value { get; set; }
        public TimeSpan Latency { get; set; }
        #endregion properties

        #region methods
        public override string ToString()
        {
            return Value;
        }

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

            set { _subDirectories = value; }
        }
        #endregion methods
    }
}
