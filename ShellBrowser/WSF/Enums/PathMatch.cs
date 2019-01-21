namespace WSF.Enums
{
    /// <summary>
    /// Enumeration can be used to indicate the type of a match between
    /// a source and a target path. Both paths describe the same location
    /// <see cref="CompleteMatch"/> or be completely <see cref="Unrelated"/>.
    /// 
    /// The source path is the parent path of target: <see cref="PartialSource"/>.
    /// The target path is the parent path of source: <see cref="PartialTarget"/>.
    /// </summary>
    public enum PathMatch
    {
        /// <summary>
        /// Both paths are completely unrelated.
        /// </summary>
        Unrelated,

        /// <summary>
        /// Both paths describe the same location.
        /// </summary>
        CompleteMatch,

        /// <summary>
        /// The source path is the parent path of target.
        /// </summary>
        PartialSource,

        /// <summary>
        /// The target path is the parent path of source.
        /// </summary>
        PartialTarget
    }
}
