using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue18102 : _IssuesUITest
{
    public Issue18102(TestDevice testDevice) : base(testDevice)
    {
    }
    public override string Issue => "Padding or Margin on FlexLayout";

    [Test]
    [Category(UITestCategories.Layout)]
    public void MarginAndPaddingShouldWorkOnNestedFlexLayout()
    {
        App.WaitForElement("Issue18102Label1");
        VerifyScreenshot();
    }
}