using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellDll
{
    public class KnownFolderGuidAttribute  : Attribute
    {
        public Guid Guid { get; set; }

        public KnownFolderGuidAttribute(Guid guid)
        {            
            Guid = guid;
        }

        public KnownFolderGuidAttribute(string str)
            : this(new Guid(str))
        {

        }

        public KnownFolderGuidAttribute(byte a, byte b, byte c, byte d , byte e, byte f, byte g, byte h, byte i, byte j, byte k)
            : this(new Guid(a,b,c,d,e,f,g,h, i,j,k))
        {

        }
    }
}
