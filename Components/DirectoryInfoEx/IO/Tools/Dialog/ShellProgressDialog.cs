using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CtrlSoft.Win.UI;
using System.Threading;
using System.Diagnostics;

namespace System.IO.Tools
{
    public class ShellProgressDialog : ICustomProgressDialog
    {
        public ShellProgressDialog()
        {
            TotalItems = 100;
            ItemsCompleted = 0;
        }

        #region ICustomProgressDialog Members
        public ProgressMode ProgressMode { get; set; }

        public string Title { get; set; }

        public string Header { get; set; }
        public string Message { get; set; }
        public string SubMessage { get; set; }

        public string Source { get; set; } //Not supported.
        public string Target { get; set; } //Not supported.

        public int TotalItems { get; set; }
        public int ItemsCompleted { get; set; }

        public bool IsCompleted
        {
            get;
            set;
        }

        public bool IsCancelEnabled
        {
            get
            {
                return true;
            }
            set
            {

            }
        }

        public bool IsRestartEnabled
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public bool IsPauseEnabled
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public bool IsResumeEnabled
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        private bool isClosing;

        private bool _isCanceled;
        public bool IsCanceled
        {
            get { return _isCanceled; }
        }

        private bool _isPaused = false;
        public bool IsPaused
        {
            get { return _isPaused; }
        }

        public event CancelClickedHandler OnCanceled;
        public event CancelClickedHandler OnPaused = new CancelClickedHandler(() => { });        

        public void ShowWindow()
        {
            new Thread(new ThreadStart(startWindow)).Start();
            Thread.Sleep(1000);
        }

        DateTime _startTime = DateTime.Now;

        private WinProgressDialog _progressDialog = null;
        private void startWindow()
        {
            _progressDialog = new WinProgressDialog(IntPtr.Zero);

            _progressDialog.OnBeforeProgressUpdate += new WinProgressDialog.BeforeProgressUpdateHandler(OnBeforeProgressUpdate);
            _progressDialog.OnUserCancelled += delegate
            {
                _isCanceled = true;
                if (OnCanceled != null)
                    OnCanceled();
            };

            _progressDialog.Total = 101;
            _progressDialog.Complete = CompletePercent;
            //Debug.WriteLine(String.Format("{0}-{1}", ItemsCompleted, TotalItems));
            _startTime = DateTime.Now;
            _progressDialog.Start(new WinProgressDialog.ProgressOperationCallback(DoStep));

            //_progressDialog.Dispose();
        }

        private uint DoStep()
        {
            Thread.Sleep(100);
            //Debug.WriteLine(String.Format("{0}-{1}", ItemsCompleted, TotalItems));
            return CompletePercent;            
            //return (uint)_progressDialog.Complete;
        }

        private static string TimeSpanToStr(TimeSpan span)
        {
            string outStr = "";

            if (span.Hours > 0) outStr += span.Hours.ToString() + " Hours ";
            if (span.Minutes > 0) outStr += span.Minutes.ToString() + " Minutes ";
            if (span.Seconds > 0) outStr += span.Seconds.ToString() + " Seconds ";

            if (outStr == "") return " ";

            return outStr + "Remaining.";
        }

        private uint CompletePercent
        {
            get
            {
                return (uint)((float)ItemsCompleted / (float)TotalItems * 100.0d);
            }
        }

        private void OnBeforeProgressUpdate(object sender, EventArgs e)
        {
            Thread.Sleep(100);
            //WinProgressDialog _progressDialog = ((WinProgressDialog)sender);

            //20: Fixed ShellProgressDialog still running after closed.
            _progressDialog.Total =  isClosing ? (uint)100 : 101;
            _progressDialog.Complete = isClosing ? (uint)100 : CompletePercent;
            //Debug.WriteLine(String.Format("{0}-{1}", ItemsCompleted, TotalItems));

            TimeSpan timeUsed = DateTime.Now.Subtract(_startTime);
            TimeSpan timeRemain = TimeSpan.FromTicks((ItemsCompleted == 0 || ItemsCompleted == 100) ? 0 : (long)((float)timeUsed.Ticks / (float)ItemsCompleted * 100));

            _progressDialog.Title = Title;
            _progressDialog.SetLine(WinProgressDialog.IPD_Lines.LineOne, Header == null ? "" : Header, false);
            _progressDialog.SetLine(WinProgressDialog.IPD_Lines.LineTwo, Message == null ? "" : Message, false);
            _progressDialog.SetLine(WinProgressDialog.IPD_Lines.LineThree, SubMessage == null ? TimeSpanToStr(timeRemain) : SubMessage, false);

            System.Threading.Thread.Sleep(100);

            if (isClosing)
                _progressDialog.Stop();

        }

        public void CloseWindow()
        {
            isClosing = true;
        }

        #endregion

       
    }
}
