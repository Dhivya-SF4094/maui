#nullable enable
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using WImage = Microsoft.UI.Xaml.Controls.Image;

namespace Microsoft.Maui.Platform
{
	public static class ImageViewExtensions
	{
		public static void Clear(this WImage imageView)
		{
			imageView.Source = null;
		}

		public static void UpdateAspect(this WImage imageView, IImage image)
		{
			imageView.Stretch = image.Aspect.ToStretch();

			if (image.Aspect == Aspect.AspectFill)
			{
				// For AspectFill (UniformToFill), the Image element must stretch to fill its
				// container so the bitmap can scale up to cover the full area and be clipped.
				imageView.VerticalAlignment = VerticalAlignment.Stretch;
				imageView.HorizontalAlignment = HorizontalAlignment.Stretch;
			}
			else
			{
				// For all other aspects, center the Image element within its container.
				imageView.VerticalAlignment = VerticalAlignment.Center;
				imageView.HorizontalAlignment = HorizontalAlignment.Center;
			}
		}

		public static void UpdateIsAnimationPlaying(this WImage imageView, IImageSourcePart image)
		{
			if (imageView.Source is BitmapImage bitmapImage && bitmapImage.IsAnimatedBitmap)
			{
				if (image.IsAnimationPlaying)
				{
					if (!bitmapImage.IsPlaying)
						bitmapImage.Play();
				}
				else
				{
					if (bitmapImage.IsPlaying)
						bitmapImage.Stop();
				}
			}
		}
	}
}