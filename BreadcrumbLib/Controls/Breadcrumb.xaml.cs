namespace BreadcrumbLib.Controls
{
    using BreadcrumbLib.Interfaces;
    using BreadcrumbLib.Utils;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Display a ToggleButton and when it's clicked, show it's content as a dropdown.
    /// </summary>
    ////    [TemplatePart(Name = "PART_SuggestBox", Type = typeof(SuggestBoxBase))]
    [TemplatePart(Name = "PART_Switch", Type = typeof(Switch))]
    [TemplatePart(Name = "PART_DropDownList", Type = typeof(DropDownList))]
    public class Breadcrumb : UserControl
    {
        #region fields
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
////        public SuggestBoxBase Control_SuggestBox { get; set; }

        public Switch Control_Switch { get; set; }

        public DropDownList Control_bexp { get; set; }
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
