using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Breadcrumb.ViewModels.ResourceLoader
{
    public class DictionaryResourceLoader : ResourceLoader
    {
        public string ResourceName { get; set; }        

        public DictionaryResourceLoader(string resourceName)
        {
            ResourceName = resourceName;
        }

        public Uri GetResourceUri()
        {
            object resource = Application.Current.Resources[ResourceName];
            if (resource is string)
                return new Uri((string)resource);
            if (resource is BitmapImage)
                return (resource as BitmapImage).UriSource;
            return null;
        }

        protected override async Task<Stream> loadAsync()
        {
            object resource = Application.Current.Resources[ResourceName];
            Uri resourceUri = GetResourceUri();
            if (resourceUri != null)
                return await new EmbeddedResourceLoader(resourceUri).LoadAsync();

            return new MemoryStream();
        }
    }
}
