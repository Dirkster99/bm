namespace BmLib.Controls
{
    using BmLib.Controls.Breadcrumbs;
    using BmLib.Interfaces;
    using BmLib.Utils;
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
    /// 
    /// </summary>
    ////    [TemplatePart(Name = "PART_SuggestBox", Type = typeof(SuggestBoxBase))]
    [TemplatePart(Name = "PART_Switch", Type = typeof(Switch))]
    [TemplatePart(Name = PART_RootDropDownList, Type = typeof(DropDownList))]
    public class Breadcrumb : UserControl
    {
        #region fields
        public const string PART_RootDropDownList = "PART_RootDropDownList";

        /// <summary>
        /// Backing store of the <see cref="SwitchHeader"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SwitchHeaderProperty =
            DependencyProperty.Register("SwitchHeader", typeof(object),
                typeof(Breadcrumb), new PropertyMetadata(null));

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

        private object _LockObject = new object();
        private bool _IsLoaded = false;
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
            this.DataContextChanged += Breadcrumb_DataContextChanged;
            this.Loaded += Breadcrumb_Loaded;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets/sets the content definition of the RootSwitchButton shown in the
        /// left most spot of the breadcrumb control.
        /// </summary>
        public object SwitchHeader
        {
            get { return (object)GetValue(SwitchHeaderProperty); }
            set { SetValue(SwitchHeaderProperty, value); }
        }

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
        /// Getter/Setter for dependency property that binds the root items drop down list
        /// of the Breadcrumb control. This list should also include overflown items
        /// (overflown items are items that cannot be displayed in the path portion of the
        /// control since UI space is too limited).
        /// </summary>
        public IEnumerable<object> RootDropDownItemsSource
        {
            get { return (IEnumerable<object>)GetValue(RootDropDownItemsSourceProperty); }
            set { SetValue(RootDropDownItemsSourceProperty, value); }
        }

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

        ////        public SuggestBoxBase Control_SuggestBox { get; set; }

        public Switch Control_Switch { get; set; }

        public DropDownList RootDropDownList { get; set; }

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
            Control_Switch = this.Template.FindName("PART_Switch", this) as Switch;                 // switch
                                                                                                    ////        Control_SuggestBox = this.Template.FindName("PART_SuggestBox", this) as SuggestBoxBase;  // sbox

            OnViewAttached();
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

        private void OnViewAttached()
        {
            this.RootDropDownList.AddValueChanged(ComboBox.SelectedValueProperty, (o, e) =>
            {
                IEntryViewModel evm = this.RootDropDownList.SelectedItem as IEntryViewModel;
                ////                    if (evm != null)
                ////                        BroadcastDirectoryChanged(evm);

                Control_Switch.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    Control_Switch.IsSwitchOn = true;
                }));
            });

            this.Control_Switch.AddValueChanged(Switch.IsSwitchOnProperty, (o, e) =>
            {
                if (!this.Control_Switch.IsSwitchOn)
                {
                    ////                        _sbox.Dispatcher.BeginInvoke(new System.Action(() =>
                    ////                        {
                    ////                            Keyboard.Focus(_sbox);
                    ////                            _sbox.Focus();
                    ////                            _sbox.SelectAll();
                    ////                        }), System.Windows.Threading.DispatcherPriority.Background);
                }
            });
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
        /// Method executes when the datacontext is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Breadcrumb_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
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
        #endregion methods
    }
}
