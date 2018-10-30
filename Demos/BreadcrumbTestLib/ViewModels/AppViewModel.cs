namespace BreadcrumbTestLib.ViewModels
{
    using System;
    using System.Threading.Tasks;

    public class AppViewModel : Base.ViewModelBase, IDisposable
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _disposed = false;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor from descriptive demo title name.
        /// </summary>
        /// <param name="demoTitle"></param>
        public AppViewModel(string demoTitle)
            : this()
        {
            DemoTitle = demoTitle;
        }

        /// <summary>
        /// Class constructor.
        /// </summary>
        protected AppViewModel()
        {
            // Initialize Breadcrumb Controlller with a containing Breadcrumb ViewModel
            BreadcrumbController = new BreadCrumbControllerViewModel();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the title of this demo
        /// </summary>
        public string DemoTitle { get; }

        /// <summary>
        /// Gets a Breadcrumb Controller ViewModel that contains a BreadCrumb Browser
        /// ViewModel who's background tasks are coordinated by this controller.
        /// </summary>
        public BreadCrumbControllerViewModel BreadcrumbController { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Method should be called after construction to initialize the viewmodel
        /// to view a default content.
        /// </summary>
        public void InitPath(string initialPath)
        {
            BreadcrumbController.InitPath(initialPath);
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
                    BreadcrumbController.Dispose();
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
        #endregion methods
    }
}
