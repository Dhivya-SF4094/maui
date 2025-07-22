using System.ComponentModel;

namespace Maui.Controls.Sample.Issues
{
	[Issue(IssueTracker.Github, -1, "RefreshView IsRefreshing property not working while binding on Windows", PlatformAffected.All)]
	public partial class IssueRefreshViewBinding : ContentPage, INotifyPropertyChanged
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

		public IssueRefreshViewBinding()
		{
			// Set IsRefreshing to true before InitializeComponent to simulate binding before control is loaded
			IsRefreshing = true;

			InitializeComponent();
			BindingContext = this;
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