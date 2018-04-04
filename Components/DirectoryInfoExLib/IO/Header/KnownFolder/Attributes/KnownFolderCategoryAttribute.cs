namespace DirectoryInfoExLib.IO.Header.KnownFolder.Attributes
{
    using DirectoryInfoExLib.IO.Header.KnownFolder.Enums;
    using System;

    public class FolderCategoryAttribute : Attribute
    {
        public KnownFolderCategory Category { get; set; }

        public FolderCategoryAttribute(KnownFolderCategory category)
        {
            Category = category;
        }
    }
}
