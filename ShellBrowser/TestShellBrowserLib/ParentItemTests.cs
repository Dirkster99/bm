namespace TestShellBrowserLib
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ShellBrowserLib;
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.Shell.Interop.Knownfolders;
    using ShellBrowserLib.Shell.Pidl;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Tests verifies whether the expected parent item can be determined
    /// through a logical or physical (if available) path.
    /// </summary>
    [TestClass]
    public class ParentItemTests
    {
        /// <summary>
        /// Verifies whether a parent folder is recognized as special folder
        /// if the child folder is a normal filsesystem disk folder.
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

                string testPath = System.IO.Path.Combine(originalPath, "System32");
                Assert.IsTrue(System.IO.Directory.Exists(testPath));

                string parentFolder = ShellBrowser.GetParentFolder(testPath);
                Assert.IsTrue(string.IsNullOrEmpty(parentFolder) == false);

                var test = ShellBrowser.Create(testPath);
                var testParent = ShellBrowser.Create(parentFolder);

                Assert.IsTrue(test != null);
                Assert.IsTrue(testParent != null);
                Assert.IsTrue(TestHelper.IsSubDirectoryOf(test.PathFileSystem, testParent.PathFileSystem));

                parentFolder = string.Empty;
                parentFolder = ShellBrowser.GetParentFolder(testPath, TypOfPath.LogicalPath);
                Assert.IsTrue(string.IsNullOrEmpty(parentFolder) == false);

                var parentFolderGuid = new Guid(parentFolder.Substring(KF_IID.IID_Prefix.Length));
                var thisSpecialParent = new Guid(KF_ID.ID_FOLDERID_Windows);

                // This item should be the 'Windows' special folder
                Assert.IsTrue(thisSpecialParent.Equals(parentFolderGuid));
            }
            finally
            {
                ptrPath = PidlManager.FreeCoTaskMem(ptrPath);
            }
        }

        /// <summary>
        /// Verifies that we can retrieve the physical storage location of a parent item
        /// of a known folder.
        /// </summary>
        [TestMethod]
        public void GetPhysicalStorageLocationOfParent()
        {
            string originalPath = null;
            IntPtr ptrPath = default(IntPtr);
            try
            {
                var specialFolderGuid = new Guid(KF_ID.ID_FOLDERID_Windows);
                using (var kfn = KnownFolderHelper.FromKnownFolderGuid(specialFolderGuid))
                {
                    kfn.Obj.GetPath(0, out ptrPath);

                    Assert.IsTrue(ptrPath != default(IntPtr));

                    originalPath = Marshal.PtrToStringUni(ptrPath);
                }

                Assert.IsFalse(string.IsNullOrEmpty(originalPath));

                string parentFolder = ShellBrowser.GetParentFolder(originalPath);
                Assert.IsTrue(string.IsNullOrEmpty(parentFolder) == false);

                var test = ShellBrowser.Create(originalPath);
                var testParent = ShellBrowser.Create(parentFolder);

                Assert.IsTrue(test != null);
                Assert.IsTrue(testParent != null);
                Assert.IsTrue(TestHelper.IsSubDirectoryOf(test.PathFileSystem, testParent.PathFileSystem));

                var testFolderGuid = new Guid(test.PathShell.Substring(KF_IID.IID_Prefix.Length));

                // This item should be the 'Windows' special folder
                Assert.IsTrue(specialFolderGuid.Equals(testFolderGuid));
            }
            finally
            {
                ptrPath = PidlManager.FreeCoTaskMem(ptrPath);
            }
        }

        /// <summary>
        /// Verifies that we can retrive the logical location of a parent item
        /// of a drive.
        /// </summary>
        [TestMethod]
        public void GetParentOfDrive()
        {
            // Get the default drive's path
            var drive = new DirectoryInfo(Environment.SystemDirectory).Root.Name;

            Assert.IsFalse(string.IsNullOrEmpty(drive));

            // Gets the logical path of the parent item
            string parentFolder = ShellBrowser.GetParentFolder(drive);
            Assert.IsFalse(string.IsNullOrEmpty(parentFolder));

            // create browser interface based objects for parent and child item
            var test = ShellBrowser.Create(drive);
            var testParent = ShellBrowser.Create(parentFolder);

            Assert.IsTrue(test != null);
            Assert.IsTrue(testParent != null);

            var parentFolderGuid = new Guid(testParent.PathShell.Substring(KF_IID.IID_Prefix.Length));
            var testParentGuid = new Guid(testParent.PathShell.Substring(KF_IID.IID_Prefix.Length));
            var thisPCGuid = new Guid(KF_ID.ID_FOLDERID_ComputerFolder);

            // The parent of a drive should be the 'This PC' special folder
            Assert.IsTrue(thisPCGuid.Equals(parentFolderGuid));
            Assert.IsTrue(thisPCGuid.Equals(testParentGuid));
        }

        /// <summary>
        /// Verifies that we can retrive the logical location of the Documents Library item
        /// which should be the Libraries item.
        /// </summary>
        [TestMethod]
        public void GetPhysicalStorageForLibrariesParentOfDocuments()
        {
            string testFolder = KF_IID.ID_FOLDERID_DocumentsLibrary;
            string parentFolder = ShellBrowser.GetParentFolder(testFolder);

            Assert.IsFalse(string.IsNullOrEmpty(parentFolder));

            var test = ShellBrowser.Create(testFolder);
            var testParent = ShellBrowser.Create(parentFolder);

            Assert.IsTrue(test != null);
            Assert.IsTrue(testParent != null);

            Assert.IsTrue(string.Compare(KF_IID.ID_FOLDERID_DocumentsLibrary, test.SpecialPathId, true)==0);
            Assert.IsTrue(string.Compare(KF_IID.ID_FOLDERID_UsersLibraries, testParent.SpecialPathId, true) == 0);

            // Item should have a physical storage location that is particularly strange:
            // PathFileSystem = "::{031E4825-7B94-4DC3-B131-E946B44C8DD5}\\Documents.library-ms"
            Assert.IsFalse(string.IsNullOrEmpty(test.PathFileSystem));
            Assert.IsTrue(string.IsNullOrEmpty(testParent.PathFileSystem));

            // Parent item of Documents at 'C:\Users\<login>\AppData\Roaming\Microsoft\Windows\Libraries\Documents.library-ms'
            // should be                   'Libraries'->'::{1b3ea5dc-b587-4786-b4ef-bd1dc332aeae}'
            ////Console.WriteLine("The Parent of: '{0}'->'{1}' is '{2}'->'{3}'",
            ////    test.Name, test.PathFileSystem, testParent.Name, parentFolder);
        }

        /// <summary>
        /// Verifies that we can retrive the logical location of the 'Public Documents' item
        /// which should be the 'Public' item.
        /// </summary>
        [TestMethod]
        public void GetPhysicalStorageForLibrariesParentOfPublicDocuments()
        {
            string testFolder = KF_IID.ID_FOLDERID_PublicDocuments;
            string parentFolder = ShellBrowser.GetParentFolder(testFolder);
            Assert.IsTrue(string.IsNullOrEmpty(parentFolder) == false);

            var test = ShellBrowser.Create(testFolder);
            var testParent = ShellBrowser.Create(parentFolder);

            Assert.IsTrue(test != null);
            Assert.IsTrue(testParent != null);

            var parentFolderGuid = new Guid(testParent.PathShell.Substring(KF_IID.IID_Prefix.Length));
            var testParentGuid = new Guid(testParent.PathShell.Substring(KF_IID.IID_Prefix.Length));
            var publicGuid = new Guid(KF_ID.ID_FOLDERID_Public);

            Assert.IsTrue(publicGuid.Equals(parentFolderGuid));
            Assert.IsTrue(publicGuid.Equals(testParentGuid));

            // Item should have a physical storage location
            Assert.IsFalse(string.IsNullOrEmpty(test.PathFileSystem));

            // Parent item of 'Public Documents' at 'C:\Users\Public\Documents'
            // should be      'Public'           -> '::{dfdf76a2-c82a-4d63-906a-5644ac457385}'
            ////Console.WriteLine("The Parent of: '{0}'->'{1}' is '{2}'->'{3}'",
            ////    test.Name, test.PathFileSystem, testParent.Name, parentFolder);
        }

        /// <summary>
        /// Verify that we get a logical storage location indicator for the parent
        /// event when we asked for a physical location (the logical path location
        /// '::{...}' is rerurned if there is no physical location of the parent).
        /// </summary>
        [TestMethod]
        public void GetPhysicalStorageForUserProfileParentOf()
        {
            var list = new List<string>();

            list.Add(KF_IID.ID_FOLDERID_Documents);
            list.Add(KF_IID.ID_FOLDERID_Music);

            foreach (var testFolder in list)
            {
                string parentFolder = ShellBrowser.GetParentFolder(testFolder);
                Assert.IsFalse(string.IsNullOrEmpty(parentFolder));

                var test = ShellBrowser.Create(testFolder);
                var testParent = ShellBrowser.Create(parentFolder);

                Assert.IsTrue(test != null);
                Assert.IsTrue(testParent != null);

                var parentFolderGuid = new Guid(testParent.PathShell.Substring(KF_IID.IID_Prefix.Length));
                var testParentGuid = new Guid(testParent.PathShell.Substring(KF_IID.IID_Prefix.Length));
                var profileGuid = new Guid(KF_ID.ID_FOLDERID_Profile);

                Assert.IsTrue(profileGuid.Equals(parentFolderGuid));
                Assert.IsTrue(profileGuid.Equals(testParentGuid));

                // Item should have a physical storage location
                Assert.IsFalse(string.IsNullOrEmpty(test.PathFileSystem));
            }

            // The Parent of: 'Documents'->'C:\Users\<login>\Documents'
            //     should be: '<login>'->'::{5e6c858f-0e22-4760-9afe-ea3317b67173}'
            //
            // The Parent of: 'Music'   -> 'C:\Users\<login>\Music'
            //     should be: '<login>' -> '::{5e6c858f-0e22-4760-9afe-ea3317b67173}'
            //
            // That is, physical and logical paths of parents are equal here.
            // So, we can get a logical (or special path) formated string even when asking
            // for a physical path.
            //
            ////Console.WriteLine("The Parent of: '{0}'->'{1}' is '{2}'->'{3}'",
            ////    test.Name, test.PathFileSystem, testParent.Name, parentFolder);
        }

        /// <summary>
        /// Verifies that items that should be under 'This PC' are found there.
        /// </summary>
        [TestMethod]
        public void GetLogicalLocationForThisPCParentOf()
        {
            var list = new List<string>();

            list.Add(KF_IID.ID_FOLDERID_Desktop);
            list.Add(KF_IID.ID_FOLDERID_Documents);
            list.Add(KF_IID.ID_FOLDERID_Music);
            list.Add(KF_IID.ID_FOLDERID_Downloads);
            list.Add(KF_IID.ID_FOLDERID_Pictures);
            list.Add(KF_IID.ID_FOLDERID_Videos);

            foreach (var testFolder in list)
            {
                string parentFolder = ShellBrowser.GetParentFolder(testFolder, TypOfPath.LogicalPath);
                Assert.IsFalse(string.IsNullOrEmpty(parentFolder));

                var test = ShellBrowser.Create(testFolder);
                var testParent = ShellBrowser.Create(parentFolder);

                Assert.IsTrue(test != null);
                Assert.IsTrue(testParent != null);

                // The Parent of: 'Documents'->'C:\Users\<login>\Documents' is 'This PC'->'::{20D04FE0-3AEA-1069-A2D8-08002B30309D}'
                // The Parent of: 'Downloads'->'C:\Users\<login>\Downloads' is 'This PC'->'::{20D04FE0-3AEA-1069-A2D8-08002B30309D}'
                // The Parent of: 'Music'->'C:\Users\<login>\Music' is 'This PC'->'::{20D04FE0-3AEA-1069-A2D8-08002B30309D}'
                // The Parent of: 'Pictures'->'C:\Users\<login>\Pictures' is 'This PC'->'::{20D04FE0-3AEA-1069-A2D8-08002B30309D}'
                // The Parent of: 'Videos'->'C:\Users\<login>\Videos' is 'This PC'->'::{20D04FE0-3AEA-1069-A2D8-08002B30309D}'
                Assert.IsTrue(KF_IID.ID_FOLDERID_ComputerFolder.Equals(parentFolder, StringComparison.InvariantCultureIgnoreCase));
                Assert.IsTrue(KF_IID.ID_FOLDERID_ComputerFolder.Equals(testParent.PathShell, StringComparison.InvariantCultureIgnoreCase));

                // Item should have a physical storage location
                Assert.IsFalse(string.IsNullOrEmpty(test.PathFileSystem));
            }
        }

        /// <summary>
        /// Verifies that the expected parent of 'This PC' is indeed the Desktop.
        /// </summary>
        [TestMethod]
        public void GetLogicalLocationForDesktopParentOf()
        {
            var testitem = KF_IID.ID_FOLDERID_ComputerFolder;

            string parentFolder = ShellBrowser.GetParentFolder(testitem, TypOfPath.LogicalPath);
            Assert.IsFalse(string.IsNullOrEmpty(parentFolder));

            var test = ShellBrowser.Create(testitem);
            var testParent = ShellBrowser.Create(parentFolder);

            Assert.IsTrue(test != null);
            Assert.IsTrue(testParent != null);

            Assert.IsTrue(KF_IID.ID_FOLDERID_Desktop.Equals(parentFolder, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(KF_IID.ID_FOLDERID_Desktop.Equals(testParent.PathShell, StringComparison.InvariantCultureIgnoreCase));

            // Item should not have a physical storage location
            Assert.IsTrue(string.IsNullOrEmpty(test.PathFileSystem));

            // but it should be indentifyable via its GUID ID representation
            Assert.IsTrue(string.Compare(test.SpecialPathId, KF_IID.ID_FOLDERID_ComputerFolder,true) == 0);
        }
    }
}
