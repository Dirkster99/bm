namespace BreadcrumbLib.Controls.Breadcrumbs
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;
	using BreadcrumbLib.BaseControls.Breadcrumb;
	using BreadcrumbLib.Controls.SuggestBox;
	using BreadcrumbLib.Utils;

	public class BreadcrumbBase : ItemsControl
	{
		#region fields
		public static readonly DependencyProperty SelectedValueProperty =
				DependencyProperty.Register("SelectedValue", typeof(object),
				typeof(BreadcrumbBase), new UIPropertyMetadata(null, OnSelectedValueChanged));

		#region ProgressBar related - IsIndeterminate, IsProgressbarVisible, ProgressBarValue
		public static readonly DependencyProperty IsIndeterminateProperty =
				DependencyProperty.Register("IsIndeterminate", typeof(bool),
				typeof(BreadcrumbBase), new UIPropertyMetadata(true));

		public static readonly DependencyProperty IsProgressbarVisibleProperty =
				DependencyProperty.Register("IsProgressbarVisible", typeof(bool),
				typeof(BreadcrumbBase), new UIPropertyMetadata(false));

		public static readonly DependencyProperty ProgressProperty =
				DependencyProperty.Register("Progress", typeof(int),
				typeof(BreadcrumbBase), new UIPropertyMetadata(0));
		#endregion ProgressBar related - IsIndeterminate, IsProgressbarVisible, ProgressBarValue

		#region IsBreadcrumbVisible, DropDownHeight, DropDownWidth
		public static readonly DependencyProperty IsBreadcrumbVisibleProperty =
				DependencyProperty.Register("IsBreadcrumbVisible", typeof(bool),
				typeof(BreadcrumbBase), new UIPropertyMetadata(true));

		public static readonly DependencyProperty DropDownHeightProperty =
					BreadcrumbCore.DropDownHeightProperty.AddOwner(typeof(BreadcrumbBase));

		public static readonly DependencyProperty DropDownWidthProperty =
				BreadcrumbCore.DropDownWidthProperty.AddOwner(typeof(BreadcrumbBase));
		#endregion

		#region Header/Icon Template
		public static readonly DependencyProperty HeaderTemplateProperty =
								BreadcrumbCore.HeaderTemplateProperty.AddOwner(typeof(BreadcrumbBase));

		public static readonly DependencyProperty IconTemplateProperty =
			 DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(BreadcrumbBase), new PropertyMetadata(null));
		#endregion

		public static readonly DependencyProperty RootItemsSourceProperty =
		 BreadcrumbCore.RootItemsSourceProperty.AddOwner(typeof(BreadcrumbBase));

		public static readonly DependencyProperty ValuePathProperty =
			 DependencyProperty.Register("ValuePath", typeof(string), typeof(BreadcrumbBase),
			 new PropertyMetadata("Value"));

		public static readonly DependencyProperty SuggestionsProperty =
				SuggestBox.SuggestionsProperty.AddOwner(typeof(BreadcrumbBase));

		public static readonly DependencyProperty TextProperty =
				SuggestBox.TextProperty.AddOwner(typeof(BreadcrumbBase));

		public static readonly DependencyProperty ButtonsProperty =
				DependencyProperty.Register("Buttons", typeof(object), typeof(BreadcrumbBase));

		protected BreadcrumbCore bcore;
		protected SuggestBoxBase tbox;
		protected ToggleButton toggle;
		#endregion fields

		#region Constructor

		static BreadcrumbBase()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbBase),
					new FrameworkPropertyMetadata(typeof(BreadcrumbBase)));
		}

		#endregion

		#region properties
		public SuggestBoxBase PART_SuggestBox
		{
			get { return this.tbox; }
		}

		public BreadcrumbCore PART_BreadcrumbCore
		{
			get { return this.bcore; }
		}

		public ToggleButton PART_Toggle
		{
			get { return this.toggle; }
		}

		/// <summary>
		/// Selected value object, it's path is retrieved from HierarchyHelper.GetPath(), not bindable at this time
		/// </summary>
		public object SelectedValue
		{
			get { return this.GetValue(SelectedValueProperty); }
			set { this.SetValue(SelectedValueProperty, value); }
		}

		#region ProgressBar related - IsIndeterminate, IsProgressbarVisible, ProgressBarValue
		/// <summary>
		/// Toggle whether the progress bar is indertminate
		/// </summary>
		public bool IsIndeterminate
		{
			get { return (bool)GetValue(IsIndeterminateProperty); }
			set { this.SetValue(IsIndeterminateProperty, value); }
		}

		/// <summary>
		/// Toggle whether Progressbar visible
		/// </summary>
		public bool IsProgressbarVisible
		{
			get { return (bool)GetValue(IsProgressbarVisibleProperty); }
			set { this.SetValue(IsProgressbarVisibleProperty, value); }
		}

		/// <summary>
		/// Value of Progressbar.
		/// </summary>
		public int Progress
		{
			get { return (int)GetValue(ProgressProperty); }
			set { this.SetValue(ProgressProperty, value); }
		}
		#endregion

		#region IsBreadcrumbVisible, DropDownHeight, DropDownWidth

		/// <summary>
		/// Toggle whether Breadcrumb (or SuggestBox) visible
		/// </summary>
		public bool IsBreadcrumbVisible
		{
			get { return (bool)GetValue(IsBreadcrumbVisibleProperty); }
			set { this.SetValue(IsBreadcrumbVisibleProperty, value); }
		}

		/// <summary>
		/// Is current dropdown (combobox) opened, this apply to the first &lt;&lt; button only
		/// </summary>
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

		#endregion

		#region Header/Icon Template
		/// <summary>
		/// DataTemplate define the header text, (see also IconTemplate)
		/// </summary>
		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { this.SetValue(HeaderTemplateProperty, value); }
		}

		/// <summary>
		/// DataTemplate define the icon.
		/// </summary>
		public DataTemplate IconTemplate
		{
			get { return (DataTemplate)GetValue(IconTemplateProperty); }
			set { this.SetValue(IconTemplateProperty, value); }
		}
		#endregion

		/// <summary>
		/// RootItemsSource - Items to be shown in BreadcrumbCore.
		/// ItemsSource - The Hierarchy for of current selected item.
		/// </summary>
		public IEnumerable RootItemsSource
		{
			get { return (IEnumerable)GetValue(RootItemsSourceProperty); }
			set { this.SetValue(RootItemsSourceProperty, value); }
		}

		/// <summary>
		/// Used by suggest box to obtain value
		/// </summary>
		public string ValuePath
		{
			get { return (string)GetValue(ValuePathProperty); }
			set { this.SetValue(ValuePathProperty, value); }
		}

		/// <summary>
		/// Suggestions shown on the SuggestionBox
		/// </summary>
		public IList<object> Suggestions
		{
			get { return (IList<object>)GetValue(SuggestionsProperty); }
			set { this.SetValue(SuggestionsProperty, value); }
		}

		/// <summary>
		/// Text shown on the SuggestionBox
		/// </summary>
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { this.SetValue(TextProperty, value); }
		}

		/// <summary>
		/// Buttons shown in the right side of the Breadcrumb
		/// </summary>
		public object Buttons
		{
			get { return this.GetValue(ButtonsProperty); }
			set { this.SetValue(ButtonsProperty, value); }
		}
		#endregion properties

		#region Methods
		public static void OnSelectedValueChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var bread = sender as BreadcrumbBase;

			if (bread.bcore != null && (e.NewValue == null || !e.NewValue.Equals(e.OldValue)))
				bread.Select(e.NewValue);
		}

		public virtual void Select(object value)
		{
			this.SelectedValue = value;
		}

		/// <summary>
		/// Is called when a control template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.bcore = this.Template.FindName("PART_BreadcrumbCore", this) as BreadcrumbCore;
			this.tbox = this.Template.FindName("PART_TextBox", this) as BreadcrumbLib.Controls.SuggestBox.SuggestBoxBase;
			this.toggle = this.Template.FindName("PART_Toggle", this) as ToggleButton;

			#region BreadcrumbCore related handlers
			// When Breadcrumb select a value, update it.
			this.AddHandler(BreadcrumbCore.SelectedValueChangedEvent, (RoutedEventHandler)((o, e) =>
			{
				Select(bcore.SelectedValue);
			}));
			#endregion

			#region SuggestBox related handlers.
			// Switch to text box on click in empty space
			this.AddHandler(BreadcrumbList.MouseDownEvent, (RoutedEventHandler)((o, e) =>
			{
				toggle.SetValue(ToggleButton.IsCheckedProperty, false);  // Hide Breadcrumb
			}));

			// Call SelectAll when text box is visible
			this.toggle.AddValueChanged(ToggleButton.IsCheckedProperty, (o, e) =>
			{
				tbox.Focus();
				tbox.SelectAll();
			});

			// Hide textbox on changed selected (path) value.
			this.AddHandler(SuggestBox.ValueChangedEvent, (RoutedEventHandler)((o, e) =>
			{
				toggle.SetValue(ToggleButton.IsCheckedProperty, true); // Show Breadcrumb
			}));

			this.AddValueChanged(BreadcrumbList.SelectedPathValueProperty, (o, e) =>
			{
				toggle.SetValue(ToggleButton.IsCheckedProperty, true); // Show Breadcrumb
			});

			this.AddValueChanged(BreadcrumbList.SelectedValueProperty, (o, e) =>
			{
				toggle.SetValue(ToggleButton.IsCheckedProperty, true); // Show Breadcrumb
			});
			#endregion
		}
		#endregion
	}
}
