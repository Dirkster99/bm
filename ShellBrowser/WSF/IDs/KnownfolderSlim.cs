namespace WSF.IDs
{
    /// <summary>
    /// Implements a simple base class to keep track of known folders base properties
    /// such as their special id (parseName), name, and file system path (if any).
    /// </summary>
    public class KnownfolderSlim
    {
        #region ctor
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="_parseName"></param>
        /// <param name="_name"></param>
        /// <param name="_fileSystemPath"></param>
        public KnownfolderSlim(string _parseName, string _name, string _fileSystemPath)
            : this()
        {
            SpecialId = _parseName;
            Name = _name;
            FileSystemPath = _fileSystemPath;
        }

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="_parseName"></param>
        /// <param name="_name"></param>
        public KnownfolderSlim(string _parseName, string _name)
            : this()
        {
            SpecialId = _parseName;
            Name = _name;
        }

        /// <summary>
        /// Class constructor.
        /// </summary>
        protected KnownfolderSlim()
        {
        }
        #endregion ctor

        /// <summary>
        /// Gets the Special ID of this known folder (if any).
        /// </summary>
        public string SpecialId { get; }

        /// <summary>
        /// Gets the Name this known folder.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the file system path of this known folder (if any).
        /// </summary>
        public string FileSystemPath { get; }
    }
}
