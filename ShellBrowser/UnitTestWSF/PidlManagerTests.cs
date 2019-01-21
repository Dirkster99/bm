namespace UnitTestWSF
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WSF.IDs;
    using WSF.Shell.Interop.Dlls;
    using WSF.Shell.Interop.Knownfolders;
    using WSF.Shell.Interop.KnownFolders;
    using WSF.Shell.Interop.ShellFolders;
    using WSF.Shell.Pidl;
    using WSF.Shell.Enums;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    [TestClass]
    public class PidlManagerTests
    {
        [TestMethod]
        public void CanDecodePidl()
        {
            IntPtr pidl = default(IntPtr);
            try
            {
                NativeMethods.SHGetKnownFolderIDList(KnownFolderGuids.FOLDERID_Cookies,
                                               KNOWN_FOLDER_FLAG.KF_NO_FLAGS,
                                               IntPtr.Zero, out pidl);

                var idList = PidlManager.Decode(pidl);
            }
            finally
            {
                if (pidl != default(IntPtr))
                    NativeMethods.ILFree(pidl);
            }
        }

        [TestMethod]
        public void CanDecodeFilesystemPidl()
        {
            IntPtr pidl = default(IntPtr);
            try
            {
                NativeMethods.SHGetKnownFolderIDList(KnownFolderGuids.FOLDERID_Documents,
                                                     KNOWN_FOLDER_FLAG.KF_NO_FLAGS,
                                                     IntPtr.Zero, out pidl);

                var idList = PidlManager.PidlToIdlist(pidl);  // Convert PIDL to IdList
            }
            finally
            {
                if (pidl != default(IntPtr))
                    NativeMethods.ILFree(pidl);
            }
        }

        [TestMethod]
        public void CanGetPidlDisplayName()
        {
            IntPtr pidl = default(IntPtr);
            try
            {
                NativeMethods.SHGetKnownFolderIDList(KnownFolderGuids.FOLDERID_Documents,
                                                     KNOWN_FOLDER_FLAG.KF_NO_FLAGS,
                                                     IntPtr.Zero, out pidl);

                var displayName = PidlManager.GetPidlDisplayName(pidl);
                Assert.AreEqual(displayName, "Documents");
            }
            finally
            {
                if (pidl != default(IntPtr))
                    NativeMethods.ILFree(pidl);
            }
        }

        [TestMethod]
        public void CanBouncePidl()
        {
            IntPtr pidl = default(IntPtr);
            try
            {
                NativeMethods.SHGetKnownFolderIDList(KnownFolderGuids.FOLDERID_Documents,
                                                   KNOWN_FOLDER_FLAG.KF_NO_FLAGS,
                                                   IntPtr.Zero, out pidl);

                var idList = PidlManager.PidlToIdlist(pidl);  // Convert pidl to idlist

                NativeMethods.ILFree(pidl);
                pidl = default(IntPtr);

                pidl = PidlManager.IdListToPidl(idList);     // Convert idlist to pidl 

                var displayName = PidlManager.GetPidlDisplayName(pidl);
                Assert.AreEqual(displayName, "Documents");
            }
            finally
            {
                if (pidl != default(IntPtr))
                    NativeMethods.ILFree(pidl);
            }
        }

        [TestMethod]
        public void CanIdentifyIdListLength()
        {
            IntPtr pidl = default(IntPtr);
            try
            {
                NativeMethods.SHGetKnownFolderIDList(KnownFolderGuids.FOLDERID_Downloads,
                                                     KNOWN_FOLDER_FLAG.KF_NO_FLAGS, IntPtr.Zero, out pidl);

                var idList = PidlManager.PidlToIdlist(pidl);

                Assert.IsTrue(idList.Ids.Count > 1);
                //Assert.That(idList.Ids.Count, Is.GreaterThan(1));
            }
            finally
            {
                if (pidl != default(IntPtr))
                    NativeMethods.ILFree(pidl);
            }
        }

        [TestMethod]
        public void CanFullRoundTripPidl()
        {
            IntPtr pidl = default(IntPtr);
            IntPtr pidl2 = default(IntPtr);
            try
            {
                NativeMethods.SHGetKnownFolderIDList(KnownFolderGuids.FOLDERID_Downloads,
                                                   KNOWN_FOLDER_FLAG.KF_NO_FLAGS,
                                                   IntPtr.Zero, out pidl);

                var idList = PidlManager.PidlToIdlist(pidl);
                pidl2 = PidlManager.IdListToPidl(idList);
                var idList2 = PidlManager.PidlToIdlist(pidl2);

                Assert.IsTrue(idList.Matches(idList2));
            }
            finally
            {
                if (pidl != default(IntPtr))
                    NativeMethods.ILFree(pidl);

                if (pidl2 != default(IntPtr))
                    NativeMethods.ILFree(pidl2);
            }
        }

        /// <summary>
        /// Veriefies that the system has defaut drive
        /// Gets the path for the default drive and verifies that we can get:
        /// 1) a pidl for the path and
        /// 2) get a path for the pidl
        /// 3) where the resulting path should be equal to the inital default drive's path.
        /// </summary>
        [TestMethod]
        public void GetPidlForDrivePath()
        {
            IntPtr pidl = default(IntPtr);
            try
            {
                // Get the default drive's path
                var drive = new DirectoryInfo(Environment.SystemDirectory).Root.Name;

                Assert.IsFalse(string.IsNullOrEmpty(drive));

                pidl = PidlManager.GetPIDLFromPath(drive);

                Assert.IsFalse(pidl == default(IntPtr));

                // Logical and physical path representation should be the same for drives
                string logicalPath = PidlManager.GetPathFromPIDL(pidl);
                string physStorePath = PidlManager.GetPathFromPIDL(pidl, TypOfPath.PhysicalStoragePath);

                Assert.IsTrue(string.Equals(drive, physStorePath, StringComparison.CurrentCulture));
                Assert.IsTrue(string.Equals(logicalPath, physStorePath, StringComparison.CurrentCulture));

                ////Console.WriteLine("Path Retrieval via PIDL: '{0}' -> Logical Path: '{1}', Physical Path: '{2}'",
                ////                  drive, logicalPath, physStorePath);
            }
            finally
            {
                pidl = PidlManager.ILFree(pidl);
            }
        }

        /// <summary>
        /// Verifies that a folder in the file system can be represented by a PIDL
        /// and the PIDL can be converted back into the intial
        /// (physical/logical) folder replresentation.
        /// </summary>
        [TestMethod]
        public void GetPidlForDirectoryPath()
        {
            // Get the default drive's path
            var drive = new DirectoryInfo(Environment.SystemDirectory).Root.Name;

            Assert.IsFalse(string.IsNullOrEmpty(drive));

            // enumerate on root directores of the default drive and
            // verify correct pidl <-> path reprentation for each directory
            foreach (var originalPath in Directory.EnumerateDirectories(drive))
            {
                IntPtr pidl = default(IntPtr);
                try
                {
                    pidl = PidlManager.GetPIDLFromPath(originalPath);

                    Assert.IsFalse(pidl == default(IntPtr));

                    string logicalPath = PidlManager.GetPathFromPIDL(pidl);
                    string physStorePath = PidlManager.GetPathFromPIDL(pidl, TypOfPath.PhysicalStoragePath);

                    // The logical path of a special path '::{...}' will differ from
                    // its physical representation 'C:\Windows'
                    // Otherwise, both repesentations should be equal for non-special folders
                    if (ShellHelpers.IsSpecialPath(logicalPath) == ShellHelpers.SpecialPath.IsSpecialPath)
                        Assert.IsFalse(string.Equals(logicalPath, physStorePath));
                    else
                        Assert.IsTrue(string.Equals(logicalPath, physStorePath));

                    ////Console.WriteLine("Path Retrieval via PIDL: '{0}' -> Logical Path: '{1}', Physical Path: '{2}'",
                    ////    originalPath, logicalPath, physStorePath);
                }
                finally
                {
                    pidl = PidlManager.ILFree(pidl);
                }
            }
        }

        /// <summary>
        /// Verifies that a physical path of a knownfolder with a physical file system
        /// representation can be by a PIDL and the PIDL can be converted back into the
        /// intial (physical/logical) path replresentation.
        /// 
        /// </summary>
        [TestMethod]
        public void GetPidlForSpecialFolderPath()
        {
            string originalPath = null;
            IntPtr pidl = default(IntPtr), ptrPath = default(IntPtr);
            try
            {
                using (var kfn = KnownFolderHelper.FromKnownFolderGuid(new Guid(KF_ID.ID_FOLDERID_Windows)))
                {
                    pidl = kfn.KnownFolderToPIDL();
                    kfn.Obj.GetPath(0, out ptrPath);

                    Assert.IsTrue(ptrPath != default(IntPtr));

                    originalPath = Marshal.PtrToStringUni(ptrPath);
                }

                Assert.IsFalse(string.IsNullOrEmpty(originalPath));

                string logicalPath = PidlManager.GetPathFromPIDL(pidl);
                string physStorePath = PidlManager.GetPathFromPIDL(pidl, TypOfPath.PhysicalStoragePath);

                Assert.IsTrue(string.Equals(originalPath, physStorePath, StringComparison.InvariantCultureIgnoreCase));
                Assert.IsFalse(string.Equals(logicalPath, physStorePath));

                ////Console.WriteLine("Path Retrieval via PIDL:'{0}' ->\nLogical Path: '{1}', Physical Path: '{2}'",
                ////    originalPath, logicalPath, physStorePath);
            }
            finally
            {
                pidl = PidlManager.ILFree(pidl);
                ptrPath = PidlManager.FreeCoTaskMem(ptrPath);
            }
        }
    }
}
