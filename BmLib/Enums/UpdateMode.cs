namespace BmLib.Enums
{
    /// <summary>
    /// Specifies whether list entries are to be replaced or updated on (re)load.
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// Existing list entries are replaced on (re)load.
        /// </summary>
        Replace,

        /// <summary>
        /// Existing list entries are updated on (re)load.
        /// </summary>
        Update
    }
}