namespace ThemedDemo.Demos.ViewModels
{
    using DirectoryInfoExLib;
    using DirectoryInfoExLib.Enums;
    using DirectoryInfoExLib.IO.Header.KnownFolder;

    public class DemoViewModel : BreadcrumbTestLib.ViewModels.AppViewModel
    {
        #region private fields
        #endregion private fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DemoViewModel()
            : base ("Themed Demo")
        {
            // Testing
            var test = Factory.FromString(@"C:\");
            var test1 = Factory.FromString(@"C:\TMP");
            var desktopPath = Factory.FromString(@"C:\Users\NOP\Desktop");
            var test2 = Factory.MyComputer;
            var test3 = Factory.CurrentUserDirectory;
            var test4 = Factory.Network;
            var test5 = Factory.RecycleBin;
            var test6 = Factory.SharedDirectory;
            var test7 = Factory.DesktopDirectory;

            var controlPanel = Factory.CreateDirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.ControlPanel));
            var documentsLibrary = Factory.CreateDirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.DocumentsLibrary));
            var documents = Factory.CreateDirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Documents));

            var desktopRoot = Factory.CreateDirectoryInfoEx(test2.Parent.FullName);
        }
        #endregion constructors

        #region properties
        #endregion properties

        #region methods
        #endregion methods
    }
}
