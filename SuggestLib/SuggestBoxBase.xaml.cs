namespace SuggestLib
{
    using SuggestLib.Utils;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// User update Suggestions when TextChangedEvent raised.
    /// </summary>
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    [TemplatePart(Name = PART_ResizeGripThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_ResizeableGrid, Type = typeof(Grid))]
    public class SuggestBoxBase : TextBox
    {
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const string PART_Root = "PART_Root";
        public const string PART_Popup = "PART_Popup";
        public const string PART_ItemList = "PART_ItemList";
        public const string PART_ContentHost = "PART_ContentHost";
        public const string PART_ResizeGripThumb = "PART_ResizeGripThumb";
        public const string PART_ResizeableGrid = "PART_ResizeableGrid";

        #region fields
        protected Popup _PART_Popup;
        protected ListBox _PART_ItemList;
        protected Grid _PART_Root;
        ////protected ScrollViewer _PART_ContentHost;
        ////protected UIElement _TextBoxView;

        protected Thumb _PART_ResizeGripThumb;
        protected Grid _PART_ResizeableGrid;

        // Controls whether the PopUp should open when the control has focus and suggestion
        // or not (not should be implemented if the pop-up just closed and the textbox is
        // focused to let the user continue to type into the text portion).
        protected bool _suggestionIsConsumed = false;
        protected bool _PopUpIsCancelled = false;
        private bool _prevState;

        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register(
                    "DisplayMemberPath", typeof(string), typeof(SuggestBoxBase),
                    new PropertyMetadata("Header"));

        public static readonly DependencyProperty ValuePathProperty = DependencyProperty.Register(
                    "ValuePath", typeof(string), typeof(SuggestBoxBase),
                    new PropertyMetadata("Value"));

        public static readonly DependencyProperty SuggestionsProperty = DependencyProperty.Register(
            "Suggestions", typeof(IList<object>), typeof(SuggestBoxBase),
            new PropertyMetadata(null, OnSuggestionsChanged));

        public static readonly DependencyProperty HeaderTemplateProperty =
            HeaderedItemsControl.HeaderTemplateProperty.AddOwner(typeof(SuggestBoxBase));

        public static readonly DependencyProperty IsPopupOpenedProperty =
            DependencyProperty.Register("IsPopupOpened", typeof(bool),
            typeof(SuggestBoxBase), new UIPropertyMetadata(false, OnIsPopUpOpenChanged));

        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register("Hint", typeof(string), typeof(SuggestBoxBase), new PropertyMetadata(""));

        public static readonly DependencyProperty IsHintVisibleProperty =
            DependencyProperty.Register("IsHintVisible", typeof(bool), typeof(SuggestBoxBase), new PropertyMetadata(true));

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged",
          RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SuggestBoxBase));
        #endregion fields

        #region Constructor
        /// <summary>
        /// Static class constructor
        /// </summary>
        static SuggestBoxBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SuggestBoxBase),
                new FrameworkPropertyMetadata(typeof(SuggestBoxBase)));
        }

        /// <summary>
        /// Standard class constructor
        /// </summary>
        public SuggestBoxBase()
        {
        }
        #endregion

        #region Events
        /// <summary>
        /// Gets/sets a routed event handler that is invoked whenever the value of the
        /// Textbox.TextProperty of this textbox derived control has changed.
        /// </summary>
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets/sets the DisplayMemberPath for the ListBox portion of the suggestion popup.
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// Gets/sets the ValuePath for the ListBox portion of the suggestion popup.
        /// </summary>
        public string ValuePath
        {
            get { return (string)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        /// <summary>
        /// Gets/sets the ItemTemplate for the ListBox portion of the suggestion popup.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets/sets a list of suggestions that are shown to suggest
        /// alternative or more complete items while a user is typing.
        /// </summary>
        public IList<object> Suggestions
        {
            get { return (IList<object>)GetValue(SuggestionsProperty); }
            set { SetValue(SuggestionsProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether the Popup portion of the control is currently open or not.
        /// </summary>
        public bool IsPopupOpened
        {
            get { return (bool)GetValue(IsPopupOpenedProperty); }
            set { SetValue(IsPopupOpenedProperty, value); }
        }

        /// <summary>
        /// Gets/sets the Watermark Hint that is shown if the user has not typed anything, yet. 
        /// </summary>
        public string Hint
        {
            get { return (string)GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether popup portion of suggestion box is currently visible or not.
        /// </summary>
        public bool IsHintVisible
        {
            get { return (bool)GetValue(IsHintVisibleProperty); }
            set { SetValue(IsHintVisibleProperty, value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Is called when a control template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            logger.DebugFormat("{0}", (string.IsNullOrEmpty(Text) ? "" : Text));

            base.OnApplyTemplate();
            _PART_Popup = this.Template.FindName(PART_Popup, this) as Popup;
            _PART_ItemList = this.Template.FindName(PART_ItemList, this) as ListBox;
            ////_PART_ContentHost = this.Template.FindName(PART_ContentHost, this) as ScrollViewer;

            // Find Grid for resizing with Thumb
            _PART_ResizeableGrid = Template.FindName(PART_ResizeableGrid, this) as Grid;

            // Find Thumb in Popup
            _PART_ResizeGripThumb = Template.FindName(PART_ResizeGripThumb, this) as Thumb;

            // Set the handler
            if (_PART_ResizeGripThumb != null && _PART_ResizeableGrid != null)
                _PART_ResizeGripThumb.DragDelta += new DragDeltaEventHandler(MyThumb_DragDelta);

            ////if (_PART_ContentHost != null)
            ////    _TextBoxView = LogicalTreeHelper.GetChildren(_PART_ContentHost).OfType<UIElement>().First();

            _PART_Root = this.Template.FindName(PART_Root, this) as Grid;

            this.GotKeyboardFocus += (o, e) =>
            {
                _PopUpIsCancelled = false;
                this.PopupIfSuggest();
                IsHintVisible = false;
            };

            this.LostKeyboardFocus += (o, e) =>
            {
                logger.DebugFormat("LostKeyboardFocus Old Focus {0} New Focus {1}",
                    (e.OldFocus == null ? "" : e.OldFocus.ToString()),
                    (e.NewFocus == null ? "" : e.NewFocus.ToString()));

                if (e.NewFocus == null)
                {
                    this.hidePopup();
                }
                else
                {
                    if (IsKeyboardFocusWithin == false)
                    {
                        logger.DebugFormat("IsKeyboardFocusWithin {0} -> hidePopup()", IsKeyboardFocusWithin);
                        _PopUpIsCancelled = true;
                        this.hidePopup();
                    }
                }

                IsHintVisible = string.IsNullOrEmpty(Text);
                this._suggestionIsConsumed = false;
            };

            if (_PART_Popup != null && _PART_Root != null)
            {
                //09-04-09 Based on SilverLaw's approach 
                _PART_Popup.CustomPopupPlacementCallback += new CustomPopupPlacementCallback(
                    (popupSize, targetSize, offset) => new CustomPopupPlacement[] {
                new CustomPopupPlacement(new Point((0.01 - offset.X),
                    (_PART_Root.ActualHeight - offset.Y)), PopupPrimaryAxis.None) });
            }

            if (_PART_ItemList != null)
                AttachHandlers(_PART_ItemList);

            SaveRestorePopUpStateOnWindowDeActivation();
        }

        /// <summary>
        /// Attempts to find the window that contains this pop-up control
        /// and attaches a handler for window activation and decativation
        /// to save and restore pop-up state such that pop up is never open
        /// when window is deactivated.
        /// </summary>
        private void SaveRestorePopUpStateOnWindowDeActivation()
        {
            logger.DebugFormat("{0}", (string.IsNullOrEmpty(Text) ? "" : Text));

            Window parentWindow = UITools.FindLogicalAncestor<Window>(this);
            if (parentWindow != null)
            {
                // Save Popup state and close popup on Window deactivated
                parentWindow.Deactivated += delegate { _prevState = IsPopupOpened; IsPopupOpened = false; };

                // Restore Popup state (may open or close popup) on Window activated
                parentWindow.Activated += delegate { IsPopupOpened = _prevState; };
            }
        }

        /// <summary>
        /// Attaches mouse and keyboard handlers to the <paramref name="itemList"/>
        /// to make selected entries navigatable for the user.
        /// </summary>
        /// <param name="itemList"></param>
        private void AttachHandlers(ListBox itemList)
        {
            logger.DebugFormat("{0}", (string.IsNullOrEmpty(Text) ? "" : Text));

            itemList.MouseDoubleClick += (o, e) => { updateValueFromListBox(); };

            itemList.PreviewMouseUp += (o, e) =>
            {
                if (itemList.SelectedValue != null)
                    updateValueFromListBox();
            };

            itemList.PreviewKeyDown += (o, e) =>
            {
                if (e.OriginalSource is ListBoxItem)
                {
                    ListBoxItem lbItem = e.OriginalSource as ListBoxItem;

                    e.Handled = true;
                    switch (e.Key)
                    {
                        case Key.Enter:
                            //Handle in OnPreviewKeyDown
                            break;
                        case Key.Oem5:
                            updateValueFromListBox(false);
                            SetValue(TextProperty, Text + "\\");
                            break;
                        case Key.Escape:
                            this.Focus();
                            hidePopup();
                            break;
                        default: e.Handled = false; break;
                    }

                    if (e.Handled)
                    {
                        Keyboard.Focus(this);
                        hidePopup();
                        this.Select(Text.Length, 0); //Select last char
                    }
                }
            };
        }

        #region Utils - Update Bindings
        /// <summary>
        /// Method is invoked when an item in the popup list is selected.
        /// 
        /// The control is derived from TextBox which is why we can set the
        /// TextBox.Text property with the SelectedValue here.
        /// 
        /// The method also calls the updateSource() method (or its override
        /// in a derived class) and closes the popup portion of the control.
        /// </summary>
        /// <param name="updateSrc">Calls the updateSource() method (or its override
        /// in a derived class) if true, method is not invoked otherwise.</param>
        private void updateValueFromListBox(bool updateSrc = true)
        {
            logger.DebugFormat("{0}", (string.IsNullOrEmpty(Text) ? "" : Text));
            this.SetValue(TextBox.TextProperty, _PART_ItemList.SelectedValue);

            if (updateSrc == true)
                updateSource();

            hidePopup();
        }

        /// <summary>
        /// Updates the TextBox.Text Binding expression (if any) and
        /// raises the <see cref="ValueChangedEvent"/> event to notify
        /// subscribers of the changed text value.
        /// </summary>
        protected virtual void updateSource()
        {
            logger.DebugFormat("{0}", (string.IsNullOrEmpty(Text) ? "" : Text));

            var txtBindingExpr = this.GetBindingExpression(TextBox.TextProperty);
            if (txtBindingExpr == null)
                return;

            txtBindingExpr.UpdateSource();
            RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }

        #endregion

        #region Utils - Popup show / hide
        /// <summary>
        /// Opens the popup control if SuggestBox has currently focus
        /// and there are suggestions available.
        /// </summary>
        protected void PopupIfSuggest()
        {
            logger.DebugFormat("{0}", (string.IsNullOrEmpty(Text) ? "" : Text));

            if (this.IsFocused)
            {
                if (Suggestions != null && Suggestions.Count > 0)
                {
                    if (this._suggestionIsConsumed == false && IsKeyboardFocusWithin)
                        IsPopupOpened = true;
                }
                else
                    IsPopupOpened = false;
            }
        }

        /// <summary>
        /// Closes the popup with the list of suggestions.
        /// </summary>
        protected void hidePopup()
        {
            logger.DebugFormat("{0}", (string.IsNullOrEmpty(Text) ? "" : Text));
            IsPopupOpened = false;
        }
        #endregion

        #region OnEventHandler
        /// <summary>
        /// Called when the System.Windows.UIElement.KeyDown occurs.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            logger.DebugFormat("{0}", (string.IsNullOrEmpty(Text) ? "" : Text));
            base.OnPreviewKeyDown(e);

            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                case Key.Prior:
                case Key.Next:
                    if (Suggestions != null && Suggestions.Count > 0 && !(e.OriginalSource is ListBoxItem))
                    {
                        PopupIfSuggest();
                        _PART_ItemList.Focus();
                        _PART_ItemList.SelectedIndex = 0;

                        ListBoxItem lbi = _PART_ItemList.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;

                        if (lbi != null)
                            lbi.Focus();

                        e.Handled = true;
                    }
                    break;

                case Key.Return:                              // close pop-up on enter
                    if (_PART_ItemList.IsKeyboardFocusWithin)
                        updateValueFromListBox();

                    hidePopup();
                    updateSource();

                    e.Handled = true;
                    break;

                case Key.Back:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        if (Text.EndsWith("\\"))
                            SetValue(TextProperty, Text.Substring(0, Text.Length - 1));
                        else
                            SetValue(TextProperty, getDirectoryName(Text) + "\\");

                        this.Select(Text.Length, 0);
                        e.Handled = true;
                    }
                    break;

                case Key.Escape:
                    _PopUpIsCancelled = true;
                    hidePopup();
                    e.Handled = true;
                    break;

                default:
                    // Other key gestures can be processed without special handlers
                    break;
            }
        }

        /// <summary>
        /// Returns:
        /// 1) The current path if it ends with a '\' character or
        /// 2) The next substring that is delimited by a '\' character or
        /// 3 an empty string if there is no '\' character present.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected static string getDirectoryName(string path)
        {
            logger.DebugFormat("{0}", (string.IsNullOrEmpty(path) ? "" : path));
            if (path.EndsWith("\\"))
                return path;
            //path = path.Substring(0, path.Length - 1); //Remove ending slash.

            int idx = path.LastIndexOf('\\');
            if (idx == -1)
                return "";

            return path.Substring(0, idx);
        }

        /// <summary>
        /// Is invoked when the bound list of suggestions in the <see cref="SuggestionsProperty"/>
        /// has changed and shows the popup list if control has focus and there are
        /// suggestions available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected static void OnSuggestionsChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            SuggestBoxBase sbox = sender as SuggestBoxBase;

            if (args.OldValue != args.NewValue)
            {
                logger.DebugFormat("_", (string.IsNullOrEmpty(sbox.Text) ? "" : sbox.Text));
                sbox.PopupIfSuggest();
            }
        }
        #endregion

        /// <summary>
        /// Method is attached on the changed handler of the IsPopUp dependency property.
        /// The IsPopUp dependency property in turn is bound to the popup and changes between
        /// true and false when the popup opens or closes.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIsPopUpOpenChanged(DependencyObject d,
                                                 DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as SuggestBoxBase;
            if (ctrl != null)
                ctrl.OnIsPopUpOpenChanged(e);
        }

        /// <summary>
        /// https://stackoverflow.com/questions/1695101/why-are-actualwidth-and-actualheight-0-0-in-this-case
        /// 
        /// Method executes when user drages the resize thumb to resize
        /// the suggestions drop down of the suggestion box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            logger.Debug("_");
            Thumb MyThumb = sender as Thumb;

            // Set the new Width and Height fo Grid, Popup they will inherit
            double yAdjust = _PART_ResizeableGrid.ActualHeight + e.VerticalChange;
            double xAdjust = _PART_ResizeableGrid.ActualWidth + e.HorizontalChange;

            // Set new Height and Width
            if (xAdjust >= 0)
                _PART_ResizeableGrid.Width = xAdjust;

            if (yAdjust >= 0)
                _PART_ResizeableGrid.Height = yAdjust;
        }

        /// <summary>
        /// Method executes when the Pop-up list is opened or closed.
        /// 
        /// The method re-focuses the textbox when the popup closes
        /// and sets the cursor at the end of the textbox string.
        /// </summary>
        /// <param name="e"></param>
        private void OnIsPopUpOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e == null)
                return;

            logger.DebugFormat("Old Value: {0}, New Value: {1}, Text {2}",
                e.NewValue, e.OldValue, this.Text);

            // Do not react if popup was cancelled (eg: focuse travelled off from here)
            // since gestures like Escape key of focus travelling should cancel
            // the drop down selection workflow
            if (IsKeyboardFocusWithin == true)
            {
                // Set cursor at end of string when pop is closed
                if (((bool)e.NewValue) == false && ((bool)e.OldValue) == true)
                {
                    if (string.IsNullOrEmpty(this.Text) == false)
                        this.SelectionStart = this.Text.Length;
                    else
                        this.SelectionStart = 0;

                    this._suggestionIsConsumed = true;
                    this.Focus();
                }
            }
        }
        #endregion
    }
}
