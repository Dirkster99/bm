using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellDll;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace System.IO
{
    #region Events
    internal class ShellChangeEventArgs : EventArgs
    {
        public ShellAPI.SHCNE Changes { get; private set; }
        public PIDL PIDL1 { get; private set; }
        public PIDL PIDL2 { get; private set; }
        internal ShellChangeEventArgs(ShellAPI.SHCNE changes, PIDL pidl1, PIDL pidl2)
        { Changes = changes; PIDL1 = pidl1; PIDL2 = pidl2; }
    }
    internal delegate void ShellChangeEventHandler(object sender, ShellChangeEventArgs e);

    public enum WatcherChangeTypesEx { All, Changed, Created, Deleted, Renamed, Ready, NotReady }
    public class FileSystemEventArgsEx : EventArgs
    {
        public WatcherChangeTypesEx ChangeType { get; protected set; }
        public virtual string FullPath { get; protected set; }
        public virtual string Name { get { return Path.GetFileName(FullPath); } }
        public virtual PIDL PIDL { get; protected set; }
        public virtual bool IsFolder { get; protected set; }

        public FileSystemEventArgsEx(WatcherChangeTypesEx changeType, bool isFolder, PIDL pidl, string fullPath)
        { ChangeType = changeType; IsFolder = isFolder; PIDL = pidl; FullPath = fullPath; }
        protected FileSystemEventArgsEx() { }
    }
    public delegate void FileSystemEventHandlerEx(object sender, FileSystemEventArgsEx e);

    public class RenameEventArgsEx : FileSystemEventArgsEx
    {
        public virtual string OldFullPath { get; protected set; }
        public virtual string OldName { get { return Path.GetFileName(OldFullPath); } }
        public RenameEventArgsEx(WatcherChangeTypesEx changeType, bool isFolder, PIDL pidl, string fullPath, string oldFullPath)
            : base(changeType, isFolder, pidl, fullPath)
        { OldFullPath = oldFullPath; }
    }
    public delegate void RenameEventHandlerEx(object sender, RenameEventArgsEx e);

    #endregion

    #region SystemWatcherWrapper
    internal class SystemWatcherWrapper : NativeWindow
    {
        uint _notifyID;
        PIDL _dirPIDL = null;
        public DirectoryInfoEx MonitorDir { get; private set; }
        public bool IncludeSubDirectories { get; private set; }


        public ShellChangeEventHandler OnEvent;

        public SystemWatcherWrapper(DirectoryInfoEx dir, bool includeSubDir)
        {            
            MonitorDir = dir;
            _dirPIDL = dir.getPIDL();
            IncludeSubDirectories = includeSubDir;

            this.CreateHandle(new CreateParams());

            ShellAPI.SHChangeNotifyEntry entry = new ShellAPI.SHChangeNotifyEntry();
            
            entry.pIdl = _dirPIDL.Ptr;
            entry.Recursively = includeSubDir;
            _notifyID = ShellAPI.SHChangeNotifyRegister(this.Handle,
                 ShellAPI.SHCNRF.InterruptLevel | ShellAPI.SHCNRF.ShellLevel,
                    ShellAPI.SHCNE.ALLEVENTS | ShellAPI.SHCNE.INTERRUPT,
                    ShellAPI.WM.SH_NOTIFY,
                    1,
                    new ShellAPI.SHChangeNotifyEntry[] { entry });
        }

        ~SystemWatcherWrapper()
        {
            this.ReleaseHandle();
            if (_notifyID > 0)
            {
                ShellAPI.SHChangeNotifyDeregister(_notifyID);
            }

            if (_dirPIDL != null)
                _dirPIDL.Free();
            _dirPIDL = null;
            GC.SuppressFinalize(this);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == (int)ShellAPI.WM.SH_NOTIFY)
            {                
                ShellAPI.SHNOTIFYSTRUCT shNotify =
                    (ShellAPI.SHNOTIFYSTRUCT)Marshal.PtrToStructure(m.WParam, typeof(ShellAPI.SHNOTIFYSTRUCT));
                if (this.OnEvent != null)
                {
                    PIDL pidl1 = (!PIDL.IsEmpty(shNotify.dwItem1)) ? new PIDL(shNotify.dwItem1, true) : null;
                    PIDL pidl2 = (!PIDL.IsEmpty(shNotify.dwItem2)) ? new PIDL(shNotify.dwItem2, true) : null;
                    OnEvent(this, new ShellChangeEventArgs(((ShellAPI.SHCNE)m.LParam), pidl1, pidl2));

                    if (pidl1 != null) pidl1.Free();
                    if (pidl2 != null) pidl2.Free();
                }
            }

        }
    }
    #endregion

    public class FileSystemWatcherEx : Component
    {
        SystemWatcherWrapper _sww;


        public DirectoryInfoEx Directory { get { return _sww.MonitorDir; } }
        public string Filter = "*.*";
        public string Path { get { return Directory.FullName; } }
        public bool IncludeSubdirectories
        {
            get { return _sww.IncludeSubDirectories; }
            set { init(Directory, value); }
        }

        public FileSystemEventHandlerEx OnChanged;
        public FileSystemEventHandlerEx OnCreated;
        public FileSystemEventHandlerEx OnDeleted;
        public RenameEventHandlerEx OnRenamed;

        //private void initNotifyFilters()
        //{
        //    NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName
        //        | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security
        //        | NotifyFilters.Size;
        //}

        protected void uninit()
        {
            if (_sww != null)
                _sww.OnEvent -= new ShellChangeEventHandler(HandleEvent);
            _sww = null;
        }

        protected void init(DirectoryInfoEx dir, bool includeSubdir)
        {
            if (_sww == null || !_sww.MonitorDir.Equals(dir) || includeSubdir != _sww.IncludeSubDirectories)
            {
                if (_sww != null)
                    _sww.OnEvent -= new ShellChangeEventHandler(HandleEvent);

                _sww = new SystemWatcherWrapper(dir, includeSubdir);
                _sww.OnEvent += new ShellChangeEventHandler(HandleEvent);
            }
        }

        public FileSystemWatcherEx(string path)
        {
            init(new DirectoryInfoEx(path), true);
        }

        public FileSystemWatcherEx(DirectoryInfoEx dir, bool includeSubdirectories)
        {
            init(dir, includeSubdirectories);
        }

        public FileSystemWatcherEx(DirectoryInfoEx dir)
        {
            init(dir, true);
        }

        protected FileSystemWatcherEx()
        {

        }

        void handleEvent(WatcherChangeTypesEx changeType, bool isFolder, PIDL pidl)
        {
            if (IOTools.MatchFileMask(pidl, Filter))
            {
                FileSystemEventArgsEx args = new FileSystemEventArgsEx(changeType, isFolder,
                    pidl, FileSystemInfoEx.PIDLToPath(pidl));
                switch (changeType)
                {
                    case WatcherChangeTypesEx.Created: if (OnCreated != null) OnCreated(this, args); break;
                    case WatcherChangeTypesEx.Changed: if (OnChanged != null) OnChanged(this, args); break;
                    case WatcherChangeTypesEx.Deleted: if (OnDeleted != null) OnDeleted(this, args); break;
                }
            }
        }

        void handleRenameEvent(bool isFolder, PIDL pidl1, PIDL pidl2)
        {
            if (IOTools.MatchFileMask(pidl1, Filter))
                if (OnRenamed != null)
                    OnRenamed(this, new RenameEventArgsEx(WatcherChangeTypesEx.Renamed, isFolder, pidl2,
                        FileSystemInfoEx.PIDLToPath(pidl2), FileSystemInfoEx.PIDLToPath(pidl1)));
        }

        internal void HandleEvent(object sender, ShellChangeEventArgs args)
        {
            //Thread thisThread = System.Threading.Thread.CurrentThread;
            //Threading.ThreadPool.QueueUserWorkItem(
            //    (WaitCallback)delegate(object state)
            //{
            //    ShellChangeEventArgs e = (ShellChangeEventArgs)state;

            //    handleEvent(this, e);
            //}, args);

            handleEvent(sender, args);
        }

        internal void handleEvent(object sender, ShellChangeEventArgs args)
        {
            if (args.PIDL1 != null)
                switch (args.Changes)
                {
                    case ShellAPI.SHCNE.CREATE: //File Created
                        handleEvent(WatcherChangeTypesEx.Created, false, args.PIDL1);
                        break;
                    case ShellAPI.SHCNE.DELETE: //File Deleted
                        handleEvent(WatcherChangeTypesEx.Deleted, false, args.PIDL1);
                        break;
                    case ShellAPI.SHCNE.UPDATEITEM:  //File Changed
                        handleEvent(WatcherChangeTypesEx.Changed, false, args.PIDL1);
                        break;
                    case ShellAPI.SHCNE.RENAMEITEM:  //File Renamed
                        handleRenameEvent(false, args.PIDL1, args.PIDL2);
                        break;
                    case ShellAPI.SHCNE.MKDIR: //Make Directory
                        handleEvent(WatcherChangeTypesEx.Created, true, args.PIDL1);
                        break;
                    //0.13: Fixed FileSystemWaterEx ignore remove directory event.
                    case ShellAPI.SHCNE.RMDIR: //Remove Directory
                        handleEvent(WatcherChangeTypesEx.Deleted, true, args.PIDL1);
                        break;
                    case ShellAPI.SHCNE.UPDATEDIR: //Directory updated
                        handleEvent(WatcherChangeTypesEx.Changed, true, args.PIDL1);
                        break;
                    case ShellAPI.SHCNE.RENAMEFOLDER:  //Folder Renamed
                        handleRenameEvent(true, args.PIDL1, args.PIDL2);
                        break;
                    case ShellAPI.SHCNE.DRIVEADD:
                    case ShellAPI.SHCNE.DRIVEADDGUI:
                        handleEvent(WatcherChangeTypesEx.Created, true, args.PIDL1);
                        break;
                    case ShellAPI.SHCNE.MEDIAINSERTED:
                        handleEvent(WatcherChangeTypesEx.Ready, true, args.PIDL1);
                        break;
                    case ShellAPI.SHCNE.MEDIAREMOVED:
                        handleEvent(WatcherChangeTypesEx.NotReady, true, args.PIDL1);
                        break;
                    case ShellAPI.SHCNE.ATTRIBUTES:
                        handleEvent(WatcherChangeTypesEx.Changed, true, args.PIDL1);
                        break;
                }
        }
    }
}
