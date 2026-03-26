using UIKit;

namespace Microsoft.Maui.Handlers
{
	[System.Runtime.Versioning.SupportedOSPlatform("ios13.0")]
	public partial class MenuFlyoutSubItemHandler
	{
		bool _isEnabledApplied;

		protected override UIMenu CreatePlatformElement()
		{
			var menuBar = VirtualView.FindParentOfType<IMenuBar>();

			IUIMenuBuilder? uIMenuBuilder = null;
			if (menuBar?.Handler?.PlatformView is IUIMenuBuilder builder)
			{
				uIMenuBuilder = builder;
			}

			var menu =
				VirtualView.ToPlatformMenu(
					VirtualView.Text,
					VirtualView.Source,
					MauiContext!,
					uIMenuBuilder,
					VirtualView.IsEnabled);

			return menu;
		}

		public static void MapIsEnabled(IMenuFlyoutSubItemHandler handler, IMenuFlyoutSubItem view)
		{
			var h = (MenuFlyoutSubItemHandler)handler;
			if (!h._isEnabledApplied)
			{
				h._isEnabledApplied = true;
				return;
			}

			h.PlatformView?.UpdateMenuElementTreeAttributes(view.IsEnabled);
			handler.DisconnectHandler();
			Rebuild();
		}

		public void Add(IMenuElement view)
		{
			Rebuild();
		}

		public void Remove(IMenuElement view)
		{
			Rebuild();
		}

		public void Clear()
		{
			Rebuild();
		}

		public void Insert(int index, IMenuElement view)
		{
			Rebuild();
		}

		static void Rebuild()
		{
			// For context flyout support this likely also needs some logic like in MenuFlyoutItemHandler.iOS.cs where
			// it follows one code path for main menus (this existing code), and a different code path for context menus that
			// rebuilds the UIMenu of the context menu.
			// https://github.com/dotnet/maui/issues/9359
			MenuBarHandler.Rebuild();
		}
	}
}
