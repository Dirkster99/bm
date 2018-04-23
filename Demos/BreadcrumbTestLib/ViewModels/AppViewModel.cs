namespace BreadcrumbTestLib.ViewModels
{
    using BreadcrumbTestLib.Demo;
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.SystemIO;
    using BreadcrumbTestLib.Tasks;
    using BreadcrumbTestLib.ViewModels.Breadcrumbs;
    using BreadcrumbTestLib.ViewModels.Interfaces;
    using DirectoryInfoExLib.Interfaces;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class AppViewModel : Base.ViewModelBase, IDisposable
    {
        #region fields
        private DiskTreeNodeViewModel _DiskTest;

        private readonly SemaphoreSlim _SlowStuffSemaphore;
        private readonly OneTaskLimitedScheduler _OneTaskScheduler;
        private readonly CancellationTokenSource _CancelTokenSource;
        private bool _disposed = false;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        public AppViewModel()
        {
            _SlowStuffSemaphore = new SemaphoreSlim(1, 1);
            _OneTaskScheduler = new OneTaskLimitedScheduler();
            _CancelTokenSource = new CancellationTokenSource();

            // Initialize Breadcrumb Tree ViewModel and SpecialFolders Test ViewModel
            BreadcrumbTest = new BreadcrumbViewModel();
            SpecialFoldersTest = new SpecialFoldersViewModel();

            this.DiskTest = new DiskTreeNodeViewModel(new DirectoryInfo(@"C:\\"), new DirectoryInfo(@"E:\\"));

            this.ExTest = new ExTreeNodeViewModel();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a Breadcrumb Tree ViewModel that drives the Breadcrumb control demo
        /// in this application.
        /// </summary>
        public BreadcrumbViewModel BreadcrumbTest { get; }

        /// <summary>
        /// Gets a viewmodel to demonstrate Icon display for special folders in Windows.
        /// </summary>
        public SpecialFoldersViewModel SpecialFoldersTest { get; }

        public DiskTreeNodeViewModel DiskTest
        {
            get
            {
                return _DiskTest;
            }

            private set
            {
                _DiskTest = value;
                NotifyPropertyChanged(() => DiskTest);
            }
        }

        public ExTreeNodeViewModel ExTest { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Method should be called after construction to initialize the viewmodel
        /// to view a default content.
        /// </summary>
        public void InitPath(string initialPath)
        {
            var selection = DiskTest.Selection as ITreeRootSelector<DiskTreeNodeViewModel, string>;
            selection.SelectAsync(@"C:\tmp");

            var sel2 = ExTest.Selection as ITreeRootSelector<ExTreeNodeViewModel, IDirectoryInfoEx>;
            sel2.SelectAsync(DirectoryInfoExLib.Factory.FromString(@"C:\tmp"));

            NavigateToFolder(initialPath);
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
                    // Dispose of the curently displayed content
                    _OneTaskScheduler.Dispose();
                    _SlowStuffSemaphore.Dispose();
                    _CancelTokenSource.Dispose();
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
        private void NavigateToFolder(string itemPath)
        {
            // XXX Todo Keep task reference, support cancel, and remove on end?
            try
            {
                // XXX Todo Keep task reference, support cancel, and remove on end?
                var timeout = TimeSpan.FromSeconds(5);
                var actualTask = new Task(() =>
                {
                    var request = new BrowseRequest<string>(itemPath, _CancelTokenSource.Token);
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
        private async Task<FinalBrowseResult<IDirectoryInfoEx>> NavigateToFolderAsync(
                                                                BrowseRequest<string> request,
                                                                       object sender)
        {
            // Make sure the task always processes the last input but is not started twice
            await _SlowStuffSemaphore.WaitAsync();
            try
            {
                var newPath = request.NewLocation;
                var cancel = request.CancelTok;

                if (cancel != null)
                    cancel.ThrowIfCancellationRequested();

                var browseResult = await BreadcrumbTest.InitPathAsync(request);

                return browseResult;
            }
            catch (Exception exp)
            {
                var result = FinalBrowseResult<IDirectoryInfoEx>.FromRequest(null, BrowseResult.InComplete);
                result.UnexpectedError = exp;
                return result;
            }
            finally
            {
                _SlowStuffSemaphore.Release();
            }
        }
        #endregion methods
    }
}
