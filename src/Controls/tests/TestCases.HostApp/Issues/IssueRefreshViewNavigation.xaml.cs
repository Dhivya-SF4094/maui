using System.ComponentModel;

namespace Maui.Controls.Sample.Issues
{
	[Issue(IssueTracker.Github, -1, "RefreshView IsRefreshing property not working with navigation on Windows", PlatformAffected.All)]
	public partial class IssueRefreshViewNavigation : ContentPage, INotifyPropertyChanged
	{
		bool _isRefreshing;

		public bool IsRefreshing
		{
			get => _isRefreshing;
			set
			{
				if (_isRefreshing != value)
				{
					_isRefreshing = value;
					OnPropertyChanged(nameof(IsRefreshing));
				}
			}
		}

		public IssueRefreshViewNavigation()
		{
			InitializeComponent();
			BindingContext = this;
		}

		async void OnNavigateClicked(object sender, EventArgs e)
		{
			// Navigate to a simple page, set IsRefreshing=true, then navigate back
			var secondPage = new ContentPage
			{
				Content = new StackLayout
				{
					Children =
					{
						new Label { Text = "Second Page", AutomationId = "SecondPageLabel" },
						new Label { Text = "Click the button below to set IsRefreshing=true on the main page while you're on this page, then return.", Margin = new Thickness(10) },
						new Button 
						{ 
							Text = "Set Refresh & Go Back", 
							AutomationId = "GoBackButton",
							Command = new Command(async () =>
							{
								// This simulates setting IsRefreshing in a navigated page
								// This is the exact scenario that was reported as broken
								IsRefreshing = true;
								await Navigation.PopAsync();
								// When we return to the main page, the RefreshView should show the refresh indicator
							})
						}
					}
				}
			};

			await Navigation.PushAsync(secondPage);
		}

		void OnToggleClicked(object sender, EventArgs e)
		{
			IsRefreshing = !IsRefreshing;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}