namespace ShellBrowserLib.Browser
{
    using System;
    using System.Text.RegularExpressions;

    internal class FilterMask
    {
        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public FilterMask()
        {
        }
        #endregion ctors

        /// <summary>
        /// Determine whether filename matches a regular expression
        /// specified in <paramref name="fileMask"/> ( * and ? supported) or not.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileMask"></param>
        /// <returns></returns>
        public bool MatchFileMask(string fileName, string fileMask)
        {
            return MatchFileMask(fileName, fileMask, false);
        }

        /// <summary>
        /// Determine whether filename matches a regular expression
        /// specified in <paramref name="fileMasks"/> ( * and ? supported) or not.
        /// 
        /// The <paramref name="fileMasks"/> parameter can contain more than 1 regular
        /// expression by delimiting each expression with ',' or ';' character.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileMasks">List of filter parameters.</param>
        /// <returns></returns>
        public bool MatchFileMasks(string fileName, string fileMasks)
        {
            string[] fileMaskList = fileMasks.Split(new char[] { ',', ';' });

            foreach (string fileMask in fileMaskList)
            {
                if (MatchFileMask(fileName, fileMask))
                    return true;

            }

            return false;
        }

        /// <summary>
        /// Determine whether filename matches a regular expression
        /// specified in <paramref name="fileMask"/> ( * and ? supported) or not.
        /// 
        /// Source: http://stackoverflow.com/questions/725341/c-file-mask
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileMask"></param>
        /// <param name="forceSlashCheck"></param>
        /// <returns></returns>
        public bool MatchFileMask(string fileName, string fileMask, bool forceSlashCheck)
        {
            if (fileName.Equals(fileMask, StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (fileMask == "*.*" || fileMask == "*" && !forceSlashCheck)
                return true;

            string pattern = constructFileMaskRegexPattern(fileMask, forceSlashCheck);

            return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(fileName);
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
