namespace Breadcrumb.ViewModels.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    /// <summary>
    /// Implemented in tree node view model, to provide selection support.
    /// </summary>
    /// <typeparam name="VM">Sub-node viewmodel type.</typeparam>
    /// <typeparam name="T">Type to identify a node, commonly string.</typeparam>
    public interface ITreeRootSelector<VM, T> : ITreeSelector<VM, T>, ICompareHierarchy<T>
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
        /// Indicate a list to compare Hierarchy of two value that identify a node.
        /// </summary>
        IEnumerable<ICompareHierarchy<T>> Comparers { get; set; }

        /// <summary>
        /// Used by Breadcrumb, items that are root items or ALL items in the selected path, 
        /// There's another Filter in xaml that only 
        /// </summary>
        ObservableCollection<VM> OverflowedAndRootItems { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Select a tree node by value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SelectAsync(T value, bool force = false);
        #endregion methods
    }
}
