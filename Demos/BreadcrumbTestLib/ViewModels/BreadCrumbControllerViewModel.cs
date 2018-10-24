namespace BreadcrumbTestLib.ViewModels
{
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.Tasks;
    using BreadcrumbTestLib.ViewModels.Breadcrumbs;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using ShellBrowserLib;
    using ShellBrowserLib.Interfaces;
    using SSCoreLib.Browse;
    using SSCoreLib.Interfaces;
    using SSCoreLib.ViewModels;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Implements a Breadcrumb Controller ViewModel that contains a BreadCrumb Browser
    /// ViewModel who's background tasks are coordinated by this controller.
    /// </summary>
    public class BreadCrumbControllerViewModel : Base.ViewModelBase, IDisposable, INavigationController
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private OneTaskLimitedScheduler _OneTaskScheduler;
        private SemaphoreSlim _Semaphore;
        private CancellationTokenSource _CancelTokenSource;
        private bool _disposed = false;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        public BreadCrumbControllerViewModel()
        {
            _Semaphore = new SemaphoreSlim(1, 1);
            _OneTaskScheduler = new OneTaskLimitedScheduler();
            _CancelTokenSource = new CancellationTokenSource();

            // Initialize Breadcrumb Tree ViewModel and SpecialFolders Test ViewModel
            BreadcrumbBrowser = new BreadcrumbViewModel();

            TaskQueue = new BrowseRequestTaskQueueViewModel();

            WeakEventManager<ICanNavigate, BrowsingEventArgs>
                .AddHandler(BreadcrumbBrowser, "BrowseEvent", Control_BrowseEvent);
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a Breadcrumb Tree ViewModel that drives the Breadcrumb control demo
        /// in this application.
        /// </summary>
        public IBreadcrumbViewModel BreadcrumbBrowser { get; }

        public IBrowseRequestTaskQueueViewModel TaskQueue { get; }
        #endregion properties

        #region methods

        #region INavigationController interface
        CancellationTokenSource INavigationController.GetCancelToken()
        {
            return TaskQueue.GetCancelTokenSrc();
        }

        void INavigationController.QueueTask(BrowseRequest<IDirectoryBrowser> request)
        {
            TaskQueue.AddTaskToQueue(request);
        }
        #endregion INavigationController interface

        /// <summary>
        /// One of the controls has changed its location in its space.
        /// This method is invoked to synchronize this change with all other controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_BrowseEvent(object sender, BrowsingEventArgs e)
        {
            var location = e.Location;

            ////            SelectedFolder = location.Path;
            ////
            ////            if (e.IsBrowsing == false && e.Result == BrowseResult.Complete)
            ////            {
            ////                // XXX Todo Keep task reference, support cancel, and remove on end?
            ////                try
            ////                {
            ////                    var timeout = TimeSpan.FromSeconds(5);
            ////                    var actualTask = new Task(() =>
            ////                    {
            ////                        var request = new BrowseRequest(location, _CancelTokenSourc.Token);
            ////
            ////                        var t = Task.Factory.StartNew(() => NavigateToFolderAsync(request, sender),
            ////                                                            request.CancelTok,
            ////                                                            TaskCreationOptions.LongRunning,
            ////                                                            _OneTaskScheduler);
            ////
            ////                        if (t.Wait(timeout) == true)
            ////                            return;
            ////
            ////                        _CancelTokenSourc.Cancel();           // Task timed out so lets abort it
            ////                        return;                         // Signal timeout here...
            ////                    });
            ////
            ////                    actualTask.Start();
            ////                    actualTask.Wait();
            ////                }
            ////                catch (System.AggregateException ex)
            ////                {
            ////                    Logger.Error(ex);
            ////                }
            ////                catch (Exception ex)
            ////                {
            ////                    Logger.Error(ex);
            ////                }
            ////            }
            ////            else
            ////            {
            ////                if (e.IsBrowsing == true)
            ////                {
            ////                    // The sender has messaged: "I am changing location..."
            ////                    // So, we set this property to tell the others:
            ////                    // 1) Don't change your location now (eg.: Disable UI)
            ////                    // 2) We'll be back to tell you the location when we know it
            ////                    if (TreeBrowser != sender)
            ////                        TreeBrowser.SetExternalBrowsingState(true);
            ////
            ////                    if (FolderTextPath != sender)
            ////                        FolderTextPath.SetExternalBrowsingState(true);
            ////
            ////                    if (FolderItemsView != sender)
            ////                        FolderItemsView.SetExternalBrowsingState(true);
            ////                }
            ////            }
        }
 
        /// <summary>
        /// Method should be called after construction to initialize the viewmodel
        /// to view a default content.
        /// </summary>
        public async Task InitPathAsync(string initialPath)
        {
            // Revert request to default if requested path is non-existing
            if (ShellBrowser.DirectoryExists(initialPath) == false)
                initialPath =  ShellBrowser.SysDefault.FullName;

            Logger.InfoFormat("'{0}'", initialPath);

            var location = ShellBrowser.Create(initialPath);
            await BreadcrumbBrowser.InitPathAsync();
            NavigateToFolder(location);
        }

        #region Disposable Interfaces
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
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    // Dispose of the currently used inner disposables
                    _OneTaskScheduler.Dispose();
                    _Semaphore.Dispose();
                    _CancelTokenSource.Dispose();
                    TaskQueue.Dispose();

                    _OneTaskScheduler = null;
                    _Semaphore = null;
                    _CancelTokenSource = null;
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            _disposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }
        #endregion Disposable Interfaces

        /// <summary>
        /// Master controller interface method to navigate all views
        /// to the folder indicated in <paramref name="folder"/>
        /// - updates all related viewmodels.
        /// </summary>
        /// <param name="itemPath"></param>
        /// <param name="requestor"</param>
        private void NavigateToFolder(IDirectoryBrowser location)
        {
            Logger.InfoFormat("'{0}'", location.FullName);

            // XXX Todo Keep task reference, support cancel, and remove on end?
            try
            {
                // XXX Todo Keep task reference, support cancel, and remove on end?
                var timeout = TimeSpan.FromSeconds(5);
                var actualTask = new Task(() =>
                {
                    ////string[] pathSegments = DirectoryInfoExLib.Factory.GetFolderSegments(location.FullName);
                    var request = new BrowseRequest<IDirectoryBrowser>(location, RequestType.Navigational, _CancelTokenSource.Token);
                    var t = Task.Factory.StartNew(() => NavigateToFolderAsync(request, null),
                                                        request.CancelTok,
                                                        TaskCreationOptions.LongRunning,
                                                        _OneTaskScheduler);

                    if (t.Wait(timeout) == true)
                        return;

                    _CancelTokenSource.Cancel();       // Task timed out so lets abort it
                    return;                     // Signal timeout here...
                });

                actualTask.Start();
                actualTask.Wait();
            }
            catch (System.AggregateException e)
            {
                Debug.WriteLine(e);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        /// <summary>
        /// Master controler interface method to navigate all views
        /// to the folder indicated in <paramref name="folder"/>
        /// - updates all related viewmodels.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="requestor"</param>
        private async Task<FinalBrowseResult<IDirectoryBrowser>> NavigateToFolderAsync(
             BrowseRequest<IDirectoryBrowser> request
            ,object sender)
        {
            Logger.InfoFormat("'{0}'", request.NewLocation);

            // Make sure the task always processes the last input but is not started twice
            await _Semaphore.WaitAsync();
            try
            {
                var newPath = request.NewLocation;
                var cancel = request.CancelTok;

                if (cancel != null)
                    cancel.ThrowIfCancellationRequested();

                var browseResult = await BreadcrumbBrowser.NavigateToAsync(
                    request,
                    "AppViewModel.NavigateToFolderAsync"
                    );

                return browseResult;
            }
            catch (Exception exp)
            {
                var result = FinalBrowseResult<IDirectoryBrowser>.FromRequest(null, BrowseResult.InComplete);
                result.UnexpectedError = exp;
                return result;
            }
            finally
            {
                _Semaphore.Release();
            }
        }
        #endregion methods
    }
}
