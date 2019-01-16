namespace SuggestBoxTestLib.DataSources.Directory
{
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
    public class DummySuggestions : ViewModelBase
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
        public DummySuggestions()
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
        /// 
        /// This sample is really easy because it simply takes the input
        /// string and add an output as suggestion to the given input.
        /// 
        /// This always returns 2 suggestions.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<SuggestQueryResultModel> SuggestAsync(string input)
        {
            var result = new SuggestQueryResultModel();

            // returns a collection of anynymous objects
            // each with a Header and Value property
            result.ResultList.Add(new { Header = input + "-add xyz", Value = input + "xyz" });
            result.ResultList.Add(new { Header = input + "-add abc", Value = input + "abc" });

            return Task.FromResult<SuggestQueryResultModel>(result);
        }
        #endregion input parser
        #endregion methods
    }
}
