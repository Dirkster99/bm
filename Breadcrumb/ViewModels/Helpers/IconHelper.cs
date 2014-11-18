using Breadcrumb.Viewmodels.Base;
using Breadcrumb.ViewModels.Interfaces;
using Breadcrumb.ViewModels.ResourceLoader;
using BreadcrumbLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Breadcrumb.ViewModels.Helpers
{
    public abstract class IconHelper : NotifyPropertyChanged, IIconHelper
    {
        public static IIconHelper FromResourceLoader(IResourceLoader resource, int? size)
        {
            return new ResourceIconHelper(resource, size);
        }

        public static IIconHelper Undefined = new NullIconHelper();

        private Stream _value = null;
        private int? _size;

        public IconHelper(int? size)
        {
            Size = size;
        }

        public int? Size { get { return _size; } private set { _size = value; } }

        public Stream Value
        {
            get
            {
                RefreshAsync(false);
                return _value;
            }
            private set
            {
                _value = value;
                NotifyOfPropertyChanged(() => Value);
            }
        }

        public async Task RefreshAsync(bool force = true)
        {
            if (_value == null || force)
                await loadValueAsync().ContinueWith(tsk =>
                    {
                        if (!tsk.IsFaulted && tsk.Result != null)
                            Value = tsk.Result;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public Task RefreshAsync()
        {
            if (_value != null)
                return RefreshAsync(true);
            else return Task.Delay(0);
        }

        protected abstract Task<Stream> loadValueAsync();
    }
}
