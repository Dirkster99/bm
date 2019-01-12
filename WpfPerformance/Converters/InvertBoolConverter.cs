namespace WpfPerformance.Converters
{
    using System;
	using System.Globalization;
	using System.Windows.Data;

    /// <summary>
    /// Implements a boolean negation converter.
    /// </summary>
	[ValueConversion(typeof(bool), typeof(bool))]
	public class InvertBoolConverter : IValueConverter
	{
		#region IValueConverter Members
        /// <summary>
        /// Converts boolean true to false.
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
        /// Converts boolean true to false.
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
