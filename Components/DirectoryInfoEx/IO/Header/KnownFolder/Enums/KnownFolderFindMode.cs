using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellDll
{
    public enum KnownFolderFindMode : int
    {
        ExactMatch = 0,
        NearestParentMatch = ExactMatch + 1
    };

}
