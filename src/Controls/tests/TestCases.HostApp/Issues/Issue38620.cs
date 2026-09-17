using System.Collections.ObjectModel;

namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 38620, "Material 3 AppBar lift target crashes under GC pressure", PlatformAffected.Android)]
public class Issue38620 : ContentPage
{
    static readonly TimeSpan StressDuration = TimeSpan.FromSeconds(35);
    readonly Random _random = new();
    readonly CarouselView _carouselView;
    readonly Label _iterationLabel;
    IDispatcherTimer _churnTimer;
    CancellationTokenSource _gcPressureCancellation;
    long _tick;

    public Issue38620()
    {
        Title = "AppBar lift target stress";

        _iterationLabel = new Label
        {
            AutomationId = "StressStatus",
            Text = "Stress running: 0 iterations",
            FontAttributes = FontAttributes.Bold
        };

        _carouselView = new CarouselView
        {
            AutomationId = "StressCarouselView",
            ItemsSource = CreateItems(),
            Loop = false,
            PeekAreaInsets = 0,
            ItemTemplate = new DataTemplate(() =>
            {
                var label = new Label { FontSize = 28 };
                label.SetBinding(Label.TextProperty, ".");

                return new ScrollView
                {
                    Content = new VerticalStackLayout
                    {
                        Padding = 20,
                        Spacing = 10,
                        Children =
                        {
                            label,
                            new BoxView { HeightRequest = 1400, Color = Colors.CornflowerBlue }
                        }
                    }
                };
            })
        };

        var stopButton = new Button
        {
            AutomationId = "StopStressButton",
            Text = "Stop stress loop",
            Command = new Command(StopStress)
        };

        var grid = new Grid
        {
            Padding = new Thickness(16, 8),
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star)
            }
        };

        grid.Add(new VerticalStackLayout
        {
            Spacing = 6,
            Children = { _iterationLabel, stopButton }
        }, 0, 0);
        grid.Add(_carouselView, 0, 1);

        Content = grid;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartStress();
    }

    protected override void OnDisappearing()
    {
        StopStress();
        base.OnDisappearing();
    }

    void StartStress()
    {
        if (_churnTimer is not null)
            return;

        _gcPressureCancellation = new CancellationTokenSource();
        var cancellationToken = _gcPressureCancellation.Token;

        _ = Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                try
                {
                    await Task.Delay(15, cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }, cancellationToken);

        _ = CompleteStressAfterDelayAsync(cancellationToken);

        _churnTimer = Dispatcher.CreateTimer();
        _churnTimer.Interval = TimeSpan.FromMilliseconds(20);
        _churnTimer.Tick += OnChurnTick;
        _churnTimer.Start();
    }

    void OnChurnTick(object sender, EventArgs e)
    {
        _tick++;
        _iterationLabel.Text = $"Stress running: {_tick} iterations";

        if (_carouselView.ItemsSource is IReadOnlyCollection<int> items && items.Count > 0)
            _carouselView.Position = _random.Next(items.Count);

        if (_tick % 25 == 0)
            _carouselView.ItemsSource = CreateItems();
    }

    async Task CompleteStressAfterDelayAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(StressDuration, cancellationToken).ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        Dispatcher.Dispatch(CompleteStress);
    }

    void CompleteStress()
    {
        StopStress();
        _iterationLabel.Text = "Stress completed";
    }

    void StopStress()
    {
        if (_churnTimer is not null)
        {
            _churnTimer.Stop();
            _churnTimer.Tick -= OnChurnTick;
            _churnTimer = null;
        }

        _gcPressureCancellation?.Cancel();
        _gcPressureCancellation?.Dispose();
        _gcPressureCancellation = null;
    }

    ObservableCollection<int> CreateItems()
    {
        return new ObservableCollection<int>(
            Enumerable.Range(0, 40).Select(_ => _random.Next()));
    }
}
