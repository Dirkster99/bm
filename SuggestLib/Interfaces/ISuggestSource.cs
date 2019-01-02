namespace SuggestLib.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a suggestion interface to generate suggestions
    /// based on sub entries of specified data.
    /// </summary>
    public interface ISuggestSource
    {
        /// <summary>
        /// Method returns a task that returns a list of suggestion objects
        /// that are associated to the <paramref name="input"/> string
        /// and given <paramref name="location"/> object.
        /// 
        /// The list of suggestion is empty if helper object is null.
        /// </summary>
        /// <param name="location">Currently selected location
        /// or root of the hierarchy being browsed.</param>
        /// <param name="input">Text input to formulate string based path.</param>
        /// <returns></returns>
        Task<ISuggestResult> SuggestAsync(object location,
                                          string input);
    }
}
