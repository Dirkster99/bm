namespace SuggestLib.SuggestSource
{
    using SuggestLib.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class NullSuggestSource : ISuggestSource
    {
        public Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper)
        {
            return Task.Run<IList<object>>(() => new List<object>());
        }
    }
}
