using Microsoft.Maui.Controls.Shapes;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 30649, "GraphicsView event handlers are triggered even when IsEnabled is set to False", PlatformAffected.Android | PlatformAffected.iOS | PlatformAffected.macOS)]
public class Issue_30649 : ContentPage
{
    Label StatusLabel;
    Label LastEventLabel;
    Switch IsEnabledSwitch;
    GraphicsView TestGraphicsView;

    public Issue_30649()
    {
        var titleLabel = new Label
        {
            Text = "Test passes if event does not trigger when IsEnabled is false",
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.DarkBlue
        };

        var isEnabledLabel = new Label
        {
            Text = "IsEnabled:",
            VerticalOptions = LayoutOptions.Center,
            FontSize = 16
        };

        IsEnabledSwitch = new Switch
        {
            IsToggled = true,
            AutomationId = "Issue30649_IsToggleSwitch"
        };
        IsEnabledSwitch.Toggled += OnIsEnabledToggled;

        StatusLabel = new Label
        {
            Text = "Enabled",
            TextColor = Colors.Green,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 16
        };

        var toggleLayout = new HorizontalStackLayout
        {
            Spacing = 15,
            HorizontalOptions = LayoutOptions.Center,
            Children = { isEnabledLabel, IsEnabledSwitch, StatusLabel }
        };

        TestGraphicsView = new GraphicsView
        {
            HeightRequest = 200,
            BackgroundColor = Colors.AliceBlue,
            IsEnabled = true,
            AutomationId = "Issue30649_GraphicsView"
        };
        TestGraphicsView.Drawable = new SimpleShapeDrawable();
        TestGraphicsView.StartInteraction += OnStartInteraction;
        TestGraphicsView.EndInteraction += OnEndInteraction;

        var border = new Border
        {
            Stroke = Colors.DarkBlue,
            StrokeThickness = 2,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 5
            },
            Content = TestGraphicsView
        };

        LastEventLabel = new Label
        {
            Text = "Touch the graphics area above",
            FontSize = 14,
            HorizontalOptions = LayoutOptions.Center,
            TextColor = Colors.DarkGreen
        };

        Content = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 15,
            Children =
                {
                    titleLabel,
                    toggleLayout,
                    border,
                    LastEventLabel
                }
        };
    }

    private void OnIsEnabledToggled(object sender, ToggledEventArgs e)
    {
        TestGraphicsView.IsEnabled = e.Value;
        StatusLabel.Text = e.Value ? "Enabled" : "Disabled";
        StatusLabel.TextColor = e.Value ? Colors.Green : Colors.Red;
    }

    private void OnStartInteraction(object sender, TouchEventArgs e)
    {
        if (e.Touches.Length > 0)
        {
            var touch = e.Touches[0];
            LastEventLabel.Text = "StartInteraction";
        }
    }

    private void OnEndInteraction(object sender, TouchEventArgs e)
    {
        if (e.Touches.Length > 0)
        {
            var touch = e.Touches[0];
            LastEventLabel.Text = "EndInteraction";
        }
    }
}

public class SimpleShapeDrawable : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.AliceBlue;
        canvas.FillRectangle(dirtyRect);
    }
}