namespace BreadcrumbTestLib.ViewModels.TreeLookupProcessors
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BmLib.Enums;

    /// <summary>
    /// Set Parent ViewModel's ITreeSelector.SelectedChild to Current ViewModel's Value.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class SetChildSelected<VM, T> : ITreeLookupProcessor<VM, T>
    {
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
        {
            Logger.InfoFormat("_");

            if (hr == HierarchicalResult.Child || hr == HierarchicalResult.Current)
                parentSelector.SelectedChild = selector.Value;

            return true;
        }
    }
}
