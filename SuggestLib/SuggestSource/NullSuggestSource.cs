namespace SuggestLib.SuggestSource
{
    using SuggestLib.Interfaces;
    using System.Threading.Tasks;

    public class NullSuggestSource : ISuggestSource
    {
        public Task<ISuggestResult> SuggestAsync(object data, string input, IHierarchyHelper helper)
        {
            return Task.Run<ISuggestResult>(() => new SuggestResult());
        }
    }
}
