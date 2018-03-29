namespace Themes.Interfaces
{
	using System.Collections.ObjectModel;
	using Themes.Base;

	public interface IThemesManager
	{
		#region properties
		/// <summary>
		/// Get the name of the currently seelcted theme.
		/// </summary>
		string SelectedThemeName { get; }

		/// <summary>
		/// Get the object that has links to all resources for the currently selected WPF theme.
		/// </summary>
		IThemeBase SelectedTheme { get; }

        string DefaultThemeName { get; }

        /// <summary>
        /// Get a list of all available themes (This property can typically be used to bind
        /// menuitems or other resources to let the user select a theme in the user interface).
        /// </summary>
        ObservableCollection<IThemeBase> ListAllThemes { get; }
		#endregion properties

		#region methods
    /// <summary>
    /// Change the WPF/EditorHighlightingTheme to the <paramref name="themeName"/> theme.
    /// </summary>
    /// <param name="themeName"></param>
    /// <returns>True if new theme is succesfully selected (was available), otherwise false</returns>
    bool SetSelectedTheme(string themeName);
		#endregion methods
	}
}
