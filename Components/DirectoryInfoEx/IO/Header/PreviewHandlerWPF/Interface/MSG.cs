using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ShellDll;

namespace PreviewHandlerWPF
{
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
