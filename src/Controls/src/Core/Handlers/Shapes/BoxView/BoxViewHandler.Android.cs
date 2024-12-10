using Microsoft.Maui.Controls.Platform;

namespace Microsoft.Maui.Controls.Handlers
{
	public partial class BoxViewHandler : ShapeViewHandler
	{
		public static void MapColor(IShapeViewHandler handler, BoxView view)
		{
			if (view.Color is not null && view.BackgroundColor is not null)
			{
				handler.ToPlatform().UpdateBackground(view.Color);
				handler.PlatformView?.InvalidateShape(view);
			}
		}
	}
}