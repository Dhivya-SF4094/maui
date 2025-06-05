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

		var collectionView = new CollectionView
		{
			ItemsSource = ViewModel.ItemList,
			Background = Colors.Green,
			EmptyView = new VerticalStackLayout
			{
				Margin = new Thickness(20),
				BackgroundColor = Colors.AliceBlue,
				Children =
				{
					new Label
					{
						Text = "Empty View",
					}
				}
			},
		};

		var stack = new StackLayout();

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