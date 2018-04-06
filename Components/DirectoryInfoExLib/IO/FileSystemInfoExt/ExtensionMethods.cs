namespace DirectoryInfoExLib.IO.FileSystemInfoExt
{
    using DirectoryInfoExLib.IO.Header.ShellDll;
    using System;

    internal static class ExtensionMethods
    {
        public static void RequestPIDL(this FileSystemInfoEx fsi, Action<PIDL, PIDL> pidlAndRelPidlFunc)
        {
            PIDL pidl = fsi.getPIDL();
            PIDL relPidl = fsi.getRelPIDL();
            try
            {
                pidlAndRelPidlFunc(pidl, relPidl);
            }
            finally
            {
                pidl.Free();
                relPidl.Free();
            }
        }

        public static T RequestPIDL<T>(this FileSystemInfoEx fsi, Func<PIDL,T> pidlFuncOnly)
        {
            PIDL pidl = fsi.getPIDL();
            try
            {
                return pidlFuncOnly(pidl);
            }
            finally
            {
                pidl.Free();
            }
        }

        public static void RequestPIDL(this FileSystemInfoEx fsi, Action<PIDL> pidlFuncOnly)
        {
            PIDL pidl = fsi.getPIDL();
            try
            {
                pidlFuncOnly(pidl);
            }
            finally
            {
                pidl.Free();
            }
        }

        public static T RequestRelativePIDL<T>(this FileSystemInfoEx fsi, Func<PIDL, T> relPidlFuncOnly)
        {
            PIDL relPidl = fsi.getRelPIDL();
            try
            {
                return relPidlFuncOnly(relPidl);
            }
            finally
            {
                relPidl.Free();
            }
        }

        public static void RequestPIDL(this FileSystemInfoEx[] fsis, Action<PIDL[], IntPtr[]> pidlFunc)
        {
            PIDL[] pidls = new PIDL[fsis.Length];            
            IntPtr[] ptrs = new IntPtr[fsis.Length];

            for (int i = 0; i < fsis.Length; i++)
            {
                pidls[i] = fsis[i].getPIDL();
                ptrs[i] = pidls[i].Ptr;
            }

            try
            {
                pidlFunc(pidls, ptrs);
            }
            finally
            {
                foreach (PIDL pidl in pidls)
                    pidl.Free();
            }
        }
    }
}
