using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Graphics.Win2D;
using Microsoft.UI.Xaml.Media;

namespace Microsoft.Maui.Platform
{
	public static class GraphicsViewExtensions
	{
		public static void UpdateDrawable(this W2DGraphicsView PlatformGraphicsView, IGraphicsView graphicsView)
		{
			PlatformGraphicsView.Drawable = graphicsView.Drawable;
		}

		public static void UpdateFlowDirection(this PlatformTouchGraphicsView platformView, IGraphicsView graphicsView)
		{
			// Apply visual mirroring for Win2D canvas content when RTL
			if (graphicsView.FlowDirection == FlowDirection.RightToLeft)
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