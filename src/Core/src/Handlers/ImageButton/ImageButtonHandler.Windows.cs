using Microsoft.Extensions.Logging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using WImage = Microsoft.UI.Xaml.Controls.Image;

namespace Microsoft.Maui.Handlers
{
	public partial class ImageButtonHandler : ViewHandler<IImageButton, Button>
	{
		Image? _image;

		PointerEventHandler? _pointerPressedHandler;
		PointerEventHandler? _pointerReleasedHandler;
		bool _isPressed;

		protected override Button CreatePlatformView()
		{
			_image = new Image
			{
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Stretch = Stretch.Uniform,
			};

			var platformImageButton = new Button
			{
				VerticalAlignment = VerticalAlignment.Stretch,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Content = _image
			};

			return platformImageButton;
		}

		protected override void ConnectHandler(Button platformView)
		{
			_pointerPressedHandler = new PointerEventHandler(OnPointerPressed);
			_pointerReleasedHandler = new PointerEventHandler(OnPointerReleased);

			if (_image != null)
			{
				_image.ImageOpened += OnImageOpened;
				_image.ImageFailed += OnImageFailed;
			}

			platformView.Click += OnClick;
			platformView.Unloaded += OnUnloaded;
			platformView.AddHandler(UIElement.PointerPressedEvent, _pointerPressedHandler, true);
			platformView.AddHandler(UIElement.PointerReleasedEvent, _pointerReleasedHandler, true);

			base.ConnectHandler(platformView);
		}

		protected override void DisconnectHandler(Button platformView)
		{
			if (_image != null)
			{
				_image.ImageOpened -= OnImageOpened;
				_image.ImageFailed -= OnImageFailed;
			}

			platformView.Click -= OnClick;
			platformView.Unloaded -= OnUnloaded;
			platformView.RemoveHandler(UIElement.PointerPressedEvent, _pointerPressedHandler);
			platformView.RemoveHandler(UIElement.PointerReleasedEvent, _pointerReleasedHandler);

			_pointerPressedHandler = null;
			_pointerReleasedHandler = null;

			base.DisconnectHandler(platformView);

			SourceLoader.Reset();
		}

		public static void MapStrokeColor(IImageButtonHandler handler, IButtonStroke buttonStroke)
		{
			(handler.PlatformView as Button)?.UpdateStrokeColor(buttonStroke);
		}

		public static void MapStrokeThickness(IImageButtonHandler handler, IButtonStroke buttonStroke)
		{
			(handler.PlatformView as Button)?.UpdateStrokeThickness(buttonStroke);
			handler.UpdateValue(nameof(IImageButton.Padding));
		}

		public static void MapCornerRadius(IImageButtonHandler handler, IButtonStroke buttonStroke)
		{
			(handler.PlatformView as Button)?.UpdateCornerRadius(buttonStroke);
			handler.UpdateValue(nameof(IImageButton.Padding));
		}

		public static void MapBackground(IImageButtonHandler handler, IImageButton imageButton)
		{
			(handler.PlatformView as Button)?.UpdateBackground(imageButton);
		}

		public static void MapPadding(IImageButtonHandler handler, IImageButton imageButton)
		{
			(handler.PlatformView as Button)?.UpdatePadding(imageButton);
		}

