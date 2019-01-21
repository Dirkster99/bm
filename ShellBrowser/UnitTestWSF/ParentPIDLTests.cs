namespace UnitTestWSF
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WSF;
    using WSF.IDs;
    using WSF.Shell.Interop.Knownfolders;
    using WSF.Shell.Pidl;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains tests that verify whethe parent items in the
    /// shell system can be identified via <see cref="List{ShellId}"/>.
    /// </summary>
    [TestClass]
    public class ParentPIDLTests
    {
        /// <summary>
        /// Verifies whether we can get the parent for a file system directory
        /// (in this case a directory below the known 'Windows' folder)
        /// via <see cref="List{ShellId}"/> class (alternative pidl representation).
        /// </summary>
        [TestMethod]
        public void GetStorageLocationForSpecialFolderParent()
        {
            string originalPath = null;
            IntPtr ptrPath = default(IntPtr);
            try
            {
                using (var kfn = KnownFolderHelper.FromKnownFolderGuid(new Guid(KF_ID.ID_FOLDERID_Windows)))
                {
                    kfn.Obj.GetPath(0, out ptrPath);

                    Assert.IsTrue(ptrPath != default(IntPtr));

                    originalPath = Marshal.PtrToStringUni(ptrPath);
                }

                Assert.IsFalse(string.IsNullOrEmpty(originalPath));

                string testFolder = System.IO.Path.Combine(originalPath, "System32");
                Assert.IsTrue(System.IO.Directory.Exists(testFolder));

                IdList parentIdList = null, relativeChild = null;
                var retVal = PidlManager.GetParentIdListFromPath(testFolder, out parentIdList, out relativeChild);

                Assert.IsTrue(relativeChild != null);
                Assert.IsTrue(relativeChild.Size == 1);

                IntPtr parentPIDL = PidlManager.IdListToPidl(IdList.Create(parentIdList.Ids));
                try
                {
                    Assert.IsFalse(parentPIDL == default(IntPtr));

                    string path = PidlManager.GetPathFromPIDL(parentPIDL);

                    Assert.IsTrue(retVal == true);
                    Assert.IsFalse(parentIdList == null);

                    // Expectation: Should display a path like 'C:\' or special folder path
                    Assert.IsFalse(string.IsNullOrEmpty(testFolder));
                    Assert.IsFalse(testFolder.Equals(path, StringComparison.InvariantCultureIgnoreCase));

                    // The expected parent of a drive is 'This PC'
                    Assert.IsTrue(path.Equals(KF_IID.ID_FOLDERID_Windows, StringComparison.InvariantCultureIgnoreCase));
                }
                finally
                {
                    parentPIDL = PidlManager.FreeCoTaskMem(parentPIDL);
                }
            }
            finally
            {
                ptrPath = PidlManager.FreeCoTaskMem(ptrPath);
            }
        }

        /// <summary>
        /// Verifies that items (drives and known folders) that should be under
        /// 'This PC' are found via parent lookup using the
        /// <see cref="List{ShellId}"/> class (alternative pidl representation).
        /// </summary>
        [TestMethod]
        public void GetLogicalLocationForThisPCParentOf()
        {
            // Get the default drive's path
            var drive = new DirectoryInfo(Environment.SystemDirectory).Root.Name;

            Assert.IsFalse(string.IsNullOrEmpty(drive));

            var list = new List<string>();

            list.Add(drive);
            list.Add(KF_IID.ID_FOLDERID_Documents);
            list.Add(KF_IID.ID_FOLDERID_Music);
            list.Add(KF_IID.ID_FOLDERID_Downloads);
            list.Add(KF_IID.ID_FOLDERID_Pictures);
            list.Add(KF_IID.ID_FOLDERID_Videos);

            foreach (var testFolder in list)
            {
                IdList parentIdList = null, relativeChild = null;
                var retVal = PidlManager.GetParentIdListFromPath(testFolder, out parentIdList, out relativeChild);

                Assert.IsTrue(relativeChild != null);
                Assert.IsTrue(relativeChild.Size == 1);

                Assert.IsTrue(retVal == true);
                Assert.IsFalse(parentIdList == null);

                IntPtr parentPIDL = PidlManager.IdListToPidl(IdList.Create(parentIdList.Ids));
                try
                {
                    Assert.IsFalse(parentPIDL == default(IntPtr));

                    string path = PidlManager.GetPathFromPIDL(parentPIDL);

                    // Expectation: Should display a path like 'C:\' or special folder path
                    Assert.IsFalse(string.IsNullOrEmpty(path));
                    Assert.IsFalse(testFolder.Equals(path, StringComparison.InvariantCultureIgnoreCase));

                    // The expected parent of a drive is 'This PC'
                    Assert.IsTrue(path.Equals(KF_IID.ID_FOLDERID_ComputerFolder, StringComparison.InvariantCultureIgnoreCase));

                    // this works for display name return PidlManager.GetPidlDisplayName(parentPIDL);
                }
                finally
                {
                    parentPIDL = PidlManager.FreeCoTaskMem(parentPIDL);
                }
            }
        }

        /// <summary>
        /// Verifies that the expected parent of 'This PC' is indeed the Desktop
        /// via <see cref="List{ShellId}"/> class (alternative pidl representation).
        /// /// </summary>
        [TestMethod]
        public void GetLogicalLocationForDesktopParentOf()
        {
            var testitem = KF_IID.ID_FOLDERID_ComputerFolder;

            IdList parentIdList = null, relativeChild = null;
            var retVal = PidlManager.GetParentIdListFromPath(testitem, out parentIdList, out relativeChild);

            Assert.IsTrue(relativeChild != null);
            Assert.IsTrue(relativeChild.Size == 1);

            Assert.IsTrue(retVal == true);
            Assert.IsFalse(parentIdList == null);
            Assert.IsTrue(parentIdList.Size == 0);  // An empty list represents the desktop

            IntPtr parentPIDL = PidlManager.IdListToPidl(IdList.Create(parentIdList.Ids));
            try
            {
                Assert.IsFalse(parentPIDL == default(IntPtr));

                string path = PidlManager.GetPathFromPIDL(parentPIDL);

                Assert.IsFalse(string.IsNullOrEmpty(path));
                Assert.IsFalse(testitem.Equals(path, StringComparison.InvariantCultureIgnoreCase));

                // The expected parent of 'This PC' is the 'Desktop'
                Assert.IsTrue(path.Equals(KF_IID.ID_FOLDERID_Desktop, StringComparison.InvariantCultureIgnoreCase));

                // this works for display name return PidlManager.GetPidlDisplayName(parentPIDL);
            }
            finally
            {
                parentPIDL = PidlManager.FreeCoTaskMem(parentPIDL);
            }
        }

        /// <summary>
        /// Verifies whether we can handle a query for the parent of a desktop item or not.
        /// via <see cref="List{ShellId}"/> class (alternative pidl representation).
        /// </summary>
        [TestMethod]
        public void GetLogicalLocationForParentOfDesktop()
        {
            var testitem = KF_IID.ID_FOLDERID_Desktop;

            IdList parentIdList = null, relativeChild = null;
            var retVal = PidlManager.GetParentIdListFromPath(testitem, out parentIdList, out relativeChild);

            Assert.IsTrue(relativeChild == null);

            Assert.IsTrue(retVal == true);

            // Desktop item is expected to have no parent
            Assert.IsTrue(parentIdList == null);
        }
    }
}
