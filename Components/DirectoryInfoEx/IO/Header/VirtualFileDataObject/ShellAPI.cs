using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics.CodeAnalysis;

namespace ShellDll
{
    public static partial class ShellAPI
    {
        //public const int DRAGDROP_S_DROP = 0x00040100;
        //public const int DRAGDROP_S_CANCEL = 0x00040101;
        //public const int DRAGDROP_S_USEDEFAULTCURSORS = 0x00040102;
        public const int DV_E_DVASPECT = -2147221397;
        public const int DV_E_FORMATETC = -2147221404;
        public const int DV_E_TYMED = -2147221399;
        public const int E_FAIL = -2147467259;
        public const uint FD_CREATETIME = 0x00000008;
        public const uint FD_WRITESTIME = 0x00000020;
        public const uint FD_FILESIZE = 0x00000040;
        public const int OLE_E_ADVISENOTSUPPORTED = -2147221501;
        //public const int S_OK = 0;
        //public const int S_FALSE = 1;
        public const int VARIANT_FALSE = 0;
        public const int VARIANT_TRUE = -1;

        public const string CFSTR_FILECONTENTS = "FileContents";
        public const string CFSTR_FILEDESCRIPTORW = "FileGroupDescriptorW";
        public const string CFSTR_PASTESUCCEEDED = "Paste Succeeded";
        public const string CFSTR_PERFORMEDDROPEFFECT = "Performed DropEffect";
        public const string CFSTR_PREFERREDDROPEFFECT = "Preferred DropEffect";

        [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Structure exists for interop.")]
        [StructLayout(LayoutKind.Sequential)]
        public struct FILEGROUPDESCRIPTOR
        {
            public UInt32 cItems;
            // Followed by 0 or more FILEDESCRIPTORs
        }

        [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Structure exists for interop.")]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FILEDESCRIPTOR
        {
            public UInt32 dwFlags;
            public Guid clsid;
            public Int32 sizelcx;
            public Int32 sizelcy;
            public Int32 pointlx;
            public Int32 pointly;
            public UInt32 dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public UInt32 nFileSizeHigh;
            public UInt32 nFileSizeLow;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
        }

        //[ComImport]
        //[Guid("00000121-0000-0000-C000-000000000046")]
        //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        //public interface IDropSource
        //{
        //    [PreserveSig]
        //    int QueryContinueDrag(int fEscapePressed, uint grfKeyState);
        //    [PreserveSig]
        //    int GiveFeedback(uint dwEffect);
        //}

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Win32 API.")]
        [DllImport("shell32.dll")]
        public static extern int SHCreateStdEnumFmtEtc(uint cfmt, FORMATETC[] afmt, out IEnumFORMATETC ppenumFormatEtc);

        [return: MarshalAs(UnmanagedType.Interface)]
        [DllImport("ole32.dll", PreserveSig = false)]
        public static extern IStream CreateStreamOnHGlobal(IntPtr hGlobal, [MarshalAs(UnmanagedType.Bool)] bool fDeleteOnRelease);

        [DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true, PreserveSig = false)]
        public static extern void DoDragDrop(System.Runtime.InteropServices.ComTypes.IDataObject dataObject, IDropSource dropSource, int allowedEffects, int[] finalEffect);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalSize(IntPtr handle);

        /// <summary>
        /// Returns true iff the HRESULT is a success code.
        /// </summary>
        /// <param name="hr">HRESULT to check.</param>
        /// <returns>True iff a success code.</returns>
        public static bool SUCCEEDED(int hr)
        {
            return (0 <= hr);
        }
    }
}
