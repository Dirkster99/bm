namespace BmLib.Controls.Breadcrumbs
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    public class BreadcrumbTree : TreeView
    {
        #region fields
        /// <summary>
        /// Backing store of the <see cref="DropDownListItemDataTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DropDownListItemDataTemplateProperty =
            DependencyProperty.Register("DropDownListItemDataTemplate", typeof(DataTemplate),
                typeof(BreadcrumbTree), new PropertyMetadata(null));
        #endregion fields

        #region Constructor
        /// <summary>
        /// Static class constructor.
        /// </summary>
        static BreadcrumbTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbTree),
                    new FrameworkPropertyMetadata(typeof(BreadcrumbTree)));
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display
        /// the drop down list of each item in the breadcrumb tree control.
        ///
        /// Returns:
        /// A <see cref="DataTemplate"/> that specifies the visualization of the data objects.
        /// The default is null (none).
        /// </summary>
        [Bindable(true)]
        public DataTemplate DropDownListItemDataTemplate
        {
            get { return (DataTemplate)GetValue(DropDownListItemDataTemplateProperty); }
            set { SetValue(DropDownListItemDataTemplateProperty, value); }
        }
        #endregion properties

        #region methods
        ////        /// <summary>
        ////        /// Measures the child elements of a <seealso cref="StackPanel"/> 
        ////        /// in anticipation of arranging them during the
        ////        /// <seealso cref="StackPanel.ArrangeOverride(System.Windows.Size)"/>
        ////        /// </summary>
        ////        /// <param name="constraint">An upper limit <seealso cref="Size"/> that should not be exceeded.</param>
        ////        /// <returns>The System.Windows.Size that represents the desired size of the element.</returns>
        ////        protected override Size MeasureOverride(Size constraint)
        ////        {
        ////            if (double.IsPositiveInfinity(constraint.Width)) // || double.IsPositiveInfinity(constraint.Height))
        ////            {
        ////                // This constrain hints a layout proplem that can cause items to NOT Overflow.
        ////                Debug.WriteLine("  +---> Warning: BreadcrumbTree.MeasureOverride(Size constraint) with constraint == Infinity");
        ////            }
        ////
        ////            var sz = base.MeasureOverride(constraint);
        ////
        ////            if (constraint.Width <= sz.Width)
        ////            {
        ////                Debug.WriteLine("");
        ////            }
        ////
        ////            return sz;
        ////        }

        /// <summary>
        /// Creates the element that is used to display a <see cref="BreadcrumbTreeItem">.
        /// </summary>
        /// <returns>A new <see cref="BreadcrumbTreeItem"> object instance.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BreadcrumbTreeItem();
        }
        #endregion methods
    }
}
