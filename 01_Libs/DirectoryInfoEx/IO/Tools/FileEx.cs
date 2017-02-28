///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2009 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Steven Roebert's work (http://www.codeproject.com/KB/miscctrl/FileBrowser.aspx)    //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using ShellDll;
using System.Runtime.InteropServices;

namespace System.IO
{
    public static class FileEx
    {
        public static bool Exists(string path)
        {
            try
            {
                FileSystemInfoEx fsInfo = new FileSystemInfoEx(path);
                return fsInfo != null && !fsInfo.IsFolder && fsInfo.Exists;
            }
            catch { return false; }
        }

        public static void Delete(string path)
        {
            new FileInfoEx(path).Delete();
        }

        public static void Move(string source, string dest)
        {
            IOTools.Move(source, dest);
            //new FileInfoEx(source).MoveTo(dest);
        }

        public static void Copy(string source, string dest)
        {
            if (!FileEx.Exists(source))
                throw new IOException("Source not exist.");
            if (FileEx.Exists(dest))
                throw new IOException("Dest already exist.");

            IOTools.Copy(source, dest);            
        }

        public static FileStreamEx OpenRead(string path)
        {
            return new FileInfoEx(path).OpenRead();
        }

        public static StreamWriter AppendText(string path)
        {
            return new FileInfoEx(path).AppendText();
        }
        

        public static StreamReader OpenText(string path)
        {            
            return new FileInfoEx(path).OpenText();
        }

        public static IEnumerable<string> ReadLines(string path, Encoding encoding)
        {            
            using (StreamReader sr = new StreamReader(OpenRead(path), encoding))
            {
                while (!sr.EndOfStream)
                    yield return sr.ReadLine();
            }
        }

        public static IEnumerable<string> ReadLines(string path)
        {
            using (StreamReader sr = OpenText(path))
            {
                while (!sr.EndOfStream)
                    yield return sr.ReadLine();
            }
        }

        public static string[] ReadAllLines(string path, Encoding encoding)
        {
            return new List<string>(ReadLines(path, encoding)).ToArray();
        }

        public static string[] ReadAllLines(string path)
        {
            return new List<string>(ReadLines(path)).ToArray();
        }

        public static FileStreamEx Open(string path, FileMode mode, FileAccess access)
        {
            return new FileInfoEx(path).Open(mode, access);        
        }

        public static FileStreamEx Open(string path, FileMode mode)
        {
            return new FileInfoEx(path).Open(mode);
        }

        public static FileStreamEx Open(string path)
        {
            return new FileInfoEx(path).Open();
            
        }

        public static FileStreamEx Create(string path)
        {
            return new FileInfoEx(path).Create();
        }

    }
}
