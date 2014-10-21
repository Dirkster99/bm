namespace BreadcrumbLib.Controls.SuggestBox
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using BreadcrumbLib.Interfaces;

  /// <summary>
  /// Suggest based on sub entries of specified data.
  /// </summary>
  public class AutoSuggestSource : ISuggestSource
  {
    #region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
    public AutoSuggestSource()
    {
    }
		#endregion constructors

		#region Methods
		public Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper)
    {
      if (helper == null)
        return Task.FromResult<IList<object>>(new List<object>());

      string valuePath = helper.ExtractPath(input);
      string valueName = helper.ExtractName(input);

      if (string.IsNullOrEmpty(valueName) && input.EndsWith(helper.Separator + string.Empty))
        valueName += helper.Separator;

			if (valuePath == string.Empty && input.EndsWith(string.Empty + helper.Separator))
        valuePath = valueName;

      var found = helper.GetItem(data, valuePath);
      List<object> retVal = new List<object>();

      if (found != null)
      {
        foreach (var item in helper.List(found))
        {
          string valuePathName = helper.GetPath(item) as string;
          if (valuePathName.StartsWith(input, helper.StringComparisonOption) &&
              !valuePathName.Equals(input, helper.StringComparisonOption))
            retVal.Add(item);
        }
      }

      return Task.FromResult<IList<object>>(retVal);
    }
    #endregion
  }
}
