using System;
using System.Collections.ObjectModel;
using Maui.Controls.Sample.Issues;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 29609, "ItemSpacing on CarouselView resizes items", PlatformAffected.Android)]
public class Issue29609 : ContentPage
{
    Issue29609_ViewModel ViewModel;
    public Issue29609()
    {
        ViewModel = new Issue29609_ViewModel();
        BindingContext = ViewModel;
        var carouselView = new CarouselView
        {
            BackgroundColor = Colors.Red,
            HeightRequest = 400,
            WidthRequest = 300,
            ItemsSource = ViewModel.Items,
            Loop = false,
            AutomationId = "ItemSpacing_CarouselView",
            ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Horizontal)
            {
                ItemSpacing = 50,
                SnapPointsAlignment = SnapPointsAlignment.Center,
                SnapPointsType = SnapPointsType.MandatorySingle
            },
            ItemTemplate = new DataTemplate(() =>
            {
                var grid = new Grid
                {
                    Margin = 0,
                    Padding = 0,
                    BackgroundColor = Colors.Yellow
                };

                var label = new Label
                {
                    FontSize = 24,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                label.SetBinding(Label.TextProperty, ".");

                grid.Children.Add(label);
                return grid;
            })
        };

        Content = carouselView;
    }
}

public partial class Issue29609_ViewModel
{
    public ObservableCollection<string> Items = new();

    public Issue29609_ViewModel()
    {
        for (var i = 0; i < 2; i++)
        {
            Items.Add($"Item {i}");
        }
    }
}