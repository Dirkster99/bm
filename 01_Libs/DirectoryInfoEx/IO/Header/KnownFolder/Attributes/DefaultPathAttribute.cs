using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellDll
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DefaultPathAttribute : Attribute
    {
        public string DefaultPath { get; set; }

        public DefaultPathAttribute(string defaultPath)
        {
            DefaultPath = defaultPath;
        }
    }
}
