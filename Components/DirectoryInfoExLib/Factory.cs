namespace DirectoryInfoExLib
{
    using DirectoryInfoExLib.Interfaces;
    using DirectoryInfoExLib.IO.FileSystemInfoExt;
    using DirectoryInfoExLib.IO.Header.KnownFolder;

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
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <returns></returns>
////        public static IDirectoryInfoEx CreateDirectoryInfoEx(SerializationInfo info,
////                                                             StreamingContext context)
////        {
////            return new DirectoryInfoEx(info, context);
////        }

        public static IDirectoryInfoEx DesktopDirectory
        {
            get
            {
                return DirectoryInfoEx.DesktopDirectory;
            }
        }

        public static IDirectoryInfoEx MyComputerDirectory
        {
            get
            {
                return DirectoryInfoEx.MyComputerDirectory;
            }
        }

        public static IDirectoryInfoEx CurrentUserDirectory
        {
            get
            {
                return DirectoryInfoEx.CurrentUserDirectory;
            }
        }

        public static IDirectoryInfoEx SharedDirectory
        {
            get
            {
                return DirectoryInfoEx.SharedDirectory;
            }
        }

        public static IDirectoryInfoEx NetworkDirectory
        {
            get
            {
                return DirectoryInfoEx.NetworkDirectory;
            }
        }

        public static IDirectoryInfoEx RecycleBinDirectory
        {
            get
            {
                return DirectoryInfoEx.RecycleBinDirectory;
            }
        }

        public static IDirectoryInfoEx FromString(string path)
        {
            return DirectoryInfoEx.FromString(path);
        }
    }
}
