using Microsoft.Maui.Controls.Core.UnitTests;
using Microsoft.Maui.Graphics;
using Xunit;

namespace Microsoft.Maui.Controls.Core.UnitTests.Layouts
{
    public class FlexLayoutPaddingMarginTests : BaseTestFixture
    {
        [Fact]
        public void FlexLayoutNestedPaddingTest()
        {
            // Create nested FlexLayouts like in the issue
            var label1 = new Label() { Text = "Layout1" };
            
            var layout1 = new FlexLayout()
            {
                BackgroundColor = Colors.Red,
                Children = { label1 },
                Margin = new Thickness(10),
                IsPlatformEnabled = true
            };

            var layout2 = new FlexLayout()
            {
                BackgroundColor = Colors.Blue,
                Children = { layout1 },
                Margin = new Thickness(10),
                IsPlatformEnabled = true
            };
            layout2.SetGrow(layout1, 1);

            var layout3 = new FlexLayout()
            {
                BackgroundColor = Colors.Orange,
                Children = { layout2 },
                Margin = new Thickness(10),
                IsPlatformEnabled = true
            };
            layout3.SetGrow(layout2, 1);

            var layout4 = new FlexLayout()
            {
                BackgroundColor = Colors.HotPink,
                Children = { layout3 },
                Margin = new Thickness(10),
                Padding = new Thickness(10),
                IsPlatformEnabled = true
            };
            layout4.SetGrow(layout3, 1);

            var rootLayout = new FlexLayout()
            {
                Children = { layout4 },
                Direction = FlexDirection.Column,
                BackgroundColor = Colors.LimeGreen,
                IsPlatformEnabled = true
            };
            rootLayout.SetGrow(layout4, 1);

            // Layout with size 400x300
            rootLayout.Layout(new Rect(0, 0, 400, 300));

            // Check if padding and margin are applied correctly
            // Layout4 should have padding on all sides
            var expectedLayout4ContentLeft = layout4.Bounds.Left + layout4.Padding.Left;
            var expectedLayout4ContentTop = layout4.Bounds.Top + layout4.Padding.Top;
            var expectedLayout4ContentRight = layout4.Bounds.Right - layout4.Padding.Right;
            var expectedLayout4ContentBottom = layout4.Bounds.Bottom - layout4.Padding.Bottom;
            
            // Layout3 should be positioned within layout4's padded area
            // and should have margin on all sides
            var expectedLayout3Left = expectedLayout4ContentLeft + layout3.Margin.Left;
            var expectedLayout3Top = expectedLayout4ContentTop + layout3.Margin.Top;
            var expectedLayout3Right = expectedLayout4ContentRight - layout3.Margin.Right;
            var expectedLayout3Bottom = expectedLayout4ContentBottom - layout3.Margin.Bottom;
            
            // Assert that layout3 is positioned correctly within layout4's padding
            Assert.True(layout3.Bounds.Left >= expectedLayout3Left, 
                $"Layout3 left should be at least {expectedLayout3Left} but was {layout3.Bounds.Left}");
            Assert.True(layout3.Bounds.Top >= expectedLayout3Top,
                $"Layout3 top should be at least {expectedLayout3Top} but was {layout3.Bounds.Top}");
            Assert.True(layout3.Bounds.Right <= expectedLayout3Right,
                $"Layout3 right should be at most {expectedLayout3Right} but was {layout3.Bounds.Right}");
            Assert.True(layout3.Bounds.Bottom <= expectedLayout3Bottom,
                $"Layout3 bottom should be at most {expectedLayout3Bottom} but was {layout3.Bounds.Bottom}");
        }
    }
}