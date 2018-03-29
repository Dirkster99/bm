namespace Themes
{
    using Themes.Interfaces;

    public sealed class Factory
    {
        private Factory(){ }

        public static IThemesManager CreateThemesManager()
        {
            return new ThemesManager();
        }
    }
}
