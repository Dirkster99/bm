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
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Constructor
        /// <summary>
        /// Class constructor.
        /// </summary>
        public PathHierarchyHelper(string parentPath,
                                   string valuePath,
                                   string subEntriesPath)
        {
            ParentPath = parentPath;
            ValuePath = valuePath;
            SubentriesPath = subEntriesPath;
            Separator = '\\';
            StringComparisonOption = StringComparison.CurrentCultureIgnoreCase;
        }

        #endregion

        #region Methods
        #region Utils Func - extractPath/Name
        public virtual string ExtractPath(string pathName)
        {
            logger.InfoFormat("Path {0}", (pathName == null? "" : pathName));

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
            logger.InfoFormat("Path {0}", (pathName == null ? "" : pathName));

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
            logger.InfoFormat("Path {0}", (item == null ? "" : item.ToString()));

            return PropertyPathHelper.GetValueFromPropertyInfo(item, ParentPath);
        }

        protected virtual string getValuePath(object item)
        {
            logger.InfoFormat("Path {0}", (item == null ? "" : item.ToString()));

            return PropertyPathHelper.GetValueFromPropertyInfo(item, ValuePath) as string;
        }

        protected virtual IEnumerable getSubEntries(object item)
        {
            logger.InfoFormat("Path {0}", (item == null ? "" : item.ToString()));

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
            logger.InfoFormat("Path {0}", (item == null ? "" : item.ToString()));

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
            logger.InfoFormat("Path {0}", (item == null ? "" : item.ToString()));

            return item == null ? "" : getValuePath(item);
        }

        public IEnumerable List(object item)
        {
            logger.InfoFormat("Path {0}", (item == null ? "" : item.ToString()));

            if (item is IEnumerable)
                return item as IEnumerable;

            var retVal = getSubEntries(item);
            if (retVal == null)
                return new List<object>();

            return retVal;
        }

        /// <summary>
        /// Get Item from path.
        /// </summary>
        /// <param name="rootItem">RootItem or ItemSource which can be used to lookup from.</param>
        /// <param name="path"></param>
        /// <returns></returns>
        public object GetItem(object rootItem, string path)
        {
            logger.InfoFormat("Path {0}", (path == null ? "" : path));

            var queue = new Queue<string>(path.Split(new char[] { Separator }, StringSplitOptions.RemoveEmptyEntries));
            object current = rootItem;
            while (current != null && queue.Any())
            {
                var nextSegment = queue.Dequeue();
                object found = null;
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
    /// Generic version of AutoHierarchyHelper, which use Path to query for hierarchy of ViewModels
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PathHierarchyHelper<T> : PathHierarchyHelper
    {
        #region Constructor

        public PathHierarchyHelper(string parentPath, string valuePath, string subEntriesPath)
            : base(parentPath, valuePath, subEntriesPath)
        {
            propInfoSubEntries = typeof(T).GetProperty(subEntriesPath);
            propInfoValue = typeof(T).GetProperty(valuePath);
            propInfoParent = typeof(T).GetProperty(parentPath);
        }

        #endregion

        #region Methods

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
        #endregion

        #region Data
        PropertyInfo propInfoValue, propInfoSubEntries, propInfoParent;
        #endregion
        #region Public Properties
        #endregion
    }
}