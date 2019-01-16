namespace BreadcrumbTestLib.ViewModels
{
    using BreadcrumbTestLib.Models;
    using BreadcrumbTestLib.Utils;
    using BreadcrumbTestLib.ViewModels.Base;
    using ShellBrowserLib;
    using ShellBrowserLib.Enums;
    using ShellBrowserLib.IDs;
    using ShellBrowserLib.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Defines a suggestion object to generate suggestions
    /// based on sub entries of a specified string.
    /// </summary>
    public class SuggestSourceDirectory : ViewModels.Base.ViewModelBase
    {
        #region fields
        private readonly LocationIndicator _LocationIndicator;
        
        private readonly Dictionary<string, CancellationTokenSource> _Queue;
        private readonly SemaphoreSlim _SlowStuffSemaphore;
        private readonly FastObservableCollection<object> _ListQueryResult;
        private ICommand _SuggestTextChangedCommand;
        private string _CurrentText;
        private bool _IsValidText = true;
        #endregion fields
      
        #region ctors
        /// <summary>
        /// Parameterized class constructor
        /// </summary>
        /// <param name="li">Is a helper object that can be used to keep state and thus
        /// speed up query time by trying to associate the current query as an extension
        /// of the last query...</param>
        internal SuggestSourceDirectory(LocationIndicator li)
            : this()
        {
          _LocationIndicator = li;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        internal SuggestSourceDirectory()
        {
            _LocationIndicator = new LocationIndicator();
            _Queue = new Dictionary<string, CancellationTokenSource>();
            _SlowStuffSemaphore = new SemaphoreSlim(1, 1);
            _ListQueryResult = new FastObservableCollection<object>();
        }
        #endregion ctors

        #region properties       
        /// <summary>
        /// Gets whether the last query text was valid or invalid.
        /// </summary>
        public bool IsValidText
        {
            get
            {
                return _IsValidText;
            }

            protected set
            {
                if (_IsValidText != value)
                {
                    _IsValidText = value;
                    NotifyPropertyChanged(() => IsValidText);
                }
            }
        }

        internal LocationIndicator CurrentPath
        {
            get
            {
                return _LocationIndicator;
            }
        }

        /// <summary>
        /// Gets/sets the text currently edit in the SuggestBox
        /// </summary>
        public string CurrentText
        {
            get
            {
                return _CurrentText;
            }

            set
            {
                if (_CurrentText != value)
                {
                    _CurrentText = value;
                    NotifyPropertyChanged(() => CurrentText);
                }
            }
        }

        /// <summary>
        /// Gets a collection of items that represent likely suggestions towards
        /// a previously entered text.
        /// </summary>
        public IEnumerable<object> ListQueryResult
        {
            get
            {
                return _ListQueryResult;
            }
        }

        /// <summary>
        /// Gets a command that queries a sub-system in order to resolve a query
        /// based on a previously entered text. The entered text is expected as
        /// parameter of this command.
        /// </summary>
        public ICommand SuggestTextChangedCommand
        {
            get
            {
                if (_SuggestTextChangedCommand == null)
                {
                    _SuggestTextChangedCommand = new RelayCommand<object>(async (p) =>
                    {
                        // We want to process empty strings here as well
                        string newText = p as string;
                        if (newText == null)
                            return;

                        var suggestions = await SuggestTextChangedCommand_Executed(newText);

                        _ListQueryResult.Clear();
                        IsValidText = suggestions.ValidInput;

                        if (IsValidText == true)
                            _ListQueryResult.AddItems(suggestions.ResultList);
                    });
                }

                return _SuggestTextChangedCommand;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Method replaces the current location indicator with the stated indicator.
        /// </summary>
        /// <param name="locations">Location indicator object that describes a current location</param>
        internal void ResetLocationIndicator(LocationIndicator locations)
        {
          _LocationIndicator.ResetPath(locations);
        }
        
        private async Task<SuggestQueryResultModel> SuggestTextChangedCommand_Executed(string queryThis)
        {
            // Cancel current task(s) if there is any...
            var queueList = _Queue.Values.ToList();

            for (int i = 0; i < queueList.Count; i++)
                queueList[i].Cancel();

            var tokenSource = new CancellationTokenSource();
            _Queue.Add(queryThis, tokenSource);

            // Make sure the task always processes the last input but is not started twice
            await _SlowStuffSemaphore.WaitAsync();
            try
            {
                // There is more recent input to process so we ignore this one
                if (_Queue.Count > 1)
                {
                    _Queue.Remove(queryThis);
                    return null;
                }

                // Do the search and return results
                var result = await SuggestAsync(_LocationIndicator, queryThis);

                return result;
            }
            catch (System.Exception exp)
            {
                System.Console.WriteLine(exp.Message);
            }
            finally
            {
                _Queue.Remove(queryThis);
                _SlowStuffSemaphore.Release();
            }

            return null;
        }

        #region input parser
        /// <summary>
        /// Method returns a task that returns a list of suggestion objects
        /// that are associated to the <paramref name="input"/> string
        /// and given <paramref name="location"/> object.
        /// </summary>
        /// <param name="location">Currently selected location.</param>
        /// <param name="input">Text input to formulate string based path.</param>
        /// <returns></returns>
        internal Task<SuggestQueryResultModel> SuggestAsync(LocationIndicator location,
                                                            string input)
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
                return Task.FromResult<SuggestQueryResultModel>(ListRootItems(input));

            // Location indicator may be pointing somewhere unrelated ...
            // Are we searching a drive based path ?
            if (ShellBrowser.IsTypeOf(input) == PathType.FileSystemPath)
                return Task.FromResult<SuggestQueryResultModel>(ParseFileSystemPath(input, li));
            else
            {
                // Win shell path folder
                IDirectoryBrowser[] path = null;
                if (ShellBrowserLib.ShellBrowser.DirectoryExists(input, out path, true))
                    return ListChildren(path, PathType.WinShellPath, li);
                else
                {
                    // List RootItems or last known parent folder's children based on seperator
                    int sepIdx = input.LastIndexOf('\\');
                    if (sepIdx <= 0)
                        return Task.FromResult<SuggestQueryResultModel>(ListRootItems(input));
                    else
                    {
                        var parentDir = input.Substring(0, sepIdx);
                        var searchMask = input.Substring(sepIdx+1) + "*";

                        // Win shell path folder
                        path = null;
                        if (ShellBrowserLib.ShellBrowser.DirectoryExists(parentDir, out path, true))
                            return ListChildren(path, PathType.WinShellPath, li, searchMask);
                    }
                }
            }

            return Task.FromResult<SuggestQueryResultModel>(new SuggestQueryResultModel());
        }

        /// <summary>
        /// Gets a list of child elements (if any available) of the list item in the array.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathType"></param>
        /// <param name="searchMask"></param>
        /// <returns></returns>
        private static Task<SuggestQueryResultModel> ListChildren(IDirectoryBrowser[] path,
                                                                  PathType pathType,
                                                                  LocationIndicator li,
                                                                  string searchMask = null)
        {
            if (li != null)
                li.ResetPath(path);

            var dirPath = path[path.Length - 1];

            SuggestQueryResultModel result = new SuggestQueryResultModel();

            string namedPath = string.Empty;
            if (pathType == PathType.WinShellPath)
            {
                namedPath = path[0].Name;
                for (int i = 1; i < path.Length; i++)
                    namedPath = namedPath + '\\' + path[i].Name;
            }

            foreach (var item in ShellBrowser.GetSlimChildItems(dirPath.PathShell, searchMask))
            {
                SuggestionListItem Itemsuggest = null;
                if (pathType == PathType.WinShellPath)
                {
                    Itemsuggest = CreateItem(item.LabelName, namedPath + '\\' + item.ParseName,
                                                PathType.WinShellPath, path);
                }
                else
                {
                    if (ShellBrowser.IsTypeOf(item.Name) != PathType.SpecialFolder)
                    {
                        Itemsuggest = CreateItem(item.LabelName, item.Name,
                                                    PathType.FileSystemPath, path);
                    }
                }

                result.ResultList.Add(Itemsuggest);
            }

            return Task.FromResult<SuggestQueryResultModel>(result);
        }

        /// <summary>
        /// Gets a list of logical drives attached to thisPC.
        /// </summary>
        /// <returns></returns>
        private SuggestQueryResultModel ListRootItems(string input)
        {
            SuggestQueryResultModel result = new SuggestQueryResultModel();

            // Get Root Items below ThisPC
            var parent = ShellBrowser.MyComputer;
            foreach (var item in ShellBrowser.GetSlimChildItems(parent.SpecialPathId,
                                                                     input + "*", SubItemFilter.NameOrParsName))
            {
                if (ShellBrowser.IsTypeOf(item.Name) != PathType.SpecialFolder)
                {
                    var Itemsuggest = CreateItem(item.LabelName, item.Name, PathType.FileSystemPath, parent);
                    result.ResultList.Add(Itemsuggest);
                }
            }

            // Get Root Items below Desktop
            parent = ShellBrowser.DesktopDirectory;
            foreach (var item in ShellBrowser.GetSlimChildItems(parent.SpecialPathId, input + "*"))
            {
                // filter out RecycleBin, ControlPanel... since its not that useful here...
                bool IsFilteredItem = string.Compare(item.Name, KF_IID.ID_FOLDERID_RecycleBinFolder, true) == 0 ||
                                      string.Compare(item.Name, "::{26EE0668-A00A-44D7-9371-BEB064C98683}", true) == 0;

                // Filter out ThisPC since its items are handled in previous loop
                bool IsThisPC = string.Compare(item.Name, "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}", true) == 0;

                if (IsFilteredItem == false && IsThisPC == false)
                {
                    var Itemsuggest = CreateItem(item.LabelName, item.ParseName, PathType.WinShellPath, parent);
                    result.ResultList.Add(Itemsuggest);
                }
            }

            // Pronounce path as invalid if list of suggestions is empty and path is non-existing
            // We do this here because path could be non-existing but we could still find filter suggestions
            // (eg: 'c' -> suggestion 'C:\')
            if (result.ResultList.Count == 0)
            {
                IDirectoryBrowser[] pathItems = null;
                if (ShellBrowser.DirectoryExists(input, out pathItems) == false)
                    result.ValidInput = false;
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
        private SuggestQueryResultModel ParseFileSystemPath(string input,
                                                            LocationIndicator li)
        {
            IDirectoryBrowser[] pathItems = null;
            if (ShellBrowser.DirectoryExists(input, out pathItems, true))
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
                if (ShellBrowser.DirectoryExists(parentDir, out pathItems, true))
                {
                    if (li != null)
                        li.ResetPath(pathItems);

                    return ListSubItems(parentDir, searchMask);
                }
            }

            return new SuggestQueryResultModel();
        }

        /// <summary>
        /// Gets a list of subdirectories to the given path if it exists.
        /// The returned items Value contain a complete path for each item.
        /// </summary>
        /// <returns></returns>
        private SuggestQueryResultModel ListSubItems(string input,
                                                     string searchMask = null)
        {
            SuggestQueryResultModel result = new SuggestQueryResultModel();

            var parent = ShellBrowser.Create(input);
            foreach (var item in ShellBrowser.GetSlimChildItems(input, searchMask))
            {
                if (ShellBrowser.IsTypeOf(item.Name) != PathType.SpecialFolder)
                {
                    result.ResultList.Add(CreateItem(item.LabelName, item.Name,
                                                     PathType.FileSystemPath, parent));
                }
            }

            return result;
        }

        private static SuggestionListItem CreateItem(string header,
                                                     string textPath,
                                                     PathType pathType,
                                                     object parent)
        {
            return new SuggestionListItem(header, textPath, parent, pathType);
        }
    }
    #endregion input parser
    #endregion methods
}
