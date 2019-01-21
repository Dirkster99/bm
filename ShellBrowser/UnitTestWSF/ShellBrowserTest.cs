namespace UnitTestWSF
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WSF;
    using WSF.Enums;
    using WSF.IDs;
    using WSF.Interfaces;
    using System.Linq;

    [TestClass]
    public class ShellBrowserTest
    {
        /// <summary>
        /// Testing SysDefault with DirectoryExists in ShellBrowser
        /// </summary>
        [TestMethod]
        public void TestSysDefault()
        {
            var dir = ShellBrowser.SysDefault;

            Assert.IsTrue(dir != null);

            Assert.IsTrue(ShellBrowser.IsTypeOf(dir.PathFileSystem) == PathType.FileSystemPath);

            IDirectoryBrowser[] pathItems;
            Assert.IsTrue(ShellBrowser.DirectoryExists(dir.PathFileSystem, out pathItems));

            Assert.IsTrue(pathItems != null);
            Assert.IsTrue(pathItems.Length > 0);
        }

        /// <summary>
        /// Tests whether Shellbrowser can parse a Windows Shell Path (sequence of item names)
        /// and whether its existance can be verified based on a string path.
        /// </summary>
        [TestMethod]
        public void TestWinShellPath()
        {
            IDirectoryBrowser libraries = ShellBrowser.Create(KF_IID.ID_FOLDERID_Libraries);
            IDirectoryBrowser music = null;
            Assert.IsTrue(libraries != null);

            foreach (var item in ShellBrowser.GetChildItems(libraries.SpecialPathId))
            {
                if (string.Compare(item.SpecialPathId, KF_IID.ID_FOLDERID_MusicLibrary, true) == 0)
                    music = item;
            }

            Assert.IsTrue(music != null);

            // String evaluates to "Libraries\Music" on an English system
            string WindowsShellPath = string.Format("{0}\\{1}", libraries.Name, music.Name);

            // Check if we can retrieve items for this through dedicated method
            IDirectoryBrowser[] winPath = ShellBrowser.GetWinShellPathItems(WindowsShellPath);

            Assert.IsTrue(winPath != null);
            Assert.IsTrue(winPath.Length == 2);

            Assert.IsTrue(ShellBrowser.IsTypeOf(WindowsShellPath) == PathType.WinShellPath);

            // Check to see if we can retrieve items for this through DirectoryExists
            IDirectoryBrowser[] pathItems;
            bool exists = ShellBrowser.DirectoryExists(WindowsShellPath, out pathItems);

            Assert.IsTrue(pathItems != null);
            Assert.IsTrue(pathItems.Length == 2);
        }

        /// <summary>
        /// Tests whether Shellbrowser can parse a Paths with '\' separators
        /// in normalized form and non-normalized form.
        /// </summary>
        [TestMethod]
        public void TestGetDirectories()
        {
            string testRefPath = @"c:\windows\win32";
            string testPath1 = @"c:windows\win32";

            // Test Path normalization
            var normPath = ShellBrowser.NormalizePath(testPath1);
            Assert.IsTrue(string.Compare(testRefPath, normPath, true) == 0);

            // Test getting directories for normalized and unnormalized paths
            var dirNormItems = ShellBrowser.GetDirectories(normPath);
            Assert.IsTrue(dirNormItems.Length == 3);

            var dirItems = ShellBrowser.GetDirectories(testPath1);
            Assert.IsTrue(dirItems.Length == 3);

            for (int i = 0; i < dirItems.Length; i++)
            {
                Assert.IsTrue(string.Compare(dirItems[i], dirNormItems[i], true) == 0);
            }
        }

        [TestMethod]
        public void TestGetFileSystemPathItems()
        {
            var windowsDir = ShellBrowser.Create(KF_IID.ID_FOLDERID_Windows);
            Assert.IsTrue( windowsDir.DirectoryPathExists() );

            // Test getting directories for normalized and unnormalized paths
            var dirWindowsStringItems = ShellBrowser.GetDirectories(windowsDir.PathFileSystem);

            var dirWindowsItems = ShellBrowser.GetFileSystemPathItems(windowsDir.PathFileSystem);

            Assert.IsTrue(dirWindowsItems.Length == dirWindowsStringItems.Length);

            for (int i = 0; i < dirWindowsItems.Length; i++)
            {
                // The name of a drive can be its label (eg name of 'c:\' drive may be 'Windows')
                // So, we need to explicitely evaluate the PathFileSystem to compare this to strings
                var dirs = ShellBrowser.GetDirectories(dirWindowsItems[i].PathFileSystem);

                Assert.IsTrue(string.Compare(dirs[dirs.Length-1],
                                             dirWindowsStringItems[i], true) == 0);
            }
        }

        /// <summary>
        /// Test whether a string path can be rooted under desktop or ThisPC item.
        /// </summary>
        [TestMethod]
        public void TestFindRoot()
        {
            var sysDefault = ShellBrowser.SysDefault;

            IDirectoryBrowser[] pathItems;
            bool exists = ShellBrowser.DirectoryExists(sysDefault.PathFileSystem, out pathItems);

            Assert.IsTrue(pathItems != null);
            bool pathIsRouted = false;
            var rootedPath = ShellBrowser.FindRoot(pathItems, sysDefault.PathFileSystem, out pathIsRouted);

            Assert.IsTrue(pathIsRouted);

            // Expectation:
            // The rooted Path should contain 'ThisPC' + drive
            // while the sysDefault should be just a drive 'C:\'
            Assert.IsTrue(pathItems.Length < rootedPath.Length);
        }

        /// <summary>
        /// Tests an alternative way for re-rooting items into the windows shell system.
        /// </summary>
        [TestMethod]
        public void TestPathItemsAsIdList()
        {
            var windowsDir = ShellBrowser.Create(KF_IID.ID_FOLDERID_Windows);
            Assert.IsTrue(windowsDir.DirectoryPathExists());

            var items = ShellBrowser.PathItemsAsIdList(windowsDir);

            IDirectoryBrowser[] pathItems;
            bool exists = ShellBrowser.DirectoryExists(windowsDir.PathFileSystem, out pathItems);

            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count > 0);

            foreach (var item in items)
            {
                var dirItem = ShellBrowser.Create(item);
                Assert.IsTrue(dirItem != null);
            }

            // Expectation:
            // The rooted Path should contain 'ThisPC' + drive
            // while the sysDefault should be just a drive 'C:\'
            Assert.IsTrue(pathItems.Length < items.Count);

            // See if FindRoot agrees with what we got
            var rootedItems = ShellBrowser.FindRoot(windowsDir);
            Assert.IsTrue(rootedItems.Length == items.Count);
        }

        /// <summary>
        /// Tests whether a given directory browser object can be converted into a
        /// sequence of parse names to describe the orginal location.
        /// </summary>
        [TestMethod]
        public void TestPathItemsAsParseNames()
        {
            var windowsDir = ShellBrowser.Create(KF_IID.ID_FOLDERID_Windows);
            Assert.IsTrue(windowsDir.DirectoryPathExists());

            var browseItems = ShellBrowser.PathItemsAsIdList(windowsDir);
            var items = ShellBrowser.PathItemsAsParseNames(windowsDir);

            // Expectation: Both lists hold the same path using a different format
            // So, there length should be equal
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count > 0);
            Assert.IsTrue(items.Count == browseItems.Count);

            for (int i = 0; i < items.Count; i++)
            {
                var dirItem = ShellBrowser.Create(items[i]);

                Assert.IsTrue(dirItem != null);
            }
        }

        /// <summary>
        /// Test if a file system parent of a file system path can be determined.
        /// </summary>
        [TestMethod]
        public void TestIsParentOf()
        {
            var windowsDir = ShellBrowser.Create(KF_IID.ID_FOLDERID_Windows);
            Assert.IsTrue(windowsDir.DirectoryPathExists());

            var dirStrings = ShellBrowser.GetDirectories(windowsDir.PathFileSystem);

            string pathExt;
            Assert.IsTrue(ShellBrowser.IsParentPathOf(dirStrings[0], windowsDir.PathFileSystem,
                                                      out pathExt));

            // Alternative 1) way to check for parent relationship between 2 paths
            var result = ShellBrowser.IsCurrentPath(dirStrings[0], windowsDir.PathFileSystem);
            Assert.IsTrue(result == PathMatch.PartialSource);

            // Alternative 2) way to check for parent relationship between 2 paths
            result = ShellBrowser.IsCurrentPath(windowsDir.PathFileSystem, dirStrings[0]);
            Assert.IsTrue(result == PathMatch.PartialTarget);

            // Check if 2 paths are the same
            result = ShellBrowser.IsCurrentPath(windowsDir.PathFileSystem, windowsDir.PathFileSystem);
            Assert.IsTrue(result == PathMatch.CompleteMatch);

            // Check if 2 paths are completely unrelated
            result = ShellBrowser.IsCurrentPath(@"X:\Data\MyPath", @"Y:\Data\MyPath");
            Assert.IsTrue(result == PathMatch.Unrelated);
        }

        /// <summary>
        /// Test if we can join two paths based on <see cref="IDirectoryBrowser"/> and
        /// string path representation.
        /// </summary>
        [TestMethod]
        public void TestFindCommonRoot()
        {
            var windowsDir = ShellBrowser.Create(KF_IID.ID_FOLDERID_Windows);
            Assert.IsTrue(windowsDir.DirectoryPathExists());

            var dirStrings = ShellBrowser.GetDirectories(windowsDir.PathFileSystem);

            string pathExt;
            Assert.IsTrue(ShellBrowser.IsParentPathOf(dirStrings[0], windowsDir.PathFileSystem,
                                                      out pathExt));

            // Alternative 1) way to check for parent relationship between 2 paths
            var result = ShellBrowser.IsCurrentPath(dirStrings[0], windowsDir.PathFileSystem);
            Assert.IsTrue(result == PathMatch.PartialSource);

            var currentPath = ShellBrowser.FindRoot(windowsDir);

            var items = ShellBrowser.FindCommonRoot(currentPath, dirStrings[0], out pathExt);

            Assert.IsTrue(items > 0);
            Assert.IsTrue(pathExt == string.Empty);

            var dirstringsItems = ShellBrowser.FindRoot(ShellBrowser.Create(dirStrings[0]));

            items = ShellBrowser.FindCommonRoot(dirstringsItems, windowsDir.PathFileSystem , out pathExt);

            Assert.IsTrue(items > 0);
            Assert.IsTrue(string.IsNullOrEmpty(pathExt) == false);

            var dirstringsList = dirstringsItems.ToList();

            bool bresult = ShellBrowser.ExtendPath(ref dirstringsList, pathExt);

            Assert.IsTrue(bresult);
            Assert.IsTrue(dirstringsItems.Length < dirstringsList.Count);
        }
    }
}
