namespace WpfPerformance.ViewModels
{
    using System.Threading.Tasks;

    public interface IItemViewModel
    {
        /// <summary>
        /// Gets a unqie ID within the collection.
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Gets whether current items are already loaded or not.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Gets whether current items are already loaded or not.
        /// </summary>
        bool IsLoaded { get; }

        #region methods
        Task<bool> LoadModel();

        Task<bool> UnlodLoadModel();
        #endregion methods
    }
}
