namespace WSF.Shell.Interop.Knownfolders
{
    using WSF.Shell.Interop.KnownFolders;
    using WSF.IDs;
    using WSF.Shell.Enums;
    using WSF.Shell.Interop.Interfaces.Knownfolders;
    using WSF.Shell.Interop.Interfaces.KnownFolders;
    using WSF.Shell.Interop.ShellFolders;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using WSF.Shell.Pidl;
    using WSF.Shell.Interop.Dlls;

    /// <summary>
    /// Creates the helper class for known folders.
    /// </summary>
    public static class KnownFolderHelper
    {
        /// <summary>
        /// Returns the native known folder object given a PID list or null
        /// if known folder is not available on the local system.
        /// </summary>
        /// <param name="pidl"></param>
        /// <returns></returns>
        internal static KnownFolderNative FromPIDL(IntPtr pidl)
        {
            KnownFolderManagerClass knownFolderManager = new KnownFolderManagerClass();

            IKnownFolderNative iknownFolder;
            HRESULT hr = knownFolderManager.FindFolderFromIDList(pidl, out iknownFolder);

            return (hr == HRESULT.S_OK) ? new KnownFolderNative(iknownFolder) : null;
        }

        /// <summary>
        /// Gets a <see cref="KnownFolderNative"/> object from an <see cref="IdList"/>
        /// based PIDL if this represents a knownfolder, or otherwise, null.
        /// </summary>
        /// <param name="ashellListId"></param>
        /// <returns></returns>
        internal static KnownFolderNative FromPIDL(IdList ashellListId)
        {
            bool isDesktop = true;

            if (ashellListId != null)
            {
                if (ashellListId.Size > 0)
                    isDesktop = false;
            }

            if (isDesktop == true)
                return KnownFolderHelper.FromPath(KF_IID.ID_FOLDERID_Desktop, true);

            IntPtr pidl = default(IntPtr);
            try
            {
                pidl = PidlManager.IdListToPidl(ashellListId);

                KnownFolderManagerClass knownFolderManager = new KnownFolderManagerClass();

                IKnownFolderNative iknownFolder;
                HRESULT hr = knownFolderManager.FindFolderFromIDList(pidl, out iknownFolder);

                return (hr == HRESULT.S_OK) ? new KnownFolderNative(iknownFolder) : null;
            }
            finally
            {
                pidl = PidlManager.ILFree(pidl);
            }
        }

        /// <summary>
        /// Returns the <see cref="KnownFolderNative"/> object that represents the desktop object.
        /// </summary>
        /// <returns></returns>
        public static KnownFolderNative GetDesktop()
        {
            IntPtr pidl = default(IntPtr);
            try
            {
                NativeMethods.SHGetKnownFolderIDList(KnownFolderGuids.FOLDERID_Desktop,
                                                     KNOWN_FOLDER_FLAG.KF_NO_FLAGS, IntPtr.Zero, out pidl);

                if (pidl != default(IntPtr))
                {
                    return KnownFolderHelper.FromPIDL(pidl);
                }
            }
            finally
            {
                if (pidl != default(IntPtr))
                    NativeMethods.ILFree(pidl);
            }

            return null;
        }

        /// <summary>
        /// Returns a known folder given a globally unique identifier.
        /// </summary>
        /// <param name="knownFolderId">A GUID for the requested known folder.</param>
        /// <returns>A known folder representing the specified name.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the given Known Folder ID is invalid.</exception>
        public static KnownFolderNative FromKnownFolderGuid(Guid knownFolderId)
        {
            IKnownFolderNative knownFolderNative;
            KnownFolderManagerClass knownFolderManager = new KnownFolderManagerClass();

            HRESULT hr = knownFolderManager.GetFolder(knownFolderId, out knownFolderNative);

            if (hr != HRESULT.S_OK)
                return null;

            return new KnownFolderNative(knownFolderNative);
        }

