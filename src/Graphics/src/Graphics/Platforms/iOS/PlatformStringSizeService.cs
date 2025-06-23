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

			var attributes = new CTStringAttributes();
			attributes.Font = font?.ToCTFont(fontSize) ?? FontExtensions.GetDefaultCTFont(fontSize);

			using var attributedString = new NSAttributedString(value, attributes);
			using var framesetter = new CTFramesetter(attributedString);

			// Get suggested frame size with unlimited constraints
			var measuredSize = framesetter.SuggestFrameSize(
				new NSRange(0, 0),
				null,
				new CGSize(float.MaxValue, float.MaxValue),
				out _);

			return new SizeF((float)measuredSize.Width, (float)measuredSize.Height);
		}
	}
}