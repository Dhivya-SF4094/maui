using System;
using System.Threading.Tasks;
using Microsoft.Maui.DeviceTests.Stubs;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.DeviceTests
{
	public partial class ImageButtonHandlerTests
	{
		[Theory]
		[InlineData(50)]
		[InlineData(40)]
		public async Task OversizedCornerRadiusStillRendersBackground(int size)
		{
			var expected = Colors.Red;
			var imageButton = new ImageButtonStub
			{
				Background = new SolidPaintStub(expected),
				CornerRadius = 100,
				Width = size,
				Height = size,
				DesiredSize = new Microsoft.Maui.Graphics.Size(size, size)
			};

			await ValidateHasColor(imageButton, expected);
		}

		[Fact(DisplayName = "Clip ImageButton with Background works Correctly")]
		public async Task ClipImageButtonWithBackgroundWorks()
		{
			Color expected = Colors.Yellow;

			var brush = new SolidPaintStub(expected);

			var imageButton = new ImageButtonStub
			{
				Background = brush,
				Clip = new EllipseShapeStub()
			};

			await ValidateHasColor(imageButton, expected);
		}

		Google.Android.Material.ImageView.ShapeableImageView GetPlatformImageButton(ImageButtonHandler buttonHandler) =>
			buttonHandler.PlatformView;

		Task PerformClick(IImageButton button)
		{
			return InvokeOnMainThreadAsync(() =>
			{
				GetPlatformImageButton(CreateHandler(button)).PerformClick();
			});
		}

		bool ImageSourceLoaded(ImageButtonHandler imageButtonHandler) =>
			imageButtonHandler.PlatformView.Drawable != null;
	}
}