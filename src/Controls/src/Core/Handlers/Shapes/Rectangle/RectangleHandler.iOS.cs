#nullable disable
using Microsoft.Maui.Controls.Shapes;

namespace Microsoft.Maui.Controls.Handlers
{
	public partial class RectangleHandler
	{
		protected override void ConnectHandler(MauiShapeView platformView)
		{
			base.ConnectHandler(platformView);

			// Redraw the rectangle when bounds change to avoid blur.
			platformView.Layer.NeedsDisplayOnBoundsChange = true;
		}

		protected override void DisconnectHandler(MauiShapeView platformView)
		{
			base.DisconnectHandler(platformView);
		}

		public static void MapRadiusX(IShapeViewHandler handler, Rectangle rectangle)
		{
			handler.PlatformView?.InvalidateShape(rectangle);
		}

		public static void MapRadiusY(IShapeViewHandler handler, Rectangle rectangle)
		{
			handler.PlatformView?.InvalidateShape(rectangle);
		}
	}
}