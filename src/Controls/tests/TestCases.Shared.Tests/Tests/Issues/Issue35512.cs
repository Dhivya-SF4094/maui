using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue35512 : _IssuesUITest
{
	public Issue35512(TestDevice device) : base(device) { }

	public override string Issue => "Button BackgroundColor does not restore to default when reset to null after dynamic update";

	[Test]
	[Category(UITestCategories.Button)]
	public void ButtonBackgroundColorResetsToDefaultAfterNullAssignment()
	{
		App.WaitForElement("TestButton");

		// Step 1: Take a screenshot of the button in its default state (baseline)
		VerifyScreenshot("DefaultState");

		// Step 2: Set the BackgroundColor to Red
		App.Tap("SetColorButton");
		App.WaitForElement("StatusLabel");

		// Step 3: Reset the BackgroundColor to null
		App.Tap("ResetColorButton");
		App.WaitForElement("StatusLabel");

		// Step 4: Verify button appearance matches default state
		// On iOS/macOS: button retains Red color (bug)
		// On Windows: button becomes Transparent (bug)
		// On Android: button correctly reverts to default (expected)
		VerifyScreenshot("DefaultState", retryTimeout: TimeSpan.FromSeconds(2));
	}
}
