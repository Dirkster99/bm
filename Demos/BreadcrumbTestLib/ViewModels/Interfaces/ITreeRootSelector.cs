namespace BreadcrumbTestLib.ViewModels.Interfaces
{
    using BreadcrumbTestLib.Models;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;
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
        IEnumerable<VM> OverflowedAndRootItems { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Method can be invoked on the tree root to select a tree node by <paramref name="targetLocation"/>.
        /// </summary>
        /// <param name="targetLocation"></param>
        /// <param name="cancelToken"></param>
        /// <param name="progress"></param>
        /// <returns>Returns a task that selects the requested tree node.</returns>
        Task<FinalBrowseResult<T>> SelectAsync(T targetLocation,
                                               BrowseRequest<string> request,
                                               CancellationToken cancelToken = default(CancellationToken),
                                               IProgressViewModel progress = null);
        #endregion methods
    }
}
