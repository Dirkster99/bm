namespace SuggestBoxTestLib.DataSources.Auto.Interfaces
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IHierarchyHelper
    {
        #region properties
        /// <summary>
        /// Gets a seperator character that is usually used to seperate one
        /// entry of one level from its sub-level entry (eg.: '/' or '\')
        /// </summary>
        char Separator { get; }

        /// <summary>
        /// Gets the string comparing option for comparing paths
        /// (on Windows this usually case insensitive and ignores the current culture)
        /// </summary>
        StringComparison StringComparisonOption { get; }


//        string ParentPath { get; }
//
//        string ValuePath { get; }
//
//        string SubentriesPath { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Used to generate ItemsSource for BreadcrumbCore.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        IEnumerable<object> GetHierarchy(object item, bool includeCurrent);

        /// <summary>
        /// Gets all sub entries below <paramref name="item"/> or
        /// an empty list if item is not <see cref="IEnumerable"/>
        /// or list of sub entries is non-existing.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        IEnumerable List(object item);

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

        /// <summary>
        /// Gets the path from a string that holds at least
        /// one <see cref="Separator"/> character or an empty string
        /// if no seperator was present.
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        string ExtractPath(string pathName);

        /// <summary>
        /// Gets the name a string that can have  one or more
        /// <see cref="Separator"/> characters or an empty string
        /// if no input string was present.
        /// 
        /// The name portion of the string is either the string itself
        /// or the remaining string after the last seperator.
        /// 
        /// input                  Name
        /// 'Libraries'         -> 'Libraries'
        /// 'Libraries\Music'   -> 'Music'
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        string ExtractName(string pathName);
        #endregion methods
    }
}