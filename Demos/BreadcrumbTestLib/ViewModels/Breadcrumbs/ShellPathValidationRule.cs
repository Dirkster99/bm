namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using WSF.Interfaces;
    using System.Globalization;
    using System.Windows.Controls;

    /// <summary>
    /// Provides a way to create a custom rule in order to check the validity of user
    ///  input and signal invalid input with WPF standard binding API.
    /// </summary>
    public class ShellPathValidationRule : ValidationRule
    {
        #region ctors
        /// <summary>
        /// Class constructors
        /// </summary>
        public ShellPathValidationRule()
        {
        }
        #endregion ctors

        /// <summary>
        /// Performs validation checks on a value.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A System.Windows.Controls.ValidationResult object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (!(value is string))
                    return new ValidationResult(false, "Invalid Path");

                IDirectoryBrowser[] pathItems;

                if (WSF.Browser.DirectoryExists((string)value, out pathItems) == false)
                    return new ValidationResult(false, "Path Not Found");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Validate Exception: '{0}'\n{1}", ex.Message, ex.StackTrace);
                return new ValidationResult(false, "Invalid Path");
            }

            return new ValidationResult(true, null);
        }
    }
}
