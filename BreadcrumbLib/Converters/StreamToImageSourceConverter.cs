using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BreadcrumbLib.Converters
{
    [ValueConversion(typeof(Stream), typeof(ImageSource))]
    public class StreamToImageSourceConverter : IValueConverter
    {
        public int? Size { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {   
            Stream stream = value as Stream;
            
            if (stream == null)
                return null;
            
            int? size = Size;
            if (parameter != null)
            {
                int temp;
                if (Int32.TryParse(parameter.ToString(), out temp))
                    size = temp;
            }

            if (size.HasValue)
            {
                 //http://stackoverflow.com/questions/952080/how-do-you-select-the-right-size-icon-from-a-multi-resolution-ico-file-in-wpf/7024970#7024970
                        var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.None);

                        var result = decoder.Frames.SingleOrDefault(f => f.Width == size.Value);
                        if (result == default(BitmapFrame))
                            result = decoder.Frames.OrderBy(f => f.Width).First();

                        return result;
            }
            else
                return BitmapFrame.Create(stream,
                                      BitmapCreateOptions.None,
                                      BitmapCacheOption.OnLoad);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
