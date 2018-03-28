using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Tools
{
    public delegate void CancelClickedHandler();
    public delegate void PauseClickedHandler();

    public enum ProgressMode { Normal, Pause, Abort, Error, Success }

    public interface ICustomProgressDialog
    {
        ProgressMode ProgressMode { get; set; }
        string Title { get; set; }
        string Header { get; set; }
        string Message { get; set; }
        string SubMessage { get; set; }
        string Source { get; set; }
        string Target { get; set; }
        int TotalItems { get; set; }
        int ItemsCompleted { get; set; }

        bool IsCompleted { get; set; }
        bool IsCancelEnabled { get; set; }
        bool IsRestartEnabled { get; set; }
        bool IsPauseEnabled { get; set; }
        bool IsResumeEnabled { get; set; }

        bool IsCanceled { get; }
        bool IsPaused { get; }

        event CancelClickedHandler OnCanceled;
        event CancelClickedHandler OnPaused;

        void ShowWindow();
        void CloseWindow();
    }
}
