namespace SuggestBoxTestLib.DataSources.Auto
{
    using SuggestBoxTestLib.DataSources.Auto.Interfaces;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Use Path to query for hierarchy of ViewModels.
    /// </summary>
    public class PathHierarchyHelper : IHierarchyHelper
    {
        #region Constructors
        /// <summary>
        /// Class constructor.
        /// </summary>
        public PathHierarchyHelper(string parentPath,
                                   string valuePath,
                                   string subEntriesPath)
            : this()
        {
            ParentPath = parentPath;
            ValuePath = valuePath;
            SubentriesPath = subEntriesPath;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected PathHierarchyHelper()
        {
            Separator = '\\';
            StringComparisonOption = StringComparison.CurrentCultureIgnoreCase;
        }
        #endregion Constructors

        #region properties
        /// <summary>
        /// Gets a seperator character that is usually used to seperate one
        /// entry of one level from its sub-level entry (eg.: '/' or '\')
        /// </summary>
        public char Separator { get; set; }

        /// <summary>
        /// Gets the string comparing option for comparing paths
        /// (on Windows this usually case insensitive and ignores the current culture)
        /// </summary>
        public StringComparison StringComparisonOption { get; set; }

        /// <summary>
        /// Path to the dependency property of the parent item.
        /// </summary>
        public string ParentPath { get; set; }

        /// <summary>
        /// Path to the value dependency property of the item.
        /// </summary>
        public string ValuePath { get; set; }

        /// <summary>
        /// Path to the child dependency property of the item.
        /// </summary>
        public string SubentriesPath { get; set; }
        #endregion properties

        #region Methods
        /// <summary>
        /// Gets the path from a string that holds at least
        /// one <see cref="Separator"/> character or an empty string
        /// if no seperator was present.
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public virtual string ExtractPath(string pathName)
        {
            if (String.IsNullOrEmpty(pathName))
                return string.Empty;

            int idx = pathName.LastIndexOf(Separator);
            if (idx == -1)
                return string.Empty;
            else
                return pathName.Substring(0, idx);
        }

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
        public virtual string ExtractName(string pathName)
        {
            if (String.IsNullOrEmpty(pathName))
                return string.Empty;

            int idx = pathName.LastIndexOf(Separator);
            if (idx == -1)
                return pathName;
            else
                return pathName.Substring(idx + 1);
        }

        /// <summary>
        /// Used to generate ItemsSource for BreadcrumbCore.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEnumerable<object> GetHierarchy(object item, bool includeCurrent)
        {
            if (includeCurrent)
                yield return item;

            var current = getParent(item);
            while (current != null)
            {
                yield return current;
                current = getParent(current);
            }
        }

        /// <summary>
        /// Gets all sub entries below <paramref name="item"/> or
        /// an empty list if item is not <see cref="IEnumerable"/>
        /// or list of sub entries is non-existing.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEnumerable List(object item)
        {
            if (item is IEnumerable)
                return item as IEnumerable;

            var retVal = getSubEntries(item);

            if (retVal == null)
                return new List<object>();

            return retVal;
        }

        /// <summary>
        /// Generate Path from an item;
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetPath(object item)
        {
            return item == null ? "" : getValuePath(item);
        }

        /// <summary>
        /// Attempts to find an hierarchy item from a given
        /// <paramref name="path"/> and <paramref name="rootItem"/>
        /// using a Breadth First Level Order lookup algorithm.
        /// 
        /// Assuming we give it a path like 'sub2/sub3' and the hierarchy contains
        /// this path then the method should return an object that represents sub3.
        /// </summary>
        /// <param name="rootItem">RootItem or ItemSource which can be used to lookup from.</param>
        /// <param name="path"></param>
        /// <returns>The item found or null</returns>
        public object GetItem(object rootItem, string path)
        {
            var queue = new Queue<string>(path.Split(new char[] { this.Separator }, StringSplitOptions.RemoveEmptyEntries));

            object current = rootItem;              // Return root item if queue is empty
            while (current != null && queue.Any())
            {
                var nextSegment = queue.Dequeue();  // Get each part of the path from root to sub-item and
                object found = null;               // and attempt to locate the object associated with the path

                foreach (var item in List(current))
                {
                    string valuePathName = getValuePath(item);
                    string value = ExtractName(valuePathName); // Value may be full path, or just current value.

                    if (value.Equals(nextSegment, StringComparisonOption))
                    {
                        found = item;
                        break;
                    }
                }

                current = found;
            }

            return current;
        }

        /// <summary>
        /// Gets a parent object given an <paramref name="item"/> and
        /// a <see cref="ParentPath"/> to the dependency property that
        /// should be used for lookup.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual object getParent(object item)
        {
            return PropertyPathHelper.GetValueFromPropertyInfo(item, ParentPath);
        }

        /// <summary>
        /// Gets an object given an <paramref name="item"/> and
        /// a <see cref="ValuePath"/> to the dependency property that
        /// should be used for lookup.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual string getValuePath(object item)
        {
            return PropertyPathHelper.GetValueFromPropertyInfo(item, ValuePath) as string;
        }

        /// <summary>
        /// Gets an object given an <paramref name="item"/> and
        /// a <see cref="SubentriesPath"/> to the dependency property that
        /// should be used for lookup.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual IEnumerable getSubEntries(object item)
        {
            return PropertyPathHelper.GetValueFromPropertyInfo(item, SubentriesPath) as IEnumerable;
        }
        #endregion methods
    }

    /// <summary>
    /// Generic version of AutoHierarchyHelper,
    /// which uses Path to query for hierarchy of ViewModels.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class PathHierarchyHelper<T> : PathHierarchyHelper
    {
        #region Constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="parentPath"></param>
        /// <param name="valuePath"></param>
        /// <param name="subEntriesPath"></param>
        public PathHierarchyHelper(string parentPath,
                                   string valuePath,
                                   string subEntriesPath)

            : base(parentPath, valuePath, subEntriesPath)
        {
            propInfoSubEntries = typeof(T).GetProperty(subEntriesPath);
            propInfoValue = typeof(T).GetProperty(valuePath);
            propInfoParent = typeof(T).GetProperty(parentPath);
        }
        #endregion

        PropertyInfo propInfoValue;
        PropertyInfo propInfoSubEntries;
        PropertyInfo propInfoParent;

        protected override object getParent(object item)
        {
            return propInfoParent.GetValue(item);
        }

        protected override IEnumerable getSubEntries(object item)
        {
            return propInfoSubEntries.GetValue(item) as IEnumerable;
        }

        protected override string getValuePath(object item)
        {
            return propInfoValue.GetValue(item) as string;
        }
    }
}