namespace SuggestLib.Themes
{
    using System.Windows;

    /// <summary>
    /// Class implements static resource keys that should be referenced to configure
    /// colors, styles and other elements that are typically changed between themes.
    /// </summary>
    public static class ResourceKeys
    {
        /// <summary>
        /// Gets the color key for the normal control enabled background color.
        /// </summary>
        public static readonly ComponentResourceKey ControlNormalBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalBackgroundKey");

        public static readonly ComponentResourceKey PopListBoxControlTemplate = new ComponentResourceKey(typeof(ResourceKeys), "PopListBoxControlTemplate");

        public static readonly ComponentResourceKey PopListBoxScrollViewerControlTemplate = new ComponentResourceKey(typeof(ResourceKeys), "PopListBoxScrollViewerControlTemplate");

        public static readonly ComponentResourceKey ResizeGripStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "ResizeGripStyleKey");       
    }
}
