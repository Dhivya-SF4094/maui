#nullable disable
using Android.Graphics.Drawables;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.AppBar;
using Google.Android.Material.Shape;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Graphics;
using AToolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Microsoft.Maui.Controls.Platform.Compatibility
{

	public class ShellToolbarAppearanceTracker : IShellToolbarAppearanceTracker
	{
		bool _disposed;
		IShellContext _shellContext;

		public ShellToolbarAppearanceTracker(IShellContext shellContext)
		{
			_shellContext = shellContext;
		}

		public virtual void SetAppearance(AToolbar toolbar, IShellToolbarTracker toolbarTracker, ShellAppearance appearance)
		{
			var foreground = appearance.ForegroundColor;
			var background = appearance.BackgroundColor;
			var titleColor = appearance.TitleColor;

			SetColors(toolbar, toolbarTracker, foreground, background, titleColor);
		}

		public virtual void ResetAppearance(AToolbar toolbar, IShellToolbarTracker toolbarTracker)
		{
			SetColors(toolbar, toolbarTracker, ShellRenderer.DefaultForegroundColor, ShellRenderer.DefaultBackgroundColor, ShellRenderer.DefaultTitleColor);
		}

		protected virtual void SetColors(AToolbar toolbar, IShellToolbarTracker toolbarTracker, Color foreground, Color background, Color title)
		{
			if (_disposed)
				return;

			Toolbar shellToolbar = _shellContext?.Shell?.Toolbar;

			if (shellToolbar is null)
				return;

			shellToolbar.BarTextColor = title ?? ShellRenderer.DefaultTitleColor;
			shellToolbar.BarBackground = new SolidColorBrush(background ?? ShellRenderer.DefaultBackgroundColor);
			shellToolbar.IconColor = foreground ?? ShellRenderer.DefaultForegroundColor;

			// Directly apply background to the native toolbar to ensure it's applied
			// even when the property change propagation chain doesn't trigger the handler mapper.
			// This can happen when Shell.BackgroundColor is set on the Shell element itself
			// rather than on individual pages.
			if (RuntimeFeature.IsMaterial3Enabled &&
				toolbar.Parent?.GetParentOfType<AppBarLayout>()?.Background is MaterialShapeDrawable)
			{
				toolbar.UpdateBarBackground(shellToolbar);
			}
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

			if (disposing)
			{
				_shellContext = null;
			}
		}

		#endregion IDisposable
	}
}