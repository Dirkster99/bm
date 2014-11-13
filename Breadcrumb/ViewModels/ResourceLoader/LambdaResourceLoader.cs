using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breadcrumb.ViewModels.ResourceLoader
{
    public class LambdaResourceLoader : ResourceLoader
    {
        private Func<Task<Stream>> _loadTaskFunc;

        public LambdaResourceLoader(Func<Task<Stream>> loadTaskFunc)
        {
            _loadTaskFunc = loadTaskFunc;
        }

        protected override Task<Stream> loadAsync()
        {
            return _loadTaskFunc();
        }
    }

}
