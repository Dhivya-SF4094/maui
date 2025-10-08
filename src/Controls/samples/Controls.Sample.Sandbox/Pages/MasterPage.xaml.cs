using MauiClient.Models;
using MauiClient.ViewModels;
using System.Diagnostics;

namespace MauiClient.Pages;

public partial class MasterPage : ContentPage
{
    public MasterPage(MasterViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MasterViewModel model)
        {
            Debug.WriteLine(nameof(OnAppearing));
            await model.ReloadAsync();
        }
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Debug.WriteLine(nameof(OnSelectionChanged));

        var navparam = new Dictionary<string, object?>
        {
            [nameof(Monkey)] = e.CurrentSelection.FirstOrDefault() as Monkey
        };

        await Shell.Current.GoToAsync(nameof(DetailPage), navparam);
    }

	private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
	{

	}
}