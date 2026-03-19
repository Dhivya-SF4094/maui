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
	}
}