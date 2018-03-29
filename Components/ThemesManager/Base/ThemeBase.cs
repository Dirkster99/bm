namespace Themes.Base
{
    using System.Collections.Generic;
    using Themes.Interfaces;

    internal class ThemeBase : Base.ViewModelBase, IThemeBase
    {
		#region fields
		private IParentSelectedTheme mParent = null;
		#endregion fields

		#region constructor
		/// <summary>
		/// Parameterized constructor
		/// </summary>
		internal ThemeBase(IParentSelectedTheme parent,
											 List<string> resources,
											 string wpfThemeName,
											 string editorThemeName,
											 string editorThemePathLocation,
											 string editorThemeFileName)
			: base()
		{
			this.mParent = parent;
			this.Resources = new List<string>(resources);
			this.WPFThemeName = wpfThemeName;

			this.EditorThemeName = editorThemeName;
			this.EditorThemeFileName = editorThemeFileName;
		}

		/// <summary>
		/// Hidden constructor
		/// </summary>
		protected ThemeBase()
			: base()
		{
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Get a list of Uri formatted resource strings that point to all relevant resources.
		/// </summary>
		public List<string> Resources { get; private set; }

		/// <summary>
		/// WPF Application skin theme (e.g. Metro)
		/// </summary>
		public string WPFThemeName { get; private set; }

		/// <summary>
		/// Get/set the name of the highlighting theme
		/// (eg.: DeepBlack, Bright Standard, True Blue)
		/// </summary>
		public string EditorThemeName { get; private set; }

		/// <summary>
		/// Get/set the location of the current highlighting theme (eg.: C:\DeepBlack.xml)
		/// </summary>
		public string EditorThemeFileName { get; private set; }

		/// <summary>
		/// Get the human read-able name of this WPF/Editor theme.
		/// </summary>
		public string HlThemeName
		{
			get
			{
				return string.Format("{0}{1}", this.WPFThemeName,
																			 this.EditorThemeName == null ? string.Empty : " (" + this.EditorThemeName + ")");
			}
		}

		/// <summary>
		/// Determine whether this theme is currently selected or not.
		/// </summary>
		public bool IsSelected
		{
			get
			{
				if (this.mParent != null)
				{
					if (this.mParent.SelectedThemeName != null)
					{
						return this.mParent.SelectedThemeName.Equals(this.HlThemeName);
					}
				}

				return false;
			}
		}
		#endregion properties

		/// <summary>
		/// Notify binding UI that the IsSelected value may have chnaged.
		/// </summary>
		public void UpdateIsSelected()
		{
			this.NotifyOfPropertyChanged(() => this.IsSelected);
		}
	}
}
