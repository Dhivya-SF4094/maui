using System;
using System.Threading.Tasks;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.AppBar;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Platform;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	[Category(TestCategory.Shell)]
	public partial class ShellTests
	{
		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task FlyoutContentResizesWhenFooterGrowsAndShrinks(bool edgeToEdge)
		{
			const double warningHeight = 200;
			const double tolerance = 1;
			var endMarker = new Label { Text = "End of flyout content", HeightRequest = 48 };
			var content = new VerticalStackLayout { Spacing = 0 };
			for (int index = 0; index < 40; index++)
				content.Add(new Label { Text = $"Flyout item {index}", HeightRequest = 48 });
			content.Add(endMarker);

			var scrollView = new ScrollView { Content = content };
			var warning = new Label { Text = "Warning", HeightRequest = warningHeight, IsVisible = false };
			var footer = new VerticalStackLayout
			{
				Spacing = 0,
				Children = { warning, new Label { Text = "Footer", HeightRequest = 48 } }
			};
			var contentTemplate = new DataTemplate(() => scrollView);
			var footerTemplate = new DataTemplate(() => edgeToEdge ? new ContentView { Content = footer } : footer);

			await RunShellTest(shell =>
			{
				shell.FlyoutContentTemplate = contentTemplate;
				shell.FlyoutFooterTemplate = footerTemplate;
			}, async shell =>
			{
				var shellContext = (IShellContext)shell.Handler;
				await OpenFlyout(shellContext);
				await OnLoadedAsync(scrollView);
				await OnLoadedAsync(footer);
				await AssertionExtensions.AssertEventually(() =>
					scrollView.Height > 0 && footer.Height > 0 && endMarker.Height > 0
					&& scrollView.ContentSize.Height > scrollView.Height
					&& Math.Abs(scrollView.ContentSize.Height - content.Height) <= tolerance);

				var platformScrollView = Assert.IsType<MauiScrollView>(scrollView.ToPlatform());
				var initialViewport = GetFrameRelativeToFlyout(shellContext, scrollView);
				var initialFooter = GetFrameRelativeToFlyout(shellContext, footer);
				Assert.True(content.Height > initialViewport.Height, "Content must require scrolling.");
				await AssertBottomReachable();

				for (int cycle = 0; cycle < 3; cycle++)
				{
					warning.IsVisible = true;
					await AssertViewport(warningHeight);
					await AssertBottomReachable();

					warning.IsVisible = false;
					await AssertViewport(0);
					await AssertBottomReachable();
				}

				Assert.Same(contentTemplate, shell.FlyoutContentTemplate);
				Assert.Same(footerTemplate, shell.FlyoutFooterTemplate);
				Assert.Same(platformScrollView, scrollView.ToPlatform());

				async Task AssertViewport(double heightChange)
				{
					await AssertionExtensions.AssertEventually(() =>
					{
						var viewport = GetFrameRelativeToFlyout(shellContext, scrollView);
						var footerFrame = GetFrameRelativeToFlyout(shellContext, footer);
						return Math.Abs(footerFrame.Height - initialFooter.Height - heightChange) <= tolerance
							&& Math.Abs(viewport.Height - initialViewport.Height + heightChange) <= tolerance
							&& Math.Abs(viewport.Bottom - footerFrame.Top) <= tolerance
							&& Math.Abs(viewport.Top - initialViewport.Top) <= tolerance
							&& Math.Abs(footerFrame.Bottom - initialFooter.Bottom) <= tolerance;
					}, message: $"Flyout viewport did not follow the footer height change of {heightChange}.");
				}

				async Task AssertBottomReachable()
				{
					await scrollView.ScrollToAsync(0, 0, false);
					await AssertionExtensions.AssertEventually(() => platformScrollView.ScrollY == 0);
					// Clamp to the native scroll limit rather than relying on element-relative ScrollTo positioning.
					await scrollView.ScrollToAsync(0, scrollView.ContentSize.Height, false);
					await AssertionExtensions.AssertEventually(() =>
						platformScrollView.ScrollY > 0 && !platformScrollView.CanScrollVertically(1),
						message: "Flyout content did not reach the native bottom scroll limit.");

					var viewport = GetFrameRelativeToFlyout(shellContext, scrollView);
					var marker = GetFrameRelativeToFlyout(shellContext, endMarker);
					var footerFrame = GetFrameRelativeToFlyout(shellContext, footer);
					Assert.True(marker.Top >= viewport.Top - tolerance,
						$"The last item is clipped at the top: marker={marker}, viewport={viewport}.");
					Assert.True(Math.Abs(marker.Bottom - viewport.Bottom) <= tolerance,
						$"The last item must align with the viewport bottom without a gap: marker={marker}, viewport={viewport}, nativePaddingBottom={platformScrollView.PaddingBottom}.");
					Assert.True(marker.Bottom <= footerFrame.Top + tolerance,
						$"The footer obscures the last item: marker={marker}, footer={footerFrame}.");
				}
			});
		}

		[Fact(DisplayName = "FlyoutIsPresented=true sets the visible status of the Shell Flyout.")]
		public async Task FlyoutIsPresentedOpenDrawer()
		{
			await RunShellTest(shell =>
			{
				shell.FlyoutContent = new VerticalStackLayout() { new Label() { Text = "Flyout Content" } };
			},
			async shell =>
			{
#if ANDROID || IOS || MACCATALYST
				var shellContext = (IShellContext)shell.Handler;
#endif
				// 1. Set FlyoutIsPresented=true to make the Shell Flyout visible.
				shell.FlyoutIsPresented = true;

				var dl = GetDrawerLayout(shellContext) as DrawerLayout;
				Assert.NotNull(dl);

				await AssertionExtensions.AssertEventually(() =>
				{
					// 2. Check that the Flyout has size.
					var flyoutFrame = GetFlyoutFrame(shellContext);
					return flyoutFrame.Width > 0 && flyoutFrame.Height > 0 && dl.IsOpen;
				});
			});
		}
	}

	[Category(TestCategory.Shell)]
	[Collection(ControlsHandlerTestBase.RunInNewWindowCollection)]
	public class ShellFlyoutSafeAreaTests : ControlsHandlerTestBase
	{
		void SetupBuilder()
		{
			EnsureHandlerCreated(builder => builder.ConfigureMauiHandlers(handlers =>
			{
				SetupShellHandlers(handlers);
				handlers.AddHandler<ContentView, ContentViewHandler>();
			}));
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(true, false)]
		[InlineData(true, true)]
		public async Task FlyoutHeaderAndFooterCanExtendIntoSystemBars(bool explicitEdges, bool softInput)
		{
			SetupBuilder();
			var headerLabel = new Label { Text = "Header", VerticalOptions = LayoutOptions.Start };
			var footerLabel = new Label { Text = "Footer", VerticalOptions = LayoutOptions.End };
			var header = new ContentView
			{
				HeightRequest = 120,
				BackgroundColor = Colors.Yellow,
				Content = new Grid { SafeAreaEdges = SafeAreaEdges.Container, Children = { headerLabel } }
			};
			var footer = new ContentView
			{
				HeightRequest = 120,
				BackgroundColor = Colors.Yellow,
				Content = new Grid { SafeAreaEdges = SafeAreaEdges.Container, Children = { footerLabel } }
			};
			if (explicitEdges)
			{
				header.SafeAreaEdges = softInput ? new SafeAreaEdges(SafeAreaRegions.SoftInput) : SafeAreaEdges.None;
				footer.SafeAreaEdges = softInput ? new SafeAreaEdges(SafeAreaRegions.SoftInput) : SafeAreaEdges.None;
			}
			var shell = await InvokeOnMainThreadAsync(() => new Shell
			{
				FlyoutHeader = header,
				FlyoutFooter = footer,
				FlyoutHeaderBehavior = FlyoutHeaderBehavior.Fixed,
				Items = { new FlyoutItem { Items = { new ContentPage() } } }
			});

			await CreateHandlerAndAddToWindow(shell, async () =>
			{
				await OnNavigatedToAsync(shell.CurrentPage);
				var drawer = ((IShellContext)shell.Handler).CurrentDrawerLayout;
				shell.FlyoutIsPresented = true;
				await AssertionExtensions.AssertEventually(() => drawer.IsOpen);
				await OnLoadedAsync(headerLabel);
				await OnLoadedAsync(footerLabel);
				var flyout = drawer.GetChildAt(1);
				var insets = ViewCompat.GetRootWindowInsets(flyout);
				Assert.NotNull(insets);
				var bars = insets.GetInsets(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.DisplayCutout());
				Assert.True(bars.Top > 0 && bars.Bottom > 0, "This test requires visible top and bottom system insets.");

				var rootLocation = new int[2];
				var headerLocation = new int[2];
				var footerLocation = new int[2];
				var headerLabelLocation = new int[2];
				var footerLabelLocation = new int[2];

				await AssertionExtensions.AssertEventually(() =>
				{
					flyout.GetLocationOnScreen(rootLocation);
					header.ToPlatform().GetLocationOnScreen(headerLocation);
					footer.ToPlatform().GetLocationOnScreen(footerLocation);
					headerLabel.ToPlatform().GetLocationOnScreen(headerLabelLocation);
					footerLabel.ToPlatform().GetLocationOnScreen(footerLabelLocation);

					return Math.Abs(headerLocation[1] - rootLocation[1]) <= 1
						&& Math.Abs(footerLocation[1] + footer.ToPlatform().Height - rootLocation[1] - flyout.Height) <= 1
						&& headerLabelLocation[1] >= rootLocation[1] + bars.Top
						&& footerLabelLocation[1] + footerLabel.ToPlatform().Height <= rootLocation[1] + flyout.Height - bars.Bottom;
				}, message: "Header and footer backgrounds must reach the flyout edges while their content avoids system bars.");

				var metrics = new global::Android.Util.DisplayMetrics();
#pragma warning disable CA1422 // GetRealMetrics includes system bars on all supported Android versions.
				flyout.Display.GetRealMetrics(metrics);
#pragma warning restore CA1422
				Assert.True(rootLocation[1] < bars.Top, "The flyout does not overlap the top system bar.");
				Assert.True(rootLocation[1] + flyout.Height > metrics.HeightPixels - bars.Bottom,
					"The flyout does not overlap the bottom system bar.");

				header.SafeAreaEdges = SafeAreaEdges.Container;
				footer.SafeAreaEdges = SafeAreaEdges.Container;
				await AssertionExtensions.AssertEventually(() =>
					flyout.PaddingTop >= bars.Top && flyout.PaddingBottom >= bars.Bottom,
					message: "Changing SafeAreaEdges must keep the header and footer within the safe area.");
				header.SafeAreaEdges = SafeAreaEdges.None;
				footer.SafeAreaEdges = SafeAreaEdges.None;
				await AssertionExtensions.AssertEventually(() =>
				{
					headerLabel.ToPlatform().GetLocationOnScreen(headerLabelLocation);
					footerLabel.ToPlatform().GetLocationOnScreen(footerLabelLocation);
					return flyout.PaddingTop == 0 && flyout.PaddingBottom == 0
						&& headerLabelLocation[1] >= rootLocation[1] + bars.Top
						&& footerLabelLocation[1] + footerLabel.ToPlatform().Height <= rootLocation[1] + flyout.Height - bars.Bottom;
				}, message: "Changing SafeAreaEdges back to None must restore edge-to-edge backgrounds.");

				var syntheticInsets = new WindowInsetsCompat.Builder(insets)
					.SetInsets(WindowInsetsCompat.Type.DisplayCutout(), AndroidX.Core.Graphics.Insets.Of(37, 0, 29, 0))
					.Build();
				ViewCompat.DispatchApplyWindowInsets(flyout, syntheticInsets);
				Assert.Equal(Math.Max(37, bars.Left), flyout.PaddingLeft);
				Assert.Equal(Math.Max(29, bars.Right), flyout.PaddingRight);
				Assert.Equal(0, header.Content.ToPlatform().PaddingLeft);
				Assert.Equal(0, footer.Content.ToPlatform().PaddingRight);
			});
		}

		[Fact]
		public async Task ClearingSafeAreaEdgesRestoresDefaultLayoutInsets()
		{
			SetupBuilder();
			var header = new Grid { HeightRequest = 120, SafeAreaEdges = SafeAreaEdges.Container };
			var footer = new Grid { HeightRequest = 120, SafeAreaEdges = SafeAreaEdges.Container };
			var shell = await InvokeOnMainThreadAsync(() => new Shell
			{
				FlyoutHeader = header,
				FlyoutFooter = footer,
				FlyoutHeaderBehavior = FlyoutHeaderBehavior.Fixed,
				Items = { new FlyoutItem { Items = { new ContentPage() } } }
			});

			await CreateHandlerAndAddToWindow(shell, async () =>
			{
				await OnNavigatedToAsync(shell.CurrentPage);
				var drawer = ((IShellContext)shell.Handler).CurrentDrawerLayout;
				shell.FlyoutIsPresented = true;
				await AssertionExtensions.AssertEventually(() => drawer.IsOpen);
				await OnLoadedAsync(header);
				await OnLoadedAsync(footer);
				var flyout = drawer.GetChildAt(1);
				var insets = ViewCompat.GetRootWindowInsets(flyout);
				Assert.NotNull(insets);
				var bars = insets.GetInsets(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.DisplayCutout());
				Assert.True(bars.Top > 0 && bars.Bottom > 0);

				await AssertionExtensions.AssertEventually(() =>
					flyout.PaddingTop >= bars.Top && flyout.PaddingBottom >= bars.Bottom);
				header.ClearValue(Grid.SafeAreaEdgesProperty);
				footer.ClearValue(Grid.SafeAreaEdgesProperty);
				ViewCompat.RequestApplyInsets(flyout);
				await AssertionExtensions.AssertEventually(() =>
					flyout.PaddingTop >= bars.Top && flyout.PaddingBottom >= bars.Bottom,
					message: "Clearing an explicit default must preserve the flyout's safe-area padding.");
				header.SafeAreaEdges = SafeAreaEdges.None;
				footer.SafeAreaEdges = SafeAreaEdges.None;
				await AssertionExtensions.AssertEventually(() =>
					flyout.PaddingTop == 0 && flyout.PaddingBottom == 0,
					message: "SafeAreaEdges.None must clear the layout's safe-area padding.");
				header.ClearValue(Grid.SafeAreaEdgesProperty);
				footer.ClearValue(Grid.SafeAreaEdgesProperty);
				await AssertionExtensions.AssertEventually(() =>
					flyout.PaddingTop >= bars.Top && flyout.PaddingBottom >= bars.Bottom,
					message: "Clearing SafeAreaEdges.None must restore the default flyout insets.");
			});
		}

		[Theory]
		[InlineData(FlyoutHeaderBehavior.Scroll)]
		[InlineData(FlyoutHeaderBehavior.CollapseOnScroll)]
		public async Task ScrollingFlyoutContentRemainsBelowSystemBar(FlyoutHeaderBehavior behavior)
		{
			SetupBuilder();
			var items = new VerticalStackLayout();
			for (int i = 0; i < 40; i++)
				items.Add(new Label { Text = $"Item {i}", HeightRequest = 48 });
			var content = new ScrollView { Content = items };
			var header = new ContentView
			{
				HeightRequest = 120,
				SafeAreaEdges = SafeAreaEdges.None,
				Content = new Label { Text = "Header" }
			};
			var shell = await InvokeOnMainThreadAsync(() => new Shell
			{
				FlyoutHeader = header,
				FlyoutContent = content,
				FlyoutHeaderBehavior = behavior,
				Items = { new FlyoutItem { Items = { new ContentPage() } } }
			});

			await CreateHandlerAndAddToWindow(shell, async () =>
			{
				await OnNavigatedToAsync(shell.CurrentPage);
				var drawer = ((IShellContext)shell.Handler).CurrentDrawerLayout;
				shell.FlyoutIsPresented = true;
				await AssertionExtensions.AssertEventually(() => drawer.IsOpen);
				await OnLoadedAsync(content);
				var flyout = drawer.GetChildAt(1);
				var insets = ViewCompat.GetRootWindowInsets(flyout);
				Assert.NotNull(insets);
				var topInset = insets.GetInsets(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.DisplayCutout()).Top;
				Assert.True(topInset > 0);

				await content.ScrollToAsync(0, 1000, false);
				await AssertionExtensions.AssertEventually(() => content.ScrollY > 0);
				var coordinator = Assert.IsAssignableFrom<CoordinatorLayout>(flyout);
				var appBar = coordinator.FindViewById<AppBarLayout>(Controls.Resource.Id.flyoutcontent_appbar);
				var appBarBehavior = Assert.IsType<AppBarLayout.Behavior>(
					Assert.IsType<CoordinatorLayout.LayoutParams>(appBar.LayoutParameters).Behavior);
				var initialTop = appBar.Top;
				appBarBehavior.OnNestedPreScroll(coordinator, appBar, content.ToPlatform(), 0,
					(int)flyout.Context.ToPixels(1000), new int[2], ViewCompat.TypeTouch);
				await AssertionExtensions.AssertEventually(() => appBar.Top < initialTop,
					message: "The flyout header did not scroll.");
				var rootLocation = new int[2];
				var contentLocation = new int[2];
				flyout.GetLocationOnScreen(rootLocation);
				content.ToPlatform().GetLocationOnScreen(contentLocation);
				Assert.True(contentLocation[1] >= rootLocation[1] + topInset,
					$"Scrolled content top {contentLocation[1]} overlaps the safe top {rootLocation[1] + topInset}.");
			});
		}
	}
}
