using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellDll
{
    public class FolderCategoryAttribute : Attribute
    {
        public KnownFolderCategory Category { get; set; }

        public FolderCategoryAttribute(KnownFolderCategory category)
        {
            Category = category;
        }
    }
}
