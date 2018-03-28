///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ShellDll;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace System.IO.Tools
{
    public class ImageExtractor
    {
        private static bool isVistaUp = Environment.OSVersion.Version.Major >= 6; //5 = XP, 6 = Vista

        //http://msdn.microsoft.com/en-us/library/bb761848(VS.85).aspx

        public static bool GetIExtractImage(
           IShellFolder2 parent,
            IntPtr filePidl,
           out IntPtr iExtractImagePtr,
           out IExtractImage iExtractImage)
        {
            if (!isVistaUp)
                throw new NotSupportedException("Require Vista or above.");

            if (parent.GetUIObjectOf(
                        IntPtr.Zero,
                        1,
                        new IntPtr[] { filePidl },
                        ref ShellAPI.IID_IExtractImage,
                        IntPtr.Zero,
                        out iExtractImagePtr) == ShellAPI.S_OK)
            {
                iExtractImage =
                    (IExtractImage)Marshal.GetTypedObjectForIUnknown(
                        iExtractImagePtr, typeof(IExtractImage));

                return true;
            }
            else
            {
                iExtractImagePtr = IntPtr.Zero;
                iExtractImage = null;

                return false;
            }
        }

        public static bool GetIExtractImage(
           FileInfoEx file,
           out IntPtr iExtractImagePtr,
           out IExtractImage iExtractImage)
        {
            using (ShellFolder2 sf2 = file.Parent.ShellFolder)
            {
                PIDL filePIDL = file.getRelPIDL();
                try
                {
                    return GetIExtractImage(sf2, filePIDL.Ptr, out iExtractImagePtr, out iExtractImage);
                }
                finally { filePIDL.Free(); }
            }
        }

        public static Bitmap ExtractImage(string fileName, Size size, bool quality)
        {
            return ExtractImage(new FileInfoEx(fileName), size, quality);

        }

        public static Bitmap ExtractImage(FileInfoEx entry, Size size, bool quality)
        {

            try
            {
                IntPtr iExtractImagePtr = IntPtr.Zero;
                IExtractImage iExtractImage = null;


                if (GetIExtractImage(entry, out iExtractImagePtr, out iExtractImage))
                    try
                    {
                        ShellDll.ShellAPI.SIZE prgSize = new ShellAPI.SIZE() { cx = size.Width, cy = size.Height };
                        IExtractImageFlags flags = IExtractImageFlags.Cache | IExtractImageFlags.Aspect;
                        if (quality) flags |= IExtractImageFlags.Quality;
                        StringBuilder location = new StringBuilder(260, 260);

                        try
                        {
                            int pdwPriority = 1;
                            iExtractImage.GetLocation(location,
                                location.Capacity, ref pdwPriority, ref prgSize, 32, ref flags);
                        }
                        catch (COMException) { }

                        IntPtr ptrBitmapImage = IntPtr.Zero;
                        iExtractImage.Extract(out ptrBitmapImage);
                        if (ptrBitmapImage != IntPtr.Zero)
                            return Bitmap.FromHbitmap(ptrBitmapImage);
                    }
                    catch (Exception) { }
                    finally
                    {
                        if (iExtractImage != null)
                            Marshal.ReleaseComObject(iExtractImage);

                        if (iExtractImagePtr != IntPtr.Zero)
                            Marshal.Release(iExtractImagePtr);
                    }

                return null;
            }
            catch (NotSupportedException) { return null; }
        }

    }
}
