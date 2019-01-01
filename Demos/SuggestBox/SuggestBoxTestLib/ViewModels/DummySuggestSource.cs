namespace SuggestBoxTestLib.ViewModels
{
    using SuggestLib.Interfaces;
    using SuggestLib.SuggestSource;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a suggestion object to generate suggestions
    /// based on sub entries of specified string.
    /// </summary>
    public class DummySuggestSource : ISuggestSource
    {
        /// <summary>
        /// Method returns a task that returns a list of suggestion objects
        /// that are associated to the <paramref name="input"/> string
        /// and given <paramref name="data"/> object.
        /// 
        /// This sample is really easy because it simply takes the input
        /// string and add an output as suggestion to the given input.
        /// 
        /// This always returns 2 suggestions.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="input"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public Task<ISuggestResult> SuggestAsync(object data,
                                                string input,
                                                IHierarchyHelper helper)
        {
            var result = new SuggestResult();

            // returns a collection of anynymous objects
            // each with a Header and Value property
            result.Suggestions.Add(new { Header = input + "-add xyz", Value = input + "xyz" });
            result.Suggestions.Add(new { Header = input + "-add abc", Value = input + "abc" });

            return Task.FromResult<ISuggestResult> (result);
        }
    }
}
