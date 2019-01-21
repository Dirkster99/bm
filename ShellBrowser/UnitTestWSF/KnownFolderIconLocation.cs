namespace UnitTestWSF
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WSF;
    using WSF.IDs;
    using WSF.Shell.Interop.Dlls;
    using WSF.Shell.Interop.Interfaces.Knownfolders;
    using WSF.Shell.Interop.Interfaces.ShellFolders;
    using WSF.Shell.Interop.Knownfolders;
    using WSF.Shell.Interop.ShellFolders;
    using WSF.Shell.Pidl;
    using WSF.Shell.Enums;
    using WSF.Shell.Interop.Interfaces;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text;
    using System.Windows.Media.Imaging;

    [TestClass]
    public class KnownFolderIconLocation
    {
        /// <summary>
        /// Workflow:
        /// https://www.neowin.net/forum/topic/316840-c-getting-a-files-large-icon/
        /// 
        /// 1) Get a PIDL to the file you want the icon of.
        /// 
        /// 2) Get the IShellFolder parent of it and the relative PIDL.
        /// 
        /// 3) Query that IShellFolder for an IExtractIcon(GetUIObjectOf) for the relative PIDL you have obtained in step2.
        /// 
        /// 4) get the location of the object with IExtractIcon::GetIconLocation
        /// 
        /// 5) Use that location to extract the icon with IExtractIcon::Extract(...)
        /// and give up the size as a long with the high word and low word of the long
        /// set as the size(MAKELONG(size, size) ).
        /// 
        /// -> Result: Gets the Icon for the parent of a known folder.
        /// </summary>
        [TestMethod]
        public void GetIconForParentOfSpecialFolder()
        {
            var testPath = KF_IID.ID_FOLDERID_Fonts;

            IdList parentIdList = null, relativeChild = null;
            var retVal = PidlManager.GetParentIdListFromPath(testPath, out parentIdList, out relativeChild);

            Assert.IsTrue(retVal);

            // Child item is the desktop -> extract and return desktop icon
            if (parentIdList == null || relativeChild == null)
            {
                throw new NotImplementedException();
            }

            IntPtr parentPtr = default(IntPtr);
            IntPtr relChildPtr = default(IntPtr);
            IntPtr ptrShellFolder = default(IntPtr);
            IntPtr ptrExtractIcon = default(IntPtr);
            IntPtr smallHicon = default(IntPtr);
            IntPtr largeHicon = default(IntPtr);
            try
            {
                parentPtr = PidlManager.IdListToPidl(parentIdList);
                relChildPtr = PidlManager.IdListToPidl(relativeChild);

                Assert.IsTrue(parentPtr != default(IntPtr));
                Assert.IsTrue(relChildPtr != default(IntPtr));

                Guid guid = typeof(IShellFolder2).GUID;
                HRESULT hr = NativeMethods.SHBindToParent(parentPtr, guid,
                                                          out ptrShellFolder, ref relChildPtr);

                Assert.IsTrue(hr == HRESULT.S_OK);

                using (var shellFolder = new ShellFolder(ptrShellFolder))
                {
                    Assert.IsTrue(shellFolder != null);

                    guid = typeof(IExtractIcon).GUID;
                    var pidls = new IntPtr[] { relChildPtr };
                    hr = shellFolder.Obj.GetUIObjectOf(IntPtr.Zero, 1, pidls, guid,
                                                        IntPtr.Zero, out ptrExtractIcon);

                    Assert.IsTrue(hr == HRESULT.S_OK);

                    using (var extractIcon = new GenericCOMFolder<IExtractIcon>(ptrExtractIcon))
                    {
                        Assert.IsTrue(extractIcon != null);

                        var iconFile = new StringBuilder(NativeMethods.MAX_PATH);
                        int index = -1;
                        uint pwFlags = 0;
                        hr = extractIcon.Obj.GetIconLocation(0, iconFile, (uint)iconFile.Capacity,
                            ref index, ref pwFlags);

                        Assert.IsTrue(hr == HRESULT.S_OK);

                        Assert.IsFalse(string.IsNullOrEmpty(iconFile.ToString()));

                        hr = extractIcon.Obj.Extract(iconFile.ToString(), index,
                                                ref smallHicon, ref largeHicon, 16);

                        Assert.IsTrue(hr == HRESULT.S_OK);
                        PngBitmapDecoder bitmapDecoder = null;
                        using (var memoryStream = new MemoryStream())
                        {
                            Icon.FromHandle(smallHicon).ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            // Decode the icon
                            bitmapDecoder = new PngBitmapDecoder(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                        }

                        Assert.IsTrue(bitmapDecoder != null);
                        Assert.IsTrue(bitmapDecoder.Frames != null);
                        Assert.IsTrue(bitmapDecoder.Frames.Count > 0);

                        Assert.IsTrue(bitmapDecoder.Frames[0] != null);

                        bitmapDecoder = null;
                        using (var memoryStream = new MemoryStream())
                        {
                            Icon.FromHandle(largeHicon).ToBitmap().Save(memoryStream, ImageFormat.Png);
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            // Decode the icon
                            bitmapDecoder = new PngBitmapDecoder(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                        }

                        Assert.IsTrue(bitmapDecoder != null);
                        Assert.IsTrue(bitmapDecoder.Frames != null);
                        Assert.IsTrue(bitmapDecoder.Frames.Count > 0);

                        Assert.IsTrue(bitmapDecoder.Frames[0] != null);
                    }
                }
            }
            finally
            {
                if (parentPtr != default(IntPtr))
                    NativeMethods.ILFree(parentPtr);

////                if (relChildPtr != default(IntPtr))    NOT NECESSARY
////                    NativeMethods.ILFree(relChildPtr);

                if (smallHicon != default(IntPtr))
                    NativeMethods.DestroyIcon(smallHicon);

                if (largeHicon != default(IntPtr))
                    NativeMethods.DestroyIcon(largeHicon);
            }
        }

        [TestMethod]
        public void GetIconForControlPanel()
        {
            var path = KF_IID.ID_FOLDERID_ControlPanelFolder;
            IKnownFolderProperties props = null;

            props = KnownFolderHelper.GetFolderPropertiesFromPath(path);

            Assert.IsTrue(props != null);
        }

        [TestMethod]
        public void GetIconForFonts()
        {
            var path = KF_IID.ID_FOLDERID_Fonts;
            IKnownFolderProperties props = null;

            props = KnownFolderHelper.GetFolderPropertiesFromPath(path);

            Assert.IsTrue(props != null);
        }
    }
}
