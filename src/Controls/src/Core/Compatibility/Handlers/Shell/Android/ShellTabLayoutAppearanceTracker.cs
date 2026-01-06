#nullable disable
using Android.Graphics.Drawables;
using Google.Android.Material.Tabs;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls.Platform.Compatibility
{
	public class ShellTabLayoutAppearanceTracker : IShellTabLayoutAppearanceTracker
	{
		bool _disposed;
		IShellContext _shellContext;

		public ShellTabLayoutAppearanceTracker(IShellContext shellContext)
		{
			_shellContext = shellContext;
		}

		public virtual void ResetAppearance(TabLayout tabLayout)
		{
			SetColors(tabLayout, ShellRenderer.GetDefaultForegroundColor(_shellContext.AndroidContext),
				ShellRenderer.GetDefaultBackgroundColor(_shellContext.AndroidContext),
				ShellRenderer.GetDefaultTitleColor(_shellContext.AndroidContext),
				ShellRenderer.GetDefaultUnselectedColor(_shellContext.AndroidContext));
		}

		public virtual void SetAppearance(TabLayout tabLayout, ShellAppearance appearance)
		{
			var foreground = appearance.ForegroundColor;
			var background = appearance.BackgroundColor;
			var titleColor = appearance.TitleColor;
			var unselectedColor = appearance.UnselectedColor;

			SetColors(tabLayout, foreground, background, titleColor, unselectedColor);
		}

		protected virtual void SetColors(TabLayout tabLayout, Color foreground, Color background, Color title, Color unselected)
		{
			var titleArgb = title.ToPlatform(ShellRenderer.GetDefaultTitleColor(_shellContext.AndroidContext)).ToArgb();
			var unselectedArgb = unselected.ToPlatform(ShellRenderer.GetDefaultUnselectedColor(_shellContext.AndroidContext)).ToArgb();

			tabLayout.SetTabTextColors(unselectedArgb, titleArgb);
			tabLayout.SetBackground(new ColorDrawable(background.ToPlatform(ShellRenderer.GetDefaultBackgroundColor(_shellContext.AndroidContext))));
			tabLayout.SetSelectedTabIndicatorColor(foreground.ToPlatform(ShellRenderer.GetDefaultForegroundColor(_shellContext.AndroidContext)));
		}

		#region IDisposable

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;
			_shellContext = null;
		}

		#endregion IDisposable
	}
}