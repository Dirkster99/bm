namespace SSCoreLib.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using WSF.Interfaces;
    using SSCoreLib.Browse;
    using SSCoreLib.Interfaces;
    using SSCoreLib.ViewModels.Base;

    /// <summary>
    /// Keeps a collection of active tasks that can be canceled in order to support
    /// management of Cancel-able background tasks that can run longer than 1 second
    /// in dependence of the actual runtime environment.
    /// </summary>
    public class TaskQueueViewModel : ViewModelBase, ITaskQueueViewModel
    {
        #region fields
        private bool _Disposed = false;
        private bool _IsProcessCancelable;

        private CancellationTokenSource _CancelTokenSourc;
        private readonly Dictionary<string, BrowseRequest<IDirectoryBrowser>> _TaskQueue;
        private readonly SemaphoreSlim _SlowStuffSemaphore;

        private ICommand _CancelProcess;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public TaskQueueViewModel()
        {
            _CancelTokenSourc = new CancellationTokenSource();
            _TaskQueue = new Dictionary<string, BrowseRequest<IDirectoryBrowser>>();
            _SlowStuffSemaphore = new SemaphoreSlim(1, 1);
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets whether there is a long running background task processing
        /// that could be canceled.
        /// </summary>
        public bool IsProcessCancelable
        {
            get { return _IsProcessCancelable; }

            protected set
            {
                if (_IsProcessCancelable != value)
                {
                    _IsProcessCancelable = value;
                    RaisePropertyChanged(() => IsProcessCancelable);
                }
            }
        }

        /// <summary>
        /// Gets a command that will cancel all currently queued tasks (if any).
        /// Canceled tasks may run to completeness or leave off in the middle of
        /// the processing - it is the responsibility of the caller to ensure
        /// clean states and correct processing after cancellation has taken place.
        /// </summary>
        public ICommand CancelProcess
        {
            get
            {
                if (_CancelProcess == null)
                {
                    _CancelProcess = new RelayCommand<object>(
                        (p) =>
                        {
                            QueueTaskCancelation();
                        },
                        (p) => { return IsProcessCancelable; } );
                }

                return _CancelProcess;

            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Adds a browse request task into the queue if the contained
        /// RequestId unique towards all tasks queued so far.
        /// </summary>
        public void AddTaskToQueue(BrowseRequest<IDirectoryBrowser> request)
        {
            _SlowStuffSemaphore.Wait();
            try
            {
                // Don't queue same request twice
                BrowseRequest<IDirectoryBrowser> trial = null;
                if (_TaskQueue.TryGetValue(request.RequestId.ToString(), out trial) == false)
                    _TaskQueue.Add(request.RequestId.ToString(), request);
                else
                    return;
            }
            finally
            {
                _SlowStuffSemaphore.Release();
            }

            // Remove task from queue when its done
            request.BrowseTask.ContinueWith((r) => { RemoveTaskFromQueue(request); });

            // Remove task if it completed in the meantime
            if (request.BrowseTask.Status == TaskStatus.RanToCompletion)
                RemoveTaskFromQueue(request);

            UpdateCancelStatus();
        }

        /// <summary>
        /// Removes a browse request task from the queue if the contained
        /// unique id can be found in the queue (Invoking this method is
        /// not required under normal circumstances because completed tasks
        /// are not queued and tasks that completed normally are automatically
        /// removed upon completion - see AddTaskToQueue() for details).
        /// 
        /// The removed task is not canceled or processed in any other way
        /// - it is the responsibility of the caller to implement additional
        /// processing if necessary.
        /// </summary>
        /// <returns>Returns the browse task that was successfully removed (if any)
        /// from the queue, otherwise this returns null.
        /// </returns>
        public void RemoveTaskFromQueue(BrowseRequest<IDirectoryBrowser> request)
        {
            _SlowStuffSemaphore.Wait();
            try
            {
                _TaskQueue.Remove(request.RequestId.ToString());
            }
            catch { }
            finally
            {
                _SlowStuffSemaphore.Release();
            }

            UpdateCancelStatus();
        }

        /// <summary>
        /// Gets the current cancellation source which can be used to generate
        /// another cancellation token which in turn is required to signal a task
        /// when it should be ending (because user canceled or timeout or such ...)
        /// </summary>
        public CancellationTokenSource GetCancelTokenSrc()
        {
            return _CancelTokenSourc;
        }

        /// <summary>
        /// Method updates the <see cref="IsProcessCancelable"/> property
        /// to indicate whether the queue currently contains cancel-able
        /// tasks or not.
        /// </summary>
        /// <returns>True if there is at least one cancel-able task,otherwise false.
        /// </returns>
        private void UpdateCancelStatus()
        {
            if (_TaskQueue.Count > 0)
                IsProcessCancelable = true;
            else
                IsProcessCancelable = false;
        }

        /// <summary>
        /// Sends all queued tasks that are scheduled with a
        /// non-default cancellation token a cancel signal
        /// to empty queue as soon as possible.
        /// </summary>
        private void QueueTaskCancelation()
        {
            _SlowStuffSemaphore.Wait();
            try
            {
                foreach (var item in _TaskQueue.Values)
                {
                    try
                    {
                        // signal task that it should cancel its processing
                        if (item.CancelTok.IsCancellationRequested == false)
                            item.CancelTokenSource.Cancel();
                    }
                    catch { }
                }
            }
            finally
            {
                _SlowStuffSemaphore.Release();
            }

            _CancelTokenSourc.Dispose();
            _CancelTokenSourc = new CancellationTokenSource();
        }

        #region IDisposable interface
        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (_Disposed == false)
                {
                    if (disposing == true)
                    {
                        // Dispose of the curently displayed content
                        if (_CancelTokenSourc != null)
                          _CancelTokenSourc.Dispose();
                        
                        _CancelTokenSourc = null;
                    }
    
                    // There are no unmanaged resources to release, but
                    // if we add them, they need to be released here.
                }
    
                _Disposed = true;
    
                //// If it is available, make the call to the
                //// base class's Dispose(Boolean) method
                ////base.Dispose(disposing);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception on dispose {0}", ex.Message);
            }
        }
        #endregion IDisposable interface
        #endregion methods
    }
}
