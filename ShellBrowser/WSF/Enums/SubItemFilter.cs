namespace WSF.Enums
{
    /// <summary>
    /// Specifies a filter for WinShell items which can be filtered by their
    /// 1: Name only or by their
    /// 2: Name or Parse Name
    /// 
    /// Example:
    /// -> Filter is 'c*'
    /// -> item.Name = "Operating System (C:)" and item.ParseName="C:\"
    /// 
    /// Would be filtered by ParseName since name does not start with 'c'.
    /// </summary>
    public enum SubItemFilter
    {
        /// <summary>
        /// Apply filter on item Names only.
        /// 
        /// Items whos Name does not satisfy the specified filter expression are not shown.
        /// </summary>
        NameOnly,

        /// <summary>
        /// Apply filter on item Name or ParseNames.
        /// 
        /// Items whos Name or ParseName do not satisfy the specified filter expression are not shown.
        /// </summary>
        NameOrParsName
    }
}
