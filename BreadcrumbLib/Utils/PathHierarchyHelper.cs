namespace BreadcrumbLib.Utils
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using BreadcrumbLib.Interfaces;

	/// <summary>
	/// Use Path to query for hierarchy of ViewModels.
	/// </summary>
	public class PathHierarchyHelper : IHierarchyHelper
	{
		#region Constructor

		public PathHierarchyHelper(string parentPath, string valuePath, string subEntriesPath)
		{
			this.ParentPath = parentPath;
			this.ValuePath = valuePath;
			this.SubentriesPath = subEntriesPath;
			this.Separator = '\\';
			this.StringComparisonOption = StringComparison.CurrentCultureIgnoreCase;
		}

		#endregion

		#region properties
		public char Separator { get; set; }

		public StringComparison StringComparisonOption { get; set; }

		public string ParentPath { get; set; }

		public string ValuePath { get; set; }

		public string SubentriesPath { get; set; }
		#endregion properties

		#region Methods
		#region Utils Func - extractPath/Name
		public virtual string ExtractPath(string pathName)
		{
			if (string.IsNullOrEmpty(pathName))
				return string.Empty;

			if (pathName.IndexOf(this.Separator) == -1)
				return string.Empty;
			else
				return pathName.Substring(0, pathName.LastIndexOf(this.Separator));
		}

		public virtual string ExtractName(string pathName)
		{
			if (string.IsNullOrEmpty(pathName))
				return string.Empty;

			if (pathName.IndexOf(this.Separator) == -1)
				return pathName;
			else
				return pathName.Substring(pathName.LastIndexOf(this.Separator) + 1);
		}
		#endregion

		#region Implements
		public IEnumerable<object> GetHierarchy(object item, bool includeCurrent)
		{
			if (includeCurrent)
				yield return item;

			var current = this.getParent(item);
			while (current != null)
			{
				yield return current;
				current = this.getParent(current);
			}
		}

		public string GetPath(object item)
		{
			return item == null ? string.Empty : this.getValuePath(item);
		}

		public IEnumerable List(object item)
		{
			return item is IEnumerable ? item as IEnumerable : this.getSubEntries(item);
		}

		public object GetItem(object rootItem, string path)
		{
			var queue = new Queue<string>(path.Split(new char[] { this.Separator },
			                                                      StringSplitOptions.RemoveEmptyEntries));
			object current = rootItem;
			while (current != null && queue.Any())
			{
				var nextSegment = queue.Dequeue();
				object found = null;

				foreach (var item in this.List(current))
				{
					string valuePathName = this.getValuePath(item);
					string value = this.ExtractName(valuePathName); // Value may be full path, or just current value.

					if (value.Equals(nextSegment, this.StringComparisonOption))
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

		#region Overridable to improve speed.
		protected virtual object getParent(object item)
		{
			return PropertyPathHelper.GetValueFromPropertyInfo(item, this.ParentPath);
		}

		protected virtual string getValuePath(object item)
		{
			return PropertyPathHelper.GetValueFromPropertyInfo(item, this.ValuePath) as string;
		}

		protected virtual IEnumerable getSubEntries(object item)
		{
			return PropertyPathHelper.GetValueFromPropertyInfo(item, this.SubentriesPath) as IEnumerable;
		}
		#endregion Overridable to improve speed.
		#endregion
	}

	/// <summary>
	/// Generic version of AutoHierarchyHelper, which use Path to query for hierarchy of ViewModels
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PathHierarchyHelper<T> : PathHierarchyHelper
	{
		#region fields
		private PropertyInfo propInfoValue, propInfoSubEntries, propInfoParent;
		#endregion fields

		#region Constructor

		public PathHierarchyHelper(string parentPath, string valuePath, string subEntriesPath)
			: base(parentPath, valuePath, subEntriesPath)
		{
			this.propInfoSubEntries = typeof(T).GetProperty(subEntriesPath);
			this.propInfoValue = typeof(T).GetProperty(valuePath);
			this.propInfoParent = typeof(T).GetProperty(parentPath);
		}

		#endregion

		#region Methods
		protected override object getParent(object item)
		{
			return this.propInfoParent.GetValue(item);
		}

		protected override IEnumerable getSubEntries(object item)
		{
			return this.propInfoSubEntries.GetValue(item) as IEnumerable;
		}

		protected override string getValuePath(object item)
		{
			return this.propInfoValue.GetValue(item) as string;
		}
		#endregion
	}
}
