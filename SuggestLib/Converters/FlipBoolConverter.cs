namespace SuggestLib.Converters
{
    using System;
	using System.Globalization;
	using System.Windows.Data;

    /// <summary>
    /// Inverts a boolean value and returns it.
    /// </summary>
	[ValueConversion(typeof(bool), typeof(bool))]
	public class FlipBoolConverter : IValueConverter
	{
		#region IValueConverter Members
        /// <summary>
        /// Returns an inverted boolean value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
				return false;
			else
                return true;
		}

        /// <summary>
        /// Returns an inverted boolean value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
				return false;
			else
                return true;
		}

		#endregion
	}
}
