using System;

namespace Controls.TestCases.HostApp.Issues;

[Issue(IssueTracker.Github, 5161, "ShellContent IsEnabledProperty does not work", PlatformAffected.iOS)]
public class Issue5161 : Shell
{
    public Issue5161()
    {
        var mainPageTab = new Tab
        {
            Title = "This is first Page",
            IsEnabled = true,
        };
        mainPageTab.Items.Add(new ShellContent
        {
            ContentTemplate = new DataTemplate(() => new Issue5161_MainPage())
        });

        var secondPageTab = new Tab
        {
            Title = "this is second Page",
            IsEnabled = false,
            AutomationId = "SecondPage"
        };
        secondPageTab.Items.Add(new ShellContent
        {
            ContentTemplate = new DataTemplate(() => new SecondPage())
        });
        var thirdTab = new Tab
        {
            Title = "This is Third Page",
            IsEnabled = true,
            AutomationId = "ThirdPage"
        };
        thirdTab.Items.Add(new ShellContent
        {
            ContentTemplate = new DataTemplate(() => new ThirdPage())
        });
        var tabBar = new TabBar();
        tabBar.Items.Add(mainPageTab);
        tabBar.Items.Add(secondPageTab);
        tabBar.Items.Add(thirdTab);
        Items.Add(tabBar);
    }

    public class Issue5161_MainPage : ContentPage
    {
        public Issue5161_MainPage()
        {
            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Children =
            {
                new Label
                {
                    Text = "First",
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand
                }
            }
            };
        }
    }

    public class SecondPage : ContentPage
    {
        public SecondPage()
        {
            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Children =
            {
                new Label
                {
                    Text = "Second",
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand
                }
            }
            };
        }
    }
    public class ThirdPage : ContentPage
    {
        public ThirdPage()
        {
            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Children =
            {
                new Label
                {
                    Text = "Third Page",
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand
                }
            }
            };
        }
    }
}
