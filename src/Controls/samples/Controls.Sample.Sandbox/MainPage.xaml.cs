namespace Maui.Controls.Sample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnToggleFlowDirectionClicked(object sender, EventArgs e)
	{
		// Toggle the page FlowDirection between LeftToRight and RightToLeft
		if (this.FlowDirection == FlowDirection.LeftToRight)
		{
			this.FlowDirection = FlowDirection.RightToLeft;
			StatusLabel.Text = "Page FlowDirection: RightToLeft";
		}
		else
		{
			this.FlowDirection = FlowDirection.LeftToRight;
			StatusLabel.Text = "Page FlowDirection: LeftToRight";
		}
	}
}