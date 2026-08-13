using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 37323, "Setting the padding value through binding or by using x:Name does not update the ScrollView padding", PlatformAffected.Android)]

public partial class Issue37323 : ContentPage
{
	private Thickness _padding = new Thickness(0);

	public new Thickness Padding
	{
		get => _padding;
		set
		{
			if (_padding != value)
			{
				_padding = value;
				OnPropertyChanged();
			}
		}
	}

	protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		base.OnPropertyChanged(propertyName);
	}

	public Issue37323()
	{
		InitializeComponent();
		BindingContext = this;
	}

	private void OnSafeAreaNoneClicked(object sender, EventArgs e)
	{
		TestScrollView.SafeAreaEdges = new SafeAreaEdges(SafeAreaRegions.None);
		SafeAreaEdgesValueLabel.Text = "None";
	}
	private void OnSafeAreaAllClicked(object sender, EventArgs e)
	{
		TestScrollView.SafeAreaEdges = new SafeAreaEdges(SafeAreaRegions.All);
		SafeAreaEdgesValueLabel.Text = "All";
	}
	private void OnSafeAreaContainerClicked(object sender, EventArgs e)
	{
		TestScrollView.SafeAreaEdges = new SafeAreaEdges(SafeAreaRegions.Container);
		SafeAreaEdgesValueLabel.Text = "Container";
	}
	private void OnSafeAreaSoftInputClicked(object sender, EventArgs e)
	{
		TestScrollView.SafeAreaEdges = new SafeAreaEdges(SafeAreaRegions.SoftInput);
		SafeAreaEdgesValueLabel.Text = "SoftInput";
	}
	private void OnSafeAreaDefaultClicked(object sender, EventArgs e)
	{
		TestScrollView.SafeAreaEdges = new SafeAreaEdges(SafeAreaRegions.Default);
		SafeAreaEdgesValueLabel.Text = "Default";
	}

	private void OnPaddingClicked(object sender, EventArgs e)
	{
		Padding = new Thickness(20);
	}
}
