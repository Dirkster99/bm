namespace BreadcrumbTestLib.ViewModels.Interfaces
{
    using BreadcrumbLib.Enums;

    public interface ICompareHierarchy<T>
    {
        HierarchicalResult CompareHierarchy(T value1, T value2);
    }
}
