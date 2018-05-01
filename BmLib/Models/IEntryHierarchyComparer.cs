namespace BmLib.Models
{
    using BmLib.Enums;

    public interface ICompareHierarchy<T>
    {
        HierarchicalResult CompareHierarchy(T value1, T value2);
    }

    /// <summary>
    /// Used by DirectoryTree and Breadcrumb to identify the location of appropriate model.
    /// </summary>
    public interface IEntryHierarchyComparer : ICompareHierarchy<IEntryModel>
    {
    }
}
