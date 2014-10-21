namespace BreadcrumbLib.BaseControls.Breadcrumb
{
	using System.Windows;
	using System.Windows.Controls;

	public class BreadcrumbOverflowPanel : ItemsControl
	{
		#region fields
		public static readonly DependencyProperty HeaderTemplateProperty =
			 DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(BreadcrumbOverflowPanel), new PropertyMetadata(null));

		public static readonly DependencyProperty IconTemplateProperty =
			 DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(BreadcrumbOverflowPanel), new PropertyMetadata(null));
		#endregion fields

		#region constructors
		/// <summary>
		/// Static class constructor
		/// </summary>
		static BreadcrumbOverflowPanel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbOverflowPanel),
			 new FrameworkPropertyMetadata(typeof(BreadcrumbOverflowPanel)));
		}
		#endregion constructors

		#region properties
		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { this.SetValue(HeaderTemplateProperty, value); }
		}

		public DataTemplate IconTemplate
		{
			get { return (DataTemplate)GetValue(IconTemplateProperty); }
			set { this.SetValue(IconTemplateProperty, value); }
		}
		#endregion properties

		#region Methods
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new BreadcrumbItem(false)
			{
				HeaderTemplate = this.HeaderTemplate,
				IconTemplate = this.IconTemplate,
				ShowToggle = false
			};
		}
		#endregion Methods
	}
}
