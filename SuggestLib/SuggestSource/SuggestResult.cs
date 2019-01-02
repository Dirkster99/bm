namespace SuggestLib.SuggestSource
{
    using SuggestLib.Interfaces;
    using System.Collections.Generic;

    /// <summary>
    /// Models a result class for the drop down portion of the SuggestionBox.
    /// 
    /// Each suggestion source <see cref="ISuggestSource"/> returns one of these objects
    /// containing a list of suggestions and whether the input was considered valid or not.
    /// </summary>
    public class SuggestResult : ISuggestResult
    {
        #region fields
        private readonly List<object> _suggestions;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor from list of suggestions
        /// </summary>
        /// <param name="list"></param>
        public SuggestResult(List<object> list)
            : this()
        {
            _suggestions.AddRange(list);
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public SuggestResult()
        {
            _suggestions = new List<object>();
            ValidPath = true;
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets a list of suugestion based on a given input.
        /// </summary>
        public IList<object> Suggestions
        {
            get { return _suggestions; }
        }

        /// <summary>
        /// Gets/sets whether the given input was considered as valid or not.
        /// </summary>
        public bool ValidPath { get; set; }
        #endregion properties
    }
}
