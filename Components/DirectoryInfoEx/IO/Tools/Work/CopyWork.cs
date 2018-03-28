///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Tools
{
    public class CopyWork : ExWorkBase
    {
        public CopyWork(int id, FileSystemInfoEx[] src, DirectoryInfoEx dest)
            : base(id,
            src.Length > 0 ? src[0].Parent : null, dest)
        {
            _copyFrom = src;
            _dest = dest;
            WorkType = WorkType.Copy;
        }

        void CopyFile(FileInfoEx item, DirectoryInfoEx destDir)
        {
            FileSystemInfoEx lookupItem = destDir[item.Name];

            if (lookupItem is FileInfoEx)
            {
                if (item.Length == (lookupItem as FileInfoEx).Length)
                {
                    string srcCRC = Helper.GetFileCRC(item.FullName);
                    string destCRC = Helper.GetFileCRC(lookupItem.FullName);
                    if (srcCRC == destCRC && item.Length == (lookupItem as FileInfoEx).Length)
                        return; //Same file, no copy needed.
                }
            }

            OverwriteMode overwrite = OverwriteMode.Replace;
            if (lookupItem != null)
                overwrite = AskOverwrite(item, lookupItem);

            switch (overwrite)
            {
                case OverwriteMode.Replace : 
                    if (lookupItem != null)
                        lookupItem.Delete();
                    FileCancelDelegate cancel = delegate { return Aborted; };
                    IOTools.CopyFile(item.FullName, PathEx.Combine(destDir.FullName, item.Name), cancel);
                break;
            }
        }        

        DirectoryInfoEx PrepateDirectoryForCopy(DirectoryInfoEx srcDir, DirectoryInfoEx baseDir, string dirName)
        {
            bool isDirectory;
            if (!baseDir.Exists)
                baseDir.Create();
            if (baseDir.Contains(dirName, out isDirectory))
            {
                FileSystemInfoEx destSubItem = baseDir[dirName];

                if (!isDirectory)
                {
                    OverwriteMode overwrite = AskOverwrite(srcDir, destSubItem);
                    switch (overwrite)
                    {
                        case OverwriteMode.KeepOriginal :
                            return null;
                        case OverwriteMode.Replace :
                            destSubItem.Delete();
                            return baseDir.CreateDirectory(dirName);
                    }
                    throw new NotImplementedException("OverwriteMode");
                }
                else return destSubItem as DirectoryInfoEx;
            }
            else return baseDir.CreateDirectory(dirName);
        }

        void CopyDirectory(DirectoryInfoEx srcDir, DirectoryInfoEx destDir)
        {
            FileInfoEx[] files = srcDir.GetFiles();
            AddTotalCount(files.Length);
            foreach (FileInfoEx fi in files)
                if (!Aborted)
                {
                    CheckPause();
                    AddCompletedCount(fi.FullName);
                    CopyFile(fi, destDir);
                }
            files = null;

            DirectoryInfoEx[] dirs = srcDir.GetDirectories();
            AddTotalCount(dirs.Length);
            foreach (DirectoryInfoEx di in dirs)
                if (!Aborted)
                {
                    CheckPause();
                    AddCompletedCount(di.FullName);
                    DirectoryInfoEx destSubDir = PrepateDirectoryForCopy(di, destDir, di.Name);
                    if (destSubDir != null)
                        CopyDirectory(di, destSubDir);
                }

            dirs = null;
        }


        protected override void DoWork()
        {
            ReportStart();
            CheckPause();

            AddTotalCount(_copyFrom.Length);
            foreach (FileSystemInfoEx item in _copyFrom)
                if (!Aborted)
                {
                    CheckPause();
                    AddCompletedCount(item.FullName);
                    if (item is FileInfoEx)
                        CopyFile(item as FileInfoEx, _dest);
                    else
                    {
                        DirectoryInfoEx dest = PrepateDirectoryForCopy(item as DirectoryInfoEx, _dest, item.Name);
                        if (dest != null)
                            CopyDirectory(item as DirectoryInfoEx, dest);
                    }
                }
            ReportCompleted();
        }


        #region Data

        FileSystemInfoEx[] _copyFrom;
        DirectoryInfoEx _dest;

        #endregion

    }
}
