﻿namespace BreadcrumbTestLib.ViewModels.TreeSelectors
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using BreadcrumbTestLib.ViewModels.Interfaces;

    /// <summary>
    /// Lookup until lookupvalue is found, and process all node.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    internal class RecrusiveBroadcast<VM, T> : ITreeLookup<VM, T>
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///  Lookup until lookupvalue is found, and process all node.  Load entries if not loaded.
        /// </summary>
        public static RecrusiveBroadcast<VM, T> LoadSubentriesIfNotLoaded = new RecrusiveBroadcast<VM, T>(false);

        /// <summary>
        ///  Lookup until lookupvalue is found, and process all node.  Skip if not loaded.
        /// </summary>
        public static RecrusiveBroadcast<VM, T> SkipIfNotLoaded = new RecrusiveBroadcast<VM, T>(false);

        private bool _loadSubEntries;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="loadSubEntries"></param>
        public RecrusiveBroadcast(bool loadSubEntries)
        {
            this._loadSubEntries = loadSubEntries;
        }
        #endregion constructors

        #region methods
        public async Task LookupAsync(T value,
                                      ITreeSelector<VM, T> parentSelector,
                                      ICompareHierarchy<T> comparer,
                                      CancellationToken cancelToken,
                                      params ITreeLookupProcessor<VM, T>[] processors)
        {
            Logger.InfoFormat("_");

            IEnumerable<VM> subentries = this._loadSubEntries ?
                            await parentSelector.EntryHelper.LoadAsync() :
                              parentSelector.EntryHelper.AllNonBindable;

            foreach (VM current in subentries)
            {
                if (cancelToken != CancellationToken.None)
                    cancelToken.ThrowIfCancellationRequested();

                if (current is ISupportTreeSelector<VM, T> && current is ISupportEntriesHelper<VM>)
                {
                    var currentSelectionHelper = (current as ISupportTreeSelector<VM, T>).Selection;
                    var compareResult = comparer.CompareHierarchy(currentSelectionHelper.Value, value);

                    if (processors.Process(compareResult, parentSelector, currentSelectionHelper))
                    {
                        await this.LookupAsync(value, currentSelectionHelper,
                                                           comparer, cancelToken, processors);
                        return;
                    }

                    break;
                }
            }
        }
        #endregion methods
    }
}
