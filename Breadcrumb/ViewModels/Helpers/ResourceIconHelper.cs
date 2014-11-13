using Breadcrumb.Viewmodels.Base;
using Breadcrumb.ViewModels.Interfaces;
using Breadcrumb.ViewModels.ResourceLoader;
using BreadcrumbLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Breadcrumb.ViewModels.Helpers
{
    public class ResourceIconHelper : IconHelper
    {
        private IResourceLoader _resourceLoader;

        internal ResourceIconHelper(IResourceLoader resourceLoader, int? size)
            : base(size)
        {
            _resourceLoader = resourceLoader;            
        }

        protected override async Task<ImageSource> loadIconAsync()
        {            
            try
            {
                ImageSource retVal = await loadFromResource(_resourceLoader);
                if (retVal != null)
                    return retVal;

                if (_resourceLoader.FailSafeLoader != null)
                    return await loadFromResource(_resourceLoader.FailSafeLoader);
            }
            catch (Exception) { }
            
            return null;

        }

        private async Task<ImageSource> loadFromResource(IResourceLoader resourceLoader)
        {
            if (!(Size.HasValue))
                return new BitmapImage() { StreamSource = await resourceLoader.LoadAsync() };

            using (var stream = await resourceLoader.LoadAsync())
            {
                if (stream.Length == 0)
                    return null;

                //http://stackoverflow.com/questions/952080/how-do-you-select-the-right-size-icon-from-a-multi-resolution-ico-file-in-wpf/7024970#7024970
                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnDemand);

                var result = decoder.Frames.SingleOrDefault(f => f.Width == Size);
                if (result == default(BitmapFrame))
                    result = decoder.Frames.OrderBy(f => f.Width).First();

                return result;
            }

        }               
    }
}
