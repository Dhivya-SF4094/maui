using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 32042, "Rectangle appears blurred on iOS and macOS when its bounds are changed at runtime within an AbsoluteLayout", PlatformAffected.iOS | PlatformAffected.macOS)]
public class Issue32042 : ContentPage
{
	AbsoluteLayout absoluteLayout;
	Rectangle rectangle;
	Button changeBoundsButton;

	public Issue32042()
	{
		// Create the AbsoluteLayout
		absoluteLayout = new AbsoluteLayout
		{
			BackgroundColor = Colors.LightGray
		};

		// Create the Rectangle
		rectangle = new Rectangle
		{
			BackgroundColor = Colors.Green,
		};

		// Initially set AutoSize bounds
		AbsoluteLayout.SetLayoutBounds(rectangle, new Rect(0, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
		AbsoluteLayout.SetLayoutFlags(rectangle, AbsoluteLayoutFlags.None);

		// Create the Button
		changeBoundsButton = new Button
		{
			Text = "Change Bounds",
			AutomationId = "Issue32042Button"
		};

		AbsoluteLayout.SetLayoutBounds(changeBoundsButton, new Rect(0.5, 0.90, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
		AbsoluteLayout.SetLayoutFlags(changeBoundsButton, AbsoluteLayoutFlags.PositionProportional);

		// Attach click event
		changeBoundsButton.Clicked += OnChangeBoundsClicked;

		// Add children to layout
		absoluteLayout.Children.Add(rectangle);
		absoluteLayout.Children.Add(changeBoundsButton);

		// Set the Content of the page
		Content = absoluteLayout;
	}

	void OnChangeBoundsClicked(object sender, EventArgs e)
	{
		// Update layout bounds at runtime
		AbsoluteLayout.SetLayoutBounds(rectangle, new Rect(50, 50, 100, 100));
	}
}