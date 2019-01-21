namespace WSF.Shell.Interop.ShellFolders
{
    using WSF.IDs;
    using WSF.Shell.Enums;
    using WSF.Shell.Interop.Interfaces.KnownFolders;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using WSF.Shell.Interop.Interfaces.ShellItems;
    using WSF.Shell.Pidl;

    /// <summary>
    /// Class wraps <see cref="IKnownFolderNative"/> COM interface to ensure
    /// correct memory management via <see cref="IDisposable"/> pattern.
    /// </summary>
    public class KnownFolderNative : IDisposable
    {
        #region fields
        private bool _disposed = false;
        private IntPtr _intPtrKnownFolder = IntPtr.Zero;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor from <see cref="IKnownFolderNative"/> interface.
        /// </summary>
        /// <param name="knownFolderNative"></param>
        public KnownFolderNative(IKnownFolderNative knownFolderNative)
            : this()
        {
            InitObject(IntPtr.Zero, knownFolderNative);
        }

        /// <summary>
        /// Class constructor from <see cref="IntPtr"/>.
        /// </summary>
        /// <param name="intPtrKnownFolderNative"></param>
        public KnownFolderNative(IntPtr intPtrKnownFolderNative)
            : this()
        {
            InitObject(intPtrKnownFolderNative);
        }

        /// <summary>
        /// Initializr objrct from <paramref name="intPtrKnownFolderNative"/>
        /// or <paramref name="iknownfolder"/>. You should use only one
        /// parameter here and set the other to default if necessary.
        /// </summary>
        /// <param name="intPtrKnownFolderNative"></param>
        /// <param name="iknownfolder"></param>
        protected void InitObject(IntPtr intPtrKnownFolderNative,
                                  IKnownFolderNative iknownfolder = null)
        {
            _intPtrKnownFolder = intPtrKnownFolderNative;

            if (_intPtrKnownFolder != IntPtr.Zero)
                Obj = (IKnownFolderNative)Marshal.GetTypedObjectForIUnknown(intPtrKnownFolderNative, typeof(IKnownFolderNative));
            else
                Obj = iknownfolder;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected KnownFolderNative()
        {
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets the encapsulated <see cref="IKnownFolderNative"/> interface
        /// to let clients invoke a method on this interface.
        /// </summary>
        public IKnownFolderNative Obj { get; protected set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Gets whether the known folder object managed in this object is a folder
        /// or file objects that is part of the file system,
        /// that is, its a file, directory, or root directory.
        /// </summary>
        /// <returns></returns>
        public bool? IsFileSystem()
        {
            bool isFileSystem = false;

            Debug.Assert(Obj != null, "Native IKnownFolder cannot be null.");
            
            // Get the native IShellItem2 from the native IKnownFolder
            IShellItem2 ishellItem;
            Guid guid = new Guid(ShellIIDGuids.IShellItem2);
            HRESULT hr = Obj.GetShellItem(0, ref guid, out ishellItem);
            
            if (hr == HRESULT.S_OK)
            {
                // If we have a valid IShellItem, try to get the FileSystem attribute.
                using (var shellItem = new ShellItem2(ishellItem))  // Will dispose the COM interface
                {
                    SFGAOF sfgao;
                    shellItem.Obj.GetAttributes(SFGAOF.FileSystem, out sfgao);
                
                    // Is this item a FileSystem item?
                    isFileSystem = (sfgao & SFGAOF.FileSystem) != 0;

                    return isFileSystem;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Calls the GetIDList method in the wrapped IKnownFolder interface to retrieve the ItemIdList
        /// for this KnownFolderItem.
        /// </summary>
        /// <returns>Type: PIDLIST_ABSOLUTE*
        /// When this method returns, contains the address of an absolute PIDL.
        /// The caller is responsible for freeing the returned PIDL when it is no longer needed by calling ILFree.</returns>
        public IntPtr KnownFolderToPIDL()
        {
            if (Obj == null)
              throw new System.ArgumentException("Native IKnownFolder cannot be null.");

            IntPtr ptrAddr = default(IntPtr);
            try
            {
                HRESULT RetVal = Obj.GetIDList((uint)KNOWN_FOLDER_FLAG.KF_NO_FLAGS, out ptrAddr);

                if (ptrAddr != default(IntPtr) && RetVal == HRESULT.S_OK)
                {
                    return ptrAddr;
                }
                else
                    throw new ArgumentException("Invalid knownFolder " + RetVal);
            }
            catch (System.ArgumentException)
            {
                // Is thrown for some folders on Win10 (not sure why?)
                // By CanonicalName:
                // SyncSetupFolder, SyncResultsFolder, ConflictFolder
                // AppUpdatesFolder, ChangeRemoveProgramsFolder, SyncCenterFolder
            }
            catch (System.UnauthorizedAccessException)
            {
                // Is thrown on Win10 (not sure why?)
                // By CanonicalName: LocalizedResourcesDir
            }

            return default(IntPtr);
        }

        /// <summary>
        /// Calls the GetIDList method in the wrapped IKnownFolder interface to retrieve the ItemIdList
        /// for this KnownFolderItem.
        /// </summary>
        /// <returns>Type: PIDLIST_ABSOLUTE*
        /// When this method returns, contains the address of an absolute PIDL
        /// in its equivalent ItemIdList form.</returns>
        public IdList KnownFolderToIdList()
        {
            if (Obj == null)
                throw new System.ArgumentException("Native IKnownFolder cannot be null.");

            IntPtr ptrFullPidl = default(IntPtr);
            try
            {
                ptrFullPidl = KnownFolderToPIDL();

                // Convert PIDL into list of shellids and remove last id
                var shellListIds = PidlManager.Decode(ptrFullPidl);

                return IdList.Create(shellListIds);
            }
            finally
            {
                ptrFullPidl = PidlManager.ILFree(ptrFullPidl);
            }
        }

        /// <summary>
        /// Returns the unique KnownFolder GUID Id of this special folder.
        /// </summary>
        /// <returns></returns>
        public string GetId()
        {
            if (Obj == null)
                throw new System.ArgumentException("Native IKnownFolder cannot be null.");

            IntPtr ptrPath = default(IntPtr);
            try
            {
                return Obj.GetId().ToString();
            }
            catch
            {
                // Some special folders may not have a file system path
                return null;
            }
            finally
            {
                ptrPath = PidlManager.FreeCoTaskMem(ptrPath);
            }
        }

        /// <summary>
        /// Returns the unique GUID Id (logical path) of this special folder.
        /// </summary>
        /// <returns></returns>
        public string GetPathShell()
        {
            if (Obj == null)
                throw new System.ArgumentException("Native IKnownFolder cannot be null.");

            IntPtr ptrPath = default(IntPtr);
            try
            {
                string s = string.Format("{0}{1}{2}", "::{", Obj.GetId(), "}");
                return s;
            }
            catch
            {
                // Some special folders may not have this path
                return null;
            }
            finally
            {
                ptrPath = PidlManager.FreeCoTaskMem(ptrPath);
            }
        }

        /// <summary>
        /// Retrieves the physical path of a known folder as a string or null
        /// if the special folder has no physical representation in the file system.
        /// 
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/bb761762(v=vs.85).aspx
        /// </summary>
        public string GetPath()
        {
            if (Obj == null)
                throw new System.ArgumentException("Native IKnownFolder cannot be null.");

            IntPtr ptrPath = default(IntPtr);
            try
            {
                Obj.GetPath(0, out ptrPath);

                return Marshal.PtrToStringUni(ptrPath);
            }
            catch
            {
                // Some special folders may not have a file system path
                return null;
            }
            finally
            {
                ptrPath = PidlManager.FreeCoTaskMem(ptrPath);
            }
        }

        #region Disposable Interfaces
        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    if (Obj != null)
                    {
                        try
                        {
                            Marshal.ReleaseComObject(Obj);
                        }
                        catch{}
                        finally
                        {
                            Obj = null;
                        }
                    }

                    if (_intPtrKnownFolder != IntPtr.Zero)
                    {
                        try
                        {
                            Marshal.Release(_intPtrKnownFolder);
                        }
                        catch { }
                        finally
                        {
                            _intPtrKnownFolder = IntPtr.Zero;
                        }
                    }

                    GC.SuppressFinalize(this);
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            _disposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }
        #endregion Disposable Interfaces
        #endregion methods
    }
}
