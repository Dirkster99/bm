namespace Breadcrumb.ViewModels
{
    using Breadcrumb.Demo;
    using Breadcrumb.Utils;
    using Breadcrumb.ViewModels.Breadcrumbs;
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Themes;
    using Themes.Base;
    using Themes.Interfaces;

    internal class AppViewModel : Base.ViewModelBase
    {
    #region fields
    private IThemesManager _Themes;
    #endregion fields

    #region constructors
    /// <summary>
    /// Class constructor.
    /// </summary>
    public AppViewModel()
    {
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
    public ICommand ViewThemeCommand{ get; }

    /// <summary>
    /// Gets a Breadcrumb Tree ViewModel that drives the Breadcrumb control demo
    /// in this application.
    /// </summary>
    public BreadcrumbViewModel BreadcrumbTest { get; }

    /// <summary>
    /// Gets a viewmodel to demonstrate Icon display for special folders in Windows.
    /// </summary>
    public SpecialFoldersViewModel SpecialFoldersTest { get; }
    #endregion properties

    #region methods
    /// <summary>
    /// Method should be called after construction to initialize the viewmodel
    /// to view a default content.
    /// </summary>
    public void InitPath(string initialPath)
    {
      BreadcrumbTest.InitPath(initialPath);
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
