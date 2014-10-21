namespace BreadcrumbLib.Controls
{
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;

	/// <summary>
	/// Display a ToggleButton and when it's clicked, show it's content as a dropdown.
	/// </summary>
	public class DropDown : HeaderedContentControl
	{
		#region fields
		public static readonly DependencyProperty IsDropDownOpenProperty =
				DependencyProperty.Register("IsDropDownOpen", typeof(bool),
				typeof(DropDown), new UIPropertyMetadata(false,
						new PropertyChangedCallback(OnIsDropDownOpenChanged)));

		public static readonly DependencyProperty IsDropDownAlignLeftProperty =
				DependencyProperty.Register("IsDropDownAlignLeft", typeof(bool),
				typeof(DropDown), new UIPropertyMetadata(false));

		public static readonly DependencyProperty PlacementTargetProperty =
				Popup.PlacementTargetProperty.AddOwner(typeof(DropDown));

		public static readonly DependencyProperty PlacementProperty =
				Popup.PlacementProperty.AddOwner(typeof(DropDown));

		public static readonly DependencyProperty HeaderButtonTemplateProperty =
				DependencyProperty.Register("HeaderButtonTemplate", typeof(ControlTemplate), typeof(DropDown));

		public static readonly DependencyProperty HorizontalOffsetProperty =
				Popup.HorizontalOffsetProperty.AddOwner(typeof(DropDown));

		public static readonly DependencyProperty VerticalOffsetProperty =
			 Popup.VerticalOffsetProperty.AddOwner(typeof(DropDown));

		private Popup _popup = null;
		private ContentPresenter _content = null;
		#endregion fields

		#region constructors
		/// <summary>
		/// Static constructor
		/// </summary>
		static DropDown()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDown),
					new System.Windows.FrameworkPropertyMetadata(typeof(DropDown)));
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		public DropDown()
		{
		}
		#endregion constructors

		#region DependencyProperties
		public bool IsDropDownOpen
		{
			get { return (bool)GetValue(IsDropDownOpenProperty); }
			set { this.SetValue(IsDropDownOpenProperty, value); }
		}

		public bool IsDropDownAlignLeft
		{
			get { return (bool)GetValue(IsDropDownAlignLeftProperty); }
			set { this.SetValue(IsDropDownAlignLeftProperty, value); }
		}

		public UIElement PlacementTarget
		{
			get { return (UIElement)GetValue(PlacementTargetProperty); }
			set { this.SetValue(PlacementTargetProperty, value); }
		}

		public PlacementMode Placement
		{
			get { return (PlacementMode)GetValue(PlacementProperty); }
			set { this.SetValue(PlacementProperty, value); }
		}

		public ControlTemplate HeaderButtonTemplate
		{
			get { return (ControlTemplate)GetValue(HeaderButtonTemplateProperty); }
			set { this.SetValue(HeaderButtonTemplateProperty, value); }
		}

		public double HorizontalOffset
		{
			get { return (double)GetValue(HorizontalOffsetProperty); }
			set { this.SetValue(HorizontalOffsetProperty, value); }
		}

		public double VerticalOffset
		{
			get { return (double)GetValue(VerticalOffsetProperty); }
			set { this.SetValue(VerticalOffsetProperty, value); }
		}

		////IsHeaderEnabled
		////IsDropDownOpen
		#endregion

		#region Methods
		/// <summary>
		/// Is called when a control template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			var popup = this.Template.FindName("PART_Popup", this);
			if (popup is Popup)
			{
				this._popup = (Popup)this.Template.FindName("PART_Popup", this);
				this._content = (ContentPresenter)this.Template.FindName("PART_Content", this);

				this._popup.AddHandler(Popup.LostFocusEvent,
					 new RoutedEventHandler((o, e) =>
					 {
						 ////(o as DropDownControl).                   
						 ////IsDropDownOpen = false;
					 }));
			}
		}

		private static void OnIsDropDownOpenChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			DropDown ddc = (DropDown)sender;
			if (ddc._popup != null)
			{
				ddc._popup.IsOpen = (bool)args.NewValue;
			}

			////if (ddc._content != null)
			////{
			////    ddc._content.Focus();
			////}
			////if (((bool)args.NewValue) && ddc._dropDownGrid != null)
			////{
			////    //Setfocu
			////    //ddc._dropDownGrid.
			////    //Debug.WriteLine(ddc._dropDownGrid.IsFocused);
			////}
		}

		#endregion
	}
}
