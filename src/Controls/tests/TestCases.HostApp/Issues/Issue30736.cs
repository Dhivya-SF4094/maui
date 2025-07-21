namespace Maui.Controls.Sample.Issues;
[Issue(IssueTracker.Github, 30736, "DatePicker's DateSelected event not firing on Windows - .NET 10 Preview 6", PlatformAffected.UWP)]
public class Issue30736 :ContentPage
{
	Label label;
	DatePicker datePicker;

	public Issue30736()
	{
		datePicker = new DatePicker
		{
			AutomationId = "Issue30736_DatePicker",
		};

		datePicker.DateSelected += DatePicker_DateSelected;

		label = new Label
		{
			Text = "Issue30736_DatePickerLabel",
		};

		var stackLayout = new StackLayout
		{
			Children =
			{
				datePicker,
				label,
			}
		};

		Content = stackLayout;
	}

	private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
	{
		label.Text = "Date Selected event Fired";
	}
}