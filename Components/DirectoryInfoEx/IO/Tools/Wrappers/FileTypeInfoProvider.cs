using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ShellDll;
using System.Diagnostics;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace System.IO.Tools
{
    public enum OpenWithType { OpenWithList, OpenWithProgIds }

    public class OpenWithInfo : IComparable<OpenWithInfo>
    {
        public static OpenWithInfo OpenAs = new OpenWithInfo() { OpenCommand = "OpenAs", KeyName = "OpenAs" };

        public string Description { get; set; }
        /// <summary>
        /// ProgIDs or ExeName (OpenWithList), can be found under HKCR
        /// </summary>
        public string KeyName { get; set; }
        /// <summary>
        /// Default open command, HKCR\KeyName\Shell\Open\Command
        /// </summary>
        public string OpenCommand { get; set; }

        public static string GetExecutablePath(string command)
        {
            Match match = Regex.Match(command, "\"(?<FileName>[^\"]*)\".*");
            return match.Groups["FileName"].Value;
            //command = command.Replace("\"%1\"", "").Replace("%1", "");
            //int idx = command.IndexOf("/");
            //if (idx != -1)
            //    command = command.Substring(0, idx - 1);
            //return command.Trim(new char[] { '"', ' ' });
        }

        /// <summary>
        /// Listed under OpenWithList or OpenWithProgIds
        /// </summary>
        public OpenWithType OpenWithType { get; set; }




        #region IComparable<OpenWithInfo> Members

        public int CompareTo(OpenWithInfo other)
        {
            return KeyName.CompareTo(other.KeyName);
        }

        #endregion
    }

    public class FileTypeInfo
    {
        public FileTypeInfo(string ext)
        {
            Debug.Assert(ext.StartsWith("."));
            Ext = ext;
            FileType = getFileType("AAA" + ext);
        }

        public string Ext { get; private set; }

        #region FileType
        public string FileType { get; private set; }
        /// <summary>
        /// Return file type of the specified file.
        /// </summary>
        private static string getFileType(string fname)
        {
            ShellDll.ShellAPI.SHFILEINFO shinfo = new ShellDll.ShellAPI.SHFILEINFO();

            ShellAPI.SHGetFileInfo(fname, 0, ref shinfo,
                          (int)Marshal.SizeOf(shinfo),
                          ShellAPI.SHGFI.TYPENAME | ShellAPI.SHGFI.SYSICONINDEX | ShellAPI.SHGFI.USEFILEATTRIBUTES
                          );
            return shinfo.szTypeName;
        }
        #endregion

        #region OpenWithInfo

        private OpenWithInfo[] _openWithList = null;
        public OpenWithInfo[] OpenWithList { get { if (_openWithList == null) _openWithList = getOpenWithInfoList(Ext); return _openWithList; } }


        private static string getOpenCommand(string keyName, bool appDir)
        {
            string regKey = (appDir ? "Applications\\" : "") + keyName + "\\shell\\open\\command";
            using (RegistryKey rk = Registry.ClassesRoot.OpenSubKey(regKey, false))
                return rk != null ? (string)rk.GetValue("") : null;
        }

        private static string getAppPath(string appName)
        {
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\" + appName, false))
                return rk != null ? (string)rk.GetValue("") : null;
        }


        private static string getPerceivedType(string ext)
        {
            string regKeyListPath = ext;
            using (RegistryKey rkKeyList = Registry.ClassesRoot.OpenSubKey(regKeyListPath, false))
                return rkKeyList != null ? rkKeyList.GetValue("PerceivedType", null) as string : null;
        }

        private static string[] getOpenWithList(string ext, OpenWithType openWithType)
        {
            List<string> retList = new List<string>();

            string regKeyListPath = ext + "\\" + (openWithType == OpenWithType.OpenWithList ? "OpenWithList" : "OpenWithProgids");
            string[] keyList = new string[0];
            using (RegistryKey rkKeyList = Registry.ClassesRoot.OpenSubKey(regKeyListPath, false))
                switch (openWithType)
                {
                    case OpenWithType.OpenWithList:
                        keyList = rkKeyList != null ? rkKeyList.GetSubKeyNames() : new string[0];
                        break;
                    case OpenWithType.OpenWithProgIds:
                        keyList = rkKeyList != null ? rkKeyList.GetValueNames() : new string[0];
                        break;
                }

            return keyList;
        }

        private static string getExePath(string paramString)
        {
            int firstQuote = paramString.IndexOf('"');
            int secQuote = firstQuote != 0 ? paramString.IndexOf(' ') : paramString.IndexOf('"', firstQuote + 1);

            if (secQuote != -1)
                paramString = paramString.Substring(0, secQuote);

            paramString = paramString.Trim('"');
            return paramString;
        }

        private static string getExeName(string paramString)
        {
            paramString = getExePath(paramString);
            paramString = Path.GetFileNameWithoutExtension(paramString);
            return paramString;
        }

        private static OpenWithInfo KeyToOpenWithInfo(string key, OpenWithType openWithType)
        {
            string openCommand = getOpenCommand(key, false);
            string description = null;

            if (openCommand == null) openCommand = getOpenCommand(key, true);
            if (openCommand == null) openCommand = getAppPath(key);

            if (openCommand != null)
            {
                description = getExeName(openCommand);
                openCommand = openCommand.IndexOf("%1") != -1 ? openCommand : openCommand + " \"%1\"";
            }

            return new OpenWithInfo()
            {
                KeyName = key,
                OpenCommand = openCommand,
                OpenWithType = openWithType,
                Description = description
            };
        }

        private static OpenWithInfo[] KeyListToOpenWithInfoList(string[] keys, OpenWithType openWithType)
        {
            List<OpenWithInfo> retList = new List<OpenWithInfo>();
            foreach (string key in keys)
                retList.Add(KeyToOpenWithInfo(key, openWithType));
            return retList.ToArray();
        }

        private static OpenWithInfo[] getOpenWithInfoList(string ext)
        {
            List<OpenWithInfo> retList = new List<OpenWithInfo>();

            //string perceivedType = getPerceivedType(ext);
            //if (perceivedType != null)

            retList.AddRange(KeyListToOpenWithInfoList(getOpenWithList(ext, OpenWithType.OpenWithList), OpenWithType.OpenWithList));
            retList.AddRange(KeyListToOpenWithInfoList(getOpenWithList(ext, OpenWithType.OpenWithProgIds), OpenWithType.OpenWithProgIds));

            if (retList.Count > 0)
            {
                retList.Sort();
            }
            return retList.ToArray();
        }

        #endregion
    }

    public static class FileTypeInfoProvider
    {
        private static Dictionary<string, FileTypeInfo> _fileInfoDic = new Dictionary<string, FileTypeInfo>();

        public static FileTypeInfo GetFileTypeInfo(string ext)
        {
            ext = ext.StartsWith(".") ? ext.ToLower() : "." + ext.ToLower();
            lock (_fileInfoDic)
            {
                if (!_fileInfoDic.ContainsKey(ext))
                    _fileInfoDic.Add(ext, new FileTypeInfo(ext));
                return _fileInfoDic[ext];
            }
        }

    }
}
