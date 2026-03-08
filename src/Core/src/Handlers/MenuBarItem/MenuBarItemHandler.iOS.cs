using UIKit;

namespace Microsoft.Maui.Handlers
{
	[System.Runtime.Versioning.SupportedOSPlatform("ios13.0")]
	public partial class MenuBarItemHandler : ElementHandler<IMenuBarItem, UIMenu>, IMenuBarItemHandler
	{
		// Tracks whether the initial property-mapping cycle has completed.
		// MapIsEnabled is invoked during EVERY handler setup; on the first call we skip
		// the disconnect+rebuild (CreatePlatformElement already reflects the current
		// IsEnabled value). On subsequent calls it is a runtime change that requires
		// the menu to be recreated since UIMenu is immutable.
		bool _isEnabledApplied;

		protected override UIMenu CreatePlatformElement()
		{
			IUIMenuBuilder? uIMenuBuilder = null;

			if (VirtualView.Parent?.Handler?.PlatformView is IUIMenuBuilder builder)
			{
				uIMenuBuilder = builder;
			}

			return
				VirtualView
					.ToPlatformMenu(
						VirtualView.Text,
						null,
						MauiContext!,
						uIMenuBuilder,
						VirtualView.IsEnabled);
		}

		public static void MapIsEnabled(IMenuBarItemHandler handler, IMenuBarItem view)
		{
			// UIMenu is immutable — we need to recreate it when IsEnabled changes.
			// Guard against the initial property-mapping call (handler just created):
			// CreatePlatformElement already passed the current IsEnabled to ToPlatformMenu,
			// so no rebuild is needed. Rebuilding here would cause an infinite loop because
			// each new handler also goes through this same initial mapping.
			var h = (MenuBarItemHandler)handler;
			if (!h._isEnabledApplied)
			{
				h._isEnabledApplied = true;
				return;
			}

			// Synchronously update the existing children's UIAction/UICommand attributes
			// so the current UIMenu immediately reflects the new state, before the async
			// rebuild below completes. OnDisconnectHandler clears child handlers, so this
			// must happen BEFORE DisconnectHandler().
			foreach (var child in view)
			{
				if (child.Handler?.PlatformView is UIMenuElement element)
					element.UpdateMenuElementAttributes(view.IsEnabled);
			}

			// Disconnect and rebuild so the UIMenu is fully recreated with the correct
			// IsEnabled state for all future renders (UIMenu itself is immutable).
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
			MenuBarHandler.Rebuild();
		}
	}
}
