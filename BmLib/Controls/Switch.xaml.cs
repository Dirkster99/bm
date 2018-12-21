namespace BmLib.Controls
{
    using System.Diagnostics;
    using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Display ContentOn or ContentOff depends on whether IsSwitchOn is true.
	/// </summary>
	public class Switch : HeaderedContentControl
	{
		#region fields
		public static readonly DependencyProperty IsSwitchOnProperty =
				DependencyProperty.Register("IsSwitchOn", typeof(bool),
				typeof(Switch), new UIPropertyMetadata(true, new PropertyChangedCallback(OnIsSwitchOnChanged)));

		public static readonly DependencyProperty ContentOnProperty =
				DependencyProperty.Register("ContentOn", typeof(object),
				typeof(Switch), new UIPropertyMetadata(null));

		public static readonly DependencyProperty ContentOffProperty =
				DependencyProperty.Register("ContentOff", typeof(object),
				typeof(Switch), new UIPropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="CanSwitchContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanSwitchContentProperty =
            DependencyProperty.Register("CanSwitchContent", typeof(bool),
                typeof(Switch), new PropertyMetadata(true));
        #endregion fields

        #region Constructor

        static Switch()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Switch),
					new FrameworkPropertyMetadata(typeof(Switch)));
		}

		#endregion

		#region properties
		public bool IsSwitchOn
		{
			get { return (bool)GetValue(IsSwitchOnProperty); }
			set { this.SetValue(IsSwitchOnProperty, value); }
		}

		public object ContentOn
		{
			get { return (object)GetValue(ContentOnProperty); }
			set { this.SetValue(ContentOnProperty, value); }
		}

		public object ContentOff
		{
			get { return (object)GetValue(ContentOffProperty); }
			set { this.SetValue(ContentOffProperty, value); }
		}

        /// <summary>
        /// Gets/sets whether the <see cref="Switch"/> control that can host 2 controls
        /// can currently by switched by the user (by clicking the left most root toggle
        /// button) or not.
        /// </summary>
        public bool CanSwitchContent
        {
            get { return (bool)GetValue(CanSwitchContentProperty); }
            set { SetValue(CanSwitchContentProperty, value); }
        }
        #endregion properties

        #region Methods
        public static void OnIsSwitchOnChanged(object sender,
                                               DependencyPropertyChangedEventArgs args)
		{
////            var dp = sender as Switch;
////
////            if (dp == null)
////                return;
////
////            System.Console.WriteLine("OnIsSwitchOnChanged {0}:{1} -> {2}", sender, args.OldValue, args.NewValue);
        }

		/// <summary>
		/// Is called when a control template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

            ////this.AddHandler(HeaderedContentControl.MouseDownEvent, (RoutedEventHandler)((o, e) =>
            ////    {
            ////        this.SetValue(IsSwitchOnProperty, !IsSwitchOn);
            ////    }));
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
                Debug.WriteLine("   +---> Warning: Switch.MeasureOverride(Size constraint) with constraint == Infinity");
            }
#endif
            return base.MeasureOverride(constraint);
        }
        #endregion
    }
}
