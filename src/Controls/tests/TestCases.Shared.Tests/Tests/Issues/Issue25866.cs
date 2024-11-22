using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue25866 : _IssuesUITest
{
    public override string Issue => "[iOS] Using safe area causes white area at the top upon entry focus";
    public Issue25866(TestDevice device) : base(device)
    {
    }

    [Test]
    [Category(UITestCategories.Entry)]
    public void WhiteAreaAtTop()
    {
        App.WaitForElement("Entry");
        App.Tap("Entry");
        VerifyScreenshot();
    }
}
