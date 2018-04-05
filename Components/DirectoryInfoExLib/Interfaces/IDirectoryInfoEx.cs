namespace DirectoryInfoExLib.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Threading;
    using DirectoryInfoExLib.IO.FileSystemInfoExt;
    using DirectoryInfoExLib.Tools;
    using DirectoryInfoExLib.IO.Tools.Interface;
    using DirectoryInfoExLib.IO.Header.KnownFolder;
    using DirectoryInfoExLib.IO.Header.KnownFolder.Enums;

    public enum DirectoryTypeEnum { dtDesktop, dtSpecial, dtDrive, dtFolder, dtRoot }

    /// <summary>
    /// Represents a directory in PIDL system.
    /// </summary>
    public interface IDirectoryInfoEx : IDisposable, ISerializable, ICloneable
    {
        #region properties
        string Label { get; }

        ShellFolder2 ShellFolder { get; }

        Storage Storage { get; }

        DirectoryInfoEx Root { get; }

        bool IsBrowsable { get; set; }

        bool IsFileSystem { get; set; }

        bool HasSubFolder { get; set; }

        DirectoryTypeEnum DirectoryType { get; }

        Environment.SpecialFolder? ShellFolderType { get; }

        KnownFolder KnownFolderType { get; }

        KnownFolderIds? KnownFolderId { get; }
        #endregion properties

        #region methods
        bool Equals(FileSystemInfoEx other);

        #region Methods - GetSubItems
        IEnumerable<DirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption, CancelDelegate cancel);

        IEnumerable<DirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption);
        IEnumerable<DirectoryInfoEx> EnumerateDirectories(String searchPattern);
        IEnumerable<DirectoryInfoEx> EnumerateDirectories();

        #region GetXXX
        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        DirectoryInfoEx[] GetDirectories(String searchPattern, SearchOption searchOption);

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        DirectoryInfoEx[] GetDirectories(String searchPattern);

        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        DirectoryInfoEx[] GetDirectories();

        Task<DirectoryInfoEx[]> GetDirectoriesAsync(String searchPattern,
                                                    SearchOption searchOption,
                                                    CancellationToken ct);
        #endregion
        #endregion
        #endregion methods
    }
}
