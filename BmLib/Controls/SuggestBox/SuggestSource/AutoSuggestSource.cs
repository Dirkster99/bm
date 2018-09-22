namespace SuggestBoxDemo.SuggestSource
{
    using BmLib.Interfaces.SuggestBox;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Suggest based on sub entries of specified data.
    /// </summary>
    public class AutoSuggestSource : ISuggestSource
    {
        #region Constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public AutoSuggestSource()
        {
        }
        #endregion

        #region Methods
        public Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper)
        {
            if (helper == null)
                return Task.FromResult<IList<object>>(new List<Object>());

            string valuePath = helper.ExtractPath(input);
            string valueName = helper.ExtractName(input);

            if (String.IsNullOrEmpty(valueName) && input.EndsWith(helper.Separator + ""))
                valueName += helper.Separator;

            if (valuePath == "" && input.EndsWith("" + helper.Separator))
                valuePath = valueName;
            var found = helper.GetItem(data, valuePath);

            List<object> retVal = new List<object>();

            if (found != null && helper != null)
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
