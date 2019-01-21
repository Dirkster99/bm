namespace WSF.Shell.Interop.ResourceIds
{
    using System;

    /// <summary>
    /// File attributes are metadata values stored by the file system on disk and
    /// are used by the system and are available to developers via various file I/O APIs.
    /// For a list of related APIs and topics, see the See Also section.
    /// 
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/gg258117(v=vs.85).aspx
    /// </summary>
    internal enum FileAttribute : UInt32
    {
        FILE_ATTRIBUTE_ARCHIVE = 0x20,
        FILE_ATTRIBUTE_COMPRESSED = 0x800,
        FILE_ATTRIBUTE_DEVICE = 0x40,
        FILE_ATTRIBUTE_DIRECTORY = 0x10,
        FILE_ATTRIBUTE_ENCRYPTED = 0x4000,
        FILE_ATTRIBUTE_HIDDEN = 0x2,
        FILE_ATTRIBUTE_INTEGRITY_STREAM = 0x8000,
        FILE_ATTRIBUTE_NORMAL = 0x80,
        FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x2000,
        FILE_ATTRIBUTE_NO_SCRUB_DATA = 0x20000,
        FILE_ATTRIBUTE_OFFLINE = 0x1000,
        FILE_ATTRIBUTE_READONLY = 0x1,
        FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS = 0x400000,
        FILE_ATTRIBUTE_RECALL_ON_OPEN = 0x40000,
        FILE_ATTRIBUTE_REPARSE_POINT = 0x400,
        FILE_ATTRIBUTE_SPARSE_FILE = 0x200,
        FILE_ATTRIBUTE_SYSTEM = 0x4,
        FILE_ATTRIBUTE_TEMPORARY = 0x100,
        FILE_ATTRIBUTE_VIRTUAL = 0x10000
    }
}
