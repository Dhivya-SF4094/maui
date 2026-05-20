#nullable disable
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Platform;

namespace Microsoft.Maui.Controls
{
	public partial class Button
	{
		internal static void MapBackground(IButtonHandler handler, Button button)
		{
			// On Windows, if the resolved background is null it could mean a local null value is
			// overriding a MAUI style that set BackgroundColor. Clear the manual override so the
			// style value can surface and the correct background is applied via the platform.
			if (button.BackgroundColor is null)
			{
				button.ClearValue(BackgroundColorProperty);

				// After clearing, if a style value has surfaced the second MapBackground call
				// triggered by OnPropertyChanged has already updated the platform view, so we
				// can return early to avoid a redundant UpdateBackground call.
				if (button.BackgroundColor is not null)
				{
					return;
				}
			}

			ButtonHandler.MapBackground(handler, button);
		}

		public static void MapImageSource(ButtonHandler handler, Button button) =>
			MapImageSource((IButtonHandler)handler, button);

		public static void MapText(IButtonHandler handler, Button button)
		{
			var text = TextTransformUtilities.GetTransformedText(button.Text, button.TextTransform);
			handler.PlatformView?.UpdateText(text);
			button.Handler?.UpdateValue(nameof(Button.ContentLayout));
		}

		public static void MapLineBreakMode(IButtonHandler handler, Button button)
		{
			handler.PlatformView?.UpdateLineBreakMode(button);
		}

		public static void MapImageSource(IButtonHandler handler, Button button)
		{
			ButtonHandler.MapImageSource(handler, button);
			button.Handler?.UpdateValue(nameof(Button.ContentLayout));
		}

		public static void MapText(ButtonHandler handler, Button button) =>
			MapText((IButtonHandler)handler, button);
	}
}