namespace BmLib.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Implements a control that can display ContentOn or ContentOff
    /// depending on whether the <see cref="Switch.IsSwitchOn"/> dependency property
    /// is true or not.
    /// </summary>
    public class Switch : HeaderedContentControl
    {
        #region fields
        /// <summary>
        /// Backing store of the <see cref="IsSwitchOn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSwitchOnProperty =
            DependencyProperty.Register("IsSwitchOn", typeof(bool),
                typeof(Switch), new PropertyMetadata(false));

        /// <summary>
        /// Backing store of the <see cref="ContentOn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentOnProperty =
            DependencyProperty.Register("ContentOn", typeof(object),
                typeof(Switch), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="ContentOff"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentOffProperty =
            DependencyProperty.Register("ContentOff", typeof(object),
                typeof(Switch), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="SwitchContentCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SwitchContentCommandProperty =
            DependencyProperty.Register("SwitchContentCommand", typeof(ICommand),
                typeof(Switch), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of the <see cref="CanSwitchContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanSwitchContentProperty =
            DependencyProperty.Register("CanSwitchContent", typeof(bool),
                typeof(Switch), new PropertyMetadata(true));
        #endregion fields

        #region ctors
        /// <summary>
        /// Static class constructor
        /// </summary>
        static Switch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Switch),
                new FrameworkPropertyMetadata(typeof(Switch)));
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets/sets whether the switch (and its content) is currently turned on or off.
        /// </summary>
        public bool IsSwitchOn
        {
            get { return (bool)GetValue(IsSwitchOnProperty); }
            set { SetValue(IsSwitchOnProperty, value); }
        }

        /// <summary>
        /// Gets/sets the content that is displayed when the switch is turned on.
        /// </summary>
        public object ContentOn
        {
            get { return (object)GetValue(ContentOnProperty); }
            set { SetValue(ContentOnProperty, value); }
        }

        /// <summary>
        /// Gets/sets the content that is displayed when the switch is turned off.
        /// </summary>
        public object ContentOff
        {
            get { return (object)GetValue(ContentOffProperty); }
            set { SetValue(ContentOffProperty, value); }
        }

        /// <summary>
        /// Gets/sets whether the <see cref="Switch"/> control that can host 2 controls
        /// can currently by switched by the user (by clicking the left most root toggle
        /// button) or not.
        /// </summary>
        public bool CanSwitchContent
        {
            get { return (bool)GetValue(CanSwitchContentProperty); }
            set { SetValue(CanSwitchContentProperty, value); }
        }

        /// <summary>
        /// Gets/sets a command to switch the content of the <see cref="Switch"/> control.
        /// </summary>
        public ICommand SwitchContentCommand
        {
            get { return (ICommand)GetValue(SwitchContentCommandProperty); }
            set { SetValue(SwitchContentCommandProperty, value); }
        }
        #endregion properties

        #region methods

        /// <summary>
        /// Measures the child elements of a <seealso cref="StackPanel"/> 
        /// in anticipation of arranging them during the
        /// <seealso cref="StackPanel.ArrangeOverride(System.Windows.Size)"/>
        /// </summary>
        /// <param name="constraint">An upper limit <seealso cref="Size"/> that should not be exceeded.</param>
        /// <returns>The System.Windows.Size that represents the desired size of the element.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
#if DEBUG
            if (double.IsPositiveInfinity(constraint.Width)) // || double.IsPositiveInfinity(constraint.Height))
            {
                // This constrain hints a layout proplem that can cause items to NOT Overflow.
                System.Diagnostics.Debug.WriteLine("   +---> Warning: Switch.MeasureOverride(Size constraint) with constraint == Infinity");
            }
#endif
            return base.MeasureOverride(constraint);
        }
        #endregion methods
    }
}
