namespace Client
{
    using WSF;
    using WSF.IDs;
    using WSF.Shell.Interop.Interfaces.Knownfolders;
    using WSF.Shell.Interop.Knownfolders;
    using System;
    using System.Collections.Generic;

    class Program
    {
        static void Main(string[] args)
        {
            TestAllFolders();

            // Show known folders with file system path and without file system path
            KnownFolder_PhysicalPathExistence();
            Console.WriteLine("\n");

            using (var desktopKnownFolder = KnownFolderHelper.GetDesktop())
            {
                Console.WriteLine("    Desktop Knownfolder Category: '{0}'", desktopKnownFolder.Obj.GetCategory());
                Console.WriteLine("Desktop Knownfolder IsFileSystem: '{0}'", desktopKnownFolder.IsFileSystem());
            }

            // test getting known folder interface from path with GUID format '::{...}'
            Console.WriteLine("\n");
            using (var myPC = KnownFolderHelper.FromPath(KF_IID.ID_FOLDERID_ComputerFolder))
            {
                Console.WriteLine("Computer Knownfolder Category: '{0}'", myPC.Obj.GetCategory());
                Console.WriteLine("Computer Knownfolder IsFileSystem: '{0}'", myPC.IsFileSystem());

                bool isFilesystem = false;
                string path = KnownFolderHelper.GetPath(out isFilesystem, myPC.Obj);
                Console.WriteLine("                                 Path: '{0}' <- Exists: {1}", path, isFilesystem);
            }

            // test getting common and per user known folder interface from file system path
            Console.WriteLine("\n");
            using (var myDocs = KnownFolderHelper.FromCanonicalName("My Music"))
            {
                Console.WriteLine("My Music Knownfolder Category: '{0}'",
                    myDocs.Obj.GetCategory());

                Console.WriteLine("My Music Knownfolder IsFileSystem: '{0}'", myDocs.IsFileSystem());

                bool isFilesystem = false;
                string path = KnownFolderHelper.GetPath(out isFilesystem, myDocs.Obj);
                Console.WriteLine("                                 Path: '{0}' <- Exists: {1}", path, isFilesystem);
            }

            var commonDocumentsPath = Environment.SpecialFolder.CommonDocuments;
            Console.WriteLine("\n");
            using (var commonDocs = KnownFolderHelper.FromPath(Environment.GetFolderPath(commonDocumentsPath)))
            {
                Console.WriteLine("COMMON Documents from path '{0}' Knownfolder Category: '{1}'",
                    commonDocumentsPath, commonDocs.Obj.GetCategory());

                Console.WriteLine("COMMON Documents Knownfolder IsFileSystem: '{0}'", commonDocs.IsFileSystem());

                bool isFilesystem = false;
                string path = KnownFolderHelper.GetPath(out isFilesystem, commonDocs.Obj);
                Console.WriteLine("                                     Path: '{0}' <- Exists: {1}", path, isFilesystem);
            }

            // test getting known folder interface from path with GUID format '::{...}'
            Console.WriteLine("\n");
            using (var myMusic = KnownFolderHelper.FromPath(KF_IID.ID_FOLDERID_Music))
            {
                Console.WriteLine("My Music Knownfolder Category: '{0}'", myMusic.Obj.GetCategory());
                Console.WriteLine("My Music Knownfolder IsFileSystem: '{0}'", myMusic.IsFileSystem());

                var props = KnownFolderHelper.GetFolderProperties(myMusic.Obj);

                Console.WriteLine("Properties: {0}", props);
            }

            // List all known folders
            Console.WriteLine("\n");
            Console.WriteLine("Listing Canonical Names for all known folders:");
            foreach (var item in KnownFolderHelper.GetAllFolders())
            {
                Console.WriteLine("Knownfolder {0}", item.Value.CanonicalName);
            }

            // Test service functions ...
            Console.WriteLine("\n");

            var directory = Browser.Create(@"C:tmp");

            var music = Browser.Create(KF_IID.ID_FOLDERID_Music);

            var desktop = Browser.Create(KF_IID.ID_FOLDERID_Desktop);

            Console.WriteLine("");
            Console.WriteLine("Enumerating children below '{0}' item", desktop.Name);
            foreach (var item in Browser.GetChildItems(desktop.SpecialPathId))
            {
                Console.WriteLine("Name '{0}' SpecialPathId '{1}' PathFileSystem '{2}'",
                    item.Name, item.SpecialPathId, item.PathFileSystem);
            }

            var thisPC = Browser.Create(KF_IID.ID_FOLDERID_ComputerFolder);

            Console.WriteLine("");
            Console.WriteLine("Enumerating children below '{0}' item", thisPC.Name);
            foreach (var item in Browser.GetChildItems(thisPC.SpecialPathId))
            {
                Console.WriteLine("Name '{0}' SpecialPathId '{1}' PathFileSystem '{2}'",
                    item.Name, item.SpecialPathId, item.PathFileSystem);
            }

            var drive = Browser.Create(@"C:");
            Console.WriteLine("");
            Console.WriteLine("Enumerating children below '{0}' item", drive.Name);
            foreach (var item in Browser.GetChildItems(drive.PathFileSystem))
            {
                Console.WriteLine("Name '{0}' SpecialPathId '{1}' PathFileSystem '{2}'",
                    item.Name, item.SpecialPathId, item.PathFileSystem);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Assignes all known folders into a category of folders:
        /// 1) with physical existance in the file system
        /// 2) without physical existance in the file system
        /// </summary>
        private static void KnownFolder_PhysicalPathExistence()
        {
            var existingFolders = new Dictionary<string, KnownfolderSlim>();
            var nonExistingFolders = new Dictionary<string, KnownfolderSlim>();

            // Get all IID value pairs defined in KF_IID in one dictionary
            var Ids = KF_IID.GetIdKnownFolders();

            foreach (KeyValuePair<string, KnownfolderSlim> pair in Ids)
            {
                var folderid = pair.Key;

                string fs_path = KnownFolderHelper.GetKnownFolderPath(pair.Key);
                bool exists = false;

                if (fs_path != null)
                    exists = System.IO.Directory.Exists(fs_path);

                if (exists == true)
                    existingFolders.Add(pair.Key, new KnownfolderSlim(pair.Key,pair.Value.Name, fs_path));
                else
                    nonExistingFolders.Add(pair.Key, pair.Value);
            }

            // Print information on all shell folders that do not exist locally
            Console.WriteLine("Folders NOT existing in file system (is either virtual or has never been created):");
            foreach (KeyValuePair<string, KnownfolderSlim> pair in nonExistingFolders)
            {
                var folderid = pair.Value;

                Console.WriteLine("'{0}' -> '{1}'", folderid, pair.Key);
            }

            // Print information on all shell folders that do exist locally
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Folders existing in file system:");
            foreach (KeyValuePair<string, KnownfolderSlim> pair in existingFolders)
            {
                var folderid = pair.Key;

                string path = KnownFolderHelper.GetKnownFolderPath(folderid);

                Console.WriteLine("'{0}' -> '{1}' @ '{2}'", folderid, pair.Key, path);
            }
        }

        private static void TestAllFolders()
        {
            Dictionary<string, IKnownFolderProperties> propColl = new Dictionary<string, IKnownFolderProperties>();

            // List all known folders
            Console.WriteLine("\n");
            foreach (var item in KnownFolderHelper.GetAllFolders())
            {
                Console.WriteLine("{0}\n", item);
            }
        }
    }
}
