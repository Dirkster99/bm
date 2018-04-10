namespace DirectoryInfoExLib.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    //using VistaBridge.Shell;
    using System.Diagnostics;
    using System.Linq;
    using DirectoryInfoExLib.IO.Header.ShellDll;
    using System.IO;
    using DirectoryInfoExLib.IO.Tools.Interface;
    using DirectoryInfoExLib.IO.Header.ShellDll.Interfaces;
    using DirectoryInfoExLib.IO.FileSystemInfoExt;
    using DirectoryInfoExLib.IO.Header.KnownFolder.Enums;
    using DirectoryInfoExLib.IO.Header.KnownFolder.Attributes;
    using DirectoryInfoExLib.IO.Tools;
    using DirectoryInfoExLib.Interfaces;

    public delegate bool FileCancelDelegate(ushort completePercent);
    public delegate bool CancelDelegate();
    public static class IOTools
    {
        #region Constants
        public static string IID_Desktop = "::{00021400-0000-0000-C000-000000000046}";
        public static string IID_UserFiles = "::{59031A47-3F72-44A7-89C5-5595FE6b30EE}";
        public static string IID_Public = "::{4336A54D-038B-4685-AB02-99BB52D3FB8B}";
        public static string IID_Library = "::{031E4825-7B94-4DC3-B131-E946B44C8DD5}";
        #endregion

        #region Path routines

        /// <summary>
        /// Expand environment path to parasable path.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string ExpandPath(string fullPath)
        {
            fullPath = Helper.RemoveSlash(fullPath);
            if (fullPath.StartsWith("%"))
                fullPath = Environment.ExpandEnvironmentVariables(fullPath);
            return fullPath;
        }

        /// <summary>
        /// Centralized method to return hash code.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static int GetHashCode(string fullPath)
        {
            return fullPath.ToLower().GetHashCode();
        }

        /// <summary>
        /// Centralized method to return hash code.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        internal static int GetHashCode(FileSystemInfoEx item)
        {
            return GetHashCode(item.FullName);
        }

        /// <summary>
        /// Return whether PIDL match fileMask ( * and ? supported)
        /// </summary>
        /// <param name="pidl"></param>
        /// <param name="fileMask"></param>
        /// <returns></returns>
        internal static bool MatchFileMask(PIDL pidl, string fileMask)
        {
            string path = FileSystemInfoEx.PIDLToPath(pidl);
            string name = PathEx.GetFileName(path);
            return MatchFileMask(name, fileMask);
        }

        private static string constructFileMaskRegexPattern(string fileMask, bool forceSlashCheck)
        {
            if (!forceSlashCheck)
            {
                return '^' +
                Regex.Escape(fileMask.Replace(".", "__DOT__")
                                .Replace("*", "__STAR__")
                                .Replace("?", "__QM__"))
                    .Replace("__DOT__", "[.]")
                    .Replace("__STAR__", ".*")
                    .Replace("__QM__", ".")
                + '$';
            }
            else
            {
                return '^' +
                 Regex.Escape(fileMask.Replace(".", "__DOT__")
                                 .Replace("\\", "__SLASH__")
                                 .Replace("**", "__DOUBLESTAR__")
                                 .Replace("*", "__STAR__")
                                 .Replace("#", "__VARIABLE__")
                                 .Replace("(", "__OPENQUOTE__")
                                 .Replace(")", "__CLOSEQUOTE__")
                                 .Replace("?", "__QM__"))
                     .Replace("__DOT__", "[.]")
                     .Replace("__DOUBLESTAR__", ".*")
                     .Replace("__STAR__", "[^\\\\]*")
                     .Replace("__SLASH__", "[\\\\]")
                     .Replace("__VARIABLE__", "?")
                     .Replace("__OPENQUOTE__", "(")
                     .Replace("__CLOSEQUOTE__", ")")
                     .Replace("__QM__", ".")
                 + '$';
            }
        }

        public static bool MatchFileMask(string fileName, string fileMask, bool forceSlashCheck, out Match match)
        {
            string pattern = constructFileMaskRegexPattern(fileMask, forceSlashCheck);
            match = new Regex(pattern, RegexOptions.IgnoreCase).Match(fileName);
            return match.Success;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/725341/c-file-mask
        /// Return whether filename match fileMask ( * and ? supported)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileMask"></param>
        /// <returns></returns>
        public static bool MatchFileMask(string fileName, string fileMask, bool forceSlashCheck)
        {
            if (fileName.Equals(fileMask, StringComparison.InvariantCultureIgnoreCase))
                return true;
            if (fileMask == "*.*" || fileMask == "*" && !forceSlashCheck)
                return true;

            string pattern = constructFileMaskRegexPattern(fileMask, forceSlashCheck);

            return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(fileName);
        }

        public static bool MatchFileMask(string fileName, string fileMask)
        {
            return MatchFileMask(fileName, fileMask, false);
        }

        public static bool MatchFileMasks(string fileName, string fileMasks)
        {
            string[] fileMaskList = fileMasks.Split(new char[] { ',', ';' });
            foreach (string fileMask in fileMaskList)
                if (MatchFileMask(fileName, fileMask))
                    return true;
            return false;
        }


        /// <summary>
        /// Get relative path of a entry based on baseDirectory.
        /// e.g. C:\Temp\AbC\1.txt (entry), C:\Temp\ (baseDirectory) will return ABC\1.txt
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="baseDirectory"></param>
        /// <returns></returns>
////        internal static string GetRelativePath(FileSystemInfoEx entry, IDirectoryInfoEx baseDirectory)
////        {
////            if (entry.FullName.IndexOf(baseDirectory.FullName, StringComparison.InvariantCultureIgnoreCase) == -1)
////            {
////                if (Debugger.IsAttached)
////                    Debugger.Break();
////                return entry.Name;
////            }
////            else return entry.FullName.Substring(baseDirectory.FullName.Length + 1);
////        }

        /// <summary>
        /// Get relative path of a entry based on baseDirectory.
        /// e.g. C:\Temp\AbC\1.txt (entry), C:\Temp\ (baseDirectory) will return ABC\1.txt
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="baseDirectory"></param>
        /// <returns></returns>
////        public static string GetRelativePath(string name, IDirectoryInfoEx baseDirectory)
////        {
////            if (name.IndexOf(baseDirectory.FullName, StringComparison.InvariantCultureIgnoreCase) == -1)
////            {
////                if (Debugger.IsAttached)
////                    Debugger.Break();
////                return PathEx.GetFileName(name);
////            }
////            else return name.Substring(baseDirectory.FullName.Length + 1);
////        }
        #endregion

        #region PIDL Array routines

        /// <summary>
        /// Return a PIDL List from one or more FileSystemInfoExs
        /// </summary>
        /// <param name="item"></param>
        /// <param name="relative"></param>
        /// <returns></returns>
        internal static PIDL[] GetPIDL(FileSystemInfoEx[] items, bool relative)
        {
            List<PIDL> retVal = new List<PIDL>();
            foreach (FileSystemInfoEx fi in items)
                if (fi.Exists)
                    fi.RequestPIDL((pidl, relPidl) =>
                        {
                            retVal.Add(relative ? relPidl : pidl);
                        });
            return retVal.ToArray();
        }

        /// <summary>
        /// Return a PIDL List from one or more FileSystemInfoExs
        /// </summary>
        /// <param name="item"></param>
        /// <param name="relative"></param>
        /// <returns></returns>
        internal static PIDL[] GetPIDL(FileSystemInfoEx item, bool relative)
        {
            return GetPIDL(new FileSystemInfoEx[] { item }, relative);
        }

        /// <summary>
        /// Free up a PIDL IntPtr List.
        /// </summary>
        /// <param name="pidls"></param>
        internal static void FreePIDL(PIDL[] pidls)
        {
            foreach (PIDL pidl in pidls)
                pidl.Free();
        }


        /// <summary>
        /// Extract IntPtr for the specified item(s), the result pointer is still owned by the input PIDL, thus dont needed to be freed.
        /// </summary>
        internal static IntPtr[] GetPIDLPtr(PIDL[] items)
        {
            List<IntPtr> retVal = new List<IntPtr>();
            foreach (PIDL pidl in items)
                retVal.Add(pidl.Ptr);
            return retVal.ToArray();
        }

        /// <summary>
        /// Extract IntPtr for the specified item(s), the result pointer is still owned by the input PIDL, thus dont needed to be freed.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static IntPtr[] GetPIDLPtr(PIDL item)
        {
            return GetPIDLPtr(new PIDL[] { item });
        }
        #endregion

        #region FileEx routines

        //0.12: IsLibraryItem
        /// <summary>
        /// Return whether if specific path is part of the library item (e.g. Picture, Musics)
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static bool IsLibraryItem(string fullName)
        {
            return (fullName.StartsWith(IOTools.IID_Library) && fullName.EndsWith(".library-ms"));
        }

        //0.13: Added HasParent
        /// <summary>
        /// Return whether parent directory contain child directory.
        /// Aware Library, UserFiles and Public directory too.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        internal static bool HasParent(FileSystemInfoEx child, IDirectoryInfoEx parent)
        {
            if (parent == null)
            {
                //if (Debugger.IsAttached)
                //    Debugger.Break();
                return false;
            }

            //::{031E4825-7B94-4DC3-B131-E946B44C8DD5}\Music.library-ms
            if (parent.FullName.StartsWith(IOTools.IID_Library) && parent.FullName.EndsWith(".library-ms"))
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

                if (child.FullName.StartsWith(IID_UserFiles) || child.FullName.StartsWith(IID_Public))
                    return false;

                var current = child.Parent;
                while (current != null && !parent.Equals(current))
                    current = current.Parent;

                return (current != null);
            }
        }

        /// <summary>
        /// Return whether parent directory contain child directory.
        /// Aware UserFiles and Public directory too.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parentFullName"></param>
        /// <returns></returns>
        internal static bool HasParent(FileSystemInfoEx child, string parentFullName)
        {
            if (child.FullName.StartsWith(parentFullName, StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (parentFullName == DirectoryInfoEx.DesktopDirectory.FullName)
                return true;

            if ((child.FullName.StartsWith(IID_UserFiles) || child.FullName.StartsWith(IID_Public)))
                return false;

            var current = child.Parent;
            while (current != null && parentFullName != current.FullName)
                current = current.Parent;

            return (current != null);
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
            return HasParent(child as FileSystemInfoEx, parentFullName);
        }

        public static bool Exists(string path)
        {
            return new FileSystemInfoEx(path).Exists;
        }

        internal static bool IsZip(IO.Header.ShellDll.ShellAPI.SFGAO attribs)
        {
            return ((attribs & IO.Header.ShellDll.ShellAPI.SFGAO.FOLDER) != 0 && (attribs & IO.Header.ShellDll.ShellAPI.SFGAO.STREAM) != 0);
        }

        /// <summary>
        /// Rename a file or folder in an directory.
        /// </summary>
        public static void Rename(string source, string destName)
        {
            //DirectoryInfoEx srcDir = new DirectoryInfoEx(Path.GetDirectoryName(source));
            FileSystemInfoEx srcElement = new FileSystemInfoEx(source);
            string srcName = Path.GetFileName(source);

            if (!srcElement.Exists || srcElement.Parent == null)
                throw new IOException("Source not exists");

            //0.15: Fixed ShellFolder not freed.
            using (ShellFolder2 srcParentShellFolder = (srcElement.Parent as DirectoryInfoEx).ShellFolder)
            {
                if (srcParentShellFolder == null)
                    throw new IOException("Source directory does not support IShellFolder");

                int hr = srcElement.RequestRelativePIDL(srcRelPidl =>
                    {
                        IntPtr tmpPtr;
                        int retVal = srcParentShellFolder.SetNameOf(IntPtr.Zero, srcRelPidl.Ptr,
                            destName, ShellAPI.SHGNO.FORPARSING, out tmpPtr);

                        PIDL tmpPIDL = new PIDL(tmpPtr, false); //consume the IntPtr, and free it.
                        tmpPIDL.Free();

                        return retVal;
                    });

                if (hr != ShellAPI.S_OK)
                    Marshal.ThrowExceptionForHR(hr);
            }
        }

        public static bool IsCancelTriggered(CancelDelegate cancel)
        {
            return cancel != null && cancel();
        }

        public static bool IsCancelTriggered(FileCancelDelegate cancel, ushort completePercent)
        {
            return cancel != null && cancel(completePercent);
        }
        #endregion
    }
}
