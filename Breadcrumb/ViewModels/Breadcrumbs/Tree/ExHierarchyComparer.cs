namespace Breadcrumb.ViewModels.Breadcrumbs
{
    using Breadcrumb.SystemIO;
    using Breadcrumb.ViewModels.Interfaces;
    using BreadcrumbLib.Defines;
    using DirectoryInfoExLib.Interfaces;
    using DirectoryInfoExLib.Tools;

    public class ExHierarchyComparer : ICompareHierarchy<IDirectoryInfoEx>
	{
		#region fields
		private PathComparer _pathComparer = new PathComparer();
		#endregion fields

		#region constructors
		/// <summary>
		/// Class cosntructor
		/// </summary>
		public ExHierarchyComparer()
		{
		}
		#endregion constructors

		#region mothods
		public HierarchicalResult CompareHierarchyInner(IDirectoryInfoEx a, IDirectoryInfoEx b)
		{
			if (a == null || b == null)
				return HierarchicalResult.Unrelated;

			if (!a.FullName.Contains("::") && !b.FullName.Contains("::"))
				return this._pathComparer.CompareHierarchy(a.FullName, b.FullName);

			if (a.FullName.Equals(b.FullName))
				return HierarchicalResult.Current;

			string key = string.Format("{0}-compare-{1}", a.FullName, b.FullName);

			if (a.FullName == b.FullName)
				return HierarchicalResult.Current;
			else if (IOTools.HasParent(b, a.FullName))
				return HierarchicalResult.Child;
			else if (IOTools.HasParent(a, b.FullName))
				return HierarchicalResult.Parent;
			else return HierarchicalResult.Unrelated;
		}

		public HierarchicalResult CompareHierarchy(IDirectoryInfoEx a, IDirectoryInfoEx b)
		{
			HierarchicalResult retVal = this.CompareHierarchyInner(a, b);
			////Debug.WriteLine(String.Format("{2} {0},{1}", a.FullPath, b.FullPath, retVal));

			return retVal;
		}

////		private bool HasParent(FileSystemInfoEx child, DirectoryInfoEx parent)
////		{
////			DirectoryInfoEx current = child.Parent;
////			while (current != null)
////			{
////				if (current.Equals(parent))
////					return true;
////				current = current.Parent;
////			}
////			return false;
////		}
		#endregion mothods
	}
}
