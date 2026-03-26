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

				// CanPerform always returns true for "MenuItem" selectors, so we
				// must guard execution ourselves when the item or any ancestor
				// menu element is disabled.
				if (HasDisabledMenuElementInPath(menuElement))
				{
					return;
				}

				menuElement.Clicked();
			}
		}

		static bool HasDisabledMenuElementInPath(IMenuElement menuElement)
		{
			IElement? current = menuElement;

			while (current is not null)
			{
				if (current is IMenuBarItem { IsEnabled: false })
					return true;

				if (current is IMenuElement { IsEnabled: false })
					return true;

				current = current.Parent;
			}

			return false;
		}

		internal static void Reset()
		{
			if (!OperatingSystem.IsIOSVersionAtLeast(13))
				return;

			menus.Clear();
		}
	}
}
