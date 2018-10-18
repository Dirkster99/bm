namespace TestShellBrowserLib
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ShellBrowserLib;
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.SharpShell.Pidl;
    using System;
    using System.IO;

    /// <summary>
    /// Tests a method for generating a path for a <see cref="IDirectoryBrowser"/>
    /// object - the path contains <see cref="idList"/> items which can be used
    /// to re-create each <see cref="IDirectoryBrowser"/> item along the path.
    /// </summary>
    [TestClass]
    public class PIDL_PathTests
    {
        [TestMethod]
        public void TestDesktopPath()
        {
            var testitem = ShellBrowser.Create(KF_IID.ID_FOLDERID_Desktop);

            var pathItems = ShellBrowser.GetPathItems(testitem);
            foreach (var item in pathItems)
            {
                string name = PidlManager.GetPidlDisplayName(item);
                Console.WriteLine(name);

                var browserItem = ShellBrowser.Create(item);
                Assert.IsTrue(browserItem != null);
            }

            // Should not contain any idList whatsoever
            Assert.IsTrue(pathItems.Count == 0);
        }

        [TestMethod]
        public void TestPCPath()
        {
            var testitem = ShellBrowser.Create(KF_IID.ID_FOLDERID_ComputerFolder);

            var pathItems = ShellBrowser.GetPathItems(testitem);
            foreach (var item in pathItems)
            {
                string name = PidlManager.GetPidlDisplayName(item);
                Console.WriteLine(name);

                var browserItem = ShellBrowser.Create(item);
                Assert.IsTrue(browserItem != null);
            }

            // Should contain the fullpidl to 'This PC'
            Assert.IsTrue(pathItems.Count == 1);
        }

        [TestMethod]
        public void TestDrivePath()
        {
            // Get the default drive's path
            var drivePath = new DirectoryInfo(Environment.SystemDirectory).Root.Name;
            var driveInfoPath = new System.IO.DriveInfo(drivePath);

            Assert.IsTrue(drivePath != null);

            var testitem = ShellBrowser.Create(drivePath);

            var pathItems = ShellBrowser.GetPathItems(testitem);
            foreach (var item in pathItems)
            {
                string name = PidlManager.GetPidlDisplayName(item);
                Console.WriteLine(name);

                var browserItem = ShellBrowser.Create(item);
                Assert.IsTrue(browserItem != null);
            }

            // Should contain the fullpidl to 'This PC', '<Drive (eg.: C:)>'
            Assert.IsTrue(pathItems.Count == 2);
        }

        [TestMethod]
        public void TestMusicPath()
        {
            var testitem = ShellBrowser.Create(KF_IID.ID_FOLDERID_Music);

            var pathItems = ShellBrowser.GetPathItems(testitem);
            foreach (var item in pathItems)
            {
                string name = PidlManager.GetPidlDisplayName(item);
                Console.WriteLine(name);

                var browserItem = ShellBrowser.Create(item);
                Assert.IsTrue(browserItem != null);
            }

            // Should contain the fullpidl to 'This PC', 'Music'
            Assert.IsTrue(pathItems.Count == 2);
        }

        [TestMethod]
        public void TestWindowsPath()
        {
            var testitem = ShellBrowser.Create(KF_IID.ID_FOLDERID_Windows);

            var pathItems = ShellBrowser.GetPathItems(testitem);
            foreach (var item in pathItems)
            {
                string name = PidlManager.GetPidlDisplayName(item);
                Console.WriteLine(name);

                var browserItem = ShellBrowser.Create(item);
                Assert.IsTrue(browserItem != null);
            }

            // Should contain the fullpidls to 'This PC', '<Drive (eg.: C:)>', 'Windows'
            Assert.IsTrue(pathItems.Count == 3);
        }

        [TestMethod]
        public void TestFontsPath()
        {
            var testitem = ShellBrowser.Create(KF_IID.ID_FOLDERID_Fonts);

            var pathItems = ShellBrowser.GetPathItems(testitem);
            foreach (var item in pathItems)
            {
                string name = PidlManager.GetPidlDisplayName(item);
                Console.WriteLine(name);

                var browserItem = ShellBrowser.Create(item);
                Assert.IsTrue(browserItem != null);
            }

            // Should contain the fullpidls to 'This PC', '<Drive (eg.: C:)>', 'Windows'. 'Fonts'
            Assert.IsTrue(pathItems.Count == 4);
        }
    }
}
