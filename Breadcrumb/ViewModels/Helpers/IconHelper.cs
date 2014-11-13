using Breadcrumb.Viewmodels.Base;
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
    public class IconHelper : NotifyPropertyChanged
    {
        private IResourceLoader _resourceLoader;
        private ImageSource _icon;
        private double? _size;

        public IconHelper(IResourceLoader resourceLoader, double? size)
        {
            _resourceLoader = resourceLoader;
            _size = size;
        }

        public ImageSource Value
        {
            get
            {
                Refresh(false);
                return _icon;
            }
            private set
            {
                _icon = value;
                NotifyOfPropertyChanged(() => Value);
            }
        }

        public void Refresh(bool force = true)
        {
            if (_icon == null || force)
                loadIconAsync(_resourceLoader)
                    .ContinueWith(async tsk =>
                {
                    if (!tsk.IsFaulted  && tsk.Result != null)
                        Value = tsk.Result;
                    else if (_resourceLoader.FailSafeLoader != null)
                        Value = await loadIconAsync(_resourceLoader.FailSafeLoader);
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task<ImageSource> loadIconAsync(IResourceLoader resourceLoader)
        {
            if (!(_size.HasValue))
                return new BitmapImage() { StreamSource = await resourceLoader.LoadAsync() };

            using (var stream = await resourceLoader.LoadAsync())
            {
                if (stream.Length == 0)
                    return null;

                //http://stackoverflow.com/questions/952080/how-do-you-select-the-right-size-icon-from-a-multi-resolution-ico-file-in-wpf/7024970#7024970
                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnDemand);

                var result = decoder.Frames.SingleOrDefault(f => f.Width == _size);
                if (result == default(BitmapFrame))
                    result = decoder.Frames.OrderBy(f => f.Width).First();

                return result;
            }

        }

        //private async Task loadIconIfNotLoadedAsync()
        //{
        //    if (_icon == null)
        //        Value = await loadIconAsync(_resourceLoader);
        //}

    }
}
