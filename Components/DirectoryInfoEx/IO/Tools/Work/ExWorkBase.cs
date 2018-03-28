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
    public abstract class ExWorkBase : WorkBase, IExWork
    {
        public ExWorkBase(int id, FileSystemInfoEx source, FileSystemInfoEx target)
            : base(id)
        {
            Source = source;
            Target = target;
        }

        public ExWorkBase(int id, FileSystemInfoEx source)
            : base(id)
        {
            Source = source;
        }

        #region IExWork Members

        public FileSystemInfoEx Source { get; private set; }

        public FileSystemInfoEx Target { get; private set; }

        public event WorkOverwriteEventHandler WorkOverwrite;
        public event WorkListEventHandler WorkList;

        #region Methods

        protected override void attachProgressDialogEvents()
        {
            base.attachProgressDialogEvents();
            ProgressDialog.Source = this.Source.FullName;
            ProgressDialog.Target = this.Target.FullName;

            if (this.Target != null)
            {                
                if (this.Source != null)
                    ProgressDialog.Message = String.Format("from {0} to {1}", this.Source.Name, this.Target.Name);
                else
                    ProgressDialog.Message = String.Format("from {0}", this.Target.Name);             
            }

            ProgressDialog.Header = this.GetType().ToString().Replace("Work", "").Replace("System.IO.Tools.", ""); //CopyWork --> Copy
            ProgressDialog.Title = this.Target != null ? this.Target.Label : this.Source != null ? this.Source.Label : "";
        }

        #region OverwriteTools

        protected OverwriteMode AskOverwrite(FileSystemInfoEx srcEntry, FileSystemInfoEx destEntry)
        {
            if (ApplyToAll)
                return DefaultOverwriteMode;

            if (WorkOverwrite != null)
            {
                WorkOverwriteEventArgs e = new WorkOverwriteEventArgs(ID, srcEntry, destEntry,
                    OverwriteMode.Replace, false);
                WorkOverwrite(this, e);
                if (e.ApplyToAll)
                    SetDefaultOverwriteMode(e.Overwrite);
                return e.Overwrite;
            }
            return OverwriteMode.Replace;            
        }

        #endregion


        #region ListTools

        protected bool ReportWorkList(FileSystemInfoEx itemList)
        {
            if (WorkList != null)
            {
                WorkListEventArgs args = new WorkListEventArgs(ID, itemList);
                WorkList(this, args);
                if (args.Cancel)
                    Abort();
                return !args.Cancel;
            }
            else return true;
        }

        #endregion

        #endregion

        #endregion

        #region Data

        //bool _overwrite = false;
       //bool _applyAll = false;

        #endregion
    }
}
