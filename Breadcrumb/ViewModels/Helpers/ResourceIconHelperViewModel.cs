using Breadcrumb.Viewmodels.Base;
using Breadcrumb.ViewModels.Interfaces;
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
        public static IIconHelperViewModel FromResourceLoader(Func<int, IResourceLoader> resourceLoaderFunc)
        {
            return new ResourceIconHelperViewModel(resourceLoaderFunc);
        }

        public static IIconHelperViewModel FromResourceLoader(IResourceLoader resourceLoader)            
        {
            return FromResourceLoader(size => resourceLoader);
        }

        
        internal ResourceIconHelperViewModel(Func<int, IResourceLoader> resourceLoaderFunc)
            : base()
        {
            Size16 = new ResourceIconHelper(resourceLoaderFunc(16), 16);
            Size32 = new ResourceIconHelper(resourceLoaderFunc(32), 32);
            Size48 = new ResourceIconHelper(resourceLoaderFunc(48), 48);
            Size64 = new ResourceIconHelper(resourceLoaderFunc(64), 64);
            Size128 = new ResourceIconHelper(resourceLoaderFunc(128), 128);
            Size256 = new ResourceIconHelper(resourceLoaderFunc(256), 256);                        
        }
      
    }

}
