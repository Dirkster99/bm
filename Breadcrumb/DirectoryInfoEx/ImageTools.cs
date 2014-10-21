namespace Breadcrumb.DirectoryInfoEx
{
	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;
	using System.Threading;
	using System.Windows;
	using System.Windows.Media.Imaging;
	using System.Windows.Threading;

	public static class ImageTools
	{
		#region Image Tools
		public static BitmapSource loadBitmap(Bitmap source)
		{
			if (source == null)
				source = new Bitmap(1, 1);
			MemoryStream ms = new MemoryStream();
			lock (source)
				source.Save(ms, ImageFormat.Png);
			ms.Position = 0;
			BitmapImage bi = new BitmapImage();
			bi.BeginInit();
			bi.StreamSource = ms;
			bi.EndInit();
			bi.Freeze();
			return bi;
		}

		/// <summary>
		/// Check if a jumbo icon is actually a 32x32 icon.
		/// </summary>
		/// <param name="bitmap"></param>
		/// <returns></returns>
		public static bool CheckImage(Bitmap bitmap)
		{
			System.Drawing.Point centre = new System.Drawing.Point(bitmap.Width / 2, bitmap.Height / 2);

			if (bitmap.GetPixel(centre.X, centre.Y) == System.Drawing.Color.FromArgb(0, 0, 0, 0))
				return false;

			////Debug.WriteLine(bitmap.GetPixel(centre.X, centre.Y));
			return true;
		}

		////public static BitmapSource loadBitmap(Bitmap source)
		////{
		////    IntPtr hBitmap = source.GetHbitmap();
		////    //Memory Leak fixes, for more info : http://social.msdn.microsoft.com/forums/en-US/wpf/thread/edcf2482-b931-4939-9415-15b3515ddac6/
		////    try
		////    {
		////        BitmapSource retVal = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
		////           BitmapSizeOptions.FromEmptyOptions());
		////        if (retVal.CanFreeze)
		////            retVal.Freeze();
		////        else if (Debugger.IsAttached)
		////            Debugger.Break();
		////        return retVal;
		////    }
		////    catch
		////    {
		////        return new BitmapImage();
		////    }
		////    finally
		////    {
		////        DeleteObject(hBitmap);
		////    }
		////}

		public static void clearBackground(WriteableBitmap target, bool dispatcher)
		{
			Bitmap bitmap = new Bitmap((int)target.Width, (int)target.Height);
			using (Graphics g = Graphics.FromImage(bitmap))
				g.FillRectangle(Brushes.White, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
			copyBitmap(loadBitmap(bitmap), target, dispatcher, 0, false);
		}

		public static void copyBitmap(BitmapSource source, WriteableBitmap target, bool dispatcher, int spacing, bool freezeBitmap)
		{
			int width = source.PixelWidth;
			int height = source.PixelHeight;
			int stride = width * ((source.Format.BitsPerPixel + 7) / 8);

			byte[] bits = new byte[height * stride];
			source.CopyPixels(bits, stride, 0);
			source = null;

			////original code.
			////writeBitmap.Dispatcher.Invoke(DispatcherPriority.Background,
			////    new ThreadStart(delegate
			////    {
			////        //UI Thread
			////        Int32Rect outRect = new Int32Rect(0, (int)(writeBitmap.Height - height) / 2, width, height);                    
			////        writeBitmap.WritePixels(outRect, bits, stride, 0);                                        
			////    }));

			////Bugfixes by h32

			if (dispatcher)
			{
				target.Dispatcher.BeginInvoke(DispatcherPriority.Background,
				new ThreadStart(delegate
				{
					// UI Thread
					var delta = target.Height - height;
					var newWidth = width > target.Width ? (int)target.Width : width;
					var newHeight = height > target.Height ? (int)target.Height : height;

					Int32Rect outRect = new Int32Rect((int)((target.Width - newWidth) / 2),
					                                  (int)((delta >= 0 ? delta : 0) / 2) + spacing,
																						newWidth - (spacing * 2),
																						newWidth - (spacing * 2));
					try
					{
						target.WritePixels(outRect, bits, stride, 0);
						if (freezeBitmap)
						{
							target.Freeze();
						}
					}
					catch (Exception e)
					{
						Debug.WriteLine(e);
						System.Diagnostics.Debugger.Break();
					}
				}));
			}
			else
			{
				var delta = target.Height - height;
				var newWidth = width > target.Width ? (int)target.Width : width;
				var newHeight = height > target.Height ? (int)target.Height : height;
				Int32Rect outRect = new Int32Rect(spacing, (int)((delta >= 0 ? delta : 0) / 2) + spacing,
				                                           newWidth - (spacing * 2),
																									 newWidth - (spacing * 2));

				try
				{
					target.WritePixels(outRect, bits, stride, 0);
					if (freezeBitmap)
						target.Freeze();
				}
				catch (Exception e)
				{
					Debug.WriteLine(e);
					System.Diagnostics.Debugger.Break();
				}
			}
		}

		public static Bitmap cutImage(Bitmap imgToCut, System.Drawing.Size size)
		{
			Bitmap b = new Bitmap(size.Width, size.Height);
			RectangleF rect = new RectangleF(0, 0, size.Width, size.Height);

			Graphics g = Graphics.FromImage((Image)b);
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

			g.DrawImage(imgToCut, rect, rect, GraphicsUnit.Pixel);
			g.Dispose();

			return b;
		}

		/// <summary>
		/// http://blog.paranoidferret.com/?p=11 , modified a little.
		/// </summary>
		/// <param name="imgToResize"></param>
		/// <param name="size"></param>
		/// <param name="spacing"></param>
		/// <returns></returns>
		public static Bitmap resizeImage(Bitmap imgToResize, System.Drawing.Size size, int spacing)
		{
			lock (imgToResize)
			{
				if (imgToResize == null) return null;
				if (imgToResize.Width == size.Width && imgToResize.Height == size.Height && spacing == 0)
					return imgToResize;

				int sourceWidth = imgToResize.Width;
				int sourceHeight = imgToResize.Height;

				if ((sourceWidth == size.Width) && (sourceHeight == size.Height))
					return imgToResize;

				float nPercent = 0;
				float nPercentW = 0;
				float nPercentH = 0;

				nPercentW = ((float)size.Width / (float)sourceWidth);
				nPercentH = ((float)size.Height / (float)sourceHeight);

				if (nPercentH < nPercentW)
					nPercent = nPercentH;
				else
					nPercent = nPercentW;

				int destWidth = (int)((sourceWidth * nPercent) - (spacing * 4));
				int destHeight = (int)((sourceHeight * nPercent) - (spacing * 4));

				int leftOffset = (int)((size.Width - destWidth) / 2);
				int topOffset = (int)((size.Height - destHeight) / 2);

				Bitmap b = new Bitmap(size.Width, size.Height);
				using (Graphics g = Graphics.FromImage((Image)b))
				{
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
					if (spacing > 0)
					{
						g.DrawLines(System.Drawing.Pens.Silver, new PointF[]
						{
                        new PointF(leftOffset - spacing, topOffset + destHeight + spacing),   // BottomLeft
                        new PointF(leftOffset - spacing, topOffset - spacing),                // TopLeft
                        new PointF(leftOffset + destWidth + spacing, topOffset - spacing)     // TopRight
						});

						g.DrawLines(System.Drawing.Pens.Gray, new PointF[]
						{
                        new PointF(leftOffset + destWidth + spacing, topOffset - spacing),              // TopRight
                        new PointF(leftOffset + destWidth + spacing, topOffset + destHeight + spacing), // BottomRight
                        new PointF(leftOffset - spacing, topOffset + destHeight + spacing),             // BottomLeft
						});
					}

					g.DrawImage(imgToResize, leftOffset, topOffset, destWidth, destHeight);
					////g.Dispose();
				}

				return b;
			}
		}

		public static Bitmap resizeJumbo(Bitmap imgToResize, System.Drawing.Size size, int spacing)
		{
			lock (imgToResize)
			{
				if (imgToResize.Width == size.Width && imgToResize.Height == size.Height && spacing == 0)
					return imgToResize;

				if (imgToResize == null) return null;

				int sourceWidth = imgToResize.Width;
				int sourceHeight = imgToResize.Height;

				float nPercent = 0;
				float nPercentW = 0;
				float nPercentH = 0;

				nPercentW = ((float)size.Width / (float)sourceWidth);
				nPercentH = ((float)size.Height / (float)sourceHeight);

				if (nPercentH < nPercentW)
					nPercent = nPercentH;
				else
					nPercent = nPercentW;

				int destWidth = 80;
				int destHeight = 80;

				int leftOffset = (int)((size.Width - destWidth) / 2);
				int topOffset = (int)((size.Height - destHeight) / 2);

				Bitmap b = new Bitmap(size.Width, size.Height);
				using (Graphics g = Graphics.FromImage((Image)b))
				{
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
					g.DrawLines(System.Drawing.Pens.Silver, new PointF[]
					{
                        new PointF(0 + spacing, size.Height - spacing),    // BottomLeft
                        new PointF(0 + spacing, 0 + spacing),              // TopLeft
                        new PointF(size.Width - spacing, 0 + spacing)      // TopRight
					});

					g.DrawLines(System.Drawing.Pens.Gray, new PointF[]
					{
                        new PointF(size.Width - spacing, 0 + spacing),           // TopRight
                        new PointF(size.Width - spacing, size.Height - spacing), // BottomRight
                        new PointF(0 + spacing, size.Height - spacing)           // BottomLeft
					});        

					g.DrawImage(imgToResize, leftOffset, topOffset, destWidth, destHeight);
					////   g.Dispose();
				}

				return b;
			}
		}

		#region Win32API
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		internal static extern bool DeleteObject(IntPtr hObject);
		#endregion Win32API
		#endregion
	}
}
