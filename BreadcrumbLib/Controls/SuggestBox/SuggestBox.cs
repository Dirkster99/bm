namespace BreadcrumbLib.Controls.SuggestBox
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Controls;
	using BreadcrumbLib.Interfaces;
	using BreadcrumbLib.Utils;

	/// <summary>
	/// Uses ISuggestSource and HierarchyHelper to suggest automatically.
	/// </summary>
	public class SuggestBox : SuggestBoxBase
	{
		#region fields
		public static readonly DependencyProperty HierarchyHelperProperty =
				DependencyProperty.Register("HierarchyHelper", typeof(IHierarchyHelper),
				typeof(SuggestBox), new UIPropertyMetadata(new PathHierarchyHelper("Parent", "Value", "SubEntries")));

		public static readonly DependencyProperty SuggestSourcesProperty = DependencyProperty.Register(
				"SuggestSources", typeof(IEnumerable<ISuggestSource>), typeof(SuggestBox), new PropertyMetadata(
						new List<ISuggestSource>(new[] { new AutoSuggestSource() })));

		public static readonly DependencyProperty RootItemProperty = DependencyProperty.Register("RootItem",
				typeof(object), typeof(SuggestBox), new PropertyMetadata(null));
		#endregion fields

		#region Constructor

		public SuggestBox()
		{
		}

		#endregion

		#region properties
		#region HierarchyHelper, SuggestSource
		public IHierarchyHelper HierarchyHelper
		{
			get { return (IHierarchyHelper)this.GetValue(HierarchyHelperProperty); }
			set { this.SetValue(HierarchyHelperProperty, value); }
		}

		public IEnumerable<ISuggestSource> SuggestSources
		{
			get { return (IEnumerable<ISuggestSource>)this.GetValue(SuggestSourcesProperty); }
			set { this.SetValue(SuggestSourcesProperty, value); }
		}
		#endregion

		/// <summary>
		/// Assigned by Breadcrumb
		/// </summary>
		public object RootItem
		{
			get { return this.GetValue(RootItemProperty); }
			set { this.SetValue(RootItemProperty, value); }
		}
		#endregion properties

		#region methods
		#region OnEventHandler
		protected static new void OnSuggestionsChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			SuggestBox sbox = sender as SuggestBox;

			if (args.OldValue != args.NewValue)
				sbox.popupIfSuggest();
		}

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			base.OnTextChanged(e);
			var suggestSources = this.SuggestSources;
			var hierarchyHelper = this.HierarchyHelper;
			string text = this.Text;
			object data = this.RootItem;
			this.IsHintVisible = string.IsNullOrEmpty(text);

			if (this.IsEnabled && suggestSources != null)
				Task.Run(async () =>
				{
					var tasks = from s in suggestSources
											select
												s.SuggestAsync(data, text, hierarchyHelper);
					await Task.WhenAll(tasks);
					return tasks.SelectMany(tsk => tsk.Result).Distinct().ToList();
				}).ContinueWith(
						(pTask) =>
						{
							if (!pTask.IsFaulted)
								this.SetValue(SuggestionsProperty, pTask.Result);
						}, TaskScheduler.FromCurrentSynchronizationContext());
		}
		#endregion

		/// <summary>
		/// Utils - Update Bindings
		/// </summary>
		protected override void updateSource()
		{
			var txtBindingExpr = this.GetBindingExpression(TextBox.TextProperty);
			if (txtBindingExpr == null)
				return;

			bool valid = true;

			if (this.HierarchyHelper != null)
				valid = this.HierarchyHelper.GetItem(this.RootItem, this.Text) != null;

			if (valid)
			{
				if (txtBindingExpr != null)
					txtBindingExpr.UpdateSource();

				this.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
			}
			else Validation.MarkInvalid(txtBindingExpr,
					new ValidationError(new PathExistsValidationRule(), txtBindingExpr,
							"Path not exists.", null));
		}
		#endregion methods
	}
}
