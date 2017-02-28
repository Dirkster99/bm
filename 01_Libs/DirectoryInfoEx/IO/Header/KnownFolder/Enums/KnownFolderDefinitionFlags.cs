using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellDll
{
    public enum KnownFolderDefinitionFlags
    {
        LocalRedirectOnly = 0x2,
        Roamable = 0x4,
        Precreate = 0x8
    }
}
