namespace Maui.Controls.Sample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void Action_Clicked(object? sender, EventArgs e)
	{
		//if(container.Count>0 && container[0] is Border border)
		//{
		//	border.Content = null;
		//}

		//border.Content = null;
		//container.Clear();

		//action.Background = Colors.Blue;
		//container.Children.Add(new Border() { Content = action });
		//container.Add(action);
		//container.Add(new Border(){Content= new Button() {Text="this is new button" } });
	}

	private void Button_Clicked(object sender, EventArgs e)
	{
	
		if (Application.Current?.Windows.Count > 0 &&
			Application.Current.Windows[0].Page is Shell shell)
		{
			var firstTab = shell.CurrentItem?.Items[0];
			var secondTab = shell.CurrentItem?.Items[1];
			if (secondTab is not null && firstTab is not null)
			{
				//secondTab.Title = "third";
				secondTab.IsEnabled =!secondTab.IsEnabled;
				//firstTab.IsEnabled = !firstTab.IsEnabled;

				//firstTab.Title = "i have changed the first title";

			}
		//	if(button1.BackgroundColor == Colors.Green)
			//{
			//	button1.BackgroundColor = Colors.Blue;
			//}
			//else
			//{
			//	button1.BackgroundColor = Colors.Green;
			//}

		}
		else
		{
			System.Diagnostics.Debug.WriteLine("Shell not found!");
		}
	
}

	private void Button_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if(e.PropertyName == "IsEnabled")
		{

		}
	}

	private void Button_Clicked_1(object sender, EventArgs e)
	{
		if (Application.Current?.Windows.Count > 0 &&
			Application.Current.Windows[0].Page is Shell shell) // C# 9 pattern matching
		{
			var secondTab = shell.CurrentItem?.Items[1];
			if (secondTab is not null)
				secondTab.IsEnabled = true;
			// Now you can safely access members of shell
			// Find the second tab by name...
		}
		else
		{
			// Handle the case where there's no Shell.  Perhaps log a message or disable the button.
			System.Diagnostics.Debug.WriteLine("Shell not found!");
		}
	}

	private void Button_Clicked_2(object sender, EventArgs e)
	{

	}
}
public interface IItem
{

}
