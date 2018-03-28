///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace System.IO.Tools
{
    public abstract class WorkBase : IWork, IComparable
    {       
        #region Constructor

        public WorkBase(int id)
        {            
            ID = id;
            ConstructTime = DateTime.Now;
            StartTime = DateTime.MaxValue;
            WorkStatus = WorkStatusType.wsCreated;
            Title = "Undefined";
        }
        #endregion


        #region Methods

        protected abstract void DoWork();        

        protected virtual void attachProgressDialogEvents()
        {
            this.WorkFinished += (WorkFinishedEventHandler)delegate(object s, WorkFinishedEventArgs e)
            {
                if (e.Success)
                    _progressDialog.CloseWindow();
                
                _progressDialog.IsCancelEnabled = false;
                _progressDialog.IsCompleted = true;
                if (e.Success)
                    _progressDialog.ProgressMode = ProgressMode.Success;
                else _progressDialog.ProgressMode = ProgressMode.Error;
        
                _progressDialog.SubMessage = e.Message;
            };

            this.WorkStart += (WorkStartEventHandler)delegate(object s, WorkStartEventArgs e)
            {
                _overwrite = null;

                _progressDialog.IsCompleted = false;
                _progressDialog.IsCancelEnabled = true;
                _progressDialog.ProgressMode = ProgressMode.Normal;
                
                _progressDialog.ShowWindow();                
            };

            this.WorkMessage += (WorkMessageEventHandler)delegate(object s, WorkMessageEventArgs e)
            {
                _progressDialog.Message = e.Message;
            };

            this.WorkProgress += (WorkProgressEventHandler)delegate(object s, WorkProgressEventArgs e)
            {
                _progressDialog.TotalItems = e.Total;
                _progressDialog.ItemsCompleted = e.Completed;                

                e.Cancel = _progressDialog.IsCanceled;
                if (e.File != "")
                    _progressDialog.SubMessage = e.File;
            };

            this.WorkPaused += (WorkPausedEventHandler)delegate(object s, WorkPausedEventArgs e)
            {
                _progressDialog.ProgressMode = ProgressMode.Pause;
            };

            _progressDialog.SubMessage = "Starting...";


        }

        #region ReportWork

        protected bool ReportWork(WorkProgressEventArgs e)
        {
            if (_aborted)
                return false;
            else
                if (WorkProgress != null)
                {
                    WorkProgress(this, e);
                    if (e.Cancel)
                        Abort();
                    return !e.Cancel;
                }
            return true;
        }
        object lockTotal = new object();

        protected bool ReportProgress(int total, int completed, string file)
        {
            Logger.Log(this, logType.progress, file);
            lock (lockTotal)
            {
                if (total != -1)
                    Total = total;
                if (completed != -1)
                    Completed = completed;
                if (!string.IsNullOrEmpty(file))
                    LastFile = file;
                ProcessingItem = file;
                return ReportWork(new WorkProgressEventArgs(this.ID, total, completed, LastFile));
            }
        }

        protected bool ReportProgress(int total, int completed)
        {
            return ReportProgress(total, completed, LastFile);
        }

        protected bool ReportProgress(string file)
        {
            Logger.Log(this, logType.progress, file);
            if (!string.IsNullOrEmpty(file))
            {
                LastFile = file;
            }

            if (!string.IsNullOrEmpty(LastFile))
            {
                return ReportProgress(-1, -1, LastFile);
            }                
            
            return true;
        }

        protected bool AddCompletedCount(int itemCompleted)
        {
            return ReportProgress(Total, Completed + itemCompleted);
        }

        protected bool AddCompletedCount(string processingItem)
        {
            Logger.Log(this, logType.progress, processingItem);
            return ReportProgress(Total, Completed + 1, processingItem);
        }

        protected bool AddTotalCount(int totalAdded)
        {
            return ReportProgress(Total + totalAdded, Completed);
        }

        protected bool AddTotalCount(string processingItem)
        {
            return ReportProgress(Total + 1, Completed, processingItem);
        }

        protected bool AddTotalCount()
        {
            return AddTotalCount(1);
        }

        protected bool CheckCancel()
        {
            return ReportProgress(-1, -1);
        }


        protected bool ReportStart()
        {
            Logger.Log(this, logType.start, "");
            if (WorkStatus != WorkStatusType.wsAborted)
                WorkStatus = WorkStatusType.wsRunning;
            WorkStartEventArgs e = new WorkStartEventArgs(ID);
            if (WorkStart != null)
            {
                WorkStart(this, e);
                return !e.Cancel;
            }
            return true;
        }      

        protected void ReportCompleted()
        {
            Logger.Log(this, logType.success, "");
            if (Total == 0)
                Total = 1;
            ReportProgress(Total, Total);

            WorkStatus = WorkStatusType.wsCompleted;
            WorkResult = WorkResultType.wrSuccess;
           
            if (WorkFinished != null)
                WorkFinished(this, WorkFinishedEventArgs.SuccessArgs(ID));

            if (_finishAction != null)
                _finishAction(this);

        }

        protected void ReportFailed(string message)
        {            
            if (!Aborted)
            {
                Logger.Log(this, logType.error, message);
                WorkStatus = WorkStatusType.wsCompleted;
                WorkResult = WorkResultType.wrFailed;

                if (WorkFinished != null)
                    WorkFinished(this, WorkFinishedEventArgs.FailedArgs(ID, message));
               
                _aborted = true; //Make sure all further operations are canceled.                                
            }
        }

        protected void ReportAborted()
        {
            Logger.Log(this, logType.error, "User Aborted.");
            WorkStatus = WorkStatusType.wsAborted;
            WorkResult = WorkResultType.wrAborted;
            if (WorkFinished != null)
                WorkFinished(this, WorkFinishedEventArgs.AbortArgs(ID));
        }

        protected void ReportMessage(string message)
        {
            Logger.Log(this, logType.progress, message);
            if (WorkMessage != null)
                WorkMessage(this, new WorkMessageEventArgs(ID, message));
        }
        #endregion

        #region Pause/Resume
        protected void CheckPause()
        {
            pauseTrigger.WaitOne();
        }

        public bool Paused
        {
            get { return _paused; }
            set { if (value) Pause(); else Resume(); }
        }

        public void Pause()
        {
            Logger.Log(this, logType.paused, "");
            pauseTrigger.Reset();
            _paused = true;
            if (WorkPaused != null)
                WorkPaused(this, WorkPausedEventArgs.PausedArgs(ID));
        }

        public void Resume()
        {
            Logger.Log(this, logType.resumed, "");
            pauseTrigger.Set();
            _paused = false;
            if (WorkResumed != null)
                WorkResumed(this, WorkResumedEventArgs.ResumedArgs(ID));
        }

        public void Abort()
        {
            Logger.Log(this, logType.progress, "Aborting...");
            _aborted = true;

            ReportAborted();
            if (Paused)
                Resume();
        }
        #endregion

        #endregion


        #region IWork Members

        public int ID { get; private set; }
        public string Title { get; protected set; }
        public DateTime ConstructTime { get; private set; }
        public DateTime StartTime { get; private set; }

        public WorkType WorkType { get; protected set; }
        public WorkStatusType WorkStatus { get; private set; }
        public WorkResultType WorkResult { get; private set; }

        public IWorkLogger Logger { get { return _logger; } set { _logger = value; } }

        public ICustomProgressDialog ProgressDialog
        {
            get { return _progressDialog; }
            set
            {
                _progressDialog = value;
                if (value != null)
                    attachProgressDialogEvents();
            }
        }
        public virtual bool IsProgressDialogEnabled
        {
            get { return _showProgressDialog; }
            set
            {
                if (_showProgressDialog != value)
                {
                    _showProgressDialog = value;
                    if (value && ProgressDialog == null)
                        ProgressDialog = new ShellProgressDialog();
                }
            }
        }

        public int Total { get; private set; }
        public int Completed { get; private set; }
        protected bool ApplyToAll { get { return _overwrite.HasValue; } }
        protected OverwriteMode DefaultOverwriteMode { get { return _overwrite.GetValueOrDefault(OverwriteMode.KeepOriginal); } }
        protected void SetDefaultOverwriteMode(OverwriteMode value) { _overwrite = value; } 
        public int PercentCompleted { get { return (int)(Total == 0 ? 0 : (float)Completed / (float)Total * 100.0d); } }
        public string ProcessingItem { get; private set; }
        public string LastFile { get; private set; }

        public bool Aborted { get { return _aborted; } }

        public string[] ProgressLog { get { return _progressLog.ToArray(); } }

        public event WorkProgressEventHandler WorkProgress;
        public event WorkMessageEventHandler WorkMessage;
        public event WorkStartEventHandler WorkStart;
        public event WorkFinishedEventHandler WorkFinished;
        public event WorkPausedEventHandler WorkPaused;
        public event WorkResumedEventHandler WorkResumed;

        public void Start(bool threaded)
        {
            StartTime = DateTime.Now;
            WorkStatus = WorkStatusType.wsCreated;
            WorkResult = WorkResultType.wrSuccess;
            Completed = 0;
            Total = 0;
            _aborted = false;

            if (threaded)
            {
                _worker = new Thread(new ThreadStart(DoWork));
                _worker.Start();
            }
            else DoWork();
        }



        #endregion


        #region Data

        private IWorkLogger _logger = new NullWorkLogger();
        OverwriteMode? _overwrite;
        Thread _worker;
        bool _showProgressDialog = false;
        ICustomProgressDialog _progressDialog = null;        
        bool _paused = false, _aborted = false;
        ManualResetEvent pauseTrigger = new ManualResetEvent(true);
        List<string> _progressLog = new List<string>();
        Action<WorkBase> _finishAction = null;
        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            WorkBase compareWork = (WorkBase)obj;
            if (compareWork == null)
                return -1;
            return ConstructTime.CompareTo(compareWork.ConstructTime);
        }

        #endregion
    }
}
