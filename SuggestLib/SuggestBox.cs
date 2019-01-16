namespace SuggestLib
{
    using Interfaces;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Implements a text based control that updates a list of suggestions
    /// when user updates a given text based path -> TextChangedEvent is raised.
    /// 
    /// This control uses <see cref="ISuggestSource"/> and HierarchyHelper
    /// to suggest entries in a seperate popup as the user types.
    /// </summary>
    public class SuggestBox : SuggestBoxBase
    {
        #region fields
        /// <summary>
        /// Implements the backing store for the <see cref="TextChangedCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextChangedCommandProperty =
            DependencyProperty.Register("TextChangedCommand",
                typeof(ICommand), typeof(SuggestBox), new PropertyMetadata(null));
        #endregion fields

        #region Constructor
        /// <summary>
        /// Static class constructor.
        /// </summary>
        static SuggestBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SuggestBox),
                new FrameworkPropertyMetadata(typeof(SuggestBox)));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public SuggestBox()
        {
            IsVisibleChanged += SuggestBox_IsVisibleChanged;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets/sets a command that should be executed whenever the text in the textbox
        /// portion of this control has changed.
        /// </summary>
        public ICommand TextChangedCommand
        {
            get { return (ICommand)GetValue(TextChangedCommandProperty); }
            set { SetValue(TextChangedCommandProperty, value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method executes when the <see cref="SuggestBoxBase.EnableSuggestions"/> dependency property
        /// has changed its value.
        /// 
        /// Overwrite this method if you want to consume changes of this property.
        /// </summary>
        /// <param name="e"></param>
        override protected void OnEnableSuggestionChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnEnableSuggestionChanged(e);

            if (((bool)e.NewValue) == true)
                QueryForSuggestions();
        }

        /// <summary>
        /// Method executes when the visibility of the control is changed to query for
        /// suggestions if this was enabled...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SuggestBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((bool)e.NewValue) == true)
                QueryForSuggestions();
        }

        /// <summary>
        /// Method executes when new text is entered in the textbox portion of the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            if (string.IsNullOrEmpty(this.Text) == false)
                IsHintVisible = false;
            else
                IsHintVisible = true;

            QueryForSuggestions();
        }

        private void QueryForSuggestions()
        {
            // A change during disabled state is likely to be caused by a bound property
            // in a viewmodel (a machine based edit rather than user input)
            // -> Lets break the message loop here to avoid unnecessary CPU processings...
            if (this.IsEnabled == false || this.IsLoaded == false)
            return;

            // Text change is likely to be from property change so we ignore it
            // if control is invisible or suggestions are currently not requested
            if (Visibility != Visibility.Visible || EnableSuggestions == false)
                return;

            if (ParentWindowIsClosing == true)
                return;

            ICommand changedCommand = this.TextChangedCommand;

            // There may not be a command bound to this after all
            if (changedCommand == null)
                return;

            var item = this.Text;

            // Check whether this attached behaviour is bound to a RoutedCommand
            if (changedCommand is RoutedCommand)
            {
                // Execute the routed command
                (changedCommand as RoutedCommand).Execute(item, this);
            }
            else
            {
                // Execute the Command as bound delegate
                changedCommand.Execute(item);
            }
        }
        #endregion
    }
}
