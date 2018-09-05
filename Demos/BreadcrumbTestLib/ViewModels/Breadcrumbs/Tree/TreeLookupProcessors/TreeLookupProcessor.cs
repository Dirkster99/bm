namespace BreadcrumbTestLib.ViewModels.TreeLookupProcessors
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using System;
    using BmLib.Enums;

    /// <summary>
    /// Implementation of ITreeLookupProcessor, which used with ITreeSelector and ITreeLookup, 
    /// when ITreeSelector.LookupAsync return any HierarchicalResult, it will be processed by these processors.    
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class TreeLookupProcessor<VM, T> : ITreeLookupProcessor<VM, T>
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Func<HierarchicalResult, ITreeSelector<VM, T>, ITreeSelector<VM, T>, bool> _processFunc;
        private HierarchicalResult _appliedResult;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="appliedResult"></param>
        /// <param name="processFunc"></param>
        public TreeLookupProcessor(HierarchicalResult appliedResult,
            Func<HierarchicalResult, ITreeSelector<VM, T>, ITreeSelector<VM, T>, bool> processFunc)
        {
            this._processFunc = processFunc;
            this._appliedResult = appliedResult;
        }
        #endregion constructors

        #region methodss
        public bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
        {
            if (this._appliedResult.HasFlag(hr))
                return this._processFunc(hr, parentSelector, selector);

            return true;
        }
        #endregion methodss
    }
}
