using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breadcrumb.ViewModels.ResourceLoader
{
    public interface IResourceLoader
    {
        /// <summary>
        /// Use this loader's LoadAsync if current code failed. (e.g. return null or exception).
        /// </summary>
        IResourceLoader FailSafeLoader { get; set; }
        Task<Stream> LoadAsync();
    }

}
