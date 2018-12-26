namespace BreadcrumbTestLib.ViewModels
{
    using ShellBrowserLib;
    using ShellBrowserLib.Enums;
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.Interfaces;
    using SuggestLib.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a suggestion object to generate suggestions
    /// based on sub entries of specified string.
    /// </summary>
    public class SuggestSourceDirectory : ISuggestSource
    {
        /// <summary>
        /// Method returns a task that returns a list of suggestion objects
        /// that are associated to the <paramref name="input"/> string
        /// and given <paramref name="data"/> object.
        /// </summary>
        /// <param name="location">Currently selected location.</param>
        /// <param name="input">Text input to formulate string based path.</param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public Task<IList<object>> SuggestAsync(object location,
                                                string input,
                                                IHierarchyHelper helper)
        {
            input = (input == null ? string.Empty : input);

            if (input.Length <= 1)
                return Task.FromResult<IList<object>>(ListRootItems());

            // Are we searching a drive based path ?
            if (ShellBrowser.IsTypeOf(input) == PathType.FileSystemPath)
                return Task.FromResult<IList<object>>(ParseFileSystemPath(input));
            else
            {
                // Win shell path folder
                IDirectoryBrowser[] path = null;
                if (ShellBrowserLib.ShellBrowser.DirectoryExists(input, out path))
                    return ListChildren(path);
                else
                {
                    // List RootItems or last known parent folder's children based on seperator
                    int sepIdx = input.LastIndexOf('\\');
                    if (sepIdx <= 0)
                        return Task.FromResult<IList<object>>(ListRootItems());
                    else
                    {
                        var parentDir = input.Substring(0, sepIdx);
                        var searchMask = input.Substring(sepIdx+1) + "*";

                        // Win shell path folder
                        path = null;
                        if (ShellBrowserLib.ShellBrowser.DirectoryExists(parentDir, out path))
                            return ListChildren(path, searchMask);
                    }
                }
            }

            return Task.FromResult<IList<object>>(new List<object>());
        }

        /// <summary>
        /// Gets a list of child elements (if any available) of the list item in the array.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchMask"></param>
        /// <returns></returns>
        private static Task<IList<object>> ListChildren(IDirectoryBrowser[] path,
                                                        string searchMask = null)
        {
            var dirPath = path[path.Length - 1];

            List<object> Items = new List<object>();
            string namedPath = path[0].Name;
            for (int i = 1; i < path.Length; i++)
                namedPath = namedPath + '\\' + path[i].Name;

            foreach (var item in ShellBrowserLib.ShellBrowser.GetChildItems(dirPath.PathShell, searchMask))
            {
                Items.Add(new { Header = item.Label, Value = namedPath + '\\' + item.Name });
            }

            return Task.FromResult<IList<object>>(Items);
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

        private List<object> ParseFileSystemPath(string input)
        {
            if (ShellBrowser.DirectoryExists(input))
                return ListDriveItems(input);

            // List RootItems or last known parent folder's children based on seperator
            int sepIdx = input.LastIndexOf('\\');
            if (sepIdx > 0)
            {
                var parentDir = input.Substring(0, sepIdx);
                var searchMask = input.Substring(sepIdx + 1) + "*";

                // Win shell path folder
                if (ShellBrowserLib.ShellBrowser.DirectoryExists(parentDir))
                    return ListDriveItems(parentDir, searchMask);
            }

            return new List<object>();
        }

        /// <summary>
        /// Gets a list of subdirectories to the given path if it exists.
        /// The returned items Value contain a complete path for each item.
        /// </summary>
        /// <returns></returns>
        private List<object> ListDriveItems(string input,
                                            string searchMask = null)
        {
            List<object> Items = new List<object>();

            foreach (var item in ShellBrowserLib.ShellBrowser.GetChildItems(input, searchMask))
            {
                Items.Add(new { Header = item.Label, Value = item.PathFileSystem });
            }

            return Items;
        }
    }
}
