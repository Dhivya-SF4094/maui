
namespace Maui.Controls.Sample.Issues
{ 
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Issue5669 : ContentPage
	{
		public Issue5669()
		{
			InitializeComponent();
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			searchBar.MaxLengthProperty = 5;
		}
	}
}