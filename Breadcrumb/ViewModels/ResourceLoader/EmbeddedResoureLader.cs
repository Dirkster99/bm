using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace Breadcrumb.ViewModels.ResourceLoader
{

    public class EmbeddedResourceLoader : ResourceLoader
    {
        Uri _resourceUri = null;

        public EmbeddedResourceLoader(Uri resourceUri)
        {
            _resourceUri = resourceUri;
        }

        public EmbeddedResourceLoader(string library, string path2Resource)            
            : this(new Uri(String.Format("pack://application:,,,/{0};component{1}", library,
                '/' + path2Resource.TrimStart('/'))))
        {
        }



        protected override async Task<Stream> loadAsync()
        {
            try
            {
                StreamResourceInfo info = Application.GetResourceStream(_resourceUri);
                return info.Stream;
            }
            catch { return null; }
        }
    }
}
