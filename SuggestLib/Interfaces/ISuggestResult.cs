namespace SuggestLib.Interfaces
{
    using System.Collections.Generic;

    public interface ISuggestResult
    {
        IList<object> Suggestions { get; }

        bool ValidPath { get; set; }
    }
}