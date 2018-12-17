namespace BreadcrumbTestLib.ViewModels
{
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.Interfaces;
    using SuggestLib.Interfaces;
    using System.Collections.Generic;
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
            input = (input == null ? string.Empty : input);

            if (input.Length <= 2)
                return Task.FromResult<IList<object>>(ListRootItems());

            // Are we searching a drive based path ?
            if ((char.ToLower(input[0]) >= 'a' && char.ToLower(input[0]) <= 'z' &&   // Drive based file system path
                 input[1] == ':' && input[2] == '\\') ||
                 (char.ToLower(input[0]) == '\\' && char.ToLower(input[0]) <= '\\')  // UNC file system path
                )
            {
                return Task.FromResult<IList<object>>(ListDriveItems(input));
            }
            else
            {
                // Shellspace path folder
                IDirectoryBrowser[] path = null;
                if (ShellBrowserLib.ShellBrowser.DirectoryExists(input, out path))
                {
                    List<object> Items = new List<object>();
                    var dirPath = path[path.Length - 1];

                    string namedPath = path[0].Name;
                    for (int i = 1; i < path.Length; i++)
                        namedPath = namedPath + '\\' + path[i].Name;

                    foreach (var item in ShellBrowserLib.ShellBrowser.GetChildItems(dirPath.PathShell))
                    {
                        Items.Add(new { Header = item.Label, Value = namedPath + '\\' + item.Name });
                    }

                    return Task.FromResult<IList<object>>(Items);
                }
            }

            return Task.FromResult<IList<object>>(new List<object>());
        }

        /// <summary>
        /// Gets a list of logical drives attached to thisPC.
        /// </summary>
        /// <returns></returns>
        private List<object> ListRootItems()
        {
            List<object> rootItems = new List<object>();

            // Get Root Items below Desktop
            // (filter out recycle bin and control panel entries since its not that useful...)
            foreach (var item in ShellBrowserLib.ShellBrowser.GetChildItems(KF_IID.ID_FOLDERID_Desktop))
            {
                if (string.Compare(item.SpecialPathId, KF_IID.ID_FOLDERID_RecycleBinFolder, true) != 0 &&
                    string.Compare(item.PathRAW, "::{26EE0668-A00A-44D7-9371-BEB064C98683}", true) != 0)
                {
                    rootItems.Add(new { Header = item.Label, Value = item.Name });
                }
            }

            return rootItems;
        }

        /// <summary>
        /// Gets a list of subdirectories to the given path if it exists.
        /// The returned items Value contain a complete path for each item.
        /// </summary>
        /// <returns></returns>
        private List<object> ListDriveItems(string input)
        {
            List<object> Items = new List<object>();

            foreach (var item in ShellBrowserLib.ShellBrowser.GetChildItems(input))
            {
                Items.Add(new { Header = item.Label, Value = item.PathFileSystem });
            }

            return Items;
        }
    }
}
