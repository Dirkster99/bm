namespace BreadcrumbLib.Defines
{
    public enum HierarchicalResult : int
    {
        Parent = 1 << 1,
        Current = 1 << 2,
        Child = 1 << 3,
        Unrelated = 1 << 4,
        Related = Parent | Current | Child,
        All = Related | Unrelated
    };

    public enum TransferMode { Copy, Move, Rename, Link }

    /// <summary>
    /// If a file already exists, change name... use IFileNameGenerator
    /// </summary>
    public enum NameGenerationMode { NoRename, Rename }

    //ToAdd a template, you have to update DisplayTemplateSelector.cs
    public enum DisplayType { Auto, Text, Number, Link, DateTime, TimeElapsed, Kb, Percent, Filename, Boolean, Image }
    public enum UpdateMode { Replace, Update }

    public enum ChangeType { Changed, Created, Deleted, Moved }
/***
    public enum UIInputType { None, Mouse, MouseLeft, MouseRight, Touch, Stylus }
    public enum UIInputState { NotApplied, Pressed, Released }
    public enum UITouchGesture { NotApplied, FlickLeft, FlickRight, FlickUp, FlickDown }

    public enum ZoomMode { ZoomIn, ZoomOut }
    public enum ViewMode : int
    {
        vmTile = 13,
        vmGrid = 14,
        vmList = 15,
        vmSmallIcon = 16,
        vmIcon = 48,
        vmLargeIcon = 80,
        vmExtraLargeIcon = 120,
        vmViewer = 256
    }

    public enum DragMode
    {
        None, //Not dragging.
        DoDragDrop, //By calling DoDragDrop(), default mode. 
        Lite //By setting DraggingDataObject
    }

    public enum SelectionModeEx : int
    {
        Single = 1,
        Multiple,
        Extended,
        SelectionHelper
    }
***/

    public enum FileAccess
    {
        Read, ReadWrite, Write
    }
/***
    public static class ShellClipboardFormats
    {
        public static string CFSTR_AUTOPLAY_SHELLIDLISTS = "Autoplay Enumerated IDList Array";
        public static string CFSTR_DRAGCONTEXT = "DragContext";
        public static string CFSTR_FILECONTENTS = "FileContents";
        public static string CFSTR_FILEDESCRIPTORA = "FileGroupDescriptor";
        public static string CFSTR_FILEDESCRIPTORW = "FileGroupDescriptorW";
        public static string CFSTR_FILENAMEA = "FileName";
        public static string CFSTR_FILENAMEMAPA = "FileNameMap";
        public static string CFSTR_FILENAMEMAPW = "FileNameMapW";
        public static string CFSTR_FILENAMEW = "FileNameW";
        public static string CFSTR_INDRAGLOOP = "InShellDragLoop";
        public static string CFSTR_INETURLA = "UniformResourceLocator";
        public static string CFSTR_INETURLW = "UniformResourceLocatorW";
        public static string CFSTR_LOGICALPERFORMEDDROPEFFECT = "Logical Performed DropEffect";
        public static string CFSTR_MOUNTEDVOLUME = "MountedVolume";
        public static string CFSTR_NETRESOURCES = "Net Resource";
        public static string CFSTR_PASTESUCCEEDED = "Paste Succeeded";
        public static string CFSTR_PERFORMEDDROPEFFECT = "Performed DropEffect";
        public static string CFSTR_PERSISTEDDATAOBJECT = "PersistedDataObject";
        public static string CFSTR_PREFERREDDROPEFFECT = "Preferred DropEffect";
        public static string CFSTR_PRINTERGROUP = "PrinterFreindlyName";
        public static string CFSTR_SHELLIDLIST = "Shell IDList Array";
        public static string CFSTR_SHELLIDLISTOFFSET = "Shell Object Offsets";
        public static string CFSTR_SHELLURL = "UniformResourceLocator";
        public static string CFSTR_TARGETCLSID = "TargetCLSID";
    }

    [Flags]
    public enum ListOptions
    {
        /// <summary>
        /// List folder 
        /// </summary>
        Folder = 1 << 0,
        /// <summary>
        /// List file
        /// </summary>
        File = 1 << 1,
        /// <summary>
        /// Include items in sub-folder.
        /// </summary>
        Recrusive = 1 << 2
    }
    ***/

}
