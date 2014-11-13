using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breadcrumb.ViewModels.ResourceLoader
{
    public static class StreamUtils
    {
        public static byte[] ToByteArray(this Stream stream, bool disposeStream = false)
        {
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            for (int totalBytesCopied = 0; totalBytesCopied < stream.Length; )
                totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
            if (disposeStream)
                stream.Dispose();
            return buffer;
        }
    }
}
