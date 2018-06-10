namespace Test
{
    using DirectoryInfoExLib;
    using DirectoryInfoExLib.Enums;
    using DirectoryInfoExLib.IO.Header.KnownFolder;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var documents = Factory.CreateDirectoryInfoEx(KnownFolder.FromKnownFolderId(KnownFolder_GUIDS.Documents));

            Console.WriteLine(documents);
            Console.WriteLine("->");
            Console.WriteLine(documents.Parent.Name); // ::{20D04FE0-3AEA-1069-A2D8-08002B30309D} -> CLSID 'My PC'

            Console.ReadKey();
        }
    }
}
