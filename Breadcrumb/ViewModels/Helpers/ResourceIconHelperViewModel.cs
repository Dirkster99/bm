using Breadcrumb.Viewmodels.Base;
using Breadcrumb.ViewModels.ResourceLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breadcrumb.ViewModels.Helpers
{
    public class ResourceIconHelperViewModel : IconHelperViewModel
    {
        private IResourceLoader _resourceLoader;

        internal ResourceIconHelperViewModel(IResourceLoader resourceLoader)
            : base()
        {
            _resourceLoader = resourceLoader;
            Size16 = new ResourceIconHelper(_resourceLoader, 16);
            Size32 = new ResourceIconHelper(_resourceLoader, 32);
            Size48 = new ResourceIconHelper(_resourceLoader, 48);
        }
      
    }

}
