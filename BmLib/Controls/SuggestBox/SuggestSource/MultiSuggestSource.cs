namespace SuggestBoxDemo.SuggestSource
{
    using BmLib.Interfaces.SuggestBox;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class MultiSuggestSource : ISuggestSource
    {
        private ISuggestSource[] _suggestSources;
        public MultiSuggestSource(params ISuggestSource[] suggestSources)
        {
            _suggestSources = suggestSources;
        }

        public MultiSuggestSource(ISuggestSource source1, params ISuggestSource[] moreSources)
        {
            _suggestSources = (new ISuggestSource[] { source1 }).Concat(moreSources).ToArray();
        }

        public async Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper)
        {
            List<object> retVal = new List<object>();
            foreach (var ss in _suggestSources)
                retVal.AddRange(await ss.SuggestAsync(data, input, helper));

            return retVal;
        }
    }
}
