namespace BreadcrumbLib.Enums
{
    /// <summary>
    /// Enumerates values that are returned by a class that implements ICompareHierarchy<T>.
    /// Such a class is typically used to compare 2 items
    /// (Item1 and Item2, e.g.: Folder: 'C:\' with Folder: 'C:\Windows')
    /// and determine their relationship.
    /// </summary>
    public enum HierarchicalResult : int
    {
        /// <summary>
        /// Item1 is a (direct or further above) parent of Item2
        /// </summary>
        Parent = 1 << 1,

        /// <summary>
        /// Item1 and Item2 describe the same location in the Hierarchy.
        /// </summary>
        Current = 1 << 2,

        /// <summary>
        /// Item1 is a child of Item2 (inverse relationship of Parent).
        /// </summary>
        Child = 1 << 3,

        /// <summary>
        /// Can be used as Default relationship.
        ///
        /// Item1 and Item2 are unrelated. Both, or either item is null
        /// or the items describe 2 completely different and rather
        /// unrelated things.
        /// </summary>
        Unrelated = 1 << 4,

        /// <summary>
        /// Item1 and Item2 have multiple relationships (Parent, Current, Child).
        ///
        /// This relationship is optional and hardly useful
        /// but its there in case its needed.
        /// </summary>
        Related = Parent | Current | Child,

        /// <summary>
        /// Item1 and Item2 are related and unrelated at the same time.
        ///
        /// This relationship is optional and hardly useful
        /// but its there in case its needed.
        /// </summary>
        All = Related | Unrelated
    }
}
