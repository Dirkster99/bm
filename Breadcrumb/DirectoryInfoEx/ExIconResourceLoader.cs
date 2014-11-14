using Breadcrumb.Defines;
using Breadcrumb.ViewModels.Helpers;
using Breadcrumb.ViewModels.ResourceLoader;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Breadcrumb.DirectoryInfoEx
{
    public class ExIconResourceLoader : ResourceLoader
    {
        private FileSystemInfoEx _fsi;
        private int? _size;
        private bool _forceLoad;

        public ExIconResourceLoader(FileSystemInfoEx fsi, bool forceLoad, int? size)
            : base()
        {
            _fsi = fsi;
            _forceLoad = forceLoad;
            _size = size;
        }

        protected override async Task<Stream> loadAsync()
        {
            IconSize size = IconSize.large;
            if (_size.HasValue)
                size = _size.Value <= 16 ? IconSize.small : 
                    _size.Value <= 32 ? IconSize.large :
                    _size.Value <= 48 ? IconSize.extraLarge :
                    _size.Value <= 128 ? IconSize.jumbo : IconSize.thumbnail;

            size = IconSize.jumbo;
            return _fsi.RequestPIDL(pidl =>
                {
                    MemoryStream ms = new MemoryStream();
                    Bitmap retBmp = null;
                    using (var imgList = new SystemImageList(size))
                        retBmp = imgList[pidl.Ptr, _fsi.IsFolder, _forceLoad];                    

                    retBmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms;
                });

        }
    }
}
