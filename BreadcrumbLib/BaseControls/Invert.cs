namespace BreadcrumbLib.BaseControls
{
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using System.Windows.Media;
	using BreadcrumbLib.Utils;

	public class Invert : ContentControl
	{
		#region fields
		public static readonly DependencyProperty NormalForegroundBrushProperty =
				DependencyProperty.Register("NormalForegroundBrush", typeof(Brush),
				typeof(Invert), new UIPropertyMetadata(Brushes.Black));

		public static readonly DependencyProperty NormalBackgroundBrushProperty =
				DependencyProperty.Register("NormalBackgroundBrush", typeof(Brush),
				typeof(Invert), new UIPropertyMetadata(Brushes.Transparent));

		public static readonly DependencyProperty MouseOverForegroundBrushProperty =
				DependencyProperty.Register("MouseOverForegroundBrush", typeof(Brush),
				typeof(Invert), new UIPropertyMetadata(Brushes.Transparent));

		public static readonly DependencyProperty MouseOverBackgroundBrushProperty =
				DependencyProperty.Register("MouseOverBackgroundBrush", typeof(Brush),
				typeof(Invert), new UIPropertyMetadata(Brushes.Black));

		public static readonly DependencyProperty DisabledForegroundBrushProperty =
				DependencyProperty.Register("DisabledForegroundBrush", typeof(Brush),
				typeof(Invert), new UIPropertyMetadata(Brushes.Transparent));

		public static readonly DependencyProperty DisabledBackgroundBrushProperty =
				DependencyProperty.Register("DisabledBackgroundBrush", typeof(Brush),
				typeof(Invert), new UIPropertyMetadata(Brushes.Gray));
		#endregion fields

		#region Constructor

		#endregion

		#region properties
		public Brush NormalForegroundBrush
		{
			get { return (Brush)GetValue(NormalForegroundBrushProperty); }
			set { this.SetValue(NormalForegroundBrushProperty, value); }
		}

		public Brush NormalBackgroundBrush
		{
			get { return (Brush)GetValue(NormalBackgroundBrushProperty); }
			set { this.SetValue(NormalBackgroundBrushProperty, value); }
		}

		public Brush MouseOverForegroundBrush
		{
			get { return (Brush)GetValue(MouseOverForegroundBrushProperty); }
			set { this.SetValue(MouseOverForegroundBrushProperty, value); }
		}

		public Brush MouseOverBackgroundBrush
		{
			get { return (Brush)GetValue(MouseOverBackgroundBrushProperty); }
			set { this.SetValue(MouseOverBackgroundBrushProperty, value); }
		}

		public Brush DisabledForegroundBrush
		{
			get { return (Brush)GetValue(DisabledForegroundBrushProperty); }
			set { this.SetValue(DisabledForegroundBrushProperty, value); }
		}

		public Brush DisabledBackgroundBrush
		{
			get { return (Brush)GetValue(DisabledBackgroundBrushProperty); }
			set { this.SetValue(DisabledBackgroundBrushProperty, value); }
		}
		#endregion properties

		#region methods
		/// <summary>
		/// Is called when a control template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.UpdateStates(false);
			this.AddValueChanged(Invert.IsEnabledProperty, (o, e) => this.UpdateStates(true));
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			this.UpdateStates(true);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			this.UpdateStates(true);
		}

		private void UpdateStates(bool useTransition)
		{
			if (this.IsEnabled == false)
			{
				VisualStateManager.GoToState(this, "Disabled", useTransition);
				this.SetValue(Control.ForegroundProperty, this.DisabledForegroundBrush);
				this.SetValue(Control.BackgroundProperty, this.DisabledBackgroundBrush);
			}
			else
				if (this.IsMouseOver)
				{
					VisualStateManager.GoToState(this, "MouseOver", useTransition);
					this.SetValue(Control.ForegroundProperty, this.MouseOverForegroundBrush);
					this.SetValue(Control.BackgroundProperty, this.MouseOverBackgroundBrush);
				}
				else
				{
					VisualStateManager.GoToState(this, "Normal", useTransition);
					this.SetValue(Control.ForegroundProperty, this.NormalForegroundBrush);
					this.SetValue(Control.BackgroundProperty, this.NormalBackgroundBrush);
				}
		}
		#endregion methods
	}
}
