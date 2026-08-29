namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 37323, "Setting the padding value through binding or by using x:Name does not update the ScrollView padding", PlatformAffected.Android)]

public partial class Issue37323 : ContentPage
{
	Thickness _scrollViewPadding;

	public Thickness ScrollViewPadding
	{
		get => _scrollViewPadding;
		set
		{
			if (_scrollViewPadding != value)
			{
				_scrollViewPadding = value;
				OnPropertyChanged();
			}
		}
	}

	public Issue37323()
	{
		InitializeComponent();
		BindingContext = this;
	}

	private void OnPaddingClicked(object sender, EventArgs e)
	{
		ScrollViewPadding = new Thickness(20);
	}
}
