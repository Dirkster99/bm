namespace BreadcrumbTestLib.Utils
{
    using System;
    using System.Windows.Input;

    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _handler;
        private readonly Func<object, bool> _canHandle;
        private bool _isEnabled = true;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="canHandle"></param>
        public RelayCommand(Action<object> handler, Func<object, bool> canHandle = null)
        {
            _handler = handler;
            _canHandle = canHandle ?? (pm => true);
        }

        public event EventHandler CanExecuteChanged;

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }

            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;

                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return IsEnabled && _canHandle(parameter);
        }

        public void Execute(object parameter)
        {
            _handler(parameter);
        }
    }
}
