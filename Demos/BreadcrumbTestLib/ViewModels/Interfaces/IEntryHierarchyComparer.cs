namespace BreadcrumbTestLib.ViewModels.Interfaces
{
    using BmLib.Enums;

    public interface ICompareHierarchy<T>
    {
        HierarchicalResult CompareHierarchy(T value1, T value2);
    }
}
