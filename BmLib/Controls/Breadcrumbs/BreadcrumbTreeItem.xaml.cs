namespace BmLib.Controls.Breadcrumbs
{
    using System.ComponentModel;
    using System.Windows;
	using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Implements the tree items collection for a breadcrumb control that
    /// specifies its items <see cref="DataTemplate"/> with <see cref="HierarchicalDataTemplate"/>s
    /// as a source data structure.
    /// </summary>
    [TemplateVisualState(Name = "ShowCaption", GroupName = "CaptionStates")]
	[TemplateVisualState(Name = "HideCaption", GroupName = "CaptionStates")]
	public class BreadcrumbTreeItem : TreeViewItem
	{
        #region fields
        /// <summary>
        /// Backing store of the <see cref="DropDownListItemDataTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DropDownListItemDataTemplateProperty =
            DependencyProperty.Register("DropDownListItemDataTemplate", typeof(DataTemplate),
                typeof(BreadcrumbTreeItem), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="ClickItemCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ClickItemCommandProperty =
            DependencyProperty.Register("ClickItemCommand",
                typeof(ICommand),
                typeof(BreadcrumbTreeItem),
                new PropertyMetadata(null));

        /// <summary>
        /// Implement the backing store of the <see cref="OverflowItemCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OverflowItemCountProperty =
            OverflowableStackPanel.OverflowItemCountProperty.AddOwner(
                typeof(BreadcrumbTreeItem), new PropertyMetadata(OnOverflowItemCountChanged));

        /// <summary>
        /// Implement the backing store of the <see cref="IsOverflowed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOverflowedProperty =
            DependencyProperty.Register("IsOverflowed", typeof(bool),
		     typeof(BreadcrumbTreeItem), new PropertyMetadata(false));

        /// <summary>
        /// Implement the backing store of the <see cref="SelectedChild"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedChildProperty =
			DependencyProperty.Register("SelectedChild", typeof(object), typeof(BreadcrumbTreeItem),
					new UIPropertyMetadata(null));

        /// <summary>
        /// Implement the backing store of the <see cref="ValuePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValuePathProperty =
		 DependencyProperty.Register("ValuePath", typeof(string), typeof(BreadcrumbTreeItem),
				 new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// Implement the backing store of the <see cref="IsChildSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsChildSelectedProperty =
				DependencyProperty.Register("IsChildSelected", typeof(bool), typeof(BreadcrumbTreeItem),
						new UIPropertyMetadata());

        /// <summary>
        /// Implement the backing store of the <see cref="IsCurrentSelected"/> dependency property.
        /// </summary>
		public static readonly DependencyProperty IsCurrentSelectedProperty =
			 DependencyProperty.Register("IsCurrentSelected", typeof(bool), typeof(BreadcrumbTreeItem),
					 new UIPropertyMetadata(false));

		////public static readonly DependencyProperty IsCaptionVisibleProperty =
		////						DependencyProperty.Register("IsCaptionVisible", typeof(bool), typeof(BreadcrumbTreeItem),
		////						new UIPropertyMetadata(true, OnIsCaptionVisibleChanged));
        #endregion fields

        #region Constructor
        /// <summary>
        /// Static constructor
        /// </summary>
        static BreadcrumbTreeItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbTreeItem),
					new FrameworkPropertyMetadata(typeof(BreadcrumbTreeItem)));
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		public BreadcrumbTreeItem()
		{
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

        /// <summary>
        /// Gets or sets the <see cref="ICommand"/> that can be bound and executed when a
        /// user clicks on a tree views item.
        /// 
        /// Default: null
        /// </summary>
        [Bindable(true)]
        public DependencyObject ClickItemCommand
        {
            get { return (DependencyObject)GetValue(ClickItemCommandProperty); }
            set { SetValue(ClickItemCommandProperty, value); }
        }

        /// <summary>
        /// Gets/sets the number of items that are overflown below this item
        /// (This number includes the item itself if it is overflown).
        /// </summary>
        public int OverflowItemCount
		{
			get { return (int)GetValue(OverflowItemCountProperty); }
			set { this.SetValue(OverflowItemCountProperty, value); }
		}

        /// <summary>
        /// Gets/sets whether this item is currently overflown
        /// (does not fit into the available view) or not.
        /// </summary>
		public bool IsOverflowed
		{
			get { return (bool)GetValue(IsOverflowedProperty); }
			set { this.SetValue(IsOverflowedProperty, value); }
		}

        /// <summary>
        /// Gets/sets the model of the selected child item.
        /// </summary>
		public object SelectedChild
		{
			get { return (object)GetValue(SelectedChildProperty); }
			set { this.SetValue(SelectedChildProperty, value); }
		}

        /// <summary>
        /// Gets/sets the instance of the model object that represents this selection helper.
        /// The model backs the ViewModel property and should be in sync with it.
        /// </summary>
        public string ValuePath
		{
			get { return (string)GetValue(ValuePathProperty); }
			set { this.SetValue(ValuePathProperty, value); }
		}

        /// <summary>
        /// Gets/sets whether a child item of this item is currently selected or not.
        /// All items along the current path (except for the selected item) must have
        /// this property set to true - the path is otherwise not visible in the
        /// breadcrumbs control.
        /// </summary>
		public bool IsChildSelected
		{
			get { return (bool)GetValue(IsChildSelectedProperty); }
			set { this.SetValue(IsChildSelectedProperty, value); }
		}

        /// <summary>
        /// Gets/sets whether this item is currently selected or not.
        /// </summary>
		public bool IsCurrentSelected
		{
			get { return (bool)GetValue(IsCurrentSelectedProperty); }
			set { this.SetValue(IsCurrentSelectedProperty, value); }
		}
		#endregion properties

		#region Methods
        /// <summary>
        /// Creates a new System.Windows.Controls.TreeViewItem to use to display the object.
        /// </summary>
        /// <returns>A new <see cref="System.Windows.Controls.TreeViewItem"/> to use to
        /// display the object.</returns>
        protected override DependencyObject GetContainerForItemOverride()
		{
			return new BreadcrumbTreeItem();
		}

        /// <summary>
        /// Sets the <see cref="IsOverflowed"/> property to true
        /// if any of the items children is overflown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnOverflowItemCountChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as BreadcrumbTreeItem).SetValue(IsOverflowedProperty, ((int)args.NewValue) > 0);
        }
        #endregion
    }
}
