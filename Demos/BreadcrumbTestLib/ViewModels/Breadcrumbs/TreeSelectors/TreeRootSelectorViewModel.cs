namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using BreadcrumbTestLib.ViewModels.Breadcrumbs.TreeSelectors;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Linq;

    /// <summary>
    /// Implemented in tree node view model, to provide selection support
    /// for the list of root drop down items.
    /// </summary>
    /// <typeparam name="VM">Sub-node viewmodel type.</typeparam>
    /// <typeparam name="M">Type to identify a node, commonly string.</typeparam>
    internal class TreeRootSelectorViewModel<VM, M> : TreeSelectorViewModel<VM, M>, ITreeRootSelector<VM, M>
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected new static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private M _selectedValue = default(M);
        private VM _SelectedViewModel = default(VM);
        #endregion fields

        #region constructors
        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="entryHelper"></param>
        public TreeRootSelectorViewModel(IBreadcrumbTreeItemHelperViewModel<VM> entryHelper)
          : base(entryHelper)
        {
        }
        #endregion constructors

        #region events
        /// <summary>
        /// Raises an event whenever the currently selected item has changed.
        /// </summary>
        public event EventHandler SelectionChanged;
        #endregion events

        #region properties
        /// <summary>
        /// Gets the currently selected viewmodel from the currently selected item.
        /// </summary>
        public VM SelectedViewModel
        {
            get
            {
                return _SelectedViewModel;
            }

            protected set
            {
                _SelectedViewModel = value;
                NotifyPropertyChanged(() => SelectedViewModel);
            }
        }

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
        public M SelectedValue
        {
            get
            {
                return _selectedValue;
            }

            set
            {
                bool bHasChanged = true;

                if (_selectedValue == null && value == null)
                    bHasChanged = false;
                else
                {
                    if ((_selectedValue != null && value == null) ||
                        (_selectedValue == null && value != null))
                        bHasChanged = true;
                    else
                    {
                        bHasChanged = ! _selectedValue.Equals(value);
                    }
                }

                if (bHasChanged == true)
                {
                    _selectedValue = value;
                    NotifyPropertyChanged(() => this.SelectedValue);
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Method is invoked at the end of each navigational change when the
        /// control's viewmodel has retrieved all required items and is about
        /// to change the selection to complete the cycle.
        /// </summary>
        /// <param name="pathItem"></param>
        public async Task ReportChildSelectedAsync(ITreeSelector<VM, M> pathItem)
        {
            Logger.InfoFormat("_");

            try
            {
                if (pathItem != null)
                {
                    SelectedViewModel = pathItem.ViewModel;
                    SelectedValue = pathItem.Value;
                }
                else
                {
                    SelectedViewModel = default(VM);
                    SelectedValue = default(M);
                }
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }

            if (this.SelectionChanged != null)
                this.SelectionChanged(this, EventArgs.Empty);

            if (pathItem.EntryHelper.IsLoaded == false)
                await pathItem.EntryHelper.LoadAsync();
        }
        #endregion
    }
}
