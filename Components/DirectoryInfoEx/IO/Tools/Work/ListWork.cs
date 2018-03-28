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
    public class ListWork : ExWorkBase
    {
        public ListWork(int id, DirectoryInfoEx[] dirs, bool listDir, bool listFile, string fileMask)
            : base(id, dirs.Length > 0 ? dirs[0] : null, null)
        {
            init(dirs);
            _listDir = listDir;
            _listFile = listFile;
            _fileMask = fileMask;
        }

        public ListWork(int id, DirectoryInfoEx[] dirs)
            : base(id, dirs.Length > 0 ? dirs[0] : null, null)
        {
            init(dirs);
        }

        public ListWork(int id, DirectoryInfoEx dir)
            : base(id, dir, null)
        {
            init(new DirectoryInfoEx[] { dir });            
        }

        void init(DirectoryInfoEx[] dirs)
        {
            _dirsToList = dirs;
            WorkType = WorkType.List;
        }

        private void ListDir(DirectoryInfoEx dir)
        {
            foreach (FileInfoEx fi in dir.GetFiles())
                if (_listFile && (String.IsNullOrEmpty(_fileMask) || IOTools.MatchFileMask(fi.Name, _fileMask)))
                    ReportWorkList(fi);

            DirectoryInfoEx[] dirs = dir.GetDirectories();
            AddTotalCount(dirs.Length);
            foreach (DirectoryInfoEx di in dirs)
            {
                if (_listDir)
                    ReportWorkList(di);
                ListDir(di);
                AddCompletedCount(di.FullName);
            }

            dirs = null;
        }


        protected override void DoWork()
        {
            ReportStart();
            CheckPause();

            AddTotalCount(_dirsToList.Length);
            foreach (DirectoryInfoEx dir in _dirsToList)
            {
                ListDir(dir);
                AddCompletedCount(dir.FullName);
            }
            ReportCompleted();
        }


        #region Data

        DirectoryInfoEx[] _dirsToList;
        bool _listDir = true, _listFile = true;
        string _fileMask = "";

        #endregion

    }
}
