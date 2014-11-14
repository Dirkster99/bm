using BreadcrumbLib.Interfaces;
using BreadcrumbLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BreadcrumbLib.Controls.SuggestBox
{
    public class SuggestBox2 : TextBox
    {
        #region fields

        private Popup _popup;
        private ListBox _itemList;
        private Grid _root;
        private ScrollViewer _host;
        private UIElement _textBoxView;
        private bool _prevState;


        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register(
            "DisplayMemberPath", typeof(string), typeof(SuggestBox2), new PropertyMetadata("Header"));

        public static readonly DependencyProperty ValuePathProperty = DependencyProperty.Register(
                "ValuePath", typeof(string), typeof(SuggestBox2), new PropertyMetadata("Value"));

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SuggestBox2));

        public static readonly DependencyProperty SuggestionsProperty = DependencyProperty.Register(
                "Suggestions", typeof(IList<object>), typeof(SuggestBox2), new PropertyMetadata(null));

        public static readonly DependencyProperty SuggestSourceProperty =
        DependencyProperty.Register("SuggestSource", typeof(ISuggestSource),
        typeof(SuggestBox2), new UIPropertyMetadata(null));

        public static readonly DependencyProperty IsPopupOpenedProperty =
                DependencyProperty.Register("IsPopupOpened", typeof(bool),
                typeof(SuggestBox2), new UIPropertyMetadata(false));

        #endregion

        #region constructors

        static SuggestBox2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SuggestBox2),
                    new FrameworkPropertyMetadata(typeof(SuggestBox2)));
        }

        public SuggestBox2()
        {
        }

        #endregion

        #region events

        #endregion

        #region properties

        /// <summary>
        /// This property are used internally to provide suggestions to PART_ItemList,
        /// Suggestion are provided by SuggestSource.SuggestAsync()
        /// </summary>
        internal IList<object> Suggestions
        {
            get { return (IList<object>)GetValue(SuggestionsProperty); }
            set { this.SetValue(SuggestionsProperty, value); }
        }

        /// <summary>
        /// DisplayMemberPath of PART_ItemList, e.g. the text to display on suggestion.
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { this.SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// ValuePath of PART_ItemList, e.g. the value on suggestion to be set to Text if doube-click or pressed enter.
        /// </summary>
        public string ValuePath
        {
            get { return (string)GetValue(ValuePathProperty); }
            set { this.SetValue(ValuePathProperty, value); }
        }

        /// <summary>
        /// Make suggestion object whenever Text changed and IsEnabled.
        /// </summary>
        public ISuggestSource SuggestSource
        {
            get { return (ISuggestSource)GetValue(SuggestSourceProperty); }
            set { SetValue(SuggestSourceProperty, value); }
        }

        /// <summary>
        /// Gets or Sets whether PART_Popup is opened.
        /// </summary>
        public bool IsPopupOpened
        {
            get { return (bool)GetValue(IsPopupOpenedProperty); }
            set { this.SetValue(IsPopupOpenedProperty, value); }
        }

        #endregion

        #region methods

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

            Keyboard.Focus(this);
            this.GotKeyboardFocus += (o, e) =>
            {
                this.popupIfSuggest();
            };

            this.LostKeyboardFocus += (o, e) =>
            {
                if (!IsKeyboardFocusWithin) this.hidePopup();
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
                        char separator = this.Text.Contains('/') ? '/' : this.Text.Contains("\\") ? '\\' : '!';

                        if (this.Text.EndsWith("\\") || this.Text.EndsWith("/"))
                            this.SetValue(SuggestBox2.TextProperty, this.Text.Substring(0, this.Text.Length - 1));
                        else
                        {
                            int idx = Text.LastIndexOfAny(new char[] { '\\', '/' });
                            if (idx != -1)
                                this.SetValue(SuggestBox2.TextProperty, Text.Substring(0, idx));
                        }

                        this.Select(this.Text.Length, 0);
                        e.Handled = true;
                    }
                    break;
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            string text = Text;

            if (IsEnabled && SuggestSource != null)
            {
                var suggestSource = SuggestSource;
                Task.Run(async () =>
                {
                    return await suggestSource.SuggestAsync(null, text, null);
                }).ContinueWith((pTask) =>
                {
                    if (!pTask.IsFaulted)
                    {
                        this.SetValue(SuggestionsProperty, pTask.Result);
                        this.popupIfSuggest();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        //protected static void OnSuggestionsChanged(object sender, DependencyPropertyChangedEventArgs args)
        //{
        //    SuggestBox2 sbox = sender as SuggestBox2;
        //    if (args.OldValue != args.NewValue)
        //        sbox.popupIfSuggest();
        //}

        #region Helpers
        /// <summary>
        /// Show popup if there's suggestion.
        /// </summary>
        protected void popupIfSuggest()
        {
            if (this.IsFocused)
                if (this.Suggestions != null && this.Suggestions.Count > 0)
                    this.IsPopupOpened = true;
                else
                    this.IsPopupOpened = false;
        }

        /// <summary>
        /// Hide popup.
        /// </summary>
        protected void hidePopup()
        {
            this.IsPopupOpened = false;
        }

        /// <summary>
        /// Update ListBox.SelectedValue to Text
        /// </summary>
        /// <param name="updateSrc"></param>
        private void updateValueFromListBox(bool updateSrc = true)
        {
            this.SetValue(TextBox.TextProperty, this._itemList.SelectedValue);

            if (updateSrc)
                this.updateSource();

            this.hidePopup();
        }

        /// <summary>
        /// Update source of TextBox.TextProperty expression 
        /// </summary>
        public virtual void updateSource()
        {
            var txtBindingExpr = this.GetBindingExpression(TextBox.TextProperty);
            if (txtBindingExpr == null)
                return;

            if (txtBindingExpr != null)
                txtBindingExpr.UpdateSource();

            this.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }
        #endregion
        #endregion
    }
}
