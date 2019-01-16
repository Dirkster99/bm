namespace SuggestBoxTestLib.DataSources
{
    using SuggestBoxTestLib.DataSources.Directory;
    using SuggestBoxTestLib.ViewModels;
    using SuggestBoxTestLib.ViewModels.Base;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Defines a suggestion object to generate suggestions
    /// based on sub entries of specified string.
    /// </summary>
    public class DirectorySuggestSource : ViewModelBase
    {
        #region fields
        private readonly Dictionary<string, CancellationTokenSource> _Queue;
        private readonly SemaphoreSlim _SlowStuffSemaphore;
        private readonly FastObservableCollection<object> _ListQueryResult;
        private ICommand _SuggestTextChangedCommand;
        private string _CurrentText;
        private bool _IsValidText = true;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DirectorySuggestSource()
        {
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
                var result = await SuggestAsync(queryThis);

                return result;
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
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
        /// <param name="input"></param>
        /// <param name="validInput"></param>
        /// <returns></returns>
        public Task<SuggestQueryResultModel> SuggestAsync(string input)
        {
            if (string.IsNullOrEmpty(input) == false)
            {
                if (input.Length <= 3)
                    return Task.FromResult<SuggestQueryResultModel>(ListDrives(input));
            }

            return Task.FromResult<SuggestQueryResultModel>(ListSubDirs(input));
        }

        private SuggestQueryResultModel ListSubDirs(string input)
        {
            if (string.IsNullOrEmpty(input))
                return GetLogicalDrives();

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
                    string folder = input.Substring(0, sepIdx + 1);
                    string searchPattern = input.Substring(sepIdx + 1) + "*";

                    string[] directories=null;
                    try
                    {
                        directories = System.IO.Directory.GetDirectories(folder, searchPattern);
                    }
                    catch
                    {
                        // Catch invalid path exceptions here ...
                    }

                    if (directories != null)
                    {
                        List<object> dirs = new List<object>();

                        for (int i = 0; i < directories.Length; i++)
                            dirs.Add(new { Header = directories[i], Value = directories[i] });

                        return new SuggestQueryResultModel(dirs);
                    }
                }
            }

            return GetLogicalDrives();
        }

        /// <summary>
        /// Gets a list of logical drives attached to thisPC.
        /// </summary>
        /// <returns></returns>
        private SuggestQueryResultModel ListDrives(string input)
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
                else
                {
                    return new SuggestQueryResultModel(new List<object>(), false);
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

        private static SuggestQueryResultModel GetLogicalDrives()
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

            return new SuggestQueryResultModel(drives);
        }

        private static SuggestQueryResultModel GetLogicalDriveOrSubDirs(
            string testDrive,
            string input)
        {
            if (System.IO.Directory.Exists(testDrive) == true)
            {
                List<object> drives = new List<object>();

                // List the drive itself if there was only 1 or 2 letters
                // since this is not a valid drive and we don'nt know if the user
                // wants to go to the drive or a folder contained in it
                if (input.Length <= 2)
                    drives.Add(new { Header = testDrive, Value = testDrive });

                // and list all sub-directories of that drive
                foreach (var item in System.IO.Directory.GetDirectories(testDrive))
                    drives.Add(new { Header = item, Value = item });

                return new SuggestQueryResultModel(drives);
            }

            return null;
        }
        #endregion input parser
        #endregion methods
    }
}
