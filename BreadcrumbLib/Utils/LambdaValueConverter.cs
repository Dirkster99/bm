using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BreadcrumbLib.Utils
{

    public static class LambdaValueConverter
    {
        public static IValueConverter ConvertUsingCast<T, T1>()
            where T : class
            where T1 : class
        {
            return new LambdaValueConverter<T, T1>(t => t as T1, t1 => t1 as T);
        }
    }

    public class LambdaValueConverter<T, T1> : IValueConverter
    {
        private Func<T, Type, object, T1> _convertFunc;
        private Func<T1, Type, object, T> _convertBackFunc;

       

        public LambdaValueConverter(Func<T, Type, object, T1> convertFunc, 
            Func<T1, Type, object, T> convertBackFunc)
        {
            _convertFunc = convertFunc;
            _convertBackFunc = convertBackFunc;
        }

        public LambdaValueConverter(Func<T, T1> convertFunc, 
            Func<T1, T> convertBackFunc)
            : this((t,_, __) => convertFunc(t), (t1, _, __) => convertBackFunc(t1))
        {

        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return _convertFunc((T)value, targetType, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
             try
            {
            return _convertBackFunc((T1)value, targetType, parameter);
            }
             catch (Exception ex)
             {
                 throw ex;
             }
        }
    }
}
