namespace BreadcrumbTestLib.Models
{
    using System.Threading.Tasks;

    internal interface IRoot<M>
    {
        /// <summary>
        /// Navigates the viewmodel (and hopefully the bound control) to a new location
        /// and ensures correct <see cref="IsBrowsing"/> state and event handling towards
        /// listing objects for
        /// <see cref="ICanNavigate"/> events.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        Task NavigateTo(M location);
    }
}
