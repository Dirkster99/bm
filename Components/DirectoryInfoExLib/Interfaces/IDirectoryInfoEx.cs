namespace DirectoryInfoExLib.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Threading;
    using DirectoryInfoExLib.Tools;
    using DirectoryInfoExLib.IO.Header.KnownFolder;
    using DirectoryInfoExLib.IO.Header.ShellDll;

    /// <summary>
    /// Implements a classification for folders.
    /// </summary>
    public enum DirectoryTypeEnum
    {
        /// <summary>
        /// The folder is the users <see cref="Environment.SpecialFolder.DesktopDirectory"/>.
        /// </summary>
        dtDesktop,

        /// <summary>
        /// This folder is a known folder in Windows.
        /// https://msdn.microsoft.com/en-us/library/bb776911
        /// </summary>
        dtSpecial,

        /// <summary>
        /// This folder is a drive.
        /// </summary>
        dtDrive,

        /// <summary>
        /// This folder is a normal (non-special or in Windows known) directory.
        /// </summary>
        dtFolder

////        ,dtRoot
    }

    /// <summary>
    /// Represents a directory in PIDL system.
    /// </summary>
    public interface IDirectoryInfoEx : IDisposable, ICloneable
    {
        #region properties
        /// <summary>
        /// Gets the name of a directory.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the name including path and extensions.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the label of a directory, ehich can be different to a name in case of a drive.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the parent directory of a directory (if any) or null (if none).
        /// </summary>
        IDirectoryInfoEx Parent { get; }

        /// <summary>
        /// Gets the root directory of a directory with
        /// which is either desktop or drive.
        /// </summary>
        IDirectoryInfoEx Root { get; }

        /// <summary>
        /// Gets the folders type classification.
        /// </summary>
        DirectoryTypeEnum DirectoryType { get; }

        /// <summary>
        /// Gets the Windows known folder (similar to <see cref="Environment.SpecialFolder"/>
        /// but extensible and customizable at run-time) or null if this folder
        /// is not a special folder in Windows.
        /// </summary>
        /// <returns></returns>
        KnownFolder KnownFolderType { get; }
        #endregion properties

        #region methods
        bool Equals(IDirectoryInfoEx other);

        IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption, CancelDelegate cancel);

        IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption);
        IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern);
        IEnumerable<IDirectoryInfoEx> EnumerateDirectories();

        #region GetXXX
        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        IDirectoryInfoEx[] GetDirectories(String searchPattern, SearchOption searchOption);

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        IDirectoryInfoEx[] GetDirectories(String searchPattern);

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        IDirectoryInfoEx[] GetDirectories();

        Task<IDirectoryInfoEx[]> GetDirectoriesAsync(String searchPattern,
                                                     SearchOption searchOption,
                                                     CancellationToken ct);
        #endregion

        T RequestPIDL<T>(Func<PIDL, PIDL, T> pidlAndRelPidlFunc);
        T RequestPIDL<T>(Func<PIDL, T> pidlFuncOnly);
        void RequestPIDL(Action<PIDL> pidlFuncOnly);
        T RequestRelativePIDL<T>(Func<PIDL, T> relPidlFuncOnly);
        #endregion methods
    }
}
