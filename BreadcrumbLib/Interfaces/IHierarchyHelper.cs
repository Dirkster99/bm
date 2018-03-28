namespace BreadcrumbLib.Interfaces
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IHierarchyHelper
    {
        #region properties
        char Separator { get; }

        StringComparison StringComparisonOption { get; }

        string ParentPath { get; }

        string ValuePath { get; }

        string SubentriesPath { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Used to generate ItemsSource for BreadcrumbCore.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        IEnumerable<object> GetHierarchy(object item, bool includeCurrent);

        /// <summary>
        /// Generate Path from an item;
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        string GetPath(object item);

        /// <summary>
        /// Get Item from path.
        /// </summary>
        /// <param name="rootItem">RootItem or ItemSource which can be used to lookup from.</param>
        /// <param name="path"></param>
        /// <returns></returns>
        object GetItem(object rootItem, string path);

        IEnumerable List(object item);

        string ExtractPath(string pathName);

        string ExtractName(string pathName);
        #endregion methods
    }
}
