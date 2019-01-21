namespace WSF.Browse
{
    using System;
    using System.Text.RegularExpressions;

    internal class FilterMask
    {
        #region fields
        private readonly string _fileMask;
        private readonly bool _forceSlashCheck;
        private string _pattern;
        private Regex _regexPattern;
        #endregion fields

        #region ctors
        /// <summary>
        /// Parameterized class constructor.
        /// </summary>
        /// <param name="fileMask"></param>
        /// <param name="forceSlashCheck"></param>
        public FilterMask(string fileMask,
                          bool forceSlashCheck = false)
            : this()
        {
            _fileMask = fileMask;
            _forceSlashCheck = forceSlashCheck;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected FilterMask()
        {
        }
        #endregion ctors

        /// <summary>
        /// Determine whether filename matches a regular expression
        /// specified in constructors parameters ( * and ? supported) or not.
        /// 
        /// Source: http://stackoverflow.com/questions/725341/c-file-mask
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool MatchFileMask(string fileName)
        {
            if (fileName.Equals(_fileMask, StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (_fileMask == "*.*" || _fileMask == "*" && _forceSlashCheck == false)
                return true;

            if (_pattern == null)
            {
                _pattern = constructFileMaskRegexPattern(_fileMask, _forceSlashCheck);
                _regexPattern = new Regex(_pattern, RegexOptions.IgnoreCase);
            }

            return _regexPattern.IsMatch(fileName);
        }

        /// <summary>
        /// Returns a regular expression pattern that has escapped typical regular
        /// expression charcters, such as, '.' and so forth.
        /// 
        /// Escapes a minimal set of characters (\, *, +, ?, |, {, [, (,), ^, $,., #, and
        /// white space) by replacing them with their escape codes. This instructs the regular
        /// expression engine to interpret these characters literally rather than as metacharacters.
        /// </summary>
        /// <param name="fileMask"></param>
        /// <param name="forceSlashCheck"></param>
        /// <returns></returns>
        protected string constructFileMaskRegexPattern(string fileMask, bool forceSlashCheck)
        {
            if (!forceSlashCheck)
            {
                return '^' +
                Regex.Escape(fileMask.Replace(".", "__DOT__")
                                .Replace("*", "__STAR__")
                                .Replace("?", "__QM__"))
                    .Replace("__DOT__", "[.]")
                    .Replace("__STAR__", ".*")
                    .Replace("__QM__", ".")
                + '$';
            }
            else
            {
                return '^' +
                 Regex.Escape(fileMask.Replace(".", "__DOT__")
                                 .Replace("\\", "__SLASH__")
                                 .Replace("**", "__DOUBLESTAR__")
                                 .Replace("*", "__STAR__")
                                 .Replace("#", "__VARIABLE__")
                                 .Replace("(", "__OPENQUOTE__")
                                 .Replace(")", "__CLOSEQUOTE__")
                                 .Replace("?", "__QM__"))
                     .Replace("__DOT__", "[.]")
                     .Replace("__DOUBLESTAR__", ".*")
                     .Replace("__STAR__", "[^\\\\]*")
                     .Replace("__SLASH__", "[\\\\]")
                     .Replace("__VARIABLE__", "?")
                     .Replace("__OPENQUOTE__", "(")
                     .Replace("__CLOSEQUOTE__", ")")
                     .Replace("__QM__", ".")
                 + '$';
            }
        }
    }
}
