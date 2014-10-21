namespace BreadcrumbLib.Utils
{
	using System.Globalization;
	using System.Windows.Controls;
	using BreadcrumbLib.Interfaces;

	public class PathExistsValidationRule : ValidationRule
	{
		#region fields
		private IHierarchyHelper _hierarchyHelper;
		private object _root;
		#endregion fields

		#region constructors
		public PathExistsValidationRule(IHierarchyHelper hierarchyHelper, object root)
		{
			this._hierarchyHelper = hierarchyHelper;
			this._root = root;
		}

		public PathExistsValidationRule()
		{
		}
		#endregion constructors

		#region methods
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			try
			{
				if (!(value is string))
					return new ValidationResult(false, "Invalid Path");

				if (this._hierarchyHelper.GetItem(this._root, (string)value) == null)
					return new ValidationResult(false, "Path Not Found");
			}
			catch
			{
				return new ValidationResult(false, "Invalid Path");
			}
			return new ValidationResult(true, null);
		}
		#endregion methods
	}
}
