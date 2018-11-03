namespace BmLib.Interfaces
{
    using System.Windows.Input;

    /// <summary>
    /// Defines an interface of an object that supports the maintainance of cancelable
    /// background tasks in order to support management of Cancel-able background tasks
    /// that can run longer than 1 second in dependence of the actual runtime environment.
    /// </summary>
    public interface IBrowseRequestTaskQueue
    {
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
    }
}
