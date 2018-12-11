namespace BmLib.Utils
{
	using System;
	using System.ComponentModel;
	using System.Windows;
	using System.Windows.Media;
    using System.Windows.Threading;
    using System.Text;

	public static class UITools
	{
		public static void AddValueChanged<T>(this T obj, DependencyProperty property, EventHandler handler) where T : DependencyObject
		{
			DependencyPropertyDescriptor desc = DependencyPropertyDescriptor.FromProperty(property, typeof(T));

            if (desc != null)
            {
                desc.AddValueChanged(obj, handler);
            }
        }

		public static void AddValueChangedDispatcher<T>(this T obj, DependencyProperty property,
				EventHandler handler, DispatcherPriority priority) where T : DependencyObject
		{
			AddValueChanged(obj, property, (o, e) =>
					{
						obj.Dispatcher.BeginInvoke(priority, handler);
					});
		}

		public static string PrintVisualAnestor(DependencyObject obj)
		{
			StringBuilder sb = new StringBuilder();
			while (obj != null)
			{
				sb.AppendLine(obj.ToString());
				obj = VisualTreeHelper.GetParent(obj);
			}
			return sb.ToString();
		}

		public static T FindAncestor<T>(DependencyObject obj, Func<T, bool> filter = null) where T : DependencyObject
		{
			filter = filter ?? (depObj => true);

			while (obj != null && obj is Visual)
			{
				T o = obj as T;

				if (o != null && filter(o))
					return o;

				obj = VisualTreeHelper.GetParent(obj);

				if (obj != null && AttachedProperties.GetSkipLookup(obj))
					obj = null;
			}

			return default(T);
		}

		public static T FindLogicalAncestor<T>(DependencyObject obj, Func<T, bool> filter = null) where T : DependencyObject
		{
			filter = filter ?? (depObj => true);

			while (obj != null)
			{
				T o = obj as T;

				if (o != null && filter(o))
					return o;

				obj = LogicalTreeHelper.GetParent(obj);

				if (obj != null && AttachedProperties.GetSkipLookup(obj))
					obj = null;
			}

			return default(T);
		}
	}
}
