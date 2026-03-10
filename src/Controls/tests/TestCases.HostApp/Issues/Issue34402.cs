using Microsoft.Maui.Controls;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 34402, "FlowDirection property not working on BoxView Control", PlatformAffected.All)]
public class Issue34402 : ContentPage
{
	public Issue34402()
	{
		var boxView = new BoxView
		{
			CornerRadius = new CornerRadius(30, 60, 10, 20),
			Color = Colors.CornflowerBlue,
			WidthRequest = 200,
			HeightRequest = 200,
			AutomationId = "MyBoxView"
		};

		var checkBox = new CheckBox
		{
			AutomationId = "RtlCheckBox"
		};

		checkBox.CheckedChanged += (s, e) =>
		{
			boxView.FlowDirection = e.Value ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
		};

		Content = new VerticalStackLayout
		{
			Padding = new Thickness(20),
			Spacing = 20,
			Children =
			{
				new Label { Text = "Toggle RTL to see corners mirror:" },
				checkBox,
				boxView
			}
		};
	}
}
