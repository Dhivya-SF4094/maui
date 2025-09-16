using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue31330 : _IssuesUITest
{
    public Issue31330(TestDevice testDevice) : base(testDevice)
    {
    }
    public override string Issue => "Rectangle renders as thin line instead of filled shape for small height values";

    [Test]
    [Category(UITestCategories.Shape)]
    public void FixRectangleRenderingForSmallHeight()
    {
        App.WaitForElement("Issue31330Button");
        App.Tap("Issue31330Button");
        VerifyScreenshot();
    }
}
