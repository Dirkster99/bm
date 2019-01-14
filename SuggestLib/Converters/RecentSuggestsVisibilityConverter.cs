namespace SuggestLib.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Implements a <see cref="IMultiValueConverter"/> that expects an int and bool
    /// value as input and outputs a <see cref="Visibility.Visible"/> if:
    /// - int > 0 and bool == true
    /// - output is otherwise <see cref="Visibility.Collapsed"/>.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class RecentSuggestsVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Convert int and bool into associated <see cref="Visibility"/> output value.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Not Implemented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
