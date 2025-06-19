using System;
using CoreGraphics;
using CoreText;
using Foundation;
using UIKit;

namespace Microsoft.Maui.Graphics.Platform
{
	public partial class PlatformStringSizeService : IStringSizeService
	{
		public SizeF GetStringSize(string value, IFont font, float fontSize)
		{
			if (string.IsNullOrEmpty(value))
			{
				return new SizeF();
			}

			using var attributedString = new NSAttributedString(
			 value,
			 new CTStringAttributes
			 {
				 Font = font?.ToCTFont(fontSize) ?? FontExtensions.GetDefaultCTFont(fontSize)
			 });

			using var framesetter = new CTFramesetter(attributedString);

			NSRange fitRange;
			var measuredSize = framesetter.SuggestFrameSize(
			 new NSRange(0, 0),
			 null,
			 new CGSize(float.MaxValue, float.MaxValue),
			 out fitRange);

			var path = new CGPath();
			path.AddRect(new RectF(0, 0, (float)measuredSize.Width, (float)measuredSize.Height));
			path.CloseSubpath();

			var suggestedSize = GetTextSize(framesetter, path);
			path.Dispose();

			var width = (float)Math.Ceiling(suggestedSize.Width);
			var height = (float)Math.Ceiling(suggestedSize.Height);

			return new SizeF(width, height);
		}
	}
}
