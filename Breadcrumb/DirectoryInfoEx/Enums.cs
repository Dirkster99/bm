namespace Breadcrumb.Defines
{
	using System;

	public enum IconSize : int
	{
		small = 0x1, large = 0x0, extraLarge = 0x2, jumbo = 0x4, thumbnail = 0x5
	}

	#region Public Enumerations
/***
	/// <summary>
	/// Available system image list sizes
	/// </summary>
	public enum SysImageListSize : int
	{
		/// <summary>
		/// System Large Icon Size (typically 32x32)
		/// </summary>
		largeIcons = 0x0,

		/// <summary>
		/// System Small Icon Size (typically 16x16)
		/// </summary>
		smallIcons = 0x1,

		/// <summary>
		/// System Extra Large Icon Size (typically 48x48).
		/// Only available under XP; under other OS the
		/// Large Icon ImageList is returned.
		/// </summary>
		extraLargeIcons = 0x2,

		jumbo = 0x4
	}
***/

	/// <summary>
	/// Flags controlling how the Image List item is 
	/// drawn
	/// </summary>
	[Flags]
	public enum ImageListDrawItemConstants : int
	{
		/// <summary>
		/// Draw item normally.
		/// </summary>
		ILD_NORMAL = 0x0,

		/// <summary>
		/// Draw item transparently.
		/// </summary>
		ILD_TRANSPARENT = 0x1,

		/// <summary>
		/// Draw item blended with 25% of the specified foreground colour
		/// or the Highlight colour if no foreground colour specified.
		/// </summary>
		ILD_BLEND25 = 0x2,

		/// <summary>
		/// Draw item blended with 50% of the specified foreground colour
		/// or the Highlight colour if no foreground colour specified.
		/// </summary>
		ILD_SELECTED = 0x4,

		/// <summary>
		/// Draw the icon's mask
		/// </summary>
		ILD_MASK = 0x10,

		/// <summary>
		/// Draw the icon image without using the mask
		/// </summary>
		ILD_IMAGE = 0x20,

		/// <summary>
		/// Draw the icon using the ROP specified.
		/// </summary>
		ILD_ROP = 0x40,

		/// <summary>
		/// Preserves the alpha channel in dest. XP only.
		/// </summary>
		ILD_PRESERVEALPHA = 0x1000,

		/// <summary>
		/// Scale the image to cx, cy instead of clipping it.  XP only.
		/// </summary>
		ILD_SCALE = 0x2000,

		/// <summary>
		/// Scale the image to the current DPI of the display. XP only.
		/// </summary>
		ILD_DPISCALE = 0x4000,

		ILD_OVERLAYMASK = 0x00000F00,

		ILD_ASYNC = 0x00008000
	}
/***
	public enum dwRop : uint
	{
		/// <summary>
		///  dest = source
		/// </summary>
		SRCCOPY = 0x00CC0020,

		/// <summary>
		///  dest = source OR dest
		/// </summary>
		SRCPAINT = 0x00EE0086,

		/// <summary>
		///  dest = source AND dest
		/// </summary>
		SRCAND = 0x008800C6,

		/// <summary>
		///  dest = source XOR dest
		/// </summary>
		SRCINVERT = 0x00660046,

		/// <summary>
		///  dest = source AND (NOT dest )
		/// </summary>
		SRCERASE = 0x00440328,

		/// <summary>
		///  dest = (NOT source)
		/// </summary>
		NOTSRCCOPY = 0x00330008,

		/// <summary>
		///  dest = (NOT src) AND (NOT dest)
		/// </summary>
		NOTSRCERASE = 0x001100A6,

		/// <summary>
		///  dest = (source AND pattern)
		/// </summary>
		MERGECOPY = 0x00C000CA

		/// <summary>
		///  dest = (NOT source) OR dest
		/// </summary>
		MERGEPAINT = 0x00BB0226,

		/// <summary>
		///  dest = pattern
		/// </summary>
		PATCOPY = 0x00F00021,

		/// <summary>
		///  dest = DPSnoo
		/// </summary>
		PATPAINT = 0x00FB0A09,

		/// <summary>
		///  dest = pattern XOR dest
		/// </summary>
		PATINVERT = 0x005A0049,

		/// <summary>
		///  dest = (NOT dest)
		/// </summary>
		DSTINVERT = 0x00550009,

		/// <summary>
		///  dest = BLACK
		/// </summary>
		BLACKNESS = 0x00000042,

		/// <summary>
		///  dest = WHITE
		/// </summary>
		WHITENESS = 0x00FF0062,
	}

	/// <summary>
	/// Enumeration containing XP ImageList Draw State options
	/// </summary>
	[Flags]
	public enum ImageListDrawStateConstants : int
	{
		/// <summary>
		/// The image state is not modified. 
		/// </summary>
		ILS_NORMAL = (0x00000000),

		/// <summary>
		/// Adds a glow effect to the icon, which causes the icon to appear to glow 
		/// with a given color around the edges. (Note: does not appear to be
		/// implemented)
		/// </summary>
		ILS_GLOW = (0x00000001), //The color for the glow effect is passed to the IImageList::Draw method in the crEffect member of IMAGELISTDRAWPARAMS. 

		/// <summary>
		/// Adds a drop shadow effect to the icon. (Note: does not appear to be
		/// implemented)
		/// </summary>
		ILS_SHADOW = (0x00000002), //The color for the drop shadow effect is passed to the IImageList::Draw method in the crEffect member of IMAGELISTDRAWPARAMS. 

		/// <summary>
		/// Saturates the icon by increasing each color component 
		/// of the RGB triplet for each pixel in the icon. (Note: only ever appears
		/// to result in a completely unsaturated icon)
		/// </summary>
		ILS_SATURATE = (0x00000004), // The amount to increase is indicated by the frame member in the IMAGELISTDRAWPARAMS method. 

		/// <summary>
		/// Alpha blends the icon. Alpha blending controls the transparency 
		/// level of an icon, according to the value of its alpha channel. 
		/// (Note: does not appear to be implemented).
		/// </summary>
		ILS_ALPHA = (0x00000008) //The value of the alpha channel is indicated by the frame member in the IMAGELISTDRAWPARAMS method. The alpha channel can be from 0 to 255, with 0 being completely transparent, and 255 being completely opaque. 
	}
***/

	/// <summary>
	/// Flags specifying the state of the icon to draw from the Shell
	/// </summary>
	[Flags]
	public enum ShellIconStateConstants
	{
		/// <summary>
		/// Get icon in normal state
		/// </summary>
		ShellIconStateNormal = 0,

		/// <summary>
		/// Put a link overlay on icon 
		/// </summary>
		ShellIconStateLinkOverlay = 0x8000,

		/// <summary>
		/// show icon in selected state 
		/// </summary>
		ShellIconStateSelected = 0x10000,

		/// <summary>
		/// get open icon 
		/// </summary>
		ShellIconStateOpen = 0x2,

		/// <summary>
		/// apply the appropriate overlays
		/// </summary>
		ShellIconAddOverlays = 0x000000020,
	}
	#endregion
}
