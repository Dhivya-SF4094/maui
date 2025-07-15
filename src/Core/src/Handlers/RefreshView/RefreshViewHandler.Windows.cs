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
			
			// If the control is already loaded (e.g., during reconnection after navigation),
			// we need to trigger the loaded logic immediately
			if (nativeView.IsLoaded && !_isLoaded)
			{
				// Use the dispatcher to ensure the control is fully ready
				MauiContext?.Services
					.GetRequiredService<IDispatcher>()
					.Dispatch(() =>
					{
						if (!_isLoaded) // Double-check in case OnLoaded was called in the meantime
						{
							_isLoaded = true;
							
							// Apply any pending refresh request
							if (_pendingRefreshRequest)
							{
								_pendingRefreshRequest = false;
								if (VirtualView?.IsRefreshing == true && _refreshCompletionDeferral == null)
									PlatformView?.RequestRefresh();
							}
							else
							{
								UpdateIsRefreshing();
							}
						}
					});
			}
		}

		protected override void DisconnectHandler(RefreshContainer nativeView)
		{
			nativeView.Loaded -= OnLoaded;
			nativeView.RefreshRequested -= OnRefresh;

			CompleteRefresh();
			_pendingRefreshRequest = false;
			_isLoaded = false; // Reset loaded state to ensure proper loading cycle on reconnection

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

			if (!VirtualView?.IsRefreshing ?? false)
				CompleteRefresh();
			else
			{
				// Always request refresh when IsRefreshing is true and we don't have an active deferral
				// This ensures the visual indicator appears correctly, especially after navigation scenarios
				if (_refreshCompletionDeferral == null)
				{
					PlatformView?.RequestRefresh();
				}
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
						if (VirtualView?.IsRefreshing == true && _refreshCompletionDeferral == null)
							PlatformView?.RequestRefresh();
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
