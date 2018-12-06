namespace SuggestLib.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISuggestSource
    {
        Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper);
    }
}