		// Windows-specific override: after applying Stretch via UpdateAspect, also reconcile
		// the MaxHeight/Width constraints that UpdateImageSource set when the source was loaded.
		public static void MapAspect(IImageButtonHandler handler, IImageButton imageButton)
		{
			// Apply Stretch and alignment via the shared ImageHandler path.
			ImageHandler.MapAspect((IImageHandler)handler, imageButton);

			if (handler.PlatformView.GetContent<WImage>() is not WImage nativeImage)
				return;

			// When aspect changes at runtime (image already loaded), the MaxHeight that
			// UpdateImageSource set may no longer be appropriate. Reconcile it here.
			var aspect = imageButton.Aspect;
			if (aspect == Aspect.AspectFill || aspect == Aspect.Fill)
			{
				nativeImage.MaxHeight = double.PositiveInfinity;
				nativeImage.MaxWidth = double.PositiveInfinity;
			}

			// For FontImageSource (CanvasImageSource), Width/Height may have been pinned.
			// Reconcile based on the new aspect.
			if (nativeImage.Source is CanvasImageSource canvas && aspect != Aspect.Center)
			{
				var size = canvas.GetImageSourceSize(handler.PlatformView);
				nativeImage.Width = double.NaN;
				nativeImage.Height = double.NaN;
				nativeImage.MaxWidth = size.Width;
				nativeImage.MaxHeight = size.Height;
			}
		}

		void OnClick(object sender, RoutedEventArgs e)
		{
			VirtualView?.Clicked();
		}

		void OnPointerPressed(object sender, PointerRoutedEventArgs e)
		{
			_isPressed = true;
			VirtualView?.Pressed();
		}

		void OnPointerReleased(object sender, PointerRoutedEventArgs e)
		{
			_isPressed = false;
			VirtualView?.Released();
		}

		void OnUnloaded(object sender, RoutedEventArgs e)
		{
			// WinUI will not raise the PointerReleased event if the pointer is pressed and then unloaded
			if (_isPressed)
			{
				VirtualView?.Released();
			}
		}

		void OnImageOpened(object sender, RoutedEventArgs routedEventArgs)
		{
			VirtualView?.UpdateIsLoading(false);
		}

		protected virtual void OnImageFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArgs)
		{
			MauiContext?.CreateLogger<ImageButtonHandler>()?.LogWarning("Image failed to load: {exceptionRoutedEventArgs.ErrorMessage}", exceptionRoutedEventArgs.ErrorMessage);
			VirtualView?.UpdateIsLoading(false);
		}

		partial class ImageButtonImageSourcePartSetter
		{
			public override void SetImageSource(ImageSource? platformImage)
			{
				if (Handler?.PlatformView is not Button button)
					return;

				button.UpdateImageSource(platformImage);

				// UpdateImageSource sets MaxHeight/Width constraints based on the image's
				// natural size, ignoring the current Aspect setting. We post-process here
				// to override those constraints when the Aspect requires it.
				var aspect = (Handler as IImageButtonHandler)?.VirtualView?.Aspect ?? Aspect.AspectFit;
				if (button.GetContent<WImage>() is not WImage nativeImage)
					return;

				if (platformImage is CanvasImageSource canvas)
				{
					// UpdateImageSource pins Width/Height to the glyph's natural size for
					// FontImageSource (CanvasImageSource), which prevents Stretch from working.
					// For aspects other than Center, clear the fixed dimensions and use
					// MaxWidth/MaxHeight so that Stretch can scale the glyph correctly.
					if (aspect != Aspect.Center)
					{
						var size = canvas.GetImageSourceSize(button);
						nativeImage.Width = double.NaN;
						nativeImage.Height = double.NaN;
						nativeImage.MaxWidth = size.Width;
						nativeImage.MaxHeight = size.Height;
					}
				}
				else if (platformImage is BitmapImage bitmapImage &&
					(aspect == Aspect.AspectFill || aspect == Aspect.Fill))
				{
					// UpdateImageSource caps MaxHeight to the image's natural height to prevent
					// upscaling. For AspectFill and Fill, the image must be allowed to expand
					// beyond its natural size to fill the container. Clear MaxHeight after the
					// ImageOpened event fires (which is where UpdateImageSource sets MaxHeight).
					void OnImageOpened(object sender, RoutedEventArgs e)
					{
						bitmapImage.ImageOpened -= OnImageOpened;
						nativeImage.MaxHeight = double.PositiveInfinity;
						nativeImage.MaxWidth = double.PositiveInfinity;
					}
					bitmapImage.ImageOpened += OnImageOpened;
				}
			}
		}
	}
}