namespace BreadcrumbLib.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Threading;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Threading;

	/// <summary>
	/// Thomas Levesque - http://stackoverflow.com/questions/3577802/wpf-getting-a-property-value-from-a-binding-path
	/// </summary>
	public static class PropertyPathHelper
	{
		#region fields
		internal static Dictionary<Tuple<Type, string>, PropertyInfo> CacheDic
				                         = new Dictionary<Tuple<Type, string>, PropertyInfo>();

		private static readonly Dummy DummyNode = new Dummy();
		#endregion fields

		#region methods
		public static object GetValueFromPropertyInfo(object obj, string[] propPaths)
		{
			var current = obj;
			foreach (var ppath in propPaths)
			{
				if (current == null)
					return null;

				Type type = current.GetType();
				var key = new Tuple<Type, string>(type, ppath);

				PropertyInfo pInfo = null;
				lock (CacheDic)
				{
					if (!(CacheDic.ContainsKey(key)))
					{
						pInfo = type.GetProperty(ppath);
						CacheDic.Add(key, pInfo);
					}
					pInfo = CacheDic[key];
				}

				if (pInfo == null)
					return null;
				current = pInfo.GetValue(current);
			}
			return current;
		}

		public static object GetValueFromPropertyInfo(object obj, string propertyPath)
		{
			return GetValueFromPropertyInfo(obj, propertyPath.Split('.'));
		}

		public static object GetValue(object obj, string propertyPath)
		{
			Dispatcher dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
			if (dispatcher == null)
				return GetValueFromPropertyInfo(obj, propertyPath);

			Binding binding = new Binding(propertyPath);
			binding.Mode = BindingMode.OneTime;
			binding.Source = obj;
			
			BindingOperations.SetBinding(PropertyPathHelper.DummyNode, Dummy.ValueProperty, binding);

			return PropertyPathHelper.DummyNode.GetValue(Dummy.ValueProperty);
		}

		public static object GetValue(object obj, BindingBase binding)
		{
			return GetValue(obj, (binding as Binding).Path.Path);
		}
		#endregion methods

		#region privates classes
		private class Dummy : DependencyObject
		{
			public static readonly DependencyProperty ValueProperty =
					DependencyProperty.Register("Value", typeof(object), typeof(Dummy), new UIPropertyMetadata(null));
		}
		#endregion privates classes
	}
}
