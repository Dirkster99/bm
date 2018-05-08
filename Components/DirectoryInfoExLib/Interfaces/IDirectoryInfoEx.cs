namespace DirectoryInfoExLib.Interfaces
{
    using System;
    using DirectoryInfoExLib.IO.Header.ShellDll;
    using DirectoryInfoExLib.IO.Header.KnownFolder;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

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
    /// Extends IDirectoryInfoEx by those API items that are currently
    /// not needed in an external browser application.
    /// </summary>
    internal interface IDirectoryInfoEx : IDirectoryBrowser
    {
        #region properties
        /// <summary>
        /// Gets the root directory of a directory
        /// which is either desktop or drive.
        /// </summary>
        IDirectoryInfoEx Root { get; }

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
        IEnumerable<IDirectoryBrowser> EnumerateDirectories();

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        IDirectoryBrowser[] GetDirectories(String searchPattern, SearchOption searchOption);

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        IDirectoryInfoEx[] GetDirectories(String searchPattern);

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
        #endregion methods
    }
}
