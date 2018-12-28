namespace SuggestLib
{
    using Interfaces;
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
        #region Constructor
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

        protected PathHierarchyHelper()
        {
            Separator = '\\';
            StringComparisonOption = StringComparison.CurrentCultureIgnoreCase;
        }
        #endregion

        #region Methods
        #region Utils Func - extractPath/Name
        public virtual string ExtractPath(string pathName)
        {
            if (String.IsNullOrEmpty(pathName))
                return "";

            if (pathName.IndexOf(Separator) == -1)
            {
                return "";
            }
            else
                return pathName.Substring(0, pathName.LastIndexOf(Separator));
        }

        public virtual string ExtractName(string pathName)
        {
            if (String.IsNullOrEmpty(pathName))
                return "";

            if (pathName.IndexOf(Separator) == -1)
                return pathName;
            else
                return pathName.Substring(pathName.LastIndexOf(Separator) + 1);
        }
        #endregion

        #region Overridable to improve speed.

        protected virtual object getParent(object item)
        {
            return PropertyPathHelper.GetValueFromPropertyInfo(item, ParentPath);
        }

        protected virtual string getValuePath(object item)
        {
            return PropertyPathHelper.GetValueFromPropertyInfo(item, ValuePath) as string;
        }

        protected virtual IEnumerable getSubEntries(object item)
        {
            return PropertyPathHelper.GetValueFromPropertyInfo(item, SubentriesPath) as IEnumerable;
        }
        #endregion

        #region Implements
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
        /// Generate Path from an item;
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetPath(object item)
        {
            return item == null ? "" : getValuePath(item);
        }

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
        /// Attempts to find an hierarchy item from a given
        /// <paramref name="path"/> and <paramref name="rootItem"/>.
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
                    string value = ExtractName(valuePathName); //Value may be full path, or just current value.

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

        #endregion Implements
        #endregion methods

        #region Public Properties

        /// <summary>
        /// Gets a seperator character that is usually used to seperate one
        /// entry of one level from its sub-level entry (eg.: '/' or '\')
        /// </summary>
        public char Separator { get; set; }

        public StringComparison StringComparisonOption { get; set; }

        public string ParentPath { get; set; }
        public string ValuePath { get; set; }
        public string SubentriesPath { get; set; }

        #endregion
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