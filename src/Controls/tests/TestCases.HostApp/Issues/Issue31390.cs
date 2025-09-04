using System.ComponentModel;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 31390, "System.ArgumentException thrown when setting FlyoutLayoutBehavior dynamically", PlatformAffected.UWP)]
public class Issue31390 : FlyoutPage
{
    private Issue31390ViewModel _viewModel;
    public Issue31390()
    {
        _viewModel = new Issue31390ViewModel();
        BindingContext = _viewModel;

        var flyoutPage = new ContentPage
        {
            Title = "Flyout",
            AutomationId = "Issue31390Flyout",
            Content = new StackLayout
            {
                Children =
                    {
                        new Label { Text = "This is Flyout", 
						AutomationId = "Issue31390_FlyoutLabel"}
                    }
            }
        };

        var detailPage = new ContentPage
        {
            Content = new StackLayout
            {
                Padding = 16,
                Spacing = 16,
                Children =
                    {
                        new Label { Text = "This is Detail Page" }
                    }
            }
        };

        var navigationPage = new NavigationPage(detailPage)
        {
            Title = "Detail"
        };

        Flyout = flyoutPage;
        Detail = navigationPage;

		var ChangeToPopover = new ToolbarItem
		{
			Text = "ChangeToPopover",
			AutomationId = "Issue31390Button"
		};
		ChangeToPopover.Clicked += NavigateToPopover_Clicked;


		var SplitOnPortrait = new ToolbarItem
		{
			Text = "SplitOnPortrait",
			AutomationId = "SplitOnPortraitButton"
		};
		SplitOnPortrait.Clicked += NavigateToSplitOnPortrait_Clicked;
		ToolbarItems.Add(ChangeToPopover);
		ToolbarItems.Add(SplitOnPortrait);
		SetBinding(FlyoutLayoutBehaviorProperty, new Binding(nameof(Issue31390ViewModel.FlyoutLayoutBehavior)));
    }

	private void NavigateToSplitOnPortrait_Clicked(object sender, EventArgs e)
	{
		_viewModel.FlyoutLayoutBehavior = FlyoutLayoutBehavior.SplitOnPortrait;
	}

	private void NavigateToPopover_Clicked(object sender, EventArgs e)
    {
        _viewModel.FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
    }
}

public class Issue31390ViewModel : INotifyPropertyChanged
{
    private FlyoutLayoutBehavior _flyoutLayoutBehavior = FlyoutLayoutBehavior.Default;
    public FlyoutLayoutBehavior FlyoutLayoutBehavior
    {
        get => _flyoutLayoutBehavior;
        set
        {
            if (_flyoutLayoutBehavior != value)
            {
                _flyoutLayoutBehavior = value;
                OnPropertyChanged(nameof(FlyoutLayoutBehavior));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}