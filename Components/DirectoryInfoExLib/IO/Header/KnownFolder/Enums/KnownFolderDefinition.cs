namespace DirectoryInfoExLib.IO.Header.KnownFolder.Enums
{
    using System;

    public struct KnownFolderDefinition
    {
        public KnownFolderCategory Category;        
        public string Name;
        public string Description;
        public Guid ParentID;
        public string RelativePath;
        public string ParsingName;
        public string Tooltip;
        public string LocalizedName;
        public string Icon;
        public string Security;
        public UInt32 Attributes;
        public KnownFolderDefinitionFlags DefinitionFlags;
        public Guid FolderTypeID;
    }
}
