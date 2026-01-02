#nullable disable
using Android.Content;
using Android.Util;
using Android.Views;
using AndroidX.Core.Content;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Graphics;
using AColor = Android.Graphics.Color;
using AColorRes = Android.Resource.Color;
using AView = Android.Views.View;

namespace Microsoft.Maui.Controls.Platform.Compatibility
{
	internal class ShellPageContainer : ViewGroup
	{
		static int? DarkBackground;
		static int? LightBackground;
		static int? Material3DarkBackground;
		static int? Material3LightBackground;


		public IViewHandler Child { get; set; }

		public bool IsInFragment { get; set; }

		public ShellPageContainer(Context context, IPlatformViewHandler child, bool inFragment = false) : base(context)
		{
			Child = child;
			IsInFragment = inFragment;
			if (child.VirtualView.Background == null)
			{
				int color;
				if (RuntimeFeature.IsMaterial3Enabled)
				{
					// Use Material 3 colorSurface with separate caching for light and dark themes
					if (ShellRenderer.IsDarkTheme)
					{
						color = Material3DarkBackground ??= ResolveColorSurface(context);
					}
					else
					{
						color = Material3LightBackground ??= ResolveColorSurface(context);
					}
				}
				else
				{
					if (ShellRenderer.IsDarkTheme)
					{
						color = DarkBackground ??= ContextCompat.GetColor(context, AColorRes.BackgroundDark);
					}
					else
					{
						color = LightBackground ??= ContextCompat.GetColor(context, AColorRes.BackgroundLight);
					}
				}

				child.PlatformView.SetBackgroundColor(new AColor(color));
			}
			child.PlatformView.RemoveFromParent();
			AddView(child.PlatformView);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			var width = r - l;
			var height = b - t;

			if (Child.PlatformView is AView aView)
				aView.Layout(0, 0, width, height);
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			if (Child.PlatformView is AView aView)
			{
				aView.Measure(widthMeasureSpec, heightMeasureSpec);
				SetMeasuredDimension(aView.MeasuredWidth, aView.MeasuredHeight);
			}
			else
				SetMeasuredDimension(0, 0);
		}

		static int ResolveColorSurface(Context context)
		{
			var typedValue = new TypedValue();
			context.Theme.ResolveAttribute(Resource.Attribute.colorSurface, typedValue, true);
			return typedValue.Data;
		}
	}
}
