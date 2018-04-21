namespace DirectoryInfoExLib.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Threading;
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
    /// Defines a delegate that determines whether processing has been canceled or not.
    /// </summary>
    /// <returns></returns>
    public delegate bool CancelDelegate();

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
        /// <summary>
        /// Fetermines whether this instance equals the <paramref name="other"/>
        /// instance or not.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool Equals(IDirectoryInfoEx other);

        /// <summary>
        /// Enumerates all items in this directory with the given search parameters.
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption, CancelDelegate cancel);

        /// <summary>
        /// Enumerates all items in this directory with the given search parameters.
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption);

        /// <summary>
        /// Enumerates all items in this directory with the given search parameters.
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        IEnumerable<IDirectoryInfoEx> EnumerateDirectories(String searchPattern);

        /// <summary>
        /// Enumerates all items in this directory with the given search parameters.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDirectoryInfoEx> EnumerateDirectories();

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

        /// <summary>
        /// Return a task that returns a list of sub directories.
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IDirectoryInfoEx[]> GetDirectoriesAsync(String searchPattern,
                                                     SearchOption searchOption,
                                                     CancellationToken ct);

        /// <summary>
        /// Executes a delegate function that returns T and
        /// accepts 2 <see cref="PIDL"/> parameters.
        /// 
        /// Function takes care of freeing <see cref="PIDL"/> objects after execution.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pidlAndRelPidlFunc"></param>
        /// <returns></returns>
        T RequestPIDL<T>(Func<PIDL, PIDL, T> pidlAndRelPidlFunc);

        /// <summary>
        /// Executes a delegate function that returns T and
        /// accepts 1 <see cref="PIDL"/> parameter.
        /// 
        /// Function takes care of freeing <see cref="PIDL"/> object after execution.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pidlFuncOnly"></param>
        /// <returns></returns>
        T RequestPIDL<T>(Func<PIDL, T> pidlFuncOnly);

        /// <summary>
        /// Executes an Action that returns void and
        /// accepts 1 <see cref="PIDL"/> parameter.
        /// 
        /// Function takes care of freeing <see cref="PIDL"/> object after execution.
        /// </summary>
        /// <param name="pidlFuncOnly"></param>
        /// <returns></returns>
        void RequestPIDL(Action<PIDL> pidlFuncOnly);

        /// <summary>
        /// Executes an delegate function that returns T and
        /// accepts 1 relative <see cref="PIDL"/> parameter.
        /// 
        /// Function takes care of freeing <see cref="PIDL"/> object after execution.
        /// </summary>
        /// <param name="relPidlFuncOnly"></param>
        /// <returns></returns>
        T RequestRelativePIDL<T>(Func<PIDL, T> relPidlFuncOnly);
        #endregion methods
    }
}
