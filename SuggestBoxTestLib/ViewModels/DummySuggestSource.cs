namespace SuggestBoxTestLib.ViewModels
{
    using BmLib.Interfaces.SuggestBox;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class DummySuggestSource : ISuggestSource
    {
        public Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper)
        {
            return Task.FromResult<IList<object>>(new List<object>()
                {
                     new { Header = input + "-add xyz", Value = input + "xyz" },
                     new { Header = input + "-add abc", Value = input + "abc" }
                });
        }

        public bool RunInDispatcher { get { return false; } }
    }
}
