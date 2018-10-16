namespace TestShellBrowserLib
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ShellBrowserLib;
    using ShellBrowserLib.IDs;
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
            foreach (var item in ShellBrowser.GetChildItems(KF_IID.ID_FOLDERID_Desktop))
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
            foreach (var item in ShellBrowser.GetChildItems(KF_IID.ID_FOLDERID_ComputerFolder))
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
            foreach (var item in ShellBrowser.GetChildItems(KF_IID.ID_FOLDERID_ProgramFiles))
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
            foreach (var item in ShellBrowser.GetChildItems(KF_IID.ID_FOLDERID_Music))
            {
                Assert.IsTrue(item != null);
                iCnt++;
            }

            sw.Stop();
            Console.WriteLine("Elapsed Time is:{0}", sw.Elapsed);

            Assert.IsTrue(iCnt > 0);
        }
    }
}
