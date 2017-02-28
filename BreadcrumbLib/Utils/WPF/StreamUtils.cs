
namespace BreadcrumbLib.Utils.WPF
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Stream related Utils. Source: FileExplorer.WPF.Utils
    /// </summary>
    public static class StreamUtils
    {
        //http://stackoverflow.com/questions/230128/best-way-to-copy-between-two-stream-instances-c
        /// <summary>
        /// Copy a stream to another stream.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="resetInputStream"></param>
        /// <param name="resetOutputStream"></param>
        /// <param name="closeOutputStream"></param>
        public static void CopyStream(Stream input, Stream output, bool resetInputStream = false, bool resetOutputStream = false, bool closeOutputStream = false)
        {
            if (resetInputStream)
                input.Seek(0, SeekOrigin.Begin);
            if (resetOutputStream)
                output.Seek(0, SeekOrigin.Begin);

            output.SetLength(input.Length);
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }

            output.Flush();
            if (closeOutputStream)
                output.Dispose();
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

        //http://stackoverflow.com/questions/11266141/c-sharp-convert-system-io-stream-to-byte
        public static byte[] ToByteArray(this Stream stream, bool disposeStream = false)
        {
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            for (int totalBytesCopied = 0; totalBytesCopied < stream.Length;)
                totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
            if (disposeStream)
                stream.Dispose();
            return buffer;
        }

        ///// <summary>
        ///// Save a iconBitmap to JPEG stream.
        ///// </summary>
        ///// <param name="iconBitmap"></param>
        ///// <param name="stream"></param>
        //public static void SaveAsJPEGStream(this Bitmap bitmap, Stream stream)
        //{
        //    //http://stackoverflow.com/questions/41665/bmp-to-jpg-png-in-c
        //    var jpegCodecInfo = ImageCodecInfo.GetImageEncoders().First((e) => { return e.MimeType == "image/jpeg"; });
        //    EncoderParameters encoderParameters = new EncoderParameters(1);
        //    encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
        //    lock (bitmap)
        //        bitmap.Save(stream, jpegCodecInfo, encoderParameters);
        //}

        ///// <summary>
        ///// Save a iconBitmap to PNG stream.
        ///// </summary>
        ///// <param name="iconBitmap"></param>
        ///// <param name="stream"></param>
        //public static void SaveAsPNGStream(this Bitmap bitmap, Stream stream)
        //{
        //    //http://stackoverflow.com/questions/1394297/generate-transparent-png-c
        //    lock (bitmap)
        //        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        //    //var pngCodecInfo = ImageCodecInfo.GetImageEncoders().First((e,p) => { return e.MimeType == "image/png"; });
        //    //EncoderParameters encoderParameters = new EncoderParameters(1);
        //    //encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
        //    //iconBitmap.Save(stream, pngCodecInfo, encoderParameters);
        //}

        ///// <summary>
        ///// Read a stream and return it's CRC, does not reset or close the stream.
        ///// </summary>
        ///// <param name="stream"></param>
        ///// <returns></returns>
        //public static string GetCRC(this Stream stream)
        //{
        //    Fesersoft.Hashing.crc32 crc32 = new Fesersoft.Hashing.crc32();
        //    uint crchash = (uint)crc32.CRC(stream);
        //    return (StringUtils.ConvertToHex(crchash));
        //}
    }
}
