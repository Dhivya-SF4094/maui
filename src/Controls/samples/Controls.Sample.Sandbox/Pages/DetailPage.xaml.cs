
using MauiClient.ViewModels;

namespace MauiClient.Pages;

public partial class DetailPage : ContentPage
{
    public DetailPage(DetailViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}