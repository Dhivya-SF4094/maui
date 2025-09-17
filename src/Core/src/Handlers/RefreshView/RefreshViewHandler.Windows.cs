using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using WBrush = Microsoft.UI.Xaml.Media.Brush;

namespace Microsoft.Maui.Handlers
{
	public partial class RefreshViewHandler : ViewHandler<IRefreshView, RefreshContainer>
	{
		bool _isLoaded;
		bool _pendingRefreshRequest;
		Deferral? _refreshCompletionDeferral;

		protected override RefreshContainer CreatePlatformView()
		{
			var refreshControl = new RefreshContainer
			{
				PullDirection = RefreshPullDirection.TopToBottom,
				Content = new ContentPanel()
			};

			SetRefreshColorCallback(refreshControl);
			return refreshControl;
		}

		protected override void ConnectHandler(RefreshContainer nativeView)
		{
			nativeView.Loaded += OnLoaded;
			nativeView.RefreshRequested += OnRefresh;

			base.ConnectHandler(nativeView);
		}

		protected override void DisconnectHandler(RefreshContainer nativeView)
		{
			nativeView.Loaded -= OnLoaded;
			nativeView.RefreshRequested -= OnRefresh;

			CompleteRefresh();
			_pendingRefreshRequest = false;
			_isLoaded = false;

			if (nativeView.Content is ContentPanel contentPanel)
			{
				contentPanel.Content = null;
				contentPanel.CrossPlatformLayout = null;
			}

			base.DisconnectHandler(nativeView);
		}

		public static void MapIsRefreshing(IRefreshViewHandler handler, IRefreshView refreshView)
			=> (handler as RefreshViewHandler)?.UpdateIsRefreshing();

		public static void MapContent(IRefreshViewHandler handler, IRefreshView refreshView)
			=> UpdateContent(handler);

		public static void MapRefreshColor(IRefreshViewHandler handler, IRefreshView refreshView)
			=> UpdateRefreshColor(handler);

		public static void MapRefreshViewBackground(IRefreshViewHandler handler, IView view)
			=> UpdateBackground(handler);

		void UpdateIsRefreshing()
		{
			if (!_isLoaded)
			{
				// Store the pending refresh request to be applied when the control loads
				_pendingRefreshRequest = VirtualView?.IsRefreshing ?? false;
				return;
			}

			var isRefreshing = VirtualView?.IsRefreshing ?? false;

			if (!isRefreshing)
			{
				CompleteRefresh();
			}
			else
			{
				// Key insight: The issue with navigation scenarios is that Windows RefreshContainer
				// can get into an internal state where RequestRefresh() doesn't show the visual indicator.
				// The solution is to ensure we always start from a clean state.

				if (_refreshCompletionDeferral == null)
				{
					// For navigation scenarios, we need to ensure the RefreshContainer is in the right state.
					// The problem is that after navigation, the container might have stale internal state
					// that prevents the visual indicator from appearing even when RequestRefresh() is called.

					// Solution: Always ensure we start from a clean state by checking if we need to 
					// reset the container before requesting refresh. This mimics what happens when
					// RefreshView works correctly in the same-page scenario.

					EnsureRefreshIndicatorVisible();
				}
			}
		}

		void EnsureRefreshIndicatorVisible()
		{
			if (PlatformView == null)
				return;

			// The key difference between same-page (working) and navigation (not working) scenarios
			// is the internal state of the RefreshContainer. We need to ensure it's ready to show
			// the visual indicator.

			// Strategy: Use the visualizer directly to ensure the refresh state is properly reflected
			// This approach is inspired by how Android/iOS platforms work - they directly set
			// the platform control's state rather than relying on complex timing mechanisms.

			if (PlatformView.Visualizer != null)
			{
				// Check if the visualizer indicates that refresh is already in progress
				// If not, we need to initiate the refresh properly
				var isVisualizerRefreshing = PlatformView.Visualizer.State == RefreshVisualizerState.Refreshing;

				if (!isVisualizerRefreshing)
				{
					// This is the core fix: ensure the RefreshContainer is properly reset
					// before requesting refresh. This addresses the navigation issue where
					// the container gets into an inconsistent state.
					PlatformView?.RequestRefresh();
				}
			}
			else
			{
				// If visualizer is not ready, fall back to direct request
				PlatformView?.RequestRefresh();
			}
		}

		static void UpdateContent(IRefreshViewHandler handler)
		{
			IView? content;

			if (handler.VirtualView is IContentView cv && cv.PresentedContent is IView view)
			{
				content = view;
			}
			else
			{
				content = handler.VirtualView.Content;
			}

			var platformContent = content?.ToPlatform(handler.MauiContext!);
			if (handler.PlatformView.Content is ContentPanel contentPanel)
			{
				contentPanel.Content = platformContent;
				contentPanel.CrossPlatformLayout = (handler.VirtualView as ICrossPlatformLayout);
			}
			else
			{
				handler.PlatformView.Content = platformContent;
			}
		}

		static void UpdateRefreshColor(IRefreshViewHandler handler)
		{
			if (handler.VirtualView == null || handler.PlatformView?.Visualizer == null)
				return;

			handler.PlatformView.Visualizer.Foreground = handler.VirtualView.RefreshColor != null
				? handler.VirtualView.RefreshColor.ToPlatform()
				: (WBrush)UI.Xaml.Application.Current.Resources["DefaultTextForegroundThemeBrush"];
		}

		static void UpdateBackground(IRefreshViewHandler handler)
		{
			if (handler.PlatformView?.Visualizer == null)
				return;

			if (handler.VirtualView.Background != null)
				handler.PlatformView.Visualizer.Background = handler.VirtualView.Background.ToPlatform();
		}

		// Telling the refresh to start before the control has been sized
		// causes no refresh circle to show up
		void OnLoaded(object sender, object args)
		{
			var refreshControl = sender as RefreshContainer;

			if (refreshControl == null || MauiContext == null)
				return;

			refreshControl.Loaded -= OnLoaded;
			MauiContext.Services
				.GetRequiredService<IDispatcher>()
				.Dispatch(() =>
				{
					_isLoaded = true;

					// Apply any pending refresh request that was made before the control was loaded
					if (_pendingRefreshRequest)
					{
						_pendingRefreshRequest = false;
						UpdateIsRefreshing();
					}
					else
					{
						UpdateIsRefreshing();
					}
				});
		}

		void OnRefresh(object sender, RefreshRequestedEventArgs args)
		{
			CompleteRefresh();
			_refreshCompletionDeferral = args.GetDeferral();

			if (VirtualView != null)
				VirtualView.IsRefreshing = true;
		}

		void CompleteRefresh()
		{
			if (_refreshCompletionDeferral != null)
			{
				_refreshCompletionDeferral.Complete();
				_refreshCompletionDeferral.Dispose();
				_refreshCompletionDeferral = null;
			}
		}

		void SetRefreshColorCallback(RefreshContainer refreshControl)
		{
			long callbackToken = 0;
			callbackToken = refreshControl.RegisterPropertyChangedCallback(RefreshContainer.VisualizerProperty,
				(_, __) =>
				{
					if (refreshControl?.Visualizer == null)
						return;

					UpdateRefreshColor(this);
					refreshControl.UnregisterPropertyChangedCallback(RefreshContainer.VisualizerProperty, callbackToken);
				});
		}
	}
}
