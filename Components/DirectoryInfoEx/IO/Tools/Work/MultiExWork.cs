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
    public class MultiExWork : ExWorkBase
    {
        public MultiExWork(int id, FileSystemInfoEx source, FileSystemInfoEx target, IEnumerable<IExWork> workList)
            : base(id, source, target)
        {
            _workList = new List<IExWork>(workList);
        }

        #region Methods

        protected void DoWorkSequential()
        {
            ReportStart();
            foreach (IExWork work in _workList)
            {
                WorkSpawner.SetIsMuted(work.ID, true);
                hookWorkEvent(work);

                work.Start(false);

                unhookWorkEvent(work);
                WorkSpawner.SetIsMuted(work.ID, false);
                
                completedWork++;
                ReportProgress(_workList.Count*100, completedWork*100, "");
            }
            ReportCompleted();
        }

        protected void DoWorkParallel()
        {
            foreach (IExWork work in _workList)
            {
                WorkSpawner.SetIsMuted(work.ID, true);
                hookWorkEvent(work);
                work.WorkFinished += (WorkFinishedEventHandler)delegate(object sender, WorkFinishedEventArgs e)
                {                    
                    unhookWorkEvent(work);
                    WorkSpawner.SetIsMuted(work.ID, false);

                    completedWork++;
                    if (completedWork == _workList.Count)
                        ReportCompleted();
                };
                work.Start(true);

                unhookWorkEvent(work);                
            }
        }

        protected override void DoWork()
        {
            completedWork = 0;
            if (sequential)
                DoWorkSequential();
            else DoWorkParallel();
        }

        private void hookWorkEvent(IExWork work)
        {
            work.WorkProgress += new WorkProgressEventHandler(OnWorkProgress);
            work.WorkStart += new WorkStartEventHandler(retVal_WorkStart);
            work.WorkFinished += new WorkFinishedEventHandler(retVal_WorkFinished);
            work.WorkList += new WorkListEventHandler(OnWorkList);
            work.WorkOverwrite += new WorkOverwriteEventHandler(OnWorkOverwrite);
        }

        private void unhookWorkEvent(IExWork work)
        {
            work.WorkProgress -= new WorkProgressEventHandler(OnWorkProgress);
            work.WorkStart -= new WorkStartEventHandler(retVal_WorkStart);
            work.WorkFinished -= new WorkFinishedEventHandler(retVal_WorkFinished);
            work.WorkList -= new WorkListEventHandler(OnWorkList);
            work.WorkOverwrite -= new WorkOverwriteEventHandler(OnWorkOverwrite);
        }

        void retVal_WorkStart(object sender, WorkStartEventArgs e)
        {
            ReportStart();
        }

        void OnWorkOverwrite(object sender, WorkOverwriteEventArgs e)
        {
            e.Overwrite = AskOverwrite(e.SrcEntry, e.DestEntry);             
            e.ApplyToAll = ApplyToAll;
        }

        void OnWorkList(object sender, WorkListEventArgs e)
        {
            ReportWorkList(e.ListedEntry);
        }

        void retVal_WorkFinished(object sender, WorkFinishedEventArgs e)
        {
            ReportCompleted();
        }

        void OnWorkProgress(object sender, WorkProgressEventArgs e)
        {
            int total = _workList.Count * 100;
            int completed = completedWork * 100 + (sender as IExWork).PercentCompleted;
            ReportProgress(total, completed, e.File);
        }

        #endregion


        #region Data
        private List<IExWork> _workList;
        private int completedWork = 0;
        private bool sequential = true;

        #endregion
    }
}
