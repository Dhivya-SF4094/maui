
using Maui.Controls.Sample;

namespace Controls.TestCases.HostApp.Issues;

[Issue(IssueTracker.Github, 32404_1, "Flow direction not working on Header/Footer in CollectionView [iOS]", PlatformAffected.iOS)]
public class HeaderFooterFlowDirection : ContentPage
{
    public HeaderFooterFlowDirection()
    {
        Grid grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star }
            }
        };

        CollectionView2 headerFooterView = new CollectionView2
        {
            EmptyView = new Label
            {
                Text = "No items to display"
            },
            Header = new Label
            {
                Text = "Header View",
            },
            Footer = new Label
            {
                Text = "Footer View"
            }
        };
        headerFooterView.FlowDirection = FlowDirection.LeftToRight;
        Grid.SetRow(headerFooterView, 1);
        CollectionView2 headerFooterStringView = new CollectionView2
        {
            EmptyView = "No items to display",
            Header = "Header String",
            Footer = "Footer String",
            Background = Colors.Gray
        };
        headerFooterStringView.FlowDirection = FlowDirection.LeftToRight;
        Grid.SetRow(headerFooterStringView, 2);

        CollectionView2 headerFooterTemplateView = new CollectionView2
        {
            EmptyView = "No items to display",
            HeaderTemplate = new DataTemplate(() =>
            {
                return new Label
                {
                    Text = "Header Template",
                };
            }),
            FooterTemplate = new DataTemplate(() =>
            {
                return new Label
                {
                    Text = "Footer Template",
                };
            }),
            Background = Colors.LightBlue
        };
        headerFooterTemplateView.FlowDirection = FlowDirection.LeftToRight;
        Grid.SetRow(headerFooterTemplateView, 3);

        Button button = new Button
        {
            Text = "Toggle FlowDirection",
            AutomationId = "ToggleFlowDirectionButton"
        };
        button.Clicked += (s, e) =>
        {
            if (headerFooterView.FlowDirection == FlowDirection.LeftToRight)
            {
                headerFooterView.FlowDirection = FlowDirection.RightToLeft;
                headerFooterStringView.FlowDirection = FlowDirection.RightToLeft;
                headerFooterTemplateView.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                headerFooterView.FlowDirection = FlowDirection.LeftToRight;
                headerFooterStringView.FlowDirection = FlowDirection.LeftToRight;
                headerFooterTemplateView.FlowDirection = FlowDirection.LeftToRight;
            }
        };
        Grid.SetRow(button, 0);
        grid.Children.Add(button);
        grid.Children.Add(headerFooterStringView);
        grid.Children.Add(headerFooterView);
        grid.Children.Add(headerFooterTemplateView);


        Content = grid;
    }
}
