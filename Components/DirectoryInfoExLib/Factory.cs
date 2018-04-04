namespace DirectoryInfoExLib
{
    using DirectoryInfoExLib.Interfaces;
    using DirectoryInfoExLib.IO.FileSystemInfoExt;
    using DirectoryInfoExLib.IO.Header.KnownFolder;
    using DirectoryInfoExLib.IO.Header.KnownFolder.Enums;
    using System.Runtime.Serialization;

    /// <summary>
    /// Implements a factory for generating internal classes that are otherwise
    /// accessible through public interface definitions only. Use this factory to generate
    /// the main classes and work with their properties and methods that are accessible
    /// through their related interface definitions.
    /// </summary>
    public sealed class Factory
    {
        private Factory() { }

        /// <summary>
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static IDirectoryInfoEx CreateDirectoryInfoEx(string fullPath)
        {
            return new DirectoryInfoEx(fullPath);
        }

        /// <summary>
        /// </summary>
        /// <param name="knownFolder"></param>
        /// <returns></returns>
        public static IDirectoryInfoEx CreateDirectoryInfoEx(KnownFolder knownFolder)
        {
            return new DirectoryInfoEx(knownFolder);
        }

        /// <summary>
        /// </summary>
        /// <param name="knownFolderId"></param>
        /// <returns></returns>
        public static IDirectoryInfoEx CreateDirectoryInfoEx(KnownFolderIds knownFolderId)
        {
            return new DirectoryInfoEx(knownFolderId);
        }

        /// <summary>
        /// </summary>
        /// <param name="csidl"></param>
        /// <returns></returns>
        public static IDirectoryInfoEx CreateDirectoryInfoEx(IO.Header.ShellDll.ShellAPI.CSIDL csidl)
        {
            return new DirectoryInfoEx(csidl);
        }

        /// <summary>
        /// </summary>
        /// <param name="shellFolder"></param>
        /// <returns></returns>
        public static IDirectoryInfoEx CreateDirectoryInfoEx(System.Environment.SpecialFolder shellFolder)
        {
            return new DirectoryInfoEx(shellFolder);
        }

        /// <summary>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IDirectoryInfoEx CreateDirectoryInfoEx(SerializationInfo info,
                                                             StreamingContext context)
        {
            return new DirectoryInfoEx(info, context);
        }

        public static DirectoryInfoEx DesktopDirectory
        {
            get
            {
                return DirectoryInfoEx.DesktopDirectory;
            }
        }

        public static DirectoryInfoEx MyComputerDirectory
        {
            get
            {
                return DirectoryInfoEx.MyComputerDirectory;
            }
        }

        public static DirectoryInfoEx CurrentUserDirectory
        {
            get
            {
                return DirectoryInfoEx.CurrentUserDirectory;
            }
        }

        public static DirectoryInfoEx SharedDirectory
        {
            get
            {
                return DirectoryInfoEx.SharedDirectory;
            }
        }

        public static DirectoryInfoEx NetworkDirectory
        {
            get
            {
                return DirectoryInfoEx.NetworkDirectory;
            }
        }

        public static DirectoryInfoEx RecycleBinDirectory
        {
            get
            {
                return DirectoryInfoEx.RecycleBinDirectory;
            }
        }
    }
}
