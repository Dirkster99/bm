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
    public delegate void WorkOverwriteEventHandler(object sender, WorkOverwriteEventArgs e);
    public delegate void WorkListEventHandler(object sender, WorkListEventArgs e);

    #region Event Handler        
    public class WorkOverwriteEventArgs : EventArgs
    {
        public int ID { get; private set; }
        public OverwriteMode Overwrite { get; set; }
        public bool ApplyToAll { get; set; }
        public FileSystemInfoEx SrcEntry { get; private set; }
        public FileSystemInfoEx DestEntry { get; private set; }
        public bool Cancel { get; set; }

        public WorkOverwriteEventArgs(int id, FileSystemInfoEx srcEntry, FileSystemInfoEx destEntry, OverwriteMode defOverwrite, bool defApplyToAll)
        {
            this.ID = id;
            this.SrcEntry = srcEntry;
            this.DestEntry = destEntry;
            this.Overwrite = defOverwrite;
            this.ApplyToAll = defApplyToAll;
        }
    }

    public class WorkListEventArgs : EventArgs
    {
        public int ID { get; private set; }
        public FileSystemInfoEx ListedEntry { get; private set; }

        public bool Cancel { get; set; }

        public WorkListEventArgs(int id, FileSystemInfoEx listedEntry)
        {
            this.ID = id;
            this.ListedEntry = listedEntry;
        }
    }
    #endregion

    public interface IExWork : IWork
    {
        FileSystemInfoEx Source { get; }
        FileSystemInfoEx Target { get; }

        event WorkOverwriteEventHandler WorkOverwrite;
        event WorkListEventHandler WorkList;
    }
}
