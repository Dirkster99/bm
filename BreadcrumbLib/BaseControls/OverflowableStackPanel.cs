namespace BreadcrumbLib.BaseControls
{
	using System;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;

	public class OverflowableStackPanel : StackPanel
	{
		#region fields
		public static DependencyProperty OverflowItemCountProperty = DependencyProperty.Register("OverflowItemCount", typeof(int),
				typeof(OverflowableStackPanel), new PropertyMetadata(0));

		public static DependencyProperty CanOverflowProperty = DependencyProperty.RegisterAttached("CanOverflow", typeof(bool),
				typeof(OverflowableStackPanel), new UIPropertyMetadata(false));

		public static DependencyProperty IsOverflowProperty = DependencyProperty.RegisterAttached("IsOverflow", typeof(bool),
			 typeof(OverflowableStackPanel), new UIPropertyMetadata(false));

		private double overflowableWH = 0;
		private double nonoverflowableWH = 0;
		#endregion fields

		#region Constructor

		#endregion

		#region properties
		public int OverflowItemCount
		{
			get { return (int)GetValue(OverflowItemCountProperty); }
			set { this.SetValue(OverflowItemCountProperty, value); }
		}

		public static bool GetCanOverflow(DependencyObject obj)
		{
			return (bool)obj.GetValue(CanOverflowProperty);
		}

		public static void SetCanOverflow(DependencyObject obj, bool value)
		{
			obj.SetValue(CanOverflowProperty, value);
		}

		public static bool GetIsOverflow(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsOverflowProperty);
		}

		public static void SetIsOverflow(DependencyObject obj, bool value)
		{
			obj.SetValue(IsOverflowProperty, value);
		}
		#endregion properties

		#region methods
		protected override Size MeasureOverride(Size constraint)
		{
			////if (double.IsPositiveInfinity(constraint.Width) || double.IsPositiveInfinity(constraint.Height))
			////    return base.MeasureOverride(constraint);

			var items = InternalChildren.Cast<UIElement>();

			this.overflowableWH = 0;
			this.nonoverflowableWH = 0;
			int overflowCount = 0;
			double maxHW = 0;

			foreach (var item in items)
			{
				item.Measure(constraint);
				maxHW = Math.Max(this.getHW(item.DesiredSize, this.Orientation), maxHW);
				if (GetCanOverflow(item))
					this.overflowableWH += this.getWH(item.DesiredSize, this.Orientation);
				else
					this.nonoverflowableWH += this.getWH(item.DesiredSize, this.Orientation);
			}

			foreach (var ele in items.Reverse())
			{
				if (GetCanOverflow(ele))
				{
					if (this.overflowableWH + this.nonoverflowableWH > this.getWH(constraint, this.Orientation))
					{
						overflowCount += 1;
						SetIsOverflow(ele, true);
						this.overflowableWH -= this.getWH(ele.DesiredSize, this.Orientation);
					}
					else
						SetIsOverflow(ele, false);
				}
			}

			this.SetValue(OverflowItemCountProperty, overflowCount);

			return this.Orientation == Orientation.Horizontal ?
							new Size(this.overflowableWH + this.nonoverflowableWH, maxHW) :
							new Size(maxHW, this.overflowableWH + this.nonoverflowableWH);
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			var items = InternalChildren.Cast<UIElement>();

			if (this.Orientation == Orientation.Horizontal)
			{
				double curX = 0;

				foreach (var item in items)
				{
					if (!GetCanOverflow(item) || !GetIsOverflow(item)) // Not overflowable or not set overflow
					{
						item.Arrange(new Rect(curX, 0, item.DesiredSize.Width, arrangeSize.Height));
						curX += item.DesiredSize.Width;
					}
					else
						item.Arrange(new Rect(0, 0, 0, 0));
				}

				return arrangeSize;
			}
			else
				return base.ArrangeOverride(arrangeSize);
		}

		private double getWH(Size size, Orientation orientation)
		{
			return orientation == Orientation.Horizontal ? size.Width : size.Height;
		}

		private double getHW(Size size, Orientation orientation)
		{
			return orientation == Orientation.Vertical ? size.Width : size.Height;
		}
		#endregion methods
	}
}
