namespace DirectoryInfoExLib.IO.Header.PreviewHandlerWPF.Interface
{
    using DirectoryInfoExLib.IO.Header.ShellDll;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public UInt32 message;
        public IntPtr wParam;
        public IntPtr lParam;
        public UInt32 time;
        public ShellAPI.POINT pt;
    }
}
