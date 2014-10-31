﻿namespace Themes
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Reflection;
	using System.Windows;
	using Themes.Base;
	using Themes.Interfaces;

	/// <summary>
	/// This class manages a list of WPF themes (Aero, Metro etc) which
	/// can be combined with TextEditorThemes (True Blue, Deep Black).
	/// 
	/// The class implements a service that can be accessed via Instance property.
	/// The exposed methodes and properties can be used to display a list available
	/// themes, determine the currently selected theme, and set the currently selected
	/// theme.
	/// </summary>
	public class ThemesManager : IThemesManager, IParentSelectedTheme
	{
		#region fields
		#region WPF Themes
		#region Expression Dark theme resources
		const string MetroDarkThemeName = "Metro Dark";
		static readonly string[] MetroDarkResources = 
    {
			"/Themes;component/Metro/Dark/ModernUI.dark.xaml",
			"/BreadcrumbLib;component/Themes/Metro/MetroDarkBrushes.xaml",
			"/FirstFloor.ModernUI;component/Assets/ScrollBar.xaml"
    };
		#endregion Expression Dark theme resources

		#region Generic theme resources
		const string GenericThemeName = "Generic";
		static readonly string[] GenericResources = 
    {
    };
		#endregion Generic theme resources

		#region Light Metro theme resources
		const string MetroLightThemeName = "Metro Light";
		static readonly string[] MetroResources = 
    {
      "/Themes;component/Metro/Light/ModernUI.light.xaml",
      "/BreadcrumbLib;component/Themes/Metro/MetroLightBrushes.xaml",
      "/FirstFloor.ModernUI;component/Assets/ScrollBar.xaml"
    };
		#endregion Light Metro theme resources
		#endregion WPF Themes

		public const string DefaultThemeName = ThemesManager.MetroLightThemeName;

		private SortedDictionary<string, ThemeBase> mTextEditorThemes = null;
		private ObservableCollection<ThemeBase> mListOfAllThemes = null;
		private string mSelectedThemeName = string.Empty;
		#endregion fields

		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public ThemesManager()
		{
			this.mSelectedThemeName = ThemesManager.DefaultThemeName;
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Get the name of the currently seelcted theme.
		/// </summary>
		public string SelectedThemeName
		{
			get
			{
				return this.mSelectedThemeName;
			}
		}

		/// <summary>
		/// Get the object that has links to all resources for the currently selected WPF theme.
		/// </summary>
		public ThemeBase SelectedTheme
		{
			get
			{
				if (this.mTextEditorThemes == null || this.mListOfAllThemes == null)
					this.BuildThemeCollections();

				ThemeBase theme;
				this.mTextEditorThemes.TryGetValue(this.mSelectedThemeName, out theme);

				// Fall back to default if all else fails
				if (theme == null)
				{
					this.mTextEditorThemes.TryGetValue(ThemesManager.DefaultThemeName, out theme);
					this.mSelectedThemeName = theme.HlThemeName;
				}

				return theme;
			}
		}

		/// <summary>
		/// Get a list of all available themes (This property can typically be used to bind
		/// menuitems or other resources to let the user select a theme in the user interface).
		/// </summary>
		public ObservableCollection<ThemeBase> ListAllThemes
		{
			get
			{
				if (this.mTextEditorThemes == null || this.mListOfAllThemes == null)
					this.BuildThemeCollections();

				return this.mListOfAllThemes;
			}
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Change the WPF/EditorHighlightingTheme to the <paramref name="themeName"/> theme.
		/// </summary>
		/// <param name="themeName"></param>
		/// <returns>True if new theme is succesfully selected (was available), otherwise false</returns>
		public bool SetSelectedTheme(string themeName)
		{
			if (this.mTextEditorThemes == null || this.mListOfAllThemes == null)
				this.BuildThemeCollections();

			ThemeBase theme;
			this.mTextEditorThemes.TryGetValue(themeName, out theme);

			// Fall back to default if all else fails
			if (theme == null)
				return false;

			this.mSelectedThemeName = themeName;

			return true;
		}

		/// <summary>
		/// Build sorted dictionary and observable collection for WPF themes.
		/// </summary>
		private void BuildThemeCollections()
		{
			this.mTextEditorThemes = this.BuildThemeDictionary();
			this.mListOfAllThemes = new ObservableCollection<ThemeBase>();

			foreach (KeyValuePair<string, ThemeBase> t in this.mTextEditorThemes)
			{
				this.mListOfAllThemes.Add(t.Value);
			}
		}

		/// <summary>
		/// Build a sorted structure of all default themes and their resources.
		/// </summary>
		/// <returns></returns>
		private SortedDictionary<string, ThemeBase> BuildThemeDictionary()
		{
			SortedDictionary<string, ThemeBase> ret = new SortedDictionary<string, ThemeBase>();

			ThemeBase t = null;
			string themeName = null;
			List<string> wpfTheme = null;

			try
			{
				string appLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

				// Metro Dark Theme
				themeName = MetroDarkThemeName;
				wpfTheme = new List<string>(MetroDarkResources);

				t = new ThemeBase(this, wpfTheme, themeName, null, null, null);
				ret.Add(t.HlThemeName, t);

				// Generic Theme
				themeName = GenericThemeName;
				wpfTheme = new List<string>(GenericResources);

				t = new ThemeBase(this, wpfTheme, themeName, null, null, null);
				ret.Add(t.HlThemeName, t);

				// Metro Light Theme
				themeName = MetroLightThemeName;
				wpfTheme = new List<string>(MetroResources);

				t = new ThemeBase(this, wpfTheme, themeName, null, null, null);
				ret.Add(t.HlThemeName, t);
			}
			catch (System.Exception exp)
			{
				string msg = string.Format("Error registering application theme '{0}' -> '{1}'",
																		themeName == null ? "(null)" : themeName,
																		t == null ? "(null)" : t.HlThemeName);

				// Log an error message and let the system boot up with default theme instead of re-throwing this
				MessageBox.Show(msg, "An error occured", MessageBoxButton.OK);
			}

			return ret;
		}
		#endregion methods
	}
}
