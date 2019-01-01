namespace SuggestLib.SuggestSource
{
    using SuggestLib.Interfaces;
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

        public async Task<ISuggestResult> SuggestAsync(object data, string input, IHierarchyHelper helper)
        {
            ISuggestResult result = new SuggestResult();

            foreach (var ss in _suggestSources)
            {
                await ss.SuggestAsync(data, input, helper);
            }

            return result;
        }
    }
}
