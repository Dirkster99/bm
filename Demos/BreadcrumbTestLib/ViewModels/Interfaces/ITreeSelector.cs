namespace BreadcrumbTestLib.ViewModels.Interfaces
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Implement Tree based structure and support LookupProcessing.
    /// </summary>
    /// <typeparam name="VM">Sub-node viewmodel type.</typeparam>
    /// <typeparam name="T">Type to identify a node, commonly string.</typeparam>
    public interface ITreeSelector<VM, T> : INotifyPropertyChanged
    {
        #region properties
        /// <summary>
        /// Whether current view model is selected.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// This is marked by TreeRootSelector, for overflow menu support.
        /// </summary>
        bool IsRoot { get; set; }

        /// <summary>
        /// Based on IsRoot and IsChildSelected
        /// </summary>
        bool IsRootAndIsChildSelected { get; }

        /// <summary>
        /// Gets whether a child of current view model is selected.
        /// </summary>
        bool IsChildSelected { get; }

        /// <summary>
        /// Gets the selected child of current view model.          
        /// </summary>
        T SelectedChild { get; set; }

        /// <summary>
        /// Lycj: Removed SetSelectedChild and SetIsSelected.
        /// 
        /// The selected child of current view model, for use in UI only.          
        /// </summary>
        T SelectedChildUI { get; set; }

        /// <summary>
        /// Gets the instance of the model object that represents this selection helper.
        /// The model backs the <see cref="ViewModel"/> property and should be in sync
        /// with it.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Gets the owning ViewModel of this selection helper.
        /// </summary>
        VM ViewModel { get; }

        /// <summary>
        /// Gets the parent's ViewModel <see cref="ITreeSelector"/>.
        /// </summary>
        ITreeSelector<VM, T> ParentSelector { get; }

        /// <summary>
        /// Gets the root's ViewModel <see cref="ITreeSelector"/>.
        /// </summary>
        ITreeRootSelector<VM, T> RootSelector { get; }

        /// <summary>
        /// Gets All sub-entries of the current tree item
        /// to support loading tree items.
        /// </summary>
        IBreadcrumbTreeItemHelperViewModel<VM> EntryHelper { get; }

        /// <summary>
        /// Gets whether this entry is currently overflowed (should be hidden
        /// because its to large for display) or a root element, or both.
        /// 
        /// This can be used by binding system to determine whether an element should
        /// be visble in the root drop down list, because overflowed or root
        /// items should be visible in the root drop down list for
        /// overflowed and root items.
        /// </summary>
        bool IsOverflowedOrRoot { get; }

        /// <summary>
        /// Gets/sets whether the BreadCrumb Tree item is currently overflowed
        /// (does not fit into the view display area) or not.
        /// </summary>
        bool IsOverflowed { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Used by a tree node to report to it's root it's selected.
        /// </summary>
        /// <param name="path"></param>
        void ReportChildSelected(Stack<ITreeSelector<VM, T>> path);

        /// <summary>
        /// Used by a tree node to report to it's parent it's deselected.
        /// </summary>
        /// <param name="path"></param>
        void ReportChildDeselected(Stack<ITreeSelector<VM, T>> path);

        /// <summary>        
        /// Find requested node using lookupProc using type T, after any HierarchicalResult,
        /// use processors to perform further action.
        /// </summary>
        /// <example>
        /// //This will select the C:\Temp Node.
        /// await LookupAsync(@"c:\temp", RecursiveSearch.LoadSubentriesIfNotLoaded, SetSelected.WhenSelected);
        /// </example>
        /// <param name="value"></param>
        /// <param name="lookupProc"></param>
        /// <param name="cancelToken"></param>
        /// <param name="processors"></param>
        /// <returns></returns>
        Task LookupAsync(T value,
                             ITreeLookup<VM, T> lookupProc,
                     CancellationToken cancelToken,
                             params ITreeLookupProcessor<VM, T>[] processors);
        #endregion methods
    }
}
