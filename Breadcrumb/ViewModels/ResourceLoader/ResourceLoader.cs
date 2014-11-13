using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breadcrumb.ViewModels.ResourceLoader
{

    public abstract class ResourceLoader : IResourceLoader
    {
        public static IResourceLoader FromEmbeddedResource(string library, string path2Resource, IResourceLoader failSafeLoader = null)
        {
            return new EmbeddedResourceLoader(library, path2Resource) { FailSafeLoader = failSafeLoader };
        }

        public static IResourceLoader CacheResource(IResourceLoader actualResource)
        {
            return new CacheResourceLoader(actualResource);
        }

        public static IResourceLoader FromResourceDictionary(string stringResourceKey, IResourceLoader failSafeLoader = null)
        {
            return new DictionaryResourceLoader(stringResourceKey) { FailSafeLoader = failSafeLoader };
        }

        #region properties
        public IResourceLoader FailSafeLoader
        {
            get;
            set;
        }

        #endregion

        #region methods
        protected abstract Task<Stream> loadAsync();

        public virtual async Task<Stream> LoadAsync()
        {
            try
            {
                var retVal = await loadAsync();
                if (retVal != null)
                    return retVal;                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (FailSafeLoader != null)
                return await FailSafeLoader.LoadAsync();
            else return null;
        }

        #endregion
    }
}
