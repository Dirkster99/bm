﻿namespace BreadcrumbTestLib.ViewModels.TreeLookupProcessors
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbLib.Defines;

    /// <summary>
    /// Set ViewModel's EntryHelper.IsExpanded to true.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class SetExpanded<VM, T> : ITreeLookupProcessor<VM, T>
    {
        /// <summary>
        /// Set ViewModel's EntryHelper.IsExpanded to true if it's child of current lookup.
        /// </summary>
        public static SetExpanded<VM, T> WhenChildSelected = new SetExpanded<VM, T>(HierarchicalResult.Child);

        /// <summary>
        /// Set ViewModel's EntryHelper.IsExpanded to true when it's unrelated node of current lookup.
        /// </summary>
        public static SetExpanded<VM, T> WhenSelected = new SetExpanded<VM, T>(HierarchicalResult.Current);

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="matchResult"></param>
        public SetExpanded(HierarchicalResult matchResult)
        {
            this.MatchResult = matchResult;
        }

        private HierarchicalResult MatchResult { get; set; }

        public bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
        {
            if (this.MatchResult.HasFlag(hr))
                selector.EntryHelper.IsExpanded = true;

            // To-Do: Add a interface for IsBringIntoView.
            ////if (hr == FileExplorer.Defines.HierarchicalResult.Current)
            ////    (selector.ViewModel as IDirectoryNodeViewModel).IsBringIntoView = true;
            return true;
        }
    }
}
