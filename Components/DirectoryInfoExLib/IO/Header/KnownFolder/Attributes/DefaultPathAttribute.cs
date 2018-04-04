namespace DirectoryInfoExLib.IO.Header.KnownFolder.Attributes
{
    using System;

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
