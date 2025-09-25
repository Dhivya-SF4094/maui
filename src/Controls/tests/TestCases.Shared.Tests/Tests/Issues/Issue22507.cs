using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue22507 : _IssuesUITest
{
    public Issue22507(TestDevice testDevice) : base(testDevice)
    {
    }
    public override string Issue => "CarouselView behaves strangely when swiping vertically in view";

    [Test]
    [Category(UITestCategories.CarouselView)]
    public void HandleCarouselVerticalScroll()
    {
        // Wait for the test page to load
        App.WaitForElement("Issue22507Label");

        App.ScrollDown("Issue22507CollectionView");
        App.WaitForElement("Item P");
        // Step 2: Swipe horizontally to navigate to the next CarouselView item (Page 2)
        App.SwipeRightToLeft();

        // Wait for Page 2 to load
        // Step 3: Scroll to the bottom of the CollectionView on page 2
        for (int i = 0; i < 2; i++)
        {
            App.ScrollDown("Issue22507CollectionView", ScrollStrategy.Gesture, 0.99, swipeSpeed: 900);
        }
        App.WaitForElement("Item 15");
    }
}
