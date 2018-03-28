///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Tamir Khason's work (http://www.codeproject.com/KB/WPF/wpf_vista_preview_handler.aspx) //
// The new version include support() method, as well as allow multiple previewers in the same app                //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using PreviewHandlerWPF;
using ShellDll;

namespace System.IO.Tools
{

    class PreviewerInfo
    {
        public Guid CLSID;
        public string Title;
        public string ConfirmSupportedExtension = "";
    }

    public static class PreviewHelper
    {
        static List<PreviewerInfo> CurrentPreviewers;
        static Guid Guid_IPreviewHandler = new Guid("{8895b1c6-b41f-4c1c-a562-0d564250836f}");

        static PreviewHelper()
        {
            loadPreviewerList();
        }

        static void loadPreviewerList()
        {
            List<PreviewerInfo> previewerList = new List<PreviewerInfo>();

            using (RegistryKey rk = 
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\PreviewHandlers"))
            {
                if (rk != null)
                {
                    string[] guids = rk.GetValueNames();

                    for (int i = 0; i < guids.Length; i++)
                    {
                        bool repeated = false;
                        foreach (PreviewerInfo curPrv in previewerList)
                            if (curPrv.CLSID.ToString() == guids[i])
                                repeated = true;
                        if (repeated)
                            break;

                        PreviewerInfo prv = new PreviewerInfo();
                        prv.CLSID = new Guid(guids[i]);
                        prv.Title = rk.GetValue(guids[i]).ToString();
                        
                        previewerList.Add(prv);
                    }
                }

            }

            CurrentPreviewers = new List<PreviewerInfo>(previewerList);
        }

        //http://www.geekzilla.co.uk/view8AD536EF-BC0D-427F-9F15-3A1BC663848E.htm
        static bool IsGUID(string expression)
        {
            if (expression != null)
            {
                Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");

                return guidRegEx.IsMatch(expression);
            }
            return false;
        }
        static PreviewerInfo getPreviewer(string filename)
        {
            string ext = PathEx.GetExtension(filename).ToLower();

            foreach (PreviewerInfo prevInfo in CurrentPreviewers)
            {
                if (prevInfo.ConfirmSupportedExtension.IndexOf(ext + ";") != -1)
                    return prevInfo;
            }

            using 
                (RegistryKey hk = 
                    Registry.ClassesRoot.OpenSubKey(string.Format(@"{0}\ShellEx\{1:B}", ext, Guid_IPreviewHandler)))
            {
                if (hk != null)
                {
                    object previewerClsidStr = hk.GetValue("");
                    if (previewerClsidStr is string && IsGUID((string)previewerClsidStr))
                    {
                        Guid previewerClsid = new Guid((string)previewerClsidStr);
                        foreach (PreviewerInfo prevInfo in CurrentPreviewers)
                            if (prevInfo.CLSID.Equals(previewerClsid))
                            {
                                prevInfo.ConfirmSupportedExtension += ext + ";";
                                return prevInfo;
                            }
                    }

                }
            }
            return null;
        }
        public static bool Support(string filename) { return getPreviewer(filename) != null; }
        public static bool Support(FileInfoEx file) { return Support(file.Name); }

        public static IPreviewHandler InitalizePreviewHandler(FileInfoEx file)
        {
            IPreviewHandler retVal = null;
            PreviewerInfo previewer = getPreviewer(file.FullName);
            if (previewer != null)
            {
                bool isInitialized = false;
                Type a = Type.GetTypeFromCLSID(previewer.CLSID, true);
                object o = Activator.CreateInstance(a);

                IInitializeWithFile fileInit = o as IInitializeWithFile;
                IInitializeWithStream streamInit = o as IInitializeWithStream;

                if (fileInit != null)
                {
                    fileInit.Initialize(file.FullName, 0);
                    isInitialized = true;
                }
                else if (streamInit != null)
                {
                    FileStreamEx stream = file.OpenRead();
                    streamInit.Initialize((IStream)stream, 0);
                    isInitialized = true;
                }

                if (isInitialized)
                    retVal = o as IPreviewHandler;
            }
            return retVal;
        }
    }
}
