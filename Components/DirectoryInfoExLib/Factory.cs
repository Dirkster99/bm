namespace DirectoryInfoExLib
{
    using DirectoryInfoExLib.Interfaces;
    using DirectoryInfoExLib.IO;
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
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Factory()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static IDirectoryBrowser CreateDirectoryInfoEx(string fullPath)
        {
            Logger.InfoFormat("fullPath {0}", fullPath);

            return new DirectoryInfoEx(fullPath);
        }

        /// <summary>
        /// </summary>
        /// <param name="knownFolder"></param>
        /// <returns></returns>
        public static IDirectoryBrowser CreateDirectoryInfoEx(KnownFolder knownFolder)
        {
            Logger.InfoFormat("knownFolder.Path {0}", knownFolder.Path);

            return new DirectoryInfoEx(knownFolder);
        }

        /// <summary>
        /// Gets the <see cref="IDirectoryInfoEx"/> interface
        /// for a user's desktop folder.
        /// </summary>
        public static IDirectoryBrowser DesktopDirectory
        {
            get
            {
                return DirectoryInfoEx.DesktopDirectory;
            }
        }

        /// <summary>
        /// Gets the <see cref="IDirectoryInfoEx"/> interface
        /// for a user's MyComputer (virtual folder).
        /// </summary>
        public static IDirectoryBrowser MyComputer
        {
            get
            {
                return DirectoryInfoEx.MyComputerDirectory;
            }
        }

        /// <summary>
        /// Gets the <see cref="IDirectoryInfoEx"/> interface
        /// for a user's full name (virtual folder).
        /// </summary>
        public static IDirectoryBrowser CurrentUserDirectory
        {
            get
            {
                return DirectoryInfoEx.CurrentUser;
            }
        }

        /// <summary>
        /// Gets the <see cref="IDirectoryInfoEx"/> interface
        /// for the Public Documents folder (%PUBLIC%\Documents).
        /// </summary>
        public static IDirectoryBrowser SharedDirectory
        {
            get
            {
                return DirectoryInfoEx.SharedDirectory;
            }
        }

        /// <summary>
        /// Gets the <see cref="IDirectoryInfoEx"/> interface
        /// for the Network (virtual folder, Legacy: My Network Places).
        /// </summary>
        public static IDirectoryBrowser Network
        {
            get
            {
                return DirectoryInfoEx.NetworkDirectory;
            }
        }

        /// <summary>
        /// Gets the <see cref="IDirectoryInfoEx"/> interface
        /// for the Rycycle Bin (virtual folder).
        /// </summary>
        public static IDirectoryBrowser RecycleBin
        {
            get
            {
                return DirectoryInfoEx.RecycleBinDirectory;
            }
        }

        /// <summary>
        /// Gets the <see cref="IDirectoryInfoEx"/> interface
        /// for the string representation in <paramref name="path"/>
        /// or null if directory does not exist.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IDirectoryBrowser FromString(string path)
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
        public static bool HasParent(IDirectoryBrowser child, string parentFullName)
        {
            Logger.InfoFormat("parentFullName {0}", parentFullName);

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

        /// <summary>
        /// Return whether parent directory contain child directory.
        /// Aware Library, UserFiles and Public directory too.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool HasParent(IDirectoryBrowser child, IDirectoryBrowser parent)
        {
            //0.13: Added HasParent
            if (parent == null)
            {
                //if (Debugger.IsAttached)
                //    Debugger.Break();
                return false;
            }

            Logger.InfoFormat("child.FullName '{0}' parent.FullName '{1}'", child.FullName, parent.FullName);

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
            Logger.InfoFormat("fullName '{0}'", fullName);

            return (fullName.StartsWith(DirectoryInfoEx.IID_Library) &&
                    fullName.EndsWith(".library-ms"));
        }

        /// <summary>
        /// Make sure that a path reference does actually work with
        /// <see cref="System.IO.DirectoryInfo"/> by replacing 'C:' by 'C:\'.
        /// </summary>
        /// <param name="dirOrfilePath"></param>
        /// <returns></returns>
        public static string NormalizePath(string dirOrfilePath)
        {
            if (string.IsNullOrEmpty(dirOrfilePath) == true)
                return null;

            // The dirinfo constructor will not work with 'C:' but does work with 'C:\'
            if (dirOrfilePath.Length < 2)
                return null;

            if (dirOrfilePath.Length == 2)
            {
                if (dirOrfilePath[dirOrfilePath.Length - 1] == ':')
                    return dirOrfilePath + System.IO.Path.DirectorySeparatorChar;
            }

            if (dirOrfilePath.Length == 3)
            {
                if (dirOrfilePath[dirOrfilePath.Length - 2] == ':' &&
                    dirOrfilePath[dirOrfilePath.Length - 1] == System.IO.Path.DirectorySeparatorChar)
                    return dirOrfilePath;

                return "" + dirOrfilePath[0] + dirOrfilePath[1] +
                            System.IO.Path.DirectorySeparatorChar + dirOrfilePath[2];
            }

            // Insert a backslash in 3rd character position if not already present
            // C:Temp\myfile -> C:\Temp\myfile
            if (dirOrfilePath.Length >= 3)
            {
                if (char.ToUpper(dirOrfilePath[0]) >= 'A' && char.ToUpper(dirOrfilePath[0]) <= 'Z' &&
                    dirOrfilePath[1] == ':' &&
                    dirOrfilePath[2] != '\\')
                {
                    dirOrfilePath = dirOrfilePath.Substring(0, 2) + "\\" + dirOrfilePath.Substring(2);
                }
            }

            // This will normalize directory and drive references into 'C:' or 'C:\Temp'
            if (dirOrfilePath[dirOrfilePath.Length - 1] == System.IO.Path.DirectorySeparatorChar)
                dirOrfilePath = dirOrfilePath.Trim(System.IO.Path.DirectorySeparatorChar);

            return dirOrfilePath;
        }

        /// <summary>
        /// Split the current folder in an array of sub-folder names and return it.
        /// </summary>
        /// <returns>Returns a string array of sub-folder names (including drive)
        /// or null if folder reference is invalid.</returns>
        public static string[] GetFolderSegments(string folder)
        {
            if (string.IsNullOrEmpty(folder) == true)
                return null;

            folder = NormalizePath(folder);

            string[] dirs = null;

            try
            {
                dirs = folder.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                if (dirs != null)
                {
                    if (dirs[0].Length == 2)                                        // Normalizing Drive representation
                    {                                                              // from 'C:' to 'C:\'
                        if (char.ToUpper(dirs[0][0]) >= 'A' && char.ToUpper(dirs[0][0]) <= 'Z' &&
                            dirs[0][1] == ':')                                    // to ensure correct processing
                            dirs[0] += System.IO.Path.DirectorySeparatorChar;    // since 'C:' is technically invalid(!)
                    }
                }
            }
            catch
            {
            }

            return dirs;
        }
    }
}
