namespace BmLib.Controls
{
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;

    /// <summary>
    /// DropDownList is a DropDown control that contains a list of items.
    /// </summary>
	public class DropDownList : ComboBox
	{
		#region fields
        /// <summary>
        /// Implements the backing staore of the <see cref="Header"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty HeaderProperty =
				HeaderedContentControl.HeaderProperty.AddOwner(typeof(DropDownList));

        /// <summary>
        /// Implements the backing staore of the <see cref="HeaderButtonTemplate"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty HeaderButtonTemplateProperty =
				DropDown.HeaderButtonTemplateProperty.AddOwner(typeof(DropDownList));

        /// <summary>
        /// Implements the backing staore of the <see cref="PlacementTarget"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty PlacementTargetProperty =
				Popup.PlacementTargetProperty.AddOwner(typeof(DropDownList));

        /// <summary>
        /// Implements the backing staore of the <see cref="Placement"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty PlacementProperty =
				Popup.PlacementProperty.AddOwner(typeof(DropDownList));

        /// <summary>
        /// Implements the backing staore of the <see cref="HorizontalOffset"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty HorizontalOffsetProperty =
				Popup.HorizontalOffsetProperty.AddOwner(typeof(DropDownList));

        /// <summary>
        /// Implements the backing staore of the <see cref="VerticalOffset"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty VerticalOffsetProperty =
			 Popup.VerticalOffsetProperty.AddOwner(typeof(DropDownList));
		#endregion fields

		#region Constructor
		/// <summary>
		/// Static class constructor
		/// </summary>
		static DropDownList()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownList),
					new System.Windows.FrameworkPropertyMetadata(typeof(DropDownList)));
		}
		#endregion

		#region properties
        /// <summary>
        /// Gets/sets the content of the <see cref="DropDown"/> button that
        /// is clicked to open the drop down list.
        /// </summary>
		public object Header
		{
			get { return this.GetValue(HeaderProperty); }
			set { this.SetValue(HeaderProperty, value); }
		}

        /// <summary>
        /// Gets/sets the <see cref="ControlTemplate"/> of the
        /// <see cref="DropDown"/> button that is clicked to open the drop down list.
        /// </summary>
		public ControlTemplate HeaderButtonTemplate
		{
			get { return (ControlTemplate)GetValue(HeaderButtonTemplateProperty); }
			set { this.SetValue(HeaderButtonTemplateProperty, value); }
		}

        /// <summary>
        /// Gets/sets the PlacementTarget (<see cref="UIElement"/>) of the
        /// <see cref="Popup"/> control that opens to show the <see cref="DropDownList"/>
        /// when the <see cref="DropDown"/> button is clicked to open the drop down list.
        /// </summary>
		public UIElement PlacementTarget
		{
			get { return (UIElement)GetValue(PlacementTargetProperty); }
			set { this.SetValue(PlacementTargetProperty, value); }
		}

        /// <summary>
        /// Gets/sets the <see cref="PlacementMode"/> of the
        /// <see cref="Popup"/> control that opens to show the <see cref="DropDownList"/>
        /// when the <see cref="DropDown"/> button is clicked to open the drop down list.
        /// </summary>
		public PlacementMode Placement
		{
			get { return (PlacementMode)GetValue(PlacementProperty); }
			set { this.SetValue(PlacementProperty, value); }
		}

        /// <summary>
        /// Gets/sets the HorizontalOffset of the
        /// <see cref="Popup"/> control that opens to show the <see cref="DropDownList"/>
        /// when the <see cref="DropDown"/> button is clicked to open the drop down list.
        /// </summary>
		public double HorizontalOffset
		{
			get { return (double)GetValue(HorizontalOffsetProperty); }
			set { this.SetValue(HorizontalOffsetProperty, value); }
		}

        /// <summary>
        /// Gets/sets the VerticalOffset of the
        /// <see cref="Popup"/> control that opens to show the <see cref="DropDownList"/>
        /// when the <see cref="DropDown"/> button is clicked to open the drop down list.
        /// </summary>
		public double VerticalOffset
		{
			get { return (double)GetValue(VerticalOffsetProperty); }
			set { this.SetValue(VerticalOffsetProperty, value); }
		}
		#endregion properties
	}
}
