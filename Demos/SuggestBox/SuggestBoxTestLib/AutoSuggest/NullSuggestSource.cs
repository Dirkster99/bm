namespace SuggestBoxTestLib.AutoSuggest
{
    using SuggestLib.Interfaces;
    using SuggestLib.SuggestSource;
    using System.Threading.Tasks;

    /// <summary>
    /// Class implements an <see cref="ISuggestSource"/> data source of suggestion
    /// that does not return any results (only a default - empty list is returned).
    /// </summary>
    public class NullSuggestSource : ISuggestSource
    {
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
        public Task<ISuggestResult> SuggestAsync(object location, string input)
        {
            return Task.Run<ISuggestResult>(() => new SuggestResult());
        }
    }
}
