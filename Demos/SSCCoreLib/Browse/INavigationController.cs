namespace SSCoreLib.Browse
{
    using WSF.Interfaces;
    using System.Threading;

    /// <summary>
    /// Is implemented by the controller viewmodel to coordinate all actions
    /// in a task based and cancelable way.
    /// </summary>
    public interface INavigationController
    {
        CancellationTokenSource GetCancelToken();

        void QueueTask(BrowseRequest<IDirectoryBrowser> request);
    }
}
