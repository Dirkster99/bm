namespace BreadcrumbTestLib.ViewModels.TreeLookupProcessors
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BmLib.Enums;

    /// <summary>
    /// Set ViewModel's Selector.IsSelected to true 
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class SetSelected<VM, T> : ITreeLookupProcessor<VM, T>
    {
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
////        /// <summary>
////        /// Set ViewModel's Selector.IsSelected to true  if it's lookupvalue of current lookup.
////        /// </summary>
////        public static SetSelected<VM, T> WhenSelected = new SetSelected<VM, T>();

        public bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
        {
            Logger.InfoFormat("_");

            if (hr == HierarchicalResult.Current)
                selector.IsSelected = true;

            return true;
        }
    }
}
