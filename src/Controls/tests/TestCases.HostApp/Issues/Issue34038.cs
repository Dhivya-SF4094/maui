namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 34038, "[macOS] IsEnabled property false not working on MenuBarItem",
	PlatformAffected.macOS)]
public class Issue34038 : TestShell
{
	protected override void Init()
	{
		var resultLabel = new Label
		{
			Text = "InitialState",
			AutomationId = "ResultLabel"
		};

		var menuBarItem = new MenuBarItem
		{
			Text = "TestMenu"
		};

		var menuFlyoutItem = new MenuFlyoutItem
		{
			Text = "Perform Action"
		};
		menuFlyoutItem.Clicked += OnMenuFlyoutItemClicked;
		//	menuFlyoutItem.Clicked += (s, e) => resultLabel.Text = "ActionFired";
		menuBarItem.Add(menuFlyoutItem);

		MenuBarItems.Add(menuBarItem);

		var disableButton = new Button
		{
			Text = "Disable Menu",
			AutomationId = "DisableMenuButton"
		};
		disableButton.Clicked += (s, e) =>
		{
			menuBarItem.IsEnabled = false;
			resultLabel.Text = "MenuBarDisabled";
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
					resultLabel
				}
			}
		};

		AddContentPage(contentPage);
		FlyoutBehavior = FlyoutBehavior.Disabled;
	}

	private void OnMenuFlyoutItemClicked(object sender, EventArgs e)
	{

	}
}
