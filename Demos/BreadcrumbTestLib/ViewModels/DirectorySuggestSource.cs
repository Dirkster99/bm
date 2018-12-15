namespace BreadcrumbTestLib.ViewModels
{
    using SuggestLib.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a suggestion object to generate suggestions
    /// based on sub entries of specified string.
    /// </summary>
    public class DirectorySuggestSource : ISuggestSource
    {
        /// <summary>
        /// Method returns a task that returns a list of suggestion objects
        /// that are associated to the <paramref name="input"/> string
        /// and given <paramref name="data"/> object.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="input"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public Task<IList<object>> SuggestAsync(object data,
                                                string input,
                                                IHierarchyHelper helper)
        {
            if (string.IsNullOrEmpty(input) == false)
            {
                if (input.Length <= 3)
                    return Task.FromResult<IList<object>>(ListDrives(input));
            }

            return Task.FromResult<IList<object>>(ListSubDirs(input));
        }

        private List<object> ListSubDirs(string input)
        {
            if (string.IsNullOrEmpty(input) == true)
                return new List<object>();

            var subDirs = GetLogicalDriveOrSubDirs(input, input);
            if (subDirs != null)
                return subDirs;

            try
            {
                // Find last seperator and list directories underneath
                // with * searchpattern
                if (subDirs == null)
                {
                    int sepIdx = input.LastIndexOf('\\');

                    if (sepIdx < input.Length)
                    {
                        string folder = input.Substring(0, sepIdx + 1);
                        string searchPattern = input.Substring(sepIdx + 1) + "*";
                        var directories = Directory.GetDirectories(folder, searchPattern).ToList();

                        if (directories != null)
                        {
                            List<object> dirs = new List<object>();

                            for (int i = 0; i < directories.Count; i++)
                                dirs.Add(new { Header = directories[i], Value = directories[i] });

                            return dirs;
                        }
                    }
                }

                return GetLogicalDrives();
            }
            catch
            {
                return subDirs;
            }
        }

        /// <summary>
        /// Gets a list of logical drives attached to thisPC.
        /// </summary>
        /// <returns></returns>
        private List<object> ListDrives(string input)
        {
            if (string.IsNullOrEmpty(input))
                return GetLogicalDrives();

            if (input.Length == 1)
            {
                if (char.ToUpper(input[0]) >= 'A' && char.ToUpper(input[0]) <= 'Z')
                {
                    // Check if we know this drive and list it with sub-folders if we do
                    var testDrive = input + ":\\";
                    var folders = GetLogicalDriveOrSubDirs(testDrive, input);
                    if (folders != null)
                        return folders;
                }
            }

            if (input.Length == 2)
            {
                if (char.ToUpper(input[1]) == ':' &&
                    char.ToUpper(input[0]) >= 'A' && char.ToUpper(input[0]) <= 'Z')
                {
                    // Check if we know this drive and list it with sub-folders if we do
                    var testDrive = input + "\\";
                    var folders = GetLogicalDriveOrSubDirs(testDrive, input);
                    if (folders != null)
                        return folders;
                }
            }

            if (input.Length == 3)
            {
                if (char.ToUpper(input[1]) == ':' &&
                    char.ToUpper(input[2]) == '\\' &&
                    char.ToUpper(input[0]) >= 'A' && char.ToUpper(input[0]) <= 'Z')
                {
                    // Check if we know this drive and list it with sub-folders if we do
                    var folders = GetLogicalDriveOrSubDirs(input, input);
                    if (folders != null)
                        return folders;
                }
            }

            return GetLogicalDrives();
        }

        private static List<object> GetLogicalDrives()
        {
            List<object> drives = new List<object>();

            foreach (var driveName in Environment.GetLogicalDrives())
            {
                if (string.IsNullOrEmpty(driveName) == false)
                {
                    string header;

                    try
                    {
                        DriveInfo d = new DriveInfo(driveName);
                        if (string.IsNullOrEmpty(d.VolumeLabel) == false)
                            header = string.Format("{0} ({1})", d.VolumeLabel, d.Name);
                        else
                            header = driveName;
                    }
                    catch
                    {
                        header = driveName;
                    }

                    drives.Add(new { Header = header, Value = driveName });
                }
            }

            return drives;
        }

        private static List<object> GetLogicalDriveOrSubDirs(
            string testDrive,
            string input)
        {
            if (Directory.Exists(testDrive) == true)
            {
                List<object> drives = new List<object>();

                // List the drive itself if there was only 1 or 2 letters
                // since this is not a valid drive and we don'nt know if the user
                // wants to go to the drive or a folder contained in it
                if (input.Length <= 2)
                    drives.Add(new { Header = testDrive, Value = testDrive });

                // and list all sub-directories of that drive
                foreach (var item in Directory.GetDirectories(testDrive))
                    drives.Add(new { Header = item, Value = item });

                return drives;
            }

            return null;
        }
    }
}
