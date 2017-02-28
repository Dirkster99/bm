using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;
using System.Drawing;
using ShellDll;
using System.Runtime.InteropServices;


namespace System.IO
{
    public static class Helper
    {
        public static bool IsUnitTesting = false;


        /// <summary>
        /// Return file crc of the specified file
        /// </summary>
        public static uint GetCRC(Stream stream)
        {
            Fesersoft.Hashing.crc32 crc32 = new Fesersoft.Hashing.crc32();
            uint crchash = (uint)crc32.CRC(stream);
            return crchash;
        }

        public static string ConvertToHex(uint input)
        {
            if (input == 0)
                return "00000000";
            else return String.Format("{0:x2}", input).ToUpper();
        }
        		
		/// <summary>
		/// Return file crc of the specified file
		/// </summary>
		public static string GetFileCRC(string filename)
		{
			if (!FileEx.Exists(filename))
				return "";

            Fesersoft.Hashing.crc32 crc32 = new Fesersoft.Hashing.crc32();
            using (FileStreamEx stmcheck = FileEx.OpenRead(filename))
            {
                uint crchash = (uint)crc32.CRC(stmcheck);
                stmcheck.Close();
                return (ConvertToHex(crchash));
            }
		}
		
		public static string GetFileMD5(string filename)
		{
			if (!FileEx.Exists(filename))
				return "";
			MD5CryptoServiceProvider csp = new MD5CryptoServiceProvider();

            using (FileStreamEx stmcheck = FileEx.OpenRead(filename))
            {
                byte[] hash = csp.ComputeHash(stmcheck);
                stmcheck.Close();

                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
		}



        public static string SizeInK(UInt64 size)
        {
            if (size == 0)
                return "0 kb";

            float sizeink = ((float)size / 1024);
            if (sizeink <= 999.99)
                return sizeink.ToString("#0.00") + " kb";

            float sizeinm = sizeink / 1024;
            if (sizeinm <= 999.99)
                return sizeinm.ToString("###,###,###,##0.#") + " mb";

            float sizeing = sizeinm / 1024;
            return sizeing.ToString("###,###,###,##0.#") + " GB";
        }

        public static string AppendSlash(string input)
        {
            if (input.EndsWith(@"\")) { return input; }
            else
            { return input + @"\"; }
        }

        /// <summary>
        /// Remove slash end of input.
        /// </summary>
        public static string RemoveSlash(string input)
        {
            if (!input.EndsWith(@":\") && input.EndsWith(@"\")) { return input.Substring(0, input.Length - 1); }
            else
            { return input; }
        }

        /// <summary>
        /// Return filename.
        /// </summary>
        public static string ExtractFileName(string input)
        {
            Int32 idx = input.LastIndexOfAny((",\\;").ToCharArray()) + 1;
            if (idx == 0)
                return input;
            return input.Substring(idx);
        }


        public static Bitmap GetFileIcon(string name)
        {
            ShellDll.ShellAPI.SHFILEINFO shinfo = new ShellAPI.SHFILEINFO();

            ShellAPI.SHGetFileInfo(name, ShellAPI.FILE_ATTRIBUTE.NORMAL,
                ref shinfo, (int)Marshal.SizeOf(shinfo), 
                ShellAPI.SHGFI.ICON | ShellAPI.SHGFI.USEFILEATTRIBUTES);

            return Icon.FromHandle(shinfo.hIcon).ToBitmap();
        }

        /// <summary>
        /// Return Current user path.
        /// </summary>		
        public static string GetCurrentUserPath()
        {
            try
            {
                RegistryKey rKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList", false);
                string dir = AppendSlash((string)rKey.GetValue("ProfilesDirectory"));
                string userdir = dir + System.Environment.UserName;
                if (Directory.Exists(userdir)) return userdir;
                if (Directory.Exists(dir)) return dir;
            }
            catch
            {

            }
            return (new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).Parent.FullName);
        }

        /// <summary>
        /// Return Shared user path.
        /// </summary>
        /// <returns></returns>
        public static string GetSharedPath()
        {
            try
            {
                RegistryKey rKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList", false);
                string dir = (string)rKey.GetValue("Public");
                if (Directory.Exists(dir)) return dir;

                rKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", false);
                dir = (string)rKey.GetValue("Common Documents");
                if (Directory.Exists(dir)) return Path.GetDirectoryName(dir.TrimEnd('\\'));
            }
            catch
            {

            }


            return Path.Combine(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).Parent.Parent.FullName, "Public");
        }

        public static bool IsDevMode
        { get { return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime; } }

        
    }
}
