namespace SuggestBoxTestLib.AutoSuggest
{
    using SuggestBoxTestLib.AutoSuggest.Interfaces;
    using SuggestLib.Interfaces;
    using SuggestLib.SuggestSource;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Implements a suggestion object to generate suggestions
    /// based on sub entries of specified data.
    /// </summary>
    public class AutoSuggestSource : ISuggestSource
    {
        #region Constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public AutoSuggestSource()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method returns a task that returns a list of suggestion objects
        /// that are associated to the <paramref name="input"/> string
        /// and given <paramref name="data"/> object.
        /// 
        /// The list of suggestion is empty if helper object is null.
        /// 
        /// This method is usually directly invoked by the SuggestBox to query data
        /// sources for suggestions as the user types a string character by character.
        /// </summary>
        /// <param name="location">Represents the root of the hierarchy that is browsed here.</param>
        /// <param name="input">Is the input string typed by the user.</param>
        /// <returns></returns>
        public Task<ISuggestResult> SuggestAsync(object location,
                                                 string input)
        {
            ISuggestResult retVal = new SuggestResult();

            var li = location as LocationIndicator;
            if (li == null)
                return Task.FromResult<ISuggestResult>(retVal);

            IHierarchyHelper hhelper = li.HierarchyHelper;

            if (hhelper == null)
                return Task.FromResult<ISuggestResult>(retVal);

            // Get the path from input string: 'c:\Windows' -> path: 'c:\'
            string valuePath = hhelper.ExtractPath(input);

            // Get the name from input string: 'c:\Windows' -> path: 'Windows'
            string valueName = hhelper.ExtractName(input);

            // Ensure that name ends with seperator if input ended with a seperator
            if (String.IsNullOrEmpty(valueName) && input.EndsWith(hhelper.Separator + ""))
                valueName += hhelper.Separator;

            // Ensure valid path if input ends with seperator and path was currently empty
            if (valuePath == "" && input.EndsWith("" + hhelper.Separator))
                valuePath = valueName;

            var found = hhelper.GetItem(li.RootItem, valuePath);

            if (found != null)
            {
                foreach (var item in hhelper.List(found))
                {
                    string valuePathName = hhelper.GetPath(item) as string;

                    if (valuePathName.StartsWith(input, hhelper.StringComparisonOption) &&
                        !valuePathName.Equals(input, hhelper.StringComparisonOption))
                        retVal.Suggestions.Add(item);
                }
            }

            return Task.FromResult<ISuggestResult>(retVal);
        }
        #endregion
    }
}
