using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellDll
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class CsidlAttribute : Attribute
    {
        public ShellAPI.CSIDL CSIDL { get; set; }        

        public CsidlAttribute(ShellAPI.CSIDL csidl)
        {
            CSIDL = csidl;
        }
    }
}
