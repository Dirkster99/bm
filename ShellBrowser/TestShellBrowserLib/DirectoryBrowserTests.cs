﻿namespace TestShellBrowserLib
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ShellBrowserLib;
    using ShellBrowserLib.Enums;
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.SharpShell.Interop.Knownfolders;
    using System;
    using System.IO;

    [TestClass]
    public class DirectoryBrowserTests
    {
        /// <summary>
        /// Verify that we can build a <see cref="IDirectoryBrowser"/> object
        /// for a default drive.
        /// 
        /// Note that the properties SpecialPathId and PathFileSystem are expected to
        /// contain different path information.
        /// </summary>
        [TestMethod]
        public void GetDefaultDrive()
        {
            // Get the default drive's path
            var drivePath = new DirectoryInfo(Environment.SystemDirectory).Root.Name;
            var driveInfoPath = new System.IO.DriveInfo(drivePath);

            Assert.IsTrue(drivePath != null);

            var drive = ShellBrowser.Create(drivePath);

            Assert.IsTrue(drive != null);

            Assert.IsTrue(string.IsNullOrEmpty(drive.SpecialPathId));
            Assert.IsTrue(drivePath.Equals(drive.PathFileSystem, StringComparison.InvariantCulture));

            Assert.IsFalse(string.IsNullOrEmpty(drive.Name));
            Assert.IsTrue(drive.DirectoryPathExists());
            Assert.IsTrue((drive.ItemType & DirectoryItemFlags.FileSystemDirectory) != 0);
            Assert.IsTrue(drive.PathType == PathHandler.FileSystem);
//            Assert.IsTrue(string.Compare(drive.Label, driveInfoPath.VolumeLabel,true) == 0);
        }

        /// <summary>
        /// Verify that we can build a <see cref="IDirectoryBrowser"/> object
        /// for a special directory.
        /// 
        /// Note that the properties SpecialPathId and PathFileSystem are expected to
        /// contain different path information.
        /// 
        /// Use the PathShell property if you want to navigate to parents
        /// or children and keep a system wide unique reference.
        /// </summary>
        [TestMethod]
        public void GetDirectory()
        {
            string dirPath = null;

            using (var kf = KnownFolderHelper.FromCanonicalName("Windows"))
            {
                Assert.IsTrue(kf != null);
                dirPath = kf.GetPath();
            }

            Assert.IsTrue(dirPath != null);

            // Lets test the directory browser object with that path
            var dir = ShellBrowser.Create(dirPath);

            Assert.IsTrue(dir != null);

            Assert.IsTrue(string.Compare(KF_IID.ID_FOLDERID_Windows, dir.SpecialPathId, true) == 0);
            Assert.IsTrue(string.Compare(dirPath, dir.PathFileSystem, true) == 0);

            var dirFolderGuid = new Guid(dir.PathShell.Substring(KF_IID.IID_Prefix.Length));
            var thisSpecialParent = new Guid(KF_ID.ID_FOLDERID_Windows);

            // This item should be the Guid of the 'Windows' special folder
            Assert.IsTrue(thisSpecialParent.Equals(dirFolderGuid));

            Assert.IsFalse(string.IsNullOrEmpty(dir.Name));
            Assert.IsTrue(dir.DirectoryPathExists());
            Assert.IsTrue((dir.ItemType & DirectoryItemFlags.FileSystemDirectory) != 0);
            Assert.IsTrue((dir.ItemType & DirectoryItemFlags.Special) != 0);
            Assert.IsTrue(dir.PathType == PathHandler.FileSystem);
        }

        /// <summary>
        /// Verify that we can build a <see cref="IDirectoryBrowser"/> object
        /// for a special known folder item that does NOT have a drirectory in
        /// in the file system.
        /// 
        /// Note that the properties SpecialPathId and PathFileSystem are expected to
        /// contain different path information.
        /// 
        /// Use the PathShell property if you want to navigate to parents
        /// or children and keep a system wide unique reference.
        /// </summary>
        [TestMethod]
        public void GetThisPC()
        {
            string dirPath = null, id = null, pathShell = null;

            using (var kf = KnownFolderHelper.FromCanonicalName("MyComputerFolder"))
            {
                Assert.IsTrue(kf != null);
                dirPath = kf.GetPath();
                id = kf.GetId();
                pathShell = kf.GetPathShell();
            }

            Assert.IsTrue(dirPath == null);    // Test a special folder that has no
            Assert.IsTrue(id != null);        // file system representation
            Assert.IsTrue(pathShell != null);
            Assert.IsTrue(pathShell != id);

            // Lets test the directory browser object with that path
            var specialItem = ShellBrowser.Create(pathShell);

            Assert.IsTrue(specialItem != null);

            Assert.IsTrue(string.Compare(KF_IID.ID_FOLDERID_ComputerFolder, pathShell, true) == 0);
            Assert.IsTrue(string.IsNullOrEmpty(specialItem.PathFileSystem));

            // This item should be the Guid of the 'This PC' special folder
            Assert.IsTrue(string.Compare(specialItem.SpecialPathId, KF_IID.ID_FOLDERID_ComputerFolder, true) == 0);

            Assert.IsFalse(string.IsNullOrEmpty(specialItem.Name));
            Assert.IsFalse(specialItem.DirectoryPathExists());
            Assert.IsTrue((specialItem.ItemType & DirectoryItemFlags.Special) != 0);
            Assert.IsTrue((specialItem.ItemType & DirectoryItemFlags.FileSystemDirectory) == 0);
            Assert.IsTrue((specialItem.ItemType & DirectoryItemFlags.SpecialDesktopFileSystemDirectory) != 0);
            Assert.IsTrue(specialItem.PathType == PathHandler.FileSystem);
        }

        /// <summary>
        /// Verify that we can build a <see cref="IDirectoryBrowser"/> object
        /// for a special known folder item that does HAVE a drirectory in
        /// in the file system.
        /// 
        /// Note that the properties SpecialPathId and PathFileSystem are expected to
        /// contain different path information.
        /// 
        /// Use the PathShell property if you want to navigate to parents
        /// or children and keep a system wide unique reference.
        /// </summary>
        [TestMethod]
        public void GetMusic()
        {
            var music = ShellBrowser.Create(KF_IID.ID_FOLDERID_Music);

            Assert.IsTrue(music != null);
            Assert.IsFalse(string.IsNullOrEmpty(music.IconResourceId));
            Assert.IsTrue(music.IconResourceId.IndexOf(',') > 0);

            Assert.IsTrue(music.ParentIdList != null);
            Assert.IsTrue(music.ParentIdList.Size >= 1);

            Assert.IsTrue(music.ChildIdList != null);
            Assert.IsTrue(music.ParentIdList.Size >= 1);

            Assert.IsFalse(string.IsNullOrEmpty(music.Name));
            Assert.IsFalse(string.IsNullOrEmpty(music.PathFileSystem));
            Assert.IsTrue(string.Compare(music.SpecialPathId, KF_IID.ID_FOLDERID_Music, true) == 0);

            Assert.IsTrue(music.DirectoryPathExists());
            Assert.IsTrue((music.ItemType & DirectoryItemFlags.Special) != 0);
            Assert.IsTrue((music.ItemType & DirectoryItemFlags.FileSystemDirectory) != 0);
            Assert.IsTrue(music.PathType == PathHandler.FileSystem);
        }
    }
}
