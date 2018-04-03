///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2009 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace DirectoryInfoExLib.IO.Tools.Interface
{
    using DirectoryInfoExLib.IO.FileSystemInfoExt;

    public interface ISupportDrag
    {
        FileSystemInfoEx[] ItemsToDrag { get; }
    }
}
