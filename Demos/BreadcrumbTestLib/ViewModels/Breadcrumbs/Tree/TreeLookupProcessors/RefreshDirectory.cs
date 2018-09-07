namespace BreadcrumbTestLib.ViewModels.TreeLookupProcessors
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BmLib.Enums;

    /// <summary>
    /// Refresh current directory using EntryHelper.LoadAsync()
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    internal class RefreshDirectory<VM, T> : ITreeLookupProcessor<VM, T>
    {
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Refresh current directory using EntryHelper.LoadAsync() if is matched current directory of current lookup.
        /// </summary>
        public static RefreshDirectory<VM, T> WhenFound = new RefreshDirectory<VM, T>(HierarchicalResult.Current);

        private HierarchicalResult _hr;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="hr"></param>
        public RefreshDirectory(HierarchicalResult hr)
        {
            Logger.InfoFormat("_");
            this._hr = hr;
        }

        public bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
        {
            Logger.InfoFormat("_");

            if (this._hr.HasFlag(hr))
                selector.EntryHelper.LoadAsync(UpdateMode.Replace, true);

            return true;
        }
    }
}
