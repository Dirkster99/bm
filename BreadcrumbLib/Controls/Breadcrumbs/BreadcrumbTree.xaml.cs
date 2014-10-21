namespace BreadcrumbLib.Controls.Breadcrumbs
{
	using System.Windows;
	using System.Windows.Controls;

	public class BreadcrumbTree : TreeView
	{
		#region fields
		public static readonly DependencyProperty OverflowedItemContainerStyleProperty =
					 DependencyProperty.Register("OverflowedItemContainerStyle", typeof(Style), typeof(BreadcrumbTree));

		public static readonly DependencyProperty MenuItemTemplateProperty =
						 DependencyProperty.Register("MenuItemTemplate", typeof(DataTemplate), typeof(BreadcrumbTree));
		#endregion fields

		#region Constructor

		static BreadcrumbTree()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbTree),
					new FrameworkPropertyMetadata(typeof(BreadcrumbTree)));
		}

		#endregion

		#region properties
		public Style OverflowedItemContainerStyle
		{
			get { return (Style)GetValue(OverflowedItemContainerStyleProperty); }
			set { this.SetValue(OverflowedItemContainerStyleProperty, value); }
		}

		public DataTemplate MenuItemTemplate
		{
			get { return (DataTemplate)GetValue(MenuItemTemplateProperty); }
			set { this.SetValue(MenuItemTemplateProperty, value); }
		}
		#endregion properties

		#region methods
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new BreadcrumbTreeItem() { };
		}
		#endregion methods
	}
}
