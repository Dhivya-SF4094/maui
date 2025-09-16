using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 31330, "Rectangle renders as thin line instead of filled shape for small height values", PlatformAffected.Android)]
public class Issue31330 : ContentPage
{
    AbsoluteLayout absoluteLayout;
    Grid grid;

    public Issue31330()
    {
        grid = new Grid { BackgroundColor = Colors.LightGray };
        absoluteLayout = new AbsoluteLayout();
        grid.Children.Add(absoluteLayout);

        var button = new Button
        {
            Text = "Add Rectangle",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
            AutomationId = "Issue31330Button"
        };
        button.Clicked += OnAddRectangleClicked;

        var mainLayout = new Grid();
        mainLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        mainLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
        mainLayout.Children.Add(button);
        Grid.SetRow(button, 0);
        Grid.SetColumn(button, 0);
        mainLayout.Children.Add(grid);
        Grid.SetRow(grid, 1);
        Grid.SetColumn(grid, 0);
        Content = mainLayout;
    }

    private void OnAddRectangleClicked(object sender, EventArgs e)
    {
        double shapeWidth = 50;
        double shapeHeight = 1.2;

        var box = new BoxView
        {
            BackgroundColor = Colors.Red
        };

        var rectBox = new Rect(
           50, 50,
            shapeWidth,
            shapeHeight
        );

        AbsoluteLayout.SetLayoutBounds(box, rectBox);
        AbsoluteLayout.SetLayoutFlags(box, AbsoluteLayoutFlags.None);
        var rectangle = new Rectangle
        {
            BackgroundColor = Colors.Blue,
        };
        var rectRectangle = new Rect(
           100, 100,
            shapeWidth,
            shapeHeight
        );

        AbsoluteLayout.SetLayoutBounds(rectangle, rectRectangle);
        AbsoluteLayout.SetLayoutFlags(rectangle, AbsoluteLayoutFlags.None);
        absoluteLayout.Children.Add(box);
        absoluteLayout.Children.Add(rectangle);
    }
}