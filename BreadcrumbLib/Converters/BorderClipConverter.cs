namespace BreadcrumbLib.Converters
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Media;

	/// <summary>
    /// Clips inner border such that CornerRadius can be used without
    /// having the inner border stcik out of the outer border.
    /// 
	/// By Marat Khasanov in http://stackoverflow.com/questions/5649875/wpf-how-to-make-the-border-trim-the-child-elements
	/// </summary>
	public class BorderClipConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 3 && values[0] is double && values[1] is double && values[2] is CornerRadius)
			{
				var width = (double)values[0];
				var height = (double)values[1];

				if (width < double.Epsilon || height < double.Epsilon)
				{
					return Geometry.Empty;
				}

				var radius = (CornerRadius)values[2];

				// Actually we need more complex geometry, when CornerRadius has different values.
				// But let me not to take this into account, and simplify example for a common value.
				var clip = new RectangleGeometry(new Rect(0, 0, width, height), radius.TopLeft, radius.TopLeft);
				clip.Freeze();

				return clip;
			}

			return DependencyProperty.UnsetValue;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
