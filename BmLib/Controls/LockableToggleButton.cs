namespace BmLib.Controls
{
    using System.Windows;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Implements a toggle button that can be locked without disabling the elements
    /// hosted inside the toggle button's content.
    /// 
    /// This implementation supports turning toggling functionality on/off without
    /// affecting other controls hosted by the toggle buttons content.
    /// 
    /// Source: https://stackoverflow.com/questions/2548863/how-can-i-prevent-a-togglebutton-from-being-toggled-without-setting-isenabled
    /// </summary>
    public class LockableToggleButton : ToggleButton
    {
        /// <summary>
        /// Backing store of the <see cref="CanToggle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanToggleProperty =
            DependencyProperty.Register("CanToggle", typeof(bool), typeof(LockableToggleButton),
                new PropertyMetadata(true));

        static LockableToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LockableToggleButton),
                    new FrameworkPropertyMetadata(typeof(LockableToggleButton)));
        }

        /// <summary>
        /// Gets/sets whether the toggle button can currently be toggled or not.
        /// </summary>
        public bool CanToggle
        {
            get { return (bool)GetValue(CanToggleProperty); }
            set { SetValue(CanToggleProperty, value); }
        }

        protected override void OnToggle()
        {
            if (CanToggle == true)
            {
                base.OnToggle();
            }
        }
    }
}
