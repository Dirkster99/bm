namespace DirectoryInfoExLib.IO.Header.KnownFolder.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class SpecialFolderAttribute : Attribute
    {
        public Environment.SpecialFolder SpecialFolder { get; set; }

        public SpecialFolderAttribute(Environment.SpecialFolder specialFolder)
        {
            SpecialFolder = specialFolder;
        }
    }
}
