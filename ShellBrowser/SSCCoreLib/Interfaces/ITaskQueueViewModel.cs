namespace SSCoreLib.Interfaces
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Input;
    using ShellBrowserLib.Interfaces;
    using SSCoreLib.Browse;

    /// <summary>
    /// Defines an interface to a viewmodel object that keeps a collection of active tasks
    /// that can be canceled in order to support management of Cancel-able background tasks
    /// that can run longer than 1 second in dependence of the actual runtime environment.
    /// </summary>
    public interface ITaskQueueViewModel : IDisposable, INotifyPropertyChanged
    {
        #region properties
        /// <summary>
        /// Gets whether there is a long running background task processing
        /// that could be canceled.
        /// </summary>
        bool IsProcessCancelable { get; }

        /// <summary>
        /// Gets a command that will cancel all currently queued tasks (if any).
        /// Canceled tasks may run to completeness or leave off in the middle of
        /// the processing - it is the responsibility of the caller to ensure
        /// clean states and correct processing after cancellation has taken place.
        /// </summary>
        ICommand CancelProcess { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Adds a browse request task into the queue if the contained
        /// RequestId unique towards all tasks queued so far.
        /// </summary>
        /// <param name="request"></param>
        void AddTaskToQueue(BrowseRequest<IDirectoryBrowser> request);

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
        /// <param name="request"></param>
        /// <returns>Returns the browse task that was successfully removed (if any)
        /// from the queue, otherwise this returns null.
        /// </returns>
        void RemoveTaskFromQueue(BrowseRequest<IDirectoryBrowser> request);

        /// <summary>
        /// Gets the current cancellation source which can be used to generate
        /// another cancellation token which in turn is required to signal a task
        /// when it should be ending (because user canceled or timeout or such ...)
        /// </summary>
        CancellationTokenSource GetCancelTokenSrc();
        #endregion
    }
}
