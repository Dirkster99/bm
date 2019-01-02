namespace SuggestLib.Converters
{
    using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

    /// <summary>
    /// This class simply converts a Boolean to a Visibility value:
    /// True  -> <see cref="Visibility.Visible"/>
    /// False -> <see cref="Visibility.Collapsed"/>
    /// </summary>
    [ValueConversion(typeof(string), typeof(Visibility))]
	public class BoolToVisibilityCollapsedConverter : IValueConverter
	{
		#region IValueConverter Members
        /// <summary>
        /// Converts a boolean false/true into a <see cref="Visibility"/> value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
				return Visibility.Visible;

            return Visibility.Collapsed;
		}

        /// <summary>
        /// Converts a <see cref="Visibility"/> into a boolean false/true value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((Visibility)value != Visibility.Collapsed)
				return true;

			return false;
		}
		#endregion
	}
}
