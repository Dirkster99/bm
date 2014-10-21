namespace BreadcrumbLib.BaseControls
{
	using System.Windows;
	using System.Windows.Controls;

	public class OneItemPanel : Panel
	{
		protected override Size MeasureOverride(Size availableSize)
		{
			for (int i = 0; i < InternalChildren.Count; i++)
			{
				UIElement element = InternalChildren[i];
				if (element.Visibility == System.Windows.Visibility.Visible)
				{
					element.Measure(availableSize);
					return element.DesiredSize;
				}
			}
			return new Size(0, 0);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			UIElement visibleElement = null;
			for (int i = 0; i < InternalChildren.Count; i++)
			{
				UIElement element = InternalChildren[i];
				if (visibleElement == null && element.Visibility == System.Windows.Visibility.Visible)
				{
					visibleElement = element;
					visibleElement.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
					break;
				}
				else
				{
					element.Arrange(new Rect(0, 0, 0, 0));
				}
			}

			return finalSize;
		}
	}
}
