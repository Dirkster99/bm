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
    public class ResourceIconHelper : IconHelper
    {
        private IResourceLoader _resourceLoader;

        internal ResourceIconHelper(IResourceLoader resourceLoader, int? size)
            : base(size)
        {
            _resourceLoader = resourceLoader;            
        }

        protected override async Task<Stream> loadValueAsync()
        {
            Stream retVal = await _resourceLoader.LoadAsync();
            if (retVal != null)
                return retVal;
            if (_resourceLoader.FailSafeLoader != null)
                return await _resourceLoader.FailSafeLoader.LoadAsync();

            return null;

        }        
    }
}
