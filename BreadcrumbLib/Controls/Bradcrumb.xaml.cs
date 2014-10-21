namespace BreadcrumbLib.Controls
{
	using System.Windows.Controls;

	/// <summary>
	/// Display a ToggleButton and when it's clicked, show it's content as a dropdown.
	/// </summary>
	public class Breadcrumb : UserControl
	{
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
		}
		#endregion constructors
	}
}
