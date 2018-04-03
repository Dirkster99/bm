namespace DirectoryInfoExLib.IO.Header.KnownFolder.Attributes
{
    using DirectoryInfoExLib.IO.Header.ShellDll;
    using System;

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
