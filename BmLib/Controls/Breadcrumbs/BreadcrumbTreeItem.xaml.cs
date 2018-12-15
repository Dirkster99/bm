﻿namespace BmLib.Controls.Breadcrumbs
{
    using System.ComponentModel;
    using System.Windows;
	using System.Windows.Controls;
    using System.Windows.Input;

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

        public static readonly DependencyProperty OverflowItemCountProperty = OverflowableStackPanel.OverflowItemCountProperty
		.AddOwner(typeof(BreadcrumbTreeItem), new PropertyMetadata(OnOverflowItemCountChanged));

		public static readonly DependencyProperty IsOverflowedProperty = DependencyProperty.Register("IsOverflowed", typeof(bool),
		 typeof(BreadcrumbTreeItem), new PropertyMetadata(false));

		public static readonly DependencyProperty SelectedChildProperty =
			DependencyProperty.Register("SelectedChild", typeof(object), typeof(BreadcrumbTreeItem),
					new UIPropertyMetadata(null));

		public static readonly DependencyProperty ValuePathProperty =
		 DependencyProperty.Register("ValuePath", typeof(string), typeof(BreadcrumbTreeItem),
				 new UIPropertyMetadata(string.Empty));

		public static readonly DependencyProperty IsChildSelectedProperty =
				DependencyProperty.Register("IsChildSelected", typeof(bool), typeof(BreadcrumbTreeItem),
						new UIPropertyMetadata());

		public static readonly DependencyProperty IsCurrentSelectedProperty =
			 DependencyProperty.Register("IsCurrentSelected", typeof(bool), typeof(BreadcrumbTreeItem),
					 new UIPropertyMetadata(false));

		public static readonly DependencyProperty IsCaptionVisibleProperty =
								DependencyProperty.Register("IsCaptionVisible", typeof(bool), typeof(BreadcrumbTreeItem),
								new UIPropertyMetadata(true, OnIsCaptionVisibleChanged));
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

        public int OverflowItemCount
		{
			get { return (int)GetValue(OverflowItemCountProperty); }
			set { this.SetValue(OverflowItemCountProperty, value); }
		}

		public bool IsOverflowed
		{
			get { return (bool)GetValue(IsOverflowedProperty); }
			set { this.SetValue(IsOverflowedProperty, value); }
		}

		public object SelectedChild
		{
			get { return (object)GetValue(SelectedChildProperty); }
			set { this.SetValue(SelectedChildProperty, value); }
		}

		public string ValuePath
		{
			get { return (string)GetValue(ValuePathProperty); }
			set { this.SetValue(ValuePathProperty, value); }
		}

		public bool IsChildSelected
		{
			get { return (bool)GetValue(IsChildSelectedProperty); }
			set { this.SetValue(IsChildSelectedProperty, value); }
		}

		public bool IsCurrentSelected
		{
			get { return (bool)GetValue(IsCurrentSelectedProperty); }
			set { this.SetValue(IsCurrentSelectedProperty, value); }
		}

		/// <summary>
		/// Display Caption
		/// </summary>
		public bool IsCaptionVisible
		{
			get { return (bool)GetValue(IsCaptionVisibleProperty); }
			set { this.SetValue(IsCaptionVisibleProperty, value); }
		}
		#endregion properties

		#region Methods
		public static void OnIsCaptionVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			(sender as BreadcrumbTreeItem).UpdateStates(true);
		}

		public static void OnOverflowItemCountChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			(sender as BreadcrumbTreeItem).SetValue(IsOverflowedProperty, ((int)args.NewValue) > 0);
		}

		/// <summary>
		/// Is called when a control template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

////            this.AddHandler(Button.ClickEvent, (RoutedEventHandler)((o, e) =>
////			{
////				if (e.Source is Button)
////				{
////					this.SetValue(IsCurrentSelectedProperty, true);
////					e.Handled = true;
////				}
////			}));

			////this.AddHandler(OverflowItem.SelectedEvent, (RoutedEventHandler)((o, e) =>
			////    {
			////        if (e.Source is OverflowItem)
			////        {
			////            IsExpanded = false;
			////        }
			////    }));
		}

        protected override DependencyObject GetContainerForItemOverride()
		{
			return new BreadcrumbTreeItem();
		}

		private void UpdateStates(bool useTransition)
		{
			if (this.IsCaptionVisible)
				VisualStateManager.GoToState(this, "ShowCaption", useTransition);
			else
				VisualStateManager.GoToState(this, "HideCaption", useTransition);
		}
        #endregion
    }
}
