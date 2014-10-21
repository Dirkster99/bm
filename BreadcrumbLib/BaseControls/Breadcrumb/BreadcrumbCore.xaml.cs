///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010  - http://www.quickzip.org/components                                                            //
// Release under MIT license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace BreadcrumbLib.BaseControls.Breadcrumb
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using BreadcrumbLib.Utils;

	public class BreadcrumbCore : ItemsControl
	{
		#region fields
		#region Events
		public static readonly RoutedEvent SelectedValueChangedEvent = EventManager.RegisterRoutedEvent("SelectedValueChanged",
			RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BreadcrumbCore));
		#endregion Events

		public static readonly DependencyProperty SelectedBreadcrumbItemProperty = DependencyProperty.Register("SelectedBreadcrumbItem",
			typeof(BreadcrumbItem), typeof(BreadcrumbCore), new PropertyMetadata(null));

		public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue",
		 typeof(object), typeof(BreadcrumbCore), new PropertyMetadata(null));

		public static readonly DependencyProperty IsRootSelectedProperty = DependencyProperty.Register("IsRootSelected",
		 typeof(bool), typeof(BreadcrumbCore), new PropertyMetadata(true));

		public static readonly DependencyProperty OverflowedItemsProperty = DependencyProperty.Register("OverflowedItems",
			 typeof(IEnumerable), typeof(BreadcrumbCore), new PropertyMetadata(null));

		public static readonly DependencyProperty IsOverflowedProperty = DependencyProperty.Register("IsOverflowed",
			 typeof(bool), typeof(BreadcrumbCore), new PropertyMetadata(false));

		public static readonly DependencyProperty LastNonVisibleIndexProperty = DependencyProperty.Register("LastNonVisibleIndex",
			typeof(int), typeof(BreadcrumbCore), new PropertyMetadata(0, OnLastNonVisibleIndexChanged));

		public static readonly DependencyProperty IsDropDownOpenProperty =
			ComboBox.IsDropDownOpenProperty.AddOwner(typeof(BreadcrumbCore),
			new PropertyMetadata(false));

		public static readonly DependencyProperty ShowDropDownProperty =
			DependencyProperty.Register("ShowDropDown", typeof(bool), typeof(BreadcrumbCore),
			new PropertyMetadata(true));

		public static readonly DependencyProperty DropDownHeightProperty =
				DependencyProperty.Register("DropDownHeight", typeof(double), typeof(BreadcrumbCore), new UIPropertyMetadata(200d));

		public static readonly DependencyProperty DropDownWidthProperty =
		DependencyProperty.Register("DropDownWidth", typeof(double), typeof(BreadcrumbCore), new UIPropertyMetadata(100d));

		public static readonly DependencyProperty RootItemsSourceProperty = DependencyProperty.Register("RootItemsSource",
				typeof(IEnumerable), typeof(BreadcrumbCore), new PropertyMetadata(null));

		public static readonly DependencyProperty HeaderTemplateProperty = HeaderedItemsControl
				.HeaderTemplateProperty.AddOwner(typeof(BreadcrumbCore));

		public static readonly DependencyProperty IconTemplateProperty =
				DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(BreadcrumbCore));
		#endregion fields

		#region constructors
		/// <summary>
		/// Static class constructor
		/// </summary>
		static BreadcrumbCore()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbCore),
					new FrameworkPropertyMetadata(typeof(BreadcrumbCore)));
		}

		/// <summary>
		/// Class constructor
		/// </summary>
		public BreadcrumbCore()
		{
			////AddHandler(BreadcrumbItem.SelectedEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs args)
			////{
			////    //Debug.WriteLine(this.GetValue(IsDropDownOpenProperty));
			////    this.SetValue(IsDropDownOpenProperty, false);
			////    //args.Handled = true;
			////});

			this.AddValueChanged(BreadcrumbCore.ItemsSourceProperty, (o, e) =>
			{
				if (this.Items.Count > 0)
				{
					BreadcrumbItem firstItem = this.ItemContainerGenerator.ContainerFromIndex(0) as BreadcrumbItem;
					if (firstItem != null)
					{
						firstItem.ShowCaption = firstItem.ShowToggle = this.Items.Count == 1;
					}
				}

				updateOverflowedItems();
			});

			this.AddHandler(BreadcrumbItem.SelectedEvent, (RoutedEventHandler)((o, e) =>
			{
				SelectedBreadcrumbItem = e.OriginalSource as BreadcrumbItem;
				if (SelectedBreadcrumbItem is BreadcrumbItem)
				{
					var item = (SelectedBreadcrumbItem as BreadcrumbItem);
					SetValue(SelectedValueProperty, item.DataContext);
				}

				this.SetValue(IsDropDownOpenProperty, false); // Close << drop down when selected.

				RaiseEvent(new RoutedEventArgs(SelectedValueChangedEvent));
				e.Handled = true;
			}));
		}
		#endregion constructors

		#region Events
		/// <summary>
		/// SelectedValue changed.
		/// </summary>
		public event RoutedEventHandler SelectedValueChanged
		{
			add { this.AddHandler(SelectedValueChangedEvent, value); }
			remove { this.RemoveHandler(SelectedValueChangedEvent, value); }
		}
		#endregion Events

		#region properties
		/// <summary>
		/// Used by BreadcrumbCorePanel, default (0) the root item is showed in OverflowPanel.
		/// </summary>
		public int DefaultLastNonVisibleIndex
		{
			get
			{
				return 0;
			}
		}

		#region Dependency properties
		#region SelectedBreadcrumbItem / SelectedValue
		/// <summary>
		/// The selected UI item.
		/// </summary>
		public BreadcrumbItem SelectedBreadcrumbItem
		{
			get { return (BreadcrumbItem)GetValue(SelectedBreadcrumbItemProperty); }
			set { this.SetValue(SelectedBreadcrumbItemProperty, value); }
		}

		/// <summary>
		/// Datacontext of the selected item.
		/// </summary>
		public object SelectedValue
		{
			get { return (object)GetValue(SelectedValueProperty); }
			set { this.SetValue(SelectedValueProperty, value); }
		}

		/// <summary>
		/// Datacontext of the selected item.
		/// </summary>
		public bool IsRootSelected
		{
			get { return (bool)GetValue(IsRootSelectedProperty); }
			set { this.SetValue(IsRootSelectedProperty, value); }
		}
		#endregion

		#region OverflowedItems, IsOverflowed, LastNonVisible
		/// <summary>
		/// Items to be displayed in OverflowPanel.
		/// </summary>
		public IEnumerable OverflowedItems
		{
			get { return (ICollection)GetValue(OverflowedItemsProperty); }
			set { this.SetValue(OverflowedItemsProperty, value); }
		}

		/// <summary>
		/// Only if Overflowed items is more than DefaultLastNonVisibleIndex + 1
		/// </summary>
		public bool IsOverflowed
		{
			get { return (bool)GetValue(IsOverflowedProperty); }
			set { this.SetValue(IsOverflowedProperty, value); }
		}

		/// <summary>
		/// Set by BreadcrumbCorePanel to define when items are overflowed.
		/// </summary>
		public int LastNonVisible
		{
			get { return (int)GetValue(LastNonVisibleIndexProperty); }
			set { this.SetValue(LastNonVisibleIndexProperty, value); }
		}

		#endregion

		#region IsDropDownOpen, DropDownWidth, DropDownHeight, RootItems
		/// <summary>
		/// Is current dropdown (combobox) opened, this apply to the first &lt;&lt; button only
		/// </summary>
		public bool IsDropDownOpen
		{
			get { return (bool)GetValue(IsDropDownOpenProperty); }
			set { this.SetValue(IsDropDownOpenProperty, value); }
		}

		/// <summary>
		/// Whether the first dropdown is shown, set by Breadcrumb.
		/// </summary>
		public bool ShowDropDown
		{
			get { return (bool)GetValue(ShowDropDownProperty); }
			set { this.SetValue(ShowDropDownProperty, value); }
		}

		/// <summary>
		/// Is current dropdown (combobox) opened, this apply to the first &lt;&lt; button only
		/// </double>
		public double DropDownHeight
		{
			get { return (double)GetValue(DropDownHeightProperty); }
			set { this.SetValue(DropDownHeightProperty, value); }
		}

		/// <summary>
		/// Is current dropdown (combobox) opened, this apply to the first &lt;&lt; button only
		/// </summary>
		public double DropDownWidth
		{
			get { return (double)GetValue(DropDownWidthProperty); }
			set { this.SetValue(DropDownWidthProperty, value); }
		}

		/// <summary>
		/// Assigned by Breadcrumb
		/// </summary>
		public IEnumerable RootItemsSource
		{
			get { return (IEnumerable)GetValue(RootItemsSourceProperty); }
			set { this.SetValue(RootItemsSourceProperty, value); }
		}
		#endregion

		#region HeaderTemplate, IconTemplate
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
		#endregion Dependency propertie
		#endregion

		#region methods
		public static void OnLastNonVisibleIndexChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			BreadcrumbCore bcore = sender as BreadcrumbCore;
			bcore.updateOverflowedItems();
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			var retVal = new BreadcrumbItem(true)
			{
				HeaderTemplate = this.HeaderTemplate,
				IconTemplate = this.IconTemplate
			};

			return retVal;
		}

		private void updateOverflowedItems()
		{
			Stack<object> overflowedItems = new Stack<object>();

			for (int i = 0; i < Math.Min(this.LastNonVisible + 1, this.Items.Count); i++)
			{
				overflowedItems.Push(this.Items[i]);
			}

			this.SetValue(OverflowedItemsProperty, overflowedItems);
			this.SetValue(IsOverflowedProperty, overflowedItems.Count() > this.DefaultLastNonVisibleIndex + 1);
		}
		#endregion methods
	}
}
