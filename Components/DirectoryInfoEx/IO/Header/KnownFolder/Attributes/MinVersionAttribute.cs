using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellDll
{
    public enum OSVersions
    { 
        Windows81, Windows8, Windows7, WindowsVista
    }    

    //public static class OSVersions
    //{
    //    public static Version Windows81 = new Version(6, 3, 9431, 0);
    //    public static Version Windows8 = new Version(6, 2, 9200, 0);
    //    public static Version Windows7 = new Version(6, 1, 0, 0);
    //    public static Version WindowsVista = new Version(6, 0, 0, 0);
    //}

    public class MinVersionAttribute : Attribute
    {
        public OSVersions Version { get; set; }

        public MinVersionAttribute(OSVersions version)
        {            
        }
    }
}
