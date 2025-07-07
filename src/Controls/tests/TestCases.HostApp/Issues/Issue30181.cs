using System.ComponentModel;

namespace Controls.TestCases.HostApp.Issues;

[Issue(IssueTracker.Github, 30181, "Entry bound to a double casts values too early, preventing small negative decimal entries", PlatformAffected.All)]
public class Issue30181 : ContentPage
{
    public Issue30181()
    {
        var viewModel = new MainPageViewModel();
        BindingContext = viewModel;

        var numericEntry = new Entry
        {
            Keyboard = Keyboard.Numeric,
            AutomationId = "Issue30181Entry"
        };
        numericEntry.SetBinding(Entry.TextProperty, nameof(viewModel.NumericValue));

        var stackLayout = new StackLayout
        {
            Padding = 20,
            Children = { numericEntry }
        };

        Content = stackLayout;
    }
}

public partial class MainPageViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    double? _numericValue;
    public double? NumericValue
    {
        get => _numericValue;
        set
        {
            if (_numericValue != value)
            {
                _numericValue = value;
                OnPropertyChanged(nameof(NumericValue));
            }
        }
    }

    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}