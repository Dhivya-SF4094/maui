namespace Maui.Controls.Sample.Issues;

[Issue(IssueTracker.Github, 18102, "Padding or Margin on FlexLayout", PlatformAffected.All)]
public class Issue18102 : ContentPage
{
    public Issue18102()
    {
        var label1 = new Label()
        {
            Text = "Layout1",
            AutomationId = "Issue18102Label1"
        };

        FlexLayout flex1 = new FlexLayout()
        {
            BackgroundColor = Colors.Red,
            Children = { label1 },
            Margin = new Thickness(10)
        };

        FlexLayout flex2 = new FlexLayout()
        {
            BackgroundColor = Colors.Blue,
            Children = { flex1 },
            Margin = new Thickness(10)
        };
        flex2.SetGrow(flex1, 1);

        FlexLayout flex3 = new FlexLayout()
        {
            BackgroundColor = Colors.Orange,
            Children = { flex2 },
            Margin = new Thickness(10),

        };
        flex3.SetGrow(flex2, 1);

        FlexLayout flex4 = new FlexLayout()
        {
            BackgroundColor = Colors.HotPink,
            Children = { flex3 },
            Margin = new Thickness(10),
            Padding = new Thickness(10),
        };
        flex4.SetGrow(flex3, 1);

        FlexLayout rootLayout = new FlexLayout()
        {

            Children = { flex4 },
            BackgroundColor = Colors.LimeGreen
        };

        rootLayout.SetGrow(flex4, 1);
        Content = rootLayout;
    }
}