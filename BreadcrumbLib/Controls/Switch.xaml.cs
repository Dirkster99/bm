namespace BreadcrumbLib.Controls
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
		#endregion properties

		#region Methods
		public static void OnIsSwitchOnChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
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
