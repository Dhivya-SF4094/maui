using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Graphics.Win2D;

namespace Microsoft.Maui.Platform
{
	public static class ShapesExtensions
	{
		public static void UpdateShape(this W2DGraphicsView platformView, IShapeView shapeView)
		{
			platformView.Drawable = new ShapeDrawable(shapeView);
		}

		public static void InvalidateShape(this W2DGraphicsView platformView, IShapeView shapeView)
		{
			platformView.Invalidate();
		}

		internal static void UpdateFlowDirection(this W2DGraphicsView platformView, IShapeView shapeView)
		{
			// Apply visual mirroring for Win2D canvas content when RTL
			if (shapeView.FlowDirection == FlowDirection.RightToLeft)
			{
				platformView.RenderTransformOrigin = new global::Windows.Foundation.Point(0.5, 0.5);
				platformView.RenderTransform = new ScaleTransform { ScaleX = -1 };
			}
			else
			{
				platformView.ClearValue(global::Microsoft.UI.Xaml.UIElement.RenderTransformProperty);
			}
		}
	}
}
