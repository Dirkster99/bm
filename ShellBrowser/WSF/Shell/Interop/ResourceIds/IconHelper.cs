namespace WSF.Shell.Interop.ResourceIds
{
    using WSF.Shell.Interop;
    using WSF.Shell.Interop.Dlls;
    using WSF.Shell.Pidl;
    using WSF.Shell.Enums;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using WSF.Shell.Interop.Interfaces.ShellFolders;
    using WSF.Shell.Interop.ShellFolders;
    using WSF.Shell.Interop.Interfaces;

    /// <summary>
    /// Gets the ResourceId (libaryName, index) of a shell icon to support
    /// IconExtaction and display in UI layer.
    /// 
    /// Parts are based on SystemImageList created By LYCJ (2014), released under MIT license
    /// +-> Based on http://vbaccelerator.com/home/net/code/libraries/Shell_Projects/SysImageList/article.asp
    /// </summary>
    internal static class IconHelper
    {
        /// <summary>
        /// Gets an icons reource id if available in the format:
        /// "filename, index"
        /// where the first part is a string and the 2nd part is a negativ integer number).
        /// 
        /// This format is usally used by the Windows Shell libraries so we use it here
        /// to add missing ResourceIds.
        /// </summary>
        /// <param name="parentIdList"></param>
        /// <param name="filename"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static bool GetIconResourceId(IdList parentIdList,
                                               out string filename,
                                               out int index)
        {
            filename = null;
            index = -1;

            IntPtr parentPtr = default(IntPtr);
            IntPtr relChildPtr = default(IntPtr);
            IntPtr ptrShellFolder = default(IntPtr);
            IntPtr ptrExtractIcon = default(IntPtr);
            IntPtr smallHicon = default(IntPtr);
            IntPtr largeHicon = default(IntPtr);
            try
            {
                if (parentIdList.Size == 0)
                {
                    parentPtr = PidlManager.IdListToPidl(parentIdList);
                    relChildPtr = PidlManager.IdListToPidl(IdList.Create());
                }
                else
                {
                    IdList parIdList = null, relChildIdList = null;
                    PidlManager.GetParentChildIdList(parentIdList, out parIdList, out relChildIdList);

                    parentPtr = PidlManager.IdListToPidl(parIdList);
                    relChildPtr = PidlManager.IdListToPidl(relChildIdList);
                }

                if (parentPtr == default(IntPtr) || relChildPtr == default(IntPtr))
                    return false;

                Guid guid = typeof(IShellFolder2).GUID;
                HRESULT hr = NativeMethods.SHBindToParent(parentPtr, guid,
                                                    out ptrShellFolder, ref relChildPtr);

                if (hr != HRESULT.S_OK)
                    return false;

                using (var shellFolder = new ShellFolder(ptrShellFolder))
                {
                    if (shellFolder == null)
                        return false;

                    guid = typeof(IExtractIcon).GUID;
                    var pidls = new IntPtr[] { relChildPtr };
                    hr = shellFolder.Obj.GetUIObjectOf(IntPtr.Zero, 1, pidls, guid,
                                                        IntPtr.Zero, out ptrExtractIcon);

                    if (hr != HRESULT.S_OK)
                        return false;

                    using (var extractIcon = new GenericCOMFolder<IExtractIcon>(ptrExtractIcon))
                    {
                        if (extractIcon == null)
                            return false;

                        var iconFile = new StringBuilder(NativeMethods.MAX_PATH);
                        uint pwFlags = 0;

                        hr = extractIcon.Obj.GetIconLocation(0, iconFile,
                                                            (uint)iconFile.Capacity,
                                                            ref index, ref pwFlags);

                        if (hr != HRESULT.S_OK)
                            return false;

                        if (string.IsNullOrEmpty(iconFile.ToString()))
                            return false;

                        filename = iconFile.ToString();

                        return true;
                    }
                }
            }
            finally
            {
                if (parentPtr != default(IntPtr))
                    NativeMethods.ILFree(parentPtr);

                ////if (relChildPtr != default(IntPtr))
                ////    Shell32.ILFree(relChildPtr);

                if (smallHicon != default(IntPtr))
                    NativeMethods.DestroyIcon(smallHicon);

                if (largeHicon != default(IntPtr))
                    NativeMethods.DestroyIcon(largeHicon);
            }
        }

        /// <summary>
        /// Gets the ResourceId (libararyName, index) of a shell icon
        /// based on an <see cref="IdList"/> object
        /// to support IconExtaction and display in UI layer.
        /// </summary>
        /// <param name="ilPidl"></param>
        /// <param name="isDirectory"></param>
        /// <param name="forceLoadFromDisk"></param>
        /// <param name="size"></param>
        /// <param name="iconState"></param>
        /// <returns></returns>
        public static string FromPidl(IdList ilPidl,
                                      bool isDirectory,
                                      bool forceLoadFromDisk,
                                      IconSize size = IconSize.large,
                                      ShellIconStateConstants iconState = ShellIconStateConstants.ShellIconStateNormal)
        {
            IntPtr ptrPidl = default(IntPtr);
            try
            {
                if ((ptrPidl = PidlManager.IdListToPidl(ilPidl)) != default(IntPtr))
                {
                    return IconHelper.FromPidl(ptrPidl, isDirectory, forceLoadFromDisk,
                                                        size, iconState);
                }
            }
            finally
            {
                if (ptrPidl != default(IntPtr))
                    ptrPidl = PidlManager.ILFree(ptrPidl);
            }

            return null;
        }

        /// <summary>
        /// Gets the ResourceId (libararyName, index) of a shell icon
        /// based on an <see cref="IntPtr"/> formated pidl id-list.
        /// to support IconExtaction and display in UI layer.
        /// 
        /// The caller is responsible for freeing the pidl in <paramref name="ptrPidl"/>.
        /// </summary>
        /// <param name="forceLoadFromDisk"></param>
        /// <param name="iconState"></param>
        /// <param name="isDirectory"></param>
        /// <param name="ptrPidl"></param>
        /// <param name="size"></param>
        public static string FromPidl(IntPtr ptrPidl,
                                      bool isDirectory,
                                      bool forceLoadFromDisk,
                                      IconSize size = IconSize.large,
                                      ShellIconStateConstants iconState = ShellIconStateConstants.ShellIconStateNormal)
        {
            if (!isXpOrAbove())
                throw new NotSupportedException("Windows XP or above required.");

            // There is no thumbnail mode in shell.
            var _size = size == IconSize.thumbnail ? IconSize.jumbo : size;

            // XP does not have extra large or jumbo icon size.
            if (!isVistaUp() && (_size == IconSize.jumbo || _size == IconSize.extraLarge))
                _size = IconSize.large;

            return getIconIndex(ptrPidl, isDirectory, forceLoadFromDisk, size, iconState);
        }

        /// <summary>
        /// Determines if we run on Windows XP or later version of Windows. 
        /// </summary>
        /// <returns>true if current verion is Windows XP or later, false otherwise.</returns>
        internal static bool isXpOrAbove()
        {
            bool ret = false;
            if (Environment.OSVersion.Version.Major > 5)
            {
                ret = true;
            }
            else if ((Environment.OSVersion.Version.Major == 5) &&
                    (Environment.OSVersion.Version.Minor >= 1))
            {
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Determines if we run on Windows Vista or later version of Windows. 
        /// </summary>
        /// <returns>true if current verion is Windows Vista or later, false otherwise.</returns>
        internal static bool isVistaUp()
        {
            return (Environment.OSVersion.Version.Major >= 6);
        }

        private static string getIconIndex(IntPtr pidlPtr,
                                           bool isDirectory,
                                           bool forceLoadFromDisk,
                                           IconSize size,
                                           ShellIconStateConstants iconState)
        {
            SHGFI dwFlags;
            FileAttribute dwAttr;

            getAttributes(isDirectory, forceLoadFromDisk, size, out dwAttr, out dwFlags);
            dwFlags = SHGFI.SHGFI_PIDL | SHGFI.SHGFI_ICONLOCATION;

            SHFILEINFO shfi = new SHFILEINFO();
            uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());
            IntPtr retVal = NativeMethods.SHGetFileInfo(pidlPtr, (UInt32)dwAttr, ref shfi, shfiSize, ((uint)(dwFlags) | (uint)iconState));
            try
            {
                if (retVal.Equals(IntPtr.Zero))
                {
                    System.Diagnostics.Debug.Assert((!retVal.Equals(IntPtr.Zero)), "Failed to get icon index");
                    return null;
                }
                else    // Return final "library, index" formated string to extract actual icon
                {
                    var iconFile = new StringBuilder(NativeMethods.MAX_PATH);
                    iconFile = iconFile.Insert(0, shfi.szDisplayName);

                    // The parseName of the containing icon is sometimes not set (eg.: 'Fonts' folder on Windows 10)
                    // Not sure why. So, instead of returning an invalid ResourcedId String this returns null.
                    if (string.IsNullOrEmpty(iconFile.ToString()) || iconFile.ToString().Length <= 0)
                        return null;

                    return string.Format("{0}, {1}", iconFile, shfi.iIcon);
                }
            }
            finally
            {
                if (retVal.Equals(IntPtr.Zero))
                {
                    if (shfi.hIcon != default(IntPtr))
                        NativeMethods.DestroyIcon(shfi.hIcon);
                }
            }
        }

        private static void getAttributes(bool isDirectory,
                                   bool forceLoadFromDisk,
                                   IconSize _size,
                                   out FileAttribute dwAttr, out SHGFI dwFlags)
        {
            dwFlags = SHGFI.SHGFI_SYSICONINDEX;
            dwAttr = 0;

            if (_size == IconSize.small)
                dwFlags |= SHGFI.SHGFI_SMALLICON;

            if (isDirectory)
            {
                dwAttr = FileAttribute.FILE_ATTRIBUTE_DIRECTORY;
            }
            else
                if (!forceLoadFromDisk)
                {
                    dwFlags |= SHGFI.SHGFI_USEFILEATTRIBUTES;
                    dwAttr = FileAttribute.FILE_ATTRIBUTE_NORMAL;
                }
        }
    }
}
