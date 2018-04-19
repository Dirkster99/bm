namespace Breadcrumb.ViewModels
{
    using Breadcrumb.Demo;
    using Breadcrumb.Models;
    using Breadcrumb.SystemIO;
    using Breadcrumb.Tasks;
    using Breadcrumb.Utils;
    using Breadcrumb.ViewModels.Breadcrumbs;
    using Breadcrumb.ViewModels.Interfaces;
    using DirectoryInfoExLib.Interfaces;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Themes;
    using Themes.Base;
    using Themes.Interfaces;

    internal class AppViewModel : Base.ViewModelBase, IDisposable
    {
        #region fields
        private IThemesManager _Themes;
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

            // Initialize Themes Manager
            _Themes = Factory.CreateThemesManager();

            // Select default theme and attach theme change command
            ChangeThemeCmd_Executed("Metro Dark", Application.Current.Dispatcher);

            ViewThemeCommand = new RelayCommand(obj =>
            {
                var theme = obj as string;

                if (theme != null)
                {
                    ChangeThemeCmd_Executed(theme, Application.Current.Dispatcher);
                }
            });

            this.DiskTest = new DiskTreeNodeViewModel(new DirectoryInfo(@"C:\\"), new DirectoryInfo(@"E:\\"));

            this.ExTest = new ExTreeNodeViewModel();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the current application themes manager.
        /// </summary>
        public IThemesManager Themes
        {
            get
            {
                return this._Themes;
            }
        }

        /// <summary>
        /// Gets the command that can cancel the current LoadSync() operation.
        /// </summary>
        public ICommand ViewThemeCommand { get; }

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

        /// <summary>
        /// This procedure changes the current WPF Application Theme into another theme
        /// while the application is running (re-boot should not be required).
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <param name="disp"></param>
        private void ChangeThemeCmd_Executed(string newThemeName,
                                             Dispatcher disp)
        {
            string oldTheme = _Themes.DefaultThemeName;

            try
            {
                if (newThemeName == null)
                    return;

                // Check if request is available
                if (newThemeName == null)
                    return;

                oldTheme = this._Themes.SelectedTheme.WPFThemeName;

                // The Work to perform on another thread
                ThreadStart start = delegate
                {
                    // This works in the UI tread using the dispatcher with highest Priority
                    disp.Invoke(DispatcherPriority.Send,
                    (Action)(() =>
                    {
                      try
                      {
                          if (this._Themes.SetSelectedTheme(newThemeName) == true)
                          {
                      ////this.mSettingsManager.SettingData.CurrentTheme = newThemeName;
                      this.ResetTheme();                        // Initialize theme in process
                  }
                      }
                      catch (Exception exp)
                      {
                          MessageBox.Show(exp.StackTrace.ToString());
                      }
                  }));
                };

                // Create the thread and kick it started!
                Thread thread = new Thread(start);

                thread.Start();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Change WPF theme.
        /// 
        /// This method can be called when the theme is to be reseted by all means
        /// (eg.: when powering application up).
        /// 
        /// !!! Use the CurrentTheme property to change !!!
        /// !!! the theme when App is running           !!!
        /// </summary>
        public void ResetTheme()
        {
            // Get WPF Theme definition from Themes Assembly
            IThemeBase nextThemeToSwitchTo = this._Themes.SelectedTheme;
            this.SwitchToSelectedTheme(nextThemeToSwitchTo);
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
                    var t = Task.Factory.StartNew(() => BreadcrumbTest.InitPathAsync(request),
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
        /// Attempt to switch to the theme stated in <paramref name="nextThemeToSwitchTo"/>.
        /// The given name must map into the <seealso cref="Themes.ThemesVM.EnTheme"/> enumeration.
        /// </summary>
        /// <param name="nextThemeToSwitchTo"></param>
        private bool SwitchToSelectedTheme(IThemeBase nextThemeToSwitchTo)
        {
            const string themesModul = "Themes.dll";
            IThemeBase theme = null;

            try
            {
                // Get WPF Theme definition from Themes Assembly
                theme = this._Themes.SelectedTheme;

                if (theme != null)
                {
                    Application.Current.Resources.MergedDictionaries.Clear();

                    string ThemesPathFileName = Assembly.GetEntryAssembly().Location;

                    ThemesPathFileName = System.IO.Path.GetDirectoryName(ThemesPathFileName);
                    ThemesPathFileName = System.IO.Path.Combine(ThemesPathFileName, themesModul);
                    Assembly assembly = Assembly.LoadFrom(ThemesPathFileName);

                    if (System.IO.File.Exists(ThemesPathFileName) == false)
                    {
                        MessageBox.Show("Cannot locate themes assembly:'" + ThemesPathFileName + "'.");

                        return false;
                    }

                    foreach (var item in theme.Resources)
                    {
                        try
                        {
                            var Res = new Uri(item, UriKind.Relative);

                            var dictionary = Application.LoadComponent(Res) as ResourceDictionary;

                            if (dictionary != null)
                                Application.Current.Resources.MergedDictionaries.Add(dictionary);
                        }
                        catch (Exception Exp)
                        {
                            MessageBox.Show(string.Format("Error Loading: '{0}'\n Stack Trace: {1}\n", item, Exp.StackTrace.ToString()));
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.StackTrace.ToString());

                return false;
            }
            finally
            {
                // Update all IsSelected properties in all themes ...
                foreach (var item in this._Themes.ListAllThemes)
                {
                    item.UpdateIsSelected();
                }
            }

            return true;
        }
        #endregion methods
    }
}
