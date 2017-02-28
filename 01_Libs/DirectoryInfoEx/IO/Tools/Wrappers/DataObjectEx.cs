///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Delay's work                                                                           //
//(http://blogs.msdn.com/delay/archive/2009/10/26/creating-something-from-nothing-developer-friendly-virtual-file-implementation-for-net.aspx)        //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices.ComTypes;
using ShellDll;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using Delay;
using System.Windows.Forms;

namespace System.IO.Tools
{
    
    public class DataObjectEx : DataObject
    {
        private FileSystemInfoEx[] _entityList = new FileSystemInfoEx[0];
        //private bool _isExtracted = false;
        public const string DataFormats_EntryString = "QuickZip.PIDL.DirectoryInfoEx.FullName";

        //private static void CreateTemporaryFileName(String fileName)
        //{
        //    if (!File.Exists(fileName))
        //    {
        //        System.IO.StreamWriter file;
        //        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
        //        file = new System.IO.StreamWriter(fileName);
        //        file.Close();
        //    }
        //}

        private bool InDragLoop()
        {
            return (0 != (int)GetData(ShellClipboardFormats.CFSTR_INDRAGLOOP));
        }


        //public override object GetData(String format)
        //{

        //    Object obj = base.GetData(format);

        //    if (format == System.Windows.Forms.DataFormats.FileDrop &&
        //         !InDragLoop() && !_isExtracted)
        //    {
        //        string s;
        //        foreach (FileSystemInfoEx entity in _entityList)
        //            s = entity.FullName;

        //        //if (entity.Parent is IVirtualDirectoryInfoExA)
        //        //{
        //        //    if (entity is IVirtualFileSystemInfoExA)
        //        //    {
        //        //        string outPath = (entity as IVirtualFileSystemInfoExA).TempPath;
        //        //        outPath = entity.FullName;
        //        //    }
        //        //}

        //        _isExtracted = true;
        //    }
        //    return obj;
        //}
        

        public DataObjectEx(FileSystemInfoEx[] entityList)
        {
            _entityList = entityList;
        }

        public string[] PathnameList()
        {
            string[] retVal = new string[_entityList.Length];

            for (int i = 0; i < _entityList.Length; i++)
                retVal[i] = _entityList[i].FullName;
            return retVal;
        }
    }


     public class ShellClipboardFormats
    {
        public const string CFSTR_SHELLIDLIST = "Shell IDList Array";
        public const string CFSTR_SHELLIDLISTOFFSET = "Shell Object Offsets";
        public const string CFSTR_NETRESOURCES = "Net Resource";
        public const string CFSTR_FILEDESCRIPTORA = "FileGroupDescriptor";
        public const string CFSTR_FILEDESCRIPTORW = "FileGroupDescriptorW";
        public const string CFSTR_FILECONTENTS = "FileContents";
        public const string CFSTR_FILENAMEA = "FileName";
        public const string CFSTR_FILENAMEW = "FileNameW";
        public const string CFSTR_PRINTERGROUP = "PrinterFreindlyName";
        public const string CFSTR_FILENAMEMAPA = "FileNameMap";
        public const string CFSTR_FILENAMEMAPW = "FileNameMapW";
        public const string CFSTR_SHELLURL = "UniformResourceLocator";
        public const string CFSTR_INETURLA = CFSTR_SHELLURL;
        public const string CFSTR_INETURLW = "UniformResourceLocatorW";
        public const string CFSTR_PREFERREDDROPEFFECT = "Preferred DropEffect";
        public const string CFSTR_PERFORMEDDROPEFFECT = "Performed DropEffect";
        public const string CFSTR_PASTESUCCEEDED = "Paste Succeeded";
        public const string CFSTR_INDRAGLOOP = "InShellDragLoop";
        public const string CFSTR_DRAGCONTEXT = "DragContext";
        public const string CFSTR_MOUNTEDVOLUME = "MountedVolume";
        public const string CFSTR_PERSISTEDDATAOBJECT = "PersistedDataObject";
        public const string CFSTR_TARGETCLSID = "TargetCLSID";
        public const string CFSTR_LOGICALPERFORMEDDROPEFFECT = "Logical Performed DropEffect";
        public const string CFSTR_AUTOPLAY_SHELLIDLISTS = "Autoplay Enumerated IDList Array";
    }

}
