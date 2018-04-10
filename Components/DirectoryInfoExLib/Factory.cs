namespace DirectoryInfoExLib
{
    using DirectoryInfoExLib.Interfaces;
    using DirectoryInfoExLib.IO.FileSystemInfoExt;
    using DirectoryInfoExLib.IO.Header.KnownFolder;
    using System;

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

        /// <summary>
        /// Return whether parent directory contain child directory.
        /// Aware UserFiles and Public directory too.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parentFullName"></param>
        /// <returns></returns>
        public static bool HasParent(IDirectoryInfoEx child, string parentFullName)
        {
            if (child.FullName.StartsWith(parentFullName, StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (parentFullName == DirectoryInfoEx.DesktopDirectory.FullName)
                return true;

            if ((child.FullName.StartsWith(DirectoryInfoEx.IID_UserFiles) ||
                 child.FullName.StartsWith(DirectoryInfoEx.IID_Public)))
                return false;

            var current = child.Parent;
            while (current != null && parentFullName != current.FullName)
                current = current.Parent;

            return (current != null);
        }

        //0.13: Added HasParent
        /// <summary>
        /// Return whether parent directory contain child directory.
        /// Aware Library, UserFiles and Public directory too.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool HasParent(IDirectoryInfoEx child, IDirectoryInfoEx parent)
        {
            if (parent == null)
            {
                //if (Debugger.IsAttached)
                //    Debugger.Break();
                return false;
            }

            //::{031E4825-7B94-4DC3-B131-E946B44C8DD5}\Music.library-ms
            if (parent.FullName.StartsWith(DirectoryInfoEx.IID_Library) && parent.FullName.EndsWith(".library-ms"))
            {
                //Reverse
                foreach (DirectoryInfoEx subDir in parent.GetDirectories())
                    if (subDir.Equals(child) || HasParent(child, subDir))
                        return true;
                return false;
            }
            else
            {
                if (child.FullName.StartsWith(parent.FullName.TrimEnd('\\') + "\\", StringComparison.InvariantCultureIgnoreCase))
                    return true;

                if (child.FullName.StartsWith(DirectoryInfoEx.IID_UserFiles) || child.FullName.StartsWith(DirectoryInfoEx.IID_Public))
                    return false;

                var current = child.Parent;
                while (current != null && !parent.Equals(current))
                    current = current.Parent;

                return (current != null);
            }
        }

        //0.12: IsLibraryItem
        /// <summary>
        /// Return whether if specific path is part of the library item (e.g. Picture, Musics)
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static bool IsLibraryItem(string fullName)
        {
            return (fullName.StartsWith(DirectoryInfoEx.IID_Library) &&
                    fullName.EndsWith(".library-ms"));
        }
    }
}
