using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace Microsoft.Maui.Handlers
{
	[System.Runtime.Versioning.SupportedOSPlatform("ios13.0")]
	public partial class MenuFlyoutItemHandler
	{
		internal static Dictionary<int, IMenuElement> menus = new Dictionary<int, IMenuElement>();

		bool IsInContextFlyout()
		{
			IElement? current = VirtualView;
			while (current != null)
			{
				if (current is Microsoft.Maui.IMenuFlyout)
					return true;
				current = current.Parent;
			}
			return false;
		}

		protected override UIMenuElement CreatePlatformElement()
		{
			// https://github.com/dotnet/maui/issues/9332
			// The menu code needs to be converted over to using `UIAction`
			// so that all of this can be the same
			if (IsInContextFlyout())
			{
				UIImage? contextUiImage = VirtualView.Source.GetPlatformMenuImage(MauiContext!);

				var uiAction = UIAction.Create(
					title: VirtualView.Text,
					image: contextUiImage,
					identifier: null,
					handler: (_) =>
				{
					if (VirtualView?.IsEnabled == false)
					{
						return;
					}
					VirtualView?.Clicked();
				});

				return uiAction;
			}

			return VirtualView.CreateMenuItem(MauiContext!);
		}

		public static void MapIsEnabled(IMenuFlyoutItemHandler handler, IMenuFlyoutItem view)
		{
			handler.PlatformView?.UpdateIsEnabled(view);
		}

		internal static void Execute(UICommand uICommand)
		{
			if (uICommand.PropertyList is NSString nsString &&
				Int32.TryParse(nsString.ToString(), out int index))
			{
				if (!menus.TryGetValue(index, out var menuElement))
					return;

				// Respect the disabled state of the parent MenuBarItem.
				// CanPerform always returns true for "MenuItem" selectors, so we
				// must guard here: if the parent MenuBarItem is disabled its children
				// should not fire even when invoked via accessibility or keyboard.
				if (menuElement.Parent is IMenuBarItem { IsEnabled: false })
					return;

				// Respect the item's own IsEnabled state.
				if (menuElement is IMenuFlyoutItem { IsEnabled: false })
				{
					return;
				}

				menuElement.Clicked();
			}
		}

		internal static void Reset()
		{
			if (!OperatingSystem.IsIOSVersionAtLeast(13))
				return;

			menus.Clear();
		}
	}
}
