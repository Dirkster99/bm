namespace BreadcrumbLib.BaseControls
{
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Take multiple ContentTemplates (ContentTemplate, ContentTemplate2) and render it using the same Content.
	/// </summary>
	public class MultiContentControl : ContentControl
	{
		#region fields
		public static readonly DependencyProperty ContentVisible1Property =
			 DependencyProperty.Register("ContentVisible1", typeof(bool), typeof(MultiContentControl),
			 new PropertyMetadata(true));

		public static readonly DependencyProperty ContentTemplate2Property =
				DependencyProperty.Register("ContentTemplate2", typeof(DataTemplate), typeof(MultiContentControl),
				new PropertyMetadata(null));

		public static readonly DependencyProperty ContentVisible2Property =
				DependencyProperty.Register("ContentVisible2", typeof(bool), typeof(MultiContentControl),
				new PropertyMetadata(false));

		public static readonly DependencyProperty ContentTemplate3Property =
				DependencyProperty.Register("ContentTemplate3", typeof(DataTemplate), typeof(MultiContentControl), new PropertyMetadata(null));

		public static readonly DependencyProperty ContentVisible3Property =
				DependencyProperty.Register("ContentVisible3", typeof(bool), typeof(MultiContentControl),
				new PropertyMetadata(true));
		#endregion fields

		#region constructors
		/// <summary>
		/// Static class constructor
		/// </summary>
		static MultiContentControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiContentControl),
					new FrameworkPropertyMetadata(typeof(MultiContentControl)));
		}
		#endregion constructors

		#region properties
		public bool ContentVisible1
		{
			get { return (bool)GetValue(ContentVisible1Property); }
			set { this.SetValue(ContentVisible1Property, value); }
		}

		public DataTemplate ContentTemplate2
		{
			get { return (DataTemplate)GetValue(ContentTemplate2Property); }
			set { this.SetValue(ContentTemplate2Property, value); }
		}

		public bool ContentVisible2
		{
			get { return (bool)GetValue(ContentVisible2Property); }
			set { this.SetValue(ContentVisible2Property, value); }
		}

		public DataTemplate ContentTemplate3
		{
			get { return (DataTemplate)GetValue(ContentTemplate3Property); }
			set { this.SetValue(ContentTemplate3Property, value); }
		}

		public bool ContentVisible3
		{
			get { return (bool)GetValue(ContentVisible3Property); }
			set { this.SetValue(ContentVisible3Property, value); }
		}
		#endregion properties

		#region Methods

		#endregion
	}
}