        /// <summary>
        /// Returns a known folder given its shell path, such as <c>C:\users\public\documents</c> or 
        /// <c>::{645FF040-5081-101B-9F08-00AA002F954E}</c> for the Recycle Bin.
        /// </summary>
        /// <param name="path">The path for the requested known folder; either a physical path or a virtual path.</param>
        /// <param name="IsSpecialPath"></param>
        /// <returns>A known folder representing the specified name.</returns>
        public static KnownFolderNative FromPath(string path,
                                                 bool? IsSpecialPath = null)
        {
            if (string.IsNullOrEmpty(path) == true)
                throw new ArgumentNullException("'path' parameter cannot be Empty or Null.");

            if (IsSpecialPath == null)
                IsSpecialPath = (ShellHelpers.IsSpecialPath(path) == ShellHelpers.SpecialPath.IsSpecialPath);

            if (IsSpecialPath == true)
            {
                // Get the KnownFolderId Guid for this special folder
                var kf_guid = new Guid(path.Substring(KF_IID.IID_Prefix.Length));

                try
                {
                    var ret = FromKnownFolderGuid(kf_guid);

                    if (ret != null)
                        return ret;
                }
                catch{ }
            }

            IntPtr pidl = default(IntPtr);
            try
            {
                pidl = ShellHelpers.PIDLFromPath(path);

                if (pidl == default(IntPtr))
                {
                    // try one more time with a trailing \0
                    pidl = ShellHelpers.PidlFromParsingName(path);
                }

                if (pidl != default(IntPtr))
                {
                    // It's probably a special folder, try to get it                
                    var kf = KnownFolderHelper.FromPIDL(pidl);

                    if (kf != null)
                    {
                        return kf;
                    }
                }
            }
            finally
            {
                pidl = PidlManager.ILFree(pidl);
            }

            return null;
        }

