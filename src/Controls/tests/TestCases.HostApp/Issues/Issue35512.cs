namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 35512, "Button BackgroundColor does not restore to default when reset to null after dynamic update", PlatformAffected.All)]
public class Issue35512 : ContentPage
{
	public Issue35512()
	{
		var testButton = new Button
		{
			Text = "Test Button",
			AutomationId = "TestButton"
		};

		var setColorButton = new Button
		{
			Text = "Set Red Background",
			AutomationId = "SetColorButton"
		};

		var resetColorButton = new Button
		{
			Text = "Reset Background to Null",
			AutomationId = "ResetColorButton"
		};

		var statusLabel = new Label
		{
			Text = "Default",
			AutomationId = "StatusLabel"
		};

		setColorButton.Clicked += (s, e) =>
		{
			testButton.BackgroundColor = Colors.Red;
			statusLabel.Text = "Set to Red";
		};

		resetColorButton.Clicked += (s, e) =>
		{
			testButton.BackgroundColor = null;
			statusLabel.Text = "Reset to null";
		};

		Content = new VerticalStackLayout
		{
			Padding = new Thickness(20),
			Spacing = 10,
			Children =
			{
				statusLabel,
				testButton,
				setColorButton,
				resetColorButton
			}
		};
	}
}
