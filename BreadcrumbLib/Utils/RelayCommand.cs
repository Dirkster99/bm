namespace BreadcrumbLib.Utils
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
			this._handler = handler;
			this._canHandle = canHandle ?? (pm => true);
		}

		public event EventHandler CanExecuteChanged;

		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}

			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;

					if (this.CanExecuteChanged != null)
					{
						this.CanExecuteChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		public bool CanExecute(object parameter)
		{
			return this.IsEnabled && this._canHandle(parameter);
		}

		public void Execute(object parameter)
		{
			this._handler(parameter);
		}
	}
}
