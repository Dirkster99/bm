namespace DirectoryInfoExLib.IO.Header.KnownFolder
{
    using DirectoryInfoExLib.IO.Header.KnownFolder.Enums;
    using System;

#pragma warning disable 0649
    /// <summary>
    /// Defines the specifics of a known folder.
    /// 
    /// When a third-party application creates their own known folder,
    /// they do so by defining it with a KNOWNFOLDER_DEFINITION structure,
    /// then registering it with the system.
    /// 
    /// Any registered known folder definition information—system-provided
    /// or application-created—can be retrived through this method.
    /// </summary>
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
