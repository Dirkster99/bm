namespace BreadcrumbTestLib.ViewModels
{
    using WSF.Enums;
    using System;

    /// <summary>
    /// Models the viewmodel item portion of the list of suggestions in the SuggestionBox.
    /// </summary>
    public class SuggestionListItem : IEquatable<SuggestionListItem>
    {
        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="headerParam"></param>
        /// <param name="textpathParam"></param>
        /// <param name="parentParam"></param>
        /// <param name="pathTypeParam"></param>
        public SuggestionListItem(string headerParam,
                                  string textpathParam,
                                  object parentParam,
                                  PathType pathTypeParam)
            :this()
        {
            Header = headerParam;
            TextPath = textpathParam;
            TextPathType = pathTypeParam;
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

        public PathType TextPathType { get; }

        public object Parent { get; }
        #endregion properties

        public bool Equals(SuggestionListItem other)
        {
            if (other == null)
                return false;

            if (TextPathType != other.TextPathType)
                return false;

            if ((string.IsNullOrEmpty(Header) == true && string.IsNullOrEmpty(other.Header) == false) ||
                (string.IsNullOrEmpty(Header) == false && string.IsNullOrEmpty(other.Header) == true) ||
                (string.IsNullOrEmpty(TextPath) == true && string.IsNullOrEmpty(other.TextPath) == false) ||
                (string.IsNullOrEmpty(TextPath) == false && string.IsNullOrEmpty(other.TextPath) == true)
                )
            {
                return false;
            }

            if (string.Compare(Header, other.Header) != 0 ||
                string.Compare(TextPath, other.TextPath) != 0)
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            var otherItem = obj as SuggestionListItem;

            if (otherItem == null)
                return false;

            return Equals(otherItem);
        }

        public override int GetHashCode()
        {
            return (TextPath != null ? TextPath.GetHashCode() : string.Empty.GetHashCode());
        }
    }
}
