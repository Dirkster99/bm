namespace SuggestLib.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converts an int value of zero or non-zero into a configurable
    /// value of type <seealso cref="Visibility"/>.
    /// 
    /// Source: http://stackoverflow.com/questions/3128023/wpf-booleantovisibilityconverter-that-converts-to-hidden-instead-of-collapsed-wh
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class IntToVisibilityPropConverter : IValueConverter
    {
        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public IntToVisibilityPropConverter()
        {
            // set defaults
            ZeroValue = Visibility.Collapsed;
            NonZeroValue = Visibility.Visible;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets/sets the <see cref="Visibility"/> value that is associated
        /// (converted into) with the boolean true value.
        /// </summary>
        public Visibility ZeroValue { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="Visibility"/> value that is associated
        /// (converted into) with the boolean false value.
        /// </summary>
        public Visibility NonZeroValue { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Converts an int value into <see cref="Visibility"/> as configured in the
        /// <see cref="ZeroValue"/> and <see cref="NonZeroValue"/> properties.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int))
                return null;

            return (int)value == 0 ? ZeroValue : NonZeroValue;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
        #endregion methods
    }
}
