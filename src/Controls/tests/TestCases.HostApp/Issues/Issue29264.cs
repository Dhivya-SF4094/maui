namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 29264, "Rotating My Recipes changes the selected item", PlatformAffected.iOS)]
public class Issue29264 : ContentPage
{
	readonly CarouselView _carousel;
	readonly Label _orientationLabel;
	readonly Label _positionLabel;

	public Issue29264()
	{
		_carousel = new CarouselView
		{
			AutomationId = "RecipeCarousel",
			ItemsSource = new[] { "First Recipe", "Second Recipe", "Third Recipe", "Fourth Recipe", "Fifth Recipe" },
			WidthRequest = 350,
			HeightRequest = 550,
			HorizontalOptions = LayoutOptions.Fill,
			Loop = false,
			ItemTemplate = new DataTemplate(() =>
			{
				var label = new Label
				{
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center
				};
				label.SetBinding(Label.TextProperty, ".");
				label.SetBinding(Label.AutomationIdProperty, ".");
				return new Grid
				{
					WidthRequest = 340,
					Padding = new Thickness(0, 0, 10, 0),
					Children = { label }
				};
			})
		};

		_orientationLabel = new Label { AutomationId = "OrientationLabel" };
		_positionLabel = new Label
		{
			AutomationId = "RecipePositionLabel",
			Text = "Position: 0"
		};
		var currentItemLabel = new Label { AutomationId = "RecipeCurrentItemLabel" };
		currentItemLabel.SetBinding(Label.TextProperty, new Binding(nameof(CarouselView.CurrentItem), source: _carousel));
		var positionChangedLabel = new Label
		{
			AutomationId = "RecipePositionChangedLabel",
			Text = "Changes: 0"
		};
		int positionChanges = 0;
		_carousel.PositionChanged += (_, e) =>
		{
			_positionLabel.Text = $"Position: {e.CurrentPosition}";
			positionChangedLabel.Text = $"Changes: {++positionChanges}";
		};

		Content = new StackLayout
		{
			Margin = 20,
			Children = { _orientationLabel, _positionLabel, currentItemLabel, positionChangedLabel, _carousel }
		};
	}

	protected override void OnSizeAllocated(double width, double height)
	{
		base.OnSizeAllocated(width, height);

		if (width <= 0 || height <= 0)
			return;

		if (width > height)
		{
			_carousel.HeightRequest = 200;
			_carousel.WidthRequest = width - 100;
			_orientationLabel.Text = "Landscape";
		}
		else
		{
			_carousel.HeightRequest = height - 300;
			_carousel.WidthRequest = 350;
			_orientationLabel.Text = "Portrait";
		}
	}
}
