namespace PerformanceTestClient
{
    using ShellBrowserLib;
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.Interfaces;
    using System;
    using System.Collections.Generic;

    class Program
    {
        static void Main(string[] args)
        {
            TestAllWinSxsFoldersRetrieval();

            Console.ReadKey();
        }

        private static void TestAllWinSxsFoldersRetrieval()
        {
            var windowsFolder = ShellBrowser.Create(KF_IID.ID_FOLDERID_Windows);

            string dirPath = System.IO.Path.Combine(windowsFolder.PathFileSystem, "WinSxs");

            Console.WriteLine("Retrieving all sub-directories from '{0}'...\n", dirPath);

            // List all known folders
            Console.WriteLine("...{0} working on it...\n", DateTime.Now);

            List<IDirectoryBrowser> result = new List<IDirectoryBrowser>();
            int i = 0;
            foreach (var item in ShellBrowser.GetChildItems(dirPath))
            {
                result.Add(item);
                i++;

                if ((i % 1000) == 0)
                    Console.Write("{0}...", i);
            }

            // List all known folders
            Console.WriteLine("{0} Done retrieving {1} entries.\n", DateTime.Now, result.Count);
            Console.ReadKey();
        }
    }
}
