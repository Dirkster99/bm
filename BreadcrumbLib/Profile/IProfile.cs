namespace BreadcrumbLib.Profile
{
    using BreadcrumbLib.Interfaces;
    using BreadcrumbLib.Models;
    using Caliburn.Micro;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IProfile
    {
/***
        #region Methods
        IComparer<IEntryModel> GetComparer(ColumnInfo column);
***/
        /// <summary>
        /// Return the entry that represent the path, or null if not exists.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<IEntryModel> ParseAsync(string path);

        Task<IList<IEntryModel>> ListAsync(IEntryModel entry, CancellationToken ct, Func<IEntryModel, bool> filter = null, bool refresh = false);
/***
        /// <summary>
        /// Return the sequence of icon is extracted and returned, EntryViewModel will run each extractor 
        /// and set Icon to it's GetIconForModel() result.
        /// Default is GetDefaultIcon.Instance then GetFromProfile.Instance.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        IEnumerable<IModelIconExtractor<IEntryModel>> GetIconExtractSequence(IEntryModel entry);


        /// <summary>
        /// Whether a path should be parsed (via ParseAsync) by this profile.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool MatchPathPattern(string path);


        #endregion

        #region Data

        #endregion
***/
        #region Public Properties
        /// <summary>
        /// If specified (not null), used as Regex to determine if a path should be parsable by this profile.
        /// </summary>
        string[] PathPatterns { get; }
        string ProfileName { get; }
        byte[] ProfileIcon { get; }
/***
        /// <summary>
        /// Convert Entry Model to another type.
        /// </summary>
        IConverterProfile[] Converters { get; }

        string RootDisplayName { get; }
 ***/
        IPathHelper Path { get; }
        /***
                IEntryHierarchyComparer HierarchyComparer { get; }
                IMetadataProvider MetadataProvider { get; }
                IEnumerable<ICommandProvider> CommandProviders { get; }

        IEventAggregator Events { get; } 
        ***/


        ISuggestSource SuggestSource { get; }
        #endregion
    }
}
