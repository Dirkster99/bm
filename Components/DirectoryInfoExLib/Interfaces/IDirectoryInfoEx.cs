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

        /// <summary>
        /// Create the directory.
        /// </summary>
        void Create();

        /// <summary>
        /// Delete this folder. (not move it to recycle bin)
        /// </summary>
        void Delete();

        /// <summary>
        /// Move this folder to specified directory (fullpath)
        /// </summary>
        void MoveTo(string destDirName);

        /// <summary>
        /// Create a subdirectory
        /// </summary>
        /// <param name="path"> directory name.</param>
        DirectoryInfoEx CreateDirectory(string path);

        #region Methods - GetSubItems
        //0.17: Added DirectoryInfoEx.EnumerateFiles/EnumerateDirectories/EnumerateFileSystemInfos() methods which work similar as the one in .Net4
        IEnumerable<FileInfoEx> EnumerateFiles(String searchPattern, SearchOption searchOption, CancelDelegate cancel);

        IEnumerable<FileInfoEx> EnumerateFiles(String searchPattern, SearchOption searchOption);

        IEnumerable<FileInfoEx> EnumerateFiles(String searchPattern);

        IEnumerable<FileInfoEx> EnumerateFiles();

        IEnumerable<DirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption, CancelDelegate cancel);

        IEnumerable<DirectoryInfoEx> EnumerateDirectories(String searchPattern, SearchOption searchOption);
        IEnumerable<DirectoryInfoEx> EnumerateDirectories(String searchPattern);
        IEnumerable<DirectoryInfoEx> EnumerateDirectories();

        IEnumerable<FileSystemInfoEx> EnumerateFileSystemInfos(String searchPattern, SearchOption searchOption, CancelDelegate cancel);

        IEnumerable<FileSystemInfoEx> EnumerateFileSystemInfos(String searchPattern, SearchOption searchOption);
        IEnumerable<FileSystemInfoEx> EnumerateFileSystemInfos(String searchPattern);
        IEnumerable<FileSystemInfoEx> EnumerateFileSystemInfos();

        #region GetXXX
        /// <summary>
        /// Return a list of sub directories and files
        /// </summary>
        FileSystemInfoEx[] GetFileSystemInfos(String searchPattern, SearchOption searchOption);

        Task<FileSystemInfoEx[]> GetFileSystemInfosAsync(String searchPattern,
            SearchOption searchOption, CancellationToken ct);

        /// <summary>
        /// Return a list of sub directories and files
        /// </summary>
        FileSystemInfoEx[] GetFileSystemInfos(String searchPattern);

        /// <summary>
        /// Return a list of sub directories and files
        /// </summary>
        FileSystemInfoEx[] GetFileSystemInfos();

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

        /// <summary>
        /// Return a list of files in that directory
        /// </summary>
        FileInfoEx[] GetFiles(String searchPattern, SearchOption searchOption);

        /// <summary>
        /// Return a list of files in that directory
        /// </summary>
        FileInfoEx[] GetFiles(String searchPattern);

        /// <summary>
        /// Return a list of files in that directory
        /// </summary>
        FileInfoEx[] GetFiles();

        Task<FileInfoEx[]> GetFilesAsync(String searchPattern,
                                         SearchOption searchOption, CancellationToken ct);
        #endregion

        FileSystemInfoEx this[string name, bool isFile] { get; }

        FileSystemInfoEx this[string name] { get; }
        #endregion
        #endregion methods
    }
}
