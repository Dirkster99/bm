namespace UnitTestWSF
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WSF;
    using WSF.IDs;
    using WSF.Shell.Interop.ShellFolders;
    using WSF.Shell.Pidl;
    using WSF.Shell.Enums;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

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
            // Get test browser object and generate path list of idListPidls
            var testitem = Browser.Create(KF_IID.ID_FOLDERID_Desktop);
            var pathItems = Browser.PathItemsAsIdList(testitem);

            // Desktop has no children or parent pidls since its the root of it all
            Assert.IsTrue(testitem.ParentIdList == null);
            Assert.IsTrue(testitem.ChildIdList == null);

            // Should not contain any idList whatsoever
            Assert.IsTrue(pathItems.Count == 0);
        }

        [TestMethod]
        public void TestPCPath()
        {
            // Get test browser object and generate path list of idListPidls
            var testitem = Browser.Create(KF_IID.ID_FOLDERID_ComputerFolder);
            var pathItems = Browser.PathItemsAsIdList(testitem);

            // Should contain the fullpidl to 'This PC'
            Assert.IsTrue(pathItems.Count == 1);

            foreach (var item in pathItems)
            {
                string displayName = PidlManager.IdListFullToName(item, SHGDNF.SHGDN_NORMAL);
                string parseName = PidlManager.IdListFullToName(item, SHGDNF.SHGDN_FORPARSING);

                Console.WriteLine("Display Name '{0}' Parse Name '{1}'", displayName, parseName);

                Assert.IsFalse(string.IsNullOrEmpty(parseName));
                Console.WriteLine(parseName);

                var browserItem = Browser.Create(parseName);
                Assert.IsTrue(browserItem != null);

                Assert.IsTrue(browserItem.EqualsParseName(parseName));

                var browserItem1 = Browser.Create(item);
                Assert.IsTrue(browserItem1 != null);

                // Object from PIDL and ParseName should realy describe same location
                Assert.IsTrue(browserItem1.Equals(browserItem));
            }
        }

        [TestMethod]
        public void TestDrivePath()
        {
            // Get test browser object and generate path list of idListPidls
            // Get the default drive's path
            var drivePath = new DirectoryInfo(Environment.SystemDirectory).Root.Name;
            var driveInfoPath = new System.IO.DriveInfo(drivePath);

            Assert.IsTrue(drivePath != null);

            var testitem = Browser.Create(drivePath);
            var pathItems = Browser.PathItemsAsIdList(testitem);

            // Should contain the fullpidl to 'This PC', '<Drive (eg.: C:)>'
            Assert.IsTrue(pathItems.Count == 2);

            foreach (var item in pathItems)
            {
                string displayName = PidlManager.IdListFullToName(item, SHGDNF.SHGDN_NORMAL);
                string parseName = PidlManager.IdListFullToName(item, SHGDNF.SHGDN_FORPARSING);

                Console.WriteLine("Display Name '{0}' Parse Name '{1}'", displayName, parseName);

                Assert.IsFalse(string.IsNullOrEmpty(parseName));
                Console.WriteLine(parseName);

                var browserItem = Browser.Create(parseName);
                Assert.IsTrue(browserItem != null);

                Assert.IsTrue(browserItem.EqualsParseName(parseName));

                var browserItem1 = Browser.Create(item);
                Assert.IsTrue(browserItem1 != null);

                // Object from PIDL and ParseName should realy describe same location
                Assert.IsTrue(browserItem1.Equals(browserItem));
            }
        }

        [TestMethod]
        public void TestMusicPath()
        {
            // Get test browser object and generate path list of idListPidls
            var testitem = Browser.Create(KF_IID.ID_FOLDERID_Music);
            var pathItems = Browser.PathItemsAsIdList(testitem);

            // Should contain the fullpidl to 'This PC', 'Music'
            Assert.IsTrue(pathItems.Count == 2);

            foreach (var item in pathItems)
            {
                string displayName = PidlManager.IdListFullToName(item, SHGDNF.SHGDN_NORMAL);
                string parseName = PidlManager.IdListFullToName(item, SHGDNF.SHGDN_FORPARSING);

                Console.WriteLine("Display Name '{0}' Parse Name '{1}'", displayName, parseName);

                Assert.IsFalse(string.IsNullOrEmpty(parseName));
                Console.WriteLine(parseName);

                var browserItem = Browser.Create(parseName, true);
                Assert.IsTrue(browserItem != null);

                Assert.IsTrue(browserItem.EqualsParseName(parseName));

                var browserItem1 = Browser.Create(item);
                Assert.IsTrue(browserItem1 != null);

                // Object from PIDL and ParseName should realy describe same location
                Assert.IsTrue(browserItem1.Equals(browserItem));
            }
        }

        [TestMethod]
        public void TestWindowsPath()
        {
            // Get test browser object and generate path list of idListPidls
            var testitem = Browser.Create(KF_IID.ID_FOLDERID_Windows);
            var pathItems = Browser.PathItemsAsIdList(testitem);

            // Should contain the fullpidls to 'This PC', '<Drive (eg.: C:)>', 'Windows'
            Assert.IsTrue(pathItems.Count == 3);

            foreach (var item in pathItems)
            {
                string displayName = PidlManager.IdListFullToName(item, SHGDNF.SHGDN_NORMAL);
                string parseName = PidlManager.IdListFullToName(item, SHGDNF.SHGDN_FORPARSING);

                Console.WriteLine("Display Name '{0}' Parse Name '{1}'", displayName, parseName);

                Assert.IsFalse(string.IsNullOrEmpty(parseName));
                Console.WriteLine(parseName);

                var browserItem = Browser.Create(parseName,true);
                Assert.IsTrue(browserItem != null);

                Assert.IsTrue(browserItem.EqualsParseName(parseName));

                var browserItem1 = Browser.Create(item, true);
                Assert.IsTrue(browserItem1 != null);

                // Object from PIDL and ParseName should realy describe same location
                Assert.IsTrue(browserItem1.Equals(browserItem));
            }
        }

        [TestMethod]
        public void TestFontsPath()
        {
            // Get test browser object and generate path list of idListPidls
            var testitem = Browser.Create(KF_IID.ID_FOLDERID_Fonts);
            var pathItems = Browser.PathItemsAsIdList(testitem);

            // Should contain the fullpidls to 'This PC', '<Drive (eg.: C:)>', 'Windows'. 'Fonts'
            Assert.IsTrue(pathItems.Count == 4);

            foreach (var item in pathItems)
            {
                string displayName = PidlManager.IdListFullToName(item, SHGDNF.SHGDN_NORMAL);
                string parseName = PidlManager.IdListFullToName(item, SHGDNF.SHGDN_FORPARSING);

                Console.WriteLine("Display Name '{0}' Parse Name '{1}'", displayName, parseName);

                Assert.IsFalse(string.IsNullOrEmpty(parseName));
                Console.WriteLine(parseName);

                var browserItem = Browser.Create(parseName,true);
                Assert.IsTrue(browserItem != null);

                Assert.IsTrue(browserItem.EqualsParseName(parseName));

                var browserItem1 = Browser.Create(item);
                Assert.IsTrue(browserItem1 != null);

                // Object from PIDL and ParseName should realy describe same location
                Assert.IsTrue(browserItem1.Equals(browserItem));
            }
        }
    }
}
