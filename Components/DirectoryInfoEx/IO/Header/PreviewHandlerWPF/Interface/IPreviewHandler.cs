using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ShellDll;

namespace PreviewHandlerWPF
{

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("8895b1c6-b41f-4c1c-a562-0d564250836f")]
    public interface IPreviewHandler
    {
        void SetWindow(IntPtr hwnd, ref ShellAPI.RECT rect);
        void SetRect(ref ShellAPI.RECT rect);
        void DoPreview();
        void Unload();
        void SetFocus();
        void QueryFocus(out IntPtr phwnd);
        [PreserveSig]
        uint TranslateAccelerator(ref MSG pmsg);
    }



}
