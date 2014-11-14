using BreadcrumbLib.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breadcrumb.DirectoryInfoEx
{
    public class ExSuggestSource : ISuggestSource
    {
        public async Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper)
        {
            //data and helper is no longer used.
            string parentPath = input.EndsWith("\\") ? input.TrimEnd('\\') : PathEx.GetDirectoryName(input);
            string lookupName = input.EndsWith("\\") ? "" : PathEx.GetFileName(input);
            var parentDir = new System.IO.DirectoryInfoEx(parentPath);
            return parentDir.GetDirectories(lookupName + "*")
                .Where(di => !di.FullName.Equals(input, StringComparison.CurrentCultureIgnoreCase))
                . Cast<object>().ToList();

        }
    }
}
