using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue17105 : _IssuesUITest
{
    public override string Issue => "Hide the password hint which is showing when the entry is focused";
    public Issue17105(TestDevice testDevice) : base(testDevice)
    {
    }

    [Test]
    [Category(UITestCategories.Entry)]
    public void HidePasswordHint()
    {
        App.WaitForElement("Entry");
        App.Tap("Entry");
        VerifyScreenshot();
    }

}
