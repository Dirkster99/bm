namespace UnitTestWSF
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WSF;
    using WSF.IDs;
    using System;
    using System.Diagnostics;

    [TestClass]
    public class ChildItemTests
    {
        /// <summary>
        /// Tests if all items below the desktop root can be retrieved without error.
        /// </summary>
        [TestMethod]
        public void GetDesktopChildItems()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int iCnt = 0;
            foreach (var item in Browser.GetChildItems(KF_IID.ID_FOLDERID_Desktop))
            {
                Assert.IsTrue(item != null);
                iCnt++;
            }

            sw.Stop();
            Console.WriteLine("Elapsed Time is:{0}", sw.Elapsed);

            Assert.IsTrue(iCnt > 0);
        }

        /// <summary>
        /// Tests items below ThisPC can be retrieved without error.
        /// </summary>
        [TestMethod]
        public void GetPCChildItems()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int iCnt = 0;
            foreach (var item in Browser.GetChildItems(KF_IID.ID_FOLDERID_ComputerFolder))
            {
                Assert.IsTrue(item != null);
                iCnt++;
            }

            sw.Stop();
            Console.WriteLine("Elapsed Time is:{0}", sw.Elapsed);

            Assert.IsTrue(iCnt > 0);
        }

        /// <summary>
        /// Tests items below Program Files can be retrieved without error.
        /// </summary>
        [TestMethod]
        public void GetProgramFilesChildItems()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int iCnt = 0;
            foreach (var item in Browser.GetChildItems(KF_IID.ID_FOLDERID_ProgramFiles))
            {
                Assert.IsTrue(item != null);
                iCnt++;
            }

            sw.Stop();
            Console.WriteLine("Elapsed Time is:{0}", sw.Elapsed);

            Assert.IsTrue(iCnt > 0);
        }

        /// <summary>
        /// Tests items below Music folder can be retrieved without error.
        /// </summary>
        [TestMethod]
        public void GetMusicChildItems()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int iCnt = 0;
            foreach (var item in Browser.GetChildItems(KF_IID.ID_FOLDERID_Music))
            {
                Assert.IsTrue(item != null);
                iCnt++;
            }

            sw.Stop();
            Console.WriteLine("Elapsed Time is:{0}", sw.Elapsed);

            Assert.IsTrue(iCnt >= 0);
        }

        /// <summary>
        /// Test split drive based sub-directory method to ensure normalized output and
        /// deterministic results.
        /// </summary>
        [TestMethod]
        public void GetOneDriveFolderItem()
        {
            string path1 = @"C:\";
            string path = @"C:";

            var items1 = Browser.GetDirectories(path1);
            var items = Browser.GetDirectories(path);

            Assert.IsTrue(items1.Length == 1);
            Assert.IsTrue(items.Length == 1);

            for (int i = 0; i < items1.Length; i++)
            {
                Assert.IsTrue(string.Compare(items1[i], items[i]) == 0);
            }
        }

        /// <summary>
        /// Test split sub-directory method to ensure normalized output and
        /// deterministic results.
        /// </summary>
        [TestMethod]
        public void GetFolderItems()
        {
            string path1 = @"C:\base\subfolder\";
            string path  = @"C:\base\subfolder";

            var items1 = Browser.GetDirectories(path1);
            var items = Browser.GetDirectories(path);

            Assert.IsTrue(items1.Length == 3);
            Assert.IsTrue(items.Length == 3);

            for (int i = 0; i < items1.Length; i++)
            {
                Assert.IsTrue(string.Compare(items1[i], items[i]) == 0);
            }
        }

        /// <summary>
        /// Test split UNC path sub-directory method to ensure normalized output and
        /// deterministic results.
        /// </summary>
        [TestMethod]
        public void GetNetworkSharedItems()
        {
            string path1 = @"\\myServer\share\subfolder\";
            string path = @"\\myServer\share\subfolder";

            var items1 = Browser.GetDirectories(path1);
            var items = Browser.GetDirectories(path);

            Assert.IsTrue(items1.Length == 3);
            Assert.IsTrue(items.Length == 3);

            for (int i = 0; i < items1.Length; i++)
            {
                Assert.IsTrue(string.Compare(items1[i], items[i]) == 0);
            }
        }
    }
}
