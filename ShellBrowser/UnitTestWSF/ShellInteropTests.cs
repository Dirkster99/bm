namespace UnitTestWSF
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WSF.Shell.Interop;
    using WSF.Shell.Interop.Interfaces.ShellFolders;
    using WSF.Shell.Interop.KnownFolders;
    using WSF.Shell.Interop.ShellFolders;
    using WSF.Shell.Pidl;
    using WSF.Shell.Enums;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using WSF.Shell.Interop.Dlls;
    using WSF.IDs;

    [TestClass]
    public class ShellInteropTests
    {
        [TestMethod]
        public void CanGetDesktopFolder()
        {
            using (var desktopShellFolder = new ShellFolderDesktop())
            {

            }
        }

        /// <summary>
        /// We must be able to get the documents known path
        /// without throwing an exception
        /// </summary>
        [TestMethod]
        public void CanGetKnownFolderPath()
        {
            IntPtr ptrOutPath = default(IntPtr);
            string fs_path = null;
            try
            {
                HRESULT result = NativeMethods.SHGetKnownFolderPath(KnownFolderGuids.FOLDERID_Documents,
                                                                    KNOWN_FOLDER_FLAG.KF_NO_FLAGS,
                                                                    IntPtr.Zero, out ptrOutPath);

                Assert.IsTrue(result == HRESULT.S_OK);

                if (result == HRESULT.S_OK)
                    fs_path = Marshal.PtrToStringUni(ptrOutPath);

                Assert.IsTrue(ptrOutPath != default(IntPtr));
                Assert.IsTrue(fs_path != null);
            }
            finally
            {
                if (ptrOutPath != default(IntPtr))
                    Marshal.FreeCoTaskMem(ptrOutPath);
            }
        }

        /// <summary>
        /// Verifies if we can get the path of a known folder
        /// as an ITEMIDLIST structure.
        /// </summary>
        [TestMethod]
        public void CanGetAndFreeKnownFolderIdList()
        {
            IntPtr pidl = default(IntPtr);
            try
            {
                // Test should fail if this throws an exception...
                NativeMethods.SHGetKnownFolderIDList(KnownFolderGuids.FOLDERID_Cookies, KNOWN_FOLDER_FLAG.KF_NO_FLAGS, IntPtr.Zero, out pidl);
                Assert.IsTrue(pidl != default(IntPtr));
            }
            finally
            {
                if (pidl != default(IntPtr))
                    NativeMethods.ILFree(pidl);
            }
        }

        /// <summary>
        /// Verifies if we can get the desktop folder pidl, get a path for it
        /// and free the pidl.
        /// </summary>
        [TestMethod]
        public void CanGetDesktopFolderLocationAndPath()
        {
            using (var desktop = new ShellFolderDesktop())
            {
                Assert.IsTrue(desktop != null);
            }
        }

        /// <summary>
        /// Test method verifies that we can retrieve all items below
        /// the desktop root item.
        /// </summary>
        [TestMethod]
        public void CanEnumerateDesktopFolders()
        {
            // Defines the type of items that we want to retieve below the desktop root item
            const SHCONTF flags = SHCONTF.NONFOLDERS | SHCONTF.FOLDERS | SHCONTF.INCLUDEHIDDEN;

            //  Get the desktop root folder.
            IntPtr ptrDesktopFolder = default(IntPtr);
            IntPtr enumerator = default(IntPtr);
            IShellFolder2 iDesktopFolder = null;

            // Enumerate over children of given shell folder item using this interface
            // https://msdn.microsoft.com/en-us/library/windows/desktop/bb761983(v=vs.85).aspx
            IEnumIDList enumIDs = null;
            try
            {
                HRESULT hr = NativeMethods.SHGetDesktopFolder(out ptrDesktopFolder);

                Assert.IsTrue(hr == HRESULT.S_OK);

                if (ptrDesktopFolder != IntPtr.Zero)
                    iDesktopFolder = (IShellFolder2)Marshal.GetTypedObjectForIUnknown(ptrDesktopFolder, typeof(IShellFolder2));

                Assert.IsTrue(iDesktopFolder != null);

                //  Create an enumerator and enumerate over each item.
                hr = iDesktopFolder.EnumObjects(IntPtr.Zero, flags, out enumerator);

                Assert.IsTrue(hr == HRESULT.S_OK);

                // Convert enum IntPtr to interface
                enumIDs = (IEnumIDList)Marshal.GetTypedObjectForIUnknown(enumerator, typeof(IEnumIDList));

                Assert.IsTrue(enumIDs != null);

                uint fetched, count = 0;
                IntPtr apidl = default(IntPtr);

                // Get one item below desktop root at a time and process by getting its display name
                for (; enumIDs.Next(1, out apidl, out fetched) == HRESULT.S_OK; count++)
                {
                    if (fetched <= 0)  // End this loop if no more items are available
                    {
                        break;
                    }

                    IntPtr ptrStr = default(IntPtr); // get strings for this item
                    try
                    {
                        string displayName = null, parseName = null;

                        ptrStr = Marshal.AllocCoTaskMem(NativeMethods.MAX_PATH * 2 + 4);
                        Marshal.WriteInt32(ptrStr, 0, 0);
                        StringBuilder buf = new StringBuilder(NativeMethods.MAX_PATH);

                        // The apidl ITEMIDLIST structures returned in the array are relative to
                        // the IShellFolder being enumerated.
                        if (iDesktopFolder.GetDisplayNameOf(apidl, SHGDNF.SHGDN_NORMAL, ptrStr) == HRESULT.S_OK)
                        {
                            NativeMethods.StrRetToBuf(ptrStr, ptrDesktopFolder, buf, NativeMethods.MAX_PATH);
                            displayName = buf.ToString();
                        }

                        if (iDesktopFolder.GetDisplayNameOf(apidl, SHGDNF.SHGDN_FORPARSING, ptrStr) == HRESULT.S_OK)
                        {
                            NativeMethods.StrRetToBuf(ptrStr, ptrDesktopFolder, buf, NativeMethods.MAX_PATH);
                            parseName = buf.ToString();
                        }

                        Assert.IsFalse(string.IsNullOrEmpty(displayName));
                        Assert.IsFalse(string.IsNullOrEmpty(parseName));
                    }
                    finally
                    {
                        ptrStr = PidlManager.FreeCoTaskMem(ptrStr);
                        apidl = PidlManager.FreeCoTaskMem(apidl);
                    }
                }

                // There should be more than one item below the desktop root item
                // 'My PC', 'Recycle Bin', and 'Network' are already three
                Assert.IsTrue(count > 2);
            }
            finally
            {
                if (enumerator != default(IntPtr))
                    Marshal.Release(enumerator);

                if (iDesktopFolder != null)
                    Marshal.ReleaseComObject(iDesktopFolder);

                if (ptrDesktopFolder != default(IntPtr))
                    Marshal.Release(ptrDesktopFolder);
            }
        }

        [TestMethod]
        public void CanGetShellFolder2()
        {
            string specialPath = KF_IID.ID_FOLDERID_UsersFiles;

            IdList parentIdList = null, relativeChild = null;
            var retVal = PidlManager.GetParentIdListFromPath(specialPath, out parentIdList, out relativeChild);

            Assert.IsTrue(retVal);

            // Child item is the desktop -> cannot implement this for desktop since it has no parent
            if (parentIdList == null || relativeChild == null)
            {
                throw new NotImplementedException();
            }

            IntPtr parentPtr = default(IntPtr);
            IntPtr relChildPtr = default(IntPtr);
            IntPtr relChildPtr1 = default(IntPtr);
            IntPtr ptrShellFolder = default(IntPtr);
            try
            {
                parentPtr = PidlManager.IdListToPidl(parentIdList);
                relChildPtr = PidlManager.IdListToPidl(relativeChild);

                Assert.IsTrue(parentPtr != default(IntPtr));
                Assert.IsTrue(relChildPtr != default(IntPtr));

                Guid guid = typeof(IShellFolder2).GUID;
                HRESULT hr = NativeMethods.SHBindToParent(relChildPtr, guid,
                                                            out ptrShellFolder, ref relChildPtr1);

                Assert.IsTrue(hr == HRESULT.S_OK);

                using (var shellFolder = new ShellFolder(ptrShellFolder))
                {
                    Assert.IsTrue(shellFolder != null);

                    var ParseName = shellFolder.GetShellFolderName(relChildPtr, SHGDNF.SHGDN_FORPARSING);
                    Assert.IsFalse(string.IsNullOrEmpty(ParseName));

                    var Name = shellFolder.GetShellFolderName(relChildPtr, SHGDNF.SHGDN_NORMAL);
                    Assert.IsFalse(string.IsNullOrEmpty(Name));
                }
            }
            finally
            {
                if (parentPtr != default(IntPtr))
                    NativeMethods.ILFree(parentPtr);

                if (relChildPtr != default(IntPtr))
                    NativeMethods.ILFree(relChildPtr);

                //                if (relChildPtr != default(IntPtr))
                //                    NativeMethods.ILFree(relChildPtr1);
            }
        }
    }
}
