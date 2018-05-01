namespace BmLib.Controls
{
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;

	public class DropDownList : ComboBox
	{
		#region fields
		public static readonly DependencyProperty HeaderProperty =
				HeaderedContentControl.HeaderProperty.AddOwner(typeof(DropDownList));

		public static readonly DependencyProperty PlacementTargetProperty =
				Popup.PlacementTargetProperty.AddOwner(typeof(DropDownList));

		public static readonly DependencyProperty PlacementProperty =
				Popup.PlacementProperty.AddOwner(typeof(DropDownList));

		public static readonly DependencyProperty HorizontalOffsetProperty =
				Popup.HorizontalOffsetProperty.AddOwner(typeof(DropDownList));

		public static readonly DependencyProperty VerticalOffsetProperty =
			 Popup.VerticalOffsetProperty.AddOwner(typeof(DropDownList));

		public static readonly DependencyProperty HeaderButtonTemplateProperty =
				DropDown.HeaderButtonTemplateProperty.AddOwner(typeof(DropDownList));
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
		public object Header
		{
			get { return this.GetValue(HeaderProperty); }
			set { this.SetValue(HeaderProperty, value); }
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

		public ControlTemplate HeaderButtonTemplate
		{
			get { return (ControlTemplate)GetValue(HeaderButtonTemplateProperty); }
			set { this.SetValue(HeaderButtonTemplateProperty, value); }
		}
		#endregion properties

		#region Methods
		/// <summary>
		/// Is called when a control template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
		}
		#endregion
	}
}
