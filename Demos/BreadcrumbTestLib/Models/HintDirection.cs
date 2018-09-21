namespace BreadcrumbTestLib.Models
{
    /// <summary>
    /// Models the directions in the tree in which the control is
    /// requested to browse in relation to the currently selected
    /// item (current path).
    /// </summary>
    public enum HintDirection
    {
        /// <summary>
        /// This default should always be applicable when the direction of the
        /// new location cannot be hinted by any of the directions below.
        /// 
        /// Example:
        /// Current Path: 'C:\Windows'
        /// New Location: 'F:\MyDocuments\pictures\nice'
        ///
        /// -> New path is rather unrelated to current path
        /// </summary>
        Unrelated = 0,

        /// <summary>
        /// The target of the navigational request is a child
        /// below the currently selected item.
        /// 
        /// Example:
        /// Current Path: 'C:\Windows'
        /// New Location: 'C:\Windows\System32' -> New path is below current path
        /// </summary>
        Down = 1,

        /// <summary>
        /// The target of the navigational request is a part
        /// of the currently selected path. That is, assuming
        /// the current path is complete towards the root, this means,
        /// the new location is part of the current path.
        /// 
        /// Example:
        /// Current Path: 'C:\Windows\System32'
        /// New Location: 'C:\Windows' -> New path is part of current
        /// </summary>
        Up = 2
    }
}