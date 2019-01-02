namespace SuggestBoxTestLib.AutoSuggest
{
    using SuggestLib.Interfaces;
    using SuggestLib.SuggestSource;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Class implments a <see cref="ISuggestSource"/> that can in turn
    /// query multiple <see cref="ISuggestSource"/>s.
    /// </summary>
    public class MultiSuggestSource : ISuggestSource
    {
        #region fields
        private ISuggestSource[] _suggestSources;
        #endregion fields

        #region ctors
        /// <summary>
        /// Parameterized class constructor
        /// </summary>
        /// <param name="suggestSources"></param>
        public MultiSuggestSource(ISuggestSource source1, params ISuggestSource[] moreSources)
            : this()
        {
            _suggestSources = (new ISuggestSource[] { source1 }).Concat(moreSources).ToArray();
        }

        /// <summary>
        /// Parameterized class constructor
        /// </summary>
        /// <param name="suggestSources"></param>
        public MultiSuggestSource(params ISuggestSource[] suggestSources)
            : this()
        {
            _suggestSources = suggestSources;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected MultiSuggestSource()
        {
        }
        #endregion ctors

        /// <summary>
        /// Method returns a task that returns a list of suggestion objects
        /// that are associated to the <paramref name="input"/> string
        /// and given <paramref name="location"/> object.
        /// 
        /// The list of suggestion is empty if helper object is null.
        /// 
        /// This method is usually directly invoked by the SuggestBox to query data
        /// sources for suggestions as the user types a string character by character.
        /// </summary>
        /// <param name="location">Represents the root of the hierarchy that is browsed here.</param>
        /// <param name="input">Is the input string typed by the user.</param>
        /// <returns></returns>
        public async Task<ISuggestResult> SuggestAsync(object location,
                                                       string input)
        {
            ISuggestResult result = new SuggestResult();

            foreach (var ss in _suggestSources)
            {
                await ss.SuggestAsync(location, input);
            }

            return result;
        }
    }
}
