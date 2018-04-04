namespace DirectoryInfoExLib.IO.Header.KnownFolder
{
    using DirectoryInfoExLib.IO.Header.KnownFolder.Enums;
    using System;

#pragma warning disable 0649
    public struct InternalKnownFolderDefinition
    {
        internal KnownFolderCategory Category;
        internal IntPtr pszName;
        internal IntPtr pszDescription;
        internal Guid ParentID;
        internal IntPtr pszRelativePath;
        internal IntPtr pszParsingName;
        internal IntPtr pszTooltip;
        internal IntPtr pszLocalizedName;
        internal IntPtr pszIcon;
        internal IntPtr pszSecurity;
        internal UInt32 dwAttributes;
        internal KnownFolderDefinitionFlags DefinitionFlags;
        internal Guid FolderTypeID;
    }
#pragma warning restore 0649
}
