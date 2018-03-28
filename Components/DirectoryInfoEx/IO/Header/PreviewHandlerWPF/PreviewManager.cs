//Lycj : I have modify it to work with WinForms, then I use it via ElementHost in WPF.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Drawing;
using ShellDll;
using System.IO;

namespace PreviewHandlerWPF
{
    public static class PreviewManager
    {
        public static List<Previewer> CurrentPreviewers;

        public static void GetAllPreviewers()
        {
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\PreviewHandlers"))
            {
                CurrentPreviewers = new List<Previewer>();

                string[] guids = rk.GetValueNames();

                for (int i = 0; i < guids.Length; i++)
                {
                    bool repeated = false;
                    foreach (Previewer curPrv in CurrentPreviewers)
                        if (curPrv.CLSID.ToString() == guids[i])
                            repeated = true;
                    if (repeated)
                        break;

                    Previewer prv = new Previewer();
                    prv.CLSID = new Guid(guids[i]);
                    prv.Title = rk.GetValue(guids[i]).ToString();

                    CurrentPreviewers.Add(prv);
                }

            }
        }

        public static IPreviewHandler pHandler;        

        public static void InvalidateAttachedPreview(IntPtr handler, Rectangle viewRect)
        {
            if (pHandler != null)
            {
                ShellAPI.RECT r = new ShellAPI.RECT(viewRect);
                pHandler.SetRect(ref r);
            }
        }

        public static void Unload()
        {
            if (pHandler != null)
            {
                pHandler.Unload();
                pHandler = null;
            }
        }

        public static void AttachPreview(IntPtr handler, FileInfoEx file, Rectangle viewRect)
        {
            if (pHandler != null)
            {
                pHandler.Unload();
            }
            string fileName = file.FullName;
            string CLSID = "8895b1c6-b41f-4c1c-a562-0d564250836f";
            Guid g = new Guid(CLSID);
            string[] exts = fileName.Split('.');
            string ext = exts[exts.Length - 1];
            using (RegistryKey hk = Registry.ClassesRoot.OpenSubKey(string.Format(@".{0}\ShellEx\{1:B}", ext, g)))
            {
                if (hk != null)
                {
                    g = new Guid(hk.GetValue("").ToString());

                    Type a = Type.GetTypeFromCLSID(g, true);
                    object o = Activator.CreateInstance(a);

                    IInitializeWithFile fileInit = o as IInitializeWithFile;
                    IInitializeWithStream streamInit = o as IInitializeWithStream;

                    bool isInitialized = false;
                    if (fileInit != null)
                    {
                        fileInit.Initialize(fileName, 0);
                        isInitialized = true;
                    }
                    else if (streamInit != null)
                    {
                        FileStreamEx stream = file.OpenRead();
                        //COMStream stream = new COMStream(File.Open(fileName, FileMode.Open));
                        streamInit.Initialize((IStream)streamInit, 0);
                        isInitialized = true;
                    }

                    if (isInitialized)
                    {
                        pHandler = o as IPreviewHandler;
                        if (pHandler != null)
                        {
                            ShellAPI.RECT r = new ShellAPI.RECT(viewRect);

                            pHandler.SetWindow(handler, ref r);
                            pHandler.SetRect(ref r);

                            pHandler.DoPreview();

                        }
                    }

                }
            }
        }
    }

    public class Previewer
    {
        public Guid CLSID { get; set; }
        public string Title { get; set; }
    }
}
