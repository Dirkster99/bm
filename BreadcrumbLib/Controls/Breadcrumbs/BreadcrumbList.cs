namespace BreadcrumbLib.Controls.Breadcrumbs
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Controls;
	using BreadcrumbLib.BaseControls.Breadcrumb;
	using BreadcrumbLib.Controls.SuggestBox;
	using BreadcrumbLib.Interfaces;
	using BreadcrumbLib.Utils;

	public class BreadcrumbList : BreadcrumbBase
	{
		#region fields
		public static readonly DependencyProperty SelectedPathValueProperty =
				DependencyProperty.Register("SelectedPathValue", typeof(string),
				typeof(BreadcrumbList), new UIPropertyMetadata(null, OnSelectedPathValueChanged));

		public static readonly DependencyProperty RootItemProperty =
		 DependencyProperty.Register("RootItem", typeof(object), typeof(BreadcrumbList),
		 new PropertyMetadata(null));

		public static readonly DependencyProperty HierarchyHelperProperty =
				DependencyProperty.Register("HierarchyHelper", typeof(IHierarchyHelper),
				typeof(BreadcrumbList), new PropertyMetadata(new PathHierarchyHelper("Parent", "Value", "SubEntries"), OnHierarchyHelperPropChanged));

		public static readonly DependencyProperty ParentPathProperty =
				DependencyProperty.Register("Parent", typeof(string),
				typeof(BreadcrumbList), new PropertyMetadata(OnHierarchyHelperPropChanged));

		public static readonly DependencyProperty SubentriesPathProperty =
			 DependencyProperty.Register("SubentriesPath", typeof(string),
				typeof(BreadcrumbList), new PropertyMetadata(OnHierarchyHelperPropChanged));

		public static readonly DependencyProperty SuggestSourceProperty =
				DependencyProperty.Register("SuggestSource", typeof(ISuggestSource),
				typeof(BreadcrumbList), new UIPropertyMetadata(new AutoSuggestSource()));

		private bool _updatingHierarchyHelper = false;
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public BreadcrumbList()
		{
		}
		#endregion constructors

		#region properties
		/// <summary>
		/// Path value of the SelectedValue object, bindable.
		/// </summary>
		public string SelectedPathValue
		{
			get { return (string)GetValue(SelectedPathValueProperty); }
			set { this.SetValue(SelectedPathValueProperty, value); }
		}

		/// <summary>
		/// Root item of the breadcrumbnail
		/// </summary>
		public object RootItem
		{
			get { return (object)GetValue(RootItemProperty); }
			set { this.SetValue(RootItemProperty, value); }
		}

		#region HierarchyHelper, ParentPath, ValuePath, SubEntriesPath, SuggestSource
		/// <summary>
		/// Uses to navigate the hierarchy, one can also set the ParentPath/ValuePath and SubEntriesPath instead.
		/// </summary>
		public IHierarchyHelper HierarchyHelper
		{
			get { return (IHierarchyHelper)GetValue(HierarchyHelperProperty); }
			set { this.SetValue(HierarchyHelperProperty, value); }
		}

		/// <summary>
		/// The path of view model to access parent.
		/// </summary>
		public string ParentPath
		{
			get { return (string)GetValue(ParentPathProperty); }
			set { this.SetValue(ParentPathProperty, value); }
		}

		/// <summary>
		/// The path of view model to access sub entries.
		/// </summary>
		public string SubentriesPath
		{
			get { return (string)GetValue(SubentriesPathProperty); }
			set { this.SetValue(SubentriesPathProperty, value); }
		}

		/// <summary>
		/// Uses by SuggestBox to suggest options.
		/// </summary>
		public ISuggestSource SuggestSource
		{
			get { return (ISuggestSource)GetValue(SuggestSourceProperty); }
			set { this.SetValue(SuggestSourceProperty, value); }
		}
		#endregion
		#endregion properties

		#region Methods
		public static void OnSelectedPathValueChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var bread = sender as BreadcrumbList;
			if (bread.bcore != null &&
					(e.NewValue == null || !e.NewValue.Equals(bread.HierarchyHelper.GetPath(bread.SelectedValue))))
				bread.Select(bread.HierarchyHelper.GetItem(bread.RootItem, e.NewValue as string));
		}

		public static void OnHierarchyHelperPropChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var bread = sender as BreadcrumbList;

			// If HierarchyHelper changed, update Parent/Value/Subentries path, vice versa.
			if (!bread._updatingHierarchyHelper)
			{
				bread._updatingHierarchyHelper = true;
				try
				{
					if (e.Property.Equals(HierarchyHelperProperty))
					{
						if (bread.HierarchyHelper.ParentPath != bread.ParentPath)
							bread.ParentPath = bread.HierarchyHelper.ParentPath;

						if (bread.HierarchyHelper.ValuePath != bread.ValuePath)
							bread.ValuePath = bread.HierarchyHelper.ValuePath;

						if (bread.HierarchyHelper.SubentriesPath != bread.SubentriesPath)
							bread.SubentriesPath = bread.HierarchyHelper.SubentriesPath;
					}
					else
					{
						bread.HierarchyHelper = new PathHierarchyHelper(bread.ParentPath, bread.ValuePath, bread.SubentriesPath);
					}
				}
				finally
				{
					bread._updatingHierarchyHelper = false;
				}
			}
		}

		/// <summary>
		/// Is called when a control template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			// Update Suggestions when text changed.
			tbox.AddHandler(TextBox.TextChangedEvent, (RoutedEventHandler)((o, e) =>
			{
				if (tbox.IsEnabled)
				{
					var suggestSource = SuggestSource;
					var hierarchyHelper = HierarchyHelper;
					string text = tbox.Text;
					object data = RootItem;

					Task.Run(async () =>
					{
						return await suggestSource.SuggestAsync(data, text, hierarchyHelper);
					}).ContinueWith((pTask) =>
					{
						if (!pTask.IsFaulted)
							this.SetValue(SuggestionsProperty, pTask.Result);
					}, TaskScheduler.FromCurrentSynchronizationContext());
				}
			}));

			this.AddValueChanged(BreadcrumbBase.ValuePathProperty, (o, e) =>
			{
				BreadcrumbList.OnHierarchyHelperPropChanged(this,
						new DependencyPropertyChangedEventArgs(BreadcrumbBase.ValuePathProperty, null, ValuePath));
			});

			this.AddValueChanged(RootItemProperty, this.OnRootItemChanged);
			this.OnRootItemChanged(this, EventArgs.Empty);
		}

		public override void Select(object value)
		{
			base.Select(value);
			if (this.bcore != null && value != null)
			{
				var hierarchy = this.HierarchyHelper.GetHierarchy(value, true).Reverse().ToList();
				this.SetValue(ItemsControl.ItemsSourceProperty, hierarchy);
				this.SelectedPathValue = this.HierarchyHelper.GetPath(value);
				this.bcore.SetValue(BreadcrumbCore.ShowDropDownProperty, this.SelectedPathValue != string.Empty);
			}
		}

		public void OnRootItemChanged(object sender, EventArgs args)
		{
			if (this.RootItem != null)
			{
				this.Items.Clear();
				this.SetValue(BreadcrumbBase.RootItemsSourceProperty, this.HierarchyHelper.List(this.RootItem));
				////bcore.RootItems = tbox.RootItems = this.HierarchyHelper.List(RootItem);
				////bcore.ShowDropDown = false;
			}
		}
		#endregion
	}
}
