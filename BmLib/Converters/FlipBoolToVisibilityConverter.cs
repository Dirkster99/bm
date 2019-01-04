namespace BmLib.Converters
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

    /// <summary>
    /// Implements a bool to visibility converter.
    /// </summary>
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class FlipBoolToVisibilityConverter : IValueConverter
	{
		#region IValueConverter Members
        /// <summary>
        /// Converts a bool true value to <see cref="Visibility.Visible"/> and
        /// false into <see cref="Visibility.Hidden"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
				return Visibility.Hidden;

            return Visibility.Visible;
		}

        /// <summary>
        /// Converts a <see cref="Visibility.Hidden"/> value to bool true and
        /// any other value to false.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((Visibility)value == Visibility.Hidden)
				return true;

			return false;
		}
		#endregion
	}
}
