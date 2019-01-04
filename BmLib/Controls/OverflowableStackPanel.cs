namespace BmLib.Controls
{
	using System;
    using System.Diagnostics;
    using System.Linq;
	using System.Windows;
	using System.Windows.Controls;

    /// <summary>
    /// Implements a StackPanel that can re-arrange its childrens depending
    /// on whether enough space is available or not.
    /// </summary>
	public class OverflowableStackPanel : StackPanel
	{
        #region fields
        /// <summary>
        /// Implements the backing store of the <see cref="OverflowItemCount"/> dependency property.
        /// </summary>
        public static DependencyProperty OverflowItemCountProperty =
            DependencyProperty.Register("OverflowItemCount", typeof(int),
				typeof(OverflowableStackPanel), new PropertyMetadata(0));

        /// <summary>
        /// Implements the backing store of the attached
        /// <see cref="SetCanOverflow(DependencyObject, bool)"/>
        /// <see cref="GetCanOverflow(DependencyObject)"/> dependency property.
        /// </summary>
		public static DependencyProperty CanOverflowProperty =
            DependencyProperty.RegisterAttached("CanOverflow", typeof(bool),
                typeof(OverflowableStackPanel), new UIPropertyMetadata(false));

        /// <summary>
        /// Implements the backing store of the attached
        /// <see cref="SetIsOverflow(DependencyObject, bool)"/>
        /// <see cref="GetIsOverflow(DependencyObject)"/>
        /// dependency property.
        /// </summary>
		public static DependencyProperty IsOverflowProperty =
            DependencyProperty.RegisterAttached("IsOverflow", typeof(bool),
                typeof(OverflowableStackPanel), new UIPropertyMetadata(false));

		private double overflowableWH = 0;
		private double nonoverflowableWH = 0;
        #endregion fields

        #region Constructor
        /// <summary>
        /// Class consructor
        /// </summary>
        public OverflowableStackPanel()
        {
        }
		#endregion

		#region properties
        /// <summary>
        /// Gets sets the number of items that are overflown in this <see cref="StackPanel"/>.
        /// </summary>
		public int OverflowItemCount
		{
			get { return (int)GetValue(OverflowItemCountProperty); }
			set { this.SetValue(OverflowItemCountProperty, value); }
		}
        #endregion properties

        #region methods
        #region CanOverflow
        /// <summary>
        /// Sets the attached CanOverflow dependency property to configure whether
        /// this <see cref="StackPanel"/> can overflow or not.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetCanOverflow(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanOverflowProperty);
        }

        /// <summary>
        /// gets the attached CanOverflow dependency property to determine whether
        /// this <see cref="StackPanel"/> is configured to be overflowable or not.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetCanOverflow(DependencyObject obj, bool value)
        {
            obj.SetValue(CanOverflowProperty, value);
        }
        #endregion CanOverflow

        #region IsOverflow
        /// <summary>
        /// Sets the attached IsOverflow dependency property to determine whether
        /// this <see cref="StackPanel"/> is overflown or not.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetIsOverflow(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsOverflowProperty);
        }

        /// <summary>
        /// Gets the attached IsOverflow dependency property to determine whether
        /// this <see cref="StackPanel"/> is overflown or not.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetIsOverflow(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOverflowProperty, value);
        }
        #endregion IsOverflow

        /// <summary>
        /// Measures the child elements of a <seealso cref="StackPanel"/> 
        /// in anticipation of arranging them during the
        /// <seealso cref="StackPanel.ArrangeOverride(System.Windows.Size)"/>
        /// </summary>
        /// <param name="constraint">An upper limit <seealso cref="Size"/> that should not be exceeded.</param>
        /// <returns>The System.Windows.Size that represents the desired size of the element.</returns>
        protected override Size MeasureOverride(Size constraint)
		{
#if DEBUG
            if (double.IsPositiveInfinity(constraint.Width)) // || double.IsPositiveInfinity(constraint.Height))
            {
                // This constrain hints a layout proplem that can cause items to NOT Overflow.
                Debug.WriteLine("---> Warning: OverflowableStackPanel.MeasureOverride(Size constraint) with constraint == Infinity");
            }
#endif
            var items = InternalChildren.Cast<UIElement>();

			this.overflowableWH = 0;
			this.nonoverflowableWH = 0;
			int overflowCount = 0;
			double maxHW = 0;

            // list of each breadcrumb item and ask each item in turn what their preferred size is
			foreach (var item in items)
			{
				item.Measure(constraint);
				maxHW = Math.Max(this.getHW(item.DesiredSize, this.Orientation), maxHW);

                if (GetCanOverflow(item) == true)
					this.overflowableWH += this.getWH(item.DesiredSize, this.Orientation);
				else
					this.nonoverflowableWH += this.getWH(item.DesiredSize, this.Orientation);
			}

			foreach (var ele in items.Reverse())
			{
				if (GetCanOverflow(ele) == true)
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

            if (this.Orientation == Orientation.Horizontal)
                return new Size(this.overflowableWH + this.nonoverflowableWH, maxHW);
            else
                return new Size(maxHW, this.overflowableWH + this.nonoverflowableWH);
		}

        /// <summary>
        /// Arranges the content of a System.Windows.Controls.StackPanel element.
        /// </summary>
        /// <param name="arrangeSize">The <see cref="System.Windows.Size"/> that this element should
        /// use to arrange its child elements.</param>
        /// <returns>
        /// The System.Windows.Size that represents the arranged size of this System.Windows.Controls.StackPanel
        /// element and its child elements.
        /// </returns>
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
