///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2009 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of Steven Roebert's work (http://www.codeproject.com/KB/miscctrl/FileBrowser.aspx)    //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using ShellDll;

namespace System.IO
{
    public class DriveInfoEx
    {
        private DirectoryInfoEx _rootDirectory;
        private DriveType _driveType;
        private Int64 _totalSize, _freeSpace;
        private bool _isReady;
        private string _name, _label, _driveFormat;

        public string Name { get { return _name; } }
        public string VolumeLabel { get { return _label; } }
        public DirectoryInfoEx RootDirectory { get { return _rootDirectory; } }
        public Int64 TotalSize { get { return _totalSize; } }
        public Int64 FreeSpace { get { return _freeSpace; } }
        public DriveType DriveType { get { return _driveType; } }
        public bool IsReady { get { return _isReady; } }
        public string DriveFormat { get { return _driveFormat; } }
        

        void init(DirectoryInfoEx rootDir)
        {
            _rootDirectory = rootDir;
            _name = rootDir.FullName;
            _label = rootDir.Label;
            

            if (rootDir.FullName.Length == 3 & rootDir.FullName.Substring(1).Equals(":\\"))
            {
                DriveInfo innerDriveInfo = new DriveInfo(this.Name);
                this._isReady = innerDriveInfo.IsReady;
                this._driveType = innerDriveInfo.DriveType;
                if (this._isReady)
                {
                    this._freeSpace = innerDriveInfo.AvailableFreeSpace;
                    this._totalSize = innerDriveInfo.TotalSize;
                    this._driveFormat = innerDriveInfo.DriveFormat;
                }

                //string objAlias = "Win32_LogicalDisk.DeviceID=\"" + rootDir.FullName.Substring(0, 1) +  ":\"";
                //Management.ManagementObject disk =
                //    new Management.ManagementObject(objAlias);
                //try
                //{
                //    _isReady = true;
                //    _totalSize = Convert.ToInt64(disk["Size"]);
                //    _freeSpace = Convert.ToInt64(disk["FreeSpace"]);
                    
                //    uint driveType = Convert.ToUInt32(disk["DriveType"]);
                //    if (driveType <= 6) _driveType = (DriveType)driveType; else _driveType = DriveType.Unknown;

                    
                //}
                //catch //Disconnected Network Drives etc. will generate an error here, just assume that it is a network drive.
                //{                    
                //    _driveType = DriveType.Network;
                //    _isReady = false;
                //}

            }
            else _driveType = DriveType.Network;
        }

        public static DriveInfoEx[] GetDrives()
        {
            List<DriveInfoEx> retVal = new List<DriveInfoEx>();
            foreach (DirectoryInfoEx dir in DirectoryInfoEx.MyComputerDirectory.GetDirectories())
                retVal.Add(new DriveInfoEx(dir));

            return retVal.ToArray();
        }

        internal DriveInfoEx(PIDL pidl)
        {
            init(new DirectoryInfoEx(pidl));
        }

        public DriveInfoEx(DirectoryInfoEx dir)
        {
            init(dir);
        }

        public DriveInfoEx(string drive)
        {
            bool isDir;
            //0.18: DriveInfoEx constructor now accept full drive name ("C" and "C:\" both accepted now)
            if (!drive.EndsWith(":\\")) drive += ":\\";
            if (DirectoryInfoEx.MyComputerDirectory.Contains(drive, out isDir))
                init(DirectoryInfoEx.MyComputerDirectory.GetSubDirectory(drive));
            else throw new ArgumentException("Drive not found.");            
        }
    }
}
