namespace BmLib.Controls
{
    using BmLib.Controls.Breadcrumbs;
    using BmLib.Interfaces;
    using SuggestLib;
    using SuggestLib.Events;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Threading;

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
        /// <summary>
        /// Gets the templated name of the required <see cref="DropDownList"/> control.
        /// </summary>
        public const string PART_RootDropDownList = "PART_RootDropDownList";

        /// <summary>
        /// Gets the templated name of the required <see cref="Switch"/> control.
        /// </summary>
        public const string PART_Switch = "PART_Switch";

        /// <summary>
        /// Gets the templated name of the required <see cref="SuggestBox"/> control.
        /// </summary>
        public const string PART_SuggestBox = "PART_SuggestBox";

        /// <summary>
        /// Gets the templated name of the required <see cref="BreadcrumbTree"/> control.
        /// </summary>
        public const string PART_BreadcrumbTree = "PART_BreadcrumbTree";

        /// <summary>
        /// Backing store of the <see cref="PathValidation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PathValidationProperty =
            DependencyProperty.Register("PathValidation", typeof(ValidationRule),
                typeof(Breadcrumb), new PropertyMetadata(null));


        /// <summary>
        /// Backing store of the <see cref="RecentLocationsItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RecentLocationsItemsSourceProperty =
            DependencyProperty.Register("RecentLocationsItemsSource", typeof(IEnumerable),
                typeof(Breadcrumb), new PropertyMetadata(null));

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
                typeof(Breadcrumb), new PropertyMetadata(true, OnIsSwitchChanged));

        /// <summary>
        /// Backing store of the <see cref="IsSwitchEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSwitchEnabledProperty =
            DependencyProperty.Register("IsSwitchEnabled", typeof(bool),
                typeof(Breadcrumb), new PropertyMetadata(true));

        /// <summary>
        /// Backing store of the <see cref="SwitchStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SwitchStyleProperty =
            DependencyProperty.Register("SwitchStyle", typeof(Style),
                typeof(Breadcrumb), new PropertyMetadata(null));
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
        /// Implements the backing store of the <see cref="OverflowGap"/> dependency property.
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

        /// <summary>
        /// Save location before switching from TreeView to suggestbox to enable
        /// cancelling and roll-back to previous location if required.
        /// </summary>
        private string _previousLocation;

        private bool _focusControsOnSwitch;

        private ICommand _SwitchCommand;
        private ICommand _RecentListCommand;
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
        /// <summary>
        /// Gets a command that opens the drop down list of recent locations
        /// to let the user select a Windows Shell or File System path that
        /// can be edit within the <see cref="SuggestBox"/> control.
        /// </summary>
        public ICommand RecentListCommand
        {
            get
            {
                if (_RecentListCommand == null)
                {
                    _RecentListCommand = new Base.RelayCommand<object>(
                            (p) => RecentListCommandExecutedAsync(p)
                    );
                }

                return _RecentListCommand;
            }
        }

        /// <summary>
        /// Gets/sets a validation rule object that can be used to validate a
        /// path and mark it as invalid when path information is entered into
        /// the textual portion of the <see cref="SuggestBox"/> in this control.
        /// </summary>
        public ValidationRule PathValidation
        {
            get { return (ValidationRule)GetValue(PathValidationProperty); }
            set { SetValue(PathValidationProperty, value); }
        }

        /// <summary>
        /// Gets/sets a list of recent locations which can be viewed in the
        /// recent locations drop down.
        /// </summary>
        public IEnumerable RecentLocationsItemsSource
        {
            get { return (IEnumerable)GetValue(RecentLocationsItemsSourceProperty); }
            set { SetValue(RecentLocationsItemsSourceProperty, value); }
        }

        #region Switch Properties
        /// <summary>
        /// Gets a command to switch the view between TreeView and TextBox based view
        /// (between BmLib.BreadcrumbTree and SuggestLib.SuggestBox).
        /// 
        /// Switching the view from text based editing back to TreeView can be canceled
        /// by parameterizing this command with an <see cref="EditResult"/> object.
        /// </summary>
        public ICommand SwitchCommand
        {
            get
            {
                if (_SwitchCommand == null)
                {
                    _SwitchCommand = new Base.RelayCommand<object>(

                        async (p) =>
                        {
                            if (IsSwitchEnabled == true)
                                await SwitchCommandExecutedAsync(p);
                        }
                        // Not using this since this would disable the button and the complete switch
                        // but the hosted contorl (treeview) should be usable when this is set to false.
                        //, (p) => IsSwitchEnabled // whether switch command can execute or not
                    );
                }

                return _SwitchCommand;
            }
        }

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
        /// Gets/sets the style for the <see cref="Switch"/> control implemented
        /// inside the <see cref="Breadcrumb"/> control. The <see cref="Switch"/> control
        /// is used to switch between the Tree and the <see cref="SuggestBox"/>  view if
        /// the <see cref="Breadcrumb"/> control should support both views.
        /// </summary>
        public Style SwitchStyle
        {
            get { return (Style)GetValue(SwitchStyleProperty); }
            set { SetValue(SwitchStyleProperty, value); }
        }
        #endregion Switch Properties

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

        private DropDownList _PART_RootDropDownList { get; set; }

        private BreadcrumbTree _PART_Tree { get; set; }

        private Switch _PART_Switch { get; set; }

        private SuggestBox _PART_SuggestBox { get; set; }

        private SuggestComboBox _PART_SuggestComboBox { get; set; }
        #endregion properties

        #region methods
        /// <summary>
		/// Is called when a control template is applied.
		/// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _PART_RootDropDownList = this.Template.FindName(PART_RootDropDownList, this) as DropDownList;
            _PART_Tree = this.Template.FindName(PART_BreadcrumbTree, this) as BreadcrumbTree;
            _PART_Switch = this.Template.FindName(PART_Switch, this) as Switch;
            _PART_SuggestBox = this.Template.FindName(PART_SuggestBox, this) as SuggestBox;
            _PART_SuggestComboBox = this.Template.FindName("PART_SuggestComboBox", this) as SuggestComboBox;

            if (_PART_SuggestBox != null)
            {
                _PART_SuggestBox.NewLocationRequestEvent += Control_SuggestBox_NewLocationRequestEvent;
            }

            var Ctrl_CancelSuggestion = this.Template.FindName("PART_CancelSuggestion", this) as ButtonBase;
            if (Ctrl_CancelSuggestion != null)
            {
                Ctrl_CancelSuggestion.Click += Ctrl_CancelSuggestion_Click;
            }

            if (_PART_SuggestComboBox != null)
            {
                _PART_SuggestComboBox.SelectionChanged += _PART_SuggestComboBox_SelectionChangedAsync;
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
            UpdateIsOverflownProperty();

            return sz; // Return current size constrain
        }

        /// <summary>
        /// Updates this <see cref="IsOverflown"/> property based on the state of the
        /// items in the <see cref="RootDropDownItemsSource"/> property.
        /// </summary>
        private void UpdateIsOverflownProperty()
        {
            try
            {
                if (RootDropDownItemsSource != null)         // Go through root items and
                {                                           // count those that are overflown
                    var list = RootDropDownItemsSource.ToList();

                    int overflowCount = 0;
                    foreach (var item in list)
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
            }
            catch
            {
                // Catch this just in case something changes in the backend during this evaluation...
            }
        }

        private void RecentListCommandExecutedAsync(object parameter)
        {
            if (_PART_SuggestComboBox != null)
            {
                _PART_SuggestComboBox.SelectedItem = null;
                _PART_SuggestComboBox.IsDropDownOpen = true;
            }
        }

        /// <summary>
        /// Method executes when the user has selected an item in the popup
        /// control of the SuggestComboBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void _PART_SuggestComboBox_SelectionChangedAsync(object sender,
                                                                       SelectionChangedEventArgs e)
        {
            if ((sender is ComboBox) == false)
                return;

            if (e.AddedItems.Count > 0)
            {
                string input = null;
                input = e.AddedItems[0] as string;
                if (string.IsNullOrEmpty(input) == true)
                {
                    var item = e.AddedItems[0] as ComboBoxItem;
                    if (item != null)
                        input = item.Content as string;
                }

                if (string.IsNullOrEmpty(input) == false)
                {
                    // Turn the switch on to SuggestBox if not already available
                    if (IsSwitchOn != false)
                        await SwitchCommandExecutedAsync(null, false);

                    _PART_SuggestBox.Text = input;
                    _PART_SuggestBox.SelectAll();

                    // Focus the newly switched UI element (requires Focusable="True")
                    await _PART_Switch.FocusSwitchAsync(false);
                }
            }
        }

        /// <summary>
        /// Method executes as part of the <see cref="SwitchCommand"/> which tests whether
        /// we are ready to switch between views and takes care of basic plumbing on the way.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="focusControl">Whether or not controls in the switched target content
        /// should get the Focus or not.</param>
        private async Task SwitchCommandExecutedAsync(object parameter,
                                                      bool focusControl = true)
        {
            if (IsSwitchOn == true) // Switching from TreeView to Suggestbox
            {
                MarkInvalidInputSuggestBox(false, null);

                // Assumption: Control is already bound and current location
                // is available in text property
                if (_BreadcrumbModel == null)
                {
                    // Overwrite this only if we are not in a cancel-suggestion-workflow
                    _previousLocation = _PART_SuggestBox.Text;
                }
                else
                {
                    object locations;

                    _PART_SuggestBox.Text = _BreadcrumbModel.UpdateSuggestPath(out locations);
                    _previousLocation = _PART_SuggestBox.Text;

                    _PART_SuggestBox.RootItem = locations;

                    _focusControsOnSwitch = focusControl;
                    IsSwitchOn = false;  // Switch to text based view
                }
            }
            else
            {
                // Switching from Suggestbox to TreeView
                var switchOnTextBoxEditResult = parameter as EditResult;

                bool isPathValid = true, goBackToPreviousLocation = false;
                string path;

                if (switchOnTextBoxEditResult != null)
                {
                    // Editing was cancelled by the user (eg.: user pressed Escape key)
                    if (switchOnTextBoxEditResult.Result == EditPathResult.Cancel)
                    {
                        path = string.Empty;
                        goBackToPreviousLocation = true;
                    }
                    else
                        path = switchOnTextBoxEditResult.NewLocation;
                }
                else
                    path = _PART_SuggestBox.Text;

                if (_BreadcrumbModel == null)
                    isPathValid = false;      // Cannot invoke viewmodel method
                else
                    isPathValid = await _BreadcrumbModel.NavigateTreeViewModel(
                        path, goBackToPreviousLocation, this._PART_SuggestBox.RootItem);

                // Canceling navigation from edit result since path appears to be invalid
                // > Stay with false path if path does not exist
                //   or viewmodel method cannot be invoked here ...
                if (isPathValid == true)
                {
                    _focusControsOnSwitch = focusControl;
                    IsSwitchOn = true;  // Switch on valid path only
                }
                else
                {
                    MarkInvalidInputSuggestBox(true, "Path does not exists.");
                }
            }
        }

        /// <summary>
        /// Method executes when the Switch property changed its value.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIsSwitchChanged(DependencyObject d,
                                              DependencyPropertyChangedEventArgs e)
        {
            var dp = d as Breadcrumb;
            
            if (dp == null)
                return;

            dp.OnSwitchChangedAsync((bool)e.OldValue, (bool)e.NewValue);
        }

        private async void OnSwitchChangedAsync(bool OldValue, bool NewValue)
        {
            // Switch from tree view to suggestbox
            if (OldValue == true && NewValue == false)
            {
                _PART_SuggestBox.InitializeSuggestions();
                _PART_SuggestBox.SelectAll();

                // Focus the newly switched UI element (requires Focusable="True")
                if (_focusControsOnSwitch)
                    await _PART_Switch.FocusSwitchAsync(false);
            }
            else
            {
                // Switch from suggestbox to tree view
                if (OldValue == false && NewValue == true)
                {
                    if (_focusControsOnSwitch)
                        await _PART_Switch.FocusSwitchAsync();
                }
            }
        }

        /// <summary>
        /// Sets or clears a validation error on the SuggestBox
        /// to indicate invalid input to the user.
        /// </summary>
        /// <param name="markError">True: Shows a red validation error rectangle around the SuggestBox
        /// (<paramref name="msg"/> should also be set).
        /// False: Clears previously set validation errors around the Text property of the SuggestBox.
        /// </param>
        /// <param name="msg">Error message (eg.: "invalid input") is set on the binding expression if
        /// <paramref name="markError"/> is true.</param>
        private void MarkInvalidInputSuggestBox(bool markError, string msg)
        {
            if (markError == true)
            {
                // Show a red validation error rectangle around SuggestionBox
                // if validation rule dependency property is available
                if (PathValidation != null)
                {
                    var bindingExpr = _PART_SuggestBox.GetBindingExpression(TextBox.TextProperty);
                    if (bindingExpr != null)
                    {
                        Validation.MarkInvalid(bindingExpr,
                                new ValidationError(PathValidation, bindingExpr, msg, null));
                    }
                }
            }
            else
            {
                // Clear validation error in case it was previously set switching from Text to TreeView
                var bindingExpr = _PART_SuggestBox.GetBindingExpression(TextBox.TextProperty);
                if (bindingExpr != null)
                    Validation.ClearInvalid(bindingExpr);
            }
        }

        /// <summary>
        /// Method executes when user clicks the cancel button in the SuggestionsBox view.
        /// 
        /// The editing is cancelled and switch flips back to previous visiblilty of TreeView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ctrl_CancelSuggestion_Click(object sender, RoutedEventArgs e)
        {
            if (IsSwitchOn == false)
            {
                var ev = new EditResult(EditPathResult.Cancel, string.Empty);

                if (this.SwitchCommand.CanExecute(null) == true)
                    this.SwitchCommand.Execute(ev);
            }
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
        private void Control_SuggestBox_NewLocationRequestEvent(object sender,
                                                            SuggestLib.Events.NextTargetLocationArgs e)
        {
            // The user requests a new location via SuggestBox Text control
            // lets have the switch do the lifting of navigating the tree view
            if (IsSwitchOn == false)
            {
                if (this.SwitchCommand.CanExecute(null) == true)
                    this.SwitchCommand.Execute(e.EditResult);
            }
        }

        #region OnLoaded DataContectChanged
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

        /// <summary>
        /// Updates the link between control and ViewModel to communicate changes
        /// via DataContext when control switches between TreeView and SuggestBox.
        /// 
        /// (method executes when DataContext is changed or control has been loaded
        /// for the first time).
        /// </summary>
        private void OnViewAttached()
        {
            if (_BreadcrumbModel != null)
            {
                _BreadcrumbModel.SelectionChanged -= _BreadcrumbModel_SelectionChanged;
            }

            _BreadcrumbModel = this.DataContext as IBreadcrumbModel;
            _BreadcrumbModel.SelectionChanged += _BreadcrumbModel_SelectionChanged;
        }

        /// <summary>
        /// Method is invoked whenever the selection of the currently
        /// selected breadcrumb item has been changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _BreadcrumbModel_SelectionChanged(object sender, EventArgs e)
        {
            // Do this with low priority to ensure that bindings are updated and
            // IsOverflown property is not currently being changed ...
            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateIsOverflownProperty();

            }, DispatcherPriority.ContextIdle);
        }
        #endregion OnLoaded DataContectChanged
        #endregion methods
    }
}
