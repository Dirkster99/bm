namespace SuggestBoxTestLib.ViewModels
{
    using SuggestLib.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
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
                if (input.Length == 1)
                {
                    return Task.FromResult<IList<object>>(ListDrives());
                }
            }

            // Strings with more than 1 character cannot be processed, yet.
            return Task.FromResult<IList<object>>(new List<Object>());
        }

        /// <summary>
        /// Gets a list of logical drives attached to thisPC.
        /// </summary>
        /// <returns></returns>
        private List<object> ListDrives()
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
    }
}
