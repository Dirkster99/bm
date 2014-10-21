namespace BreadcrumbLib.Controls.Breadcrumbs
{
  using System.Windows;

  public class BreadcrumbExpander : DropDownList
  {
		#region constructors
		/// <summary>
		/// Static class constructor
		/// </summary>
		static BreadcrumbExpander()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbExpander),
          new FrameworkPropertyMetadata(typeof(BreadcrumbExpander)));
		}
		#endregion constructors
  }
}
