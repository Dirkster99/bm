namespace SuggestBoxTestLib.AutoSuggest
{
    using Interfaces;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Controls;

    internal class PathExistsValidationRule : ValidationRule
    {
        #region fields
        private IHierarchyHelper _hierarchyHelper;
        private object _root;
        #endregion fields

        #region Constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="hierarchyHelper"></param>
        /// <param name="root"></param>
        public PathExistsValidationRule(IHierarchyHelper hierarchyHelper, object root)
        {
            _hierarchyHelper = hierarchyHelper;
            _root = root;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public PathExistsValidationRule()
        {

        }
        #endregion

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (!(value is string))
                    return new ValidationResult(false, "Invalid Path");

                if (_hierarchyHelper.GetItem(_root, (string)value) == null)
                    return new ValidationResult(false, "Path Not Found");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Validate Exception: '{0}'\n{1}", ex.Message, ex.StackTrace);

                return new ValidationResult(false, "Invalid Path");
            }

            return new ValidationResult(true, null);
        }
    }
}