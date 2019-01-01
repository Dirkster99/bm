namespace BreadcrumbTestLib.ViewModels
{
    using BreadcrumbTestLib.Models;
    using ShellBrowser.Enums;
    using ShellBrowserLib;
    using ShellBrowserLib.Enums;
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.Interfaces;
    using SuggestLib.Interfaces;
    using SuggestLib.SuggestSource;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a suggestion object to generate suggestions
    /// based on sub entries of a specified string.
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
        public Task<ISuggestResult> SuggestAsync(object location,
                                                string input,
                                                IHierarchyHelper helper)
        {
            input = (input == null ? string.Empty : input);

            // location indicator matches input? if so, use this to list suggestion
            var li = location as LocationIndicator;
            if (li != null && input.Length > 1)
            {
                input = ShellBrowser.NormalizePath(input);

                PathType pathType = PathType.Unknown;
                string path = li.GetPath(input, out pathType);

                var match = ShellBrowser.IsCurrentPath(input, path);

                switch (match)
                {
                    case PathMatch.CompleteMatch:
                        return ListChildren(li.GetPathModels(), pathType, li);

                    case PathMatch.PartialSource:
                        {
                            string pathExt = null;
                            var pathModels = li.GetPathModels();
                            int idx = ShellBrowser.FindCommonRoot(pathModels, input, out pathExt);

                            if (idx > 0)
                            {
                                idx++;  // Keep common root and remove everything else

                                li.RemoveRange(idx, li.Count - idx);
                                return ListChildren(li.GetPathModels(), pathType, li);
                            }
                        }
                        break;

                    case PathMatch.PartialTarget:
                        {
                            var pathList = li.Items.ToList();
                            string pathExt = null;
                            if (ShellBrowser.IsParentPathOf(path, input, out pathExt))
                            {
                                if (ShellBrowser.ExtendPath(ref pathList, pathExt))
                                {
                                    li.ResetPath(pathList);
                                    return ListChildren(li.GetPathModels(), pathType, li);
                                }
                            }
                        }
                        break;

                    case PathMatch.Unrelated:
                    default:
                        li.Clear();      // Clear loaction indicator since path is completely new ...
                        break;
                }
            }

            if (input.Length <= 1)
                return Task.FromResult<ISuggestResult>(ListRootItems(input));

            // Location indicator may be pointing somewhere unrelated ...
            // Are we searching a drive based path ?
            if (ShellBrowser.IsTypeOf(input) == PathType.FileSystemPath)
                return Task.FromResult<ISuggestResult>(ParseFileSystemPath(input, li));
            else
            {
                // Win shell path folder
                IDirectoryBrowser[] path = null;
                if (ShellBrowserLib.ShellBrowser.DirectoryExists(input, out path))
                    return ListChildren(path, PathType.WinShellPath, li);
                else
                {
                    // List RootItems or last known parent folder's children based on seperator
                    int sepIdx = input.LastIndexOf('\\');
                    if (sepIdx <= 0)
                        return Task.FromResult<ISuggestResult>(ListRootItems(input));
                    else
                    {
                        var parentDir = input.Substring(0, sepIdx);
                        var searchMask = input.Substring(sepIdx+1) + "*";

                        // Win shell path folder
                        path = null;
                        if (ShellBrowserLib.ShellBrowser.DirectoryExists(parentDir, out path))
                            return ListChildren(path, PathType.WinShellPath, li, searchMask);
                    }
                }
            }

            return Task.FromResult<ISuggestResult>(new SuggestResult());
        }

        /// <summary>
        /// Gets a list of child elements (if any available) of the list item in the array.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathType"></param>
        /// <param name="searchMask"></param>
        /// <returns></returns>
        private static Task<ISuggestResult> ListChildren(IDirectoryBrowser[] path,
                                                        PathType pathType,
                                                        LocationIndicator li,
                                                        string searchMask = null)
        {
            if (li != null)
                li.ResetPath(path);

            var dirPath = path[path.Length - 1];

            SuggestResult result = new SuggestResult();

            string namedPath = path[0].Name;
            for (int i = 1; i < path.Length; i++)
                namedPath = namedPath + '\\' + path[i].Name;

            foreach (var item in ShellBrowserLib.ShellBrowser.GetChildItems(dirPath.PathShell, searchMask))
            {
                if (string.IsNullOrEmpty(item.PathFileSystem) == false)
                {
                    if (pathType == PathType.WinShellPath)
                    {
                        AddItem(result, item.Label, namedPath + '\\' + item.Name,
                                PathType.WinShellPath, path);
                    }
                    else
                    {
                            AddItem(result, item.Label, item.PathFileSystem,
                                    PathType.FileSystemPath, path);
                    }
                }
            }

            return Task.FromResult<ISuggestResult>(result);
        }

        /// <summary>
        /// Gets a list of logical drives attached to thisPC.
        /// </summary>
        /// <returns></returns>
        private ISuggestResult ListRootItems(string input)
        {
            SuggestResult result = new SuggestResult();

            // Get Root Items below ThisPC
            var parent = ShellBrowser.MyComputer;
            foreach (var item in ShellBrowser.GetChildItems(parent.SpecialPathId,
                                                            input + "*", SubItemFilter.NameOrParsName))
            {
                if (string.IsNullOrEmpty(item.PathFileSystem) == false)
                {
                    AddItem(result, item.Label, item.PathFileSystem, PathType.FileSystemPath, parent);
                }
            }

            // Get Root Items below Desktop
            parent = ShellBrowser.DesktopDirectory;
            foreach (var item in ShellBrowser.GetChildItems(parent.SpecialPathId, input + "*"))
            {
                // filter out RecycleBin, ControlPanel... since its not that useful here...
                bool IsFilteredItem = string.Compare(item.SpecialPathId, KF_IID.ID_FOLDERID_RecycleBinFolder, true) == 0 ||
                                      string.Compare(item.PathRAW, "::{26EE0668-A00A-44D7-9371-BEB064C98683}", true) == 0;

                // Filter out ThisPC since its items are handled in previous loop
                bool IsThisPC = string.Compare(item.SpecialPathId, KF_IID.ID_FOLDERID_ComputerFolder, true) == 0;

                if (IsFilteredItem == false && IsThisPC == false)
                {
                    AddItem(result, item.Label, item.Name, PathType.WinShellPath, parent);
                }
            }

            // Pronounce path as invalid if list of suggestions is empty and path is non-existing
            // We do this here because path could be non-existing but we could still find filter suggestions
            // (eg: 'c' -> suggestion 'C:\')
            if (result.Suggestions.Count == 0)
            {
                IDirectoryBrowser[] pathItems = null;
                if (ShellBrowser.DirectoryExists(input, out pathItems) == false)
                    result.ValidPath = false;
            }

            return result;
        }

        /// <summary>
        /// Checks whether the:
        /// 1) given path exists or its
        /// 2) parent path exists
        /// 
        /// and returns a list of children for 1) or 2).
        /// 
        /// Method resests <paramref name="li"/> if either path can be resolved and
        /// parameter is not null.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="li"></param>
        /// <returns></returns>
        private ISuggestResult ParseFileSystemPath(string input,
                                                   LocationIndicator li)
        {
            IDirectoryBrowser[] pathItems = null;
            if (ShellBrowser.DirectoryExists(input, out pathItems))
            {
                if (li != null)
                    li.ResetPath(pathItems);

                return ListSubItems(input);
            }

            // List RootItems or last known parent folder's children based on seperator
            int sepIdx = input.LastIndexOf('\\');
            if (sepIdx > 0)
            {
                var parentDir = input.Substring(0, sepIdx);
                var searchMask = input.Substring(sepIdx + 1) + "*";

                // Win shell path folder
                if (ShellBrowser.DirectoryExists(parentDir, out pathItems))
                {
                    if (li != null)
                        li.ResetPath(pathItems);

                    return ListSubItems(parentDir, searchMask);
                }
            }

            return new SuggestResult();
        }

        /// <summary>
        /// Gets a list of subdirectories to the given path if it exists.
        /// The returned items Value contain a complete path for each item.
        /// </summary>
        /// <returns></returns>
        private ISuggestResult ListSubItems(string input,
                                            string searchMask = null)
        {
            SuggestResult result = new SuggestResult();

            var parent = ShellBrowser.Create(input);
            foreach (var item in ShellBrowserLib.ShellBrowser.GetChildItems(input, searchMask))
            {
                AddItem(result, item.Label, item.PathFileSystem, PathType.FileSystemPath, parent);
            }

            return result;
        }

        private static void AddItem(ISuggestResult result,
                                    string header,
                                    string textPath, PathType pathType,
                                    object parent)
        {
            var newItem = new SuggestionListItem(header, textPath, parent, pathType);
            result.Suggestions.Add(newItem);
        }
    }
}
