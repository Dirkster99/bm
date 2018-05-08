namespace DirectoryInfoExLib.Interfaces
{
    using DirectoryInfoExLib.IO.Header.ShellDll;
    using System;

    /// <summary>
    /// Represents a directory in PIDL system.
    /// </summary>
    public interface IDirectoryBrowser : IDisposable, ICloneable
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
        IDirectoryBrowser Parent { get; }

        /// <summary>
        /// Gets the folders type classification.
        /// </summary>
        DirectoryTypeEnum DirectoryType { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Return a list of sub directories
        /// </summary>
        IDirectoryBrowser[] GetDirectories();

        /// <summary>
        /// Gets an <see cref="IntPtr"/> representing a PIDL item list
        /// that represents this item.
        /// </summary>
        /// <returns></returns>
        IntPtr GetPIDLIntPtr();
        #endregion methods
    }
}
