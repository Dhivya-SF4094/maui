
using Microsoft.Maui.Platform;
using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue14448 : _IssuesUITest
{
    public Issue14448(TestDevice testDevice) : base(testDevice)
    {
    }
    public override string Issue => "maui title bar disappears and does not re-appear on iOS when using shell.searchhandler";

    [Test]
    [Category(UITestCategories.Shell)]
    public void ShellTitleShouldNotDisappear()
    {
        // First verify the title is visible
        App.WaitForElement("Home");

        // Try to locate the search field by placeholder text
        App.WaitForElement("Enter item name");

        // Tap on the search field to focus it
        App.Tap("Enter item name");

        // Enter text into the search field
        App.EnterText("Enter item name", "Item 1");

        // Verify the title is still visible after interacting with search
        App.WaitForElement("Home");

        // Take screenshot to verify UI state
        VerifyScreenshot();
    }
}
