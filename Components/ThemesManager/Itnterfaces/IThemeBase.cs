namespace Themes.Base
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface IThemeBase : INotifyPropertyChanged
    {
        string EditorThemeFileName { get; }
        string EditorThemeName { get; }
        string HlThemeName { get; }
        bool IsSelected { get; }
        List<string> Resources { get; }
        string WPFThemeName { get; }

        void UpdateIsSelected();
    }
}