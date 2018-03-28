using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace System.IO
{
    public static class PathEx
    {
        public static string GetDirectoryName(string path)
        {
            if (path.EndsWith("\\"))
                path = path.Substring(0, path.Length - 1); //Remove ending slash.

            int idx = path.LastIndexOf('\\');
            if (idx == -1)
                return "";
            return path.Substring(0, idx);
        }

        public static string GetFileName(string path)
        {
            int idx = path.LastIndexOf('\\');
            if (idx == -1)
                return path;
            return path.Substring(idx + 1);
        }

        public static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public static string ChangeExtension(string path, string extension)
        {
            return Path.ChangeExtension(path, extension);
        }

        //20: Fixed crash when no extension in path.
        public static string RemoveExtension(string path)
        {
            if (!String.IsNullOrEmpty(PathEx.GetExtension(path)))
                return path.Replace(PathEx.GetExtension(path), "");
            return path;
        }

        public static string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public static string FullNameToGuidName(string fullName)
        {
            if (fullName.IndexOf(DirectoryInfoEx.CurrentUserDirectory.FullName) != -1)
                return fullName.Replace(DirectoryInfoEx.CurrentUserDirectory.FullName, IOTools.IID_UserFiles);

            if (fullName.IndexOf(DirectoryInfoEx.SharedDirectory.FullName) != -1)
                return fullName.Replace(DirectoryInfoEx.SharedDirectory.FullName, IOTools.IID_Public);

            return fullName;
        }

    }
}
