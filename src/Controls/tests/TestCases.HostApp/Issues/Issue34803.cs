namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 34803, "[iOS] Back Button long press navigation cannot be prevented by canceling the navigation", PlatformAffected.iOS)]
public class Issue34803 : Shell
{
	public Issue34803()
	{
		Items.Add(new ShellContent
		{
			ContentTemplate = new DataTemplate(() => new Issue34803RootPage()),
			Title = "Root",
			Route = "issue34803_root"
		});

		Routing.RegisterRoute("issue34803_page1", typeof(Issue34803Page1));
		Routing.RegisterRoute("issue34803_page2", typeof(Issue34803Page2));
		Routing.RegisterRoute("issue34803_page3", typeof(Issue34803Page3));
	}

	protected override void OnNavigating(ShellNavigatingEventArgs args)
	{
		base.OnNavigating(args);

		// Cancel any back navigation (URI containing "..").
		// On iOS, the race condition in ShellSectionRenderer causes this cancellation to be
		// ineffective: the view controller is already popped before OnNavigating fires.
		if (args.Target?.Location?.OriginalString?.Contains("..", StringComparison.Ordinal) == true)
		{
			args.Cancel();
		}
	}
}

file class Issue34803RootPage : ContentPage
{
	public Issue34803RootPage()
	{
		Title = "Root";
		Content = new VerticalStackLayout
		{
			Padding = 20,
			Spacing = 20,
			VerticalOptions = LayoutOptions.Center,
			Children =
			{
				new Label
				{
					Text = "Root Page",
					AutomationId = "RootPageLabel",
					HorizontalOptions = LayoutOptions.Center
				},
				new Button
				{
					Text = "Go to Page One",
					AutomationId = "GoToPage1Button",
					Command = new Command(async () => await Shell.Current.GoToAsync("issue34803_page1"))
				}
			}
		};
	}
}

file class Issue34803Page1 : ContentPage
{
	public Issue34803Page1()
	{
		Title = "Page One";
		Content = new VerticalStackLayout
		{
			Padding = 20,
			Spacing = 20,
			VerticalOptions = LayoutOptions.Center,
			Children =
			{
				new Label
				{
					Text = "Page One",
					AutomationId = "Page1Label",
					HorizontalOptions = LayoutOptions.Center
				},
				new Button
				{
					Text = "Go to Page Two",
					AutomationId = "GoToPage2Button",
					Command = new Command(async () => await Shell.Current.GoToAsync("issue34803_page2"))
				}
			}
		};
	}
}

file class Issue34803Page2 : ContentPage
{
	public Issue34803Page2()
	{
		Title = "Page Two";
		Content = new VerticalStackLayout
		{
			Padding = 20,
			Spacing = 20,
			VerticalOptions = LayoutOptions.Center,
			Children =
			{
				new Label
				{
					Text = "Page Two",
					AutomationId = "Page2Label",
					HorizontalOptions = LayoutOptions.Center
				},
				new Button
				{
					Text = "Go to Page Three",
					AutomationId = "GoToPage3Button",
					Command = new Command(async () => await Shell.Current.GoToAsync("issue34803_page3"))
				}
			}
		};
	}
}

file class Issue34803Page3 : ContentPage
{
	public Issue34803Page3()
	{
		Title = "Page Three";
		Content = new VerticalStackLayout
		{
			Padding = 20,
			Spacing = 20,
			VerticalOptions = LayoutOptions.Center,
			Children =
			{
				new Label
				{
					Text = "Page Three - back navigation prevented",
					AutomationId = "Page3Label",
					HorizontalOptions = LayoutOptions.Center,
					HorizontalTextAlignment = TextAlignment.Center
				}
			}
		};
	}
}
