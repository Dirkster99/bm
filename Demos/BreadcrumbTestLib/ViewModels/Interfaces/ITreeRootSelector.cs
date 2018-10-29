namespace BreadcrumbTestLib.ViewModels.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Implemented in tree node view model, to provide selection support
    /// for the list of root drop down items.
    /// </summary>
    /// <typeparam name="VM">Sub-node viewmodel type.</typeparam>
    /// <typeparam name="M">Type to identify a node, commonly string.</typeparam>
    public interface ITreeRootSelector<VM, M> : ITreeSelector<VM, M>
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
        /// Gets the last item in the path of viewmodel items.
        /// 
        /// Example path is: 'This PC', 'C:', 'Program Files'
        /// -> This property should reference the 'Program Files' item.
        /// </summary>
        ITreeSelector<VM, M> SelectedSelector { get; }

        /// <summary>
        /// Gets/sets the selected second level root item (eg. This PC, Library Desktop, or Desktop Folder).
        /// below the root desktop item.
        /// 
        /// This property usually changes:
        /// 1) When the user opens the drop down and selects 1 item in the dropdownlist of the RootDropDown button or
        /// 2) When the control navigates to a unrelated second level root address
        ///    (eg.: From 'This PC','C:\' to 'Libraries','Music')
        /// 
        /// Source:
        /// DropDownList Binding with SelectedValue="{Binding Selection.SelectedValue}"
        /// </summary>
        M SelectedValue { get; set; }

        /// <summary>
        /// Used by Breadcrumb, items that are root items or ALL items in the selected path, 
        /// There's another Filter in xaml that only 
        /// </summary>
        IEnumerable<VM> OverflowedAndRootItems { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Update the root drop down list with the list of root items
        /// and overflowable (non-root) items.
        /// </summary>
        /// <param name="rootItems"></param>
        /// <param name="pathItems"></param>
        void UpdateOverflowedItems(IEnumerable<VM> rootItems, IEnumerable<VM> pathItems);

        /// <summary>
        /// Used by a tree node to report to it's root it's selected.
        /// </summary>
        /// <param name="pathItem"></param>
        Task ReportChildSelectedAsync(ITreeSelector<VM, M> pathItem);
        #endregion methods
    }
}
