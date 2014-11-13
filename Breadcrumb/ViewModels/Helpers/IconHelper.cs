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
    public abstract class IconHelper : NotifyPropertyChanged, IIconHelper
    {
        public static IIconHelper FromResourceLoader(IResourceLoader resource, int? size)
        {
            return new ResourceIconHelper(resource, size);
        }

        public static IIconHelper Undefined = new NullIconHelper();

        private ImageSource _icon;
        private int? _size;

        public IconHelper(int? size)
        {            
            Size = size;
        }

        public int? Size { get { return _size; } private set { _size = value; } }

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
                loadIconAsync().ContinueWith(tsk =>
                    {
                        if (!tsk.IsFaulted && tsk.Result != null)
                            Value = tsk.Result;                        
                    }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Refresh()
        {
            Refresh(true);
        }

        protected abstract Task<ImageSource> loadIconAsync();
    }
}
