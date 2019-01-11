namespace SuggestLib.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class RecentSuggestsVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            if (values.Length < 1)
                return Binding.DoNothing;

            if ((values[0] is int) == false || (values[1] is bool) == false)
                return Binding.DoNothing;

            int count = (int)values[0];
            bool isEnabled = (bool)values[1];

            if (isEnabled == false || count == 0)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
