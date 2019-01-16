namespace SuggestBoxTestLib.DataSources.Auto
{
    using SuggestBoxTestLib.AutoSuggest;
    using SuggestBoxTestLib.DataSources.Auto.Interfaces;
    using SuggestBoxTestLib.DataSources.Directory;
    using SuggestBoxTestLib.ViewModels;
    using SuggestBoxTestLib.ViewModels.Base;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Defines a suggestion object to generate suggestions
    /// based on sub entries of specified string.
    /// </summary>
    public class AutoSuggestSource : ViewModelBase
    {
        #region fields
        private readonly LocationIndicator _LocationIndicator;

        private readonly Dictionary<string, CancellationTokenSource> _Queue;
        private readonly SemaphoreSlim _SlowStuffSemaphore;
        private readonly FastObservableCollection<object> _ListQueryResult;
        private ICommand _SuggestTextChangedCommand;
        private string _CurrentText;
        private bool _IsValidText = true;
        private bool _Processing;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="li">Is a helper object that can be used to keep state and thus
        /// speed up query time by trying to associate the current query as an extension
        /// of the last query...</param>
        public AutoSuggestSource(LocationIndicator li)
            : this()
        {
            _LocationIndicator = li;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected AutoSuggestSource()
        {
            _Queue = new Dictionary<string, CancellationTokenSource>();
            _SlowStuffSemaphore = new SemaphoreSlim(1, 1);
            _ListQueryResult = new FastObservableCollection<object>();
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets a property that indicates whether the initally constructed hierarchy for the
        /// SuggestBoxAuto2 SuggestBox is ready for consumtion or not.
        /// 
        /// Returns false if the background task is still busy constructing hierarchy items.
        /// </summary>
        public bool Processing
        {
            get { return _Processing; }
            private set
            {
                if (_Processing != value)
                {
                    _Processing = value;
                    NotifyPropertyChanged(() => Processing);
                }
            }
        }

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
        public void SetProcessing(bool value)
        {
            Processing = value;
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
        /// and given <paramref name="data"/> object.
        /// 
        /// The list of suggestion is empty if helper object is null.
        /// 
        /// This method is usually directly invoked by the SuggestBox to query data
        /// sources for suggestions as the user types a string character by character.
        /// </summary>
        /// <param name="location">Represents the root of the hierarchy that is browsed here.</param>
        /// <param name="input">Is the input string typed by the user.</param>
        /// <returns></returns>
        public Task<SuggestQueryResultModel> SuggestAsync(LocationIndicator location,
                                                          string input)
        {
            SuggestQueryResultModel retVal = new SuggestQueryResultModel();

            if (location == null)
                return Task.FromResult<SuggestQueryResultModel>(retVal);

            IHierarchyHelper hhelper = location.HierarchyHelper;

            if (hhelper == null)
                return Task.FromResult<SuggestQueryResultModel>(retVal);

            // Get the path from input string: 'c:\Windows' -> path: 'c:\'
            string valuePath = hhelper.ExtractPath(input);

            // Get the name from input string: 'c:\Windows' -> path: 'Windows'
            string valueName = hhelper.ExtractName(input);

            // Ensure that name ends with seperator if input ended with a seperator
            if (String.IsNullOrEmpty(valueName) && input.EndsWith(hhelper.Separator + ""))
                valueName += hhelper.Separator;

            // Ensure valid path if input ends with seperator and path was currently empty
            if (valuePath == "" && input.EndsWith("" + hhelper.Separator))
                valuePath = valueName;

            var found = hhelper.GetItem(location.RootItem, valuePath);

            if (found != null)
            {
                foreach (var item in hhelper.List(found))
                {
                    string valuePathName = hhelper.GetPath(item) as string;

                    if (valuePathName.StartsWith(input, hhelper.StringComparisonOption) &&
                        !valuePathName.Equals(input, hhelper.StringComparisonOption))
                        retVal.ResultList.Add(item);
                }
            }

            return Task.FromResult<SuggestQueryResultModel>(retVal);
        }
        #endregion input parser
        #endregion methods
    }
}
