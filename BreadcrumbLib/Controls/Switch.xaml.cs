namespace BreadcrumbLib.Controls
{
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
		#endregion
	}
}
