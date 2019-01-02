namespace SuggestLib
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Implements a control that lets the user pick a suggestion
    /// from a popup list of recently visited locations.
    /// </summary>
    public class SuggestComboBox : ComboBox
    {
        #region fields
        /// <summary>
        /// Implements the backing store for the <see cref="PopUpWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopUpWidthProperty =
            DependencyProperty.Register("PopUpWidth", typeof(double), typeof(SuggestComboBox),
                new PropertyMetadata(10.0));

        #region ToggleRecentListCommand
        /// <summary>
        /// Backing store of the <see cref="ToggleRecentListCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToggleRecentListCommandProperty =
            DependencyProperty.Register("ToggleRecentListCommand", typeof(ICommand),
                typeof(SuggestComboBox), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="ToggleRecentListCommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToggleRecentListCommandParameterProperty =
            DependencyProperty.Register("ToggleRecentListCommandParameter", typeof(object),
                typeof(SuggestComboBox), new PropertyMetadata(null));
        #endregion ToggleRecentListCommand
        #endregion fields

        /// <summary>
        /// Static class constructor
        /// </summary>
        static SuggestComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SuggestComboBox),
                new FrameworkPropertyMetadata(typeof(SuggestComboBox)));
        }

        /// <summary>
        /// Gets/sets the width if the pop-up that opens underneath the toggle button.
        /// Setting a custom width can be useful since the pop-up is otherwise as wide
        /// or small as the largest entry is. Aligning the width with the size of
        /// other controls (eg. TextBox that acts similar to the edit host inside the
        /// combobox) can be a useful thing to do.
        /// </summary>
        public double PopUpWidth
        {
            get { return (double)GetValue(PopUpWidthProperty); }
            set { SetValue(PopUpWidthProperty, value); }
        }

        #region ToggleRecentListCommand
        /// <summary>
        /// Gets/sets a command that toggles the pop-up portion of the suggestion list.
        /// </summary>
        public ICommand ToggleRecentListCommand
        {
            get { return (ICommand)GetValue(ToggleRecentListCommandProperty); }
            set { SetValue(ToggleRecentListCommandProperty, value); }
        }

        /// <summary>
        /// Gets/sets a command parameter for command
        /// that toggles the pop-up portion of the suggestion list.
        /// </summary>
        public object ToggleRecentListCommandParameter
        {
            get { return (object)GetValue(ToggleRecentListCommandParameterProperty); }
            set { SetValue(ToggleRecentListCommandParameterProperty, value); }
        }
        #endregion SwitchContentCommand
    }
}
