﻿namespace DirectoryInfoExLib.IO.Header.KnownFolder.Enums
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
