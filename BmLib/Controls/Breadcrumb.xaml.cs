namespace BmLib.Controls
{
    using BmLib.Controls.Breadcrumbs;
    using BmLib.Interfaces;
    using BmLib.Utils;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

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
    [TemplatePart(Name = "PART_DropDownList", Type = typeof(DropDownList))]
    public class Breadcrumb : UserControl
    {
        #region fields
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
        /// Getter/Setter for dependency property that binds the root items drop down list
        /// of the Breadcrumb control. This list should also include overflown items
        /// (overflown items are items that cannot be displayed in the path portion of the
        /// control since UI space is too limited).
        /// </summary>
        public static readonly DependencyProperty RootDropDownItemsSourceProperty =
            DependencyProperty.Register("RootDropDownItemsSource",
                typeof(IEnumerable<object>),
                typeof(Breadcrumb), new PropertyMetadata(null));

        /// <summary>
        /// Implement a dependency property to determine whether all path portions of the
        /// breadcrumb contol fit into the current breadcrumb display or not.
        /// </summary>
        public static readonly DependencyProperty IsOverflownProperty =
            DependencyProperty.Register("IsOverflown",
                typeof(bool),
                typeof(Breadcrumb), new PropertyMetadata(false));

        #region BreadCrumbTree Dependency Properties
        public HierarchicalDataTemplate BreadcrumbTreeItemTemplate
        {
            get { return (HierarchicalDataTemplate)GetValue(BreadcrumbTreeItemTemplateProperty); }
            set { SetValue(BreadcrumbTreeItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BreadcrumbTreeItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BreadcrumbTreeItemTemplateProperty =
            DependencyProperty.Register("BreadcrumbTreeItemTemplate",
                typeof(HierarchicalDataTemplate),
                typeof(Breadcrumb), new PropertyMetadata(null));
        #endregion BreadCrumbTree Dependency Properties

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

        ////        public SuggestBoxBase Control_SuggestBox { get; set; }

        public Switch Control_Switch { get; set; }

        public DropDownList Control_bexp { get; set; }

        private BreadcrumbTree Control_Tree { get; set; }
        #endregion properties

        #region methods
        /// <summary>
		/// Is called when a control template is applied.
		/// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
////        Control_SuggestBox = this.Template.FindName("PART_SuggestBox", this) as SuggestBoxBase;  // sbox
            Control_Switch = this.Template.FindName("PART_Switch", this) as Switch;                 // switch
            Control_bexp = this.Template.FindName("PART_DropDownList", this) as DropDownList;      // bexp

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
            this.Control_bexp.AddValueChanged(ComboBox.SelectedValueProperty, (o, e) =>
            {
                IEntryViewModel evm = this.Control_bexp.SelectedItem as IEntryViewModel;
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
