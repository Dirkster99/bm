namespace Breadcrumb.SystemIO
{
	using System;
	using Breadcrumb.Defines;
	using Breadcrumb.ViewModels.Interfaces;

	public class PathComparer : ICompareHierarchy<string>
	{
		#region fields
		private StringComparison _stringComparsion = StringComparison.InvariantCultureIgnoreCase;
		#endregion fields

		#region constructors
		public PathComparer(StringComparison comparsion = StringComparison.CurrentCultureIgnoreCase)
		{
			this._stringComparsion = comparsion;
		}
		#endregion constructors

		#region Methods
		public HierarchicalResult CompareHierarchy(string path1, string path2)
		{
			if (path1 == null || path2 == null)
				return HierarchicalResult.Unrelated;

			if (path1.Equals(path2, this._stringComparsion))
				return HierarchicalResult.Current;

			if (!path1.EndsWith("\\"))
				path1 += "\\";

			if (!path2.EndsWith("\\"))
				path2 += "\\";

			if (path1.StartsWith(path2, this._stringComparsion))
				return HierarchicalResult.Parent;

			if (path2.StartsWith(path1, this._stringComparsion))
				return HierarchicalResult.Child;

			return HierarchicalResult.Unrelated;
		}
		#endregion

		#region Public Properties

		#endregion
	}
}
