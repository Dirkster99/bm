namespace SuggestLib.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    /// <summary>
    /// Converts an array of double values into their sum() and returns it.
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class DoubleAddConverter : IMultiValueConverter
    {
        /// <summary>
        /// Expects an array of doubles and converts these values to their sum.
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

            double[] doubleValues = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if ((values[i] is double) == false)
                    return Binding.DoNothing;

                doubleValues[i] = (double)values[i];
            }

            double resultSum = doubleValues.Sum();

            return resultSum;
        }

        /// <summary>
        /// Not implemented.
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
