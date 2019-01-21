namespace WSF.Shell.Interop.ShellFolders
{
    using WSF.IDs;
    using WSF.Shell.Enums;
    using System;
    using WSF.Shell.Interop.Dlls;

    internal static class ShellHelpers
    {
        internal enum SpecialPath
        {
            None = 0,
            IsSpecialPath = 1,
            ContainsSpecialPath = 2
        }

        /// <summary>
        /// Determines if the given path follows the special folder reference
        /// format used in IIDs in <see cref="KF_IID"/>
        /// (refers to a special folder) or not.
        /// 
        /// The Special case <see cref="SpecialPath.ContainsSpecialPath"/> indicates
        /// a path that contains a special folderid together with something else and
        /// is, therefore, likely to require more processing than just retireving a folder.
        /// 
        /// &lt;Drive>:\Users\&lt;User>\Desktop" -> ::{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}"
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SpecialPath IsSpecialPath(string path)
        {
            if (string.IsNullOrEmpty(path) == true)
                return SpecialPath.None;

            if (path.Length == (KF_IID.IID_Prefix.Length + 38))
            {
                if (path.Substring(0, KF_IID.IID_Prefix.Length) == KF_IID.IID_Prefix)
                    return SpecialPath.IsSpecialPath;
            }
            else
            {
                // There are some minor exotic cases where the special folder id is longer than
                // one guid (or KnownFolderId).
                if (path.Length > (KF_IID.IID_Prefix.Length + KF_ID.ID_Length))
                {
                    if (path.Substring(0, KF_IID.IID_Prefix.Length) == KF_IID.IID_Prefix)
                        return SpecialPath.ContainsSpecialPath;
                }
            }

            return SpecialPath.None;
        }

        /// <summary>
        /// Converts a path representation into a PIDL
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IntPtr PIDLFromPath(string path)
        {
            if (string.IsNullOrEmpty(path) == true)
                return IntPtr.Zero;

            IntPtr pidlPtr = default(IntPtr);

            if (IsSpecialPath(path) == SpecialPath.IsSpecialPath) // Handle Special Folder path notation
            {
                // Get the KnownFolderId Guid for this special folder
                var kf_guid = new Guid(path.Substring(KF_IID.IID_Prefix.Length));

                HRESULT result = NativeMethods.SHGetKnownFolderPath(kf_guid, 0, IntPtr.Zero, out pidlPtr);

                if (result == HRESULT.S_OK)
                {
                    return pidlPtr;
                }

                return IntPtr.Zero;
            }

            using (var desktopFolder = new ShellFolderDesktop())
            {
                SFGAO pdwAttributes = 0;
                uint pchEaten = 0;

                if (desktopFolder.Obj.ParseDisplayName(IntPtr.Zero, IntPtr.Zero,
                                                          path, ref pchEaten,
                                                          out pidlPtr, ref pdwAttributes) == (uint)HRESULT.S_OK)
                    return pidlPtr;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets the PIDL from a given parsing name as <see cref="IntPtr"/> reference
        /// or IntPtr.Zero if knownfolder could not be resolved.
        ///
        /// https://github.com/aybe/Windows-API-Code-Pack-1.1/blob/master/source/WindowsAPICodePack/Shell/Common/ShellHelper.cs
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Type: PIDLIST_ABSOLUTE*
        /// The address of a pointer to a variable of type ITEMIDLIST that receives the item identifier list for the object.
        /// If an error occurs, then this parameter is set to NULL.
        /// The caller is responsible for freeing the returned PIDL when it is no longer needed by calling ILFree.</returns>
        internal static IntPtr PidlFromParsingName(string name)
        {
            IntPtr pidlFull = default(IntPtr);

            SFGAOF sfgao;

            var retCode = NativeMethods.SHParseDisplayName(name, IntPtr.Zero,
                                                           out pidlFull, (SFGAOF)0, out sfgao);

            return (retCode == HRESULT.S_OK ? pidlFull : IntPtr.Zero);
        }
    }
}
