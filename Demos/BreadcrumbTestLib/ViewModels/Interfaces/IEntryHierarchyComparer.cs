namespace BreadcrumbTestLib.ViewModels.Interfaces
{
    using BmLib.Enums;

    /// <summary>
    /// Defines an interface that all tree objects implement to determine
    /// the relationships of their items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICompareHierarchy<T>
    {
        /// <summary>
        /// Method is invoked to compute the relationship of 2 items in a tree.
        /// (Parent, Current, Child <see cref="HierarchicalResult"/> enumeration)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        HierarchicalResult CompareHierarchy(T value1, T value2);
    }
}
