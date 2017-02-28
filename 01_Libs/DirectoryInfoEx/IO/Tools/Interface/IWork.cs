///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace System.IO.Tools
{
    public enum WorkType { Unknown, List, Copy, Move, Delete };
    public enum WorkStatusType { wsCreated, wsRunning, wsAborted, wsCompleted }
    public enum WorkResultType { wrSuccess, wrAborted, wrFailed }
    public enum OverwriteMode { Ask, KeepOriginal, Replace };

    #region Event Handler
    public delegate void WorkProgressEventHandler(object sender, WorkProgressEventArgs e);
    public delegate void WorkMessageEventHandler(object sender, WorkMessageEventArgs e);
    public delegate void WorkStartEventHandler(object sender, WorkStartEventArgs e);
    public delegate void WorkFinishedEventHandler(object sender, WorkFinishedEventArgs e);
    public delegate void WorkPausedEventHandler(object sender, WorkPausedEventArgs e);
    public delegate void WorkResumedEventHandler(object sender, WorkResumedEventArgs e);
    

    public class WorkProgressEventArgs : CancelEventArgs
    {
        public int ID { get; private set; }
        public int Total { get; private set; }
        public int Completed { get; private set; }
        public string File { get; private set; }       

        public WorkProgressEventArgs(int id, int total, int completed, string file)
        {
            this.ID = id;
            this.Total = total;
            this.Completed = completed;
            this.File = file;
        }
    }

    public class WorkMessageEventArgs : CancelEventArgs
    {
        public int ID { get; private set; }        
        public string Message { get; private set; }

        public WorkMessageEventArgs(int id, string message)
        {
            this.ID = id;
            this.Message = message;
        }
    }

    public class WorkFinishedEventArgs : EventArgs
    {
        public int ID { get; private set; }
        public bool Success { get; private set; }
        public string Message { get; private set; }

        public WorkFinishedEventArgs(int id, bool success, string message)
        {
            ID = id;
            Success = success;
            Message = message;
        }
        public static WorkFinishedEventArgs SuccessArgs(int id) { return new WorkFinishedEventArgs(id, true, "Success"); }
        public static WorkFinishedEventArgs FailedArgs(int id, string message) { return new WorkFinishedEventArgs(id, false, "Error " + message); }
        public static WorkFinishedEventArgs AbortArgs(int id) { return new WorkFinishedEventArgs(id, false, "User Aborted"); }
    }

    public class WorkStartEventArgs : CancelEventArgs
    {
        public int ID { get; private set; }

        public WorkStartEventArgs(int id)
        {
            ID = id;
        }
    }

    public class WorkPausedEventArgs : EventArgs
    {
        public int ID { get; private set; }

        public WorkPausedEventArgs(int id)
        {
            ID = id;
        }

        public static WorkPausedEventArgs PausedArgs(int id) { return new WorkPausedEventArgs(id); }
    }

    public class WorkResumedEventArgs : EventArgs
    {
        public int ID { get; private set; }

        public WorkResumedEventArgs(int id)
        {
            ID = id;
        }

        public static WorkResumedEventArgs ResumedArgs(int id) { return new WorkResumedEventArgs(id); }
    }
    #endregion



    public interface IWork
    {
        int ID { get; }
        string Title { get; }
        DateTime ConstructTime { get; }
        DateTime StartTime { get; }
        WorkType WorkType { get; }

        //Progress
        WorkStatusType WorkStatus { get; }
        WorkResultType WorkResult { get; }
        int Total { get; }
        int Completed { get; }
        int PercentCompleted { get; }
        string ProcessingItem { get; }        
        bool Paused { get; set; }
        bool Aborted { get; }
        string[] ProgressLog { get; }

        ICustomProgressDialog ProgressDialog { get; set; }
        bool IsProgressDialogEnabled { get; set; }

        IWorkLogger Logger { get; set; }

        event WorkProgressEventHandler WorkProgress;
        event WorkMessageEventHandler WorkMessage;
        event WorkStartEventHandler WorkStart;
        event WorkFinishedEventHandler WorkFinished;
        event WorkPausedEventHandler WorkPaused;
        event WorkResumedEventHandler WorkResumed;

        void Start(bool threaded);
        void Abort();
    }
}
