namespace Breadcrumb.ViewModels
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Threading;
	using System.Windows;
	using System.Windows.Input;
	using System.Windows.Threading;
	using Breadcrumb.Demo;
	using Breadcrumb.DirectoryInfoEx;
	using Breadcrumb.SystemIO;
	using Breadcrumb.Utils;
	using Breadcrumb.ViewModels.Interfaces;
	using Themes;
	using Themes.Base;
    using BreadcrumbLib.Utils;

	public class AppViewModel : Breadcrumb.Viewmodels.Base.NotifyPropertyChanged
	{
		#region fields
		private ThemesManager mThemes;

		private DiskTreeNodeViewModel mDiskTest;
		////private ExTreeNodeViewModel mExTest;
		#endregion fields

		#region constructors
		public AppViewModel()
		{
			this.SpecialFoldersTest = new SpecialFoldersViewModel();

			this.mThemes = new ThemesManager();

			this.ChangeThemeCmd_Executed("Metro Dark", Application.Current.Dispatcher);

			this.ViewThemeCommand = new RelayCommand(obj =>
			{
				var theme = obj as string;

				if (theme != null)
				{
					this.ChangeThemeCmd_Executed(theme, Application.Current.Dispatcher);
				}
			});

			this.DiskTest = new DiskTreeNodeViewModel(new DirectoryInfo(@"C:\\"), new DirectoryInfo(@"D:\\"));
			(this.DiskTest.Selection as ITreeRootSelector<DiskTreeNodeViewModel, string>).SelectAsync(@"C:\Temp");

			this.ExTest = new ExTreeNodeViewModel();
			(this.ExTest.Selection as ITreeRootSelector<ExTreeNodeViewModel, FileSystemInfoEx>).SelectAsync(DirectoryInfoEx.FromString(@"C:\temp"));

			this.ExTest1 = new ExTreeNodeViewModel();
			(this.ExTest1.Selection as ITreeRootSelector<ExTreeNodeViewModel, FileSystemInfoEx>).SelectAsync(DirectoryInfoEx.FromString(@"C:\temp"));

			// If you want to show only root directories, try Toggle this line in TreeRootSelector.
			////updateRootItemsAsync(this, _rootItems, 2);
		}
		#endregion constructors

		#region properties
		/// <summary>
		/// Gets the current application themes manager.
		/// </summary>
		public ThemesManager Themes
		{
			get
			{
				return this.mThemes;
			}
		}

		/// <summary>
		/// Gets the command that can cancel the current LoadSync() operation.
		/// </summary>
		public ICommand ViewThemeCommand
		{
			get; private set;
		}

		public DiskTreeNodeViewModel DiskTest
		{
			get
			{
				return this.mDiskTest;
			}

			private set
			{
				this.mDiskTest = value;
			}
		}

		public ExTreeNodeViewModel ExTest { get; private set; }

		public ExTreeNodeViewModel ExTest1 { get; private set; }

		public SpecialFoldersViewModel SpecialFoldersTest { get; private set; }
		#endregion properties

		#region methods
		/// <summary>
		/// This procedure changes the current WPF Application Theme into another theme
		/// while the application is running (re-boot should not be required).
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		/// <param name="disp"></param>
		private void ChangeThemeCmd_Executed(string newThemeName,
																				 System.Windows.Threading.Dispatcher disp)
		{
			string oldTheme = ThemesManager.DefaultThemeName;

			try
			{
				if (newThemeName == null)
					return;

				// Check if request is available
				if (newThemeName == null)
					return;

				oldTheme = this.mThemes.SelectedTheme.WPFThemeName;

				// The Work to perform on another thread
				ThreadStart start = delegate
				{
					// This works in the UI tread using the dispatcher with highest Priority
					disp.Invoke(DispatcherPriority.Send,
					(Action)(() =>
					{
						try
						{
							if (this.mThemes.SetSelectedTheme(newThemeName) == true)
							{
								////this.mSettingsManager.SettingData.CurrentTheme = newThemeName;
								this.ResetTheme();                        // Initialize theme in process

                                this.ExTest.Selection.RefreshIconsAsync();
                                this.ExTest1.Selection.RefreshIconsAsync();

                                //this.ExTest.Selection.LookupAsync(this.ExTest.Selection.Value,
                                //    TreeSelectors.RecrusiveBroadcast<ExTreeNodeViewModel, FileSystemInfoEx>.SkipIfNotLoaded, 
                                //    CancellationToken.None,
                                //    TreeLookupProcessors.RefreshIcons<ExTreeNodeViewModel, FileSystemInfoEx>.IfLoaded);
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
			ThemeBase nextThemeToSwitchTo = this.mThemes.SelectedTheme;
			this.SwitchToSelectedTheme(nextThemeToSwitchTo);
		}

		/// <summary>
		/// Attempt to switch to the theme stated in <paramref name="nextThemeToSwitchTo"/>.
		/// The given name must map into the <seealso cref="Themes.ThemesVM.EnTheme"/> enumeration.
		/// </summary>
		/// <param name="nextThemeToSwitchTo"></param>
		private bool SwitchToSelectedTheme(ThemeBase nextThemeToSwitchTo)
		{
			const string themesModul = "Themes.dll";
			ThemeBase theme = null;

			try
			{
				// Get WPF Theme definition from Themes Assembly
				theme = this.mThemes.SelectedTheme;

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
				foreach (var item in this.mThemes.ListAllThemes)
				{
					item.UpdateIsSelected();
				}
			}

			return true;
		}
		#endregion methods
	}
}
