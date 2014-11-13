using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breadcrumb.ViewModels.ResourceLoader
{
    public class CacheResourceLoader : ResourceLoader
    {
        private IResourceLoader _actualResource;
        private MemoryStream _cachedStream;

        public CacheResourceLoader(IResourceLoader actualResource)
        {
            _actualResource = actualResource;
        }

        protected override async Task<Stream> loadAsync()
        {
            if (_cachedStream == null)
            {
                Stream retVal = await _actualResource.LoadAsync();
                _cachedStream = new MemoryStream();
                await CopyStreamAsync(retVal, _cachedStream);
            }
            return new MemoryStream(_cachedStream.ToByteArray());
        }

        public static async Task CopyStreamAsync(Stream input, Stream output, bool resetInputStream = false,
           bool resetOutputStream = false, bool closeOutputStream = false, Action<short> progress = null)
        {
            if (progress == null)
                progress = p => { };

            output.SetLength(input.Length);
            if (resetInputStream)
                input.Seek(0, SeekOrigin.Begin);
            if (resetOutputStream)
                output.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[32768];
            int read;
            float totalRead = 0;
            while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                totalRead += read;
                progress((short)Math.Truncate((totalRead / input.Length * 100.0)));
                await output.WriteAsync(buffer, 0, read).ConfigureAwait(false);
            }

            await output.FlushAsync().ConfigureAwait(false);

            if (closeOutputStream)
                output.Dispose();
        }

    }


}
