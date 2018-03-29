namespace Themes.Base
{
	using System;
	using System.ComponentModel;
	using System.Linq.Expressions;

	/// <summary>
	/// Helper use this instead of PropertyChangedBase so it can be port to other framework.
	/// </summary>
	public class NotifyPropertyChanged : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void NotifyOfPropertyChanged<T>(Expression<Func<T>> expression)
		{
			this.NotifyOfPropertyChanged(this.GetPropertyName<T>(expression));
		}

		protected virtual void NotifyOfPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		protected string GetPropertyName<T>(Expression<Func<T>> expression)
		{
			MemberExpression memberExpression = (MemberExpression)expression.Body;
			return memberExpression.Member.Name;
		}
	}
}
