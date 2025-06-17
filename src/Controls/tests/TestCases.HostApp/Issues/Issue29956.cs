// Issue29956.cs
// Add your test or reproduction code for issue 29956 here.

using System.ComponentModel;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 29956, "ImageButton Border Not Rendering Correctly with URI and File Images when applying Aspect property", PlatformAffected.Android)]
public class Issue29956 : ContentPage
{
    private readonly ImageViewModel _viewModel = new ImageViewModel();
    private ImageButton _myImageButton;

    public Issue29956()
    {
        // Set binding context
        BindingContext = _viewModel;

        // Create layout
        var verticalLayout = new VerticalStackLayout();

        var horizontalLayout = new HorizontalStackLayout();

        var aspectFit = new RadioButton
        {
            Content = "AspectFit",
            IsChecked = true,
            GroupName = "AspectGroup",
            AutomationId = "ImageAspectFit"
        };
        aspectFit.CheckedChanged += AspectRadio_CheckedChanged;

        var aspectFill = new RadioButton
        {
            Content = "AspectFill",
            GroupName = "AspectGroup",
            AutomationId = "ImageAspectFill"
        };
        aspectFill.CheckedChanged += AspectRadio_CheckedChanged;

        var fill = new RadioButton
        {
            Content = "Fill",
            GroupName = "AspectGroup",
            AutomationId = "ImageFill"
        };
        fill.CheckedChanged += AspectRadio_CheckedChanged;

        var center = new RadioButton
        {
            Content = "Center",
            GroupName = "AspectGroup",
            AutomationId = "ImageCenter"
        };
        center.CheckedChanged += AspectRadio_CheckedChanged;

        horizontalLayout.Children.Add(aspectFit);
        horizontalLayout.Children.Add(aspectFill);
        horizontalLayout.Children.Add(fill);
        horizontalLayout.Children.Add(center);

        _myImageButton = new ImageButton
        {
            Source = new Uri("https://aka.ms/campus.jpg"),
            BorderWidth = 20,
            BorderColor = Colors.Black,
            WidthRequest = 350,
            HeightRequest = 450
        };
        _myImageButton.SetBinding(ImageButton.AspectProperty, nameof(ImageViewModel.Aspect));

        verticalLayout.Children.Add(horizontalLayout);
        verticalLayout.Children.Add(_myImageButton);

        Content = verticalLayout;
    }

    private void AspectRadio_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is not RadioButton rb || !rb.IsChecked)
            return;

        switch (rb.Content?.ToString())
        {
            case "AspectFit":
                _viewModel.Aspect = Aspect.AspectFit;
                break;
            case "AspectFill":
                _viewModel.Aspect = Aspect.AspectFill;
                break;
            case "Fill":
                _viewModel.Aspect = Aspect.Fill;
                break;
            case "Center":
                _viewModel.Aspect = Aspect.Center;
                break;
        }
    }
}

public class ImageViewModel : INotifyPropertyChanged
{
    private Aspect _aspect = Aspect.AspectFit;
    public Aspect Aspect
    {
        get => _aspect;
        set
        {
            if (_aspect != value)
            {
                _aspect = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}



