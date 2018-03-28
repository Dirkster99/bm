using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellDll
{
    public enum KnownFolderRedirectionCapabilities
    {
        AllowAll = 0xff,
        Redirectable = 0x1,
        DenyAll = 0xfff00,
        DenyPolicyRedirected = 0x100,
        DenyPolicy = 0x200,
        DenyPermissions = 0x400
    }
}
