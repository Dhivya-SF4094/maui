namespace Maui.Controls.Sample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnTestButtonClicked(object sender, EventArgs e)
	{
		// Test changing CharacterSpacing dynamically
		TestSearchBar.CharacterSpacing = TestSearchBar.CharacterSpacing == 5 ? 15 : 5;
		TestSearchBar2.CharacterSpacing = TestSearchBar2.CharacterSpacing == 10 ? 20 : 10;
		
		// Update the button text to show the change
		TestButton.Text = $"CharacterSpacing: {TestSearchBar.CharacterSpacing}, {TestSearchBar2.CharacterSpacing}";
	}
}