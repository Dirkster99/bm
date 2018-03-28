using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ShellDll;

namespace PreviewHandlerWPF
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("b7d14566-0509-4cce-a71f-0a554233bd9b")]
    public interface IInitializeWithFile
    {
        void Initialize([MarshalAs(UnmanagedType.LPWStr)] string pszFilePath, uint grfMode);
    }

}
