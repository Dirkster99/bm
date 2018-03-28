using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO
{
    public static class DirectoryEx
    {
        public static DirectoryInfoEx CreateDirectory(string path) 
        {
            DirectoryInfoEx dir = new DirectoryInfoEx(path);
            dir.Create();
            return dir;
            //if (DirectoryEx.Exists(path))
            //    throw new IOException(path + " already exist.");
            
            //string name = Path.GetFileName(path);
            //DirectoryInfoEx rootDir = new DirectoryInfoEx(Path.GetDirectoryName(path));

            //IntPtr outPtr;
            //rootDir.Storage.CreateStorage(name, ShellDll.ShellAPI.STGM.FAILIFTHERE | 
            //    ShellDll.ShellAPI.STGM.CREATE, 0, 0, out outPtr);
            //Storage storage = new Storage(outPtr);

            //return new DirectoryInfoEx(path);
        }

        public static bool Exists(string path)
        {
            try
            {
                FileSystemInfoEx fsInfo = new FileSystemInfoEx(path);
                return fsInfo != null && fsInfo.IsFolder && fsInfo.Exists;
            }
            catch { return false; }
        }

        public static void Delete(string path)
        {
           new DirectoryInfoEx(path).Delete();
        }

        public static void Move(string sourceDirName, string destDirName)
        {
            IOTools.Move(sourceDirName, destDirName);
        }

        public static void Copy(string source, string dest)
        {
            if (!DirectoryEx.Exists(source))
                throw new IOException("Source not exist.");
            //if (DirectoryEx.Exists(dest))
            //    throw new IOException("Dest already exist.");

            IOTools.Copy(source, dest);

            string[] subFiles = GetFiles(source);
            foreach (string subFile in subFiles)
                IOTools.Copy(subFile, subFile.Replace(source, dest));

            string[] subDirs = GetDirectories(source);
            foreach (string subdir in subDirs)
                Copy(subdir, subdir.Replace(source, dest));
            
        }

        private static string[] FSListToStringList(FileSystemInfoEx[] fsList)
        {
            string[] retVal = new string[fsList.Length];
            for (int i = 0; i < fsList.Length; i++)
                retVal[i] = fsList[i].FullName;
            return retVal;
        }

        public static string[] GetDirectories(string path)
        {
            DirectoryInfoEx rootDir = new DirectoryInfoEx(path);
            return FSListToStringList(rootDir.GetDirectories());
        }

        public static string[] GetFiles(string path)
        {
            DirectoryInfoEx rootDir = new DirectoryInfoEx(path);
            return FSListToStringList(rootDir.GetFiles());
        }

        public static string[] GetFileSystemEntries(string path)
        {
            DirectoryInfoEx rootDir = new DirectoryInfoEx(path);
            return FSListToStringList(rootDir.GetFileSystemInfos());
        }

        #region Enumerate
        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            DirectoryInfoEx rootDir = new DirectoryInfoEx(path);
            IEnumerator<FileInfoEx> fileEnumerator = rootDir.EnumerateFiles(searchPattern, searchOption).GetEnumerator();
            while (fileEnumerator.MoveNext())
                yield return fileEnumerator.Current.FullName;
        }

        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            return EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateFiles(string path)
        {
            return EnumerateFiles(path, "*");
        }

        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            DirectoryInfoEx rootDir = new DirectoryInfoEx(path);
            IEnumerator<DirectoryInfoEx> dirEnumerator = rootDir.EnumerateDirectories(searchPattern, searchOption).GetEnumerator();
            while (dirEnumerator.MoveNext())
                yield return dirEnumerator.Current.FullName;
        }

        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
        {
            return EnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateDirectories(string path)
        {
            return EnumerateDirectories(path, "*");
        }

        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
        {
            DirectoryInfoEx rootDir = new DirectoryInfoEx(path);
            IEnumerator<FileSystemInfoEx> fsiEnumerator = rootDir.EnumerateFileSystemInfos(searchPattern, searchOption).GetEnumerator();
            while (fsiEnumerator.MoveNext())
                yield return fsiEnumerator.Current.FullName;
        }

        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
        {
            return EnumerateFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<string> EnumerateFileSystemEntries(string path)
        {
            return EnumerateFileSystemEntries(path, "*");
        }
        #endregion


        public static DirectoryInfoEx GetParent(string path)
        {
            DirectoryInfoEx dir = new DirectoryInfoEx(path);
            return dir.Parent;
        }


        public static string GetDirectoryRoot(string path)
        {
            return new DirectoryInfoEx(path).Root.FullName;
        }

       
    }
}
