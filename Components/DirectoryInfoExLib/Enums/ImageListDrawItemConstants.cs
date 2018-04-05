namespace DirectoryInfoExLib.Enums
{
    using System;

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
}
