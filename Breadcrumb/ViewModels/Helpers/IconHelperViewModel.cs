using Breadcrumb.Viewmodels.Base;
using Breadcrumb.ViewModels.ResourceLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breadcrumb.ViewModels.Helpers
{
    public class IconHelperViewModel : NotifyPropertyChanged
    {
        private IResourceLoader _resourceLoader;

        public IconHelperViewModel(IResourceLoader resourceLoader)
        {
            _resourceLoader = resourceLoader;
            Size16 = new IconHelper(_resourceLoader, 16);
            Size32 = new IconHelper(_resourceLoader, 32);
            Size48 = new IconHelper(_resourceLoader, 48);
        }

        public void Refresh()
        {
            Size16.Refresh();
            Size32.Refresh();
            Size48.Refresh();
        }

        public IconHelper Size16 { get; set; }
        public IconHelper Size32 { get; set; }
        public IconHelper Size48 { get; set; }
    }

}
