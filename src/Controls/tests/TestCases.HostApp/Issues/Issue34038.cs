namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 34038, "[macOS] IsEnabled property false not working on MenuBarItem",
	PlatformAffected.macOS)]
public class Issue34038 : TestShell
{
	Label _resultLabel = null!;

	protected override void Init()
	{
		_resultLabel = new Label
		{
			Text = "InitialState",
			AutomationId = "ResultLabel"
		};

		var menuBarItem = new MenuBarItem
		{
			Text = "TestMenu"
		};

		var subMenu = new MenuFlyoutSubItem
		{
			Text = "SubMenu",
			AutomationId = "SubMenu"
		};

		var subMenuItem = new MenuFlyoutItem
		{
			Text = "Perform SubMenu Action",
			AutomationId = "SubMenuItem"
		};
		subMenuItem.Clicked += OnSubMenuItemClicked;
		subMenu.Add(subMenuItem);
		menuBarItem.Add(subMenu);

		var menuFlyoutItem = new MenuFlyoutItem
		{
			Text = "Perform Action",
			AutomationId = "FirstMenuItem"
		};

		menuFlyoutItem.Clicked += OnFirstMenuItemClicked;
		menuBarItem.Add(menuFlyoutItem);

		var secondMenuFlyoutItem = new MenuFlyoutItem
		{
			Text = "Perform Second Action",
			AutomationId = "SecondMenuItem"
		};
		secondMenuFlyoutItem.Clicked += OnSecondMenuItemClicked;
		menuBarItem.Add(secondMenuFlyoutItem);

		MenuBarItems.Add(menuBarItem);

		var disableButton = new Button
		{
			Text = "Disable Menu Bar",
			AutomationId = "DisableMenuButton"
		};
		disableButton.Clicked += (s, e) =>
		{
			menuBarItem.IsEnabled = false;
			_resultLabel.Text = "MenuBarDisabled";
		};

		var disableItemButton = new Button
		{
			Text = "Enable menubar and disable Menu Item",
			AutomationId = "DisableItemButton"
		};
		disableItemButton.Clicked += (s, e) =>
		{
			menuBarItem.IsEnabled = true;
			subMenu.IsEnabled = true;
			secondMenuFlyoutItem.IsEnabled = false;
			_resultLabel.Text = "MenuItemDisabled";
		};

		var disableSubMenuButton = new Button
		{
			Text = "Enable menubar and disable SubMenu",
			AutomationId = "DisableSubMenuButton"
		};
		disableSubMenuButton.Clicked += (s, e) =>
		{
			menuBarItem.IsEnabled = true;
			secondMenuFlyoutItem.IsEnabled = true;
			subMenu.IsEnabled = false;
			_resultLabel.Text = "SubMenuDisabled";
		};

		var contentPage = new ContentPage
		{
			Content = new VerticalStackLayout
			{
				Padding = new Thickness(20),
				Spacing = 20,
				Children =
				{
					new Label { Text = "MenuBarItem IsEnabled Test" },
					disableButton,
					disableItemButton,
					disableSubMenuButton,
					_resultLabel
				}
			}
		};

		AddContentPage(contentPage);
		FlyoutBehavior = FlyoutBehavior.Disabled;
	}

	private void OnSecondMenuItemClicked(object sender, EventArgs e)
	{
		_resultLabel.Text = "SecondActionFired";
	}

	private void OnFirstMenuItemClicked(object sender, EventArgs e)
	{
		_resultLabel.Text = "FirstActionFired";
	}

	private void OnSubMenuItemClicked(object sender, EventArgs e)
	{
		_resultLabel.Text = "SubMenuActionFired";
	}
}
