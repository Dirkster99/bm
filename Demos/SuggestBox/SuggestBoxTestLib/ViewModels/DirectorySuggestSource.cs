namespace SuggestBoxTestLib.ViewModels
{
    using SuggestLib.Interfaces;
    using SuggestLib.SuggestSource;
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
        public Task<ISuggestResult> SuggestAsync(object data,
                                                string input,
                                                IHierarchyHelper helper)
        {
            if (string.IsNullOrEmpty(input) == false)
            {
                if (input.Length <= 3)
                    return Task.FromResult<ISuggestResult>(ListDrives(input));
            }

            return Task.FromResult<ISuggestResult>(ListSubDirs(input));
        }

        private ISuggestResult ListSubDirs(string input)
        {
            var subDirs = GetLogicalDriveOrSubDirs(input, input);
            if (subDirs != null)
                return subDirs;

            // Find last seperator and list directories underneath
            // with * searchpattern
            if (subDirs == null)
            {
                int sepIdx = input.LastIndexOf('\\');

                if (sepIdx < input.Length)
                {
                    string folder = input.Substring(0, sepIdx+1);
                    string searchPattern = input.Substring(sepIdx + 1) + "*";
                    var directories = Directory.GetDirectories(folder, searchPattern).ToList();

                    if (directories != null)
                    {
                        ISuggestResult dirs = new SuggestResult();

                        for (int i = 0; i < directories.Count; i++)
                            dirs.Suggestions.Add(new { Header = directories[i], Value = directories[i] });

                        return dirs;
                    }
                }
            }

            return GetLogicalDrives();
        }

        /// <summary>
        /// Gets a list of logical drives attached to thisPC.
        /// </summary>
        /// <returns></returns>
        private ISuggestResult ListDrives(string input)
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

        private static ISuggestResult GetLogicalDrives()
        {
            ISuggestResult drives = new SuggestResult();

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

                    drives.Suggestions.Add(new { Header = header, Value = driveName });
                }
            }

            return drives;
        }

        private static ISuggestResult GetLogicalDriveOrSubDirs(
            string testDrive,
            string input)
        {
            if (Directory.Exists(testDrive) == true)
            {
                ISuggestResult drives = new SuggestResult();

                // List the drive itself if there was only 1 or 2 letters
                // since this is not a valid drive and we don'nt know if the user
                // wants to go to the drive or a folder contained in it
                if (input.Length <= 2)
                    drives.Suggestions.Add(new { Header = testDrive, Value = testDrive });

                // and list all sub-directories of that drive
                foreach (var item in Directory.GetDirectories(testDrive))
                    drives.Suggestions.Add(new { Header = item, Value = item });

                return drives;
            }

            return null;
        }
    }
}
