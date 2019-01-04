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
	public class BoolToVisibilityCollapsedConverter : IValueConverter
	{
        #region IValueConverter Members
        /// <summary>
        /// Converts a bool true value to <see cref="Visibility.Visible"/> and
        /// false into <see cref="Visibility.Collapsed"/>.
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
        /// Converts a <see cref="Visibility.Collapsed"/> value to bool false and
        /// any other value to true.
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