        /// <summary>
        /// Returns the known folder given its canonical name.
        /// See <see cref="IKnownFolderProperties.CanonicalName"/>
        /// </summary>
        /// <param name="canonicalName">A non-localized canonical name for the known folder, such as 'MyComputerFolder'.</param>
        /// <returns>A known folder representing the specified name.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the given canonical name is invalid or if the KnownFolder could not be created.</exception>
        public static KnownFolderNative FromCanonicalName(string canonicalName)
        {
            IKnownFolderNative knownFolderNative;
            IKnownFolderManager knownFolderManager = (IKnownFolderManager)new KnownFolderManagerClass();

            try
            {
                knownFolderManager.GetFolderByName(canonicalName, out knownFolderNative);

                if (knownFolderNative != null)
                    return new KnownFolderNative(knownFolderNative);
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Gets the path of this this known folder or null if known folder is not
        /// available or does not have a representation in the file system.
        /// </summary>
        /// <param name="fileExists">
        /// Returns false if the folder is virtual, or a boolean
        /// value that indicates whether this known folder exists.
        /// </param>
        /// <param name="knownFolderNative">Native IKnownFolder reference</param>
        /// <returns>
        /// A <see cref="System.String"/> containing the path, or <see cref="System.String.Empty"/>
        /// if this known folder does not exist.
        /// </returns>
        public static string GetPath(out bool fileExists, IKnownFolderNative knownFolderNative)
        {
            string kfPath = null;
            fileExists = true;

            if (knownFolderNative == null)
            {
                fileExists = false;
                return null;
            }

            // Virtual folders do not have path.
            if (knownFolderNative.GetCategory() == FolderCategory.Virtual)
            {
                fileExists = false;
                return kfPath;
            }

            IntPtr ptrPath = default(IntPtr);
            try
            {
                knownFolderNative.GetPath(0, out ptrPath);

                if (ptrPath != default(IntPtr))
                    kfPath = Marshal.PtrToStringUni(ptrPath);
            }
            catch (System.IO.FileNotFoundException)
            {
                fileExists = false;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                fileExists = false;
            }
            finally
            {
                if (ptrPath != default(IntPtr))
                    Marshal.FreeCoTaskMem(ptrPath);
            }

            return kfPath;
        }

        /// <summary>
        /// Populates an object that contains a known folder's properties.
        /// </summary>
        public static IKnownFolderProperties GetFolderProperties(IKnownFolderNative knownFolderNative)
        {
            Debug.Assert(knownFolderNative != null);

            NativeFolderDefinition nativeFolderDefinition = default(NativeFolderDefinition);
            try
            {
                knownFolderNative.GetFolderDefinition(out nativeFolderDefinition);

                KnownFolderProperties knownFolderProperties =
                                    new KnownFolderProperties(knownFolderNative,
                                                              nativeFolderDefinition);

                return knownFolderProperties;
            }
            catch
            {
                return default(KnownFolderProperties);
            }
            finally
            {
                // Clean up memory. 
                ////FolderCategory category
                Marshal.FreeCoTaskMem(nativeFolderDefinition.name);
                Marshal.FreeCoTaskMem(nativeFolderDefinition.description);

                ////Guid parentId;

                Marshal.FreeCoTaskMem(nativeFolderDefinition.relativePath);
                Marshal.FreeCoTaskMem(nativeFolderDefinition.parsingName);
                Marshal.FreeCoTaskMem(nativeFolderDefinition.tooltip);
                Marshal.FreeCoTaskMem(nativeFolderDefinition.localizedName);
                Marshal.FreeCoTaskMem(nativeFolderDefinition.icon);
                Marshal.FreeCoTaskMem(nativeFolderDefinition.security);

                ////UInt32 attributes;
                ////DefinitionOptions definitionOptions;
                ////Guid folderTypeId;
            }
        }

        /// <summary>
        /// Populates an object that contains a known folder's properties
        /// as specified by a special path stated in <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path for the requested known folder; (a virtual path).</param>
        /// <returns>A known folder representing the specified name.</returns>
        public static IKnownFolderProperties GetFolderPropertiesFromPath(string path)
        {
            using (var kf = KnownFolderHelper.FromPath(path))
            {
                if (kf != null)
                {
                    return KnownFolderHelper.GetFolderProperties(kf.Obj);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a strongly-typed read-only collection of all the registered known folders.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Guid, IKnownFolderProperties> GetAllFolders()
        {
            // Should this method be thread-safe?? (It'll take a while
            // to get a list of all the known folders, create the managed wrapper
            // and return the read-only collection.
            var foldersList = new Dictionary<Guid, IKnownFolderProperties>();
            var pathList = new Dictionary<string, IKnownFolderProperties>();
            uint count;
            IntPtr folders = IntPtr.Zero;

            try
            {
                KnownFolderManagerClass knownFolderManager = new KnownFolderManagerClass();
                var result = knownFolderManager.GetFolderIds(out folders, out count);

                if (count > 0 && folders != IntPtr.Zero)
                {
                    // Loop through all the KnownFolderID elements
                    for (int i = 0; i < count; i++)
                    {
                        // Read the current pointer
                        IntPtr current = new IntPtr(folders.ToInt64() + (Marshal.SizeOf(typeof(Guid)) * i));

                        // Convert to Guid
                        Guid knownFolderID = (Guid)Marshal.PtrToStructure(current, typeof(Guid));

                        try
                        {
                            using (var nativeKF = FromKnownFolderGuid(knownFolderID))
                            {
                                var kf = KnownFolderHelper.GetFolderProperties(nativeKF.Obj);

                                // Add to our collection if it's not null (some folders might not exist on the system
                                // or we could have an exception that resulted in the null return from above method call
                                if (kf != null)
                                {
                                    foldersList.Add(kf.FolderId, kf);

                                    if (kf.IsExistsInFileSystem == true)
                                        pathList.Add(kf.Path, kf);
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            finally
            {
                if (folders != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(folders);
            }

            return foldersList;
        }

        /// <summary>
        /// Gets the actual file system path (e.g.: 'C:\Windows') for a KnownfolderID.
        /// 
        /// The <paramref name="knownfolderIdPath"/> is expected to follow the format
        /// in <seealso cref="KF_IID"/> definitions.
        /// </summary>
        /// <param name="knownfolderIdPath">Identifies the known folder in question</param>
        /// <returns>null if path does not exist or the actual path.</returns>
        public static string GetKnownFolderPath(string knownfolderIdPath)
        {
            // Get the KnownFolderId Guid for this special folder
            var kf_guid = new Guid(knownfolderIdPath.Substring(KF_IID.IID_Prefix.Length));

            try
            {
                using (var kf = KnownFolderHelper.FromKnownFolderGuid(kf_guid))
                {
                    if (kf != null)
                        return kf.GetPath();
                }

            }
            catch
            {
                // Guard: in case the caller has passed an invalid id we return null.
            }

            return null;
        }
    }
}
