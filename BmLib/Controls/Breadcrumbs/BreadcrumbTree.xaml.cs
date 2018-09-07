namespace BmLib.Controls.Breadcrumbs
{
    using System.Diagnostics;
    using System.Windows;
	using System.Windows.Controls;

	public class BreadcrumbTree : TreeView
	{
		#region fields
		public static readonly DependencyProperty MenuItemTemplateProperty =
						 DependencyProperty.Register("MenuItemTemplate",
                             typeof(DataTemplate),
                             typeof(BreadcrumbTree),
                             new PropertyMetadata(null));
		#endregion fields

		#region Constructor

		static BreadcrumbTree()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbTree),
					new FrameworkPropertyMetadata(typeof(BreadcrumbTree)));
		}

		#endregion

		#region properties
		public DataTemplate MenuItemTemplate
		{
			get { return (DataTemplate)GetValue(MenuItemTemplateProperty); }
			set { this.SetValue(MenuItemTemplateProperty, value); }
		}
        #endregion properties

        #region methods
        /// <summary>
        /// Measures the child elements of a <seealso cref="StackPanel"/> 
        /// in anticipation of arranging them during the
        /// <seealso cref="StackPanel.ArrangeOverride(System.Windows.Size)"/>
        /// </summary>
        /// <param name="constraint">An upper limit <seealso cref="Size"/> that should not be exceeded.</param>
        /// <returns>The System.Windows.Size that represents the desired size of the element.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (double.IsPositiveInfinity(constraint.Width)) // || double.IsPositiveInfinity(constraint.Height))
            {
                // This constrain hints a layout proplem that can cause items to NOT Overflow.
                Debug.WriteLine("  +---> Warning: BreadcrumbTree.MeasureOverride(Size constraint) with constraint == Infinity");
            }

            var sz = base.MeasureOverride(constraint);

            if (constraint.Width <= sz.Width)
            {
                Debug.WriteLine("");
            }

            return sz;
        }

        protected override DependencyObject GetContainerForItemOverride()
		{
			return new BreadcrumbTreeItem() { };
		}
        #endregion methods
    }
/***
    public class CopyOfBreadcrumbTree : TreeView
    {
        #region fields
        public static readonly DependencyProperty MenuItemTemplateProperty =
                         DependencyProperty.Register("MenuItemTemplate",
                             typeof(DataTemplate),
                             typeof(CopyOfBreadcrumbTree),
                             new PropertyMetadata(null));
        #endregion fields

        #region Constructor

        static CopyOfBreadcrumbTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CopyOfBreadcrumbTree),
                    new FrameworkPropertyMetadata(typeof(CopyOfBreadcrumbTree)));
        }

        #endregion

        #region properties
        public DataTemplate MenuItemTemplate
        {
            get { return (DataTemplate)GetValue(MenuItemTemplateProperty); }
            set { this.SetValue(MenuItemTemplateProperty, value); }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Measures the child elements of a <seealso cref="StackPanel"/> 
        /// in anticipation of arranging them during the
        /// <seealso cref="StackPanel.ArrangeOverride(System.Windows.Size)"/>
        /// </summary>
        /// <param name="constraint">An upper limit <seealso cref="Size"/> that should not be exceeded.</param>
        /// <returns>The System.Windows.Size that represents the desired size of the element.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (double.IsPositiveInfinity(constraint.Width)) // || double.IsPositiveInfinity(constraint.Height))
            {
                // This constrain hints a layout proplem that can cause items to NOT Overflow.
                Debug.WriteLine("  +---> Warning: CopyOfBreadcrumbTree.MeasureOverride(Size constraint) with constraint == Infinity");
            }

            var sz = base.MeasureOverride(constraint);

            if (constraint.Width <= sz.Width)
            {
                Debug.WriteLine("");
            }

            return sz;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BreadcrumbTreeItem() { };
        }
        #endregion methods
    }
***/
}
