using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellDll
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class SpecialFolderAttribute : Attribute
    {
        public Environment.SpecialFolder SpecialFolder { get; set; }

        public SpecialFolderAttribute(Environment.SpecialFolder specialFolder)
        {
            SpecialFolder = specialFolder;
        }
    }
}
