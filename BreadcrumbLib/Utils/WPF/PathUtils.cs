namespace BreadcrumbLib.Utils.WPF
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Source: FileExplorer.WPF.Utils
    /// </summary>
    public static class PathUtils
    {
        //Andre, Richard Ev on http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        public static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        //Dour High Arch on http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        public static string SanitizePath(string path, char replaceChar, char directorySeparatorChar = '\\')
        {
            int filenamePos = path.LastIndexOf(directorySeparatorChar) + 1;
            var sb = new System.Text.StringBuilder();
            sb.Append(path.Substring(0, filenamePos));
            for (int i = filenamePos; i < path.Length; i++)
            {
                char filenameChar = path[i];
                foreach (char c in Path.GetInvalidFileNameChars())
                    if (filenameChar.Equals(c))
                    {
                        filenameChar = replaceChar;
                        break;
                    }

                sb.Append(filenameChar);
            }

            return sb.ToString();
        }

        /// <summary>
        /// For BuildAction = Resource 
        /// return something like this : 
        /// pack://application:,,,/FileExplorer3.WPF;component/Themes/Default/Colors.xaml
        /// </summary>
        /// <param name="library"></param>
        /// <param name="path2Resource"></param>
        /// <returns></returns>
        public static Uri MakeResourceUri(string library, string path2Resource)
        {
            return new Uri(String.Format("pack://application:,,,/{0};component{1}", library,
                '/' + path2Resource.TrimStart('/')));
        }

        /// <summary>
        /// For BuildAction = EmbeddedResource
        /// return something like this :
        /// FileExplorer.WPF.Themes.Default.Colors.ico
        /// </summary>
        /// <param name="library"></param>
        /// <param name="path2Resource"></param>
        /// <returns></returns>
        public static string MakeResourcePath(string library, string path2Resource)
        {
            if (path2Resource.Contains("\\"))
                throw new ArgumentException("path2Resource");
            return String.Format("{0}.{1}", library, path2Resource.TrimStart('/').Replace('/', '.'));
        }
    }
}
