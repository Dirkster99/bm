namespace BreadcrumbLib.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// ???
    /// </summary>
    public interface ISuggestSource
    {
        /// <summary>
        /// ???
        /// </summary>
        /// <param name="data"></param>
        /// <param name="input"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper);
    }
}
