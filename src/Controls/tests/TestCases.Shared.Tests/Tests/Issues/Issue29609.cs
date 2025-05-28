using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Tests.Issues;

public class Issue29609 : _IssuesUITest
{
    public Issue29609(TestDevice testDevice) : base(testDevice)
    {
    }
    public override string Issue => "ItemSpacing on CarouselView resizes items";

    [Test]
    [Category(UITestCategories.CarouselView)]
    public void VerifySpacingAffectsItemSize()
    {
        App.WaitForElement("ItemSpacing_CarouselView");
        VerifyScreenshot();

    }
}