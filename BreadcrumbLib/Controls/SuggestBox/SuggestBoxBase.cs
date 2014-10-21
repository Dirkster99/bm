namespace BreadcrumbLib.Controls.SuggestBox
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;
	using System.Windows.Input;
	using BreadcrumbLib.Utils;

	/// <summary>
	/// User update Suggestions when TextChangedEvent raised.
	/// </summary>
	public class SuggestBoxBase : TextBox
	{
		#region fields
		#region Events
		public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged",
			RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SuggestBoxBase));
		#endregion

		#region ParentPath, DisplayMemberPath, ValuePath, SubEntriesPath
		public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register(
				"DisplayMemberPath", typeof(string), typeof(SuggestBoxBase), new PropertyMetadata("Header"));

		public static readonly DependencyProperty ValuePathProperty = DependencyProperty.Register(
				"ValuePath", typeof(string), typeof(SuggestBoxBase), new PropertyMetadata("Value"));
		#endregion

		#region Suggestions
		public static readonly DependencyProperty SuggestionsProperty = DependencyProperty.Register(
				"Suggestions", typeof(IList<object>), typeof(SuggestBoxBase), new PropertyMetadata(null, OnSuggestionsChanged));
		#endregion

		#region HeaderTemplate
		public static readonly DependencyProperty HeaderTemplateProperty =
				HeaderedItemsControl.HeaderTemplateProperty.AddOwner(typeof(SuggestBoxBase));
		#endregion

		#region IsPopupOpened, DropDownPlacementTarget
		public static readonly DependencyProperty IsPopupOpenedProperty =
				DependencyProperty.Register("IsPopupOpened", typeof(bool),
				typeof(SuggestBoxBase), new UIPropertyMetadata(false));

		public static readonly DependencyProperty DropDownPlacementTargetProperty =
				DependencyProperty.Register("DropDownPlacementTarget", typeof(object), typeof(SuggestBoxBase));
		#endregion

		#region Hint(Unused), IsHintVisible (Unused)
		public static readonly DependencyProperty HintProperty =
				DependencyProperty.Register("Hint", typeof(string), typeof(SuggestBoxBase), new PropertyMetadata(string.Empty));

		public static readonly DependencyProperty IsHintVisibleProperty =
				DependencyProperty.Register("IsHintVisible", typeof(bool), typeof(SuggestBoxBase), new PropertyMetadata(true));
		#endregion

		private Popup _popup;
		private ListBox _itemList;
		private Grid _root;
		private ScrollViewer _host;
		private UIElement _textBoxView;
		private bool _prevState;
		#endregion fields

		#region Constructor

		static SuggestBoxBase()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SuggestBoxBase),
					new FrameworkPropertyMetadata(typeof(SuggestBoxBase)));
		}

		public SuggestBoxBase()
		{
		}

		#endregion

		#region Public properties
		#region Events
		public event RoutedEventHandler ValueChanged
		{
			add { this.AddHandler(ValueChangedEvent, value); }
			remove { this.RemoveHandler(ValueChangedEvent, value); }
		}
		#endregion

		#region ParentPath, DisplayMemberPath, ValuePath, SubEntriesPath
		public string DisplayMemberPath
		{
			get { return (string)GetValue(DisplayMemberPathProperty); }
			set { this.SetValue(DisplayMemberPathProperty, value); }
		}

		public string ValuePath
		{
			get { return (string)GetValue(ValuePathProperty); }
			set { this.SetValue(ValuePathProperty, value); }
		}
		#endregion

		#region Suggestions
		public IList<object> Suggestions
		{
			get { return (IList<object>)GetValue(SuggestionsProperty); }
			set { this.SetValue(SuggestionsProperty, value); }
		}
		#endregion

		#region HeaderTemplate
		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { this.SetValue(HeaderTemplateProperty, value); }
		}
		#endregion

		#region IsPopupOpened, DropDownPlacementTarget
		public bool IsPopupOpened
		{
			get { return (bool)GetValue(IsPopupOpenedProperty); }
			set { this.SetValue(IsPopupOpenedProperty, value); }
		}

		public object DropDownPlacementTarget
		{
			get { return (object)GetValue(DropDownPlacementTargetProperty); }
			set { this.SetValue(DropDownPlacementTargetProperty, value); }
		}
		#endregion

		#region Hint(Unused), IsHintVisible (Unused)
		public string Hint
		{
			get { return (string)GetValue(HintProperty); }
			set { this.SetValue(HintProperty, value); }
		}

		public bool IsHintVisible
		{
			get { return (bool)GetValue(IsHintVisibleProperty); }
			set { this.SetValue(IsHintVisibleProperty, value); }
		}
		#endregion
		#endregion properties

		#region Methods
		/// <summary>
		/// Is called when a control template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this._popup = this.Template.FindName("PART_Popup", this) as Popup;
			this._itemList = this.Template.FindName("PART_ItemList", this) as ListBox;
			this._host = this.Template.FindName("PART_ContentHost", this) as ScrollViewer;
			this._textBoxView = LogicalTreeHelper.GetChildren(this._host).OfType<UIElement>().First();
			this._root = this.Template.FindName("root", this) as Grid;

			this.GotKeyboardFocus += (o, e) =>
			{
				this.popupIfSuggest();
				this.IsHintVisible = false;
			};

			this.LostKeyboardFocus += (o, e) =>
			{
				if (!IsKeyboardFocusWithin) this.hidePopup();
				this.IsHintVisible = string.IsNullOrEmpty(this.Text);
			};

			// 09-04-09 Based on SilverLaw's approach 
			this._popup.CustomPopupPlacementCallback += new CustomPopupPlacementCallback(
					(popupSize, targetSize, offset) => new CustomPopupPlacement[] {
                new CustomPopupPlacement(new Point((0.01 - offset.X),
                    (this._root.ActualHeight - offset.Y)), PopupPrimaryAxis.None) });

			#region _itemList event handlers - MouseDblClick, PreviewMouseUp, PreviewKeyDown
			this._itemList.MouseDoubleClick += (o, e) =>
			{
				this.updateValueFromListBox();
			};

			this._itemList.PreviewMouseUp += (o, e) =>
			{
				if (this._itemList.SelectedValue != null)
					this.updateValueFromListBox();
			};

			this._itemList.PreviewKeyDown += (o, e) =>
			{
				if (e.OriginalSource is ListBoxItem)
				{
					ListBoxItem lbItem = e.OriginalSource as ListBoxItem;

					e.Handled = true;
					switch (e.Key)
					{
						case Key.Enter:  // Handle in OnPreviewKeyDown
							break;

						case Key.Oem5:
							this.updateValueFromListBox(false);
							SetValue(TextProperty, Text + "\\");
							break;

						case Key.Escape:
							this.Focus();
							hidePopup();
							break;

						default:
							e.Handled = false; break;
					}

					if (e.Handled)
					{
						Keyboard.Focus(this);
						hidePopup();
						this.Select(Text.Length, 0); // Select last char
					}
				}
			};
			#endregion

			#region Hide popup when switch to another window
			Window parentWindow = UITools.FindLogicalAncestor<Window>(this);

			if (parentWindow != null)
			{
				parentWindow.Deactivated += delegate
				{
					this._prevState = this.IsPopupOpened;
					this.IsPopupOpened = false;
				};

				parentWindow.Activated += delegate
				{
					this.IsPopupOpened = this._prevState;
				};
			}
			#endregion
		}

		protected static string getDirectoryName(string path)
		{
			if (path.EndsWith("\\"))
				return path;

			////path = path.Substring(0, path.Length - 1); //Remove ending slash.

			int idx = path.LastIndexOf('\\');
			if (idx == -1)
				return string.Empty;

			return path.Substring(0, idx);
		}

		#region OnEventHandler
		protected static void OnSuggestionsChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			SuggestBoxBase sbox = sender as SuggestBoxBase;
			if (args.OldValue != args.NewValue)
				sbox.popupIfSuggest();
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);

			switch (e.Key)
			{
				case Key.Up:
				case Key.Down:
				case Key.Prior:
				case Key.Next:
					if (this.Suggestions != null && this.Suggestions.Count > 0 && !(e.OriginalSource is ListBoxItem))
					{
						this.popupIfSuggest();
						this._itemList.Focus();
						this._itemList.SelectedIndex = 0;

						ListBoxItem lbi = this._itemList.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;

						if (lbi != null)
							lbi.Focus();

						e.Handled = true;
					}
					break;

				case Key.Return:
					if (this._itemList.IsKeyboardFocusWithin)
						this.updateValueFromListBox();

					this.hidePopup();
					this.updateSource();

					e.Handled = true;
					break;

				case Key.Back:
					if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
					{
						if (this.Text.EndsWith("\\"))
							this.SetValue(SuggestBoxBase.TextProperty, this.Text.Substring(0, this.Text.Length - 1));
						else
							this.SetValue(SuggestBoxBase.TextProperty, SuggestBoxBase.getDirectoryName(this.Text) + "\\");

						this.Select(this.Text.Length, 0);
						e.Handled = true;
					}
					break;
			}
		}
		#endregion

		#region Utils - Update Bindings
		protected virtual void updateSource()
		{
			var txtBindingExpr = this.GetBindingExpression(TextBox.TextProperty);
			if (txtBindingExpr == null)
				return;

			if (txtBindingExpr != null)
				txtBindingExpr.UpdateSource();

			this.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
		}
		#endregion Utils - Update Bindings

		#region Utils - Popup show / hide
		protected void popupIfSuggest()
		{
			if (this.IsFocused)
				if (this.Suggestions != null && this.Suggestions.Count > 0)
					this.IsPopupOpened = true;
				else
					this.IsPopupOpened = false;
		}

		protected void hidePopup()
		{
			this.IsPopupOpened = false;
		}
		#endregion

		#region Utils - Update Bindings
		private void updateValueFromListBox(bool updateSrc = true)
		{
			this.SetValue(TextBox.TextProperty, this._itemList.SelectedValue);

			if (updateSrc)
				this.updateSource();

			this.hidePopup();
		}
		#endregion Utils - Update Bindings
		#endregion methods
	}
}
