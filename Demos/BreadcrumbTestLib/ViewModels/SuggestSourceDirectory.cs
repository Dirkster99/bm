namespace BreadcrumbTestLib.ViewModels
{
    using BreadcrumbTestLib.Models;
    using ShellBrowser.Enums;
    using ShellBrowserLib;
    using ShellBrowserLib.Enums;
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.Interfaces;
    using SuggestLib.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
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

            // location indicator matches input? if so, use this to list suggestion
            var li = location as LocationIndicator;
            if (li != null && input.Length > 1)
            {
                input = ShellBrowser.NormalizePath(input);

                PathType pathType = PathType.Unknown;
                string path = li.GetPath(out pathType);

                var match = ShellBrowser.IsCurrentPath(input, path);

                switch (match)
                {
                    case PathMatch.CompleteMatch:
                        return ListChildren(li.GetPathModels(), pathType);

                    case PathMatch.PartialSource:
                        {
                            string pathExt = null;
                            var pathModels = li.GetPathModels();
                            int idx = ShellBrowser.FindCommonRoot(pathModels, input, out pathExt);

                            if (idx > 0)
                            {
                                idx++;  // Keep common root and remove everything else

                                li.RemoveRange(idx, li.Count - idx);
                                return ListChildren(li.GetPathModels(), pathType);
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
                                    return ListChildren(li.GetPathModels(), pathType);
                                }
                            }
                        }
                        break;

                    case PathMatch.Unrelated:
                    default:
                        break;
                }
            }

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
                    return ListChildren(path, PathType.WinShellPath);
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
                            return ListChildren(path, PathType.WinShellPath, searchMask);
                    }
                }
            }

            return Task.FromResult<IList<object>>(new List<object>());
        }

        /// <summary>
        /// Gets a list of child elements (if any available) of the list item in the array.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathType"></param>
        /// <param name="searchMask"></param>
        /// <returns></returns>
        private static Task<IList<object>> ListChildren(IDirectoryBrowser[] path,
                                                        PathType pathType,
                                                        string searchMask = null)
        {
            var dirPath = path[path.Length - 1];

            List<object> Items = new List<object>();
            string namedPath = path[0].Name;
            for (int i = 1; i < path.Length; i++)
                namedPath = namedPath + '\\' + path[i].Name;

            foreach (var item in ShellBrowserLib.ShellBrowser.GetChildItems(dirPath.PathShell, searchMask))
            {
                if (pathType == PathType.WinShellPath)
                {
                    AddItem(ref Items, item.Label, namedPath + '\\' + item.Name,
                            PathType.WinShellPath, path);
                }
                else
                {
                    AddItem(ref Items, item.Label, item.PathFileSystem,
                            PathType.FileSystemPath, path);
                }
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
            var parent = ShellBrowser.Create(KF_IID.ID_FOLDERID_Desktop);

            // Get Root Items below Desktop
            foreach (var item in ShellBrowserLib.ShellBrowser.GetChildItems(KF_IID.ID_FOLDERID_Desktop))
            {
                // filter out RecycleBin and ControlPanel...
                if (string.Compare(item.SpecialPathId, KF_IID.ID_FOLDERID_RecycleBinFolder, true) != 0 &&
                    string.Compare(item.PathRAW, "::{26EE0668-A00A-44D7-9371-BEB064C98683}", true) != 0)
                {
                    AddItem(ref rootItems, item.Label, item.Name, PathType.WinShellPath,
                                           parent);
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

            var parent = ShellBrowser.Create(input);
            foreach (var item in ShellBrowserLib.ShellBrowser.GetChildItems(input, searchMask))
            {
                AddItem(ref Items, item.Label, item.PathFileSystem, PathType.FileSystemPath,
                                   parent);
            }

            return Items;
        }

        private static void AddItem(ref List<object> items,
                                    string header,
                                    string textPath, PathType pathType,
                                    object parent)
        {
            var newItem = new SuggestionListItem(header, textPath, parent, pathType);
            items.Add(newItem);

            ////items.Add(new
            ////{
            ////    Header = header,
            ////    Value = value,
            ////    Parent = parent
            ////});
        }
    }
}
