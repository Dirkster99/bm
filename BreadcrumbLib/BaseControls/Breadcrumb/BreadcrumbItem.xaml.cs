///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under MIT license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace BreadcrumbLib.BaseControls.Breadcrumb
{
	using System.Windows;
	using System.Windows.Controls;

	public class BreadcrumbItem : HeaderedItemsControl
	{
		#region fields
		public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected",
			RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BreadcrumbItem));

		#region ShowCaption, Toggle, Icon
		public static readonly DependencyProperty ShowCaptionProperty =
								DependencyProperty.Register("ShowCaption", typeof(bool), typeof(BreadcrumbItem),
								new UIPropertyMetadata(true, OnShowCaptionChanged));

		public static readonly DependencyProperty ShowToggleProperty =
								DependencyProperty.Register("ShowToggle", typeof(bool), typeof(BreadcrumbItem),
								new UIPropertyMetadata(true));

		public static readonly DependencyProperty ShowIconProperty =
								DependencyProperty.Register("ShowIcon", typeof(bool), typeof(BreadcrumbItem),
								new UIPropertyMetadata(false));
		#endregion ShowCaption, Toggle, Icon

		#region IsTopLevel, IsOverflowed, IsShadowItem(Unused), IsSeparator, IsLoading (Unused)
		public static readonly DependencyProperty IsTopLevelProperty =
								DependencyProperty.Register("IsTopLevel", typeof(bool), typeof(BreadcrumbItem),
								new UIPropertyMetadata(OnIsTopLevelChanged));

		public static readonly DependencyProperty IsOverflowedProperty =
							 DependencyProperty.Register("IsOverflowed", typeof(bool), typeof(BreadcrumbItem),
							 new UIPropertyMetadata(false, OnIsOverflowedChanged));

		public static readonly DependencyProperty IsShadowItemProperty =
							 DependencyProperty.Register("IsShadowItem", typeof(bool), typeof(BreadcrumbItem),
							 new UIPropertyMetadata(true));

		public static readonly DependencyProperty IsSeparatorProperty =
							 DependencyProperty.Register("IsSeparator", typeof(bool), typeof(BreadcrumbItem),
							 new UIPropertyMetadata(false));

		public static readonly DependencyProperty IsLoadingProperty =
							DependencyProperty.Register("IsLoading", typeof(bool), typeof(BreadcrumbItem),
							new UIPropertyMetadata(false));
		#endregion IsTopLevel, IsOverflowed, IsShadowItem(Unused), IsSeparator, IsLoading (Unused)

		public static readonly DependencyProperty IsDropDownOpenProperty =
			 ComboBox.IsDropDownOpenProperty.AddOwner(typeof(BreadcrumbItem),
			 new PropertyMetadata(false));

		public static readonly DependencyProperty IconTemplateProperty =
				DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(BreadcrumbItem));

		private bool _isTopLevel = false;
		#endregion fields

		#region constructors
		/// <summary>
		/// Static class constructor
		/// </summary>
		static BreadcrumbItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbItem), new FrameworkPropertyMetadata(typeof(BreadcrumbItem)));
		}

		/// <summary>
		/// Parameterized class constructor
		/// </summary>
		/// <param name="isTopLevel"></param>
		public BreadcrumbItem(bool isTopLevel = false)
		{
			this._isTopLevel = isTopLevel;
			////this.Loaded += delegate { _loaded = true; };
		}
		#endregion constructors

		#region events
		/// <summary>
		/// The current item is clicked.
		/// </summary>
		public event RoutedEventHandler Selected
		{
			add { this.AddHandler(SelectedEvent, value); }
			remove { this.RemoveHandler(SelectedEvent, value); }
		}

		////Animation related event - unused.
		////public static readonly RoutedEvent ShowingCaptionEvent = EventManager.RegisterRoutedEvent("ShowingCaption",
		////    RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BreadcrumbItem));
		////
		////public event RoutedEventHandler ShowingCaption
		////{
		////    add { AddHandler(ShowingCaptionEvent, value); }
		////    remove { RemoveHandler(ShowingCaptionEvent, value); }
		////}
		////
		////public static readonly RoutedEvent HidingCaptionEvent = EventManager.RegisterRoutedEvent("HidingCaption",
		////    RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BreadcrumbItem));
		////
		////public event RoutedEventHandler HidingCaption
		////{
		////    add { AddHandler(HidingCaptionEvent, value); }
		////    remove { RemoveHandler(HidingCaptionEvent, value); }
		////}
		#endregion events

		#region properties
		#region ShowCaption, Toggle, Icon
		/// <summary>
		/// Display Caption
		/// </summary>
		public bool ShowCaption
		{
			get { return (bool)GetValue(ShowCaptionProperty); }
			set { this.SetValue(ShowCaptionProperty, value); }
		}

		/// <summary>
		/// Display Toggle
		/// </summary>
		public bool ShowToggle
		{
			get { return (bool)GetValue(ShowToggleProperty); }
			set { this.SetValue(ShowToggleProperty, value); }
		}

		/// <summary>
		/// Display Icon
		/// </summary>
		public bool ShowIcon
		{
			get { return (bool)GetValue(ShowIconProperty); }
			set { this.SetValue(ShowIconProperty, value); }
		}
		#endregion

		#region IsTopLevel, IsOverflowed, IsShadowItem(Unused), IsSeparator, IsLoading (Unused)
		/// <summary>
		/// IsTopLevel?
		/// </summary>
		public bool IsTopLevel
		{
			get { return (bool)GetValue(IsTopLevelProperty); }
			set { this.SetValue(IsTopLevelProperty, value); }
		}

		/// <summary>
		/// IsOverflowed?
		/// </summary>
		public bool IsOverflowed
		{
			get { return (bool)GetValue(IsOverflowedProperty); }
			set { this.SetValue(IsOverflowedProperty, value); }
		}

		/// <summary>
		/// For 1st level BreadcrumbItem, grey color if true. 
		/// </summary>
		public bool IsShadowItem
		{
			get { return (bool)GetValue(IsShadowItemProperty); }
			set { this.SetValue(IsShadowItemProperty, value); }
		}

		/// <summary>
		/// Display separator, use for 2nd level BreadcrumbItem only.
		/// </summary>
		public bool IsSeparator
		{
			get { return (bool)GetValue(IsSeparatorProperty); }
			set { this.SetValue(IsSeparatorProperty, value); }
		}

		/// <summary>
		/// Display separator, use for 2nd level BreadcrumbItem only.
		/// </summary>
		public bool IsLoading
		{
			get { return (bool)GetValue(IsLoadingProperty); }
			set { this.SetValue(IsLoadingProperty, value); }
		}
		#endregion

		/// <summary>
		/// Is current dropdown (combobox) opened
		/// </summary>
		public bool IsDropDownOpen
		{
			get { return (bool)GetValue(IsDropDownOpenProperty); }
			set { this.SetValue(IsDropDownOpenProperty, value); }
		}

		/// <summary>
		/// DataTemplate for display the icon.
		/// </summary>
		public DataTemplate IconTemplate
		{
			get { return (DataTemplate)GetValue(IconTemplateProperty); }
			set { this.SetValue(IconTemplateProperty, value); }
		}
		#endregion

		#region methods
		////public void raiseShowCaptionEvent(bool value)
		////{
		////    //Fix:69: http://social.msdn.microsoft.com/forums/en-US/wpf/thread/6ec60f31-5a6f-486e-a4ac-309505987735/
		////    //sometimes that element genuinely isn't there yet. (after loaded event / BreadcrumbItem.cs)
		////    try
		////    {
		////        if (value)
		////        {

		////                RaiseEvent(new RoutedEventArgs(ShowingCaptionEvent));
		////        }
		////        else
		////        {

		////                RaiseEvent(new RoutedEventArgs(HidingCaptionEvent));
		////        }
		////    }
		////    catch
		////    {

		////    }
		////}

		public static void OnShowCaptionChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			////if (args.NewValue != args.OldValue)
			////{
			////    BreadcrumbItem item = (BreadcrumbItem)sender;
			////    bool newShowCaption = (bool)args.NewValue;
			////    if (item.ShowCaption != newShowCaption)
			////        if (item._loaded)
			////        {
			////            item.raiseShowCaptionEvent(newShowCaption);
			////        }
			////        else
			////        {
			////            RoutedEventHandler action = null;
			////            action = (RoutedEventHandler)delegate
			////            {
			////                item.Loaded -= action;
			////                if (!item._showCaptionHandled && newShowCaption)
			////                    item.raiseShowCaptionEvent(newShowCaption);
			////                item._showCaptionHandled = true;
			////            };
			////            item.Loaded += action;
			////        }
			////}
		}

		public static void OnIsTopLevelChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			(sender as BreadcrumbItem).SetValue(ShowIconProperty, !(bool)args.NewValue);
		}

		public static void OnIsOverflowedChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			(sender as BreadcrumbItem).SetValue(ShowIconProperty, (bool)args.NewValue);
		}

		/// <summary>
		/// Is called when a control template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.SetValue(IsTopLevelProperty, this._isTopLevel);
			this.SetValue(ShowIconProperty, this._isTopLevel == false);
			////if (!ShowCaption)
			////    raiseShowCaptionEvent(ShowCaption);

			// When clicked, raise selected event.
			this.AddHandler(Button.ClickEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs args)
			{
				if ((args.OriginalSource is Button))
					RaiseEvent(new RoutedEventArgs(SelectedEvent));

				args.Handled = true;
			});

			// When selected, close drop down.
			this.AddHandler(BreadcrumbItem.SelectedEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs args)
			{
				this.SetValue(IsDropDownOpenProperty, false);
			});
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			BreadcrumbItem retVal = new BreadcrumbItem(false)
			{
				// Dirkster this code looks strange to me
				////HeaderTemplate = HeaderTemplate,
				////IconTemplate = IconTemplate,
			};

            retVal.HeaderTemplate = this.HeaderTemplate;
            retVal.IconTemplate = this.IconTemplate;
			retVal.ShowToggle = false;

            return retVal;
		}
		#endregion methods
	}
}
