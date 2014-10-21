namespace BreadcrumbLib.BaseControls
{
	using System.Windows;
	using System.Windows.Controls;

	public class ClipCorner : Border
	{
		#region fields
		//// Dirkster Do not hide inherited property
		////public static readonly DependencyProperty CornerRadiusProperty =
		////		DependencyProperty.Register("CornerRadius", typeof(CornerRadius),
		////		typeof(ClipCorner), new PropertyMetadata(new CornerRadius(0)));
		#endregion fields

		#region constructors
		/// <summary>
		/// Static class constructor
		/// </summary>
		static ClipCorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ClipCorner),
					new FrameworkPropertyMetadata(typeof(ClipCorner)));
		}
		#endregion constructors

		#region properties
		//// Dirkster Do not hide inherited property
		////public CornerRadius CornerRadius
		////{
		////	get { return (CornerRadius)GetValue(CornerRadiusProperty); }
		////	set { this.SetValue(CornerRadiusProperty, value); }
		////}
		#endregion
	}
}
