namespace SuggestLib.Interfaces
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IHierarchyHelper
    {
        /// <summary>
        /// Gets a seperator character that is usually used to seperate one
        /// entry of one level from its sub-level entry (eg.: '/' or '\')
        /// </summary>
        char Separator { get; }

        StringComparison StringComparisonOption { get; }

        string ParentPath { get; }

        string ValuePath { get; }

        string SubentriesPath { get; }

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

        string ExtractPath(string pathName);

        string ExtractName(string pathName);

        IEnumerable List(object item);
    }
}