namespace SuggestLib.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines properties and methods of an object that is used to generate a list
    /// of suggestion results and whether the given path was considered as valid or not.
    /// 
    /// This type of object is typically used by a <see cref="ISuggestSource"/> object.
    /// </summary>
    public interface ISuggestResult
    {
        /// <summary>
        /// Gets a list of suugestion based on a given input.
        /// </summary>
        IList<object> Suggestions { get; }

        /// <summary>
        /// Gets/sets whether the given input was considered as valid or not.
        /// </summary>
        bool ValidPath { get; set; }
    }
}