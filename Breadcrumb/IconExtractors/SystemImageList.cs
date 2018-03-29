namespace BreadCrumbLib.IconExtractors
{
	using System;
	using System.Drawing;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Threading;
	using Breadcrumb.Defines;

	/// <summary>
	/// Created By LYCJ (2014), released under MIT license
	/// I did some tidy up Based on http://vbaccelerator.com/home/net/code/libraries/Shell_Projects/SysImageList/article.asp
	/// </summary>
	public class SystemImageList : IDisposable
	{
		#region fields
		private const int MAX_PATH = 260;

		private const int FILE_ATTRIBUTE_NORMAL = 0x80;
		private const int FILE_ATTRIBUTE_DIRECTORY = 0x10;

		private const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100;
		private const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x2000;
		private const int FORMAT_MESSAGE_FROM_HMODULE = 0x800;
		private const int FORMAT_MESSAGE_FROM_STRING = 0x400;
		private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
		private const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;
		private const int FORMAT_MESSAGE_MAX_WIDTH_MASK = 0xFF;

		private IntPtr _ptrImageList = IntPtr.Zero;
		private IImageList _iImageList = null;
		private bool _disposed = false;
		private IconSize _size;
		#endregion fields

		#region constructors
		public SystemImageList(IconSize size)
		{
			if (!this.isXpOrAbove())
				throw new NotSupportedException("Windows XP or above required.");

			// There is no thumbnail mode in shell.
			this._size = size == IconSize.thumbnail ? IconSize.jumbo : size;

			// XP do not have extra large or jumbo.
			if (!isVistaUp() && (this._size == IconSize.jumbo || this._size == IconSize.extraLarge))
				this._size = IconSize.large;

			Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");

			// Get the System IImageList object from the Shell:
			int hr = SHGetImageList((int)this._size, ref iidImageList, ref this._iImageList);

			if (hr != 0)
				Marshal.ThrowExceptionForHR(hr);

			// the image list handle is the IUnknown pointer, but using Marshal.GetIUnknownForObject doesn't return
			// the right value.  It really doesn't hurt to make a second call to get the handle:            
			SHGetImageListHandle((int)this._size, ref iidImageList, ref this._ptrImageList);

			////int cx = 0, cy = 0;
			////ImageList_GetIconSize(_ptrImageList, ref cx, ref cy);
			////Debug.WriteLine(cx);

			////_iImageList.SetImageCount(2);
		}

		~SystemImageList()
		{
			this.Dispose(false);
		}
		#endregion constructors

		#region Private Enumerations
		[Flags]
		private enum SHGetFileInfoConstants : int
		{
			SHGFI_ICON = 0x100,                // get icon 
			SHGFI_DISPLAYNAME = 0x200,         // get display name 
			SHGFI_TYPENAME = 0x400,            // get type name 
			SHGFI_ATTRIBUTES = 0x800,          // get attributes 
			SHGFI_ICONLOCATION = 0x1000,       // get icon location 
			SHGFI_EXETYPE = 0x2000,            // return exe type 
			SHGFI_SYSICONINDEX = 0x4000,       // get system icon index             
			SHGFI_LINKOVERLAY = 0x8000,        // put a link overlay on icon 
			SHGFI_SELECTED = 0x10000,          // show icon in selected state 
			SHGFI_ATTR_SPECIFIED = 0x20000,    // get only specified attributes 
			SHGFI_LARGEICON = 0x0,             // get large icon 
			SHGFI_SMALLICON = 0x1,             // get small icon 
			SHGFI_OPENICON = 0x2,              // get open icon 
			SHGFI_SHELLICONSIZE = 0x4,         // get shell size icon 
			SHGFI_PIDL = 0x8,                  // pszPath is a pidl 
			SHGFI_USEFILEATTRIBUTES = 0x10,     // use passed dwFileAttribute 
			SHGFI_ADDOVERLAYS = 0x000000020,     // apply the appropriate overlays
			SHGFI_OVERLAYINDEX = 0x000000040     // Get the index of the overlay
		}
		#endregion

		#region Private ImageList COM Interop (XP)
		[ComImportAttribute]
		[GuidAttribute("46EB5926-582E-4017-9FDF-E8998DAA0950")]
		[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
		////helpstring("Image List"),
		private interface IImageList
		{
			[PreserveSig]
			int Add(IntPtr hbmImage, IntPtr hbmMask, ref int pi);

			[PreserveSig]
			int ReplaceIcon(int i, IntPtr hicon, ref int pi);

			[PreserveSig]
			int SetOverlayImage(int iImage, int iOverlay);

			[PreserveSig]
			int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);

			[PreserveSig]
			int AddMasked(IntPtr hbmImage, int crMask, ref int pi);

			[PreserveSig]
			int Draw(ref IMAGELISTDRAWPARAMS pimldp);

			[PreserveSig]
			int Remove(int i);

			[PreserveSig]
			int GetIcon(int i, int flags, ref IntPtr picon);

			[PreserveSig]
			int GetImageInfo(int i, ref IMAGEINFO pImageInfo);

			[PreserveSig]
			int Copy(int iDst, IImageList punkSrc, int iSrc, int uFlags);

			[PreserveSig]
			int Merge(int i1, IImageList punk2, int i2, int dx, int dy, ref Guid riid, ref IntPtr ppv);

			[PreserveSig]
			int Clone(ref Guid riid, ref IntPtr ppv);

			[PreserveSig]
			int GetImageRect(int i, ref RECT prc);

			[PreserveSig]
			int GetIconSize(ref int cx, ref int cy);

			[PreserveSig]
			int SetIconSize(int cx, int cy);

			[PreserveSig]
			int GetImageCount(ref int pi);

			[PreserveSig]
			int SetImageCount(int uNewCount);

			[PreserveSig]
			int SetBkColor(int clrBk, ref int pclr);

			[PreserveSig]
			int GetBkColor(ref int pclr);

			[PreserveSig]
			int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);

			[PreserveSig]
			int EndDrag();

			[PreserveSig]
			int DragEnter(IntPtr hwndLock, int x, int y);

			[PreserveSig]
			int DragLeave(IntPtr hwndLock);

			[PreserveSig]
			int DragMove(int x, int y);

			[PreserveSig]
			int SetDragCursorImage(ref IImageList punk, int iDrag, int dxHotspot, int dyHotspot);

			[PreserveSig]
			int DragShowNolock(int fShow);

			[PreserveSig]
			int GetDragImage(ref POINT ppt, ref POINT pptHotspot, ref Guid riid, ref IntPtr ppv);

			[PreserveSig]
			int GetItemFlags(int i, ref int dwFlags);

			[PreserveSig]
			int GetOverlayImage(int iOverlay, ref int piIndex);
		}
		#endregion

		#region indexers
		public Bitmap this[string fileName, bool isDirectory, bool forceLoadFromDisk, ShellIconStateConstants iconState]
		{
			get
			{
				try
				{
					Icon icon = this.getIcon(this.getIconIndex(fileName, isDirectory, forceLoadFromDisk, iconState));

					return icon == null ? new Bitmap(1, 1) : icon.ToBitmap();
				}
				catch
				{
					return new Bitmap(1, 1);
				}
			}
		}

		public Bitmap this[IntPtr pidlPtr, bool isDirectory, bool forceLoadFromDisk, ShellIconStateConstants iconState]
		{
			get
			{
				try
				{
					Icon icon = this.getIcon(this.getIconIndex(pidlPtr, isDirectory, forceLoadFromDisk, iconState));

					return icon == null ? new Bitmap(1, 1) : icon.ToBitmap();
				}
				catch
				{
					return new Bitmap(1, 1);
				}
			}
		}

		public Bitmap this[string fileName, bool isDirectory, bool forceLoadFromDisk]
		{
			get { return this[fileName, isDirectory, forceLoadFromDisk, ShellIconStateConstants.ShellIconStateNormal]; }
		}

		public Bitmap this[IntPtr pidlPtr, bool isDirectory, bool forceLoadFromDisk]
		{
			get { return this[pidlPtr, isDirectory, forceLoadFromDisk, ShellIconStateConstants.ShellIconStateNormal]; }
		}

		public Bitmap this[string fileName, bool isDirectory]
		{
			get { return this[fileName, isDirectory, false, ShellIconStateConstants.ShellIconStateNormal]; }
		}

		public Bitmap this[IntPtr pidlPtr, bool isDirectory]
		{
			get { return this[pidlPtr, isDirectory, false, ShellIconStateConstants.ShellIconStateNormal]; }
		}
		#endregion indexers

		#region Win32API
		#region UnmanagedCode
		public static IntPtr Test()
		{
			SHFILEINFO shfi = new SHFILEINFO();
			uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());
			return SHGetFileInfo(@"C:\", 16, ref shfi, shfiSize, 16384);
		}
		#endregion
		#endregion

		#region Methods
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			////logger.Debug(String.Format("Dispose, disposing : {0}", disposing));

			if (!this._disposed)
			{
				if (disposing)
				{
					if (this._iImageList != null)
						Marshal.ReleaseComObject(this._iImageList);

					this._iImageList = null;
				}
			}

			this._disposed = true;
		}

		private static IImageList getImageListInterface(IconSize size)
		{
			IImageList iImageList = null;
			Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
			int hr = SHGetImageList((int)size, ref iidImageList, ref iImageList); // Get the System IImageList object from the Shell:
			if (hr != 0)
				Marshal.ThrowExceptionForHR(hr);
			return iImageList;
		}

		private static bool isVistaUp()
		{
			return (Environment.OSVersion.Version.Major >= 6);
		}

		#region UnmanagedCode
		[DllImport("shell32.dll")]
		private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
				ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

		////[DllImport("shell32", CharSet = CharSet.Unicode)]
		////private static extern IntPtr SHGetFileInfo(
		////    string pszPath,
		////    uint dwFileAttributes,
		////    ref SHFILEINFO psfi,
		////    uint cbFileInfo,
		////    uint uFlags);

		[DllImport("shell32.dll")]
		private static extern IntPtr SHGetFileInfo(IntPtr pszPath, uint dwFileAttributes,
				ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

		////[DllImport("shell32", CharSet = CharSet.Unicode)]
		////private static extern IntPtr SHGetFileInfo(
		////    IntPtr pszPath,
		////    uint dwFileAttributes,
		////    ref SHFILEINFO psfi,
		////    uint cbFileInfo,
		////    uint uFlags);

		[DllImport("user32.dll")]
		private static extern int DestroyIcon(IntPtr hIcon);

		[DllImport("kernel32")]
		private static extern int FormatMessage(
				int dwFlags,
				IntPtr lpSource,
				int dwMessageId,
				int dwLanguageId,
				string lpBuffer,
				uint nSize,
				int argumentsLong);

		[DllImport("kernel32")]
		private static extern int GetLastError();

		[DllImport("comctl32")]
		private static extern int ImageList_Draw(
				IntPtr hIml,
				int i,
				IntPtr hdcDst,
				int x,
				int y,
				int fStyle);

		[DllImport("comctl32")]
		private static extern int ImageList_DrawIndirect(
				ref IMAGELISTDRAWPARAMS pimldp);

		[DllImport("comctl32")]
		private static extern int ImageList_GetIconSize(
				IntPtr himl,
				ref int cx,
				ref int cy);

		[DllImport("comctl32")]
		private static extern int ImageList_GetImageInfo(
				IntPtr himl,
				int i,
				ref IMAGEINFO pImageInfo);

		[DllImport("comctl32")]
		private static extern IntPtr ImageList_GetIcon(
				IntPtr himl,
				int i,
				int flags);

		/// <summary>
		/// SHGetImageList is not exported correctly in XP.  See KB316931
		/// http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316931
		/// Apparently (and hopefully) ordinal 727 isn't going to change.
		/// </summary>
		[DllImport("shell32.dll", EntryPoint = "#727")]
		private static extern int SHGetImageList(
				int iImageList,
				ref Guid riid,
				ref IImageList ppv);

		[DllImport("shell32.dll", EntryPoint = "#727")]
		private static extern int SHGetImageListHandle(
				int iImageList,
				ref Guid riid,
				ref IntPtr handle);
		#endregion

		private void getAttributes(bool isDirectory, bool forceLoadFromDisk, out uint dwAttr, out SHGetFileInfoConstants dwFlags)
		{
			dwFlags = SHGetFileInfoConstants.SHGFI_SYSICONINDEX;
			dwAttr = 0;

			if (this._size == IconSize.small)
				dwFlags |= SHGetFileInfoConstants.SHGFI_SMALLICON;

			if (isDirectory)
			{
				dwAttr = FILE_ATTRIBUTE_DIRECTORY;
			}
			else
				if (!forceLoadFromDisk)
				{
					dwFlags |= SHGetFileInfoConstants.SHGFI_USEFILEATTRIBUTES;
					dwAttr = FILE_ATTRIBUTE_NORMAL;
				}
		}

		private int getIconIndex(string fileName, bool isDirectory, bool forceLoadFromDisk, ShellIconStateConstants iconState)
		{
			SHGetFileInfoConstants dwFlags;
			uint dwAttr;
			
			this.getAttributes(isDirectory, forceLoadFromDisk, out dwAttr, out dwFlags);

			// sFileSpec can be any file.
			if (fileName.EndsWith(".lnk", StringComparison.InvariantCultureIgnoreCase))
			{
				dwFlags |= SHGetFileInfoConstants.SHGFI_LINKOVERLAY | SHGetFileInfoConstants.SHGFI_ICON;
				iconState = ShellIconStateConstants.ShellIconStateLinkOverlay;
				forceLoadFromDisk = true;
			}

			SHFILEINFO shfi = new SHFILEINFO();
			uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());
			IntPtr retVal = SHGetFileInfo(fileName, dwAttr, ref shfi, shfiSize, ((uint)(dwFlags) | (uint)iconState));

			if (retVal.Equals(IntPtr.Zero))
			{
				if (forceLoadFromDisk)
					return this.getIconIndex(Path.GetFileName(fileName), isDirectory, false, iconState);
				else
				{
					System.Diagnostics.Debug.Assert((!retVal.Equals(IntPtr.Zero)), "Failed to get icon index");

					return -1;
				}
			}
			else
				return shfi.IIcon;
		}

		private int getIconIndex(IntPtr pidlPtr, bool isDirectory, bool forceLoadFromDisk, ShellIconStateConstants iconState)
		{
			SHGetFileInfoConstants dwFlags;
			uint dwAttr;
			
			this.getAttributes(isDirectory, forceLoadFromDisk, out dwAttr, out dwFlags);
			dwFlags |= SHGetFileInfoConstants.SHGFI_PIDL;

			SHFILEINFO shfi = new SHFILEINFO();
			uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());
			IntPtr retVal = SHGetFileInfo(pidlPtr, dwAttr, ref shfi, shfiSize, ((uint)(dwFlags) | (uint)iconState));

			if (retVal.Equals(IntPtr.Zero))
			{
				System.Diagnostics.Debug.Assert((!retVal.Equals(IntPtr.Zero)), "Failed to get icon index");
				return -1;
			}
			else
				return shfi.IIcon;
		}

		private Icon getIcon(int index)
		{
			if (this == null) return null;
			if (index == -1) return null;

			Icon icon = null;

			IntPtr hIcon = IntPtr.Zero;

			////if (_iImageList == null || Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
			////{
			hIcon = ImageList_GetIcon(this._ptrImageList, index,
					                     (int)(ImageListDrawItemConstants.ILD_TRANSPARENT | ImageListDrawItemConstants.ILD_SCALE));
			////}
			////else
			////{
			////InvalidCastException if run through this.
			////    _iImageList.GetIcon(index, (int)ImageListDrawItemConstants.ILD_TRANSPARENT, ref hIcon);
			////}

			if (hIcon != IntPtr.Zero)
			{
				icon = System.Drawing.Icon.FromHandle(hIcon);
			}
			
			return icon != null ? icon.Clone() as Icon : null;
		}

		////static int CLR_NONE    = (int)0xffffffff;
		////static int CLR_INVALID = CLR_NONE;
		////static int CLR_DEFAULT = (int)0xff000000;

		private Bitmap getBitmap(int index, ImageListDrawItemConstants flags)
		{
			Size bitmapSize = this.GetImageListIconSize();

			Bitmap bitmap = new Bitmap(bitmapSize.Width, bitmapSize.Height);

			using (Graphics g = Graphics.FromImage(bitmap))
			{
				try
				{
					g.FillRectangle(Brushes.White, new Rectangle(0, 0, bitmapSize.Width, bitmapSize.Height));

					IntPtr hdc = g.GetHdc();

					IMAGELISTDRAWPARAMS pimldp = new IMAGELISTDRAWPARAMS();
					pimldp.HdcDst = hdc;
					pimldp.CbSize = Marshal.SizeOf(pimldp.GetType());
					pimldp.I = index;
					pimldp.X = 0;
					pimldp.Y = 0;
					pimldp.CX = bitmapSize.Width;
					pimldp.CY = bitmapSize.Height;
					////pimldp.rgbBk = Color.Silver.ToArgb();
					////pimldp.rgbFg = Color.Silver.ToArgb();
					////pimldp.crEffect = Color.White.ToArgb();
					////pimldp.Frame = 255;
					////pimldp.fState = 0x00000008;                    
					////pimldp.dwRop = (int)(dwRop.BLACKNESS);
					pimldp.FStyle = (int)flags;

					if (this._iImageList == null || Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
					{
						int ret = ImageList_DrawIndirect(ref pimldp);
					}
					else
					{
						this._iImageList.Draw(ref pimldp);
					}
				}
				finally
				{
					g.ReleaseHdc();
				}
			}

			bitmap.MakeTransparent();

			return bitmap;
		}

		private Bitmap getBitmap(int index)
		{
			////Bitmap mask = getBitmap(index, ImageListDrawItemConstants.ILD_MASK);
			Bitmap normal = this.getBitmap(index, ImageListDrawItemConstants.ILD_TRANSPARENT
					                           | ImageListDrawItemConstants.ILD_IMAGE | ImageListDrawItemConstants.ILD_SCALE);

			////Size bitmapSize = GetImageListIconSize();
			////Bitmap bitmap = new Bitmap(bitmapSize.Width, bitmapSize.Height);

			return normal;
			////string output;
			////return MaskImagePtr(normal, mask, out output);
		}

		private bool isXpOrAbove()
		{
			bool ret = false;
			if (Environment.OSVersion.Version.Major > 5)
			{
				ret = true;
			}
			else if ((Environment.OSVersion.Version.Major == 5) &&
					(Environment.OSVersion.Version.Minor >= 1))
			{
				ret = true;
			}
			return ret;
			////return false;
		}

		private Size GetImageIconSize(int index)
		{
			IMAGEINFO imgInfo = new IMAGEINFO();
			int hr = 0;

			if (this._iImageList == null || Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
				hr = ImageList_GetImageInfo(this._ptrImageList, index, ref imgInfo);
			else
				hr = this._iImageList.GetImageInfo(index, ref imgInfo);

			if (hr != 0)
				Marshal.ThrowExceptionForHR(hr);

			RECT rect = imgInfo.RcImage;

			return new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
		}

		private Size GetImageListIconSize()
		{
			int cx = 0, cy = 0;

			if (this._iImageList == null || Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
				ImageList_GetIconSize(this._ptrImageList, ref cx, ref cy);
			else
				this._iImageList.GetIconSize(ref cx, ref cy);

			return new Size(cx, cy);
		}
		#endregion

		#region Private ImageList structures
		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct POINT
		{
			private int x;
			private int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct IMAGELISTDRAWPARAMS
		{
			public int CbSize;
			public IntPtr Himl;
			public int I;
			public IntPtr HdcDst;
			public int X;
			public int Y;
			public int CX;
			public int CY;
			public int XBitmap;        // x offest from the upperleft of bitmap
			public int YBitmap;        // y offset from the upperleft of bitmap
			public int RgbBk;
			public int RgbFg;
			public int FStyle;
			public int DwRop;
			public int FState;
			public int Frame;
			public int CrEffect;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct IMAGEINFO
		{
			public IntPtr HbmImage;
			public IntPtr HbmMask;
			public int Unused1;
			public int Unused2;
			public RECT RcImage;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SHFILEINFO
		{
			public IntPtr HIcon;
			public int IIcon;
			public int DwAttributes;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
			public string SzDisplayName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string SzTypeName;
		}
		#endregion
	}
}
