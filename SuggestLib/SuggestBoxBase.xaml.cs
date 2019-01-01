namespace SuggestLib
{
    using SuggestLib.Events;
    using SuggestLib.Utils;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// User update Suggestions when TextChangedEvent raised.
    /// </summary>
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    [TemplatePart(Name = PART_ResizeGripThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_ResizeableGrid, Type = typeof(Grid))]
    public class SuggestBoxBase : TextBox
    {
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
        protected Thumb _PART_ResizeGripThumb;
        protected Grid _PART_ResizeableGrid;

        // Controls whether the PopUp should open when the control has focus and suggestion
        // or not (not should be implemented if the pop-up just closed and the textbox is
        // focused to let the user continue to type into the text portion).
        protected bool _suggestionIsConsumed = false;
        protected bool _PopUpIsCancelled = false;
        private bool _prevState;

        /// <summary>
        /// Implements the backing store of the <see cref="DisplayMemberPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(SuggestBoxBase),
                    new PropertyMetadata("Header"));

        /// <summary>
        /// Implements the backing store of the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValuePathProperty =
            DependencyProperty.Register("ValuePath", typeof(string), typeof(SuggestBoxBase),
                    new PropertyMetadata("Value"));

        /// <summary>
        /// Implements the backing store of the <see cref="Suggestions"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SuggestionsProperty =
            DependencyProperty.Register("Suggestions", typeof(IList<object>), typeof(SuggestBoxBase),
            new PropertyMetadata(null, OnSuggestionsChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="HeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            HeaderedItemsControl.HeaderTemplateProperty.AddOwner(typeof(SuggestBoxBase));

        /// <summary>
        /// Implements the backing store of the <see cref="IsPopupOpened"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPopupOpenedProperty =
            DependencyProperty.Register("IsPopupOpened", typeof(bool),
            typeof(SuggestBoxBase), new UIPropertyMetadata(false, OnIsPopUpOpenChanged));

        /// <summary>
        /// Implements the backing store of the <see cref="Hint"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register("Hint", typeof(string),
                typeof(SuggestBoxBase), new PropertyMetadata(""));

        /// <summary>
        /// Implements the backing store of the <see cref="IsHintVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsHintVisibleProperty =
            DependencyProperty.Register("IsHintVisible", typeof(bool), typeof(SuggestBoxBase), new PropertyMetadata(true));

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged",
                  RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SuggestBoxBase));

        /// <summary>
        /// Implements the backing store of the <see cref="PopupBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupBorderBrushProperty =
            DependencyProperty.Register("PopupBorderBrush", typeof(Brush),
                typeof(SuggestBoxBase),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(00, 00, 00, 00))));

        /// <summary>
        /// Implements the backing store of the <see cref="PopupBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupBorderThicknessProperty =
            DependencyProperty.Register("PopupBorderThickness", typeof(Thickness),
                typeof(SuggestBoxBase), new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// Implements the backing store of the <see cref="EnableSuggestions"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableSuggestionsProperty =
            DependencyProperty.Register("EnableSuggestions",
                typeof(bool), typeof(SuggestBoxBase), new PropertyMetadata(true, OnEnableSuggestionChanged));
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

        public event EventHandler<NextTargetLocationArgs> NewLocationRequestEvent;
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
        /// Gets/sets the Watermark Hint that is shown if the user has not typed anything, yet,
        /// and if <see cref="IsHintVisible"/> is consfigured accordingly.
        /// </summary>
        public string Hint
        {
            get { return (string)GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether Watermark in the textbox portion of suggestion box
        /// is currently visible or not.
        /// </summary>
        public bool IsHintVisible
        {
            get { return (bool)GetValue(IsHintVisibleProperty); }
            set { SetValue(IsHintVisibleProperty, value); }
        }

        /// <summary>
        /// Gets/sets the <see cref="Brush"/> of the border in the popup
        /// of the suggestion box.
        /// </summary>
        public Brush PopupBorderBrush
        {
            get { return (Brush)GetValue(PopupBorderBrushProperty); }
            set { SetValue(PopupBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets/sets the <see cref="Thickness"/> of the border in the popup
        /// of the suggestion box.
        /// </summary>
        public Thickness PopupBorderThickness
        {
            get { return (Thickness)GetValue(PopupBorderThicknessProperty); }
            set { SetValue(PopupBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether suggestions should currently be queried and viewed or not.
        /// </summary>
        public bool EnableSuggestions
        {
            get { return (bool)GetValue(EnableSuggestionsProperty); }
            set { SetValue(EnableSuggestionsProperty, value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Is called when a control template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _PART_Popup = this.Template.FindName(PART_Popup, this) as Popup;
            _PART_ItemList = this.Template.FindName(PART_ItemList, this) as ListBox;

            // Find Grid for resizing with Thumb
            _PART_ResizeableGrid = Template.FindName(PART_ResizeableGrid, this) as Grid;

            // Find Thumb in Popup
            _PART_ResizeGripThumb = Template.FindName(PART_ResizeGripThumb, this) as Thumb;

            // Set the handler
            if (_PART_ResizeGripThumb != null && _PART_ResizeableGrid != null)
                _PART_ResizeGripThumb.DragDelta += new DragDeltaEventHandler(MyThumb_DragDelta);

            _PART_Root = this.Template.FindName(PART_Root, this) as Grid;

            this.GotKeyboardFocus += SuggestBoxBase_GotKeyboardFocus;

            this.LostKeyboardFocus += SuggestBoxBase_LostKeyboardFocus;

            if (_PART_ItemList != null)
                AttachHandlers(_PART_ItemList);

            SaveRestorePopUpStateOnWindowDeActivation();
        }

        /// <summary>
        /// Call this method to clear the list of available suggestion before
        /// or after using this SuggestionBox. Usually this method should be called
        /// before the SuggestBox is used (eg. is visible or gets the focus).
        /// 
        /// But it can also be called after using the SuggestionBox to select an entry
        /// to initialize the control for its next usage - doing so makes ensure fresh
        /// values being pulled each time the SuggestionBox is used (with different values).
        /// </summary>
        public void InitializeSuggestions()
        {
            if (Suggestions != null)
            {
                if (Suggestions.Count > 0)
                    Suggestions.Clear();  // Clear previous suggestion to ensure new/current results
            }
        }

        private void SuggestBoxBase_LostKeyboardFocus(object sender,
                                                      KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus == null)
                SetPopUp(false, "LostKeyboardFocus");
            else
            {
                if (IsKeyboardFocusWithin == false &&
                    e.NewFocus != e.OldFocus)
                {
                    _PopUpIsCancelled = true;
                    SetPopUp(false, "LostKeyboardFocus");
                }
            }

            IsHintVisible = string.IsNullOrEmpty(Text);
            this._suggestionIsConsumed = false;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            // if (this.AutoSelectBehavior == AutoSelectBehavior.Never)
            //     return;

            // Select All on gaining focus requires to not implement cursor
            // positioning on mouse click into the TextBox control
            if (this.IsKeyboardFocusWithin == false)
            {
                this.Focus();
                e.Handled = true;  //prevent from removing the selection
            }
        }

        private void SuggestBoxBase_GotKeyboardFocus(object sender,
                                                     KeyboardFocusChangedEventArgs e)
        {
            // Select all text when control gains focus from a different control.
            // If the focus was not in one of the children (or popup),
            // we select all the text 
            //            if (!TreeHelper.IsDescendantOf(e.OldFocus as DependencyObject, this))
            //                this.SelectAll();

            _PopUpIsCancelled = false;
            this.PopupIfSuggest();
            IsHintVisible = false;
        }

        /// <summary>
        /// Attempts to find the window that contains this pop-up control
        /// and attaches a handler for window activation and deactivation
        /// to save and restore pop-up state such that pop up is never open
        /// when window is deactivated.
        /// </summary>
        private void SaveRestorePopUpStateOnWindowDeActivation()
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                // Save Popup state and close popup on Window deactivated
                parentWindow.Deactivated += delegate
                {
                    _prevState = IsPopupOpened;
                    SetPopUp(false, "parentWindow.Deactivated");
                };

                // Restore Popup state (may open or close popup) on Window activated
                parentWindow.Activated += delegate
                {
                    SetPopUp(_prevState, "parentWindow.Activated");
                };

                parentWindow.PreviewMouseDown += ParentWindow_PreviewMouseDown;
                parentWindow.SizeChanged += ParentWindow_SizeChanged;
                parentWindow.LocationChanged += ParentWindow_LocationChanged;
            }
        }

        /// <summary>
        /// Close the pop-up if user drags the window around while its open.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParentWindow_LocationChanged(object sender, EventArgs e)
        {
            if (IsPopupOpened)
            {
                _PopUpIsCancelled = true;
                SetPopUp(false, "ParentWindow_SizeChanged");
                //e.Handled = true;
            }
        }

        /// <summary>
        /// Close pop-up element if window is re-positioned while pop-up is open
        /// (instead of looking stupid with an open popup at an invalid x,y position).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsPopupOpened)
            {
                _PopUpIsCancelled = true;
                SetPopUp(false, "ParentWindow_SizeChanged");
                //e.Handled = true;
            }
        }

        /// <summary>
        /// Is invoked when any element in the window is clicked and closes
        /// the popup if it was open and the clicked item was something else.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParentWindow_PreviewMouseDown(object sender,
                                                   MouseButtonEventArgs e)
        {
            // Close the pop-up if target of mouse click was not within the visual tree
            // of this control - this closes the pop-up when window is moved on open popup
            if (!TreeHelper.IsDescendantOf(e.OriginalSource as DependencyObject, this))
            {
                if (IsPopupOpened)
                {
                    _PopUpIsCancelled = true;
                    SetPopUp(false, "ParentWindow_PreviewMouseDown");
                    //e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Attaches mouse and keyboard handlers to the <paramref name="itemList"/>
        /// to make selected entries navigatable for the user.
        /// </summary>
        /// <param name="itemList"></param>
        private void AttachHandlers(ListBox itemList)
        {
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

                    bool Eventhandled = false;
                    e.Handled = true;
                    switch (e.Key)
                    {
                        case Key.Enter:
                        case Key.Tab:
                            //Handle in OnPreviewKeyDown of TextBox
                            break;

                        case Key.Escape:
                            Keyboard.Focus(this);
                            SetPopUp(false, "itemList.PreviewKeyDown");
                            Eventhandled = true;
                            break;

                        default:
                            e.Handled = false;
                            Eventhandled = true;
                            break;
                    }

                    // Close pop-up and update textbox with selected item from listbox
                    if (Eventhandled == false)
                    {
                        Keyboard.Focus(this);
                        SetPopUp(false, "itemList.PreviewKeyDown");
                        this.Select(Text.Length, 0); // Select last char
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
            this.SetValue(TextBox.TextProperty, _PART_ItemList.SelectedValue);

            if (updateSrc == true)
                updateSource();

            SetPopUp(false, "updateValueFromListBox");
        }

        /// <summary>
        /// Updates the TextBox.Text Binding expression (if any) and
        /// raises the <see cref="ValueChangedEvent"/> event to notify
        /// subscribers of the changed text value.
        /// </summary>
        protected virtual void updateSource()
        {
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
            if (this.IsFocused)
            {
                if (Suggestions != null && Suggestions.Count > 0)
                {
                    if (this._suggestionIsConsumed == false && IsKeyboardFocusWithin)
                        SetPopUp(true, "PopupIfSuggest() 1");
                }
                else
                    SetPopUp(false, "PopupIfSuggest() 2");
            }
        }

        private void SetPopUp(bool newIsOpenValue, string sourceOfReuqest)
        {
            if (IsPopupOpened != newIsOpenValue)
            {
                IsPopupOpened = newIsOpenValue;
            }
        }
        #endregion

        #region OnEventHandler
        /// <summary>
        /// Called when the System.Windows.UIElement.KeyDown occurs.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
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

                case Key.Return:    // close pop-up with selection on enter or tab
                case Key.Tab:
                    if (IsPopupOpened == true)
                    {
                        if (_PART_ItemList.IsKeyboardFocusWithin)
                            updateValueFromListBox(false);

                        SetPopUp(false, "OnPreviewKeyDown");
                        updateSource();

                        e.Handled = true;
                    }
                    else
                    {
                        // Time to tell the outside world: We are changing location
                        MessageEditResult(EditPathResult.OK);
                    }
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
                    if (IsPopupOpened == true)
                    {
                        _PopUpIsCancelled = true;
                        SetPopUp(false, "OnPreviewKeyDown");
                        e.Handled = true;
                    }
                    else
                    {
                        // Tell the outside world: We are NOT changing location
                        MessageEditResult(EditPathResult.Cancel);
                    }
                    break;

                default:
                    // Other key gestures can be processed without special handlers
                    break;
            }
        }

        /// <summary>
        /// Method creates and sends an <see cref="EditResult"/> event to
        /// attached listners (if any). This event can be used to react on simple
        /// keyboard short-cuts like Enter or Escape...
        /// </summary>
        /// <param name="result"></param>
        protected void MessageEditResult(EditPathResult result)
        {
            var message = new EditResult(result, Text);

            NewLocationRequestEvent?.Invoke(this, new NextTargetLocationArgs(message));
        }

        /// <summary>
        /// Method executes when the <see cref="EnableSuggestions"/> dependency property
        /// has changed its value.
        /// 
        /// Overwrite this method if you want to consume changes of this property.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnEnableSuggestionChanged(DependencyPropertyChangedEventArgs e)
        {
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

            // Do not react if popup was cancelled (eg: focus travelled off from here)
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

        /// <summary>
        /// Method executes when the <see cref="EnableSuggestions"/> dependency property
        /// has changed its value.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnEnableSuggestionChanged(DependencyObject d,
                                                      DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as SuggestBoxBase;
            if (ctrl != null)
                ctrl.OnEnableSuggestionChanged(e);
        }
        #endregion
    }
}
