namespace BreadcrumbTestLib.ViewModels.Interfaces
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implemented in tree node view model, to provide selection support.
    /// </summary>
    /// <typeparam name="VM">Sub-node viewmodel type.</typeparam>
    /// <typeparam name="T">Type to identify a node, commonly string.</typeparam>
    public interface ITreeRootSelector<VM, T> : ITreeSelector<VM, T> ////, ICompareHierarchy<T>
    {
        #region events
        /// <summary>
        /// Raised when a node is selected, use SelectedValue/ViewModel to return the selected item.
        /// </summary>
        event EventHandler SelectionChanged;
        #endregion events

        #region properties
        /// <summary>
        /// Selected node.
        /// </summary>
        VM SelectedViewModel { get; }

        /// <summary>
        /// Get instance of ITreeSelector of the selected ViewModel.
        /// </summary>
        ITreeSelector<VM, T> SelectedSelector { get; }

        /// <summary>
        /// Value of SelectedViewModel.
        /// </summary>
        T SelectedValue { get; set; }

        /// <summary>
        /// Used by Breadcrumb, items that are root items or ALL items in the selected path, 
        /// There's another Filter in xaml that only 
        /// </summary>
        IEnumerable<VM> OverflowedAndRootItems { get; }
        #endregion properties

        /// <summary>
        /// Update the root drop down list with the list of root items
        /// and overflowable (non-root) items.
        /// </summary>
        /// <param name="rootItems"></param>
        /// <param name="pathItems"></param>
        void UpdateOverflowedItems(IEnumerable<VM> rootItems, IEnumerable<VM> pathItems);
    }
}
