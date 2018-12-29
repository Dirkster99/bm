namespace BreadcrumbTestLib.ViewModels
{
    /// <summary>
    /// Models the viewmodel item portion of the list of suggestions in the SuggestionBox.
    /// </summary>
    public class SuggestionListItem
    {
        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="headerParam"></param>
        /// <param name="textpathParam"></param>
        /// <param name="parentParam"></param>
        public SuggestionListItem(string headerParam,
                                  string textpathParam,
                                  object parentParam)
            :this()
        {
            Header = headerParam;
            TextPath = textpathParam;
            Parent = parentParam;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected SuggestionListItem()
        {
        }
        #endregion ctors

        #region properties
        public string Header { get; }

        public string TextPath { get; }

        public object Parent { get; }
        #endregion properties
    }
}
