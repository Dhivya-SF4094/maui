using Android.Graphics;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Platform
{
	public static class ActivityIndicatorExtensions
	{
		public static void UpdateIsRunning(this ProgressBar progressBar, IActivityIndicator activityIndicator)
		{
			progressBar.Visibility = GetActivityIndicatorVisibility(activityIndicator);
		}

		internal static void UpdateIsRunning(this MaterialActivityIndicator progressBar, IActivityIndicator activityIndicator)
		{
			progressBar.Visibility = activityIndicator.GetActivityIndicatorVisibility();
		}

		internal static ViewStates GetActivityIndicatorVisibility(this IActivityIndicator activityIndicator)
		{
			if (activityIndicator.Visibility == Visibility.Visible)
			{
				return activityIndicator.IsRunning ? ViewStates.Visible : ViewStates.Invisible;
			}
			else
			{
				return activityIndicator.Visibility.ToPlatformVisibility();
			}
		}

		public static void UpdateColor(this ProgressBar progressBar, IActivityIndicator activityIndicator)
		{
			var color = activityIndicator.Color;

			if (color != null)
				progressBar.IndeterminateDrawable?.SetColorFilter(color.ToPlatform(), FilterMode.SrcIn);
			else
				progressBar.IndeterminateDrawable?.ClearColorFilter();
		}

		internal static void UpdateColor(this MaterialActivityIndicator progressBar, IActivityIndicator activityIndicator)
		{
			var color = activityIndicator.Color;

			if (color != null)
			{
				// Material 3 uses SetIndicatorColor which takes an int array
				progressBar.SetIndicatorColor(new[] { color.ToPlatform().ToArgb() });
			}
			else
			{
				// Reset to default theme color
				var context = progressBar.Context;
				if (context != null)
				{
					var typedValue = new global::Android.Util.TypedValue();
					context.Theme?.ResolveAttribute(
						global::Android.Resource.Attribute.ColorPrimary,
						typedValue,
						true);

					if (typedValue.Data != 0)
					{
						progressBar.SetIndicatorColor(new[] { typedValue.Data });
					}
				}
			}
			//var color = activityIndicator.Color;
			//progressBar.IndeterminateDrawable?.SetColorFilter(color.ToPlatform(), FilterMode.SrcIn);
			// var color = activityIndicator.Color;

			// if (color != null)
			// 	progressBar.IndeterminateDrawable?.SetColorFilter(color.ToPlatform(), FilterMode.SrcIn);
			// else
			// 	progressBar.IndeterminateDrawable?.ClearColorFilter();
		}
	}
}