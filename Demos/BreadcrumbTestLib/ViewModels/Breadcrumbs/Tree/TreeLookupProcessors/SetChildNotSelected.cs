namespace BreadcrumbTestLib.ViewModels.TreeLookupProcessors
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BmLib.Enums;

    /// <summary>
    /// Set ViewModel's ITreeSelector.SelectedChild to null.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class SetChildNotSelected<VM, T> : ITreeLookupProcessor<VM, T>
    {
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Set ViewModel's ITreeSelector.SelectedChild to null if it's child of current lookup.
        /// </summary>
////        public static SetChildNotSelected<VM, T> WhenChild = new SetChildNotSelected<VM, T>(HierarchicalResult.Child);

        /// <summary>
        ///  Set ViewModel's ITreeSelector.SelectedChild to null if it's NOT a child of current lookup.
        ///  (including unrelated directory and parent of requested directory)
        /// </summary>
        public static SetChildNotSelected<VM, T> WhenNotChild = new SetChildNotSelected<VM, T>(HierarchicalResult.Current |
                HierarchicalResult.Parent | HierarchicalResult.Unrelated);

        private HierarchicalResult _hr;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="hr"></param>
        public SetChildNotSelected(HierarchicalResult hr)
        {
            Logger.InfoFormat("_");
            this._hr = hr;
        }

        public bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
        {
            Logger.InfoFormat("_");

            if (this._hr.HasFlag(hr))
                selector.SelectedChild = default(T);

            return true;
        }
    }
}
