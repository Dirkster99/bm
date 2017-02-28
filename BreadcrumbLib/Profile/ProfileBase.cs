namespace BreadcrumbLib.Profile
{
    using BreadcrumbLib.Controls.SuggestBox;
    using BreadcrumbLib.Interfaces;
    using BreadcrumbLib.Models;
    using BreadcrumbLiv.Viewmodels.Base;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class ProfileBase : NotifyPropertyChanged, IProfile
    {

        #region Constructor

////        public ProfileBase(IEventAggregator events, params IConverterProfile[] converters)
        public ProfileBase(params IConverterProfile[] converters)
        {
            ProfileName = "Unspecified";
            ProfileIcon = null;
            RootDisplayName = "Root";

            Path = PathHelper.Disk;
            SuggestSource = new ProfileSuggestionSource(this);

            HierarchyComparer = PathComparer.LocalDefault;
////            MetadataProvider = new BasicMetadataProvider();
////            CommandProviders = new List<ICommandProvider>();
            //PathMapper = NullDiskPatheMapper.Instance;

////            DragDrop = new NullDragDropHandler();
////            Events = events ?? new EventAggregator();
            _pathPatterns = new string[] { "." };
            Converters = converters;
        }

        #endregion

        #region Methods

////        public virtual IComparer<IEntryModel> GetComparer(ColumnInfo column)
////        {
////            return column.Comparer;
////        }
////
////        public virtual IEnumerable<IModelIconExtractor<IEntryModel>> GetIconExtractSequence(IEntryModel entry)
////        {
////            yield return GetDefaultIcon.Instance;
////        }

        //public virtual IModelIconExtractor<IEntryModel> GetThumbnailExtractor(IEntryModel entry)
        //{
        //    return null;
        //}

        public abstract Task<IEntryModel> ParseAsync(string path);
        public abstract Task<IList<IEntryModel>> ListAsync(IEntryModel entry, CancellationToken ct, Func<IEntryModel, bool> filter = null, bool refresh = false);

        public virtual bool MatchPathPattern(string path)
        {
            if (path == "")
                return true;

            foreach (var pattern in _pathPatterns)
                if (new Regex(pattern, RegexOptions.IgnoreCase).Match(path).Success)
                    return true;
            return false;
        }

        private void setConverters(IConverterProfile[] converters)
        {
            _converters = converters ?? new IConverterProfile[] { };
            foreach (var conv in _converters)
                conv.SetOwner(this);
            _mSuggestSource = new MultiSuggestSource(_suggestSource, Converters.Select(c => c.SuggestSource).ToArray());
        }


        #endregion

        #region Data

        string[] _pathPatterns = new string[] { };
        private IConverterProfile[] _converters = new IConverterProfile[] { };
        private ISuggestSource _suggestSource = new NullSuggestSource();
        private ISuggestSource _mSuggestSource = new NullSuggestSource();

        #endregion

        #region Public Properties
        public string[] PathPatterns
        {
            get { return _pathPatterns; }
            protected set
            {
                _pathPatterns = value;
            }
        }

        public string ProfileName { get; protected set; }
        public byte[] ProfileIcon { get; protected set; }
        public IPathHelper Path { get; protected set; }
        public ISuggestSource SuggestSource
        {
            get { return _mSuggestSource; }
            protected set
            {
                _suggestSource = value;
                _mSuggestSource = new MultiSuggestSource(value, Converters.Select(c => c.SuggestSource).ToArray());
            }
        }
////        public IDragDropHandler DragDrop { get; protected set; }
        public string RootDisplayName { get; protected set; }
        public IEntryHierarchyComparer HierarchyComparer { get; protected set; }
////        public IMetadataProvider MetadataProvider { get; protected set; }
////        public IEnumerable<ICommandProvider> CommandProviders { get; protected set; }
        //public IDiskPathMapper PathMapper { get; protected set; }
////        public IEventAggregator Events { get; protected set; }
        public IConverterProfile[] Converters
        {
            get
            {
                return _converters;
            }

            set
            {
                setConverters(value);
            }
        }
        #endregion

    }
}
