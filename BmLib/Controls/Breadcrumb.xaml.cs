namespace BmLib.Controls
{
    using BmLib.Controls.Breadcrumbs;
    using BmLib.Interfaces;
    using SuggestLib;
    using SuggestLib.Events;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// BreadcrumbControl contains 3 main displays:
    /// 1 - Display a BreadcrumbTree control with a switch to
    ///     2 - A SuggestBox (textbox with suggested drop down options)
    ///     
    ///       2.1 - Switch is toggled ON via toggle button on root of BreadcrumbTree
    ///       2.2 - Switch is toggled ON via mouse click in OverflowGap (right most part of BreadcrumbTree view)
    ///             2.3 - Switch is toggled OFF via Cancel (Escape) or
    ///                                             OK (Enter) on SuggestBox
    ///                                                -> new path navigation or error
    ///     
    /// and
    /// 3 - A ToggleButton,
    ///     and when ToggleButton is clicked, show it's content as a
    ///     combobox dropdown.
    /// </summary>
    [TemplatePart(Name = PART_SuggestBox, Type = typeof(SuggestBoxBase))]
    [TemplatePart(Name = PART_Switch, Type = typeof(Switch))]
    [TemplatePart(Name = PART_RootDropDownList, Type = typeof(DropDownList))]
    [TemplatePart(Name = PART_BreadcrumbTree, Type = typeof(BreadcrumbTree))]
    public class Breadcrumb : UserControl
    {
        #region fields
        public const string PART_RootDropDownList = "PART_RootDropDownList";
        public const string PART_Switch = "PART_Switch";
        public const string PART_SuggestBox = "PART_SuggestBox";
        public const string PART_BreadcrumbTree = "PART_BreadcrumbTree";

        #region Switch DPs
        /// <summary>
        /// Backing store of the <see cref="SwitchHeader"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SwitchHeaderProperty =
            DependencyProperty.Register("SwitchHeader", typeof(object),
                typeof(Breadcrumb), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="IsSwitchOn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSwitchOnProperty =
            DependencyProperty.Register("IsSwitchOn", typeof(bool),
                typeof(Breadcrumb), new PropertyMetadata(true, OnIsSwitchOnChanged));

        /// <summary>
        /// Backing store of the <see cref="IsSwitchEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSwitchEnabledProperty =
            DependencyProperty.Register("IsSwitchEnabled", typeof(bool),
                typeof(Breadcrumb), new PropertyMetadata(true));

        /// <summary>
        /// Backing store of the <see cref="SwitchTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SwitchTemplateProperty =
            DependencyProperty.Register("SwitchTemplate",
                typeof(ControlTemplate), typeof(Breadcrumb), new PropertyMetadata(null));
        #endregion Switch DPs

        #region Tree dependency properties
        /// <summary>
        /// Backing store of the <see cref="TreeItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TreeItemTemplateProperty =
            DependencyProperty.Register("TreeItemTemplate", typeof(DataTemplate),
                typeof(Breadcrumb), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="TreeItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TreeItemsSourceProperty =
            DependencyProperty.Register("TreeItemsSource", typeof(System.Collections.IEnumerable),
                typeof(Breadcrumb), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="TreeItemContainerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TreeItemContainerStyleProperty =
            DependencyProperty.Register("TreeItemContainerStyle", typeof(Style),
                typeof(Breadcrumb), new PropertyMetadata(null));
        #endregion Tree dependency properties

        /// <summary>
        /// Backing store of the <see cref="DropDownListItemDataTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DropDownListItemDataTemplateProperty =
            DependencyProperty.Register("DropDownListItemDataTemplate", typeof(DataTemplate),
                typeof(Breadcrumb), new PropertyMetadata(null));

        /// <summary>
        /// Implements the backing store field of the OverflowGap dependency property.
        /// 
        /// The OverflowGap dependency property defines the gap
        /// that is displayed in the right most part of the BreadcrumbTree
        /// view to let the user click into this area when the switch should
        /// be turned on to display the text display with the SuggestBox.
        /// </summary>
        public static readonly DependencyProperty OverflowGapProperty =
            DependencyProperty.Register("OverflowGap",
                typeof(double), typeof(Breadcrumb), new PropertyMetadata(10.0));

        /// <summary>
        /// Implement a dependency property to determine whether all path portions of the
        /// breadcrumb contol fit into the current breadcrumb display or not.
        /// </summary>
        public static readonly DependencyProperty IsOverflownProperty =
            DependencyProperty.Register("IsOverflown",
                typeof(bool),
                typeof(Breadcrumb), new PropertyMetadata(false));

        #region RootDropDown
        /// <summary>
        /// Getter/Setter for dependency property that binds the root items drop down list
        /// of the Breadcrumb control. This list should also include overflown items
        /// (overflown items are items that cannot be displayed in the path portion of the
        /// control since UI space is too limited).
        /// </summary>
        public static readonly DependencyProperty RootDropDownItemsSourceProperty =
            DependencyProperty.Register("RootDropDownItemsSource",
                typeof(IEnumerable<object>), typeof(Breadcrumb), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="RootDropDownSelectedValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RootDropDownSelectedValueProperty =
            DependencyProperty.Register("RootDropDownSelectedValue",
                typeof(object), typeof(Breadcrumb), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="RootDropDownSelectedValuePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RootDropDownSelectedValuePathProperty =
            DependencyProperty.Register("RootDropDownSelectedValuePath",
                typeof(string), typeof(Breadcrumb), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="RootDropDownListHeader"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RootDropDownListHeaderProperty =
            DependencyProperty.Register("RootDropDownListHeader", typeof(object),
                typeof(Breadcrumb), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="RootDropDownListItemContainerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RootDropDownListItemContainerStyleProperty =
            DependencyProperty.Register("RootDropDownListItemContainerStyle", typeof(Style),
                typeof(Breadcrumb), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="RootDropDownSelectionChangedCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RootDropDownSelectionChangedCommandProperty =
            DependencyProperty.Register("RootDropDownSelectionChangedCommand", typeof(ICommand),
                typeof(Breadcrumb), new PropertyMetadata(null));
        #endregion RootDropDown

        /// <summary>
        /// Backing store of the <see cref="Progressing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProgressingProperty =
            DependencyProperty.Register("Progressing", typeof(IProgress),
                typeof(Breadcrumb), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="TaskQueueProcessing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TaskQueueProcessingProperty =
            DependencyProperty.Register("TaskQueueProcessing", typeof(IBrowseRequestTaskQueue),
                typeof(Breadcrumb), new PropertyMetadata(null));

        private object _LockObject = new object();
        private bool _IsLoaded;
        private IBreadcrumbModel _BreadcrumbModel;
        private EditResult _SwitchBoxEditResult;

        /// <summary>
        /// Save location before switching from TreeView to suggestbox to enable
        /// cancelling and roll-back to previous location if required.
        /// </summary>
        private string _previousLocation;
        private bool _SwitchToOnCanceled;
        #endregion fields

        #region constructors
        /// <summary>
        /// Static constructor
        /// </summary>
        static Breadcrumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Breadcrumb),
                    new System.Windows.FrameworkPropertyMetadata(typeof(Breadcrumb)));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public Breadcrumb()
        {
        }
        #endregion constructors

        #region properties
        #region Switch DPs
        /// <summary>
        /// Gets/sets the content definition of the RootSwitchButton shown in the
        /// left most spot of the breadcrumb control.
        /// </summary>
        public object SwitchHeader
        {
            get { return (object)GetValue(SwitchHeaderProperty); }
            set { SetValue(SwitchHeaderProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether the <see cref="Switch"/> control that can host 2 controls
        /// is currently switched:
        /// 1) On  (true)  (Showing BreadcrumbTree control) or
        /// 2) off (false) (Showing TextInput control)
        /// </summary>
        public bool IsSwitchOn
        {
            get { return (bool)GetValue(IsSwitchOnProperty); }
            set { SetValue(IsSwitchOnProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether the <see cref="Switch"/> control that can host 2 controls
        /// can currently by switched by the user (by clicking into the <see cref="OverflowGap"/>
        /// or clicking the left most root toggle button) or not.
        /// </summary>
        public bool IsSwitchEnabled
        {
            get { return (bool)GetValue(IsSwitchEnabledProperty); }
            set { SetValue(IsSwitchEnabledProperty, value); }
        }

        /// <summary>
        /// Gets/sets the control template for the <see cref="Switch"/> control implemented
        /// inside the <see cref="Breadcrumb"/> control. The <see cref="Switch"/> control
        /// is used to switch between the Tree and the <see cref="SuggestBox"/>  view if
        /// the <see cref="Breadcrumb"/> control should support both views.
        /// </summary>
        public ControlTemplate SwitchTemplate
        {
            get { return (ControlTemplate)GetValue(SwitchTemplateProperty); }
            set { SetValue(SwitchTemplateProperty, value); }
        }
        #endregion DPs

        #region Tree dependency properties
        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display
        /// each item in the breadcrumb tree control.
        ///
        /// Returns:
        /// A System.Windows.DataTemplate that specifies the visualization of the data objects.
        /// The default is null (none).
        /// </summary>
        [Bindable(true)]
        public DataTemplate TreeItemTemplate
        {
            get { return (DataTemplate)GetValue(TreeItemTemplateProperty); }
            set { SetValue(TreeItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets a collection used to generate the content of the
        /// inner System.Windows.Controls.ItemsControl in the breadcrumb tree control.
        ///
        /// Returns:
        ///     A collection that is used to generate the content of the System.Windows.Controls.ItemsControl.
        ///     The default is null.
        /// </summary>
        [Bindable(true)]
        public System.Collections.IEnumerable TreeItemsSource
        {
            get { return (System.Collections.IEnumerable)GetValue(TreeItemsSourceProperty); }
            set { SetValue(TreeItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> that is applied to the container element
        /// generated for each item of the breadcrumb tree.
        ///
        /// Returns:
        /// The <see cref="Style"/> that is applied to the container element generated for
        /// each item of the breadcrumb tree. The default is null.
        /// </summary>
        [Bindable(true)]
        [Category("Content")]
        public Style TreeItemContainerStyle
        {
            get { return (Style)GetValue(TreeItemContainerStyleProperty); }
            set { SetValue(TreeItemContainerStyleProperty, value); }
        }
        #endregion Tree dependency properties

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display
        /// the:
        /// 1) Root DropDownList of the Breadcrumb control and the
        /// 2) DropDownList of each item in the breadcrumb tree control.
        ///
        /// Returns:
        /// A <see cref="DataTemplate"/> that specifies the visualization of the data objects.
        /// The default is null (none).
        /// </summary>
        public DataTemplate DropDownListItemDataTemplate
        {
            get { return (DataTemplate)GetValue(DropDownListItemDataTemplateProperty); }
            set { SetValue(DropDownListItemDataTemplateProperty, value); }
        }

        /// <summary>
        /// Implement a dependency property to determine whether all path portions of the
        /// breadcrumb contol fit into the current breadcrumb display or not.
        /// </summary>
        public bool IsOverflown
        {
            get { return (bool)GetValue(IsOverflownProperty); }
            set { SetValue(IsOverflownProperty, value); }
        }

        /// <summary>
        /// Implements the OverflowGap dependency property which is the gap
        /// that is displayed in the right most part of the BreadcrumbTree
        /// view to let the user click into this area when the switch should
        /// be turned on to display the text display with the SuggestBox.
        /// </summary>
        public double OverflowGap
        {
            get { return (double)GetValue(OverflowGapProperty); }
            set { SetValue(OverflowGapProperty, value); }
        }

        #region RootDropDown
        /// <summary>
        /// Getter/Setter for dependency property that binds the RootItemsDropDownList
        /// of the Breadcrumb control. This list should also include overflown items
        /// (overflown items are items that cannot be displayed in the path portion of the
        /// control since UI space is too limited).
        /// </summary>
        public IEnumerable<object> RootDropDownItemsSource
        {
            get { return (IEnumerable<object>)GetValue(RootDropDownItemsSourceProperty); }
            set { SetValue(RootDropDownItemsSourceProperty, value); }
        }

        /// <summary>
        /// Getter/Setter for dependency property that binds the Selected Value of the
        /// RootItemsDropDownList of the Breadcrumb control.
        /// </summary>
        public object RootDropDownSelectedValue
        {
            get { return (object)GetValue(RootDropDownSelectedValueProperty); }
            set { SetValue(RootDropDownSelectedValueProperty, value); }
        }

        /// <summary>
        /// Getter/Setter to determine the Selected Value Path for the Selected Value of
        /// the RootItemsDropDownList of the Breadcrumb control.
        /// </summary>
        public string RootDropDownSelectedValuePath
        {
            get { return (string)GetValue(RootDropDownSelectedValuePathProperty); }
            set { SetValue(RootDropDownSelectedValuePathProperty, value); }
        }

        /// <summary>
        /// Identifies the Header dependency property.
        ///
        /// Returns:
        /// The identifier for the Header dependency property.
        /// </summary>
        public object RootDropDownListHeader
        {
            get { return (object)GetValue(RootDropDownListHeaderProperty); }
            set { SetValue(RootDropDownListHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> that is applied to the container element
        /// generated for each item of the root drop down list in the breadcrumb control.
        ///
        /// Returns:
        /// The <see cref="Style"/> that is applied to the container element generated for
        /// each item of the root drop down list in the breadcrumb control. The default is null.
        /// </summary>
        [Bindable(true)]
        [Category("Content")]
        public Style RootDropDownListItemContainerStyle
        {
            get { return (Style)GetValue(RootDropDownListItemContainerStyleProperty); }
            set { SetValue(RootDropDownListItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Gets/sets the <see cref="ICommand"/> interface that determines the command
        /// to be invoked when the selected item in the RootDropDownList was changed by
        /// the user.
        /// </summary>
        public ICommand RootDropDownSelectionChangedCommand
        {
            get { return (ICommand)GetValue(RootDropDownSelectionChangedCommandProperty); }
            set { SetValue(RootDropDownSelectionChangedCommandProperty, value); }
        }
        #endregion RootDropDown

        /// <summary>
        /// Gets/sets the <see cref="IProgress"/> based values required to display
        /// a progress display when a long running back task is processing data.
        /// </summary>
        public IProgress Progressing
        {
            get { return (IProgress)GetValue(ProgressingProperty); }
            set { SetValue(ProgressingProperty, value); }
        }

        /// <summary>
        /// Gets/sets the <see cref="IBrowseRequestTaskQueue"/> based values required to display
        /// status about the ability to cancel currently executing tasks and commands
        /// to actually cancel executing tasks.
        /// </summary>
        public IBrowseRequestTaskQueue TaskQueueProcessing
        {
            get { return (IBrowseRequestTaskQueue)GetValue(TaskQueueProcessingProperty); }
            set { SetValue(TaskQueueProcessingProperty, value); }
        }

        private SuggestBoxBase Control_SuggestBox { get; set; }

        private Switch Control_Switch { get; set; }

        private DropDownList RootDropDownList { get; set; }

        private BreadcrumbTree Control_Tree { get; set; }
        #endregion properties

        #region methods
        /// <summary>
		/// Is called when a control template is applied.
		/// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RootDropDownList = this.Template.FindName(PART_RootDropDownList, this) as DropDownList;   // bexp
            Control_Switch = this.Template.FindName(PART_Switch, this) as Switch;                  // switch
            Control_SuggestBox = this.Template.FindName(PART_SuggestBox, this) as SuggestBoxBase; // sbox
            Control_Tree = this.Template.FindName(PART_BreadcrumbTree, this) as BreadcrumbTree;

            if (Control_SuggestBox != null)
            {
                Control_SuggestBox.NewLocationRequestEvent += Control_SuggestBox_NewLocationRequestEvent;
            }

            this.Loaded += Breadcrumb_Loaded;
            DataContextChanged += Breadcrumb_DataContextChanged;
        }

        /// <summary>
        /// Measures the child elements of a <seealso cref="StackPanel"/> 
        /// in anticipation of arranging them during the
        /// <seealso cref="StackPanel.ArrangeOverride(System.Windows.Size)"/>
        /// </summary>
        /// <param name="constraint">An upper limit <seealso cref="Size"/> that should not be exceeded.</param>
        /// <returns>The System.Windows.Size that represents the desired size of the element.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
#if DEBUG
            if (double.IsPositiveInfinity(constraint.Width)) // || double.IsPositiveInfinity(constraint.Height))
            {
                // This constrain hints a layout proplem that can cause items to NOT Overflow.
                Debug.WriteLine("    +---> Warning: Breadcrumb.MeasureOverride(Size constraint) with constraint == Infinity");
            }
#endif
            var sz = base.MeasureOverride(constraint);

            if (RootDropDownItemsSource != null)         // Go through root items and
            {                                           // count those that are overflown
                int overflowCount = 0;
                foreach (var item in RootDropDownItemsSource)
                {
                    if (item is IOverflown)
                    {
                        if ((item as IOverflown).IsOverflown)
                            overflowCount++;
                    }
                    else
                        break;
                }

                // Set dependency property to determine whether control is overlown or not
                IsOverflown = (overflowCount > 0 ? true : false);
            }

            return sz; // Return current size constrain
        }

        /// <summary>
        /// Method executes when the Switch property changed its value.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIsSwitchOnChanged(DependencyObject d,
                                                DependencyPropertyChangedEventArgs e)
        {
            var dp = d as Breadcrumb;
            
            if (dp == null)
                return;

            dp.OnSwitchChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private async void OnSwitchChanged(bool OldValue, bool NewValue)
        {
            // Switch from suggestbox to tree view
            if (OldValue == false && NewValue == true)
            {
                bool isPathValid = true, goBackToPreviousLocation = false;
                string path;

                if (_SwitchBoxEditResult != null)
                {
                    // Editing was cancelled by the user (eg.: user pressed Escape key)
                    if (_SwitchBoxEditResult.Result == EditPathResult.Cancel)
                    {
                        path = string.Empty;
                        goBackToPreviousLocation = true;
                    }
                    else
                        path = _SwitchBoxEditResult.NewLocation;
                }
                else
                    path = Control_SuggestBox.Text;

                if (_BreadcrumbModel == null)
                    isPathValid = false;      // Cannot invoke viewmodel method
                else
                    isPathValid = await _BreadcrumbModel.NavigateTreeViewModel(path, goBackToPreviousLocation);

                // Canceling navigation from edit result since path appears to be invalid
                // > Stay with false path if path does not exist
                //   or viewmodel method cannot be invoked here ...
                if (isPathValid == false)
                {
                    _SwitchToOnCanceled = true;
                    IsSwitchOn = false;  // Switch back to SuggestBox
                }
            }
            else
            {
                // Switch from tree view to suggestbox
                if (OldValue == true && NewValue == false)
                {
                    // Assumption: Control is already bound and current location
                    // is available in text property
                    if (_SwitchToOnCanceled == false)
                    {
                        if (_BreadcrumbModel == null)
                        {
                            // Overwrite this only if we are not in a cancel-suggestion-workflow
                            _previousLocation = Control_SuggestBox.Text;
                        }
                        else
                        {
                            _previousLocation = _BreadcrumbModel.UpdateSuggestPath();
                        }
                    }
                    else
                        _SwitchToOnCanceled = false;

                    await Control_SuggestBox.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Control_SuggestBox.InitializeSuggestions();
                        Keyboard.Focus(Control_SuggestBox);
                        Control_SuggestBox.SelectAll();
                        Control_SuggestBox.Focus();

                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
            }

            _SwitchBoxEditResult = null;
        }

        /// <summary>
        /// Method executes when the SuggestionBox signals that editing location
        /// has been OK'ed (user pressed enter) or cancel'ed (user pressed Escape).
        /// 
        /// These signals are then recorded and processed via IsSwitchOn property
        /// handler which can also be invoked via Toggle Button which is processed
        /// as OK.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_SuggestBox_NewLocationRequestEvent(
            object sender,
            SuggestLib.Events.NextTargetLocationArgs e)
        {
            _SwitchBoxEditResult = e.EditResult;

            // The user requests a new location via SuggestBox Text control
            // lets have the switch do the lifting of navigating the tree view
            if (IsSwitchOn == false)
                IsSwitchOn = true;
        }

        private void Breadcrumb_DataContextChanged(object sender,
                                                   DependencyPropertyChangedEventArgs e)
        {
            lock (_LockObject)
            {
                if (_IsLoaded == true)
                {
                    if (e.NewValue != null)
                        OnViewAttached();
                }
            }
        }

        /// <summary>
        /// Method executes when the control has finished loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Breadcrumb_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            lock (_LockObject)
            {
                _IsLoaded = true;

                if (DataContext != null)
                    OnViewAttached();
            }
        }

        private void OnViewAttached()
        {
            this._BreadcrumbModel = this.DataContext as IBreadcrumbModel;
        }
        #endregion methods
    }
}
