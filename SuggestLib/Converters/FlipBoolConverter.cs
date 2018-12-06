namespace SuggestLib.Converters
{
    using System;
	using System.Globalization;
	using System.Windows.Data;

	[ValueConversion(typeof(bool), typeof(bool))]
	public class FlipBoolConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
				return false;
			else return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
				return false;
			else return true;
		}

		#endregion
	}
}
