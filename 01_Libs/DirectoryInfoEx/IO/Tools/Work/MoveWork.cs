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
    public class MoveWork : ExWorkBase
    {
        public MoveWork(int id, FileSystemInfoEx[] src, DirectoryInfoEx dest)
            : base(id,
            src.Length > 0 ? src[0].Parent : null, dest)
        {
            _copyFrom = src;
            _dest = dest;
            WorkType = WorkType.Move;
        }

        void MoveFile(FileInfoEx item, DirectoryInfoEx destDir)
        {
            FileSystemInfoEx lookupItem = destDir[item.Name];

            OverwriteMode overwrite = OverwriteMode.Replace;
            if (lookupItem != null)
                overwrite = AskOverwrite(item, lookupItem);

            switch (overwrite)
            {
                case OverwriteMode.Replace:
                    if (lookupItem != null)
                        lookupItem.Delete();
                    IOTools.Move(item.FullName, PathEx.Combine(destDir.FullName, item.Name));
                    break;
            }
        }


        DirectoryInfoEx PrepateDirectoryForMove(DirectoryInfoEx srcDir, DirectoryInfoEx baseDir, string dirName)
        {
            bool isDirectory;
            if (!baseDir.Exists)
                baseDir.Create();
            if (baseDir.Contains(dirName, out isDirectory))
            {
                FileSystemInfoEx destSubItem = baseDir[dirName];

                OverwriteMode overwrite = AskOverwrite(srcDir, destSubItem);
                switch (overwrite )
                {
                    case OverwriteMode.KeepOriginal : 
                        return null;
                    case OverwriteMode.Replace : 
                        if (!isDirectory)
                        {
                            destSubItem.Delete();
                            return baseDir.CreateDirectory(dirName);
                        }
                        else return destSubItem as DirectoryInfoEx;                        
                }
                throw new NotImplementedException("OverwriteMode");
            }
            else return baseDir.CreateDirectory(dirName);
        }


        void MoveDirectory(DirectoryInfoEx srcDir, DirectoryInfoEx destDir)
        {
            FileInfoEx[] files = srcDir.GetFiles();
            AddTotalCount(files.Length);
            foreach (FileInfoEx fi in files)
                if (!Aborted)
                {
                    CheckPause();
                    AddCompletedCount(fi.FullName);
                    MoveFile(fi, destDir);
                }
            files = null;

            DirectoryInfoEx[] dirs = srcDir.GetDirectories();
            AddTotalCount(dirs.Length);
            foreach (DirectoryInfoEx di in dirs)
                if (!Aborted)
                {
                    CheckPause();
                    AddCompletedCount(di.FullName);
                    DirectoryInfoEx destSubDir = PrepateDirectoryForMove(di, destDir, di.Name);
                    if (destSubDir != null)
                        MoveDirectory(di, destSubDir);
                }
            dirs = null;

            if (srcDir.Exists && srcDir.GetFileSystemInfos().Length == 0)
                srcDir.Delete();
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
                        MoveFile(item as FileInfoEx, _dest);
                    else
                    {
                        DirectoryInfoEx dest = PrepateDirectoryForMove(item as DirectoryInfoEx, _dest, item.Name);
                        if (dest != null)
                            MoveDirectory(item as DirectoryInfoEx, dest);
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
