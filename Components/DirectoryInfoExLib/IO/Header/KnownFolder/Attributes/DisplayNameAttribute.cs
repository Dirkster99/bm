namespace DirectoryInfoExLib.IO.Header.KnownFolder.Attributes
{
    using System;

    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
