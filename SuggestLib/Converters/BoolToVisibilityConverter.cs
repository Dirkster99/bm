namespace SuggestLib.Converters
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	/// <summary>
	/// This class simply converts a Boolean to a Visibility
	/// with an optional invert
	/// </summary>
	[ValueConversion(typeof(Boolean), typeof(Visibility))]
	public class BoolToVisibilityConverter : IValueConverter
	{
		#region IValueConverter implementation
		/// <summary>
		/// Converts Boolean to Visibility
		/// </summary>
		public object Convert(object value, Type targetType,
				object parameter, CultureInfo culture)
		{
			if (value == null)
				return Binding.DoNothing;

			bool input = false;
			bool.TryParse(value.ToString(), out input);

			bool invertActive = true;
			if (parameter != null)
				bool.TryParse(parameter.ToString(), out invertActive);

			if (input)
				return invertActive ? Visibility.Visible : Visibility.Collapsed;
			else
				return invertActive ? Visibility.Collapsed : Visibility.Visible;
		}

		/// <summary>
		/// Convert back, but its not implemented
		/// </summary>
		public object ConvertBack(object value, Type targetType,
				object parameter, CultureInfo culture)
		{
			if ((Visibility)value != Visibility.Hidden)
				return true;
			return false;
		}
		#endregion
	}
}
