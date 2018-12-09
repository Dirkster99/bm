namespace SuggestLib.Themes
{
    using System.Windows;

    /// <summary>
    /// Class implements static resource keys that should be referenced to configure
    /// colors, styles and other elements that are typically changed between themes.
    /// </summary>
    public static class ResourceKeys
    {
        #region Accent Keys
        /// <summary>
        /// Accent Color Key - This Color key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentColorKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentColorKey");

        /// <summary>
        /// Accent Brush Key - This Brush key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentBrushKey");
        #endregion Accent Keys

        #region styles and templates
        /// <summary>
        /// Gets the Style of the ListBox in the pop-up element of the suggest box.
        /// </summary>
        public static readonly ComponentResourceKey PopListBoxControlTemplate = new ComponentResourceKey(typeof(ResourceKeys), "PopListBoxControlTemplate");

        /// <summary>
        /// Gets the Style of the ScrollViewer in the pop-up element of the suggest box.
        /// 
        /// https://www.codeproject.com/Tips/1271095/A-Custom-WPF-ScrollViewer
        /// </summary>
        public static readonly ComponentResourceKey PopListBoxScrollViewerControlTemplate = new ComponentResourceKey(typeof(ResourceKeys), "PopListBoxScrollViewerControlTemplate");

        /// <summary>
        /// Gets the Style of the ResizeGrip in the pop-up element of the suggest box.
        /// </summary>
        public static readonly ComponentResourceKey ResizeGripStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "ResizeGripStyleKey");
        #endregion styles and templates

        /// <summary>
        /// Gets a the applicable foreground Brush key that should be used for coloring text.
        /// </summary>
        public static readonly ComponentResourceKey ControlTextBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlTextBrushKey");

        /// <summary>
        /// Gets a background Brush key of an input control.
        /// </summary>
        public static readonly ComponentResourceKey ControlInputBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlInputBackgroundKey");

        /// <summary>
        /// Gets a border Brush key of an input control.
        /// </summary>
        public static readonly ComponentResourceKey ControlInputBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlInputBorderKey");

        #region Pop-Up
        /// <summary>
        /// Gets a background Color key of a Pop-Up control.
        /// </summary>
        public static readonly ComponentResourceKey ControlPopupBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlPopupBackgroundKey");

        /// <summary>
        /// Gets a background Brush key of a Pop-Up control.
        /// </summary>
        public static readonly ComponentResourceKey ControlPopupBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlPopupBackgroundBrushKey");

        // Color definitions for pop-up ResizeThumb dragging area
        #region Popup ResizeThumb Item
        /// <summary>
        /// Gets the background Brush key of the thumb that is used to resize
        /// the pop-up element of the suggestbox.
        /// </summary>
        public static readonly ComponentResourceKey ResizeThumbBackgroundDraggingKey = new ComponentResourceKey(typeof(ResourceKeys), "ResizeThumbBackgroundDraggingKey");
        #endregion Popup ResizeThumb Item
        #endregion Pop-Up
    }
}
