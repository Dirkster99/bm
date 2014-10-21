namespace Breadcrumb.Utils
{
	using System;
	using System.Collections.Generic;

#if !NETFX_CORE
	using System.Drawing;
	using System.Drawing.Imaging;
#endif

	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows;

#if !NETFX_CORE
	using System.Windows.Media.Imaging;
	using System.Windows.Threading;
#endif

#if !NETFX_CORE
	/// <summary>
	/// http://www.codeproject.com/Articles/104929/Bitmap-to-BitmapSource
	/// </summary>
	public static class BitmapSourceUtils
	{
		private static bool InvokeRequired
		{
			get { return Application.Current != null && Dispatcher.CurrentDispatcher != Application.Current.Dispatcher; }
		}

		public static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
		{
			if (bitmap == null)
				throw new ArgumentNullException("bitmap");

			if (Application.Current == null || Application.Current.Dispatcher == null)
				return null; // Is it possible?

			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					// You need to specify the image format to fill the stream. 
					// I'm assuming it is PNG
					bitmap.Save(memoryStream, ImageFormat.Png);
					memoryStream.Seek(0, SeekOrigin.Begin);

					// Make sure to create the bitmap in the UI thread
					if (InvokeRequired)
						return (BitmapSource)Application.Current.Dispatcher.Invoke(
								new Func<Stream, BitmapSource>(CreateBitmapSourceFromBitmap),
								DispatcherPriority.Normal,
								memoryStream);

					return CreateBitmapSourceFromBitmap(memoryStream);
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static BitmapSource CreateBitmapSourceFromBitmap(Stream stream)
		{
			BitmapDecoder bitmapDecoder = BitmapDecoder.Create(
					stream,
					BitmapCreateOptions.PreservePixelFormat,
					BitmapCacheOption.OnLoad);

			// This will disconnect the stream from the image completely...
			WriteableBitmap writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
			writable.Freeze();

			return writable;
		}

		public static BitmapSource CreateBitmapSourceFromBitmap(byte[] bytes)
		{
			MemoryStream stream = new MemoryStream(bytes);
			stream.Seek(0, SeekOrigin.Begin);

			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.StreamSource = stream;
			image.EndInit();
			image.Freeze();

			return image;

			////using (Bitmap bitmap = new Bitmap(new MemoryStream(bytes)))
			////    return CreateBitmapSourceFromBitmap(bitmap);
		}
	}
#endif
}
