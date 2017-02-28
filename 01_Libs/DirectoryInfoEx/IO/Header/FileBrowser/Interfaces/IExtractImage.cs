using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ShellDll
{
    //http://msdn.microsoft.com/en-us/library/bb761848(VS.85).aspx

    [Flags]
    public enum IExtractImageFlags : int
    {
        Async = 0x1,
        Cache = 0x2,
        Aspect = 0x4,
        Offline = 0x8,
        Gleam = 0x10,
        Screen = 0x20,
        OriginalSize = 0x40,
        NoStamp = 0x80,
        NoBorder = 0x100,
        Quality = 0x200
    }

    [ComImportAttribute()]
    [GuidAttribute("BB2E617C-0920-11d1-9A0B-00C04FC2D6C1")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IExtractImage
    {
        void GetLocation(
            [Out, MarshalAs(UnmanagedType.LPWStr)]
        StringBuilder pszPathBuffer,
            int cch,
            ref int pdwPriority,
            ref ShellDll.ShellAPI.SIZE prgSize,
            int dwRecClrDepth,
            ref IExtractImageFlags pdwFlags);

        void Extract(
            out IntPtr phBmpThumbnail);
    }
}
