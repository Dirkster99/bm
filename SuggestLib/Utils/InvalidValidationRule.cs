namespace SuggestLib.Utils
{
    using System.Globalization;
    using System.Windows.Controls;

    /// <summary>
    /// Implements a validation rule that will always be invalid.
    /// </summary>
    public class InvalidValidationRule : ValidationRule
    {
        #region Constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public InvalidValidationRule()
        {
        }
        #endregion

        /// <summary>
        /// Always returns an invalid state - use this for standard
        /// marking of invalid input with red rectangle.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(false, "Invalid Path");
        }
    }
}