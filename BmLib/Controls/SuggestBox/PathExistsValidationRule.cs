namespace BmLib.Controls.SuggestBox
{
    using Interfaces.SuggestBox;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Controls;

    public class PathExistsValidationRule : ValidationRule
    {
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Constructor

        public PathExistsValidationRule(IHierarchyHelper hierarchyHelper, object root)
        {
            _hierarchyHelper = hierarchyHelper;
            _root = root;
        }

        public PathExistsValidationRule()
        {

        }

        #endregion

        #region Methods

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

        #endregion

        #region Data

        IHierarchyHelper _hierarchyHelper;
        object _root;

        #endregion

        #region Public Properties

        #endregion
    }
}