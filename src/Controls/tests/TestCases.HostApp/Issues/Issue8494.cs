using System.Collections.ObjectModel;

namespace Maui.Controls.Sample.Issues;
[Issue(IssueTracker.Github, 8494, "Margin doesn't work inside CollectionView EmptyView", PlatformAffected.UWP)]

public class Issue8494 : ContentPage
{
	Issue8494EmptyViewModel ViewModel;

	public Issue8494()
	{
		ViewModel = new Issue8494EmptyViewModel();

		var label = new Label
		{
			Text = "EmptyView should be laid out with respect to the Specified margin",
			AutomationId = "EmptyViewDescriptionLabel",
		};

		var emptyViewLayout = new StackLayout
		{
			Margin = new Thickness(40),
			Children =
				{
					new Label
					{
						Text = "Stay Home! Until:",
						BackgroundColor = Colors.Blue,
						HorizontalTextAlignment = TextAlignment.Center,
					},
					new Button
					{
						Text = "Find a Destination",
						BackgroundColor = Colors.Yellow,
					}
				}
		};

		var collectionView = new CollectionView
		{
			BackgroundColor = Colors.Green,
			EmptyView = emptyViewLayout
		};

		var stack = new StackLayout
		{
			VerticalOptions = LayoutOptions.Center,
		};

		stack.Children.Add(label);
		stack.Children.Add(collectionView);
		Content = stack;
	}
}

public class Issue8494EmptyViewModel
{
	public ObservableCollection<string> ItemList { get; set; }

	public Issue8494EmptyViewModel()
	{
		ItemList = new ObservableCollection<string>();
	}
}