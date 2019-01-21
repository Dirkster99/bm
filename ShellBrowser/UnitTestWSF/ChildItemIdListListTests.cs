namespace UnitTestWSF
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WSF;
    using WSF.IDs;
    using WSF.Interfaces;
    using WSF.Shell.Interop.Interfaces.Knownfolders;
    using WSF.Shell.Interop.Knownfolders;
    using WSF.Shell.Pidl;

    [TestClass]
    public class ChildItemIdListListTests
    {
        /// <summary>
        /// Tests if all items below the desktop root can be retrieved without error.
        /// </summary>
        [TestMethod]
        public void GetDesktopChildItems()
        {
            int iCnt = 0;
            foreach (var item in Browser.GetChildItems(KF_IID.ID_FOLDERID_Desktop))
            {
                // First item is null if we are looking at an item under the desktop item
                var fullIdList = (item.ParentIdList != null ?
                                  PidlManager.Combine(item.ParentIdList, item.ChildIdList) :
                                  item.ChildIdList);

                IKnownFolderProperties props = null;
                using (var kf = KnownFolderHelper.FromPIDL(fullIdList))
                {
                    if (kf != null)
                        props = KnownFolderHelper.GetFolderProperties(kf.Obj);
                }

                Assert.IsTrue(item != null);
                iCnt++;
            }

            Assert.IsTrue(iCnt > 0);
        }

        /// <summary>
        /// Tests items below ThisPC can be retrieved without error.
        /// </summary>
        [TestMethod]
        public void GetPCChildItems()
        {
            int iCnt = 0;
            foreach (var item in Browser.GetChildItems(KF_IID.ID_FOLDERID_ComputerFolder))
            {
                // First item is null if we are looking at an item under the desktop item
                var fullIdList = (item.ParentIdList != null ?
                                  PidlManager.Combine(item.ParentIdList, item.ChildIdList) :
                                  item.ChildIdList);

                IKnownFolderProperties props = null;
                using (var kf = KnownFolderHelper.FromPIDL(fullIdList))
                {
                    if (kf != null)
                        props = KnownFolderHelper.GetFolderProperties(kf.Obj);
                }

                Assert.IsTrue(item != null);
                iCnt++;
            }

            Assert.IsTrue(iCnt > 0);
        }

        /// <summary>
        /// Tests items below Program Files can be retrieved without error.
        /// </summary>
        [TestMethod]
        public void GetProgramFilesChildItems()
        {
            int iCnt = 0;
            foreach (var item in Browser.GetChildItems(KF_IID.ID_FOLDERID_ProgramFiles))
            {
                Assert.IsTrue(item != null);
                iCnt++;
            }
        }

        /// <summary>
        /// Tests the filter enumeration parameter which should currently result in exactly one match.
        /// </summary>
        [TestMethod]
        public void GetFilteredProgramFilesChildItem()
        {
            IDirectoryBrowser filterReferenceItem = null;
            int iCnt = 0;
            foreach (var item in Browser.GetChildItems(KF_IID.ID_FOLDERID_ProgramFiles))
            {
                Assert.IsTrue(item != null);
                filterReferenceItem = item;

                iCnt++;
            }

            // Filtering for an arbitrary item in the programm files folder should yield
            // a collection with one item (assuming that all item names are unique)
            int iCntFiltered = 0;
            foreach (var item in Browser.GetChildItems(KF_IID.ID_FOLDERID_ProgramFiles,
                                                            filterReferenceItem.Name))
            {
                Assert.IsTrue(item != null);
                iCntFiltered++;
            }

            Assert.IsTrue(iCntFiltered == 1);

            // Filtering for an arbitrary '*' item in the programm files folder should yield
            // a collection with one item (assuming that all item names are unique)
            iCntFiltered = 0;
            foreach (var item in Browser.GetChildItems(KF_IID.ID_FOLDERID_ProgramFiles,
                                                            "*" + filterReferenceItem.Name + "*"))
            {
                Assert.IsTrue(item != null);
                iCntFiltered++;
            }

            Assert.IsTrue(iCntFiltered == 1);
        }

        /// <summary>
        /// Tests items below Music folder can be retrieved without error.
        /// </summary>
        [TestMethod]
        public void GetMusicChildItems()
        {
            int iCnt = 0;
            foreach (var item in Browser.GetChildItems(KF_IID.ID_FOLDERID_Music))
            {
                Assert.IsTrue(item != null);
                iCnt++;
            }
        }
    }
}
