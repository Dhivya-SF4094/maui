using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.TestCases.Tests.Issues;

public class Issue34803 : _IssuesUITest
{
	public override string Issue => "[iOS] Back Button long press navigation cannot be prevented by canceling the navigation";

	public Issue34803(TestDevice device) : base(device) { }

	[Test]
	[Category(UITestCategories.Shell)]
	public void BackNavigationShouldBeCancelledByShellOnNavigating()
	{
		// Navigate from Root → Page One → Page Two → Page Three via button chain
		App.WaitForElement("GoToPage1Button");
		App.Tap("GoToPage1Button");

		App.WaitForElement("GoToPage2Button");
		App.Tap("GoToPage2Button");

		App.WaitForElement("GoToPage3Button");
		App.Tap("GoToPage3Button");

		// Verify we're on Page Three
		App.WaitForElement("Page3Label");

		// Tap the iOS navigation bar back button.
		// Shell.OnNavigating overrides args.Cancel(), which should prevent the navigation.
		// Bug: on iOS, ShellSectionRenderer's DidPopItem fires before OnNavigating, so the
		// view controller is already popped and the cancellation has no effect.
		if (App is AppiumIOSApp iosApp && HelperExtensions.IsIOS26OrHigher(iosApp))
		{
			App.TapBackArrow();
		}
		else
		{
			App.TapBackArrow("Page Two");
		}

		// Navigation should have been cancelled: we must still be on Page Three.
		// If the bug is present, the VC was already popped and we're on Page Two instead.
		App.WaitForElement("Page3Label");
		Assert.That(App.FindElement("Page3Label").GetText(),
			Is.EqualTo("Page Three - back navigation prevented"),
			"Back navigation should have been prevented by Shell.OnNavigating calling args.Cancel().");
	}
}
