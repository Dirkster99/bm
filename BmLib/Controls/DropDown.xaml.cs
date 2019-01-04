namespace BmLib.Controls
{
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;

    /// <summary>
    /// Display a ToggleButton and when it's clicked, show it's content as a dropdown.
    /// </summary>
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    public class DropDown : HeaderedContentControl
	{
        #region fields
        /// <summary>
        /// Gets the templated name of the required <see cref="Popup"/> control.
        /// </summary>
        public const string PART_Popup = "PART_Popup";

        /// <summary>
        /// Implements the backing staore of the <see cref="IsDropDownOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
				DependencyProperty.Register("IsDropDownOpen", typeof(bool),
				typeof(DropDown), new UIPropertyMetadata(false,
						new PropertyChangedCallback(OnIsDropDownOpenChanged)));

        /// <summary>
        /// Implements the backing staore of the <see cref="IsDropDownAlignLeft"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty IsDropDownAlignLeftProperty =
				DependencyProperty.Register("IsDropDownAlignLeft", typeof(bool),
				typeof(DropDown), new UIPropertyMetadata(false));

        /// <summary>
        /// Implements the backing staore of the <see cref="PlacementTarget"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty PlacementTargetProperty =
				Popup.PlacementTargetProperty.AddOwner(typeof(DropDown));

        /// <summary>
        /// Implements the backing staore of the <see cref="Placement"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty PlacementProperty =
				Popup.PlacementProperty.AddOwner(typeof(DropDown));

        /// <summary>
        /// Implements the backing staore of the <see cref="HeaderButtonTemplate"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty HeaderButtonTemplateProperty =
				DependencyProperty.Register("HeaderButtonTemplate",
                    typeof(ControlTemplate), typeof(DropDown));

        /// <summary>
        /// Implements the backing staore of the <see cref="HorizontalOffset"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty HorizontalOffsetProperty =
				Popup.HorizontalOffsetProperty.AddOwner(typeof(DropDown));

        /// <summary>
        /// Implements the backing staore of the <see cref="VerticalOffset"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty VerticalOffsetProperty =
			 Popup.VerticalOffsetProperty.AddOwner(typeof(DropDown));

		private Popup _popup = null;
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
        /// <summary>
        /// Gets/sets whether the drop down is currently open or not.
        /// </summary>
		public bool IsDropDownOpen
		{
			get { return (bool)GetValue(IsDropDownOpenProperty); }
			set { this.SetValue(IsDropDownOpenProperty, value); }
		}

        /// <summary>
        /// Gets/sets whether the drop down is aligned left or not.
        /// </summary>
		public bool IsDropDownAlignLeft
		{
			get { return (bool)GetValue(IsDropDownAlignLeftProperty); }
			set { this.SetValue(IsDropDownAlignLeftProperty, value); }
		}

        /// <summary>
        /// Gets/sets the PlacementTarget of the pop up control that implements
        /// the drop down.
        /// </summary>
		public UIElement PlacementTarget
		{
			get { return (UIElement)GetValue(PlacementTargetProperty); }
			set { this.SetValue(PlacementTargetProperty, value); }
		}

        /// <summary>
        /// Gets/sets the <see cref="PlacementMode"/> of the pop up control that implements
        /// the drop down.
        /// </summary>
		public PlacementMode Placement
		{
			get { return (PlacementMode)GetValue(PlacementProperty); }
			set { this.SetValue(PlacementProperty, value); }
		}

        /// <summary>
        /// Gets/sets the <see cref="ControlTemplate"/> of the button
        /// that is clicked to open the drop down.
        /// </summary>
		public ControlTemplate HeaderButtonTemplate
		{
			get { return (ControlTemplate)GetValue(HeaderButtonTemplateProperty); }
			set { this.SetValue(HeaderButtonTemplateProperty, value); }
		}

        /// <summary>
        /// Gets/sets horizontal offset of the pop-up control that
        /// impements the drop down.
        /// </summary>
		public double HorizontalOffset
		{
			get { return (double)GetValue(HorizontalOffsetProperty); }
			set { this.SetValue(HorizontalOffsetProperty, value); }
		}

        /// <summary>
        /// Gets/sets vertical offset of the pop-up control that
        /// impements the drop down.
        /// </summary>
		public double VerticalOffset
		{
			get { return (double)GetValue(VerticalOffsetProperty); }
			set { this.SetValue(VerticalOffsetProperty, value); }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Is called when a control template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

            _popup = this.Template.FindName("PART_Popup", this) as Popup;
		}

		private static void OnIsDropDownOpenChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			DropDown ddc = (DropDown)sender;
			if (ddc._popup != null)
			{
				ddc._popup.IsOpen = (bool)args.NewValue;
			}
		}

		#endregion
	}
}
